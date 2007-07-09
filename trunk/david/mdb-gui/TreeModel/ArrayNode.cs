using System;
using System.Text;

using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend.TreeModel
{
	public class ArrayNode: AbstractNode
	{
		string name;
		StackFrame stackFrame;
		TargetArrayObject obj;
		
		ArraySubsetNode universalSubset;
		
		public override Gdk.Pixbuf Image {
			get { return Pixmaps.PublicClass; }
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Value {
			get { return this.Type; }
		}
		
		public override string Type {
			get { return obj.TypeName; }
		}
		
		public override bool HasChildNodes {
			get { return universalSubset.HasChildNodes; }
		}
		
		public override AbstractNode[] ChildNodes {
			get { return universalSubset.ChildNodes; }
		}
		
		public ArrayNode(string name, StackFrame stackFrame, TargetArrayObject obj)
		{
			this.name = name;
			this.stackFrame = stackFrame;
			this.obj = obj;
			
			universalSubset = new ArraySubsetNode(stackFrame, obj, new int[0]);
		}
	}
}
