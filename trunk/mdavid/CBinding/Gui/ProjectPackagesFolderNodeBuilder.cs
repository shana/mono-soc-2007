using System;
using System.Collections;

using MonoDevelop.Components.Commands;
using MonoDevelop.Core.Gui;
using MonoDevelop.Ide.Gui.Pads;

namespace CBinding
{
	public enum CProjectCommands {
		AddPackage
	}
	
	public class ProjectPackagesFolderNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(ArrayList); }
		}
		
		public override Type CommandHandlerType {
			get { return typeof(ProjectPackagesFolderNodeCommandHandler); }
		}
		
		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return "Packages";
		}
		
		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			label = "Packages";
			icon = Context.GetIcon (Stock.OpenReferenceFolder);
			closedIcon = Context.GetIcon (Stock.ClosedReferenceFolder);
		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return ((ArrayList)dataObject).Count > 0;
		}
		
		public override string ContextMenuAddinPath {
			get { return "/CBinding/Views/ProjectBrowser/ContextMenu/PackagesFolderNode"; }
		}
	}
	
	public class ProjectPackagesFolderNodeCommandHandler : NodeCommandHandler
	{
		
		[CommandHandler (CProjectCommands.AddPackage)]
		public void AddPackageToProject ()
		{
			MonoDevelop.Ide.Gui.IdeApp.Services.MessageService.ShowMessage ("Add a package");
		}
	}
}
