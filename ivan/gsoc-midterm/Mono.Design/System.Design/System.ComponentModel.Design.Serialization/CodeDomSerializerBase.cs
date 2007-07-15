//
// System.ComponentModel.Design.Serialization.CodeDomSerializerBase
//
// Authors:	 
//	  Ivan N. Zlatev (contact i-nZ.net)
//
// (C) 2007 Ivan N. Zlatev

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if NET_2_0

using System;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;

namespace Mono.Design
{
	public abstract class CodeDomSerializerBase
	{

		private class ExpressionTable : Hashtable // just so that we have a specific type to append to the context stack
		{
		}

		protected CodeExpression SerializeToExpression (IDesignerSerializationManager manager, object instance)
		{
			if (manager == null)
				throw new ArgumentNullException ("manager");
			if (instance == null)
				throw new ArgumentNullException ("instance");

			CodeExpression expression = this.GetExpression (manager, instance); // 1 - IDesignerSerializationManager.GetExpression
			if (expression == null) {
				CodeDomSerializer serializer = this.GetSerializer (manager, instance); // 2 - manager.GetSerializer().Serialize()
				if (serializer != null) {
					object serialized = serializer.Serialize (manager, instance);
					expression = serialized as CodeExpression; // 3 - CodeStatement or CodeStatementCollection
					if (expression == null) {
						CodeStatement statement = serialized as CodeStatement;
						CodeStatementCollection statements = serialized as CodeStatementCollection;

						if (statement != null || statements != null) {
							CodeStatementCollection contextStatements = null;

							StatementContext context = manager.Context[typeof (StatementContext)] as StatementContext;
							if (context != null)
								contextStatements = context.StatementCollection[instance];

							if (contextStatements == null)
								contextStatements = manager.Context[typeof (CodeStatementCollection)] as CodeStatementCollection;

							if (contextStatements != null) {
								if (statements != null)
									contextStatements.AddRange (statements);
								else
									contextStatements.Add (statement);
							}
						}
					}
					if (expression == null)
						expression = this.GetExpression (manager, instance); // 4

					if (expression == null)
						manager.ReportError ("SerializeToExpression: " + instance.GetType ().AssemblyQualifiedName + " failed.");
				}
			}
			return expression;
		}

		protected CodeDomSerializer GetSerializer (IDesignerSerializationManager manager, object instance)
		{
			DesignerSerializerAttribute attrInstance, attrType;
			attrType = attrInstance = null;

			AttributeCollection attributes = TypeDescriptor.GetAttributes (instance);
			foreach (Attribute a in attributes) {
				DesignerSerializerAttribute designerAttr = a as DesignerSerializerAttribute;
				if (designerAttr != null && manager.GetType (designerAttr.SerializerBaseTypeName) == typeof (CodeDomSerializer)) {
					attrInstance = designerAttr;
					break;
				}
			}

			attributes = TypeDescriptor.GetAttributes (instance.GetType ());
			foreach (Attribute a in attributes) {
				DesignerSerializerAttribute designerAttr = a as DesignerSerializerAttribute;
				if (designerAttr != null && manager.GetType (designerAttr.SerializerBaseTypeName) == typeof (CodeDomSerializer)) {
					attrType = designerAttr;
					break;
				}
			}

			// if there is metadata modification in the instance then create the specified serializer instead of the one
			// in the Type.
			if (attrType != null && attrInstance != null && attrType.SerializerTypeName != attrInstance.SerializerTypeName)
				return Activator.CreateInstance (manager.GetType (attrInstance.SerializerTypeName)) as CodeDomSerializer;
			else
				return this.GetSerializer (manager, instance.GetType ());
		}

		protected CodeDomSerializer GetSerializer (IDesignerSerializationManager manager, Type instanceType)
		{
			return manager.GetSerializer (instanceType, typeof (CodeDomSerializer)) as CodeDomSerializer;
		}

		protected CodeExpression GetExpression (IDesignerSerializationManager manager, object instance)
		{
			if (manager == null)
				throw new ArgumentNullException ("manager");
			if (instance == null)
				throw new ArgumentNullException ("instance");

			CodeExpression expression = null;

			ExpressionTable expressions = manager.Context[typeof (ExpressionTable)] as ExpressionTable;
			if (expressions != null) // 1st try: ExpressionTable
				expression = expressions [instance] as CodeExpression;

			if (expression == null) { // 2nd try: RootContext
				RootContext context = manager.Context[typeof (RootContext)] as RootContext;
				if (context.Value == instance)
					expression = context.Expression;
			}

			if (expression == null) { // 3rd try: IReferenceService (instnace.property.property.property
				string name = manager.GetName (instance);
				if (name == null || name.IndexOf (".") == -1) {
					IReferenceService service = manager.GetService (typeof (IReferenceService)) as IReferenceService;
					if (service != null) {
						name = service.GetName (instance);
						if (name != null && name.IndexOf (".") != -1) {
							string[] parts = name.Split (new char[] { ',' });
							instance = manager.GetInstance (parts[0]);
							if (instance != null) {
								expression = SerializeToExpression (manager, instance);
								if (expression != null) {
									for (int i=1; i < parts.Length; i++)
										expression = new CodePropertyReferenceExpression (expression, parts[i]);
								}
							}
						}
					}
				}
			}
			return expression;
		}

