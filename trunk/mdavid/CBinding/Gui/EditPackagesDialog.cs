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
using System.Collections.Generic;

using Mono.Addins;

namespace CBinding
{
	public partial class EditPackagesDialog : Gtk.Dialog
	{
		private Gtk.ListStore packageListStore = new Gtk.ListStore (typeof(bool), typeof(string), typeof(string));
		private Gtk.ListStore selectedPackagesListStore = new Gtk.ListStore (typeof(string), typeof(string));
		private CProject project;
		
		public EditPackagesDialog(CProject project)
		{
			this.Build();
			
			this.project = project;
			
			Gtk.CellRendererToggle toggleRenderer = new Gtk.CellRendererToggle ();
			toggleRenderer.Activatable = true;
			toggleRenderer.Toggled += OnPackageToggled;
			toggleRenderer.Xalign = 0;
			
			Gtk.CellRendererText textRenderer = new Gtk.CellRendererText ();
			
			Gtk.CellRendererPixbuf pixbufRenderer = new Gtk.CellRendererPixbuf ();
			pixbufRenderer.StockId = "md-package";
			
			Gtk.TreeViewColumn packagePackageColumn = new Gtk.TreeViewColumn ();
			packagePackageColumn.Title = "Package";
			packagePackageColumn.PackStart (pixbufRenderer, false);
			packagePackageColumn.PackStart (textRenderer, true);
			packagePackageColumn.AddAttribute (textRenderer, "text", 1);
			
			packageTreeView.Model = packageListStore;
			packageTreeView.HeadersVisible = true;
			packageTreeView.AppendColumn ("", toggleRenderer, "active", 0);
			packageTreeView.AppendColumn (packagePackageColumn);
			packageTreeView.AppendColumn ("Version", textRenderer, "text", 2);
			
			Gtk.TreeViewColumn selectedPackageColumn = new Gtk.TreeViewColumn ();
			selectedPackageColumn.Title = "Package";
			selectedPackageColumn.PackStart (pixbufRenderer, false);
			selectedPackageColumn.PackStart (textRenderer, true);
			selectedPackageColumn.AddAttribute (textRenderer, "text", 0);
			
			selectedPackagesTreeView.Model = selectedPackagesListStore;
			selectedPackagesTreeView.HeadersVisible = true;
			selectedPackagesTreeView.AppendColumn (selectedPackageColumn);
			selectedPackagesTreeView.AppendColumn ("Version", textRenderer, "text", 1);
			
			foreach (string dir in ScanDirs ()) {
				if (Directory.Exists (dir)) {	
					DirectoryInfo di = new DirectoryInfo (dir);
					FileInfo[] availablePackages = di.GetFiles ("*.pc");
					
					foreach (FileInfo f in availablePackages) {
						if (!IsValidPackage (f.FullName)) { 
							continue;
						}
						string name = f.Name.Substring (0, f.Name.LastIndexOf ('.'));
						string version = GetPackageVersion (f.FullName);
						bool inProject = IsInProject (name);
						packageListStore.AppendValues (inProject, name, version);
						
						if (inProject)
							selectedPackagesListStore.AppendValues (name, version);
					}
				}
			}
			
			buttonOk.Clicked += OnOkButtonClick;
			buttonCancel.Clicked += OnCancelButtonClick;
			removeButton.Clicked += OnRemoveButtonClick;
		}
		
		private string[] ScanDirs ()
		{
			List<string> dirs = new List<string> ();
			string pkg_var = Environment.GetEnvironmentVariable ("PKG_CONFIG_PATH");
			string[] pkg_paths;
			
			dirs.Add ("/usr/lib/pkgconfig");
			dirs.Add ("/usr/share/pkgconfig");
			dirs.Add ("/usr/local/lib/pkgconfig");
			dirs.Add ("/usr/local/share/pkgconfig");
			
			if (pkg_var == null) return dirs.ToArray ();
			
			pkg_paths = pkg_var.Split (':');
			
			foreach (string dir in pkg_paths) {
				if (!dirs.Contains (dir) && !string.IsNullOrEmpty (dir)) {
					dirs.Add (dir);
				}
			}
			
			return dirs.ToArray ();
		}
		
		private void OnOkButtonClick (object sender, EventArgs e)
		{
			project.Packages.Clear ();
			
			Gtk.TreeIter iter;
			
			bool has_elem = selectedPackagesListStore.GetIterFirst (out iter);
			while (has_elem) {
				string package = (string)selectedPackagesListStore.GetValue (iter, 0);
				project.Packages.Add (new ProjectPackage(package));
				if (!selectedPackagesListStore.IterNext (ref iter)) break;
			}
			
			Destroy ();
		}
		
		private void OnCancelButtonClick (object sender, EventArgs e)
		{
			Destroy ();
		}
		
		private void OnRemoveButtonClick (object sender, EventArgs e)
		{
			Gtk.TreeIter iter;
			
			selectedPackagesTreeView.Selection.GetSelected (out iter);
			
			if (!selectedPackagesListStore.IterIsValid (iter)) return;
			
			string package = (string)selectedPackagesListStore.GetValue (iter, 0);
			
			selectedPackagesListStore.Remove (ref iter);
			
			Gtk.TreeIter search_iter;
			bool has_elem = packageListStore.GetIterFirst (out search_iter);
				
			if (has_elem)
			{
				while (true) {
					string current = (string)packageListStore.GetValue (search_iter, 1);
					
					if (current.Equals (package)) {
						packageListStore.SetValue (search_iter, 0, false);
						break;
					}
					
					if (!packageListStore.IterNext (ref search_iter))
						break;
				}
			}
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
				if (line.StartsWith ("Cflags:", true, null)) {
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
			
			foreach (ProjectPackage p in project.Packages) {
				if (package.Equals (p.File)) {
					exists = true;
					break;
				}
			}
			
			return exists;
		}
	}
}
