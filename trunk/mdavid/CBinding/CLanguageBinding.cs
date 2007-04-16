using System;
using System.Xml;
using System.Diagnostics;
using System.CodeDom.Compiler;

using MonoDevelop.Core;
using MonoDevelop.Core.Properties;
using MonoDevelop.Projects;
using MonoDevelop.Projects.Parser;
using MonoDevelop.Projects.CodeGeneration;
using MonoDevelop.Ide.Gui;

namespace CBinding
{
	public class CLanguageBinding : IDotNetLanguageBinding
	{
		public const string LanguageName = "C";
		private CCompilerManager manager = new CCompilerManager ();
		private GlobalProperties properties = new GlobalProperties ();

		public string Language {
			get { return LanguageName; }
		}
		
		public bool IsSourceCodeFile (string fileName)
		{
			Debug.Assert(manager != null);
			return manager.CanCompile(fileName);
		}
		
		public ICompilerResult Compile (ProjectFileCollection projectFiles,
		                                ProjectReferenceCollection references,
		                                DotNetProjectConfiguration configuration,
		                                IProgressMonitor monitor)
		{
			Debug.Assert(manager != null);
			return manager.Compile(projectFiles, references, configuration, monitor);
		}
		
		public ICloneable CreateCompilationParameters (XmlElement projectOptions)
		{
			CCompilerParameters parameters = new CCompilerParameters ();
			
			if (IdeApp.ProjectOperations.CurrentSelectedProject != null)
			{
				// FIXME: selected from global options, this is default
				parameters.Output = IdeApp.ProjectOperations.CurrentSelectedProject.Name;
			}
			
			if (properties != null)
			{
				parameters.IncludePath = properties.IncludePath;
				parameters.LibPath = properties.LibPath;
				parameters.BinPath = properties.BinPath;
				parameters.CompilerPath = properties.CompilerCommand;
			}
			
			return parameters;
		}
		
		public string CommentTag
		{
			get { return "//"; }
		}
		
		public CodeDomProvider GetCodeDomProvider ()
		{
			return null;
		}
		
		public string GetFileName (string baseName)
		{
			// TODO: implement
			return baseName;
		}
			
		public IParser Parser {
			get { return null; }
		}
		
		public IRefactorer Refactorer {
			get { return null; }
		}
		
		public ClrVersion[] GetSupportedClrVersions ()
		{
			return new ClrVersion[] { ClrVersion.Net_1_1, ClrVersion.Net_2_0 };
		}
		
		public class GlobalProperties
		{
			IProperties props = (IProperties) Runtime.Properties.GetProperty ("CBinding.GlobalPropertiess", new DefaultProperties ());

			public string CompilerCommand {
				get { return props.GetProperty ("CompilerCommand", "gcc"); }
				set { props.SetProperty ("CompilerCommand", value != null ? value : "gcc"); }
			}
			
			public string IncludePath {
				get { return props.GetProperty ("IncludePath", "/usr/include/"); }
				set { props.SetProperty ("IncludePath", value != null ? value : "/usr/include/"); }
			}
			
			public string LibPath {
				get { return props.GetProperty ("LibPath", "/usr/lib/"); }
				set { props.SetProperty ("LibPath", value != null ? value : "/usr/lib/"); }
			}
			
			public string BinPath {
				get { return props.GetProperty ("BinPath", "/usr/bin/"); }
				set { props.SetProperty ("BinPath", value != null ? value : "/usr/bin/"); }
			}
		}
	}
}
