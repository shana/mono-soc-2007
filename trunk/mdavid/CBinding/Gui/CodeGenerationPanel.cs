using System;
using System.IO;
using System.Collections;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Gui.Dialogs;

namespace CBinding
{
	public partial class CodeGenerationPanel : Gtk.Bin
	{
		private CProjectConfiguration configuration;
		private CCompilationParameters compilationParameters;
		
		public CodeGenerationPanel (IProperties customizationObject)
		{
			this.Build();
			
			configuration = (CProjectConfiguration)customizationObject.GetProperty ("Config");
			compilationParameters = (CCompilationParameters)configuration.CompilationParameters;
			
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
			
			extraArgsEntry.Text = compilationParameters.ExtraArguments;
			
			foreach (string lib in configuration.Libs)
				libTextView.Buffer.Text += lib + "\n";
			
			foreach (string libpath in configuration.LibPaths)
				libPathTextView.Buffer.Text += libpath + "\n";
			
			foreach (string include in configuration.Includes)
				includePathTextView.Buffer.Text += include + "\n";
			
			addLibButton.Clicked += OnLibAdded;
			removeLibButton.Clicked += OnLibRemoved;
			libPathAddButton.Clicked += OnLibPathAdded;
			libPathRemoveButton.Clicked += OnLibPathRemoved;
			includePathAddButton.Clicked += OnIncludePathAdded;
			includePathRemoveButton.Clicked += OnIncludePathRemoved;
		}
		
		private void OnIncludePathAdded (object sender, EventArgs e)
		{
			if (includePathEntry.Text.Length > 0) {
				includePathTextView.Buffer.Text += includePathEntry.Text + "\n";
				includePathEntry.Text = string.Empty;
			}
		}
		
		private void OnIncludePathRemoved (object sender, EventArgs e)
		{
			DeleteLine (includePathEntry.Text, includePathTextView.Buffer);
		}
		
		private void OnLibPathAdded (object sender, EventArgs e)
		{
			if (libPathEntry.Text.Length > 0) {
				libPathTextView.Buffer.Text += libPathEntry.Text + "\n";
				libPathEntry.Text = string.Empty;
			}
		}
		
		private void OnLibPathRemoved (object sender, EventArgs e)
		{
			DeleteLine (libPathEntry.Text, libPathTextView.Buffer);
		}
		
		private void OnLibAdded (object sender, EventArgs e)
		{
			if (libAddEntry.Text.Length > 0) {
				libTextView.Buffer.Text += libAddEntry.Text + "\n";
				libAddEntry.Text = string.Empty;
			}
		}
		
		private void OnLibRemoved (object sender, EventArgs e)
		{
			DeleteLine (libAddEntry.Text, libTextView.Buffer);			
		}
		
		private void DeleteLine (string line, Gtk.TextBuffer buffer)
		{
			StringReader reader = new StringReader (buffer.Text);
			Gtk.TextIter start;
			Gtk.TextIter end;
			string tmpline;
			int lineNum = 0;
			bool found = false;
			
			while ((tmpline = reader.ReadLine ()) != null) {
				if (tmpline.Equals (line)) {
					found = true;
					break;
				}
				
				lineNum++;
			}
			
			reader.Close ();
			
			if (found) {
				start = buffer.GetIterAtLine (lineNum);
				end = buffer.GetIterAtLine (lineNum + 1);
				buffer.Delete (ref start, ref end);
			}
		}
		
		public bool Store ()
		{
			if (compilationParameters == null || configuration == null)
				return false;
			
			string line;
			StringReader reader;
			
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
			
			compilationParameters.ExtraArguments = extraArgsEntry.Text;
			
			reader = new StringReader (libTextView.Buffer.Text);
			configuration.Libs.Clear ();
			while ((line = reader.ReadLine ()) != null)
				configuration.Libs.Add (line);
			reader.Close ();
			
			reader = new StringReader (libPathTextView.Buffer.Text);
			configuration.LibPaths.Clear ();
			while ((line = reader.ReadLine ()) != null)
				configuration.LibPaths.Add (line);
			reader.Close ();
			
			reader = new StringReader (includePathTextView.Buffer.Text);
			configuration.Includes.Clear ();
			while ((line = reader.ReadLine ()) != null)
				configuration.Includes.Add (line);
			reader.Close ();
			
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
