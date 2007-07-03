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
		TextTag [] tags;
		DocumentTag docTag;
		TextTag paddingVisible = buffer.TagTable.Lookup ("padding-visible");
		TextTag paddingInvisible = buffer.TagTable.Lookup ("padding-invisible");
		nextIter.ForwardChar ();
		
		while (!currentIter.Equal (end)) {
			tags = currentIter.Tags;
			
			#if DEBUG
			Console.WriteLine ("Offset: {0} Char: {1}", currentIter.Offset, currentIter.Char);
			#endif
			
			if (readingValue) {
				attributeValue += currentIter.Char;
			}
			
			if (readingText && !currentIter.HasTag (paddingVisible) &&  !currentIter.HasTag (paddingInvisible)) {
				elementText += currentIter.Char;
			}
			
			foreach (TextTag tag in tags) {
				#if DEBUG
				Console.WriteLine ("Posible Begin Tags: {0} Begins: {1} Ends: {2}", tag.Name, currentIter.BeginsTag (tag) ? "True" : "False", TagEndsHere (tag, currentIter, nextIter)? "True" : "False");
				#endif
				
				if (currentIter.BeginsTag (tag)) {
					docTag = tag as DocumentTag;
					
					if (docTag.IsElement) {
						string elementName = docTag.Name.Split ('#')[0];
						xmlWriter.WriteStartElement (null, elementName, null);
						
						#if DEBUG
						Console.WriteLine ("Wrote Start Element: " + elementName);
						#endif
					} else if (docTag.IsAttribute) {
						attributeName = docTag.Name.Split (':')[1].Split ('#')[0];
						xmlWriter.WriteStartAttribute (null, attributeName, null);
						
						#if DEBUG
						Console.WriteLine ("Wrote Start Attribute: {0}", attributeName);
						#endif
						
						readingValue = true;
						attributeValue = currentIter.Char;
					} else if (docTag.IsSerializableText) {
						readingText = true;
						elementText = currentIter.Char;
					}
				}
			}
			
			Array.Reverse (tags);
			foreach (TextTag tag in tags) {
				#if DEBUG
				Console.WriteLine ("Posible End Tags: {0} Begins: {1} Ends: {2}", tag.Name, currentIter.BeginsTag (tag) ? "True" : "False", TagEndsHere (tag, currentIter, nextIter)? "True" : "False");
				#endif
				
				if (TagEndsHere (tag, currentIter, nextIter)) {
					docTag = tag as DocumentTag;
					if (docTag.IsElement) {
						xmlWriter.WriteEndElement ();
						
						#if DEBUG
						Console.WriteLine ("Wrote End Element: " + docTag.Name);
						#endif
					} else if (docTag.IsAttribute) {
						xmlWriter.WriteString (attributeValue);
						xmlWriter.WriteEndAttribute ();
						
						#if DEBUG
						Console.WriteLine ("Wrote End Attribute: {0} Value: {1}", attributeName, attributeValue);
						#endif
						
						readingValue = false;
						attributeValue = attributeName = String.Empty;
					} else if (docTag.IsSerializableText) {
						xmlWriter.WriteString (elementText);
						elementText = String.Empty;
						readingText = false;
					}
				}
			}
			
			currentIter.ForwardChar ();
			nextIter.ForwardChar ();
			
			#if DEBUG
			Console.WriteLine ("State: {0} Char: {1} \n", xmlWriter.WriteState.ToString (), currentIter.Char);
			#endif
			
			while (Application.EventsPending ())
				Application.RunIteration ();
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
		int depth = 0;
		bool emptyElement, isDynamic;
		string dSuffix;
		Stack stack = new Stack ();
		TagStart tagStart;
		TextIter insertAt, applyStart, applyEnd;
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextTag tag;
		int count = 1;
		
		while (xmlReader.Read ()) {
			switch (xmlReader.NodeType) {
				case XmlNodeType.Element:
					emptyElement = xmlReader.IsEmptyElement;
					isDynamic = tagTable.IsDynamic (xmlReader.Name);
					dSuffix = String.Empty;
					tagStart = new TagStart ();
					tagStart.Start = offset;
					depth++;
					
					// Check first if element is dynamic, if true try to lookup a previous creation, if not we create it
					// If false we lookup in the table.
					if (isDynamic) {
						dSuffix = '#' + depth.ToString ();
					}
					
					tagStart.Tag = tagTable.Lookup (xmlReader.Name + dSuffix);
					if (isDynamic && tagStart.Tag == null)
						tagStart.Tag = tagTable.CreateDynamicTag (xmlReader.Name + dSuffix);
					else if (isDynamic && tagStart.Tag != null &&  tagStart.Tag.Priority < ((TagStart) stack.Peek ()).Tag.Priority) {
						tagStart.Tag = tagTable.CreateDynamicTag (xmlReader.Name + dSuffix + "." + count);
						dSuffix += "." + count;
						count++;
					}
					
					#if DEBUG
					try {
						Console.WriteLine ("Element: {0} Start: {1}", tagStart.Tag.Name, tagStart.Start);
					} catch (NullReferenceException) {
						Console.WriteLine ("Error: Missing {0} element", xmlReader.Name);
						Environment.Exit (1);
					}
					#endif
					
					if (xmlReader.HasAttributes)
						offset = DeserializeAttributes (buffer, offset, xmlReader, dSuffix);
					
					if (emptyElement) {
						if (tagStart.Start != offset) {
							applyStart = buffer.GetIterAtOffset (tagStart.Start);
							applyEnd = buffer.GetIterAtOffset (offset);
						} else {
							// Padding to conserve empty element
							insertAt = buffer.GetIterAtOffset (offset);
							buffer.InsertWithTagsByName (ref insertAt, " ", "padding-invisible");
							offset += 1;
							
							applyStart = buffer.GetIterAtOffset (tagStart.Start);
							applyEnd = buffer.GetIterAtOffset (offset);
						}
						
						buffer.ApplyTag (tagStart.Tag, applyStart, applyEnd);
						
						// Padding between tag regions
						insertAt = buffer.GetIterAtOffset (offset);
						buffer.InsertWithTagsByName (ref insertAt, " ", "padding-invisible");
						offset += 1;
						depth--;
						
						#if DEBUG
						Console.WriteLine ("Empty Element: {0}, Start: {1}, End: {2}", tagStart.Tag.Name, tagStart.Start, offset);
						#endif
						break;
					} else
						stack.Push (tagStart);
					break;
				case XmlNodeType.Text:
					string tagName;
					tagStart = stack.Peek () as TagStart;
					
					DocumentTag docTag = (DocumentTag) tagStart.Tag;
					
					// Check first if element text is dynamic, if true try to lookup a previous creation, if not we create it
					// If false we lookup in the table.
					if (docTag.IsDynamic)
						tagName = docTag.Name.Split ('#')[0] + ":Text" + '#' + docTag.Name.Split ('#')[1];
					else
						tagName = docTag.Name + ":Text";
					
					tag = tagTable.Lookup (tagName);
					if (tag == null)
						tag = tagTable.CreateDynamicTag (tagName);
					
					#if DEBUG
					Console.WriteLine ("Text: {0} Value: {1} Start: {2}", tagName, xmlReader.Value, offset);
					#endif
					
					insertAt = buffer.GetIterAtOffset (offset);
					buffer.InsertWithTags (ref insertAt, xmlReader.Value, tag);
					
					offset += xmlReader.Value.Length;
					break;
				case XmlNodeType.EndElement:
					tagStart = stack.Pop () as TagStart;
					
					#if DEBUG
					Console.WriteLine ("Element: {0}, End: {1}", tagStart.Tag.Name, offset);
					#endif
					
					if (tagStart.Start != offset) {
						applyStart = buffer.GetIterAtOffset (tagStart.Start);
						applyEnd = buffer.GetIterAtOffset (offset);
					} else {
						// Padding to conserve empty element
						insertAt = buffer.GetIterAtOffset (offset);
						buffer.InsertWithTagsByName (ref insertAt, " ", "padding-invisible");
						offset += 1;
						
						applyStart = buffer.GetIterAtOffset (tagStart.Start);
						applyEnd = buffer.GetIterAtOffset (offset); 
					}
					
					buffer.ApplyTag (tagStart.Tag, applyStart, applyEnd);
					
					#if DEBUG
					Console.WriteLine ("Applied: {0}, Start: {1}, End: {2}", tagStart.Tag.Name, tagStart.Start, offset);
					#endif
					
					// Padding between tag regions
					insertAt = buffer.GetIterAtOffset (offset);
					buffer.InsertWithTagsByName (ref insertAt, " ", "padding-invisible");
					offset += 1;
					depth--;
					break;
				case XmlNodeType.Whitespace:
					break;
				default:
					Console.WriteLine ("Unhandled Element {0}. Value: '{1}'",
						    xmlReader.NodeType,
						    xmlReader.Value);
					break;
			}
			
			while (Application.EventsPending ())
				Application.RunIteration ();
		}
	}
	
	private static int DeserializeAttributes (TextBuffer buffer, int offset, XmlTextReader xmlReader, string tagSuffix)
	{
		int result = offset;
		string tagName = xmlReader.Name;
		TextIter applyStart, applyEnd;
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextTag tagAttributes;
		
		switch (tagName) {
//			case "Type":
//			case "TypeSignature":
//			case "Member":
//			case "MemberSignature":
//				result = DeserializeAttributesNewline (buffer, offset, xmlReader);
//				break;
//			case "link":
//			case "see":
//				result = DeserializeAttributesSpace (buffer, offset, xmlReader);
//				break;
//			case "since":
//				result = DeserializeAttributesNone (buffer, offset, xmlReader);
//				break;
			default:
				result = DeserializeAttributesNone (buffer, offset, xmlReader, tagSuffix);
				break;
		}
		
		tagName += ":Attributes" + tagSuffix;
		tagAttributes = tagTable.Lookup (tagName);
		
		if (!tagSuffix.Equals (String.Empty) && tagAttributes == null)
			tagAttributes = tagTable.CreateDynamicTag (tagName);
		
		applyStart = buffer.GetIterAtOffset (offset);
		applyEnd = buffer.GetIterAtOffset (result);
		buffer.ApplyTag (tagAttributes, applyStart, applyEnd);
		
		#if DEBUG
		Console.WriteLine ("Attributes: {0} Start: {1} End: {2}", tagName, offset, result);
		#endif
		
		return result;
	}
	
