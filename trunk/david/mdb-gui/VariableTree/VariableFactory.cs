using System;

using Mono.Debugger;
using Mono.Debugger.Languages;

namespace Mono.Debugger.Frontend.TreeModel
{
	public static class VariableFactory
	{
		public static AbstractVariable Create(TargetVariable variable, StackFrame stackFrame)
		{
			TargetObject obj;
			
			try {
				obj = variable.GetObject(stackFrame);
			} catch {
				return new ErrorVariable(variable.Name, "Can not get object");
			}
			
			return Create(variable.Name, obj, stackFrame);
		}
		
		public static AbstractVariable Create(string name, TargetObject obj, StackFrame stackFrame)
		{
			if (obj == null) {
				return new ErrorVariable(name, "Object is null");
			}
			
			if (obj.IsNull) {
				return new NullVariable(name);
			}
			
			try {
				switch (obj.Kind) {
					case TargetObjectKind.Array:
						return new ArrayVariable(name, stackFrame, (TargetArrayObject)obj);
					case TargetObjectKind.Pointer:
						TargetPointerObject pobj = (TargetPointerObject)obj;
						if (!pobj.Type.IsTypesafe) {
							return new ErrorVariable(name, "Pointer is not typesafe");
						}
						try {
							TargetObject deref = pobj.GetDereferencedObject(stackFrame.Thread);
							return VariableFactory.Create(name, deref, stackFrame);
						} catch {
							return new ErrorVariable(name, "Can not dereference object");
						}
					case TargetObjectKind.Object:
						try {
							TargetObject deref = ((TargetObjectObject)obj).GetDereferencedObject(stackFrame.Thread);
							return VariableFactory.Create(name, deref, stackFrame);
						} catch {
							return new ErrorVariable(name, "Can not dereference object");
						}
					case TargetObjectKind.Struct:
					case TargetObjectKind.Class:
						return new ClassVariable(name, stackFrame, (TargetClassObject)obj);
					case TargetObjectKind.Fundamental:
						return new FundamentalVariable(name, stackFrame, (TargetFundamentalObject)obj);
					case TargetObjectKind.Enum:
						return new EnumVariable(name, stackFrame, (TargetEnumObject)obj);
					default:
						return new ErrorVariable(name, "Unknown kind of object");
				}
			} catch (Exception e) {
				return new ErrorVariable(name, e.Message);
			}
		}
	}
}
