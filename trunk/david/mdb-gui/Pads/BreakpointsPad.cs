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
	public class BreakpointsPad : Gtk.ScrolledWindow
	{
		MdbGui mdbGui;
		Interpreter interpreter;
		
		Gtk.TreeView tree;
		Gtk.TreeStore store;
		
		const int ColumnImage       = 0;
		const int ColumnID          = 1;
		const int ColumnEnabled     = 2;
		const int ColumnActivated   = 3;
		const int ColumnThreadGroup = 4;
		const int ColumnLocation    = 5;
		
		string[] columnHeaders = new string[] {
			"",
			"ID",
			"Enabled",
			"Activated",
			"Thread group",
			"Location"
		};
		
		Type[] columnTypes = new Type[] {
			typeof (Gdk.Pixbuf),
			typeof (int),
			typeof (string),
			typeof (string),
			typeof (string),
			typeof (string)
		};
		
		public BreakpointsPad(MdbGui mdbGui)
		{
			this.mdbGui = mdbGui;
			this.interpreter = mdbGui.Interpreter;
			
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
			
			tree.RowActivated += new RowActivatedHandler(RowActivated);
		}
		
		void RowActivated(object sender, RowActivatedArgs args)
		{
			TreeIter it;
			store.GetIter(out it, args.Path);
			int id = (int)store.GetValue(it, ColumnID);
		}
		
		public void UpdateDisplay ()
		{
			Event[] events = interpreter.Session.Events;
			
			store.Clear();
			
			foreach (Event handle in events) {
				if (handle is SourceBreakpoint) {
					TreeIter it = store.AppendNode();
					
					store.SetValue(it, ColumnImage, handle.IsEnabled && handle.IsActivated ? Pixmaps.Breakpoint : Pixmaps.BreakpointDisabled);
					store.SetValue(it, ColumnID, handle.Index);
					store.SetValue(it, ColumnEnabled, handle.IsEnabled ? "Yes" : "No");
					store.SetValue(it, ColumnActivated, handle.IsActivated ? "Yes" : "No");
					store.SetValue(it, ColumnThreadGroup, handle.ThreadGroup != null ? handle.ThreadGroup.Name : "global");
					store.SetValue(it, ColumnLocation, handle.Name);
				}
			}
		}
	}
}
