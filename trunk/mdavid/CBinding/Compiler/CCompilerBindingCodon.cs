using System;
using System.Collections;
using System.ComponentModel;

using Mono.Addins;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Projects;

namespace CBinding
{
	[Description ("A C/C++ Compiler implementation. The specified class must implement ICompiler")]
	internal class CCompilerBindingCodon : TypeExtensionNode
	{		
		public ICompiler Compiler {
			get { return (ICompiler)base.GetInstance (); }
		}
	}
}
