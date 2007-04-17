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
	public class CCompilerManager
	{
		public bool CanCompile (string fileName)
		{
			return ((fileName.ToUpper ().EndsWith (".C"))   ||
			        (fileName.ToUpper ().EndsWith (".CPP")) ||
			        (fileName.ToUpper ().EndsWith (".H"))   ||
			        (fileName.ToUpper ().EndsWith (".HPP")));
		}
		
		public ICompilerResult Compile (ProjectFileCollection projectFiles,
		                                ProjectReferenceCollection references,
		                                DotNetProjectConfiguration configuration,
		                                IProgressMonitor monitor)
		{
			// TODO: current implementation is weak as a twig
			CCompilerParameters parameters = (CCompilerParameters)configuration.CompilationParameters;
			CompilerResults cr = new CompilerResults (new TempFileCollection ());			
			string compiler = parameters.Compiler;
			string outdir = configuration.OutputDirectory;
			StringBuilder args = new StringBuilder ();
			StringWriter output = new StringWriter ();
			StringWriter error = new StringWriter ();
			
			foreach (ProjectFile f in projectFiles)
			{
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
			
			if (parameters.ObjectOnly)
				args.Append ("-c ");
			
			if (parameters.IncludePath != string.Empty)
				args.Append ("-I" + parameters.IncludePath + " ");
			
			if (parameters.LibPath != string.Empty)
				args.Append ("-L" + parameters.LibPath + " ");
			
			if (parameters.Output != string.Empty)
			{
				if (parameters.ObjectOnly)
					args.Append ("-o " + outdir + "/" + parameters.Output + ".o");
				else
					args.Append ("-o " + outdir + "/" + parameters.Output);
			}
			
			DoCompilation (monitor, compiler, args.ToString (), configuration, output, error);
			return new DefaultCompilerResult (cr, "");
		}
		
		// FIXME: make private
		public string GeneratePkgConfigArguments (IProgressMonitor monitor,
		                                           DotNetProjectConfiguration configuration,
		                                           ProjectReferenceCollection references)
		{
			monitor.Log.WriteLine ("Generating pkg-config arguments...");
			
			StringBuilder args = new StringBuilder ();
			
			if (references != null)
			{
				foreach (ProjectReference r in references)
				{
					args.Append(r.Reference + " ");
				}
			}
			
			return args.ToString ();
		}
		
		private bool DoCompilation (IProgressMonitor monitor,
		                            string compiler, string args,
		                            DotNetProjectConfiguration configuration,
		                            TextWriter output, TextWriter error)
		{
			LogTextWriter errorWriter = new LogTextWriter ();
			errorWriter.ChainWriter (monitor.Log);
			errorWriter.ChainWriter (error);
			
			LogTextWriter outputWriter = new LogTextWriter ();
			outputWriter.ChainWriter (monitor.Log);
			outputWriter.ChainWriter (output);
			
			monitor.Log.WriteLine ("Compiling project...");
			
			// TEMP
			monitor.Log.WriteLine ("using command: " + compiler + " " + args);
			
			Process p = Runtime.ProcessService.StartProcess (compiler, args, null, outputWriter, errorWriter, null);
			p.WaitForExit();
			
			return (p.ExitCode == 0);
		}
	}
}
