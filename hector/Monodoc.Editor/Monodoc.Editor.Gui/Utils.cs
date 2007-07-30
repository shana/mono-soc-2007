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
	
	
	public static int AddString (TextBuffer buffer, int offset, string data, string suffix)
	{
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextTag tag = tagTable.Lookup ("format" + suffix);
		
		if (tag == null)
			tag = tagTable.CreateDynamicTag ("format" + suffix);
		buffer.InsertWithTags (ref insertAt, data, tag);
		
		return insertAt.Offset;
	}
	
	public static int AddNewLine (TextBuffer buffer, int offset, string suffix)
	{
		TextIter insertAt = buffer.GetIterAtOffset (offset);
		// AddNewLine (buffer, ref insertAt, suffix);
		
		return insertAt.Offset;
	}
}
}