//
// EditorWindow.cs: Main window of the app.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using Gtk;
using System;
using System.IO;
using Monodoc.Editor.Gui;
using Monodoc.Editor.Utils;

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
		if (dialog.Run () == (int) ResponseType.Ok) {
			try {
				MonoDocument doc = new MonoDocument (dialog.Document);
				DocumentBufferArchiver.Deserialize (docEditView.Buffer, doc.Text);
			} catch (ArgumentException argexp) {
				Console.WriteLine (argexp.Message);
			}
		}
		
		dialog.Destroy ();
	}

	private void OnSaveAsActivated (object sender, System.EventArgs e)
	{
		SaveDocDialog dialog = new SaveDocDialog ();
		if (dialog.Run () == (int) ResponseType.Ok) {
			using (FileStream fileStream = new FileStream (dialog.Document, FileMode.CreateNew)) {
				using (StreamWriter streamWriter = new StreamWriter (fileStream)) {
					streamWriter.Write (DocumentBufferArchiver.Serialize (docEditView.Buffer));
				}
			}
		}
		
		dialog.Destroy ();
	}

	private void OnSaveActivated (object sender, System.EventArgs e)
	{
		Console.WriteLine ("Serialize: \n{0}", DocumentBufferArchiver.Serialize (docEditView.Buffer));
	}
}
}