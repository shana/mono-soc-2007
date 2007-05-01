using System;
using System.Collections;
using System.ComponentModel;

using MonoDevelop.Core.AddIns;
using MonoDevelop.Projects;

namespace CBinding
{
	[Description ("A C/C++ Compiler implementation. The specified class must subclass CCompiler")]
	[CodonNameAttribute ("CCompilerBinding")]
	public class CCompilerBindingCodon : ClassCodon
	{
		CCompiler compiler;
		
		public CCompiler Compiler {
			get { return compiler; }
		}
		
		public override object BuildItem (object owner, ArrayList subItems, ConditionCollection conditions)
		{
			compiler = (CCompiler)AddIn.CreateObject (Class);
			return this;
		}
	}
}
