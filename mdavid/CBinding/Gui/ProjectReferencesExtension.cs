using System;

using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui.Pads;

namespace CBinding
{
	public class ProjectReferencesExtension : NodeBuilderExtension
	{
		public override bool CanBuildNode (Type dataType)
		{
			return typeof(ProjectReferenceCollection).IsAssignableFrom (dataType);
		}
		
		public override void GetNodeAttributes (ITreeNavigator parentNode, object dataObject, ref NodeAttributes attributes)
		{
			attributes |= NodeAttributes.Hidden;
		}
	}
}
