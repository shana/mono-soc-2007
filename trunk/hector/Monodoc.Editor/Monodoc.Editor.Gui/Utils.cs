//
// Utils.cs: Miscellaneous helper classes.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using Gtk;
using System;

namespace Monodoc.Editor.Gui {
public class DocumentUtils {
	private static int counter = 0;
	
	public static bool TagEndsHere (TextTag tag, TextIter currentIter, TextIter nextIter)
	{
		return (currentIter.HasTag (tag) && !nextIter.HasTag (tag));
	}
	
	public static TextTag GetLastTag (TextIter iter)
	{
		TextTag [] tags = iter.Tags;
		int last_index = tags.Length  - 1;
		
		return tags [last_index];
	}
	
	public static TextTag GetAssociatedTextTag (TextBuffer buffer, TextTag tag)
	{
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		DocumentTag elementTag = (DocumentTag) tag;
		string elementName = elementTag.Name;
		string suffix, tagName;
		
		// Check if element is dynamic:
		// True: We create a tagname with suffix :Text#[0-9]* so its unique.
		// False: We create a tagname with a standard suffix :Text
		if (elementTag.IsDynamic) {
			suffix = "#" + elementName.Split ('#') [1];
			tagName = elementName.Split ('#')[0] + ":Text" + suffix;
		} else {
			suffix = String.Empty;
			tagName = elementName + ":Text";
		}
		
		TextTag textTag = tagTable.Lookup (tagName);
		if (textTag == null)
			textTag = tagTable.CreateDynamicTag (tagName);
		
		return textTag;
	}
	
	public static int AddPadding (TextBuffer buffer, int offset, string suffix)
	{
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		AddPadding (buffer, ref insertAt, suffix);
		
		return insertAt.Offset;
	}
	
	public static void AddPadding (TextBuffer buffer, ref TextIter insertAt, string suffix)
	{
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextTag tag = tagTable.Lookup ("padding" + suffix);
		
		if (tag == null)
			tag = tagTable.CreateDynamicTag ("padding" + suffix);
		buffer.InsertWithTags (ref insertAt, " ", tag);
	}
	
	public static int AddPaddingEmpty (TextBuffer buffer, int offset,  string suffix)
	{
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		AddPaddingEmpty (buffer, ref insertAt, suffix);
		
		return insertAt.Offset;
	}
	
	public static void AddPaddingEmpty (TextBuffer buffer, ref TextIter insertAt, string suffix)
	{
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextTag tag = tagTable.Lookup ("padding-empty" + suffix);
		
		if (tag == null)
			tag = tagTable.CreateDynamicTag ("padding-empty" + suffix);
		buffer.InsertWithTags (ref insertAt, " ", tag);
	}
	
	public static int AddText (TextBuffer buffer, int offset, string data, TextTag tag)
	{
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		AddText (buffer, ref insertAt, data, tag);
		
		return insertAt.Offset;
	}
	
	public static void AddText (TextBuffer buffer, ref TextIter insertAt, string data, TextTag tag)
	{
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextTag textTag = tagTable.Lookup ("significant-whitespace" + "#" +  tag.Name);
		if (textTag == null)
			textTag = tagTable.CreateDynamicTag ("significant-whitespace" + "#" + tag.Name);
		
		string trimData = data.Trim ();
		int index = data.IndexOf (trimData);
		
		string startSpace = data.Substring (0, index);
		string prefixSpace = String.Empty;
		if (!startSpace.Equals (String.Empty)) {
			if (startSpace.Length == 1) {
				prefixSpace = startSpace;
				startSpace = String.Empty;
			} else {
				prefixSpace = startSpace.Substring (startSpace.Length - 1);
				startSpace = startSpace.Substring (0, startSpace.Length - 1);
			}
		}
		
		string endSpace = data.Substring (index + trimData.Length);
		string postSpace = String.Empty;
		if (!endSpace.Equals (String.Empty)) {
			if (endSpace.Length == 1) {
				if (endSpace.Equals (" ")) {
					postSpace = endSpace;
					endSpace = String.Empty;
				}
			} else {
				if (endSpace.Substring (0, 1).Equals (" ")) {
					postSpace = endSpace.Substring (0, 1);
					endSpace = endSpace.Substring (1);
				}
			}
		}
		
		buffer.InsertWithTags (ref insertAt, Escape (startSpace), textTag);
		buffer.InsertWithTags (ref insertAt, prefixSpace + trimData + postSpace, tag);
		buffer.InsertWithTags (ref insertAt, Escape (endSpace), textTag);
	}
	
