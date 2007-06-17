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

namespace Monodoc.Editor.Gui {
public partial class DocumentEditor : TextView {
	public DocumentEditor (TextBuffer buffer) : base (buffer)
	{
	}
	
	public DocumentEditor () : base (new TextBuffer (null))
	{
	}
}
}