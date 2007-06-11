//
// EditPackagesDialog.cs: Allows you to add and remove pkg-config packages to the project
//
// Authors:
//   Marcos David Marin Amador <MarcosMarin@gmail.com>
//
// Copyright (C) 2007 Marcos David Marin Amador
//
//
// This source code is licenced under The MIT License:
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

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
			
			string pkg_path = Environment.GetEnvironmentVariable ("PKG_CONFIG_PATH");
			string[] dirs = null;
			
			if (pkg_path != null)
				dirs = pkg_path.Split (':');
			
			if (dirs != null && dirs.Length > 0) {
				foreach (string dir in dirs) {
					DirectoryInfo di = new DirectoryInfo (dir);
					FileInfo[] availablePackages = di.GetFiles ("*.pc");
					
					foreach (FileInfo f in availablePackages) {
						// TODO: filter packages
						// TODO: project packages should be true
						string name = f.Name.Substring (0, f.Name.LastIndexOf ('.'));
						packageListStore.AppendValues (false, name);
					}
				}
			} else {
				DirectoryInfo di = new DirectoryInfo (@"/usr/lib/pkgconfig");
				FileInfo[] availablePackages = di.GetFiles ("*.pc");
				
				foreach (FileInfo f in availablePackages) {
					// TODO: filter packages
					// TODO: project packages should be true
					string name = f.Name.Substring (0, f.Name.LastIndexOf ('.'));
					packageListStore.AppendValues (false, name);
				}
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
