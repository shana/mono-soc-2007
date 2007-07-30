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
		
		public ThreadPad(MdbGui mdbGui): base(mdbGui, ThreadsStore.ColumnTypes)
		{
			this.remoteStore = new ThreadsStore(mdbGui.Interpreter);
			
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
			MdbGui.UpdateGUI();
		}
		
		public override void UpdateDisplay()
		{
			remoteStore.UpdateTree();
			GtkTreeStoreUpdater.Update(remoteStore, GtkStore);
		}
	}
}
