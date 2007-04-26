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
		//private CProjectConfiguration configuration;
		//private CCompilationParameters compilationParameters;
		
		public CodeGenerationPanel (IProperties customizationObject)
		{
			this.Build();
		}
		
		public bool Store ()
		{			
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
