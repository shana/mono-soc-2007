using System;
using System.Xml;
using System.Diagnostics;

using MonoDevelop.Projects;
using MonoDevelop.Projects.Serialization;

namespace CBinding
{
	public class CCompilerParameters : ICloneable
	{
		[ItemProperty("output")]
		private string output = string.Empty;
		
		[ItemProperty("objectonly")]
		private bool objectonly = false;
		
		[ItemProperty("genwarnings")]
		private bool genwarnings = true;
		
		[ItemProperty("includepath")]
		private string includepath = string.Empty;
		
		[ItemProperty("libpath")]
		private string libpath = string.Empty;
		
		[ItemProperty("binpath")]
		private string binpath = string.Empty;
		
		[ItemProperty("compilerpath")]
		private string compilerpath = string.Empty;
		
		public object Clone ()
		{
			return MemberwiseClone ();
		}
		
		public string Output {
			get { return output; }
			set { output = value; }
		}
		
		public bool ObjectOnly {
			get { return objectonly; }
			set { objectonly = value; }
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
		
		public string CompilerPath {
			get { return compilerpath; }
			set { compilerpath = value; }
		}
	}
}
