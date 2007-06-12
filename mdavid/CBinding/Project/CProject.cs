//
// CProject.cs: C/C++ Project
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
using System.Xml;
using System.Collections;
using System.ComponentModel;
using System.CodeDom.Compiler;

using MonoDevelop.Core;
using MonoDevelop.Projects;
using MonoDevelop.Projects.Serialization;

namespace CBinding
{
	public enum Language {
		C,
		CPP
	};
	
	[DataInclude(typeof(CProjectConfiguration))]
	public class CProject : Project
	{
		[ItemProperty ("compiler", ValueType = typeof(CCompiler))]
		private CCompiler compiler_manager;
		
		[ItemProperty]
		private Language language;
		
    	private ProjectPackageCollection packages = new ProjectPackageCollection ();
		
		public CProject ()
		{
		}
		
		public CProject (ProjectCreateInformation info,
		                 XmlElement projectOptions, string language)
		{
			string binPath = ".";
			
			if (info != null) {
				Name = info.ProjectName;
				binPath = info.BinPath;
			}
			
			switch (language)
			{
			case "C":
				this.language = Language.C;
				break;
			case "CPP":
				this.language = Language.CPP;
				break;
			}
			
			Compiler = null; // use default compiler depending on language
			
			CProjectConfiguration configuration =
				(CProjectConfiguration)CreateConfiguration ("Debug");
				
			Configurations.Add (configuration);
			
			configuration =
				(CProjectConfiguration)CreateConfiguration ("Release");
				
			configuration.DebugMode = false;
			Configurations.Add (configuration);
			
			foreach (CProjectConfiguration c in Configurations) {
				c.OutputDirectory = Path.Combine (binPath, c.Name);
				c.SourceDirectory = BaseDirectory;
				c.Output = Name;
				
				if (projectOptions != null) {
					if (projectOptions.Attributes["Target"] != null) {
						c.CompileTarget = (CBinding.CompileTarget)Enum.Parse (
						     typeof(CBinding.CompileTarget),
							 projectOptions.Attributes["Target"].InnerText);
					}
					if (projectOptions.Attributes["PauseConsoleOutput"] != null) {
						c.PauseConsoleOutput = bool.Parse (
							projectOptions.Attributes["PauseConsoleOutput"].InnerText);
					}
				}
			}			
		}
		
		public override string ProjectType {
			get { return "C/C++"; }
		}
		
		public override string[] SupportedLanguages {
			get { return new string[] { "C", "CPP" }; }
		}
		
		public override bool IsCompileable (string fileName)
		{
			if (language == Language.C) {
				return (Path.GetExtension (fileName.ToUpper ()) == ".C");
			} else {
				return (Path.GetExtension (fileName.ToUpper ()) == ".CPP" ||
				        Path.GetExtension (fileName.ToUpper ()) == ".C");
			}
		}
		
		protected override ICompilerResult DoBuild (IProgressMonitor monitor)
		{
			CProjectConfiguration pc = (CProjectConfiguration)ActiveConfiguration;
			foreach (ProjectFile f in ProjectFiles) {
				if (f.BuildAction == BuildAction.FileCopy)
					Runtime.FileService.CopyFile (
						f.Name, Path.Combine (pc.OutputDirectory, 
						                      Path.GetFileName (f.Name)));
			}
			
			return compiler_manager.Compile (
				ProjectFiles, packages,
				(CProjectConfiguration)ActiveConfiguration,
			    monitor);
		}
		
		protected override void DoExecute (IProgressMonitor monitor,
		                                   ExecutionContext context)
		{
			// TODO: implement
		}
		
		public override string GetOutputFileName ()
		{
			CProjectConfiguration conf = (CProjectConfiguration)ActiveConfiguration;
			return conf.Name;
		}
		
		public override IConfiguration CreateConfiguration (string name)
		{
			CProjectConfiguration conf = new CProjectConfiguration ();
			
			conf.Name = name;
			conf.CompilationParameters = new CCompilationParameters ();
			
			return conf;
		}
		
		public Language Language {
			get { return language; }
		}
		
		public CCompiler Compiler {
			get { return compiler_manager; }
			set {
				if (value != null) {
					compiler_manager = value;
				} else {
					if (language == Language.C)
						compiler_manager = new GccCompiler ();
					else
						compiler_manager = new GppCompiler ();
				}
			}
		}
		
		[Browsable(false)]
		[ItemProperty ("Packages")]
		public ProjectPackageCollection Packages {
			get { return packages; }
		}
		
		protected override void OnFileAddedToProject (ProjectFileEventArgs e)
		{
			base.OnFileAddedToProject (e);
			
			if (!IsCompileable (e.ProjectFile.Name))
				e.ProjectFile.BuildAction = BuildAction.Nothing;
		}
		
		internal void NotifyPackageRemovedFromProject (Package package)
		{
		}
		
		internal void NotifyPackageAddedToProject (Package package)
		{
		}
	}
}
