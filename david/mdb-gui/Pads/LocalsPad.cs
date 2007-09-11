using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using Gtk;

using Mono.Debugger;
using Mono.Debugger.Languages;
using Mono.Debugger.Frontend.TreeModel;

namespace Mono.Debugger.Frontend
{
	public class LocalsPad: DebuggerPad
	{
		LocalsStore remoteStore;
		
		Hashtable expandedNodes = new Hashtable();
		
		public LocalsPad(DebuggerService debuggerService): base(debuggerService, LocalsStore.ColumnTypes)
		{
			this.remoteStore = debuggerService.LocalsStore;
			
			AddImageTextColumn("Name", LocalsStore.ColumnImage, LocalsStore.ColumnName);
			AddTextColumn("Value", LocalsStore.ColumnValue);
			AddTextColumn("Type", LocalsStore.ColumnType);
			
			GtkTree.RowExpanded += new RowExpandedHandler(RowExpanded);
			GtkTree.TestCollapseRow += new TestCollapseRowHandler(TestCollapseRow);
			GtkTree.TestExpandRow += new TestExpandRowHandler(TestExpandRow);
		}
		
		protected void TestCollapseRow(object o, TestCollapseRowArgs args)
		{
			// Remove from the expandedNodes table
			string fullName = (string)GtkStore.GetValue(args.Iter, LocalsStore.ColumnFullName);
			if (expandedNodes.ContainsKey(fullName)) {
				expandedNodes.Remove(fullName);
			}
			
			// Notify remote store
			RemoteTreeNodeRef nodeRef = (RemoteTreeNodeRef)GtkStore.GetValue(args.Iter, LocalsStore.ColumnReference);
			remoteStore.CollapseNode(nodeRef);
		}
		
		protected void TestExpandRow(object o, TestExpandRowArgs args)
		{
			// Add to the expandedNodes table
			string fullName = (string)GtkStore.GetValue(args.Iter, LocalsStore.ColumnFullName);
			expandedNodes[fullName] = null; // No value, just insert the key
			
			// Notify remote store
			RemoteTreeNodeRef nodeRef = (RemoteTreeNodeRef)GtkStore.GetValue(args.Iter, LocalsStore.ColumnReference);
			remoteStore.ExpandNode(nodeRef);
			
			GtkTreeStoreUpdater.Update(remoteStore, GtkStore);
			
			TreeIter it;
			GtkStore.GetIter(out it, args.Path);
			int childCount = GtkStore.IterNChildren(it);
			if (childCount == 0) {
				args.RetVal = true;  // Cancel expanding
			} else {
				args.RetVal = false;
			}
		}
		
		protected void RowExpanded(object o, RowExpandedArgs args)
		{
			TreeIter it;
			GtkStore.GetIter(out it, args.Path);
			int childCount = GtkStore.IterNChildren(it);
			for(int i = 0; i < childCount; i++) {
				TreePath childPath = args.Path.Copy();
				childPath.AppendIndex(i);
				
				TreeIter childIter;
				GtkStore.GetIter(out childIter, childPath);
				string childFullName = (string)GtkStore.GetValue(childIter, LocalsStore.ColumnFullName);
				// This node was expanded in the past - expand it
				if (childFullName != null && expandedNodes.ContainsKey(childFullName)) {
					GtkTree.ExpandRow(childPath, false);
				}
			}
		}
		
		public override void ReceiveUpdates()
		{
			GtkTreeStoreUpdater.Update(remoteStore, GtkStore);
			
			int childCount = GtkStore.IterNChildren();
			for(int i = 0; i < childCount; i++) {
				TreePath childPath = new TreePath(new int[] {i});
				
				TreeIter childIter;
				GtkStore.GetIter(out childIter, childPath);
				string childFullName = (string)GtkStore.GetValue(childIter, LocalsStore.ColumnFullName);
				// This node was expanded in the past - expand it
				if (expandedNodes.ContainsKey(childFullName)) {
					GtkTree.ExpandRow(childPath, false);
				}
			}
		}
	}
}
