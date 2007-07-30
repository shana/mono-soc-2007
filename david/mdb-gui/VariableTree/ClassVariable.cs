using System;
using System.Collections;

using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend.TreeModel
{
	public class ClassVariable: AbstractVariable
	{
		string name;
		StackFrame stackFrame;
		TargetClassObject obj;
		
		public override PixmapRef Image {
			get { return Pixmaps.PublicClass; }
		}
		
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
		
		public override AbstractVariable[] ChildNodes {
			get { return GetChildNodes(); }
		}
		
		public ClassVariable(string name, StackFrame stackFrame, TargetClassObject obj)
		{
			this.name = name;
			this.stackFrame = stackFrame;
			this.obj = obj;
		}
		
		AbstractVariable[] GetChildNodes()
		{
			ArrayList nodes = new ArrayList();
			
			TargetClassObject parent = obj.GetParentObject(stackFrame.Thread);
			if (parent != null && parent.Type != parent.Type.Language.ObjectType) {
				nodes.Add(new ClassVariable("<Base class>", stackFrame, parent));
			}
			
			if (obj.Type.StaticFields.Length > 0) {
				nodes.Add(new StaticMembers(this));
			}
			
			nodes.AddRange(FieldsToNodes(obj.Type.Fields));
			
			return (AbstractVariable[])nodes.ToArray(typeof(AbstractVariable));
		}
		
		AbstractVariable[] FieldsToNodes(TargetFieldInfo[] fields)
		{
			ArrayList nodes = new ArrayList();
			
			foreach(TargetFieldInfo field in fields) {
				AbstractVariable node;
				try {
					TargetObject fobj = obj.GetField(stackFrame.Thread, field);
					node = VariableFactory.Create(field.Name, fobj, stackFrame);
				} catch {
					node = new ErrorVariable(field.Name, "Can not get field value");
				}
				nodes.Add(node);
			}
			
			return (AbstractVariable[])nodes.ToArray(typeof(AbstractVariable));
		}
		
		class StaticMembers: AbstractVariable
		{
			ClassVariable parentNode;
			
			public override string Name {
				get { return "<Static members>"; }
			}
			
			public override bool HasChildNodes {
				get { return true; }
			}
			
			public override AbstractVariable[] ChildNodes {
				get {
					return parentNode.FieldsToNodes(parentNode.obj.Type.StaticFields);
				}
			}
			
			public StaticMembers(ClassVariable parentNode)
			{
				this.parentNode = parentNode;
			}
		}
	}
}
