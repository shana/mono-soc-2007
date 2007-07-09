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
		MdbGui mdbGui;
		Interpreter interpreter;
		
		Gtk.TreeView tree;
		Gtk.TreeStore store;
		Hashtable threadRows;
		
		string[] columnHeaders = new string[] {
			"",
			"ID",
			"PID",
			"TID",
			"Name",
			"State",
			"Current Location"
		};
		
		Type[] columnTypes = new Type[] {
			typeof (Gdk.Pixbuf),
			typeof (int),
			typeof (int),
			typeof (string),
			typeof (string),
			typeof (string),
			typeof (string)
		};
		
		public ThreadPad(MdbGui mdbGui)
		{
			this.mdbGui = mdbGui;
			this.interpreter = mdbGui.Interpreter;
			
			threadRows = new Hashtable ();
			
			this.ShadowType = ShadowType.In;
			
			store = new TreeStore(columnTypes);
			
			tree = new TreeView (store);
			tree.RulesHint = true;
			tree.HeadersVisible = true;
			
			{
				TreeViewColumn column = new TreeViewColumn ();
				CellRenderer iconRenderer = new CellRendererPixbuf ();
				column.Title = columnHeaders[0];
				column.PackStart (iconRenderer, false);
				column.AddAttribute (iconRenderer, "pixbuf", 0);
				column.Resizable = true;
				column.Alignment = 0.0f;
				tree.AppendColumn (column);
			}
			
			for(int i = 1; i < columnHeaders.Length; i++) {
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
				bool current = interpreter.HasCurrentThread && interpreter.CurrentThread.ID == thread.ID;
				
				string location;
				if (thread.IsStopped) {
					try {
						location = thread.GetBacktrace().Frames[0].SourceAddress.Name;
					} catch {
						location = "";
					}
				} else {
					location = "";
				}
				
				store.SetValue (it, 0, current ? Pixmaps.Arrow : Pixmaps.Empty);
				store.SetValue (it, 1, thread.ID);
				store.SetValue (it, 2, thread.PID);
				store.SetValue (it, 3, String.Format("{0:x}", thread.TID));
				store.SetValue (it, 4, thread.Name);
				store.SetValue (it, 5, thread.State.ToString());
				store.SetValue (it, 6, location);
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
			
			if (interpreter.HasTarget) {
				foreach (Process process in interpreter.Processes) {
					foreach (Thread thread in process.GetThreads ()) {
						UpdateThread(thread);
						threadsToRemove.Remove(thread);
					}
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
