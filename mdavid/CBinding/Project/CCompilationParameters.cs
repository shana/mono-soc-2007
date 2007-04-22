using System;
using System.Xml;
using System.Diagnostics;
using System.Collections;

using MonoDevelop.Projects;
using MonoDevelop.Projects.Serialization;

namespace CBinding
{
	public class CCompilationParameters : ICloneable
	{		
		[ItemProperty ("genwarnings")]
		private bool genwarnings = false;
		
		[ItemProperty ("Includes")]
		[ItemProperty ("Include", Scope = 1, ValueType = typeof(string))]
    	private ArrayList includes;
		
		[ItemProperty ("LibPaths")]
		[ItemProperty ("LibPath", Scope = 1, ValueType = typeof(string))]
		[ExpandedCollection]
    	private ArrayList libpaths;
		
		[ItemProperty ("Libs")]
		[ItemProperty ("Lib", Scope = 1, ValueType = typeof(string))]
		[ExpandedCollection]
    	private ArrayList libs;
		
		public object Clone ()
		{
			return MemberwiseClone ();
		}
		
		public bool GenWarnings {
			get { return genwarnings; }
			set { genwarnings = value; }
		}
		
		public ArrayList Includes {
			get { return includes; }
			set { includes = value; }
		}
		
		public ArrayList LibPaths {
			get { return libpaths; }
			set { libpaths = value; }
		}
		
		public ArrayList Libs {
			get { return libs; }
			set { libs = value; }
		}
	}
}
