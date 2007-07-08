using System;

using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend.TreeModel
{
	public class FundamentalNode: AbstractNode
	{
		TargetFundamentalObject obj;
		string name;
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
			this.obj = obj;
			this.value = obj.GetObject(stackFrame.Thread).ToString();
		}
	}
}
