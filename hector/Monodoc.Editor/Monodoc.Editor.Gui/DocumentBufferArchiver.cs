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
		TextTag tag;
		TextIter insert_at;
		
		DocumentTagTable docTable = buffer.TagTable as DocumentTagTable;
		int curr_depth = -1;
		
		while (xmlReader.Read ()) {
			switch (xmlReader.NodeType) {
				case XmlNodeType.Element:
					tagStart = new TagStart ();
					tagStart.Start = offset;
					
					tagStart.Tag = buffer.TagTable.Lookup (xmlReader.Name);
					if (xmlReader.HasAttributes) {
						string tagName = xmlReader.Name + ":Attributes";
						string attributes = "";
						
						while (xmlReader.MoveToNextAttribute ()) {
							attributes += xmlReader.Name + ": " + xmlReader.Value + " ";
						}
						
						attributes += "\n";
						insert_at = buffer.GetIterAtOffset (offset);
						buffer.InsertWithTagsByName (ref insert_at, attributes, tagName);
						
						offset += attributes.Length;
					}
					
					stack.Push (tagStart);
					break;
				case XmlNodeType.Text:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					insert_at = buffer.GetIterAtOffset (offset);
					buffer.Insert (ref insert_at, xmlReader.Value);
					
					offset += xmlReader.Value.Length;
					break;
				case XmlNodeType.EndElement:
					tagStart = (TagStart) stack.Pop ();
					if (tagStart.Tag == null)
						break;
					
					TextIter applyStart, applyEnd;
					applyStart = buffer.GetIterAtOffset (tagStart.Start);
					applyEnd = buffer.GetIterAtOffset (offset);
					
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
}
}