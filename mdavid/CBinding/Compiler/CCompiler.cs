using System.Text;
using System.CodeDom.Compiler;

using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace CBinding
{
	public abstract class CCompiler : ICompiler
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
		
		protected string GeneratePkgArgs (ProjectReferenceCollection references)
		{
			if (references == null || references.Count < 1)
				return string.Empty;
			
			StringBuilder libs = new StringBuilder ();
			
			foreach (ProjectReference pr in references)
				libs.Append (pr.Reference + " ");
			
			return string.Format ("'pkg-config --cflags --libs {0}'", libs.ToString ());
		}
	}
}
