//
// DocumentBufferArchiver.cs: Class that handles the serialization and deserialization of the buffer.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using Gtk;
using System;
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
		
		while (xmlReader.Read ()) {
			switch (xmlReader.NodeType) {
			case XmlNodeType.Element:
				offset = InsertStartElement (buffer, offset, xmlReader, stack, ref depth, ref count);
				break;
			case XmlNodeType.Text:
				offset = InsertText (buffer, offset, xmlReader.Value, stack);
				break;
			case XmlNodeType.EndElement:
				offset = InsertEndElement (buffer, offset, stack, ref depth); 
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
		// Suffix has format: #{depth level}
		if (isDynamic)
			suffix = '#' + depth.ToString ();
		
		// We first lookup the tag name, if the element is dynamic, we can
		// have three scenarios.
		// 1) The tag is not in the table: So we create it in the spot.
		// 2) Tag is in table but it priority is wrong: We created a new
		// dynamic tag with an extra suffix. Format #{depth level}.{count}
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
		
		// We add any needed string to give format to the document.
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
			
			// End string added for empty elements.
			offset = FormatEmpty (buffer, offset, suffix, elementName);
			depth--;
			
			#if DEBUG
			Console.WriteLine ("Empty Element: {0}, Start: {1}, End: {2}", tagStart.Tag.Name, tagStart.Start, offset);
			#endif
		} else
			stack.Push (tagStart);
		
		return offset;
	}
	
	private static int InsertText (TextBuffer buffer, int offset, string data, Stack stack)
	{
		string tagName;
		TextTag textTag;
		TagStart tagStart = (TagStart) stack.Peek ();
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		DocumentTag elementTag = (DocumentTag) tagStart.Tag;
		string elementName = elementTag.Name;
		
		// Check if element is dynamic:
		// True: We create a tagname with suffix :Text#[0-9]* so its unique.
		// False: We create a tagname with a standard suffix :Text
		if (elementTag.IsDynamic)
			tagName = elementName.Split ('#')[0] + ":Text" + '#' + elementName.Split ('#')[1];
		else
			tagName = elementName + ":Text";
		
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
	
	private static int InsertEndElement (TextBuffer buffer, int offset, Stack stack, ref int depth)
	{
		TextIter applyStart, applyEnd;
		TagStart tagStart = (TagStart) stack.Pop ();
		string suffix =  '#' + depth.ToString ();
		
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
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		switch (elementName) {
		case "AssemblyName":
			AddString (buffer, ref insertAt, "Assembly Name: ", suffix);
			break;
		case "AssemblyPublicKey":
			AddString (buffer, ref insertAt, "Assembly PublicKey: ", suffix);
			break;
		case "AssemblyVersion":
			AddString (buffer, ref insertAt, "Assembly Version: ", suffix);
			break;
		case "MemberOfLibrary":
			AddString (buffer, ref insertAt, "From Library: ", suffix);
			break;
		case "ThreadSafetyStatement":
			AddString (buffer, ref insertAt, "Threading Safety: ", suffix);
			break;
		case "ThreadingSafetyStatement":
			AddString (buffer, ref insertAt, "Threading Safety: ", suffix);
			break;
		case "summary":
			AddString (buffer, ref insertAt, "Summary:", suffix);
		        AddNewLine (buffer, ref insertAt, suffix);
			break;
		case "remarks":
			AddString (buffer, ref insertAt, "Remarks:", suffix);
			AddNewLine (buffer, ref insertAt, suffix);
			break;
		case "Members":
			AddString (buffer, ref insertAt, "Members:", suffix);
			AddNewLine (buffer, ref insertAt, suffix);
			AddNewLine (buffer, ref insertAt, suffix);
			break;
		case "MemberType":
			AddString (buffer, ref insertAt, "Member Type: ", suffix);
			break;
		case "ReturnType":
			AddString (buffer, ref insertAt, "Member Return Type: ", suffix);
			break;
		case "since":
			AddString (buffer, ref insertAt, "Since version: ", suffix);
			break;
		default:
			break;
		}
		
		return insertAt.Offset;
	}
	
	private static int FormatEmpty (TextBuffer buffer, int offset, string suffix, string elementName)
	{
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		switch (elementName) {
		case "see":
		case "since":
		case "Parameters":
		case "remarks":
		case "MemberSignature":
			AddPadding (buffer, ref insertAt, suffix);
			break;
		default:
			AddNewLine (buffer, ref insertAt, suffix);
			break;
		}
		
		return insertAt.Offset;
	}
	
	private static int FormatEnd (TextBuffer buffer, int offset, string suffix, string elementName)
	{
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		switch (elementName) {
		case "para":
		case "Docs":
		case "Base":
		case "BaseTypeName":
		case "Attribute":
		case "AttributeName":
		case "Members":
		case "Member":
		case "Type":
		case "link":
			break;
		case "summary":
		case "ThreadSafetyStatement":
		case "ThreadingSafetyStatement":
			AddNewLine (buffer, ref insertAt, suffix);
			AddNewLine (buffer, ref insertAt, suffix);
			break;
		default:
			AddNewLine (buffer, ref insertAt, suffix);
			break;
		}
		
		return insertAt.Offset;
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

	private static void AddString (TextBuffer buffer, ref TextIter insertAt, string data, string suffix)
	{
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextTag tag = tagTable.Lookup ("format" + suffix);
		
		if (tag == null)
			tag = tagTable.CreateDynamicTag ("format" + suffix);
		buffer.InsertWithTags (ref insertAt, data, tag);
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

	private static void AddPadding (TextBuffer buffer, ref TextIter insertAt, string suffix)
	{
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextTag tag = tagTable.Lookup ("padding" + suffix);
		
		if (tag == null)
			tag = tagTable.CreateDynamicTag ("padding" + suffix);
		buffer.InsertWithTags (ref insertAt, " ", tag);
	}
	
	private static int DeserializeAttributes (TextBuffer buffer, int offset, XmlTextReader xmlReader, string tagSuffix)
	{
		string elementName, tagName;
		elementName = tagName = xmlReader.Name;
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextIter insertAt, applyStart, applyEnd;
		insertAt = applyStart = buffer.GetIterAtOffset (offset);
		TextTag tagAttributes;
		
		// Lookup Attributes tag in table, if it is not present we create one.
		tagName += ":Attributes" + tagSuffix;
		tagAttributes = tagTable.Lookup (tagName);
		if (tagAttributes == null)
			tagAttributes = tagTable.CreateDynamicTag (tagName);
		
		switch (elementName) {
		case "Type":
			DeserializeAttributesType (buffer, ref insertAt, xmlReader, tagSuffix);
			break;
		case "TypeSignature":
			DeserializeAttributesTypeSignature (buffer, ref insertAt, xmlReader, tagSuffix);
			break;
		case "Member":
			DeserializeAttributesMember (buffer, ref insertAt, xmlReader, tagSuffix);
			break;
		case "MemberSignature":
			DeserializeAttributesMemberSignature (buffer, ref insertAt, xmlReader, tagSuffix);
			break;
		default:
			DeserializeAttributesNone (buffer, ref insertAt, xmlReader, tagSuffix);
			break;
		}
		
		applyEnd = insertAt;
		buffer.ApplyTag (tagAttributes, applyStart, applyEnd);
		
		#if DEBUG
		Console.WriteLine ("Attributes: {0} Start: {1} End: {2}", tagName, offset, insertAt.Offset);
		#endif
		
		return insertAt.Offset;
	}
	
	private static void DeserializeAttributesType (TextBuffer buffer, ref TextIter insertAt, XmlTextReader xmlReader, string tagSuffix)
	{
		string tagName = String.Empty;
		string tagPrefix = xmlReader.Name + ":";
		
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		while (xmlReader.MoveToNextAttribute ()) {
			tagName = tagPrefix + xmlReader.Name + tagSuffix;
			
			TextTag tag = tagTable.Lookup (tagName);
			if (tag == null)
				tag = tagTable.CreateDynamicTag (tagName);
			
			buffer.InsertWithTags (ref insertAt, xmlReader.Value, tag);
			
			#if DEBUG
			Console.WriteLine ("Attribute: {0} Start: {1} End: {2}", tagName, offset, insertAt.Offset);
			#endif
		}
		
		AddNewLine (buffer, ref insertAt, tagSuffix);
	}
	
	private static void DeserializeAttributesTypeSignature (TextBuffer buffer, ref TextIter insertAt, XmlTextReader xmlReader, string tagSuffix)
	{
		string tagName = String.Empty;
		string tagPrefix = xmlReader.Name + ":";
		
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
	}
	
	private static void DeserializeAttributesMember (TextBuffer buffer, ref TextIter insertAt, XmlTextReader xmlReader, string tagSuffix)
	{
		string tagName = String.Empty;
		string tagPrefix = xmlReader.Name + ":";
		
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		while (xmlReader.MoveToNextAttribute ()) {
			tagName = tagPrefix + xmlReader.Name + tagSuffix;
			
			TextTag formatTag = tagTable.Lookup ("format" + tagSuffix);
			if (formatTag == null)
				formatTag = tagTable.CreateDynamicTag ("format" + tagSuffix);
			
			if (xmlReader.Name == "MemberName")
				buffer.InsertWithTags (ref insertAt, "Member Name: ", formatTag);
			
			TextTag tag = tagTable.Lookup (tagName);
			if (tag == null)
				tag = tagTable.CreateDynamicTag (tagName);
			
			buffer.InsertWithTags (ref insertAt, xmlReader.Value, tag);
			AddNewLine (buffer, ref insertAt, tagSuffix);
			
			#if DEBUG
			Console.WriteLine ("Attribute: {0} Start: {1} End: {2}", tagName, offset, insertAt.Offset);
			#endif
		}
	}
	
	private static void DeserializeAttributesMemberSignature (TextBuffer buffer, ref TextIter insertAt, XmlTextReader xmlReader, string tagSuffix)
	{
		string tagName = String.Empty;
		string tagPrefix = xmlReader.Name + ":";
		
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		while (xmlReader.MoveToNextAttribute ()) {
			tagName = tagPrefix + xmlReader.Name + tagSuffix;
			
			TextTag formatTag = tagTable.Lookup ("format" + tagSuffix);
			if (formatTag == null)
				formatTag = tagTable.CreateDynamicTag ("format" + tagSuffix);
			
			if (xmlReader.Name == "Language")
				buffer.InsertWithTags (ref insertAt, "Member Language: ", formatTag);
			
			if (xmlReader.Name == "Value")
				buffer.InsertWithTags (ref insertAt, "Member Signature: ", formatTag);
			
			TextTag tag = tagTable.Lookup (tagName);
			if (tag == null)
				tag = tagTable.CreateDynamicTag (tagName);
			
			buffer.InsertWithTags (ref insertAt, xmlReader.Value, tag);
			AddNewLine (buffer, ref insertAt, tagSuffix);
			
			#if DEBUG
			Console.WriteLine ("Attribute: {0} Start: {1} End: {2}", tagName, offset, insertAt.Offset);
			#endif
		}
	}
	
	private static void DeserializeAttributesNone (TextBuffer buffer, ref TextIter insertAt, XmlTextReader xmlReader, string tagSuffix)
	{
		string tagName = String.Empty;
		string tagPrefix = xmlReader.Name + ":";
		
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