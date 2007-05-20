using System;

using Mono.Addins;

using MonoDevelop.Components.Commands;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Pads;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;
using MonoDevelop.Ide.Commands;

namespace CBinding
{
	public class ProjectReferencesExtension : NodeBuilderExtension
	{
		public override bool CanBuildNode (Type dataType)
		{
			return typeof(ProjectReferenceCollection).IsAssignableFrom (dataType);
		}
		
		public override void BuildNode (ITreeBuilder builder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			label = "Packages";
		}
	}
	
	public class ProjectReferencesExtensionCommandHandler : NodeCommandHandler
	{
		[CommandHandler (ProjectCommands.AddReference)]
		public void AddReferenceToProject ()
		{
			IdeApp.Services.MessageService.ShowMessage ("Add a package!");
		}
		
		public override void ActivateItem ()
		{
			IdeApp.Services.MessageService.ShowMessage ("Add a package!");
		}

	}
}
