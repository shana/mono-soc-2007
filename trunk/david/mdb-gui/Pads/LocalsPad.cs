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
		Interpreter interpreter;
		
		Gtk.TreeView tree;
		Gtk.TreeStore store;
		
		internal const int ColumnNode     = 0;
		internal const int ColumnImage    = 1;
		internal const int ColumnName     = 2;
		internal const int ColumnValue    = 3;
		internal const int ColumnType     = 4;
		
		const string imageBase = "Mono.Debugger.Frontend.pixmaps.Icons.16x16.";
		Gdk.Pixbuf imageField = Gdk.Pixbuf.LoadFromResource(imageBase + "PublicField");
		
		public LocalsPad(Interpreter interpreter)
		{
			this.interpreter = interpreter;
			
			this.ShadowType = ShadowType.In;
			
			store = new TreeStore (
				typeof(AbstractNode),
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
			TargetVariable[] localVars = currentFrame.Locals;
			foreach (TargetVariable variable in localVars) {
				AbstractNode node = NodeFactory.Create(variable, currentFrame);
				AppendNode(null, node);
			}
		}
		
		protected void TestExpandRow(object o, TestExpandRowArgs args)
		{
			TreeIter it;
			
			// Remove all current children
			while(true) {
				store.GetIter(out it, args.Path);
				if (store.IterChildren(out it, it)) {
					store.Remove(ref it);
				} else {
					break;
				}
			}
			
			// Add childs
			store.GetIter(out it, args.Path);
			AbstractNode node = (AbstractNode)store.GetValue(it, ColumnNode);
			AbstractNode[] childs;
			try {
				childs = node.ChildNodes;
			} catch {
				AppendNode(args.Path, new ErrorNode("-","Can not get child nodes"));
				return;
			}
			foreach(AbstractNode child in node.ChildNodes) {
				AppendNode(args.Path, child);
			}
		}
		
		void AppendNode(TreePath path, AbstractNode node)
		{
			object[] values = new object[] {
				node,
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
			
			// Placeholder so that the item is expandable
			if (node.HasChildNodes) {
				store.AppendNode(iterNewRow);
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
