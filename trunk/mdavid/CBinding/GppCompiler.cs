using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.CodeDom.Compiler;

using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Core.Gui.Components;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui;

namespace CBinding
{
	public class GppCompiler : CCompiler
	{		
		public override ICompilerResult Compile (
			ProjectFileCollection projectFiles,
		    ProjectReferenceCollection references,
		    CProjectConfiguration configuration,
		    IProgressMonitor monitor)
		{
			StringBuilder args = new StringBuilder ();
			CompilerResults cr = new CompilerResults (new TempFileCollection ());
			bool res = false;
			string outputName = configuration.OutputDirectory + "/" +
				configuration.CompiledOutputName;
			CCompilationParameters cp =
				(CCompilationParameters)configuration.CompilationParameters;
			
			if (cp.GenWarnings)
				args.Append ("-Wall ");
			
			foreach (ProjectFile f in projectFiles) {
				if (f.BuildAction == BuildAction.Compile)
					res = DoCompilation (f, args.ToString (), monitor, cr);
				else
					res = true;
				
				if (!res) break;
			}
			
			if (res) {
				switch (configuration.CompileTarget)
				{
				case CBinding.CompileTarget.Bin:
					MakeBin (
						projectFiles, monitor, outputName);
					break;
				case CBinding.CompileTarget.StaticLibrary:
					MakeStaticLibrary (
						projectFiles, monitor, outputName);
					break;
				case CBinding.CompileTarget.SharedLibrary:
					MakeSharedLibrary (
						projectFiles, monitor, outputName);
					break;
				}
			}
			
			return new DefaultCompilerResult (cr, "");
		}
		
		private void MakeBin(ProjectFileCollection projectFiles,
		                     IProgressMonitor monitor, string outputName)
		{
			string objectFiles = ObjectFiles (projectFiles);
			
			monitor.Log.WriteLine ("Generating binary...");
			
			Process p = Runtime.ProcessService.StartProcess (
				"g++", "-o " + outputName + " " + objectFiles,
				null, null);
			p.WaitForExit ();
		}
		
		private void MakeStaticLibrary (ProjectFileCollection projectFiles,
		                                IProgressMonitor monitor, string outputName)
		{
			string objectFiles = ObjectFiles (projectFiles);
			
			monitor.Log.WriteLine ("Generating static library...");
			
			Process p = Runtime.ProcessService.StartProcess (
				"ar", "rcs " + outputName + " " + objectFiles,
				null, null);
			p.WaitForExit ();
		}
		
		private void MakeSharedLibrary(ProjectFileCollection projectFiles,
		                               IProgressMonitor monitor, string outputName)
		{
			string objectFiles = ObjectFiles (projectFiles);
			
			monitor.Log.WriteLine ("Generating shared object...");
			
			Process p = Runtime.ProcessService.StartProcess (
				"ld", "-shared -o " + outputName + " " + objectFiles,
				null, null);
			p.WaitForExit ();
		}
		
		///<summary>
		///Compiles a source file into object code
		///</summary>
		private bool DoCompilation (ProjectFile file, string args,
		                            IProgressMonitor monitor,
		                            CompilerResults cr)
		{
			StringWriter error = new StringWriter ();
			LogTextWriter chainedError = new LogTextWriter ();
			chainedError.ChainWriter (monitor.Log);
			chainedError.ChainWriter (error);
			
			string outputName = file.Name.Substring (
				0, file.Name.LastIndexOf (".")) + ".o";
			
			Process p = Runtime.ProcessService.StartProcess (
				"g++", file.Name + " " + args + "-c -o " + outputName,
				null, null, error, null);
				
			p.WaitForExit ();
			ParseOutput (error.ToString (), cr);
			
			return p.ExitCode == 0;
		}
		
		private string ObjectFiles (ProjectFileCollection projectFiles)
		{
			StringBuilder objectFiles = new StringBuilder ();
			
			foreach (ProjectFile f in projectFiles) {
				if (f.BuildAction == BuildAction.Compile) {
					objectFiles.Append (
						f.Name.Substring (
							0, f.Name.LastIndexOf (".")) + ".o ");
				}
			}
			
			return objectFiles.ToString ();
		}
		
		protected override void ParseOutput (string errorString, CompilerResults cr)
		{
			TextReader reader = new StringReader (errorString);
			string next;
			
			while ((next = reader.ReadLine ()) != null) {
				CompilerError error = CreateErrorFromErrorString (next);
				if (error != null)
					cr.Errors.Add (error);
			}
			
			reader.Close ();
		}
		
		private CompilerError CreateErrorFromErrorString (string errorString)
		{
			CompilerError error = new CompilerError ();
			string[] split;
			
			split = errorString.Split(new char[] { ':' });
			
			if (split.Length < 4)
				return null;
			
			if (split.Length == 4) {
				error.FileName = split[0];
				error.Line = int.Parse (split[1]);
				error.IsWarning = split[2].Equals ("warning");
				error.ErrorText = split[3];
			} else {
				error.FileName = split[0];
				error.Line = int.Parse (split[1]);
				error.Column = int.Parse (split[2]);
				error.IsWarning = split[3].Contains ("warning");
				error.ErrorText = split[4];
			}
			
			return error;
		}
	}
}
