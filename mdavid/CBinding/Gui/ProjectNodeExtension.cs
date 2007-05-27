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
			
			// FIXME: Add child on top of other nodes
			builder.AddChild (p.Packages);
		}
	}
}
