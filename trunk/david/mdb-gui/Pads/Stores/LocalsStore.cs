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
		
		AbstractNode rootNode;
		
		public const int ColumnFullName     = 0;
		public const int ColumnUpdateChilds = 1;
		public const int ColumnImage        = 2;
		public const int ColumnName         = 3;
		public const int ColumnValue        = 4;
		public const int ColumnType         = 5;
		
		public static Type[] ColumnTypes = new Type[] {
			typeof(string),
			typeof(bool),
			typeof(Gdk.Pixbuf),
			typeof(string),
			typeof(string),
			typeof(string)
		};
		
		public LocalsStore(Interpreter interpreter)
		{
			this.interpreter = interpreter;
			this.rootNode = new LocalsRootNode(interpreter);
			this.RootNode.SetValue(ColumnUpdateChilds, true);
		}
		
		public void UpdateTree()
		{
			UpdateNodeRecursive(RootNode, rootNode);
		}
		
		public void ExpandNode(RemoteTreePath path)
		{
			RemoteTreeNode node = GetNode(path);
			node.SetValue(ColumnUpdateChilds, true);
			UpdateNodeRecursive(node, (AbstractNode)node.Tag);
		}
		
		public void CollapseNode(RemoteTreePath path)
		{
			GetNode(path).SetValue(ColumnUpdateChilds, false);
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
			bool updateChilds = node.GetValue(ColumnUpdateChilds) != null && (bool)node.GetValue(ColumnUpdateChilds);
			if (!variable.HasChildNodes) {
				// Does not have childs
				node.Clear();
			} else if (!updateChilds) {
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
				
				Console.WriteLine("{0} current nodes, {1} new variables", node.ChildCount, variables.Length);
				
				// Iterate over the current tree nodes and update them
				// Try to do it with minimal number of changes to the tree
				for(int i = 0; i < node.ChildCount; /* no-op */ ) {
					string childNodeName = (string)node.GetChild(i).GetValue(ColumnName);
					
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
					Console.WriteLine("Looking for variable '{0}': index = {1}", childNodeName, varIndex);
					
					// Not found - remove this node
					if (varIndex == -1) {
						node.GetChild(i).Remove();
						continue;
					}
					
					// Insert the variables before the match
					while(i < varIndex) {
						UpdateNodeRecursive(node.InsertNode(i), variables[i]);
						i++;
					}
					
					// Update the match
					UpdateNodeRecursive(node.GetChild(i), variables[i]);
					i++;
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
