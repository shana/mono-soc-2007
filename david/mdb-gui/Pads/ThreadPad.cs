using GLib;
using Gtk;
using GtkSharp;
using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend
{
	public class ThreadPad : Gtk.ScrolledWindow
	{
		Interpreter interpreter;
		
		Gtk.TreeView tree;
		Gtk.TreeStore store;
		Hashtable threadRows;

		public ThreadPad (Interpreter interpreter)
		{
			this.interpreter = interpreter;
			
			threadRows = new Hashtable ();

			this.ShadowType = ShadowType.In;

			store = new TreeStore (typeof (int),
					       typeof (int),
					       typeof (string),
					       typeof (string));

			tree = new TreeView (store);
			tree.RulesHint = true;
			tree.HeadersVisible = true;

			TreeViewColumn Col;
			CellRenderer ThreadRenderer;

			Col = new TreeViewColumn ();
			ThreadRenderer = new CellRendererText ();
			Col.Title = "Id";
			Col.PackStart (ThreadRenderer, true);
			Col.AddAttribute (ThreadRenderer, "text", 0);
			Col.Resizable = true;
			Col.Alignment = 0.0f;
			tree.AppendColumn (Col);

			Col = new TreeViewColumn ();
			ThreadRenderer = new CellRendererText ();
			Col.Title = "PID";
			Col.PackStart (ThreadRenderer, true);
			Col.AddAttribute (ThreadRenderer, "text", 1);
			Col.Resizable = true;
			Col.Alignment = 0.0f;
			tree.AppendColumn (Col);

			Col = new TreeViewColumn ();
			ThreadRenderer = new CellRendererText ();
			Col.Title = "State";
			Col.PackStart (ThreadRenderer, true);
			Col.AddAttribute (ThreadRenderer, "text", 2);
			Col.Resizable = true;
			Col.Alignment = 0.0f;
			tree.AppendColumn (Col);

			Col = new TreeViewColumn ();
			ThreadRenderer = new CellRendererText ();
			Col.Title = "Current Location";
			Col.PackStart (ThreadRenderer, true);
			Col.AddAttribute (ThreadRenderer, "text", 3);
			Col.Resizable = true;
			Col.Alignment = 0.0f;
			tree.AppendColumn (Col);

			Add (tree);
			ShowAll ();

//			((DebuggingService)Services.DebuggingService).ThreadStateEvent += (EventHandler) Services.DispatchService.GuiDispatch (new EventHandler (OnThreadEvent));
		}
		
//		void IPadContent.Initialize (IPadWindow window)
//		{
//			window.Title = "Threads";
//			window.Icon = Stock.OutputIcon;
//		}

		void AddThread (Thread thread)
		{
			TreeIter it = store.AppendNode();
			threadRows.Add (thread, new TreeRowReference (store, store.GetPath (it)));
			UpdateThread(thread);
		}

		void UpdateThread (Thread thread)
		{
			TreeRowReference row = (TreeRowReference)threadRows[thread];
			TreeIter it;

			if (row != null && store.GetIter (out it, row.Path)) {
				store.SetValue (it, 0, thread.ID);
				store.SetValue (it, 1, thread.PID);
				store.SetValue (it, 2, thread.State.ToString());

				string location;
				if (thread.IsStopped)
					location = thread.GetBacktrace().Frames[0].SourceAddress.Name;
				else
					location = "";

				store.SetValue (it, 3, location);
			} else {
				AddThread (thread);
			}
		}

		void RemoveThread (Thread thread)
		{
			TreeRowReference row = (TreeRowReference)threadRows[thread];
			TreeIter it;

			if (row != null && store.GetIter (out it, row.Path))
				store.Remove (ref it);

			threadRows.Remove (thread);
		}

		public void UpdateDisplay ()
		{
			Hashtable threadsToRemove = (Hashtable)threadRows.Clone();
			
			foreach (Process process in interpreter.Processes) {
				foreach (Thread thread in process.GetThreads ()) {
					UpdateThread(thread);
					threadsToRemove.Remove(thread);
				}
			}
			
			foreach (Thread thread in threadsToRemove.Keys) {
				RemoveThread(thread);
			}
		}

		public void CleanDisplay ()
		{
			UpdateDisplay ();
		}

		public void RedrawContent ()
		{
			UpdateDisplay ();
		}

		protected void OnThreadEvent (object o, EventArgs args)
		{
			UpdateDisplay ();
		}

		public Gtk.Widget Control {
			get {
				return this;
			}
		}

		public string Id {
			get { return "MonoDevelop.Debugger.ThreadPad"; }
		}

		public string DefaultPlacement {
			get { return "Bottom"; }
		}
	}
}
