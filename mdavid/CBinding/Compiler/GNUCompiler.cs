//
// GNUCompiler.cs: Provides most functionality to compile using a GNU compiler (gcc and g++)
//
// Authors:
//   Marcos David Marin Amador <MarcosMarin@gmail.com>
//
// Copyright (C) 2007 Marcos David Marin Amador
//
//
// This source code is licenced under The MIT License:
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.CodeDom.Compiler;

using Mono.Addins;

using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Core.Gui.Components;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui;

namespace CBinding
{
	public abstract class GNUCompiler : CCompiler
	{
		public override ICompilerResult Compile (
			ProjectFileCollection projectFiles,
		    ProjectPackageCollection packages,
		    CProjectConfiguration configuration,
		    IProgressMonitor monitor)
		{
			StringBuilder args = new StringBuilder ();
			CompilerResults cr = new CompilerResults (new TempFileCollection ());
			bool res = false;
			
			string outputName = string.Format ("{0}/{1}",
			    configuration.OutputDirectory,
			    configuration.CompiledOutputName);
			
			CCompilationParameters cp =
				(CCompilationParameters)configuration.CompilationParameters;
			
			switch (cp.WarningLevel)
			{
			case WarningLevel.None:
				args.Append ("-w ");
				break;
			case WarningLevel.Normal:
				// nothing
				break;
			case WarningLevel.All:
				args.Append ("-Wall ");
				break;
			}
			
			args.Append ("-O" + cp.OptimizationLevel + " ");
			
			if (cp.ExtraCompilerArguments != null && cp.ExtraCompilerArguments.Length > 0)
				args.Append (cp.ExtraCompilerArguments + " ");
			
			if (cp.DefineSymbols != null && cp.DefineSymbols.Length > 0)
				args.Append (ProcessDefineSymbols (cp.DefineSymbols) + " ");
			
			if (configuration.Includes != null)
				foreach (string inc in configuration.Includes)
					args.Append ("-I" + inc + " ");
			
			foreach (ProjectFile f in projectFiles) {
				if (f.Subtype == Subtype.Directory) continue;
				
				if (f.BuildAction == BuildAction.Compile) {
					bool compile = false;
					string objectFile = Path.ChangeExtension (f.Name, ".o");
					
					if (!File.Exists (objectFile))
						compile = true;
					else if (File.GetLastWriteTime (objectFile) < File.GetLastWriteTime (f.Name))
					    compile = true;
					
					if (compile)
						res = DoCompilation (f, args.ToString (), packages, monitor, cr);
				}
				else
					res = true;
				
				if (!res) break;
			}

			if (res) {
				switch (configuration.CompileTarget)
				{
				case CBinding.CompileTarget.Bin:
					MakeBin (
						projectFiles, packages, configuration, cr, monitor, outputName);
					break;
				case CBinding.CompileTarget.StaticLibrary:
					MakeStaticLibrary (
						projectFiles, monitor, outputName);
					break;
				case CBinding.CompileTarget.SharedLibrary:
					MakeSharedLibrary (
						projectFiles, packages, configuration, cr, monitor, outputName);
					break;
				}
			}
			
			return new DefaultCompilerResult (cr, "");
		}
		
		private void MakeBin(ProjectFileCollection projectFiles,
		                     ProjectPackageCollection packages,
		                     CProjectConfiguration configuration,
		                     CompilerResults cr,
		                     IProgressMonitor monitor, string outputName)
		{
			if (!NeedsUpdate (projectFiles, outputName)) return;
			
			string objectFiles = StringArrayToSingleString (ObjectFiles (projectFiles));
			string pkgargs = GeneratePkgLinkerArgs (packages);
			StringBuilder args = new StringBuilder ();
			CCompilationParameters cp =
				(CCompilationParameters)configuration.CompilationParameters;
			
			if (cp.ExtraLinkerArguments != null && cp.ExtraLinkerArguments.Length > 0)
				args.Append (cp.ExtraLinkerArguments + " ");
			
			if (configuration.LibPaths != null)
				foreach (string libpath in configuration.LibPaths)
					args.Append ("-L" + libpath + " ");
			
			if (configuration.Libs != null)
				foreach (string lib in configuration.Libs)
					args.Append ("-l" + lib + " ");
			
			monitor.Log.WriteLine ("Generating binary...");
			
			string command = string.Format ("{0} -o {1} {2} {3} {4}",
			    linkerCommand, outputName, objectFiles, args.ToString (), pkgargs);
			
			monitor.Log.WriteLine ("using: " + command);
			
			ProcessWrapper p = Runtime.ProcessService.StartProcess (
			    "bash", null, null, (ProcessEventHandler)null, null, null, true);
			
			p.StandardInput.WriteLine (command);
			p.StandardInput.Close ();
			p.WaitForExit ();
			
			string line;
			StringWriter error = new StringWriter ();
			
			while ((line = p.StandardError.ReadLine ()) != null)
				error.WriteLine (line);
			
			monitor.Log.WriteLine (error.ToString ());
			
			ParseCompilerOutput (error.ToString (), cr);
			
			error.Close ();
			
			ParseLinkerOutput (error.ToString (), cr);
		}
		
