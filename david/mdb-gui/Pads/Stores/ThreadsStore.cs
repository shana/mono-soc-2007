using System;
using System.Collections;

using Mono.Debugger;
using Mono.Debugger.Languages;
using Mono.Debugger.Frontend.TreeModel;

namespace Mono.Debugger.Frontend
{
	public class ThreadsStore: RemoteTreeStore
	{
		DebuggerService debuggerService;
		Interpreter interpreter;
		
		Hashtable threadToTreeNode = new Hashtable();
		
		public const int ColumnReference = 0;
		public const int ColumnSelected  = 1;
		public const int ColumnID        = 2;
		public const int ColumnPID       = 3;
		public const int ColumnTID       = 4;
		public const int ColumnName      = 5;
		public const int ColumnState     = 6;
		public const int ColumnLocation  = 7;
		
		public static Type[] ColumnTypes = new Type[] {
			typeof(RemoteTreeNodeRef),
			typeof(Gdk.Pixbuf),
			typeof(int),
			typeof(int),
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string)
		};
		
		public ThreadsStore(DebuggerService debuggerService, Interpreter interpreter)
		{
			this.debuggerService = debuggerService;
			this.interpreter = interpreter;
		}
		
		public void SelectThread(int id)
		{
			try {
				interpreter.CurrentThread = interpreter.GetThread(id);
				debuggerService.NotifyStateChange();
			} catch {
				// There might be a race condition and the thread might have just terminated
				Console.WriteLine("Failed to select thread {0}", id);
			}
		}
		
		void UpdateThread (Thread thread)
		{
			RemoteTreeNode node = (RemoteTreeNode)threadToTreeNode[thread];

			if (node == null) {
				node = RootNode.AppendNode();
				threadToTreeNode.Add(thread, node);
			}
			
			bool current = interpreter.HasCurrentThread && interpreter.CurrentThread.ID == thread.ID;
			
			string location;
			if (thread.IsStopped) {
				try {
					location = thread.GetBacktrace().Frames[0].SourceAddress.Name;
				} catch {
					location = "";
				}
			} else {
				location = "";
			}
			
			node.SetValue(ColumnSelected, current ? Pixmaps.Arrow : Pixmaps.Empty);
			node.SetValue(ColumnID, thread.ID);
			node.SetValue(ColumnPID, thread.PID);
			node.SetValue(ColumnTID, String.Format("{0:x}", thread.TID));
			node.SetValue(ColumnName, thread.Name);
			node.SetValue(ColumnState, thread.State.ToString());
			node.SetValue(ColumnLocation, location);
		}
		
		void RemoveThread (Thread thread)
		{
			RemoteTreeNode node = (RemoteTreeNode)threadToTreeNode[thread];
			
			if (node != null) {
				node.Remove();
			}
			
			threadToTreeNode.Remove(thread);
		}
		
		public void UpdateTree(ref bool abort)
		{
			Hashtable threadsToRemove = (Hashtable)threadToTreeNode.Clone();
			
			if (interpreter.HasTarget) {
				foreach (Process process in interpreter.Processes) {
					if (abort) return;
					foreach (Thread thread in process.GetThreads()) {
						UpdateThread(thread);
						threadsToRemove.Remove(thread); // Ok if not in the table
					}
				}
			}
			
			foreach (Thread thread in threadsToRemove.Keys) {
				RemoveThread(thread);
			}
		}
	}
}
