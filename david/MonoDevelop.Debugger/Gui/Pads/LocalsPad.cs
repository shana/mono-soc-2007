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
	public class LocalsPad: AbstractPadContent, IPadContent
	{
		Mono.Debugger.Frontend.LocalsPad localsPad;

		public LocalsPad()
		{
			System.Console.WriteLine("<debugger> Creating LocalsPad");
			localsPad = new Mono.Debugger.Frontend.LocalsPad(DebuggingService.RemoteDebugger);
		}

		public override Gtk.Widget Control {
			get {
				return localsPad;
			}
		}
	}
}
