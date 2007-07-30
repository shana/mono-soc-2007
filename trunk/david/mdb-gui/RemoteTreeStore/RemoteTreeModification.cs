using System;

namespace Mono.Debugger.Frontend
{
	[Serializable]
	public abstract class RemoteTreeModification
	{
		[Serializable]
		public class InsertNode: RemoteTreeModification
		{
			public RemoteTreePath ParentNodePath;
			public int NodeIndex;
			
			public InsertNode(RemoteTreePath parentNodePath, int nodeIndex)
			{
				this.ParentNodePath = parentNodePath;
				this.NodeIndex = nodeIndex;
			}
			
			public override string ToString()
			{
				return string.Format("[InsertNode ParentNodePath={0} NodeIndex={1}]", this.ParentNodePath, this.NodeIndex);
			}
		}
		
		[Serializable]
		public class RemoveNode: RemoteTreeModification
		{
			public RemoteTreePath NodePath;
			
			public RemoveNode(RemoteTreePath nodePath)
			{
				this.NodePath = nodePath;
			}
			
			public override string ToString()
			{
				return string.Format("[RemoveNode NodePath={0}]", this.NodePath);
			}
		}
		
		[Serializable]
		public class UpdateNode: RemoteTreeModification
		{
			public RemoteTreePath NodePath;
			public int ColumnIndex;
			public object Value;
			
			public UpdateNode(RemoteTreePath nodePath, int columnIndex, object value)
			{
				this.NodePath = nodePath;
				this.ColumnIndex = columnIndex;
				this.Value = value;
			}
			
			public override string ToString()
			{
				return string.Format("[UpdateNode NodePath={0} ColumnIndex={1} Value={2}]", this.NodePath, this.ColumnIndex, this.Value);
			}
		}
	}
}
