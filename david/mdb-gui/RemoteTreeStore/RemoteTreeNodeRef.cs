using System;

namespace Mono.Debugger.Frontend
{
	/// <summary>
	/// Serializable reference to RemoteTreeNode
	/// </summary>
	[Serializable]
	public class RemoteTreeNodeRef
	{
		static long nextId = 0;
		
		long id;
		
		public long ID {
			get { return id; }
		}
		
		public RemoteTreeNodeRef()
		{
			this.id = nextId;
			nextId++;
		}
		
		public override string ToString()
		{
			return string.Format("[RemoteTreeNodeRef Id={0}]", this.id);
		}
		
		public override int GetHashCode()
		{
			return id.GetHashCode();
		}
		
		public override bool Equals(object obj)
		{
			if (!(obj is RemoteTreeNodeRef)) return false; 
			RemoteTreeNodeRef myRemoteTreeNodeRef = (RemoteTreeNodeRef)obj;
			return this.id == myRemoteTreeNodeRef.id;
		}
		
	}
}
