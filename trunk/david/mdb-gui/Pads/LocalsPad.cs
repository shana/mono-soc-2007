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
		
		public LocalsPad(MdbGui mdbGui): base(mdbGui, LocalsStore.ColumnTypes)
		{
			this.remoteStore = new LocalsStore(mdbGui.Interpreter);
			
			AddImageTextColumn("Name", LocalsStore.ColumnImage, LocalsStore.ColumnName);
			AddTextColumn("Value", LocalsStore.ColumnValue);
			AddTextColumn("Type", LocalsStore.ColumnType);
			
			GtkTree.RowExpanded += new RowExpandedHandler(RowExpanded);
			GtkTree.TestCollapseRow += new TestCollapseRowHandler(TestCollapseRow);
			GtkTree.TestExpandRow += new TestExpandRowHandler(TestExpandRow);
		}
		
		protected void TestCollapseRow(object o, TestCollapseRowArgs args)
		{
			string id = (string)GtkStore.GetValue(args.Iter, LocalsStore.ColumnID);
			if (expandedNodes.ContainsKey(id)) {
				expandedNodes.Remove(id);
			}
		}
		
		protected void TestExpandRow(object o, TestExpandRowArgs args)
		{
			string id = (string)GtkStore.GetValue(args.Iter, LocalsStore.ColumnID);
			expandedNodes[id] = null; // No value, just insert the key
			int childCount = remoteStore.ExpandNode(new RemoteTreePath(args.Path.Indices));
			if (childCount == 0) {
				args.RetVal = true;  // Cancel expanding
			} else {
				args.RetVal = false;
			}
		}
		
		protected void RowExpanded(object o, RowExpandedArgs args)
		{
			string id = (string)GtkStore.GetValue(args.Iter, LocalsStore.ColumnID);
			
			int childCount = GtkStore.IterNChildren(args.Iter);
			for(int i = 0; i < childCount; i++) {
				TreePath childPath = args.Path.Copy();
				childPath.AppendIndex(i);
				
				TreeIter childIter;
				GtkStore.GetIter(out childIter, childPath);
				string childId = (string)GtkStore.GetValue(childIter, LocalsStore.ColumnID);
				// This node was expanded in the past - expand it
				if (expandedNodes.ContainsKey(childId)) {
					GtkTree.ExpandRow(childPath, false);
				}
			}
		}
		
		public override void UpdateDisplay()
		{
			remoteStore.UpdateTree();
		}
	}
}
