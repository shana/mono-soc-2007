using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using Gtk;

using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend
{
	public class LocalsPad : Gtk.ScrolledWindow
	{
		Interpreter interpreter;
		
		Gtk.TreeView tree;
		Gtk.TreeStore store;
		
		internal const int NAME_COL = 0;
		internal const int VALUE_COL = 1;
		internal const int TYPE_COL = 2;
		internal const int RAW_VIEW_COL = 3;
		internal const int PIXBUF_COL = 4;
		
		public LocalsPad(Interpreter interpreter)
		{
			this.interpreter = interpreter;
			
			this.ShadowType = ShadowType.In;
			
			store = new TreeStore (typeof (string),
						    typeof (string),
						    typeof (string),
						    typeof (bool),
						    typeof (Gdk.Pixbuf));
			
			tree = new TreeView (store);
			tree.RulesHint = true;
			tree.HeadersVisible = true;
			
			TreeViewColumn nameCol = new TreeViewColumn ();
			CellRenderer nameRenderer = new CellRendererText ();
			CellRenderer iconRenderer = new CellRendererPixbuf ();
			nameCol.Title = "Name";
			nameCol.PackStart (iconRenderer, false);
			nameCol.PackStart (nameRenderer, true);
			nameCol.AddAttribute (iconRenderer, "pixbuf", PIXBUF_COL);
			nameCol.AddAttribute (nameRenderer, "text", NAME_COL);
			nameCol.Resizable = true;
			nameCol.Alignment = 0.0f;
			tree.AppendColumn (nameCol);
			
			TreeViewColumn valueCol = new TreeViewColumn ();
			CellRenderer valueRenderer = new CellRendererText ();
			valueCol.Title = "Value";
			valueCol.PackStart (valueRenderer, true);
			valueCol.AddAttribute (valueRenderer, "text", VALUE_COL);
			valueCol.Resizable = true;
			nameCol.Alignment = 0.0f;
			tree.AppendColumn (valueCol);
			
			TreeViewColumn typeCol = new TreeViewColumn ();
			CellRenderer typeRenderer = new CellRendererText ();
			typeCol.Title = "Type";
			typeCol.PackStart (typeRenderer, true);
			typeCol.AddAttribute (typeRenderer, "text", TYPE_COL);
			typeCol.Resizable = true;
			nameCol.Alignment = 0.0f;
			tree.AppendColumn (typeCol);
			
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
			foreach (TargetVariable var in localVars) {
				TargetObject obj = var.GetObject(currentFrame);
				
				string val;
				if (obj == null || obj.IsNull) {
					val = "<null>";
				} else {
					switch (obj.Kind) {
						case TargetObjectKind.Array:
							val = "<Array>";
							break;
						case TargetObjectKind.Pointer:
							val = "<Pointer>";
							break;
						case TargetObjectKind.Object:
							val = "<Object>";
							break;
						case TargetObjectKind.Class:
							val = "<Class>";
							break;
						case TargetObjectKind.Struct:
							val = "<Struct>";
							break;
						case TargetObjectKind.Fundamental:
							val = ((TargetFundamentalObject)obj).GetObject(currentThread).ToString();
							break;
						case TargetObjectKind.Enum:
							val = "<Enum>";
							break;
						default:
							val = "<Unknown type>";
							break;
					}
				}
				
				string type;
				if (obj != null) {
					type = obj.TypeName;
				} else {
					type = string.Empty;
				}
				
				store.AppendValues(
					var.Name,      // Name
					val,           // Value
					type,          // Type
					false,         // Raw-view
					null           // Pixbuf
				);
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