		protected void SetExpression (IDesignerSerializationManager manager, object instance, CodeExpression expression)
		{
			SetExpression (manager, instance, expression, false);
		}

		// XXX: isPreset - what does this do when set?
		//
		protected void SetExpression (IDesignerSerializationManager manager, object instance, CodeExpression expression, bool isPreset)
		{
			if (manager == null)
				throw new ArgumentNullException ("manager");
			if (instance == null)
				throw new ArgumentNullException ("instance");
			if (expression == null)
				throw new ArgumentNullException ("expression");

			ExpressionTable expressions = manager.Context[typeof (ExpressionTable)] as ExpressionTable;
			if (expressions == null) {
				expressions = new ExpressionTable ();
				manager.Context.Append (expressions);
			}

			expressions[instance] = expression;
		}

		protected bool IsSerialized (IDesignerSerializationManager manager, object value) 
		{
			return this.IsSerialized (manager, value, false);
		}

		// XXX: What should honorPreset do?
		protected bool IsSerialized (IDesignerSerializationManager manager, object instance, bool honorPreset) 
		{
			if (instance == null)
				throw new ArgumentNullException ("instance");
			if (manager == null)
				throw new ArgumentNullException ("manager");

			if (this.GetExpression (manager, instance) != null)
				return true;
			else
				return false;
		}

		protected CodeExpression SerializeCreationExpression (IDesignerSerializationManager manager, object value, out bool isComplete) 
		{
			if (value == null)
				throw new ArgumentNullException ("value");
			if (manager == null)
				throw new ArgumentNullException ("manager");

			CodeExpression expression = null;

			TypeConverter converter = TypeDescriptor.GetConverter (value);
			if (converter != null && converter.CanConvertTo (typeof (InstanceDescriptor))) {
				InstanceDescriptor descriptor = converter.ConvertTo (value, typeof (InstanceDescriptor)) as InstanceDescriptor;
				isComplete = descriptor.IsComplete;
				expression = this.SerializeInstanceDescriptor (manager, descriptor);
			} else {
				expression = new CodeObjectCreateExpression (value.GetType ().FullName, new CodeExpression[0]);
				isComplete = false;
			}

			return expression;
		}

		private CodeExpression SerializeInstanceDescriptor (IDesignerSerializationManager manager, InstanceDescriptor descriptor)
		{
			CodeExpression expression = null;
			MemberInfo member = descriptor.MemberInfo;
			CodeExpression target = new CodeTypeReferenceExpression (member.DeclaringType);

			if (member is PropertyInfo) {
				expression = new CodePropertyReferenceExpression (target, member.Name);
			} else if (member is FieldInfo) {
				expression = new CodeFieldReferenceExpression (target, member.Name);
			} else if (member is MethodInfo) {
				expression = new CodeMethodReferenceExpression (target, member.Name);
			} else if (member is ConstructorInfo) {
				CodeExpression[] paramExpressions = null;

				// process ctor params' expressions
				if (descriptor.Arguments != null && descriptor.Arguments.Count > 0) {
					paramExpressions = new CodeExpression[descriptor.Arguments.Count];
					object[] arguments = new object [descriptor.Arguments.Count];
					descriptor.Arguments.CopyTo (arguments, 0);

					for (int i=0; i < paramExpressions.Length; i++) {
						ExpressionContext parentContext = manager.Context[typeof (ExpressionContext)] as ExpressionContext;
						if (parentContext != null) { // check if there is an expression context to add to
							ExpressionContext currentContext = new ExpressionContext (parentContext.Expression, arguments[i].GetType (), parentContext.Owner);
							manager.Context.Push (currentContext);
						}

						paramExpressions[i] = this.SerializeToExpression (manager, arguments[i]);

						manager.Context.Pop ();
					}
				}

				expression = new CodeObjectCreateExpression (member.DeclaringType, paramExpressions);
			}

			return expression;
		}

		protected void SerializeEvent (IDesignerSerializationManager manager, CodeStatementCollection statements, 
									   object value, EventDescriptor descriptor) 
		{
			if (descriptor == null)
				throw new ArgumentNullException ("descriptor");
			if (value == null)
				throw new ArgumentNullException ("value");
			if (statements == null)
				throw new ArgumentNullException ("statements");
			if (manager == null)
				throw new ArgumentNullException ("manager");

