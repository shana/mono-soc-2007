using System;
using System.Collections;

using Mono.Debugger;
using Mono.Debugger.Languages;
using Mono.Debugger.Frontend.TreeModel;

namespace Mono.Debugger.Frontend
{
	public class ThreadsStore: RemoteTreeStore
	{
		Interpreter interpreter;
		
		Hashtable threadRows = new Hashtable();
		
		public const int ColumnSelected = 0;
		public const int ColumnID       = 1;
		public const int ColumnPID      = 2;
		public const int ColumnTID      = 3;
		public const int ColumnName     = 4;
		public const int ColumnState    = 5;
		public const int ColumnLocation = 6;
		
		public new static Type[] ColumnTypes = new Type[] {
			typeof (Gdk.Pixbuf),
			typeof (int),
			typeof (int),
			typeof (string),
			typeof (string),
			typeof (string),
			typeof (string)
		};
		
		public ThreadsStore(Interpreter interpreter): base(ColumnTypes)
		{
			this.interpreter = interpreter;
		}
		
		public void SelectThread(int id)
		{
			interpreter.CurrentThread = interpreter.GetThread(id);
		}
		
		void UpdateThread (Thread thread)
		{
			RemoteTreeNode node = (RemoteTreeNode)threadRows[thread];

			if (node == null) {
				node = RootNode.AppendNode();
				threadRows.Add(thread, node);
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
			RemoteTreeNode node = (RemoteTreeNode)threadRows[thread];
			
			if (node != null) {
				node.Remove();
			}
			
			threadRows.Remove(thread);
		}
		
		public void UpdateTree()
		{
			Hashtable threadsToRemove = (Hashtable)threadRows.Clone();
			
			if (interpreter.HasTarget) {
				foreach (Process process in interpreter.Processes) {
					foreach (Thread thread in process.GetThreads()) {
						UpdateThread(thread);
						threadsToRemove.Remove(thread);
					}
				}
			}
			
			foreach (Thread thread in threadsToRemove.Keys) {
				RemoveThread(thread);
			}
		}
	}
}
