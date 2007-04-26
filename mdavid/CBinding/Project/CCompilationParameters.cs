using System;
using System.Xml;
using System.Diagnostics;
using System.Collections;

using MonoDevelop.Projects;
using MonoDevelop.Projects.Serialization;

namespace CBinding
{
	public enum WarningLevel {
		None,
		Normal,
		All
	}
	
	public class CCompilationParameters : ICloneable
	{		
		[ItemProperty ("WarningLevel")]
		private WarningLevel warningLevel = WarningLevel.Normal;
		
		[ItemProperty ("OptimizationLevel")]
		private int optimization = 0;
		
		[ItemProperty ("ExtraArguments")]
		private string extraargs = string.Empty;
		
		public object Clone ()
		{
			return MemberwiseClone ();
		}
		
		public WarningLevel WarningLevel {
			get { return warningLevel; }
			set { warningLevel = value; }
		}
		
		public int OptimizationLevel {
			get { return optimization; }
			set {
				if (value >= 0 && value <= 3)
					optimization = value;
				else
					optimization = 0;
			}
		}
		
		public string ExtraArguments {
			get { return extraargs; }
			set { extraargs = value; }
		}
	}
}