//	private static int DeserializeAttributesNewline (TextBuffer buffer, int offset, XmlTextReader xmlReader)
//	{
//		string tagPrefix = xmlReader.Name + ":";
//		
//		TextIter insertAt = buffer.GetIterAtOffset (offset);
//		while (xmlReader.MoveToNextAttribute ()) {
//			string tagName = tagPrefix + xmlReader.Name;
//			buffer.InsertWithTagsByName (ref insertAt, xmlReader.Value, tagName);
//			buffer.InsertWithTagsByName (ref insertAt, "\n", "padding-visible");
//		}
//		
//		#if DEBUG
//		Console.WriteLine ("Attribute: {0} Start: {1} End: {2}", tagPrefix + xmlReader.Name, offset, insertAt.Offset);
//		#endif
//		
//		return insertAt.Offset;
//	}
//	
//	private static int DeserializeAttributesSpace (TextBuffer buffer, int offset, XmlTextReader xmlReader)
//	{
//		string tagPrefix = xmlReader.Name + ":";
//		
//		TextIter insertAt = buffer.GetIterAtOffset (offset);
//		while (xmlReader.MoveToNextAttribute ()) {
//			string tagName = tagPrefix + xmlReader.Name;
//			buffer.InsertWithTagsByName (ref insertAt, xmlReader.Value, tagName);
//			buffer.InsertWithTagsByName (ref insertAt, " ", "padding-visible");
//		}
//		
//		#if DEBUG
//		Console.WriteLine ("Attribute: {0} Start: {1} End: {2}", tagPrefix + xmlReader.Name, offset, insertAt.Offset);
//		#endif
//		
//		return insertAt.Offset;
//	}
	
	private static int DeserializeAttributesNone (TextBuffer buffer, int offset, XmlTextReader xmlReader, string tagSuffix)
	{
		string tagName = String.Empty;
		string tagPrefix = xmlReader.Name + ":";
		
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		while (xmlReader.MoveToNextAttribute ()) {
			tagName = tagPrefix + xmlReader.Name + tagSuffix;
			
			TextTag tag = tagTable.Lookup (tagName);
			if (tag == null)
				tag = tagTable.CreateDynamicTag (tagName);
			
			buffer.InsertWithTags (ref insertAt, xmlReader.Value, tag);
		}
		
		#if DEBUG
		Console.WriteLine ("Attribute: {0} Start: {1} End: {2}", tagName, offset, insertAt.Offset);
		#endif
		
		return insertAt.Offset;
	}
	
	private static bool TagEndsHere (TextTag tag, TextIter currentIter, TextIter nextIter)
	{
		return (currentIter.HasTag (tag) && !nextIter.HasTag (tag));
	}
}
}