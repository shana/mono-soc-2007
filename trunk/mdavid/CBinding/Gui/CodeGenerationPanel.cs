using System;
using System.IO;
using System.Collections;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Gui.Dialogs;

namespace CBinding
{
	public partial class CodeGenerationPanel : Gtk.Bin
	{
		//private CProject project;
		private CProjectConfiguration configuration;
		private CCompilationParameters compilationParameters;
		
		public CodeGenerationPanel (IProperties customizationObject)
		{
			this.Build();
			
			//project = (CProject)customizationObject.GetProperty ("Project");
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
			
			foreach (string lib in configuration.Libs) {
				libTextView.Buffer.Text += lib + "\n";
			}
			
			addLibButton.Clicked += OnLibAdded;
			removeLibButton.Clicked += OnLibRemoved;
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
			string lib;
			StringReader reader = new StringReader (libTextView.Buffer.Text);
			Gtk.TextIter start;
			Gtk.TextIter end;
			int line = 0;
			bool found = false;
			
			while ((lib = reader.ReadLine ()) != null) {
				if (lib.Equals (libAddEntry.Text)) {
					found = true;
					break;
				}
				
				line++;
			}
			
			reader.Close ();
			
			start = libTextView.Buffer.GetIterAtLine (line);
			end = libTextView.Buffer.GetIterAtLine (line + 1);
			
			if (found)
				libTextView.Buffer.Delete (ref start, ref end);
		}
		
		public bool Store ()
		{
			if (compilationParameters == null || configuration == null)
				return false;
			
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
			
			string lib;
			StringReader reader = new StringReader (libTextView.Buffer.Text);
			
			configuration.Libs.Clear ();
			
			while ((lib = reader.ReadLine ()) != null)
				configuration.Libs.Add (lib);
			
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
