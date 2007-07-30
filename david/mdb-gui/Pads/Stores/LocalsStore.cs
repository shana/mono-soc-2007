using System;
using System.Collections;

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
		}
		
		AbstractNode[] GetRootVariables()
		{
			ArrayList rootVariables = new ArrayList();
			
			StackFrame currentFrame;
			
			try {
				currentFrame = interpreter.CurrentThread.GetBacktrace().CurrentFrame;
			} catch {
				return new AbstractNode[0];
			}
			
			foreach (TargetVariable variable in currentFrame.Method.Parameters) {
				rootVariables.Add(NodeFactory.Create(variable, currentFrame));
			}
				
			foreach (TargetVariable variable in currentFrame.Locals) {
				rootVariables.Add(NodeFactory.Create(variable, currentFrame));
			}
			
			if (currentFrame.Method.HasThis) {
				rootVariables.Add(NodeFactory.Create(currentFrame.Method.This, currentFrame));
			}
			
			return (AbstractNode[])rootVariables.ToArray(typeof(AbstractNode));
		}
		
		void UpdateNodeRecursive(RemoteTreeNode node, AbstractNode variable)
		{
			// Get full name of the node
			string fullNamePrefix;
			if (node != RootNode && node.Parent != RootNode) {
				fullNamePrefix = (string)node.Parent.GetValue(ColumnFullName) + ".";
			} else {
				fullNamePrefix = String.Empty;
			}
			string fullName = fullNamePrefix + variable.Name;
			
			// Update the node itself
			node.Tag = variable;
			node.SetValue(ColumnFullName, fullName);
			node.SetValue(ColumnImage, variable.Image);
			node.SetValue(ColumnName, variable.Name);
			node.SetValue(ColumnValue, variable.Value);
			node.SetValue(ColumnType, variable.Type);
			
			// Recursively update the childs of this node
			if (!variable.HasChildNodes) {
				// Does not have childs
				node.Clear();
			} if (!updateChilds) {
				// Has childs but do not update them
				if (node.ChildCount == 0) {
					// Placeholder so that the item is expandable
					node.AppendNode();
				}
			} else {
				// Update childs
				AbstractNode[] variables;
				try {
					variables = ((AbstractNode)node.Tag).ChildNodes;
				} catch {
					variables = new AbstractNode[] { new ErrorNode(String.Empty, "Can not get child nodes") };
				}
				
				// Iterate over the current tree nodes and update them
				// Try to do it with minimal number of changes to the tree
				for(int i = 0; i < node.ChildCount; i++) {
					RemoteTreeNode childNode = node.GetChild(i);
					string childNodeName = (string)childNode.GetValue(ColumnName);
					
					// Update 'i'th node to 'i'th variable
					
					// Find a variable with the same name as this node
					// (includes the case where there are no variables left)
					int varIndex = -1;
					for(int j = i; j < variables.Length; j++) {
						if (variables[j].Name == childNodeName) {
							varIndex = j;
							break;
						}
					}
					
					// Not found - remove this node
					if (varIndex == -1) {
						childNode.Remove();
						continue;
					}
					
					// Insert the variables before the match
					for(int j = i; j < varIndex; j++) {
						RemoteTreeNode newNode = childNode.InsertNode(j);
						UpdateNodeRecursive(newNode, variables[j]);
					}
					
					// Update the match
					UpdateNodeRecursive(childNode, variables[varIndex]);
				}
				
				// Add any variables left over
				for(int i = node.ChildCount; i < variables.Length; i++) {
					RemoteTreeNode newNode = node.AppendNode();
					UpdateNodeRecursive(newNode, variables[i]);
				}
			}
		}
	}
}
