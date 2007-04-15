using System;
using System.Xml;
using System.CodeDom.Compiler;

using MonoDevelop.Core;
using MonoDevelop.Projects;
using MonoDevelop.Projects.Parser;
using MonoDevelop.Projects.CodeGeneration;

namespace CBinding
{
	public class CLanguageBinding : IDotNetLanguageBinding
	{
		public const string LanguageName = "C";

		public string Language {
			get { return LanguageName; }
		}
		
		public bool IsSourceCodeFile (string fileName)
		{
			// FIXME: implement
			return true;
		}
		
		public ICompilerResult Compile (ProjectFileCollection projectFiles,
										ProjectReferenceCollection references,
										DotNetProjectConfiguration configuration,
										IProgressMonitor monitor)
		{
			// FIXME: implement
			return null;
		}
		
		public ICloneable CreateCompilationParameters (XmlElement projectOptions)
		{
			// FIXME: implement
			return null;
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
			// FIXME: implement
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
	}
}
