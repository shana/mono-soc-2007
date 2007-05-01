using System.CodeDom.Compiler;

using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace CBinding
{
	public abstract class CCompiler
	{
		protected string compilerCommand;
		protected string linkerCommand;
		
		public abstract string Name {
			get;
		}
			
		public abstract Language Language {
			get;
		}
		
		public abstract ICompilerResult Compile (
			ProjectFileCollection projectFiles,
		    ProjectReferenceCollection references,
		    CProjectConfiguration configuration,
		    IProgressMonitor monitor);
		    
		protected abstract void ParseCompilerOutput (string errorString, CompilerResults cr);
		
		protected abstract void ParseLinkerOutput (string errorString, CompilerResults cr);
	}
}
