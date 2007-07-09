using System;
using System.Collections;

using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend.TreeModel
{
	public class ClassNode: AbstractNode
	{
		string name;
		StackFrame stackFrame;
		TargetClassObject obj;
		
		public override string Name {
			get { return name; }
		}
		
		public override string Value {
			get { return obj.TypeName; }
		}
		
		public override string Type {
			get { return obj.TypeName; }
		}
		
		public override bool HasChildNodes {
			get { return true; }
		}
		
		public override AbstractNode[] ChildNodes {
			get { return GetChildNodes(); }
		}
		
		public ClassNode(string name, StackFrame stackFrame, TargetClassObject obj)
		{
			this.name = name;
			this.stackFrame = stackFrame;
			this.obj = obj;
		}
		
		AbstractNode[] GetChildNodes()
		{
			ArrayList nodes = new ArrayList();
			
			TargetClassObject parent = obj.GetParentObject(stackFrame.Thread);
			if (parent != null && parent.Type != parent.Type.Language.ObjectType) {
				nodes.Add(new ClassNode("<Base class>", stackFrame, parent));
			}
			
			TargetFieldInfo[] fields = obj.Type.Fields;
			foreach(TargetFieldInfo field in fields) {
				AbstractNode node;
				try {
					TargetObject fobj = obj.GetField(stackFrame.Thread, field);
					node = NodeFactory.Create(field.Name, fobj, stackFrame);
				} catch {
					node = new ErrorNode(field.Name, "Can not get field value");
				}
				nodes.Add(node);
			}
			
			return (AbstractNode[])nodes.ToArray(typeof(AbstractNode));
		}
	}
}
