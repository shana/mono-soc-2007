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
	public class CallstackPad: DebuggerPad
	{
		CallstackStore remoteStore;
		
		public CallstackPad(DebuggerService debuggerService): base(debuggerService, CallstackStore.ColumnTypes)
		{
			this.remoteStore = debuggerService.CallstackStore;
			
			AddImageColumn(String.Empty, CallstackStore.ColumnSelected);
			AddTextColumn("#", CallstackStore.ColumnLevel);
			AddTextColumn("Address", CallstackStore.ColumnAddress);
			AddTextColumn("Method name", CallstackStore.ColumnName);
			AddTextColumn("Source file", CallstackStore.ColumnSource);
			
			GtkTree.RowActivated += new RowActivatedHandler(RowActivated);
		}
		
		void RowActivated(object sender, RowActivatedArgs args)
		{
			remoteStore.SelectFrame(args.Path.Indices[0]);
		}
		
		public override void ReceiveUpdates()
		{
			GtkTreeStoreUpdater.Update(remoteStore, GtkStore);
		}
	}
}
