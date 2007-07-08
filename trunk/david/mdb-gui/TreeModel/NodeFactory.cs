using System;

using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend.TreeModel
{
	public static class NodeFactory
	{
		public static AbstractNode Create(TargetVariable variable, StackFrame stackFrame)
		{
			TargetObject obj;
			
			try {
				obj = variable.GetObject(stackFrame);
			} catch {
				return new ErrorNode(variable.Name, "Can not get object");
			}
			
			if (obj == null) {
				return new ErrorNode(variable.Name, "Object is null");
			}
			
			if (obj.IsNull) {
				return new NullNode(variable.Name);
			}
			
			try {
				switch (obj.Kind) {
					case TargetObjectKind.Array:
						return new ErrorNode(variable.Name, "Unimplemented - Array");
					case TargetObjectKind.Pointer:
						return new ErrorNode(variable.Name, "Unimplemented - Pointer");
					case TargetObjectKind.Object:
						return new ErrorNode(variable.Name, "Unimplemented - Object");
					case TargetObjectKind.Class:
						return new ErrorNode(variable.Name, "Unimplemented - Class");
					case TargetObjectKind.Struct:
						return new ErrorNode(variable.Name, "Unimplemented - Struct");
					case TargetObjectKind.Fundamental:
						return new FundamentalNode(variable.Name, stackFrame, (TargetFundamentalObject)obj);
					case TargetObjectKind.Enum:
						return new ErrorNode(variable.Name, "Unimplemented - Enum");
					default:
						return new ErrorNode(variable.Name, "Unknown kind of object");
				}
			} catch (Exception e) {
				return new ErrorNode(variable.Name, e.Message);
			}
		}
	}
}
