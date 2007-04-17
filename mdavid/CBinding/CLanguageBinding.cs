using MonoDevelop.Projects;
using MonoDevelop.Projects.Parser;
using MonoDevelop.Projects.CodeGeneration;

namespace CBinding
{
	public class CLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "C";

		public string Language {
			get { return LanguageName; }
		}
	
		public string CommentTag
		{
			get { return "//"; }
		}
		
		public bool IsSourceCodeFile (string fileName)
		{
			return (fileName.ToUpper ().EndsWith (".C")) ||
			       (fileName.ToUpper ().EndsWith (".H"));
		}

		public string GetFileName (string baseName)
		{
			// TODO: implement
			return baseName + ".c";
		}
			
		public IParser Parser {
			get { return null; }
		}
		
		public IRefactorer Refactorer {
			get { return null; }
		}
	}
}
