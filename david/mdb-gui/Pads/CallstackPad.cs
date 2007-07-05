using GLib;
using Gtk;
using GtkSharp;
using System;
using System.IO;
using System.Collections;
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
		
		const int ColumnSelected = 0;
		const int ColumnLevel = 1;
		const int ColumnAddress = 2;
		const int ColumnName = 3;
		const int ColumnSource = 4;
		
		string[] columnHeaders = new string[] {
			" ",
			"#",
			"Address",
			"Method name",
			"Source file"
		};

		Gtk.TreeView tree;
		Gtk.TreeStore store;

		public CallstackPad (Interpreter interpreter)
		{
			this.interpreter = interpreter;
			
			this.ShadowType = ShadowType.In;
			
			store = new TreeStore (
				typeof (string),
				typeof (string),
				typeof (string),
				typeof (string),
				typeof (string)
			);
			
			tree = new TreeView (store);
			tree.RulesHint = true;
			tree.HeadersVisible = true;
			
			for(int i = 0; i < columnHeaders.Length; i++) {
				TreeViewColumn column = new TreeViewColumn ();
				CellRenderer renderer = new CellRendererText ();
				column.Title = columnHeaders[i];
				column.PackStart (renderer, true);
				column.AddAttribute (renderer, "text", i);
				column.Resizable = true;
				column.Alignment = 0.0f;
				tree.AppendColumn (column);
			}
			
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
			if (!interpreter.HasCurrentThread ||
			    interpreter.CurrentThread.GetBacktrace() == null ||
			    interpreter.CurrentThread.GetBacktrace().Frames == null) {
				
				store.Clear();
				return;
			}
			
			int currentFrameIndex = interpreter.CurrentThread.GetBacktrace().CurrentFrameIndex;
			StackFrame[] callstack = interpreter.CurrentThread.GetBacktrace().Frames;
			
			
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
				StackFrame frame = callstack[i];
				
				// Get the name
				string name;
				if (frame.Method != null) {
					if (frame.Method.IsLoaded) {
						long offset = frame.TargetAddress - frame.Method.StartAddress;
						if (offset >= 0)
							name = String.Format("{0}+0x{1:x}", frame.Method.Name, offset);
						else
							name = String.Format("{0}-0x{1:x}", frame.Method.Name, -offset);
					} else {
						name = String.Format("{0}", frame.Method.Name);
					}
				} else if (frame.Name != null)
					name = frame.Name.ToString();
				else
					name = string.Empty;
				
				// Get the source file
				string source;
				try {
					source = frame.SourceAddress.Name;
				} catch {
					source = string.Empty;
				}
				
				TreeIter it;
				store.IterNthChild(out it, i);
				store.SetValue(it, ColumnSelected, i == currentFrameIndex ? "*" : " ");
				store.SetValue(it, ColumnLevel, "#" + frame.Level);
				store.SetValue(it, ColumnAddress, frame.TargetAddress.ToString());
				store.SetValue(it, ColumnName, name);
				store.SetValue(it, ColumnSource, source);
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
