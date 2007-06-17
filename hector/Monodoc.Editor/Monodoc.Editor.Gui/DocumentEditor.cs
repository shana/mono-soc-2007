//
// DocumentEditor.cs: Main window of the app.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using Gtk;

namespace Monodoc.Editor {
public partial class DocumentEditor : Gtk.TextView {
	public DocumentEditor (Gtk.TextBuffer buffer) : base (buffer)
	{
	}
	
	public DocumentEditor () : base (new Gtk.TextBuffer (null))
	{
	}
}
}