using System;
using System.Text;

using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend.TreeModel
{
	public class ArrayVariable: AbstractVariable
	{
		string name;
		StackFrame stackFrame;
		TargetArrayObject obj;
		
		ArraySubsetVariable universalSubset;
		
		public override PixmapRef Image {
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
		
		public override AbstractVariable[] ChildNodes {
			get { return universalSubset.ChildNodes; }
		}
		
		public ArrayVariable(string name, StackFrame stackFrame, TargetArrayObject obj)
		{
			this.name = name;
			this.stackFrame = stackFrame;
			this.obj = obj;
			
			universalSubset = new ArraySubsetVariable(stackFrame, obj, new int[0]);
		}
	}
}