		private void MakeStaticLibrary (ProjectFileCollection projectFiles,
		                                IProgressMonitor monitor, string outputName)
		{
			if (!NeedsUpdate (projectFiles, outputName)) return;
			
			string objectFiles = StringArrayToSingleString (ObjectFiles (projectFiles));
			
			monitor.Log.WriteLine ("Generating static library...");
			
			Process p = Runtime.ProcessService.StartProcess (
				"ar", "rcs " + outputName + " " + objectFiles,
				null, null);
			p.WaitForExit ();
		}
		
		private void MakeSharedLibrary(ProjectFileCollection projectFiles,
		                               ProjectPackageCollection packages,
		                               CProjectConfiguration configuration,
		                               CompilerResults cr,
		                               IProgressMonitor monitor, string outputName)
		{
			if (!NeedsUpdate (projectFiles, outputName)) return;
			
			string objectFiles = StringArrayToSingleString (ObjectFiles (projectFiles));
			string pkgargs = GeneratePkgLinkerArgs (packages);
			StringBuilder args = new StringBuilder ();
			CCompilationParameters cp =
				(CCompilationParameters)configuration.CompilationParameters;
			
			if (cp.ExtraLinkerArguments != null && cp.ExtraLinkerArguments.Length > 0)
				args.Append (cp.ExtraLinkerArguments + " ");
			
			if (configuration.LibPaths != null)
				foreach (string libpath in configuration.LibPaths)
					args.Append ("-L" + libpath + " ");
			
			if (configuration.Libs != null)
				foreach (string lib in configuration.Libs)
					args.Append ("-l" + lib + " ");
			
			monitor.Log.WriteLine ("Generating shared object...");
			
			string command = string.Format ("{0} -shared -o {1} {2} {3} {4}",
			    linkerCommand, outputName, objectFiles, args.ToString (), pkgargs);
			
			monitor.Log.WriteLine ("using: " + command);
			
			ProcessWrapper p = Runtime.ProcessService.StartProcess (
			    "bash", null, null, (ProcessEventHandler)null, null, null, true);
			
			p.StandardInput.WriteLine (command);
			p.StandardInput.Close ();
			p.WaitForExit ();
			
			string line;
			StringWriter error = new StringWriter ();
			
			while ((line = p.StandardError.ReadLine ()) != null)
				error.WriteLine (line);
			
			monitor.Log.WriteLine (error.ToString ());
			
			ParseCompilerOutput (error.ToString (), cr);
			
			error.Close ();
			
			ParseLinkerOutput (error.ToString (), cr);
		}
		
		private string ProcessDefineSymbols (string symbols)
		{
			StringBuilder processed = new StringBuilder (symbols);
			
			// Take care of multi adyacent spaces
			for (int i = 0; i < processed.Length; i++) {
				if (i + 1 < processed.Length &&
				    processed[i] == ' ' &&
				    processed[i + 1] == ' ') {
					processed.Remove (i--, 1);
				}
			}
			
			return processed.ToString ()
				            .Trim ()
				            .Replace (" ", " -D")
				            .Insert (0, "-D");
		}
		
