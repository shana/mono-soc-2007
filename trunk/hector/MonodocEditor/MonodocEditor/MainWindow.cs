// /home/hector/Projects/mono-soc-2007/hector/MonodocEditor/MonodocEditor/MainWindow.cs created with MonoDevelop
// User: hector at 7:38 AM 5/31/2007
//
//
//
using System;
using Gtk;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}