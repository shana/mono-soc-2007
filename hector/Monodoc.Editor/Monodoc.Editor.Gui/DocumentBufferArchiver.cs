//
// DocumentBufferArchiver.cs: Class that handles the serialization and deserialization of the buffer.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using Gtk;
using System.IO;
using System.Xml;
using System.Collections;

namespace Monodoc.Editor.Gui {
public class DocumentBufferArchiver {
	private class TagStart 
	{
		public int Start;
		public TextTag Tag;
	}
	
	static bool TagEndsHere (TextTag tag, TextIter currentIter, TextIter nextIter)
	{
		return (currentIter.HasTag (tag) && !nextIter.HasTag (tag));
	}
	
	public static string Serialize (TextBuffer buffer)
	{
		return Serialize (buffer, buffer.StartIter, buffer.EndIter);
	}

	public static string Serialize (TextBuffer buffer, TextIter start, TextIter end)
	{
		StringWriter stream = new StringWriter ();
		XmlTextWriter xmlWriter = new XmlTextWriter (stream);
		xmlWriter.Formatting = Formatting.Indented;
		
		Serialize (buffer, start, end, xmlWriter);
		
		xmlWriter.Close ();
		return stream.ToString ();
	}
	
	public static void Serialize (TextBuffer buffer, TextIter start, TextIter end, XmlTextWriter xmlWriter)
	{
		TextIter currentIter = start;
		TextIter nextIter = start;
		bool readingValue = false;
		bool readingText = false;
		string attributeName = "";
		string attributeValue = "";
		string elementText = "";
		System.Object [] tags;
		nextIter.ForwardChar ();
		Stack stack = new Stack ();
		TextTag ignore = buffer.TagTable.Lookup ("ignore");
		
		while (!currentIter.Equal (end)) {
			
			#if DEBUG
			Console.WriteLine ("Offset: {0} Char: {1}", currentIter.Offset, currentIter.Char);
			#endif
			
			if (readingValue) {
				attributeValue += currentIter.Char;
			}
			
			if (readingText && !currentIter.HasTag (ignore)) {
				elementText += currentIter.Char;
			}
			
			foreach (TextTag tag in currentIter.Tags) {
				#if DEBUG
				Console.WriteLine ("Posible Begin Tags: {0} Begins: {1} Ends: {2}", tag.Name, currentIter.BeginsTag (tag) ? "True" : "False", TagEndsHere (tag, currentIter, nextIter)? "True" : "False");
				#endif
				
				if (currentIter.BeginsTag (tag)) {
					if (tag.Name.EndsWith (":Attributes") || tag.Name.Equals ("ignore")) {
					} else if (tag.Name.IndexOf (":") == -1) {
						bool textNode = HasTextNode (tag.Name);
						
						if (readingText) {
							int length = elementText.Length;
							xmlWriter.WriteString (elementText.Substring (0, length - 1));
							readingText = false;
						}
						
						if (textNode) {
							stack.Push (tag);
							readingText = true;
							elementText = currentIter.Char;
						}
						
						xmlWriter.WriteStartElement (null, tag.Name, null);
						
						#if DEBUG
						Console.WriteLine ("Wrote Start Element: " + tag.Name);
						#endif
					} else if (tag.Name.IndexOf (":") > 0) {
						attributeName = tag.Name.Split (':')[1];
						xmlWriter.WriteStartAttribute (null, attributeName, null);
						
						#if DEBUG
						Console.WriteLine ("Wrote Start Attribute: {0}", attributeName);
						#endif
						
						readingValue = true;
						attributeValue += currentIter.Char;
					}
				}
			}
			
			tags = Reverse (currentIter.Tags);
			foreach (TextTag tag in tags) {
				#if DEBUG
				Console.WriteLine ("Posible End Tags: {0} Begins: {1} Ends: {2}", tag.Name, currentIter.BeginsTag (tag) ? "True" : "False", TagEndsHere (tag, currentIter, nextIter)? "True" : "False");
				#endif
				
				if (TagEndsHere (tag, currentIter, nextIter)) {
					if (tag.Name.EndsWith (":Attributes") || tag.Name.Equals ("ignore")) {
					} else if (tag.Name.IndexOf (":") == -1) {
						string startName = "";
						TextTag startTag;
						
						if (stack.Count != 0 && ((TextTag) stack.Peek ()).Name.Equals (tag.Name)) {
							startTag = stack.Pop () as TextTag;
							startName = startTag.Name;
						}
						
						if (readingText && startName.Equals (tag.Name)) {
							xmlWriter.WriteString (elementText);
							elementText = String.Empty;
							
							if (stack.Count != 0)
								readingText = true;
							else
								readingText = false;
						} else if (!readingText && !startName.Equals (tag.Name) && stack.Count != 0) {
							elementText = String.Empty;
							readingText = true;
						}
						
						xmlWriter.WriteEndElement ();
						elementText = String.Empty;
						
						#if DEBUG
						Console.WriteLine ("Wrote End Element: " + tag.Name);
						#endif
						
					} else if (tag.Name.IndexOf (":") > 0) {
						xmlWriter.WriteString (attributeValue);
						xmlWriter.WriteEndAttribute ();
						
						#if DEBUG
						Console.WriteLine ("Wrote End Attribute: {0} Value: {1}", attributeName, attributeValue);
						#endif
						
						readingValue = false;
						attributeValue = attributeName = elementText =  String.Empty;
					}
				}
			}
			
			currentIter.ForwardChar ();
			nextIter.ForwardChar ();
			
			#if DEBUG
			Console.WriteLine ("State: {0} Char: {1} \n", xmlWriter.WriteState.ToString (), currentIter.Char);
			#endif
		}
	}
	
