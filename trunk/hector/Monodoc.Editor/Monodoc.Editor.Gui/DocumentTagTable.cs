
//
// DocumentTagTable.cs: TextTagTable based class that represent the table of tags for a buffer.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using Gtk;

namespace Monodoc.Editor.Gui {
public class DocumentTagTable : TextTagTable {
	
	private static DocumentTagTable instance;
	
	public static DocumentTagTable Instance {
		get {
			if (instance == null)
				instance = new DocumentTagTable ();
			return instance;
		}
	}
	
	public DocumentTagTable () : base ()
	{
		InitCommonTags ();
	}
	
	private void InitCommonTags ()
	{
	}
}
}