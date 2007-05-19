using System;
using System.IO;

namespace CBinding
{
	public partial class AddPathDialog : Gtk.Dialog
	{
		string path;
		
		public AddPathDialog ()
		{
			this.Build ();
			
			Gtk.FileFilter filter = new Gtk.FileFilter ();
			
			filter.AddMimeType ("x-directory/normal");
			filter.Name = "Folders";
			
			filechooserwidget1.SetCurrentFolder ("/usr");
			filechooserwidget1.AddFilter (filter);
			
			buttonOk.Clicked += OnOkButtonClick;
			buttonCancel.Clicked += OnCancelButtonClick;
		}
		
		private void OnOkButtonClick (object sender, EventArgs e)
		{
			path = filechooserwidget1.Filename;
			Destroy ();
		}
		
		private void OnCancelButtonClick (object sender, EventArgs e)
		{
			Destroy ();
		}
		
		public string SelectedPath {
			get { return path; }
		}
	}
}
