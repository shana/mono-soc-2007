using System;
using System.Collections;

using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend.TreeModel
{
	public class LocalsRootNode: AbstractNode
	{
		Interpreter interpreter;
		
		public override string Name {
			get { return "Local variables"; }
		}
		
		public override string Value {
			get { return String.Empty; }
		}
		
		public override string Type {
			get { return String.Empty; }
		}
		
		public override bool HasChildNodes {
			get { return true; }
		}
		
		public override AbstractNode[] ChildNodes {
			get { return GetChilds(); }
		}
		
		public LocalsRootNode(Interpreter interpreter)
		{
			this.interpreter = interpreter;
		}
		
		AbstractNode[] GetChilds()
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
	}
}
