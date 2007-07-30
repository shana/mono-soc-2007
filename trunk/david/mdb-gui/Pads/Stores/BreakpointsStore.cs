using System;

using Mono.Debugger;
using Mono.Debugger.Languages;
using Mono.Debugger.Frontend.TreeModel;

namespace Mono.Debugger.Frontend
{
	public class BreakpointsStore: RemoteTreeStore
	{
		Interpreter interpreter;
		
		public const int ColumnImage       = 0;
		public const int ColumnID          = 1;
		public const int ColumnEnabled     = 2;
		public const int ColumnActivated   = 3;
		public const int ColumnThreadGroup = 4;
		public const int ColumnLocation    = 5;
		
		public new static Type[] ColumnTypes = new Type[] {
			typeof (Gdk.Pixbuf),
			typeof (int),
			typeof (string),
			typeof (string),
			typeof (string),
			typeof (string)
		};
		
		public BreakpointsStore(Interpreter interpreter): base(ColumnTypes)
		{
			this.interpreter = interpreter;
		}
		
		public void UpdateTree()
		{
			Event[] events = interpreter.Session.Events;
			
			RootNode.Clear();
			
			foreach (Event handle in events) {
				if (handle is SourceBreakpoint) {
					RemoteTreeNode node = RootNode.AppendNode();
					node.SetValue(ColumnImage, handle.IsEnabled && handle.IsActivated ? Pixmaps.Breakpoint : Pixmaps.BreakpointDisabled);
					node.SetValue(ColumnID, handle.Index);
					node.SetValue(ColumnEnabled, handle.IsEnabled ? "Yes" : "No");
					node.SetValue(ColumnActivated, handle.IsActivated ? "Yes" : "No");
					node.SetValue(ColumnThreadGroup, handle.ThreadGroup != null ? handle.ThreadGroup.Name : "global");
					node.SetValue(ColumnLocation, handle.Name);
				}
			}
		}
	}
}