			MemberCodeDomSerializer serializer = manager.GetSerializer (descriptor.GetType (), typeof (MemberCodeDomSerializer)) as MemberCodeDomSerializer;
			if (serializer != null && serializer.ShouldSerialize (manager, value, descriptor))
				serializer.Serialize (manager, value, descriptor, statements);
		}

		protected void SerializeEvents (IDesignerSerializationManager manager, CodeStatementCollection statements, 
										object value, params Attribute[] filter)
		{
			if (filter == null)
				throw new ArgumentNullException ("filter");
			if (value == null)
				throw new ArgumentNullException ("value");
			if (statements == null)
				throw new ArgumentNullException ("statements");
			if (manager == null)
				throw new ArgumentNullException ("manager");

			EventDescriptorCollection events = TypeDescriptor.GetEvents (value, filter);
			foreach (EventDescriptor e in events)
				this.SerializeEvent (manager, statements, value, e);
		}

		protected void SerializeProperty (IDesignerSerializationManager manager, CodeStatementCollection statements, object value, PropertyDescriptor propertyToSerialize)
		{
			if (propertyToSerialize == null)
				throw new ArgumentNullException ("propertyToSerialize");
			if (value == null)
				throw new ArgumentNullException ("value");
			if (statements == null)
				throw new ArgumentNullException ("statements");
			if (manager == null)
				throw new ArgumentNullException ("manager");

			MemberCodeDomSerializer serializer = manager.GetSerializer (propertyToSerialize.GetType (), typeof (MemberCodeDomSerializer)) as MemberCodeDomSerializer;
			if (serializer != null && serializer.ShouldSerialize (manager, value, propertyToSerialize))
				serializer.Serialize (manager, value, propertyToSerialize, statements);
		}
		
		protected void SerializeProperties (IDesignerSerializationManager manager, CodeStatementCollection statements, 
											object value, Attribute[] filter) 
		{
			if (filter == null)
				throw new ArgumentNullException ("filter");
			if (value == null)
				throw new ArgumentNullException ("value");
			if (statements == null)
				throw new ArgumentNullException ("statements");
			if (manager == null)
				throw new ArgumentNullException ("manager");

			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties (value, filter);
			foreach (PropertyDescriptor property in properties)
				this.SerializeProperty (manager, statements, value, property);
		}

		protected virtual object DeserializeInstance (IDesignerSerializationManager manager, Type type, 
													  object[] parameters, string name, bool addToContainer)
		{
			if (type == null)
				throw new ArgumentNullException ("type");
			if (manager == null)
				throw new ArgumentNullException ("manager");

			return manager.CreateInstance (type, parameters, name, addToContainer);
		}

		protected string GetUniqueName (IDesignerSerializationManager manager, object instance)
		{
			if (instance == null)
				throw new ArgumentNullException ("instance");
			if (manager == null)
				throw new ArgumentNullException ("manager");

			string name = manager.GetName (instance);
			if (name == null) {
				INameCreationService service = manager.GetService (typeof (INameCreationService)) as INameCreationService;
				name = service.CreateName (null, instance.GetType ());
			}
			if (name == null) {
				name = instance.GetType ().Name.ToLower ();
			}
			manager.SetName (instance, name);
			return name;
		}

		// TODO
		protected void DeserializeExpression (IDesignerSerializationManager manager, string name, CodeExpression expression) 
		{
			throw new NotImplementedException ();
		}

		// TODO
		protected void DeserializeStatement (IDesignerSerializationManager manager, CodeStatement statement) 
		{
			throw new NotImplementedException ();
		}
		
#region Resource Serialization - TODO
		protected CodeExpression SerializeToResourceExpression (IDesignerSerializationManager manager, object value) 
		{
			throw new NotImplementedException ();
		}
		
		protected CodeExpression SerializeToResourceExpression (IDesignerSerializationManager manager, object value, bool ensureInvariant) 
		{
			throw new NotImplementedException ();
		}
		
		protected void SerializePropertiesToResources (IDesignerSerializationManager manager, CodeStatementCollection statements, object value, Attribute[] filter) 
		{
			throw new NotImplementedException ();
		}

		protected void SerializeResource (IDesignerSerializationManager manager, string resourceName, object value)
		{
			throw new NotImplementedException ();
		}

		protected void SerializeResourceInvariant (IDesignerSerializationManager manager, string resourceName, object value) 
		{
			throw new NotImplementedException ();
		}

		protected void DeserializePropertiesFromResources (IDesignerSerializationManager manager, object value, Attribute[] filter)
		{
			throw new NotImplementedException ();
		}
#endregion
	}
}
#endif
