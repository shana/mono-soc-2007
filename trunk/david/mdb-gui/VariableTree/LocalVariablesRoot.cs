using System;
using System.Collections;

using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend.TreeModel
{
	public class LocalVariablesRoot: AbstractVariable
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
		
		public override AbstractVariable[] ChildNodes {
			get { return GetChilds(); }
		}
		
		public LocalVariablesRoot(Interpreter interpreter)
		{
			this.interpreter = interpreter;
		}
		
		AbstractVariable[] GetChilds()
		{
			ArrayList rootVariables = new ArrayList();
			
			StackFrame currentFrame;
			
			try {
				currentFrame = interpreter.CurrentThread.GetBacktrace().CurrentFrame;
			} catch {
				return new AbstractVariable[0];
			}
			
			foreach (TargetVariable variable in currentFrame.Method.Parameters) {
				rootVariables.Add(VariableFactory.Create(variable, currentFrame));
			}
				
			foreach (TargetVariable variable in currentFrame.Locals) {
				rootVariables.Add(VariableFactory.Create(variable, currentFrame));
			}
			
			if (currentFrame.Method.HasThis) {
				rootVariables.Add(VariableFactory.Create(currentFrame.Method.This, currentFrame));
			}
			
			return (AbstractVariable[])rootVariables.ToArray(typeof(AbstractVariable));
		}
	}
}
