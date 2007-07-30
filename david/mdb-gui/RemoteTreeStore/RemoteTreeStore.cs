using System;
using System.Collections;

namespace Mono.Debugger.Frontend
{
	/// <summary>
	/// A tree store designed to be mantained in one process and shown in other.
	/// </summary>
	public class RemoteTreeStore
	{
		RemoteTreeNode rootNode;
		ArrayList modifications = new ArrayList();
		
		Hashtable referenceToNode = new Hashtable();
		
		public RemoteTreeNode RootNode {
			get { return rootNode; }
		}
		
		public RemoteTreeStore()
		{
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
		
		public RemoteTreeNode GetNode(RemoteTreeNodeRef reference)
		{
			if (referenceToNode.Contains(reference)) {
				return (RemoteTreeNode)referenceToNode[reference];
			} else {
				return null;
			}
		}
		
		internal void AddModification(RemoteTreeModification modification)
		{
			modifications.Add(modification);
		}
		
		internal void NotifyNodeAdded(RemoteTreeNode node)
		{
			referenceToNode.Add(node.Reference, node);
		}
		
		internal void NotifyNodeRemoved(RemoteTreeNode node)
		{
			referenceToNode.Remove(node.Reference);
			
			for(int i = 0; i < node.ChildCount; i++) {
				NotifyNodeRemoved(node.GetChild(i));
			}
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
