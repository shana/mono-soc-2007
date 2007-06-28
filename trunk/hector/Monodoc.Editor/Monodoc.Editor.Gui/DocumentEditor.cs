//
// DocumentEditor.cs: TextView based class that represent the editor for Monodoc documentation..
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using Gtk;

namespace Monodoc.Editor.Gui {
public class DocumentEditor : TextView {
	public DocumentEditor (TextBuffer buffer) : base (buffer)
	{
		InitializeProperties ();
	}
	
	public DocumentEditor () : base (new DocumentBuffer ())
	{
		InitializeProperties ();
	}
	
	private void InitializeProperties ()
	{
		this.CanFocus = true;
		this.Editable = false;
		this.WrapMode = WrapMode.Word;
	}
}
}