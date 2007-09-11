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
	public class MdbConsolePad: AbstractPadContent, IPadContent
	{
		Mono.Debugger.Frontend.MdbConsolePad mdbConsolePad;

		public MdbConsolePad()
		{
			System.Console.WriteLine("<debugger> Creating MdbConsolePad");
			mdbConsolePad = new Mono.Debugger.Frontend.MdbConsolePad(DebuggingService.RemoteDebugger);
		}

		public override Gtk.Widget Control {
			get {
				return mdbConsolePad;
			}
		}
	}
}
