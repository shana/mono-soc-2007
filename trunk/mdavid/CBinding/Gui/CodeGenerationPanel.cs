//
// CodeGenerationPanel.cs: Code generation panel to configure project
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
using System.Collections;

using MonoDevelop.Core;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Gui.Dialogs;

namespace CBinding
{
	public partial class CodeGenerationPanel : Gtk.Bin
	{
		private CProjectConfiguration configuration;
		private CCompilationParameters compilationParameters;
		private Gtk.ListStore libStore = new Gtk.ListStore (typeof(string));
		private Gtk.ListStore libPathStore = new Gtk.ListStore (typeof(string));
		private Gtk.ListStore includePathStore = new Gtk.ListStore (typeof(string));
		
		public CodeGenerationPanel (IProperties customizationObject)
		{
			this.Build ();
			
			configuration = (CProjectConfiguration)customizationObject.GetProperty ("Config");
			compilationParameters = (CCompilationParameters)configuration.CompilationParameters;
			
			libTreeView.Model = libStore;
			libTreeView.HeadersVisible = false;
			libTreeView.AppendColumn ("Library", new Gtk.CellRendererText (), "text", 0);
			
			libPathTreeView.Model = libPathStore;
			libPathTreeView.HeadersVisible = false;
			libPathTreeView.AppendColumn ("Library", new Gtk.CellRendererText (), "text", 0);
			
			includePathTreeView.Model = includePathStore;
			includePathTreeView.HeadersVisible = false;
			includePathTreeView.AppendColumn ("Include", new Gtk.CellRendererText (), "text", 0);
			
			switch (compilationParameters.WarningLevel)
			{
			case WarningLevel.None:
				noWarningRadio.Active = true;
				break;
			case WarningLevel.Normal:
				normalWarningRadio.Active = true;
				break;
			case WarningLevel.All:
				allWarningRadio.Active = true;
				break;
			}
			
			optimizationSpinButton.Value = compilationParameters.OptimizationLevel;
			
			switch (configuration.CompileTarget)
			{
			case CBinding.CompileTarget.Bin:
				targetComboBox.Active = 0;
				break;
			case CBinding.CompileTarget.StaticLibrary:
				targetComboBox.Active = 1;
				break;
			case CBinding.CompileTarget.SharedLibrary:
				targetComboBox.Active = 2;
				break;
			}
			
			extraArgsEntry.Text = compilationParameters.ExtraCompilerArguments;
			
			extraLinkerArgsEntry.Text = compilationParameters.ExtraLinkerArguments;
			
			foreach (string lib in configuration.Libs)
				libStore.AppendValues (lib);
			
			foreach (string libPath in configuration.LibPaths)
				libPathStore.AppendValues (libPath);
			
			foreach (string includePath in configuration.Includes)
				includePathStore.AppendValues (includePath);

			addLibButton.Clicked += OnLibAdded;
			removeLibButton.Clicked += OnLibRemoved;
			libPathAddButton.Clicked += OnLibPathAdded;
			libPathRemoveButton.Clicked += OnLibPathRemoved;
			includePathAddButton.Clicked += OnIncludePathAdded;
			includePathRemoveButton.Clicked += OnIncludePathRemoved;
			browseButton.Clicked += OnBrowseButtonClick;
			includePathBrowseButton.Clicked += OnIncludePathBrowseButtonClick;
			libPathBrowseButton.Clicked += OnLibPathBrowseButtonClick;
		}
		
		private void OnIncludePathAdded (object sender, EventArgs e)
		{
			if (includePathEntry.Text.Length > 0) {				
				includePathStore.AppendValues (includePathEntry.Text);
				includePathEntry.Text = string.Empty;
			}
		}
		
		private void OnIncludePathRemoved (object sender, EventArgs e)
		{
			Gtk.TreeIter iter;
			includePathTreeView.Selection.GetSelected (out iter);
			includePathStore.Remove (ref iter);
		}
		
