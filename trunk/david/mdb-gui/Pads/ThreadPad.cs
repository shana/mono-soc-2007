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
	public class ThreadPad: DebuggerPad
	{
		ThreadsStore remoteStore;
		
		public ThreadPad(DebuggerService debuggerService): base(debuggerService, ThreadsStore.ColumnTypes)
		{
			this.remoteStore = debuggerService.ThreadsStore;
			
			AddImageColumn(String.Empty, ThreadsStore.ColumnSelected);
			AddTextColumn("ID", ThreadsStore.ColumnID);
			AddTextColumn("PID", ThreadsStore.ColumnPID);
			AddTextColumn("TID", ThreadsStore.ColumnTID);
			AddTextColumn("Name", ThreadsStore.ColumnName);
			AddTextColumn("State", ThreadsStore.ColumnState);
			AddTextColumn("Current Location", ThreadsStore.ColumnLocation);
			
			GtkTree.RowActivated += new RowActivatedHandler(RowActivated);
		}
		
		void RowActivated(object sender, RowActivatedArgs args)
		{
			TreeIter it;
			GtkStore.GetIter(out it, args.Path);
			int id = (int)GtkStore.GetValue(it, ThreadsStore.ColumnID);
			remoteStore.SelectThread(id);
		}
		
		public override void ReceiveUpdates()
		{
			GtkTreeStoreUpdater.Update(remoteStore, GtkStore);
		}
	}
}
