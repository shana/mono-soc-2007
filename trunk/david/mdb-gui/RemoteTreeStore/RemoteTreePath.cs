using System;
using System.Text;

namespace Mono.Debugger.Frontend
{
	/// <summary>
	/// Path to a node in RemoteTreeStore.
	/// </summary>
	[Serializable]
	public class RemoteTreePath
	{
		public static RemoteTreePath Empty = new RemoteTreePath(new int[0]);
		
		int[] indices;
		
		public int[] Indices {
			get { return indices; }
		}
		
		public RemoteTreePath(int[] indices)
		{
			this.indices = indices;
		}
		
		public RemoteTreePath AppendIndex(int index)
		{
			int[] newPath = new int[indices.Length + 1];
			Array.Copy(indices, newPath, indices.Length);
			newPath[indices.Length] = index;
			return new RemoteTreePath(newPath);
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Root");
			foreach(int i in indices) {
				sb.Append(':');
				sb.Append(i);
			}
			return sb.ToString();
		}
	}
}
