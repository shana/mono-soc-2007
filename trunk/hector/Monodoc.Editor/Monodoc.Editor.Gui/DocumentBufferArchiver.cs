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
		public string Name;
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
		DocumentTag docTag;
		nextIter.ForwardChar ();
		
		// We iterate over all the buffer.
		while (!currentIter.Equal (end)) {
			ArrayList beginTags, endTags;
			beginTags = new ArrayList ();
			endTags = new ArrayList ();
			GetArrays (currentIter, nextIter, beginTags, endTags);
			
			#if DEBUG
			Console.WriteLine ("Offset: {0} Char: {1}", currentIter.Offset, currentIter.Char);
			#endif
			
			if (readingValue)
				attributeValue += currentIter.Char;
			
			if (readingText)
				elementText += currentIter.Char;
			
			foreach (TextTag tag in beginTags) {
				#if DEBUG
				Console.WriteLine ("Begin Tags: {0} Begins: {1} Ends: {2}", tag.Name, currentIter.BeginsTag (tag) ? "True" : "False", TagEndsHere (tag, currentIter, nextIter)? "True" : "False");
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
			
			foreach (TextTag tag in endTags) {
				#if DEBUG
				Console.WriteLine ("End Tags: {0} Begins: {1} Ends: {2}", tag.Name, currentIter.BeginsTag (tag) ? "True" : "False", TagEndsHere (tag, currentIter, nextIter)? "True" : "False");
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
			
//			while (Application.EventsPending ())
//				Application.RunIteration ();
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
		int depth, count;
		count = depth = 0;
		Stack stack = new Stack ();
		TagStart tagStart;
		
		while (xmlReader.Read ()) {
			switch (xmlReader.NodeType) {
			case XmlNodeType.Element:
				offset = InsertStartElement (buffer, offset, xmlReader, stack, ref depth, ref count);
				break;
			case XmlNodeType.Text:
				tagStart = (TagStart) stack.Peek ();
				DocumentTag docTag = (DocumentTag) tagStart.Tag;
				offset = InsertText (buffer, offset, xmlReader.Value, docTag);
				break;
			case XmlNodeType.EndElement:
				tagStart = (TagStart) stack.Pop ();
				offset = InsertEndElement (buffer, offset, tagStart, ref depth); 
				break;
			case XmlNodeType.Whitespace:
				break;
			default:
				Console.WriteLine ("Unhandled Element {0}. Value: '{1}'",
					    xmlReader.NodeType,
					    xmlReader.Value);
				break;
			}
			
//			while (Application.EventsPending ())
//				Application.RunIteration ();
		}
	}
	
	private static int InsertStartElement (TextBuffer buffer, int offset, XmlTextReader xmlReader, Stack stack, ref int depth, ref int count)
	{
		string elementName = xmlReader.Name;
		string suffix = String.Empty;
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		bool emptyElement = xmlReader.IsEmptyElement;
		bool isDynamic = tagTable.IsDynamic (elementName);
		TextIter applyStart, applyEnd;
		TagStart tagStart = new TagStart ();
		tagStart.Start = offset;
		tagStart.Name = elementName;
		depth++;
		
		// We define a suffix so each dynamic tag has an unique name.
		if (isDynamic)
			suffix = '#' + depth.ToString ();
		
		// We first lookup the tag name, if the element is dynamic, we can
		// have three scenarios.
		// 1) The tag is not in the table: So we create it in the spot.
		// 2) Tag is in table but it priority is wrong: We created a new
		// dynamic tag with an extra suffix.
		// 3) Tag is in table with right priority: We reuse it and we don't
		// create a new dymamic tag.
		tagStart.Tag = tagTable.Lookup (elementName + suffix);
		if (isDynamic && tagStart.Tag == null)
			tagStart.Tag = tagTable.CreateDynamicTag (elementName + suffix);
		else if (isDynamic && tagStart.Tag != null &&  tagStart.Tag.Priority < ((TagStart) stack.Peek ()).Tag.Priority) {
			suffix += "." + count;
			tagStart.Tag = tagTable.CreateDynamicTag (elementName + suffix);
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
		
		offset = FormatStart (buffer, offset, suffix, elementName);
		
		// If element has attributes we have to get them and deserialize them.
		if (xmlReader.HasAttributes)
			offset = DeserializeAttributes (buffer, offset, xmlReader, suffix);
		
		// Special case when an elment is empty. Because the way TextTag
		// are implemented, empty ranges get lost in the buffer so we but
		// a padding.
		if (emptyElement) {
			if (tagStart.Start != offset) {
				applyStart = buffer.GetIterAtOffset (tagStart.Start);
				applyEnd = buffer.GetIterAtOffset (offset);
			} else {
				// Padding to conserve empty element
				offset = AddPadding (buffer, offset, suffix);
				
				applyStart = buffer.GetIterAtOffset (tagStart.Start);
				applyEnd = buffer.GetIterAtOffset (offset);
			}
			
			buffer.ApplyTag (tagStart.Tag, applyStart, applyEnd);
			
			if (suffix == String.Empty)
				suffix = "#0";
			
			// Padding between tag regions
			offset = AddPadding (buffer, offset, suffix + ".1");
			depth--;
			
			#if DEBUG
			Console.WriteLine ("Empty Element: {0}, Start: {1}, End: {2}", tagStart.Tag.Name, tagStart.Start, offset);
			#endif
		} else
			stack.Push (tagStart);
		
		return offset;
	}
	
	private static int InsertText (TextBuffer buffer, int offset, string data, DocumentTag elementTag)
	{
		string tagName;
		TextTag textTag;
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		
		// Check if element is dynamic:
		// True: We create a tagname with suffix :Text#[0-9]* so its unique.
		// False: We create a tagname with a standard suffix :Text
		if (elementTag.IsDynamic)
			tagName = elementTag.Name.Split ('#')[0] + ":Text" + '#' + elementTag.Name.Split ('#')[1];
		else
			tagName = elementTag.Name + ":Text";
		
		textTag = tagTable.Lookup (tagName);
		if (textTag == null)
			textTag = tagTable.CreateDynamicTag (tagName);
		
		#if DEBUG
		Console.WriteLine ("Text: {0} Value: {1} Start: {2}", tagName, data, offset);
		#endif
		
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		buffer.InsertWithTags (ref insertAt, data, textTag);
		offset += data.Length;
		
		return offset;
	}
	
	private static int InsertEndElement (TextBuffer buffer, int offset, TagStart tagStart, ref int depth)
	{
		string suffix =  '#' + depth.ToString ();
		TextIter applyStart, applyEnd;
		
		#if DEBUG
		Console.WriteLine ("Element: {0}, End: {1}", tagStart.Tag.Name, offset);
		#endif
		
		if (tagStart.Start != offset) {
			applyStart = buffer.GetIterAtOffset (tagStart.Start);
			applyEnd = buffer.GetIterAtOffset (offset);
			buffer.ApplyTag (tagStart.Tag, applyStart, applyEnd);
			offset = FormatEnd (buffer, offset, suffix, tagStart.Name);
		} else {
			// Padding to conserve empty element
			offset = AddPadding (buffer, offset, suffix);
			
			applyStart = buffer.GetIterAtOffset (tagStart.Start);
			applyEnd = buffer.GetIterAtOffset (offset); 
			buffer.ApplyTag (tagStart.Tag, applyStart, applyEnd);
		}
		depth--;
		
		#if DEBUG
		Console.WriteLine ("Applied: {0}, Start: {1}, End: {2}", tagStart.Tag.Name, tagStart.Start, offset);
		#endif
		
		// Padding between tag regions
		suffix = '#' + depth.ToString ();
		offset = AddPadding (buffer, offset, suffix);
		
		return offset;
	}
	
	private static int FormatStart (TextBuffer buffer, int offset, string suffix, string elementName)
	{
		switch (elementName) {
//		case "AssemblyInfo":
//			offset = AddString (buffer, offset, "About Assembly:", suffix);
//			offset = AddNewLine (buffer, offset, suffix);
//			break;
		case "AssemblyName":
			offset = AddString (buffer, offset, "Name: ", suffix);
			break;
		case "AssemblyVersion":
			offset = AddString (buffer, offset, "Version: ", suffix);
			break;
		case "ThreadSafetyStatement":
			offset = AddString (buffer, offset, "Thread Safety: ", suffix);
			break;
		case "summary":
			offset = AddString (buffer, offset, "Summary:", suffix);
			offset = AddNewLine (buffer, offset, suffix);
			break;
		case "remarks":
			offset = AddString (buffer, offset, "Remarks:", suffix);
			offset = AddNewLine (buffer, offset, suffix);
			break;
		default:
			break;
		}
		
		return offset;
	}
	
	private static int FormatEnd (TextBuffer buffer, int offset, string suffix, string elementName)
	{
		switch (elementName) {
		case "para":
		case "remarks":
		case "Docs":
		case "Base":
		case "BaseTypeName":
		case "Attribute":
		case "AttributeName":
		case "Members":
		case "Member":
		case "since":
		case "Type":
		case "link":
			break;
		default:
			offset = AddNewLine (buffer, offset, suffix);
			break;
		}
		
		return offset;
	}
	
	private static int AddString (TextBuffer buffer, int offset, string data, string suffix)
	{
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextTag tag = tagTable.Lookup ("format" + suffix);
		
		if (tag == null)
			tag = tagTable.CreateDynamicTag ("format" + suffix);
		buffer.InsertWithTags (ref insertAt, data, tag);
		
		return insertAt.Offset;
	}
	
	private static int AddNewLine (TextBuffer buffer, int offset, string suffix)
	{
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		AddNewLine (buffer, ref insertAt, suffix);
		
		return insertAt.Offset;
	}
	
	private static void AddNewLine (TextBuffer buffer, ref TextIter insertAt, string suffix)
	{
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextTag tag = tagTable.Lookup ("newline" + suffix);
		
		if (tag == null)
			tag = tagTable.CreateDynamicTag ("newline" + suffix);
		buffer.InsertWithTags (ref insertAt, "\n", tag);
	}
	
	private static int AddPadding (TextBuffer buffer, int offset, string suffix)
	{
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextTag tag = tagTable.Lookup ("padding" + suffix);
		
		if (tag == null)
			tag = tagTable.CreateDynamicTag ("padding" + suffix);
		buffer.InsertWithTags (ref insertAt, " ", tag);
		
		return insertAt.Offset;
	}
	
	private static int DeserializeAttributes (TextBuffer buffer, int offset, XmlTextReader xmlReader, string tagSuffix)
	{
		int result = offset;
		string elementName, tagName;
		elementName = tagName = xmlReader.Name;
		TextIter applyStart, applyEnd;
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextTag tagAttributes;
		
		tagName += ":Attributes" + tagSuffix;
		tagAttributes = tagTable.Lookup (tagName);
		if (tagAttributes == null)
			tagAttributes = tagTable.CreateDynamicTag (tagName);
		
		switch (elementName) {
		case "Type":
		case "TypeSignature":
			result = DeserializeAttributesTypeSignature (buffer, offset, xmlReader, tagSuffix);
			break;
		default:
			result = DeserializeAttributesNone (buffer, offset, xmlReader, tagSuffix);
			break;
		}
		
		applyStart = buffer.GetIterAtOffset (offset);
		applyEnd = buffer.GetIterAtOffset (result);
		buffer.ApplyTag (tagAttributes, applyStart, applyEnd);
		
		#if DEBUG
		Console.WriteLine ("Attributes: {0} Start: {1} End: {2}", tagName, offset, result);
		#endif
		
		return result;
	}
	
	private static int DeserializeAttributesTypeSignature (TextBuffer buffer, int offset, XmlTextReader xmlReader, string tagSuffix)
	{
		string tagName = String.Empty;
		string tagPrefix = xmlReader.Name + ":";
		
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		while (xmlReader.MoveToNextAttribute ()) {
			tagName = tagPrefix + xmlReader.Name + tagSuffix;
			
			TextTag formatTag = tagTable.Lookup ("format" + tagSuffix);
			if (formatTag == null)
				formatTag = tagTable.CreateDynamicTag ("format" + tagSuffix);
			
			if (xmlReader.Name == "Language")
				buffer.InsertWithTags (ref insertAt, "Language: ", formatTag);
			
			if (xmlReader.Name == "Maintainer")
				buffer.InsertWithTags (ref insertAt, "Maintainer: ", formatTag);
			
			if (xmlReader.Name == "Value")
				buffer.InsertWithTags (ref insertAt, "Signature: ", formatTag);
			
			TextTag tag = tagTable.Lookup (tagName);
			if (tag == null)
				tag = tagTable.CreateDynamicTag (tagName);
			
			buffer.InsertWithTags (ref insertAt, xmlReader.Value, tag);
			AddNewLine (buffer, ref insertAt, tagSuffix);
			
			#if DEBUG
			Console.WriteLine ("Attribute: {0} Start: {1} End: {2}", tagName, offset, insertAt.Offset);
			#endif
		}
		
		AddNewLine (buffer, ref insertAt, tagSuffix);
		
		if (tagPrefix != "Type:")
			offset = AddPadding (buffer, insertAt.Offset, tagSuffix);
		else
			offset = insertAt.Offset;
		return offset;
	}
	
	private static int DeserializeAttributesNewline (TextBuffer buffer, int offset, XmlTextReader xmlReader, string tagSuffix)
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
			
			AddNewLine (buffer, ref insertAt, tagSuffix);
		}
		
		#if DEBUG
		Console.WriteLine ("Attribute: {0} Start: {1} End: {2}", tagName, offset, insertAt.Offset);
		#endif
		
		return insertAt.Offset;
	}

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
	
	private static void GetArrays (TextIter currentIter, TextIter nextIter, ArrayList beginTags, ArrayList endTags)
	{
		TextTag [] tags = currentIter.Tags;
		int last_index = tags.Length  - 1;
		TextTag last = tags [last_index];
		
		if (currentIter.BeginsTag (last)) {
			beginTags.InsertRange (0, tags);
			
			if (TagEndsHere (last,currentIter, nextIter)) {
				Array.Reverse (tags);
				endTags.InsertRange (0, tags);
			} else
				endTags = null;
		} else if (TagEndsHere (last, currentIter, nextIter)) {
			beginTags = null;
			Array.Reverse (tags);
			endTags.InsertRange (0, tags);
		}
	}
}
}