	public static void Deserialize (TextBuffer buffer, string content)
	{
		Deserialize (buffer, buffer.StartIter, content);
	}
	
	public static void Deserialize (TextBuffer buffer, TextIter start, string content)
	{
		StringReader stringReader = new StringReader (content);
		XmlTextReader xmlReader = new XmlTextReader (stringReader);
		xmlReader.Namespaces = false;
		Deserialize (buffer, buffer.StartIter, xmlReader);
	}
	
	public static void Deserialize (TextBuffer buffer, TextIter start,  XmlTextReader xmlReader)
	{
		int offset = start.Offset;
		bool emptyElement;
		Stack stack = new Stack ();
		TagStart tagStart;
		TextIter insertAt, applyStart, applyEnd;
		
		
		while (xmlReader.Read ()) {
			switch (xmlReader.NodeType) {
				case XmlNodeType.Element:
					emptyElement = xmlReader.IsEmptyElement;
					tagStart = new TagStart ();
					tagStart.Start = offset;
					tagStart.Tag = buffer.TagTable.Lookup (xmlReader.Name);
					
					#if DEBUG
					Console.WriteLine ("Element: {0} Start: {1}", tagStart.Tag.Name, tagStart.Start);
					#endif
					
					if (xmlReader.HasAttributes)
						offset = DeserializeAttributes (buffer, offset, xmlReader);
					
					if (emptyElement) {
						applyStart = buffer.GetIterAtOffset (tagStart.Start);
						applyEnd = buffer.GetIterAtOffset (offset);
						buffer.ApplyTag (tagStart.Tag, applyStart, applyEnd);
						
						#if DEBUG
						Console.WriteLine ("Empty Element: {0}, Start: {1}, End: {2}", tagStart.Tag.Name, tagStart.Start, offset);
						#endif
						break;
					} else
						stack.Push (tagStart);
					break;
				case XmlNodeType.Text:
					#if DEBUG
					Console.WriteLine ("Text: {0} Start: {1}", xmlReader.Value, offset);
					#endif
					
					insertAt = buffer.GetIterAtOffset (offset);
					buffer.Insert (ref insertAt, xmlReader.Value);
					
					offset += xmlReader.Value.Length;
					break;
				case XmlNodeType.EndElement:
					int realOffset = offset;
					tagStart = stack.Pop () as TagStart;
					
					#if DEBUG
					Console.WriteLine ("Element: {0}, End: {1}", tagStart.Tag.Name, offset);
					#endif
					
					// Padding between tag regions
					insertAt = buffer.GetIterAtOffset (offset);
					buffer.InsertWithTagsByName (ref insertAt, " ", "ignore");
					offset += 1;
					
					applyStart = buffer.GetIterAtOffset (tagStart.Start);
					applyEnd = buffer.GetIterAtOffset (realOffset);
					buffer.ApplyTag (tagStart.Tag, applyStart, applyEnd);
					
					#if DEBUG
					Console.WriteLine ("Applied: {0}, Start: {1}, End: {2}", tagStart.Tag.Name, tagStart.Start, realOffset);
					#endif
					break;
				default:
					Console.WriteLine ("Unhandled Element {0}. Value: '{1}'",
						    xmlReader.NodeType,
						    xmlReader.Value);
					break;
			}
		}
	}
	
	private static int DeserializeAttributes (TextBuffer buffer, int offset, XmlTextReader xmlReader)
	{
		int result = offset;
		string tagName = xmlReader.Name;
		TextIter applyStart, applyEnd;
		
		switch (tagName) {
			case "Type":
				result = DeserializeTypeAttributes (buffer, offset, xmlReader);
				break;
			case "TypeSignature":
				result = DeserializeTypeSignatureAttributes (buffer, offset, xmlReader);
				break;
			case "Member":
				result = DeserializeMemberAttributes (buffer, offset, xmlReader);
				break;
			case "MemberSignature":
				result = DeserializeMemberSignatureAttributes (buffer, offset, xmlReader);
				break;
			case "link":
				result = DeserializeLinkAttributes (buffer, offset, xmlReader);
				break;
			case "see":
				result = DeserializeSeeAttributes (buffer, offset, xmlReader);
				break;
			case "since":
				result = DeserializeSinceAttributes (buffer, offset, xmlReader);
				break;
			default:
				break;
		}
		
		TextTag tagAttributes = buffer.TagTable.Lookup (tagName + ":Attributes");
		applyStart = buffer.GetIterAtOffset (offset);
		applyEnd = buffer.GetIterAtOffset (result);
		buffer.ApplyTag (tagAttributes, applyStart, applyEnd);
		
		#if DEBUG
		Console.WriteLine ("Attributes: {0} Start: {1} End: {2}", tagName, offset, result);
		#endif
		
		return result;
	}
	
