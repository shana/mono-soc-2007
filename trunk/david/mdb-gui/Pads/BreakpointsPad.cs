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
	public class BreakpointsPad: DebuggerPad
	{
		BreakpointsStore remoteStore;
		
		public BreakpointsPad(MdbGui mdbGui): base(mdbGui, BreakpointsStore.ColumnTypes)
		{
			this.remoteStore = mdbGui.DebuggerService.BreakpointsStore;
			
			AddImageColumn(String.Empty, BreakpointsStore.ColumnImage);
			AddTextColumn("ID", BreakpointsStore.ColumnID);
			AddTextColumn("Enabled", BreakpointsStore.ColumnEnabled);
			AddTextColumn("Activated", BreakpointsStore.ColumnActivated);
			AddTextColumn("Thread group", BreakpointsStore.ColumnThreadGroup);
			AddTextColumn("Location", BreakpointsStore.ColumnName);
			
			this.GtkTree.RowActivated += new RowActivatedHandler(RowActivated);
		}
		
		void RowActivated(object sender, RowActivatedArgs args)
		{
			TreeIter it;
			GtkStore.GetIter(out it, args.Path);
			int id = (int)GtkStore.GetValue(it, BreakpointsStore.ColumnID);
		}
		
		public SourceCodeLocation[] GetBreakpoints()
		{
			ArrayList breakpoints = new ArrayList();
			
			int count = GtkStore.IterNChildren();
			for(int i = 0; i < count; i++) {
				TreeIter childIter;
				GtkStore.IterNthChild(out childIter, i);
				SourceCodeLocation bpLocation = (SourceCodeLocation)GtkStore.GetValue(childIter, BreakpointsStore.ColumnLocation);
				breakpoints.Add(bpLocation);
			}
			
			return (SourceCodeLocation[])breakpoints.ToArray(typeof(SourceCodeLocation));
		}
		
		public override void ReceiveUpdates()
		{
			GtkTreeStoreUpdater.Update(remoteStore, GtkStore);
		}
	}
}
