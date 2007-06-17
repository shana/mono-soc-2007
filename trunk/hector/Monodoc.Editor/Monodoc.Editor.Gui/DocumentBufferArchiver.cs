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
		
		DocumentTagTable docTable = buffer.TagTable as DocumentTagTable;
		int curr_depth = -1;
		
		while (xmlReader.Read ()) {
			switch (xmlReader.NodeType) {
//				case XmlNodeType.Element:
//					break;
//				case XmlNodeType.Text:
//				case XmlNodeType.Whitespace:
//				case XmlNodeType.SignificantWhitespace:
//					break;
//				case XmlNodeType.EndElement:
//					break;
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