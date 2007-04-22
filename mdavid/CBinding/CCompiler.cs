using System.CodeDom.Compiler;

using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace CBinding
{
	public abstract class CCompiler
	{
		public abstract ICompilerResult Compile (
			ProjectFileCollection projectFiles,
		    ProjectReferenceCollection references,
		    CProjectConfiguration configuration,
		    IProgressMonitor monitor);
		    
		protected abstract void ParseOutput (string errorString, CompilerResults cr);
	}
}
