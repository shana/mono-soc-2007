using System;

using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend.TreeModel
{
	public class FundamentalNode: AbstractNode
	{
		string name;
		StackFrame stackFrame;
		TargetFundamentalObject obj;
		
		string value;
		
		public override string Name {
			get { return name; }
		}
		
		public override string Value {
			get { return value; }
		}
		
		public FundamentalNode(string name, StackFrame stackFrame, TargetFundamentalObject obj)
		{
			this.name = name;
			this.stackFrame = stackFrame;
			this.obj = obj;
			
			this.value = obj.GetObject(stackFrame.Thread).ToString();
		}
	}
}