	private static int DeserializeTypeAttributes (TextBuffer buffer, int offset, XmlTextReader xmlReader)
	{
		string tagPrefix = xmlReader.Name + ":";
		int resultOffset = offset;
		
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		while (xmlReader.MoveToNextAttribute ()) {
			string tagName = tagPrefix + xmlReader.Name;
			buffer.InsertWithTagsByName (ref insertAt, xmlReader.Value, tagName);
			buffer.Insert (ref insertAt, "\n");
			resultOffset += xmlReader.Value.Length + 1;
		}
		
		return resultOffset;
	}
	
	private static int DeserializeTypeSignatureAttributes (TextBuffer buffer, int offset, XmlTextReader xmlReader)
	{
		string tagPrefix = xmlReader.Name + ":";
		
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		while (xmlReader.MoveToNextAttribute ()) {
			string tagName = tagPrefix + xmlReader.Name;
			buffer.InsertWithTagsByName (ref insertAt, xmlReader.Value, tagName);
			buffer.Insert (ref insertAt, "\n");
		}
		
		return insertAt.Offset;
	}
	
	private static int DeserializeMemberAttributes (TextBuffer buffer, int offset, XmlTextReader xmlReader)
	{
		string tagPrefix = xmlReader.Name + ":";
		
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		while (xmlReader.MoveToNextAttribute ()) {
			string tagName = tagPrefix + xmlReader.Name;
			buffer.InsertWithTagsByName (ref insertAt, xmlReader.Value, tagName);
			buffer.Insert (ref insertAt, "\n");
		}
		
		return insertAt.Offset;
	}
	
	private static int DeserializeMemberSignatureAttributes (TextBuffer buffer, int offset, XmlTextReader xmlReader)
	{
		string tagPrefix = xmlReader.Name + ":";
		
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		while (xmlReader.MoveToNextAttribute ()) {
			string tagName = tagPrefix + xmlReader.Name;
			buffer.InsertWithTagsByName (ref insertAt, xmlReader.Value, tagName);
			buffer.Insert (ref insertAt, "\n");
		}
		
		return insertAt.Offset;
	}
	
	private static int DeserializeLinkAttributes (TextBuffer buffer, int offset, XmlTextReader xmlReader)
	{
		string tagPrefix = xmlReader.Name + ":";
		
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		while (xmlReader.MoveToNextAttribute ()) {
			string tagName = tagPrefix + xmlReader.Name;
			buffer.InsertWithTagsByName (ref insertAt, xmlReader.Value, tagName);
			buffer.InsertWithTagsByName (ref insertAt, " ", "ignore");
		}
		
		return insertAt.Offset;
	}
	
	private static int DeserializeSeeAttributes (TextBuffer buffer, int offset, XmlTextReader xmlReader)
	{
		string tagPrefix = xmlReader.Name + ":";
		
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		while (xmlReader.MoveToNextAttribute ()) {
			string tagName = tagPrefix + xmlReader.Name;
			buffer.InsertWithTagsByName (ref insertAt, xmlReader.Value, tagName);
			buffer.InsertWithTagsByName (ref insertAt, " ", "ignore");
		}
		
		return insertAt.Offset;
	}
	
	private static int DeserializeSinceAttributes (TextBuffer buffer, int offset, XmlTextReader xmlReader)
	{
		string tagPrefix = xmlReader.Name + ":";
		
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		while (xmlReader.MoveToNextAttribute ()) {
			string tagName = tagPrefix + xmlReader.Name;
			buffer.InsertWithTagsByName (ref insertAt, xmlReader.Value, tagName);
		}
		
		#if DEBUG
		Console.WriteLine ("Attribute: {0} Start: {1} End: {2}", tagPrefix + xmlReader.Name, offset, insertAt.Offset);
		#endif
		
		return insertAt.Offset;
	}
	
	private static System.Object [] Reverse (TextTag [] tags)
	{
		ArrayList list = new ArrayList ();
		foreach (TextTag tag in tags) {
			list.Insert (0, tag);
		}
		
		return list.ToArray ();
	}
	
	private static bool HasTextNode (string name) 
	{
		bool result = false;
		switch (name) {
			case "AssemblyName":
				result = true;
				break;
			case "AssemblyVersion":
				result = true;
				break;
			case "ThreadSafetyStatement":
				result = true;
				break;
			case "BaseTypeName":
				result = true;
				break;
			case "AttributeName":
				result = true;
				break;
			case "MemberType":
				result = true;
				break;
			case "ReturnType":
				result = true;
				break;
			case "summary":
				result = true;
				break;
			case "para":
				result = true;
				break;
			case "link":
				result = true;
				break;
		}
		
		return result;
	}
}
}