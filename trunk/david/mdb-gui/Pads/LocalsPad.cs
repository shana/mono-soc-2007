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
	public class LocalsPad : Gtk.ScrolledWindow
	{
		MdbGui mdbGui;
		Interpreter interpreter;
		
		Gtk.TreeView tree;
		Gtk.TreeStore store;
		
		Hashtable expandedNodes = new Hashtable();
		
		internal const int ColumnNode     = 0;
		internal const int ColumnID       = 1;
		internal const int ColumnImage    = 2;
		internal const int ColumnName     = 3;
		internal const int ColumnValue    = 4;
		internal const int ColumnType     = 5;
		
		public LocalsPad(MdbGui mdbGui)
		{
			this.mdbGui = mdbGui;
			this.interpreter = mdbGui.Interpreter;
			
			this.ShadowType = ShadowType.In;
			
			store = new TreeStore (
				typeof(AbstractNode),
				typeof(string),
				typeof(Gdk.Pixbuf),
				typeof(string),
				typeof(string),
				typeof(string)
			);
			
			tree = new TreeView (store);
			tree.RulesHint = true;
			tree.HeadersVisible = true;
			
			TreeViewColumn nameCol = new TreeViewColumn ();
			CellRenderer nameRenderer = new CellRendererText ();
			CellRenderer iconRenderer = new CellRendererPixbuf ();
			nameCol.Title = "Name";
			nameCol.PackStart (iconRenderer, false);
			nameCol.PackStart (nameRenderer, true);
			nameCol.AddAttribute (iconRenderer, "pixbuf", ColumnImage);
			nameCol.AddAttribute (nameRenderer, "text", ColumnName);
			nameCol.Resizable = true;
			nameCol.Alignment = 0.0f;
			tree.AppendColumn (nameCol);
			
			TreeViewColumn valueCol = new TreeViewColumn ();
			CellRenderer valueRenderer = new CellRendererText ();
			valueCol.Title = "Value";
			valueCol.PackStart (valueRenderer, true);
			valueCol.AddAttribute (valueRenderer, "text", ColumnValue);
			valueCol.Resizable = true;
			nameCol.Alignment = 0.0f;
			tree.AppendColumn (valueCol);
			
			TreeViewColumn typeCol = new TreeViewColumn ();
			CellRenderer typeRenderer = new CellRendererText ();
			typeCol.Title = "Type";
			typeCol.PackStart (typeRenderer, true);
			typeCol.AddAttribute (typeRenderer, "text", ColumnType);
			typeCol.Resizable = true;
			nameCol.Alignment = 0.0f;
			tree.AppendColumn (typeCol);
			
			tree.RowExpanded += new RowExpandedHandler(RowExpanded);
			tree.TestCollapseRow += new TestCollapseRowHandler(TestCollapseRow);
			tree.TestExpandRow += new TestExpandRowHandler(TestExpandRow);
			
			Add (tree);
			ShowAll ();
		}
		
//		void IPadContent.Initialize (IPadWindow window)
//		{
//			window.Title = "Locals";
//			window.Icon = Stock.OutputIcon;
//		}
		
		public void UpdateDisplay ()
		{
			if (!interpreter.HasCurrentThread ||
			    interpreter.CurrentThread.CurrentFrame == null) {
				
				store.Clear();
				return;
			}
			
			Mono.Debugger.Thread currentThread = interpreter.CurrentThread;
			StackFrame currentFrame = currentThread.CurrentFrame;
			
			store.Clear();
			
			foreach (TargetVariable variable in currentFrame.Method.Parameters) {
				AbstractNode node = NodeFactory.Create(variable, currentFrame);
				AppendNode(null, node);
			}
				
			foreach (TargetVariable variable in currentFrame.Locals) {
				AbstractNode node = NodeFactory.Create(variable, currentFrame);
				AppendNode(null, node);
			}
			
			if (currentFrame.Method.HasThis) {
				AbstractNode node = NodeFactory.Create(currentFrame.Method.This, currentFrame);
				AppendNode(null, node);
			}
		}
		
		protected void TestCollapseRow(object o, TestCollapseRowArgs args)
		{
			string id = (string)store.GetValue(args.Iter, ColumnID);
			if (expandedNodes.ContainsKey(id)) {
				expandedNodes.Remove(id);
			}
			//Console.WriteLine("TestCollapseRow " + id);
		}
		
		protected void TestExpandRow(object o, TestExpandRowArgs args)
		{
			string id = (string)store.GetValue(args.Iter, ColumnID);
			expandedNodes[id] = null; // No value, just insert the key
			//Console.WriteLine("TestExpandRow " + id);
			ExpandNode(args.Path);
		}
		
		protected void RowExpanded(object o, RowExpandedArgs args)
		{
			string id = (string)store.GetValue(args.Iter, ColumnID);
			expandedNodes[id] = null; // No value, just insert the key
			//Console.WriteLine("RowExpanded " + id);
			
			int childCount = store.IterNChildren(args.Iter);
			for(int i = 0; i < childCount; i++) {
				TreePath childPath = args.Path.Copy();
				childPath.AppendIndex(i);
				
				TreeIter childIter;
				store.GetIter(out childIter, childPath);
				string childId = (string)store.GetValue(childIter, ColumnID);
				// This node was expanded in the past - expand it
				if (expandedNodes.ContainsKey(childId)) {
					//Console.WriteLine("Should be expanded: " + childId);
					tree.ExpandRow(childPath, false);
				}
			}
		}
		
		void ExpandNode(TreePath path)
		{
			TreeIter it;
			
			// Remove all current children
			while(true) {
				store.GetIter(out it, path);
				if (store.IterChildren(out it, it)) {
					store.Remove(ref it);
				} else {
					break;
				}
			}
			
			// Add childs
			store.GetIter(out it, path);
			AbstractNode node = (AbstractNode)store.GetValue(it, ColumnNode);
			AbstractNode[] childs;
			try {
				childs = node.ChildNodes;
			} catch {
				AppendNode(path, new ErrorNode(String.Empty, "Can not get child nodes"));
				return;
			}
			foreach(AbstractNode child in node.ChildNodes) {
				AppendNode(path, child);
			}
		}
		
		void AppendNode(TreePath path, AbstractNode node)
		{
			string idPrefix = String.Empty;
			if (path != null) {
				TreeIter it;
				store.GetIter(out it, path);
				idPrefix = (string)store.GetValue(it, ColumnID) + ".";
			}
			string id = idPrefix + node.Name;
			
			object[] values = new object[] {
				node,
				id,
				node.Image,
				node.Name,
				node.Value,
				node.Type
			};
			
			TreeIter iterNewRow;
			if (path == null) {
				iterNewRow = store.AppendValues(values);
			} else {
				TreeIter it;
				store.GetIter(out it, path);
				iterNewRow = store.AppendValues(it, values);
			}
			TreePath pathNewRow = store.GetPath(iterNewRow);
			
			// Placeholder so that the item is expandable
			if (node.HasChildNodes) {
				store.AppendNode(iterNewRow);
			}
			
			// This node was expanded in the past - expand it
			if (expandedNodes.ContainsKey(id)) {
				//Console.WriteLine("Should be expanded: " + id);
				tree.ExpandRow(pathNewRow, false);
			}
		}
		
		public Gtk.Widget Control {
			get {
				return this;
			}
		}
		
		public string Id {
			get { return "MonoDevelop.Debugger.LocalsPad"; }
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
