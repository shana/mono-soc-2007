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
	public class CallstackPad: AbstractPadContent, IPadContent
	{
		Mono.Debugger.Frontend.CallstackPad callstackPad;

		public CallstackPad()
		{
			System.Console.WriteLine("<debugger> Creating CallstackPad");
			callstackPad = new Mono.Debugger.Frontend.CallstackPad(DebuggingService.RemoteDebugger);
		}

		public override Gtk.Widget Control {
			get {
				return callstackPad;
			}
		}
	}
}
