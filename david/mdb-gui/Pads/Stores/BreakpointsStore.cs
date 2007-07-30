using System;
using System.Collections;

using Mono.Debugger;
using Mono.Debugger.Languages;
using Mono.Debugger.Frontend.TreeModel;

namespace Mono.Debugger.Frontend
{
	public class BreakpointsStore: RemoteTreeStore
	{
		Interpreter interpreter;
		
		Hashtable breakpointToTreeNode = new Hashtable();
		
		public const int ColumnImage       = 0;
		public const int ColumnID          = 1;
		public const int ColumnEnabled     = 2;
		public const int ColumnActivated   = 3;
		public const int ColumnThreadGroup = 4;
		public const int ColumnLocation    = 5;
		
		public static Type[] ColumnTypes = new Type[] {
			typeof (Gdk.Pixbuf),
			typeof (int),
			typeof (string),
			typeof (string),
			typeof (string),
			typeof (string)
		};
		
		public BreakpointsStore(Interpreter interpreter)
		{
			this.interpreter = interpreter;
		}
		
		void UpdateBreakpoint(SourceBreakpoint breakpoint)
		{
			RemoteTreeNode node = (RemoteTreeNode)breakpointToTreeNode[breakpoint];

			if (node == null) {
				node = RootNode.AppendNode();
				breakpointToTreeNode.Add(breakpoint, node);
			}
			
			node.SetValue(ColumnImage, breakpoint.IsEnabled && breakpoint.IsActivated ? Pixmaps.Breakpoint : Pixmaps.BreakpointDisabled);
			node.SetValue(ColumnID, breakpoint.Index);
			node.SetValue(ColumnEnabled, breakpoint.IsEnabled ? "Yes" : "No");
			node.SetValue(ColumnActivated, breakpoint.IsActivated ? "Yes" : "No");
			node.SetValue(ColumnThreadGroup, breakpoint.ThreadGroup != null ? breakpoint.ThreadGroup.Name : "global");
			node.SetValue(ColumnLocation, breakpoint.Name);
		}
		
		void RemoveBreakpoint(SourceBreakpoint breakpoint)
		{
			RemoteTreeNode node = (RemoteTreeNode)breakpointToTreeNode[breakpoint];
			
			if (node != null) {
				node.Remove();
			}
			
			breakpointToTreeNode.Remove(breakpoint);
		}
		
		public void UpdateTree()
		{
			Hashtable breakpointsToRemove = (Hashtable)breakpointToTreeNode.Clone();
			
			foreach (Event handle in interpreter.Session.Events) {
				if (handle is SourceBreakpoint) {
					UpdateBreakpoint((SourceBreakpoint)handle);
					breakpointsToRemove.Remove(handle); // Ok if not in the table
				}
			}
			
			foreach (SourceBreakpoint breakpoint in breakpointsToRemove.Keys) {
				RemoveBreakpoint(breakpoint);
			}
		}
	}
}
