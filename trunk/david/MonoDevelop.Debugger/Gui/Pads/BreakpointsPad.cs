using System;
using System.Collections;
using System.IO;
using System.Diagnostics;

using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Ide.Gui;

using Gtk;

namespace MonoDevelop.Debugger.Gui.Pads
{
	public class BreakpointsPad: AbstractPadContent, IPadContent
	{
		Mono.Debugger.Frontend.BreakpointsPad breakpointsPad;

		public BreakpointsPad()
		{
			System.Console.WriteLine("<debugger> Creating BreakpointsPad");
			breakpointsPad = new Mono.Debugger.Frontend.BreakpointsPad(DebuggingService.RemoteDebugger);
		}

		public override Gtk.Widget Control {
			get {
				return breakpointsPad;
			}
		}
	}
}
