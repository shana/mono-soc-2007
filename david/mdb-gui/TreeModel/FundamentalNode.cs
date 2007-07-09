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
		
		public override string Name {
			get { return name; }
		}
		
		public override string Value {
			get {
				return obj.GetObject(stackFrame.Thread).ToString();
			}
		}
		
		public override string Type {
			get {
				return obj.TypeName;
			}
		}
		
		public FundamentalNode(string name, StackFrame stackFrame, TargetFundamentalObject obj)
		{
			this.name = name;
			this.stackFrame = stackFrame;
			this.obj = obj;
		}
	}
}
