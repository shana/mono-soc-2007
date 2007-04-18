using System;
using System.Xml;
using System.Diagnostics;

using MonoDevelop.Projects;
using MonoDevelop.Projects.Serialization;

namespace CBinding
{
	public class CCompilationParameters : ICloneable
	{		
		[ItemProperty("genwarnings")]
		private bool genwarnings = false;
		
		[ItemProperty("includepath")]
		private string includepath = string.Empty;
		
		[ItemProperty("libpath")]
		private string libpath = string.Empty;
		
		[ItemProperty("binpath")]
		private string binpath = string.Empty;
		
		[ItemProperty("compiler")]
		private string compiler = string.Empty;
		
		public object Clone ()
		{
			return MemberwiseClone ();
		}
		
		public bool GenWarnings {
			get { return genwarnings; }
			set { genwarnings = value; }
		}
		
		public string IncludePath {
			get { return includepath; }
			set { includepath = value; }
		}
		
		public string LibPath {
			get { return libpath; }
			set { libpath = value; }
		}
		
		public string BinPath {
			get { return binpath; }
			set { binpath = value; }
		}
		
		public string Compiler {
			get { return compiler; }
			set { compiler = value; }
		}
	}
}
