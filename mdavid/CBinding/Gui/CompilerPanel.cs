using System;
using System.IO;
using System.Collections;

using MonoDevelop.Core;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Gui.Dialogs;

namespace CBinding
{
	public partial class CompilerPanel : Gtk.Bin
	{
		private CProject project;
		object[] compilers;
		
		public CompilerPanel (IProperties customizationObject)
		{
			this.Build ();
			
			project = (CProject)customizationObject.GetProperty ("Project");
			
			// get compilers
			
			// for some reason this shoots an invalid cast exception
			foreach (ICompiler compiler in compilers) {
				if (compiler.Language == project.Language)
					compilerComboBox.AppendText (compiler.Name);
			}

			
			int active = 0;
			foreach (object compiler in compilerComboBox) {
				if (compiler.Equals (project.Compiler.Name))
					break;
				active++;
			}

			compilerComboBox.Active = active;
		}
		
		public bool Store ()
		{
			if (project == null)
				return false;
			
			if (compilers != null) {
				foreach (CCompiler compiler in compilers) {
					if (compilerComboBox.ActiveText == compiler.Name) {
						project.Compiler = compiler;
						break;
					}
				}
			} else {
				// Use default compiler depending on language.
				project.Compiler = null;
			}

			return true;
		}
	}
	
	public class CompilerPanelBinding : AbstractOptionPanel
	{
		CompilerPanel panel;
		
		public override void LoadPanelContents ()
		{
			panel = new CompilerPanel ((IProperties)CustomizationObject);
			Add (panel);
		}

		
		public override bool StorePanelContents ()
		{
			return panel.Store ();
		}
	}
}
