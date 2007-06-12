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
		// TODO: add perhaps an icon?
		private Gtk.TreeStore packageListStore = new Gtk.TreeStore (typeof(bool), typeof(string), typeof(string));
		private Gtk.TreeStore selectedPackagesListStore = new Gtk.TreeStore (typeof(string), typeof(string));
		private CProject project;
		
		public EditPackagesDialog(CProject project)
		{
			this.Build();
			
			this.project = project;
			
			Gtk.CellRendererToggle package_toggle = new Gtk.CellRendererToggle ();
			package_toggle.Activatable = true;
			package_toggle.Toggled += OnPackageToggled;
			package_toggle.Xalign = 0;
			
			packageTreeView.Model = packageListStore;
			packageTreeView.HeadersVisible = true;
			packageTreeView.AppendColumn ("", package_toggle, "active", 0);
			packageTreeView.AppendColumn ("Package", new Gtk.CellRendererText (), "text", 1);
			packageTreeView.AppendColumn ("Version", new Gtk.CellRendererText (), "text", 2);
			
			selectedPackagesTreeView.Model = selectedPackagesListStore;
			selectedPackagesTreeView.HeadersVisible = true;
			selectedPackagesTreeView.AppendColumn ("Package", new Gtk.CellRendererText (), "text", 0);
			selectedPackagesTreeView.AppendColumn ("Version", new Gtk.CellRendererText (), "text", 1);
			
			string pkg_path = Environment.GetEnvironmentVariable ("PKG_CONFIG_PATH");
			string[] dirs = null;
			
			if (pkg_path != null)
				dirs = pkg_path.Split (':');
			
			if (dirs != null && dirs.Length > 0) {
				foreach (string dir in dirs) {
					DirectoryInfo di = new DirectoryInfo (dir);
					FileInfo[] availablePackages = di.GetFiles ("*.pc");
					
					foreach (FileInfo f in availablePackages) {
						if (!IsValidPackage (f.FullName)) continue;
						string name = f.Name.Substring (0, f.Name.LastIndexOf ('.'));
						string version = GetPackageVersion (f.FullName);
						bool inProject = IsInProject (name);
						packageListStore.AppendValues (inProject, name, version);
						
						if (inProject)
							selectedPackagesListStore.AppendValues (name, version);
					}
				}
			} else {
				DirectoryInfo di = new DirectoryInfo (@"/usr/lib/pkgconfig");
				FileInfo[] availablePackages = di.GetFiles ("*.pc");
				
				foreach (FileInfo f in availablePackages) {
					if (!IsValidPackage (f.FullName)) continue;
					string name = f.Name.Substring (0, f.Name.LastIndexOf ('.'));
					string version = GetPackageVersion (f.FullName);
					bool inProject = IsInProject (name);
					packageListStore.AppendValues (inProject, name, version);
					
					if (inProject)
						selectedPackagesListStore.AppendValues (name, version);
				}
			}
			
			buttonOk.Clicked += OnOkButtonClick;
			buttonCancel.Clicked += OnCancelButtonClick;
		}
		
		private void OnOkButtonClick (object sender, EventArgs e)
		{
			project.Packages.Clear ();
			
			Gtk.TreeIter iter;
			
			bool has_elem = selectedPackagesListStore.GetIterFirst (out iter);
			while (has_elem) {
				string package = (string)selectedPackagesListStore.GetValue (iter, 0);
				project.Packages.Add (new Package(package));
				if (!selectedPackagesListStore.IterNext (ref iter)) break;
			}
			
			Destroy ();
		}
		
		private void OnCancelButtonClick (object sender, EventArgs e)
		{
			Destroy ();
		}
		
		private void OnPackageToggled (object sender, Gtk.ToggledArgs args)
		{
			Gtk.TreeIter iter;
			bool old = true;
			string name;
			string version;

			if (packageListStore.GetIter (out iter, new Gtk.TreePath (args.Path))) {
				old = (bool)packageListStore.GetValue (iter, 0);
				packageListStore.SetValue (iter, 0, !old);
			}
			
			name = (string)packageListStore.GetValue (iter, 1);
			version = (string)packageListStore.GetValue(iter, 2);
			
			if (old == false) {
				selectedPackagesListStore.AppendValues (name, version);
			} else {
				Gtk.TreeIter search_iter;
				bool has_elem = selectedPackagesListStore.GetIterFirst (out search_iter);
				
				if (has_elem)
				{
					while (true) {
						string current = (string)selectedPackagesListStore.GetValue (search_iter, 0);
						Console.WriteLine (current);
						
						if (current.Equals (name)) {
							selectedPackagesListStore.Remove (ref search_iter);
							break;
						}
						
						if (!selectedPackagesListStore.IterNext (ref search_iter))
							break;
					}
				}
			}
		}
		
		private string GetPackageVersion (string package)
		{
			StreamReader reader = new StreamReader (package);
			
			string line;
			string version = string.Empty;
			
			while ((line = reader.ReadLine ()) != null) {
				if (line.StartsWith ("Version:")) {
					version = line.Split(':')[1].TrimStart ();
				}
			}
			
			reader.Close ();
			
			return version;
		}
		
		private bool IsValidPackage (string package)
		{
			bool valid = false;
			StreamReader reader = new StreamReader (package);
			
			string line;
			
			while ((line = reader.ReadLine ()) != null) {
				if (line.StartsWith ("Cflags:")) {
					valid = true;
					break;
				}
			}
			reader.Close ();
			
			return valid;
		}
		
		private bool IsInProject (string package)
		{
			bool exists = false;
			
			foreach (Package p in project.Packages) {
				if (package.Equals (p.Name)) {
					exists = true;
					break;
				}
			}
			
			return exists;
		}
	}
}
