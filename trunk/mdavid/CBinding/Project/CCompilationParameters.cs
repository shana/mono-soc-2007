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
	}
}
