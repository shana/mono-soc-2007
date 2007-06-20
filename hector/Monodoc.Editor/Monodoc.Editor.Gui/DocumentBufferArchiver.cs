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
		return (currentIter.HasTag (tag) && !nextIter.HasTag (tag)) || nextIter.IsEnd;
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
		
		nextIter.ForwardChar ();
		
		TextIter temp = start;
		temp.Offset = 522;
		if (temp.EndsTag (buffer.TagTable.Lookup ("Members")))
			Console.WriteLine ("True");
		
		while (!currentIter.Equal (end)) {
			Console.WriteLine ("Offset: {0}", currentIter.Offset);
			foreach (TextTag tag in currentIter.Tags) {
				Console.WriteLine ("Tag: {0}, Begins: {1}, Ends: {2}", tag.Name, currentIter.BeginsTag (tag) ? "True" : "False", TagEndsHere (tag, currentIter, nextIter)? "True" : "False"); 
				if (currentIter.BeginsTag (tag)) {
					if (tag.Name.EndsWith (":Attributes")) {
					} else if (tag.Name.IndexOf (":") == -1) {
						Console.WriteLine ("Start: " + currentIter.Offset);
						xmlWriter.WriteStartElement (null, tag.Name, null);
					}
				} else if (TagEndsHere (tag, currentIter, nextIter)) {
					if (tag.Name.IndexOf (":") == -1) {
						Console.WriteLine ("End: " + currentIter.Offset);
						xmlWriter.WriteEndElement ();
					}
				}
			}
			
			currentIter.ForwardChar ();
			nextIter.ForwardChar ();
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
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
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
					buffer.Insert (ref insertAt, " ");
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
		Console.WriteLine ("Attribute: {0} Start: {1} End: {2}", tagName, offset, result);
		#endif
		
		return result;
	}
	
	private static int DeserializeTypeAttributes (TextBuffer buffer, int offset, XmlTextReader xmlReader)
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
			buffer.Insert (ref insertAt, " ");
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
			buffer.Insert (ref insertAt, " ");
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
		
		return insertAt.Offset;
	}
}
}