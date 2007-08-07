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
				Console.WriteLine ("Begin Tags: {0} Begins: {1} Ends: {2}", tag.Name, currentIter.BeginsTag (tag) ? "True" : "False", DocumentUtils.TagEndsHere (tag, currentIter, nextIter)? "True" : "False");
				#endif
				
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
			
			foreach (TextTag tag in endTags) {
				#if DEBUG
				Console.WriteLine ("End Tags: {0} Begins: {1} Ends: {2}", tag.Name, currentIter.BeginsTag (tag) ? "True" : "False", DocumentUtils.TagEndsHere (tag, currentIter, nextIter)? "True" : "False");
				#endif
				
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
					if (docTag.Name.IndexOf ("significant-whitespace") != -1) {
						xmlWriter.WriteString (DocumentUtils.Unescape (elementText));
					} else
						xmlWriter.WriteString (elementText);
					
					elementText = String.Empty;
					readingText = false;
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
		bool isDynamic = DocumentTagTable.IsDynamic (elementName);
		TextIter insertAt, applyStart, applyEnd;
		TagStart tagStart = new TagStart ();
		tagStart.Start = offset;
		tagStart.Name = elementName;
		depth++;
		
		// We define a suffix so each dynamic tag has an unique name.
		// Suffix has format: #{depth level}
		if (isDynamic)
			suffix = "#" + depth;
		
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
		
		// Special case when an elment is empty.
		// Case A: If element is editable a string stub is inserted to allow edition.
		// Case B: If element is not editable then a padding is inserted to handle
		// TextTag behaviour in which zero length ranges are lost.
		if (emptyElement) {
			if (((DocumentTag) tagStart.Tag).IsEditable) {
				insertAt = buffer.GetIterAtOffset (offset);
				buffer.Insert (ref insertAt, "[");
				offset += 1;
				
				offset = DocumentUtils.AddStub (buffer, offset, "Click to Add Documentation", suffix);
				
				insertAt = buffer.GetIterAtOffset (offset);
				buffer.Insert (ref insertAt, "]");
				offset += 1;
			} else
				offset = DocumentUtils.AddPaddingEmpty (buffer, offset, suffix);
			
			applyStart = buffer.GetIterAtOffset (tagStart.Start);
			applyEnd = buffer.GetIterAtOffset (offset);
			buffer.ApplyTag (tagStart.Tag, applyStart, applyEnd);
			offset = FormatEnd (buffer, offset, suffix, elementName);
			
			// Padding between tag regions
			offset = DocumentUtils.AddPadding (buffer, offset, suffix);
			depth--;
			
			#if DEBUG
			Console.WriteLine ("Empty Element: {0}, Start: {1}, End: {2}", tagStart.Tag.Name, tagStart.Start, offset);
			#endif
		} else {
			stack.Push (tagStart);
			
			if (((DocumentTag) tagStart.Tag).IsEditable) {
				insertAt = buffer.GetIterAtOffset (offset);
				buffer.Insert (ref insertAt, "[");
				offset += 1;
			}
		}
		
		return offset;
	}
	
	private static int InsertText (TextBuffer buffer, int offset, string data, Stack stack)
	{
		TagStart tagStart = (TagStart) stack.Peek ();
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		TextTag textTag = DocumentUtils.GetAssociatedTextTag (buffer, tagStart.Tag);
		DocumentUtils.AddText (buffer, ref insertAt, data, textTag);
//		buffer.InsertWithTags (ref insertAt, data, textTag);
		
		#if DEBUG
		Console.WriteLine ("Text: {0} Value: {1} Start: {2}", textTag.Name, data, offset);
		#endif
		
		return insertAt.Offset;
	}
	
	private static int InsertEndElement (TextBuffer buffer, int offset, Stack stack, ref int depth)
	{
		TextIter insertAt, applyStart, applyEnd;
		TagStart tagStart = (TagStart) stack.Pop ();
		string suffix =  '#' + depth.ToString ();
		
		#if DEBUG
		Console.WriteLine ("Element: {0}, End: {1}", tagStart.Tag.Name, offset);
		#endif
		
		if (((DocumentTag) tagStart.Tag).IsEditable) {
			insertAt = buffer.GetIterAtOffset (offset);
			buffer.Insert (ref insertAt, "]");
			offset += 1;
		} else if (tagStart.Start == offset)
			offset = DocumentUtils.AddPaddingEmpty (buffer, offset, suffix);
		
//		if (((DocumentTag) tagStart.Tag).IsEditable && tagStart.Empty)
//			offset = DocumentUtils.AddStub (buffer, offset, "To be added test", suffix);
		
		applyStart = buffer.GetIterAtOffset (tagStart.Start);
		applyEnd = buffer.GetIterAtOffset (offset);
		buffer.ApplyTag (tagStart.Tag, applyStart, applyEnd);
		offset = FormatEnd (buffer, offset, suffix, tagStart.Name);
		depth--;
		
		#if DEBUG
		Console.WriteLine ("Applied: {0}, Start: {1}, End: {2}", tagStart.Tag.Name, tagStart.Start, offset);
		#endif
		
		// Padding between tag regions
		suffix = "#" + depth;
		offset = DocumentUtils.AddPadding (buffer, offset, suffix);
		
		return offset;
	}
	
	private static int FormatStart (TextBuffer buffer, int offset, string suffix, string elementName)
	{
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		switch (elementName) {
		case "AssemblyName":
			DocumentUtils.AddString (buffer, ref insertAt, "Assembly Name: ", suffix);
			break;
		case "AssemblyPublicKey":
			DocumentUtils.AddString (buffer, ref insertAt, "Assembly PublicKey: ", suffix);
			break;
		case "AssemblyVersion":
			DocumentUtils.AddString (buffer, ref insertAt, "Assembly Version: ", suffix);
			break;
		case "MemberOfLibrary":
			DocumentUtils.AddString (buffer, ref insertAt, "From Library: ", suffix);
			break;
		case "ThreadSafetyStatement":
			DocumentUtils.AddString (buffer, ref insertAt, "Threading Safety: ", suffix);
			break;
		case "ThreadingSafetyStatement":
			DocumentUtils.AddString (buffer, ref insertAt, "Threading Safety: ", suffix);
			break;
		case "summary":
			DocumentUtils.AddString (buffer, ref insertAt, "Summary:\n", suffix);
			break;
		case "remarks":
			DocumentUtils.AddString (buffer, ref insertAt, "Remarks:\n", suffix);
			break;
		case "Members":
			DocumentUtils.AddString (buffer, ref insertAt, "Members:\n\n", suffix);
			break;
		case "MemberType":
			DocumentUtils.AddString (buffer, ref insertAt, "Member Type: ", suffix);
			break;
		case "ReturnType":
			DocumentUtils.AddString (buffer, ref insertAt, "Member Return Type: ", suffix);
			break;
		case "since":
			DocumentUtils.AddString (buffer, ref insertAt, "Since version: ", suffix);
			break;
		default:
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
		case "see":
		case "since":
		case "paramref":
		case "Parameters":
		case "MemberSignature":
			break;
		case "summary":
		case "ThreadSafetyStatement":
		case "ThreadingSafetyStatement":
			DocumentUtils.AddString (buffer, ref insertAt, "\n\n",  suffix);
			break;
		default:
			DocumentUtils.AddString (buffer, ref insertAt, "\n", suffix);
			break;
		}
		
		return insertAt.Offset;
	}
	
	private static int DeserializeAttributes (TextBuffer buffer, int offset, XmlTextReader xmlReader, string tagSuffix)
	{
		string elementName, tagName;
		elementName = tagName = xmlReader.Name;
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextIter insertAt, applyStart, applyEnd;
		insertAt = buffer.GetIterAtOffset (offset);
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
		
		applyStart = buffer.GetIterAtOffset (offset);
		applyEnd = buffer.GetIterAtOffset (insertAt.Offset);
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
			Console.WriteLine ("Attribute: {0} End: {1}", tagName, insertAt.Offset);
			#endif
		}
		
		DocumentUtils.AddNewLine (buffer, ref insertAt, tagSuffix);
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
			DocumentUtils.AddNewLine (buffer, ref insertAt, tagSuffix);
			
			#if DEBUG
			Console.WriteLine ("Attribute: {0} End: {1}", tagName, insertAt.Offset);
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
			DocumentUtils.AddNewLine (buffer, ref insertAt, tagSuffix);
			
			#if DEBUG
			Console.WriteLine ("Attribute: {0} End: {1}", tagName, insertAt.Offset);
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
			DocumentUtils.AddNewLine (buffer, ref insertAt, tagSuffix);
			
			#if DEBUG
			Console.WriteLine ("Attribute: {0} End: {1}", tagName, insertAt.Offset);
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
		Console.WriteLine ("Attribute: {0} End: {1}", tagName, insertAt.Offset);
		#endif
	}
	
	private static void GetArrays (TextIter currentIter, TextIter nextIter, ArrayList beginTags, ArrayList endTags)
	{
		TextTag [] tags = currentIter.Tags;
		int last_index = tags.Length  - 1;
		TextTag last = tags [last_index];
		
		if (currentIter.BeginsTag (last))
			GetBeginTags (currentIter, tags, beginTags);
		
		if (DocumentUtils.TagEndsHere (last, currentIter, nextIter))
			GetEndTags (currentIter, nextIter, tags, endTags);
	}

	private static void GetBeginTags (TextIter currentIter, TextTag [] tagArray, ArrayList beginTags)
	{
		foreach (TextTag tag in tagArray) {
			if (!currentIter.BeginsTag (tag))
				continue;
			
			beginTags.Add (tag);
		}
	}
	
	private static void GetEndTags (TextIter currentIter, TextIter nextIter, TextTag [] tagArray, ArrayList endTags)
	{
		Array.Reverse (tagArray);
		foreach (TextTag tag in tagArray) {
			if (!DocumentUtils.TagEndsHere (tag, currentIter, nextIter))
				continue;
			
			endTags.Add (tag);
		}
	}
}
}