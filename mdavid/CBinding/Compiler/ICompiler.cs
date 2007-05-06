using Mono.Addins;

using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace CBinding
{
	[TypeExtensionPoint ("/CBinding/Compilers")]
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
