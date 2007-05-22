using System;
using System.Collections;

using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui.Pads;

namespace CBinding
{
	public class ProjectNodeExtension : NodeBuilderExtension
	{
		public override bool CanBuildNode (Type dataType)
		{
			return typeof(CProject).IsAssignableFrom (dataType);
		}
		
		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			CProject p = dataObject as CProject;
			
			// FIXME: Correctly move builder so that Packages is on top
			builder.MoveToParent (typeof(CProject));
			builder.AddChild (p.Packages);
		}
	}
}
