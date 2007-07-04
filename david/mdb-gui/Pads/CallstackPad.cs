using GLib;
using Gtk;
using GtkSharp;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Mono.Debugger;
using Mono.Debugger.Frontend;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend
{
	public class CallstackPad : Gtk.ScrolledWindow
	{
		Interpreter interpreter;

		Gtk.TreeView tree;
		Gtk.TreeStore store;

		public CallstackPad (Interpreter interpreter)
		{
			this.interpreter = interpreter;
			
			this.ShadowType = ShadowType.In;

			store = new TreeStore (typeof (string));

			tree = new TreeView (store);
			tree.RulesHint = true;
			tree.HeadersVisible = true;

			TreeViewColumn frameCol = new TreeViewColumn ();
			CellRenderer frameRenderer = new CellRendererText ();
			frameCol.Title = "Frame";
			frameCol.PackStart (frameRenderer, true);
			frameCol.AddAttribute (frameRenderer, "text", 0);
			frameCol.Resizable = true;
			frameCol.Alignment = 0.0f;
			tree.AppendColumn (frameCol);

			Add (tree);
			ShowAll ();

//			Services.DebuggingService.PausedEvent += (EventHandler) Services.DispatchService.GuiDispatch (new EventHandler (OnPausedEvent));
//			Services.DebuggingService.ResumedEvent += (EventHandler) Services.DispatchService.GuiDispatch (new EventHandler (OnResumedEvent));
//			Services.DebuggingService.StoppedEvent += (EventHandler) Services.DispatchService.GuiDispatch (new EventHandler (OnStoppedEvent));
		}
		
//		void IPadContent.Initialize (IPadWindow window)
//		{
//			window.Title = "Call Stack";
//			window.Icon = Stock.OutputIcon;
//		}

		public void UpdateDisplay ()
		{
			if (!interpreter.HasCurrentThread) {
				store.Clear();
				return;
			}
			
			Mono.Debugger.Thread currentThread = interpreter.CurrentThread;
			StackFrame[] callstack = currentThread.GetBacktrace().Frames;
			
			// Adjust the number of rows to match the callstack length
			while (store.IterNChildren() > callstack.Length) {
				// Delete first row
				TreeIter it;
				store.GetIterFirst(out it);
				store.Remove(ref it);
			}
			while (store.IterNChildren() < callstack.Length) {
				// Add extra row
				store.PrependNode();
			}
			
			// Update the values of the rows (in reverse order)
			for (int i = callstack.Length - 1; i >= 0; i--) {
				TreeIter it;
				store.IterNthChild(out it, i);
				store.SetValue(it, 0, callstack[i].ToString());
			}
		}

//		protected void OnStoppedEvent (object o, EventArgs args)
//		{
//			current_frame = null;
//			UpdateDisplay ();
//		}
//
//		protected void OnResumedEvent (object o, EventArgs args)
//		{
//		}
//
//		protected void OnPausedEvent (object o, EventArgs args)
//		{
//			DebuggingService dbgr = (DebuggingService)Services.DebuggingService;
//			current_frame = dbgr.CurrentFrame;
//			UpdateDisplay ();
//		}

		public Gtk.Widget Control {
			get {
				return this;
			}
		}

		public string Id {
			get { return "MonoDevelop.Debugger.StackTracePad"; }
		}

		public string DefaultPlacement {
			get { return "Bottom"; }
		}

		public void RedrawContent ()
		{
			UpdateDisplay ();
		}
	}
}
