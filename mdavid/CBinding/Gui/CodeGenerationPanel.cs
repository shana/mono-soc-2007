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
			
			switch (configuration.CompileTarget)
			{
			case CBinding.CompileTarget.Bin:
				targetCombo.Active = 0;
				break;
			case CBinding.CompileTarget.StaticLibrary:
				targetCombo.Active = 1;
				break;
			case CBinding.CompileTarget.SharedLibrary:
				targetCombo.Active = 2;
				break;
			}
			
			compilationParameters.GenWarnings = warningAllRadio.Active;
			
			if (compilationParameters.Libs != null)
				foreach (string lib in compilationParameters.Libs)
					librariesEntry.Buffer.Text += lib + "\n";
			
			targetCombo.Changed += OnTargetChanged;
			addButton.Clicked += OnLibAdded;
		}
		
		private void OnLibAdded (object sender, EventArgs e)
		{
			if (libToAddEntry.Buffer != null &&
			    libToAddEntry.Buffer.Text != string.Empty)
			{
				librariesEntry.Buffer.Text += libToAddEntry.Buffer.Text + "\n";
			}
		}
		
		private void OnTargetChanged (object sender, EventArgs e)
		{
			switch (targetCombo.ActiveText)
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
		}
		
		public bool Store ()
		{
			if (compilationParameters == null)
				return true;
			
			compilationParameters.GenWarnings = warningAllRadio.Active;
			
			if (compilationParameters.Libs == null)
				compilationParameters.Libs = new ArrayList ();
			
			StringReader libReader = new StringReader (librariesEntry.Buffer.Text);
			string lib;
			while ((lib = libReader.ReadLine()) != null) {
				compilationParameters.Libs.Add (lib);
			}
			libReader.Close ();
			
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