		/// <summary>
		/// Compiles a source file into object code
		/// </summary>
		private bool DoCompilation (ProjectFile file, string args,
		                            ProjectPackageCollection packages,
		                            IProgressMonitor monitor,
		                            CompilerResults cr)
		{			
			string outputName = Path.ChangeExtension (file.Name, ".o");
			string pkgargs = GeneratePkgCompilerArgs (packages);
			
			string command = String.Format("{0} {1} {2} -c -o {3} {4}",
			    compilerCommand, file.Name, args,outputName, pkgargs);
			
			monitor.Log.WriteLine ("using: " + command);
			
			ProcessWrapper p = Runtime.ProcessService.StartProcess (
			    "bash", null, null, (ProcessEventHandler)null, null, null, true);
			
			p.StandardInput.WriteLine (command);
			p.StandardInput.Close ();
			p.WaitForExit ();
			
			string line;
			StringWriter error = new StringWriter ();
			
			while ((line = p.StandardError.ReadLine ()) != null)
				error.WriteLine (line);
			
			monitor.Log.WriteLine (error.ToString ());
			
			ParseCompilerOutput (error.ToString (), cr);
			
			error.Close ();
			
			return p.ExitCode == 0;
		}
		
		private string[] ObjectFiles (ProjectFileCollection projectFiles)
		{
			List<string> objectFiles = new List<string> ();
			
			foreach (ProjectFile f in projectFiles) {
				if (f.BuildAction == BuildAction.Compile) {
					objectFiles.Add (Path.ChangeExtension (f.Name, ".o"));
				}
			}
			
			return objectFiles.ToArray ();
		}
		
		private string StringArrayToSingleString (string[] array)
		{
			StringBuilder str = new StringBuilder ();
			
			foreach (string s in array) {
				str.Append (s + " ");
			}
			
			return str.ToString ();
		}
		
		private bool NeedsUpdate (ProjectFileCollection projectFiles,
		                          string target)
		{
			if (!File.Exists (target))
				return true;
			
			foreach (string obj in ObjectFiles (projectFiles))
				if (File.Exists (obj) &&
				    File.GetLastWriteTime (obj) > File.GetLastWriteTime (target))
					return true;
			
			return false;
		}
		
		protected override void ParseCompilerOutput (string errorString, CompilerResults cr)
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
		
		private static Regex withColRegex = new Regex (
		    @"^\s*(?<file>.*):(?<line>\d*):(?<column>\d*):\s*(?<level>.*)\s*:\s(?<message>.*)",
		    RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		private static Regex noColRegex = new Regex (
		    @"^\s*(?<file>.*):(?<line>\d*):\s*(?<level>.*)\s*:\s(?<message>.*)",
		    RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		
		private CompilerError CreateErrorFromErrorString (string errorString)
		{
			CompilerError error = new CompilerError ();
			
			Match withColMatch = withColRegex.Match (errorString);
			
			if (withColMatch.Success)
			{
				error.FileName = withColMatch.Groups["file"].Value;
				error.Line = int.Parse (withColMatch.Groups["line"].Value);
				error.Column = int.Parse (withColMatch.Groups["column"].Value);
				error.IsWarning = withColMatch.Groups["level"].Value.Equals ("warning");
				error.ErrorText = withColMatch.Groups["message"].Value;
				
				return error;
			}
			
			Match noColMatch = noColRegex.Match (errorString);
			
			if (noColMatch.Success)
			{
				error.FileName = noColMatch.Groups["file"].Value;
				error.Line = int.Parse (noColMatch.Groups["line"].Value);
				error.IsWarning = noColMatch.Groups["level"].Value.Equals ("warning");
				error.ErrorText = noColMatch.Groups["message"].Value;
				
				return error;
			}
			
			return null;
		}
		
		protected override void ParseLinkerOutput (string errorString, CompilerResults cr)
		{
			TextReader reader = new StringReader (errorString);
			string next;
			
			while ((next = reader.ReadLine ()) != null) {
				CompilerError error = CreateLinkerErrorFromErrorString (next);
				if (error != null)
					cr.Errors.Add (error);
			}
			
			reader.Close ();
		}
		
		// FIXME: needs to be improved UPDATE: or does it...?
		private CompilerError CreateLinkerErrorFromErrorString (string errorString)
		{
			CompilerError error = new CompilerError ();
			
			error.ErrorText = errorString;
			
			return error;
		}
	}
}
