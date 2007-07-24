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
}
}