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
	public class ThreadPad: AbstractPadContent, IPadContent
	{
		Mono.Debugger.Frontend.ThreadPad threadPad;

		public ThreadPad()
		{
			System.Console.WriteLine("<debugger> Creating ThreadPad");
			threadPad = new Mono.Debugger.Frontend.ThreadPad(DebuggingService.RemoteDebugger);
		}

		public override Gtk.Widget Control {
			get {
				return threadPad;
			}
		}
	}
}
