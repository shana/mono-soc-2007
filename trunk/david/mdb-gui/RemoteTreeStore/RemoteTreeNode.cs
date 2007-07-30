using System;
using System.Collections;

namespace Mono.Debugger.Frontend
{
	/// <summary>
	/// A node of RemoteTreeStore
	/// </summary>
	public class RemoteTreeNode
	{
		RemoteTreeStore remoteTreeStore;
		RemoteTreeNode parent;
		ArrayList values = new ArrayList();
		ArrayList childs = new ArrayList();
		object tag;
		
		public RemoteTreeStore RemoteTreeStore {
			get { return remoteTreeStore; }
		}
		
		public RemoteTreeNode Parent {
			get { return parent; }
		}
		
		public ArrayList Values {
			get { return values; }
		}
		
		public int ChildCount {
			get { return childs.Count; }
		}
		
		/// <summary>
		/// User defined value associated with this node.  It is not remoted.
		/// </summary>
		public object Tag {
			get { return tag; }
			set { tag = value; }
		}
		
		int myIndexHint = 0;
		
		/// <summary> Zero based index of this node in the parent </summary>
		public int MyIndex {
			get {
				if (parent == null) {
					// This is the root node
					throw new Exception("Root node does not have an index");
				} else {
					if (Parent.ChildCount > myIndexHint && Parent.GetChild(myIndexHint) == this) {
						return myIndexHint;
					} else {
						myIndexHint = Parent.childs.IndexOf(this);
						if (myIndexHint == -1) {
							throw new Exception("Inconsistent data.  Child not found.");
						}
						return myIndexHint;
					}
				}
			}
		}
		
		public RemoteTreePath Path {
			get {
				if (parent == null) {
					// This is the root node
					return RemoteTreePath.Empty;
				} else {
					return Parent.Path.AppendIndex(MyIndex);
				}
			}
		}
		
		public RemoteTreeNode(RemoteTreeStore remoteTreeStore, RemoteTreeNode parent)
		{
			this.remoteTreeStore = remoteTreeStore;
			this.parent = parent;
		}
		
		/// <summary> Insert a new node so that it is located at the given index </summary>
		public RemoteTreeNode InsertNode(int index)
		{
			remoteTreeStore.AddModification(new RemoteTreeModification.InsertNode(this.Path, index));
			
			RemoteTreeNode newNode = new RemoteTreeNode(remoteTreeStore, this);
			childs.Insert(index, newNode);
			return newNode;
		}
		
		/// <summary> Remove this node </summary>
		public void Remove()
		{
			remoteTreeStore.AddModification(new RemoteTreeModification.RemoveNode(this.Path));
			
			parent.childs.RemoveAt(MyIndex);
		}
		
		/// <summary> Set one of the values in this node </summary>
		public void SetValue(int columnIndex, object newValue)
		{
			// Do not change if the value is same
			object oldValue = GetValue(columnIndex);
			if ((newValue == null && oldValue == null) || 
			    (newValue != null && oldValue != null && newValue.Equals(oldValue))) {
				return;
			}
			
			remoteTreeStore.AddModification(new RemoteTreeModification.UpdateNode(this.Path, columnIndex, newValue));
			
			while(columnIndex >= Values.Count) {
				Values.Add(null);
			}
			Values[columnIndex] = newValue;
		}
		
		// Convenience methods:
		
		/// <summary> Get a child with given index </summary>
		public RemoteTreeNode GetChild(int index)
		{
			return (RemoteTreeNode)childs[index];
		}
		
		/// <summary> Get a value in the given column </summary>
		public object GetValue(int columnIndex)
		{
			if (columnIndex >= this.Values.Count) {
				return null;
			} else {
				return this.Values[columnIndex];
			}
		}
		
		/// <summary> Insert a node and set its values </summary>
		public RemoteTreeNode InsertNode(int index, params object[] values)
		{
			RemoteTreeNode newNode = InsertNode(index);
			GetChild(index).SetValues(values);
			return newNode;
		}
		
		/// <summary> Add a node at the end </summary>
		public RemoteTreeNode AppendNode()
		{
			return InsertNode(childs.Count);
		}
		
		/// <summary> Add a node at the end and set its values </summary>
		public RemoteTreeNode AppendNode(params object[] values)
		{
			return InsertNode(childs.Count, values);
		}
		
		/// <summary> Add a node at the start </summary>
		public RemoteTreeNode PrependNode()
		{
			return InsertNode(0);
		}
		
		/// <summary> Add a node at the start and set its values </summary>
		public RemoteTreeNode PrependNode(params object[] values)
		{
			return InsertNode(0, values);
		}
		
		/// <summary> Set values of this node </summary>
		public void SetValues(params object[] newValues)
		{
			for(int i = 0; i < newValues.Length; i++) {
				SetValue(i, newValues[i]);
			}
		}
		
		/// <summary> Remove all child nodes </summary>
		public void Clear()
		{
			while (ChildCount > 0) {
				GetChild(0).Remove();
			}
		}
	}
}
