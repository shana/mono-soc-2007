using System;

using Mono.Debugger;
using Mono.Debugger.Languages;
using Mono.Debugger.Frontend.TreeModel;

namespace Mono.Debugger.Frontend
{
	public class LocalsStore: RemoteTreeStore
	{
		Interpreter interpreter;
		
		public const int ColumnFullName = 0;
		public const int ColumnImage    = 1;
		public const int ColumnName     = 2;
		public const int ColumnValue    = 3;
		public const int ColumnType     = 4;
		
		public static Type[] ColumnTypes = new Type[] {
			typeof(string),
			typeof(Gdk.Pixbuf),
			typeof(string),
			typeof(string),
			typeof(string)
		};
		
		public LocalsStore(Interpreter interpreter)
		{
			this.interpreter = interpreter;
		}
		
		public void UpdateTree()
		{
			StackFrame currentFrame;
			
			try {
				currentFrame = interpreter.CurrentThread.GetBacktrace().CurrentFrame;
			} catch {
				RootNode.Clear();
				return;
			}
			
			RootNode.Clear();
			
			foreach (TargetVariable variable in currentFrame.Method.Parameters) {
				AbstractNode node = NodeFactory.Create(variable, currentFrame);
				AppendNode(RootNode, node);
			}
				
			foreach (TargetVariable variable in currentFrame.Locals) {
				AbstractNode node = NodeFactory.Create(variable, currentFrame);
				AppendNode(RootNode, node);
			}
			
			if (currentFrame.Method.HasThis) {
				AbstractNode node = NodeFactory.Create(currentFrame.Method.This, currentFrame);
				AppendNode(RootNode, node);
			}
		}
		
		void AppendNode(RemoteTreeNode treeNode, AbstractNode node)
		{
			string idPrefix;
			if (treeNode == RootNode) {
				idPrefix = String.Empty;
			} else {
				idPrefix = (string)treeNode.GetValue(ColumnFullName) + ".";
			}
			string id = idPrefix + node.Name;
			
			RemoteTreeNode newNode = treeNode.AppendNode();
			newNode.Tag = node;
			newNode.SetValue(ColumnFullName, id);
			newNode.SetValue(ColumnImage, node.Image);
			newNode.SetValue(ColumnName, node.Name);
			newNode.SetValue(ColumnValue, node.Value);
			newNode.SetValue(ColumnType, node.Type);
			
			// Placeholder so that the item is expandable
			if (node.HasChildNodes) {
				newNode.AppendNode();
			}
		}
		
		/// <summary> Expands the given node </summary>
		/// <returns> Number of child nodes </returns>
		public int ExpandNode(RemoteTreePath path)
		{
			RemoteTreeNode treeNode = GetNode(path);
			
			// Remove all current children
			treeNode.Clear();
			
			// Add childs
			AbstractNode[] childs;
			try {
				childs = ((AbstractNode)treeNode.Tag).ChildNodes;
			} catch {
				AppendNode(treeNode, new ErrorNode(String.Empty, "Can not get child nodes"));
				return 1;
			}
			foreach(AbstractNode child in childs) {
				AppendNode(treeNode, child);
			}
			return childs.Length;
		}
	}
}
