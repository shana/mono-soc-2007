using System;

using Mono.Addins;

using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui.Pads;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;

namespace CBinding
{
	public class ProjectReferencesExtension : NodeBuilderExtension
	{
		public override bool CanBuildNode (Type dataType)
		{
			// Just for testing purposes...
			return true;
		}
		
		// Can't figure out why this doesn't work
		public override void BuildNode (ITreeBuilder builder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			// TEST
			MonoDevelop.Ide.Gui.IdeApp.Services.MessageService.ShowMessage ("This code was called!");
			label = "pretty label please change";
		}
	}
}
