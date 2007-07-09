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
			
			return Create(variable.Name, obj, stackFrame);
		}
		
		public static AbstractNode Create(string name, TargetObject obj, StackFrame stackFrame)
		{
			if (obj == null) {
				return new ErrorNode(name, "Object is null");
			}
			
			if (obj.IsNull) {
				return new NullNode(name);
			}
			
			try {
				switch (obj.Kind) {
					case TargetObjectKind.Array:
						return new ArrayNode(name, stackFrame, (TargetArrayObject)obj);
					case TargetObjectKind.Pointer:
						return new ErrorNode(name, "Unimplemented - Pointer");
					case TargetObjectKind.Object:
						return new ErrorNode(name, "Unimplemented - Object");
					case TargetObjectKind.Struct:
					case TargetObjectKind.Class:
						return new ClassNode(name, stackFrame, (TargetClassObject)obj);
					case TargetObjectKind.Fundamental:
						return new FundamentalNode(name, stackFrame, (TargetFundamentalObject)obj);
					case TargetObjectKind.Enum:
						return new EnumNode(name, stackFrame, (TargetEnumObject)obj);
					default:
						return new ErrorNode(name, "Unknown kind of object");
				}
			} catch (Exception e) {
				return new ErrorNode(name, e.Message);
			}
		}
	}
}