	public static string Escape (string whitespace)
	{
		return whitespace.Replace ("\n", "N");
	}
	
	public static string Unescape (string whitespace)
	{
		return whitespace.Replace ("N", "\n");
	}
	
	public static int AddStub (TextBuffer buffer, int offset, string data, string suffix)
	{
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		AddStub (buffer, ref insertAt, data, suffix);
		
		return insertAt.Offset;
	}
	
	public static void AddStub (TextBuffer buffer, ref TextIter insertAt, string data, string suffix)
	{
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		
		TextTag textTag = tagTable.Lookup ("stub" + suffix + "#" + counter);
		if (textTag == null)
			textTag = tagTable.CreateDynamicTag ("stub" + suffix + "#" +  counter);
		
		counter++;
		buffer.InsertWithTags (ref insertAt, data, textTag);
	}
	
	public static int AddString (TextBuffer buffer, int offset, string data, string suffix)
	{
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		AddString (buffer, ref insertAt, data, suffix);
		
		return insertAt.Offset;
	}
	
	public static void AddString (TextBuffer buffer, ref TextIter insertAt, string data, string suffix)
	{
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextTag tag = tagTable.Lookup ("format" + suffix);
		
		if (tag == null)
			tag = tagTable.CreateDynamicTag ("format" + suffix);
		buffer.InsertWithTags (ref insertAt, data, tag);
		
		tag = tagTable.Lookup ("format-end" + suffix);
		
		if (tag == null)
			tag = tagTable.CreateDynamicTag ("format-end" + suffix);
		buffer.InsertWithTags (ref insertAt, " ", tag);
	}
	
	public static int AddNewLine (TextBuffer buffer, int offset, string suffix)
	{
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		AddNewLine (buffer, ref insertAt, suffix);
		
		return insertAt.Offset;
	}
	
	public static void AddNewLine (TextBuffer buffer, ref TextIter insertAt, string suffix)
	{
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextTag tag = tagTable.Lookup ("newline" + suffix);
		
		if (tag == null)
			tag = tagTable.CreateDynamicTag ("newline" + suffix);
		buffer.InsertWithTags (ref insertAt, "\n", tag);
	}
}

public class TextRange {
	private TextBuffer buffer;
	private TextMark start_mark;
	private TextMark end_mark;
	
	public TextRange (TextIter start, TextIter end)
	{
		if (start.Buffer != end.Buffer)
			throw new Exception ("Start buffer and end buffer do not match");
		
		buffer = start.Buffer;
		start_mark = buffer.CreateMark (null, start, true);
		end_mark = buffer.CreateMark (null, end, true);
	}
	
	public TextBuffer Buffer {
		get {
			return buffer;
		}
	}
	
	public string Text {
		get {
			return Start.GetText (End);
		}
	}
	
	public int Length {
		get {
			return  Text.Length;
		}
	}
	
	public TextIter Start {
		get {
			return buffer.GetIterAtMark (start_mark);
		}
		
		set {
			buffer.MoveMark (start_mark, value);
		}
	}
	
	public TextIter End {
		get {
			return buffer.GetIterAtMark (end_mark);
		}
		
		set {
			buffer.MoveMark (end_mark, value);
		}
	}
	
	public void Erase ()
	{
		TextIter startIter = Start;
		TextIter endIter = End;
		buffer.Delete (ref startIter, ref endIter);
	}
	
	public void Destroy ()
	{
		buffer.DeleteMark (start_mark);
		buffer.DeleteMark (end_mark);
	}
	
	public void RemoveTag (TextTag tag)
	{
		buffer.RemoveTag (tag, Start, End);
	}
}
}