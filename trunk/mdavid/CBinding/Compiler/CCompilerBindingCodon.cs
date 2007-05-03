using System;
using System.Collections;
using System.ComponentModel;

using MonoDevelop.Core.AddIns;
using MonoDevelop.Projects;

namespace CBinding
{
	[Description ("A C/C++ Compiler implementation. The specified class must implement ICompiler")]
	[CodonNameAttribute ("CCompilerBinding")]
	public class CCompilerBindingCodon : ClassCodon
	{
		ICompiler compiler;
		
		public ICompiler Compiler {
			get { return compiler; }
		}
		
		public override object BuildItem (object owner, ArrayList subItems, ConditionCollection conditions)
		{
			compiler = (ICompiler)AddIn.CreateObject (Class);
			return this;
		}
	}
}
