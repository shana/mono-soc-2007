//
// EditorWindow.cs: Main window of the app.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using Gtk;
using Monodoc.EcmaUtils;

namespace Monodoc.Editor {
public partial class EditorWindow : Gtk.Window {
	public EditorWindow () : base (Gtk.WindowType.Toplevel)
	{
		this.Build ();
	}
	
	private void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	private void OnQuitActivated(object sender, System.EventArgs e)
	{
		Application.Quit ();
	}

	private void OnOpenActivated(object sender, System.EventArgs e)
	{
		OpenDocDialog dialog = new OpenDocDialog ();	
		dialog.Run ();
		EcmaReader ecmaReader = new EcmaReader (dialog.Document);
	}
}
}