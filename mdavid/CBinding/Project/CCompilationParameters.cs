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
		
		public object Clone ()
		{
			return MemberwiseClone ();
		}
		
		public bool GenWarnings {
			get { return genwarnings; }
			set { genwarnings = value; }
		}
	}
}
