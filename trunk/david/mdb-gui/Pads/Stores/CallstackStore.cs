using System;

using Mono.Debugger;
using Mono.Debugger.Languages;
using Mono.Debugger.Frontend.TreeModel;

namespace Mono.Debugger.Frontend
{
	public class CallstackStore: RemoteTreeStore
	{
		DebuggerService debuggerService;
		Interpreter interpreter;
		
		public const int ColumnReference = 0;
		public const int ColumnSelected  = 1;
		public const int ColumnLevel     = 2;
		public const int ColumnAddress   = 3;
		public const int ColumnName      = 4;
		public const int ColumnSource    = 5;
		
		public static Type[] ColumnTypes = new Type[] {
			typeof(RemoteTreeNodeRef),
			typeof(Gdk.Pixbuf),
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string)
		};
		
		public CallstackStore(DebuggerService debuggerService, Interpreter interpreter)
		{
			this.debuggerService = debuggerService;
			this.interpreter = interpreter;
		}
		
		public void SelectFrame(int index)
		{
			if (interpreter.HasCurrentThread &&
			    interpreter.CurrentThread.GetBacktrace() != null &&
			    interpreter.CurrentThread.GetBacktrace().Frames.Length > index) {
				
				interpreter.CurrentThread.GetBacktrace().CurrentFrameIndex = index;
				debuggerService.NotifyStateChange();
			}
		}
		
		public void UpdateTree(ref bool abort)
		{
			StackFrame[] callstack;
			int currentFrameIndex;
			
			try {
				callstack = interpreter.CurrentThread.GetBacktrace().Frames;
				currentFrameIndex = interpreter.CurrentThread.GetBacktrace().CurrentFrameIndex;
			} catch {
				RootNode.Clear();
				return;
			}
			
			// Adjust the number of rows to match the callstack length
			while (RootNode.ChildCount > callstack.Length) {
				// Delete first row
				RootNode.GetChild(0).Remove();
			}
			while (RootNode.ChildCount < callstack.Length) {
				// Add extra row
				RootNode.PrependNode();
			}
			
			// Update the values of the rows (in reverse order)
			for (int i = callstack.Length - 1; i >= 0; i--) {
				if (abort) return;
				StackFrame frame = callstack[i];
				
				// Get the name
				string name;
				if (frame.Method != null) {
					if (frame.Method.IsLoaded) {
						long offset = frame.TargetAddress - frame.Method.StartAddress;
						if (offset >= 0)
							name = String.Format("{0}+0x{1:x}", frame.Method.Name, offset);
						else
							name = String.Format("{0}-0x{1:x}", frame.Method.Name, -offset);
					} else {
						name = String.Format("{0}", frame.Method.Name);
					}
				} else if (frame.Name != null)
					name = frame.Name.ToString();
				else
					name = string.Empty;
				
				// Get the source file
				string source;
				try {
					source = frame.SourceAddress.Name;
				} catch {
					source = string.Empty;
				}
				
				RemoteTreeNode node = RootNode.GetChild(i);
				node.SetValue(ColumnSelected, i == currentFrameIndex ? Pixmaps.Arrow : Pixmaps.Empty);
				node.SetValue(ColumnLevel, "#" + frame.Level);
				node.SetValue(ColumnAddress, frame.TargetAddress.ToString());
				node.SetValue(ColumnName, name);
				node.SetValue(ColumnSource, source);
			}
		}
	}
}
