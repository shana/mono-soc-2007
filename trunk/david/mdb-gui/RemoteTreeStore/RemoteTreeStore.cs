using System;
using System.Collections;

namespace Mono.Debugger.Frontend
{
	/// <summary>
	/// A tree store designed to be mantained in one process and shown in other.
	/// </summary>
	public class RemoteTreeStore
	{
		Type[] columnTypes;
		RemoteTreeNode rootNode;
		ArrayList modifications = new ArrayList();
		
		public Type[] ColumnTypes {
			get { return columnTypes; }
		}
		
		public RemoteTreeNode RootNode {
			get { return rootNode; }
		}
		
		public RemoteTreeStore(params Type[] columnTypes)
		{
			this.columnTypes = columnTypes;
			this.rootNode = new RemoteTreeNode(this, null);
		}
		
		public RemoteTreeNode GetNode(int[] path)
		{
			return GetNode(new RemoteTreePath(path));
		}
		
		public RemoteTreeNode GetNode(RemoteTreePath path)
		{
			RemoteTreeNode current = RootNode;
			foreach(int index in path.Indices) {
				current = current.GetChild(index);
			}
			return current;
		}
		
		internal void AddModification(RemoteTreeModification modification)
		{
			modifications.Add(modification);
		}
		
		/// <summary>
		/// Get a list of modifications done to the tree.  Calling of this methods
		/// results in clearing of the list.
		/// </summary>
		public RemoteTreeModification[] GetModifications()
		{
			RemoteTreeModification[] mods = (RemoteTreeModification[])modifications.ToArray(typeof(RemoteTreeModification));
			modifications.Clear();
			return mods;
		}
	}
}
