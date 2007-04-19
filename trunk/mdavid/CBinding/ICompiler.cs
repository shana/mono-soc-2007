using System.CodeDom.Compiler;

using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace CBinding
{
	public interface ICompiler
	{
		ICompilerResult Compile (ProjectFileCollection projectFiles,
		                                ProjectReferenceCollection references,
		                                CProjectConfiguration configuration,
		                                IProgressMonitor monitor);
		
		void ParseOutput (string errorString, CompilerResults cr);		                                
	}
}
