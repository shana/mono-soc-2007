using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.CodeDom.Compiler;

using Mono.Addins;

using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Core.Gui.Components;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui;

namespace CBinding
{
	[Extension ("/CBinding/Compilers")]
	public class GccCompiler : GNUCompiler
	{		
		public override string Name {
			get { return "Default C Compiler"; }
		}
		
		public override Language Language {
			get { return Language.C; }
		}
		
		public GccCompiler ()
		{
			compilerCommand = "gcc";
			linkerCommand = "gcc";
		}
	}
}
