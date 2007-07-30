using System;

namespace Mono.Debugger.Frontend
{
	[Serializable]
	public abstract class RemoteTreeModification
	{
		[Serializable]
		public class InsertNode: RemoteTreeModification
		{
			public RemoteTreePath PartenNodePath;
			public int NodeIndex;
			
			public InsertNode(RemoteTreePath partenNodePath, int nodeIndex)
			{
				this.PartenNodePath = partenNodePath;
				this.NodeIndex = nodeIndex;
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
		}
	}
}
