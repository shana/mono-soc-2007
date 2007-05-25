using System;
using System.IO;

using MonoDevelop.Projects;
using MonoDevelop.Core;
using MonoDevelop.Projects.Parser;
using MonoDevelop.Projects.CodeGeneration;

namespace CBinding
{
	public class CLanguageBinding : ILanguageBinding
	{
		public string Language {
			// FIXME
			get { return "C"; }
		}
		
		public string CommentTag {
			get { return "//"; }
		}
		
		public bool IsSourceCodeFile (string fileName)
		{
			return (Path.GetExtension (fileName.ToUpper ()) == ".CPP" ||
			        Path.GetExtension (fileName.ToUpper ()) == ".C");
		}
		
		public IParser Parser {
			get { return null; }
		}
		
		public IRefactorer Refactorer {
			get { return null; }
		}
		
		public string GetFileName (string baseName)
		{
			return baseName + ".c";
		}
	}
}
