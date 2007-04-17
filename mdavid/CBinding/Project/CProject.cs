using System.CodeDom.Compiler;

using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace CBinding
{
	public class CProject : Project
	{
		public override string ProjectType {
			get { return "C/C++"; }
		}
		
		public override string[] SupportedLanguages {
			get { return new string[] { "C", "C++" }; }
		}
		
		public override bool IsCompileable (string fileName)
		{
			return ((fileName.ToUpper ().EndsWith (".C"))   ||
			        (fileName.ToUpper ().EndsWith (".CPP")) ||
			        (fileName.ToUpper ().EndsWith (".CXX")) ||
			        (fileName.ToUpper ().EndsWith (".C++")));
		}
		
		protected override ICompilerResult DoBuild (IProgressMonitor monitor)
		{
			// TODO: implement
			return new DefaultCompilerResult (new CompilerResults (null), "");
		}
		
		protected override void DoExecute (IProgressMonitor monitor, ExecutionContext context)
		{
			// TODO: implement
		}
		
		public override string GetOutputFileName ()
		{
			// TODO: implement
			return null;
		}
		
		public override IConfiguration CreateConfiguration (string name)
		{
			// TODO: implement
			return null;
		}

	}
}
