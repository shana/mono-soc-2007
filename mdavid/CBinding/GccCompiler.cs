// **** REMAKE ***

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
	public class GccCompiler : ICompiler
	{		
		public ICompilerResult Compile (ProjectFileCollection projectFiles,
		                                ProjectReferenceCollection references,
		                                CProjectConfiguration configuration,
		                                IProgressMonitor monitor)
		{
			CCompilationParameters parameters =
				(CCompilationParameters)configuration.CompilationParameters;
			
			CompilerResults cr = new CompilerResults (new TempFileCollection ());			
			string compiler = "gcc";
			string outdir = configuration.OutputDirectory;
			string outputName = configuration.CompiledOutputName;
			bool res;
			StringBuilder args = new StringBuilder ();
			StringWriter output = new StringWriter ();
			StringWriter error = new StringWriter ();
			
			foreach (ProjectFile f in projectFiles) {
				if (f.Subtype != Subtype.Directory) {
					switch (f.BuildAction)
					{
					case BuildAction.Compile:
						args.Append (f.Name + " ");
						break;
					}
				}
			}
			
			if (parameters.GenWarnings)
				args.Append ("-Wall ");

			if (parameters.IncludePath != string.Empty)
				args.Append ("-I" + parameters.IncludePath + " ");
			
			if (parameters.LibPath != string.Empty)
				args.Append ("-L" + parameters.LibPath + " ");
			
			switch (configuration.CompileTarget)
			{
			case CBinding.CompileTarget.Bin:
				args.Append ("-o " + outdir + "/" + outputName);
				break;
			case CBinding.CompileTarget.SharedLibrary:
				args.Append ("-c ");
				break;
			case CBinding.CompileTarget.StaticLibrary:
				args.Append ("-c ");
				break;
			}
			
			res = DoCompilation (
				monitor, configuration, compiler, args.ToString (), output, error);
				
			monitor.Log.WriteLine ("res = {0}", res.ToString ());
				
			ParseOutput (error.ToString (), cr);
			
			if (res &&
			    configuration.CompileTarget == CBinding.CompileTarget.StaticLibrary) {
				CompileToStaticLibrary (
					projectFiles, monitor, configuration, output, error);
			}
			
			return new DefaultCompilerResult (cr, "");
		}
		
		private void CompileToStaticLibrary (ProjectFileCollection projectFiles,
		                                     IProgressMonitor monitor,
		                                     CProjectConfiguration configuration,
		                                     TextWriter output, TextWriter error)
		{
			string workingdir = configuration.OutputDirectory;
			LogTextWriter errorWriter = new LogTextWriter ();
			errorWriter.ChainWriter (monitor.Log);
			errorWriter.ChainWriter (error);
			
			LogTextWriter outputWriter = new LogTextWriter ();
			outputWriter.ChainWriter (monitor.Log);
			outputWriter.ChainWriter (output);
			
			StringBuilder ofiles = new StringBuilder ();
			
			foreach (ProjectFile f in projectFiles) {
				if (f.BuildAction == BuildAction.Compile) {
					int index = f.Name.LastIndexOf (".");
					ofiles.Append (f.Name.Substring (0, index) + ".o ");
				}
			}
			
			monitor.Log.WriteLine ("Generating static library...");
			
			//TEMP
			monitor.Log.WriteLine ("working dir: " + workingdir);
			monitor.Log.WriteLine (
				"Using Command: ar rcs " + configuration.CompiledOutputName +
				" " + ofiles);
			
			Process p = Runtime.ProcessService.StartProcess (
				"ar", "rcs " + configuration.CompiledOutputName + " " + ofiles,
				workingdir, outputWriter, errorWriter, null);
			
			p.WaitForExit();
		}
		
//		private void CompileToSharedLibrary (IProgressMonitor monitor,
//		                                     CProjectConfiguration configuration,
//		                                     TextWriter output, TextWriter error)
//		{
//		}
		
		// FIXME: make private
		public string GeneratePkgConfigArguments (IProgressMonitor monitor,
		                                          CProjectConfiguration configuration,
		                                          ProjectReferenceCollection references)
		{
			monitor.Log.WriteLine ("Generating pkg-config arguments...");
			
			StringBuilder args = new StringBuilder ();
			
			if (references != null)
				foreach (ProjectReference r in references)
					args.Append(r.Reference + " ");
			
			return args.ToString ();
		}
		
		private bool DoCompilation (IProgressMonitor monitor,
		                            CProjectConfiguration configuration,
		                            string compiler, string args,
		                            TextWriter output, TextWriter error)
		{	
			string workingdir = configuration.OutputDirectory;
			// Go up two levels
			workingdir = workingdir.Substring (0, workingdir.LastIndexOf ("/"));
			workingdir = workingdir.Substring (0, workingdir.LastIndexOf ("/"));
			
			LogTextWriter errorWriter = new LogTextWriter ();
			errorWriter.ChainWriter (monitor.Log);
			errorWriter.ChainWriter (error);
			
			LogTextWriter outputWriter = new LogTextWriter ();
			outputWriter.ChainWriter (monitor.Log);
			outputWriter.ChainWriter (output);
			
			monitor.Log.WriteLine ("Compiling project...");
			
			// TEMP
			monitor.Log.WriteLine ("working dir: " + workingdir);
			monitor.Log.WriteLine ("using command: " + compiler + " " + args);
			
			Process p = Runtime.ProcessService.StartProcess (
				compiler, args, workingdir, outputWriter, errorWriter, null);
			p.WaitForExit();
			
			return (p.ExitCode == 0);
		}
		
		public void ParseOutput (string errorString, CompilerResults cr)
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
				error.IsWarning = split[3].Equals ("warning");
				error.ErrorText = split[4];
			}
			
			return error;
		}
	}
}
