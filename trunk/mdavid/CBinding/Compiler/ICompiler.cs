using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace CBinding
{
	public interface ICompiler
	{
		string Name {
			get;
		}
		
		Language Language {
			get;
		}
		
		ICompilerResult Compile (
			ProjectFileCollection projectFiles,
		    ProjectReferenceCollection references,
		    CProjectConfiguration configuration,
		    IProgressMonitor monitor);
	}
}
