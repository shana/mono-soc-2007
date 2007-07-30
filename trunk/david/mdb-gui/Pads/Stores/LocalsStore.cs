using System;
using System.Collections;

using Mono.Debugger;
using Mono.Debugger.Languages;
using Mono.Debugger.Frontend.TreeModel;

namespace Mono.Debugger.Frontend
{
	public class LocalsStore: RemoteTreeStore
	{
		DebuggerService debuggerService;
		Interpreter interpreter;
		
		AbstractVariable rootVariable;
		
		public const int ColumnReference    = 0;
		public const int ColumnFullName     = 1;
		public const int ColumnUpdateChilds = 2;
		public const int ColumnImage        = 3;
		public const int ColumnName         = 4;
		public const int ColumnValue        = 5;
		public const int ColumnType         = 6;
		
		public static Type[] ColumnTypes = new Type[] {
			typeof(RemoteTreeNodeRef),
			typeof(string),
			typeof(bool),
			typeof(Gdk.Pixbuf),
			typeof(string),
			typeof(string),
			typeof(string)
		};
		
		public LocalsStore(DebuggerService debuggerService, Interpreter interpreter)
		{
			this.debuggerService = debuggerService;
			this.interpreter = interpreter;
			this.rootVariable = new LocalVariablesRoot(interpreter);
			this.RootNode.SetValue(ColumnUpdateChilds, true);
		}
		
		public void UpdateTree()
		{
			UpdateNodeRecursive(RootNode, rootVariable);
		}
		
		public void ExpandNode(RemoteTreeNodeRef nodeRef)
		{
			RemoteTreeNode node = GetNode(nodeRef);
			
			// There might be a race condition and the node might have been just removed
			if (node != null) {
				node.SetValue(ColumnUpdateChilds, true);
				UpdateNodeRecursive(node, (AbstractVariable)node.Tag);
			}
		}
		
		public void CollapseNode(RemoteTreeNodeRef nodeRef)
		{
			RemoteTreeNode node = GetNode(nodeRef);
			
			// There might be a race condition and the node might have been just removed
			if (node != null) {
				node.SetValue(ColumnUpdateChilds, false);
			}
		}
		
		void UpdateNodeRecursive(RemoteTreeNode node, AbstractVariable variable)
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
				AbstractVariable[] variables;
				try {
					variables = ((AbstractVariable)node.Tag).ChildNodes;
				} catch {
					variables = new AbstractVariable[] { new ErrorVariable(String.Empty, "Can not get child nodes") };
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
