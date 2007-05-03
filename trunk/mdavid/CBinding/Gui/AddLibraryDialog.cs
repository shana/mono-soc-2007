using System;
using System.IO;

namespace CBinding
{
	public partial class AddLibraryDialog : Gtk.Dialog
	{
		private string lib = string.Empty;
		
		public AddLibraryDialog()
		{
			this.Build();
			
			Modal = true;
			
			Gtk.FileFilter libs = new Gtk.FileFilter ();
			Gtk.FileFilter all = new Gtk.FileFilter ();
			
			libs.AddPattern ("*.a");
			libs.Name = "Library";
			
			all.AddPattern ("*.*");
			all.Name = "All Files";
			
			filechooserwidget1.AddFilter (libs);
			filechooserwidget1.AddFilter (all);
			filechooserwidget1.SetCurrentFolder ("/usr/lib");
			
			buttonOk.Clicked += OnOkButtonClick;
			buttonCancel.Clicked += OnCancelButtonClick;
		}
		
		private void OnOkButtonClick (object sender, EventArgs e)
		{
			lib = System.IO.Path.GetFileNameWithoutExtension (
				filechooserwidget1.Filename);
			
			if (lib.StartsWith ("lib"))
				lib = lib.Remove (0, 3);
			
			Destroy ();
		}
		
		private void OnCancelButtonClick (object sender, EventArgs e)
		{
			Destroy ();
		}
		
		public string Library {
			get { return lib; }
		}
	}
}