		private void OnLibPathAdded (object sender, EventArgs e)
		{
			if (libPathEntry.Text.Length > 0) {
				libPathStore.AppendValues (libPathEntry.Text);
				libPathEntry.Text = string.Empty;
			}
		}
		
		private void OnLibPathRemoved (object sender, EventArgs e)
		{
			Gtk.TreeIter iter;
			libPathTreeView.Selection.GetSelected (out iter);
			libPathStore.Remove (ref iter);
		}
		
		private void OnLibAdded (object sender, EventArgs e)
		{
			if (libAddEntry.Text.Length > 0) {				
				libStore.AppendValues (libAddEntry.Text);
				libAddEntry.Text = string.Empty;
			}
		}
		
		private void OnLibRemoved (object sender, EventArgs e)
		{
			Gtk.TreeIter iter;
			libTreeView.Selection.GetSelected (out iter);
			libStore.Remove (ref iter);
		}
		
		private void OnBrowseButtonClick (object sender, EventArgs e)
		{
			AddLibraryDialog dialog = new AddLibraryDialog ();
			dialog.Run ();
			libAddEntry.Text = dialog.Library;
		}
		
		private void OnIncludePathBrowseButtonClick (object sender, EventArgs e)
		{
			AddPathDialog dialog = new AddPathDialog ();
			dialog.Run ();
			includePathEntry.Text = dialog.SelectedPath;
		}
		
		private void OnLibPathBrowseButtonClick (object sender, EventArgs e)
		{
			AddPathDialog dialog = new AddPathDialog ();
			dialog.Run ();
			libPathEntry.Text = dialog.SelectedPath;
		}
		
		public bool Store ()
		{
			if (compilationParameters == null || configuration == null)
				return false;
			
			string line;
			Gtk.TreeIter iter;
			
			if (noWarningRadio.Active)
				compilationParameters.WarningLevel = WarningLevel.None;
			else if (normalWarningRadio.Active)
				compilationParameters.WarningLevel = WarningLevel.Normal;
			else
				compilationParameters.WarningLevel = WarningLevel.All;
			
			compilationParameters.OptimizationLevel = (int)optimizationSpinButton.Value;
			
			switch (targetComboBox.ActiveText)
			{
			case "Executable":
				configuration.CompileTarget = CBinding.CompileTarget.Bin;
				break;
			case "Static Library":
				configuration.CompileTarget = CBinding.CompileTarget.StaticLibrary;
				break;
			case "Shared Object":
				configuration.CompileTarget = CBinding.CompileTarget.SharedLibrary;
				break;
			}
			
			compilationParameters.ExtraCompilerArguments = extraArgsEntry.Text;
			
			compilationParameters.ExtraLinkerArguments = extraLinkerArgsEntry.Text;
			
			libStore.GetIterFirst (out iter);
			configuration.Libs.Clear ();
			while (libStore.IterIsValid (iter)) {
				line = (string)libStore.GetValue (iter, 0);
				configuration.Libs.Add (line);
				libStore.IterNext (ref iter);
			}
			
			libPathStore.GetIterFirst (out iter);
			configuration.LibPaths.Clear ();
			while (libPathStore.IterIsValid (iter)) {
				line = (string)libPathStore.GetValue (iter, 0);
				configuration.LibPaths.Add (line);
				libPathStore.IterNext (ref iter);
			}
			
			includePathStore.GetIterFirst (out iter);
			configuration.Includes.Clear ();
			while (includePathStore.IterIsValid (iter)) {
				line = (string)includePathStore.GetValue (iter, 0);
				configuration.Includes.Add (line);
				includePathStore.IterNext (ref iter);
			}
			
			return true;
		}
	}
	
	public class CodeGenerationPanelBinding : AbstractOptionPanel
	{
		CodeGenerationPanel panel;
		
		public override void LoadPanelContents ()
		{
			panel = new CodeGenerationPanel ((IProperties)CustomizationObject);
			Add (panel);
		}

		
		public override bool StorePanelContents ()
		{
			return panel.Store ();
		}
	}
}
