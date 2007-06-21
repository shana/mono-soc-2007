//
// OpenDocDialog.cs: Dialog to open a XML Mono documenation.
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
public partial class OpenDocDialog : Gtk.Dialog {
	private string filename = String.Empty;
	
	public OpenDocDialog()
	{
		this.Build();
		FileFilter allFiles = new FileFilter ();
		FileFilter xmlFiles = new FileFilter ();
		
		allFiles.AddPattern ("*.*");
		allFiles.Name = "All Files";
		
		xmlFiles.AddPattern ("*.xml");
		xmlFiles.Name = "XML Files";
		
		openFileDialog.AddFilter (xmlFiles);
		openFileDialog.AddFilter (allFiles);
		openFileDialog.SetCurrentFolder ("/usr/src/");
	}
	
	private void OnButtonCancelClicked (object sender, System.EventArgs e)
	{
		Destroy ();
	}
	
	private void OnButtonOkClicked (object sender, System.EventArgs e)
	{
		filename = openFileDialog.Filename;
		Console.WriteLine ("Filename: "  + filename);
		
		if (Directory.Exists (filename))
			openFileDialog.SetCurrentFolder (filename);
		else
			Destroy ();
	}
	
	private void OnOpenFileDialogFileActivated (object sender, System.EventArgs e)
	{
		filename = openFileDialog.Filename;
		
		#if DEBUG
		Console.WriteLine ("Filename: "  + filename);
		#endif
		
		Destroy ();
	}
	
	public string Document {
		get { return filename; }
	}
}
}
