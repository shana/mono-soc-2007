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
	class TagStart 
	{
		public int Start;
		public TextTag Tag;
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
		Stack stack = new Stack ();
		TagStart tagStart;
		TextIter insert_at;
		
		
		while (xmlReader.Read ()) {
			switch (xmlReader.NodeType) {
				case XmlNodeType.Element:
					bool empty = xmlReader.IsEmptyElement;
					tagStart = new TagStart ();
					tagStart.Start = offset;
					tagStart.Tag = buffer.TagTable.Lookup (xmlReader.Name);
					
					Console.WriteLine ("Element: {0} Start: {1}", tagStart.Tag.Name, tagStart.Start);
					
					if (xmlReader.HasAttributes) {
						offset = DeserializeAttributes (buffer, offset, xmlReader);
					}
					
					if (empty) {
						TextIter applyStart, applyEnd;
						applyStart = buffer.GetIterAtOffset (tagStart.Start);
						applyEnd = buffer.GetIterAtOffset (offset);
					
						Console.WriteLine ("TagName: {0}, Start: {1}, End: {2}", xmlReader.Name, tagStart.Start, offset);
					
						buffer.ApplyTag (tagStart.Tag, applyStart, applyEnd);
						break;
					} else
						stack.Push (tagStart);
					break;
				case XmlNodeType.Text:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					Console.WriteLine ("Text: " + xmlReader.Value);
					
					insert_at = buffer.GetIterAtOffset (offset);
					buffer.Insert (ref insert_at, xmlReader.Value);
					
					offset += xmlReader.Value.Length;
					Console.WriteLine ("Offset: " + offset);
					break;
				case XmlNodeType.EndElement:
					tagStart = (TagStart) stack.Pop ();
					
					Console.WriteLine ("Element: {0}, End: {1}", tagStart.Tag.Name, offset);
					
					TextIter applyStart, applyEnd;
					applyStart = buffer.GetIterAtOffset (tagStart.Start);
					applyEnd = buffer.GetIterAtOffset (offset);
					
					Console.WriteLine ("TagName: {0}, Start: {1}, End: {2}", xmlReader.Name, tagStart.Start, offset);
					
					buffer.ApplyTag (tagStart.Tag, applyStart, applyEnd);
					break;
				default:
					Console.WriteLine ("Unhandled element {0}. Value: '{1}'",
						    xmlReader.NodeType,
						    xmlReader.Value);
					break;
			}
		}
	}
	
	private static int DeserializeAttributes (TextBuffer buffer, int offset, XmlTextReader xmlReader)
	{
		int result = offset;
		switch (xmlReader.Name) {
			case "Type":
				result = DeserializeTypeAttributes (buffer, offset, xmlReader);
				break;
			case "TypeSignature":
				result = DeserializeTypeSignatureAttributes (buffer, offset, xmlReader);
				break;
			case "link":
				result = DeserializeLinkAttributes (buffer, offset, xmlReader);
				break;
			case "see":
				result = DeserializeSeeAttributes (buffer, offset, xmlReader);
				break;
			default:
				break;
		}
		
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
}
}