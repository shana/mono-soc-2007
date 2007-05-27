using System;
using System.IO;

namespace CBinding
{
	public partial class EditPackagesDialog : Gtk.Dialog
	{
		// TODO: add package version and perhaps an icon?
		private Gtk.TreeStore packageListStore = new Gtk.TreeStore (typeof(bool), typeof(string));
		
		public EditPackagesDialog()
		{
			// TODO: Project packages TreeView
			this.Build();
			
			Gtk.CellRendererToggle crt = new Gtk.CellRendererToggle ();
			crt.Activatable = true;
			crt.Toggled += OnPackageToggled;
			
			packageTreeView.Model = packageListStore;
			packageTreeView.HeadersVisible = true;
			packageTreeView.AppendColumn ("", crt, "active", 0);
			packageTreeView.AppendColumn ("Package", new Gtk.CellRendererText (), "text", 1);
			
			// TODO: Read from all directories in the pkg env variable
			DirectoryInfo di = new DirectoryInfo (@"/usr/lib/pkgconfig");
			FileInfo[] availablePackages = di.GetFiles ("*.pc");
			
			foreach (FileInfo f in availablePackages) {
				// TODO: filter packages
				// TODO: project packages should be true
				string name = f.Name.Substring (0, f.Name.LastIndexOf ('.'));
				packageListStore.AppendValues (false, name);
			}
			
			buttonOk.Clicked += OnOkButtonClick;
			buttonCancel.Clicked += OnCancelButtonClick;
		}
		
		private void OnOkButtonClick (object sender, EventArgs e)
		{
			// TODO: add packeges from project packages tree view to the project itself
			Destroy ();
		}
		
		private void OnCancelButtonClick (object sender, EventArgs e)
		{
			Destroy ();
		}
		
		private void OnPackageToggled (object sender, Gtk.ToggledArgs args)
		{
			Gtk.TreeIter iter;
			
			//TODO: Add or remove package from package project tree view as needed

			if (packageListStore.GetIter (out iter, new Gtk.TreePath (args.Path))) {
				bool old = (bool)packageListStore.GetValue (iter, 0);
				packageListStore.SetValue (iter, 0, !old);
			}
		}
	}
}
