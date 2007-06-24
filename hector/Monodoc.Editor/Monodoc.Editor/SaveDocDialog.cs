//
// SaveDocDialog.cs: Dialog to save a XML Mono documenation.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using System.IO;
using Gtk;

namespace Monodoc.Editor {
public partial class SaveDocDialog : Gtk.Dialog {
	private string filename = String.Empty;
	
	public SaveDocDialog()
	{
		this.Build();
		saveFileDialog.SetCurrentFolder (Environment.CurrentDirectory);
		saveFileDialog.DoOverwriteConfirmation = true;
	}
	
	private void OnButtonCancelClicked (object sender, System.EventArgs e)
	{
	}
	
	private void OnButtonOkClicked (object sender, System.EventArgs e)
	{
		filename = saveFileDialog.Filename;
		if (filename != null)
			Respond (ResponseType.Ok);
	}
	
	private void OnSaveFileDialogFileActivated (object sender, System.EventArgs e)
	{
		filename = saveFileDialog.Filename;
		Respond (ResponseType.Ok);
	}
	
	public string Document {
		get { return filename; }
	}
}
}
