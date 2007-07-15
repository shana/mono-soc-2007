//
// System.ComponentModel.Design.Serialization.CodeDomSerializer
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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;

namespace Mono.Design
{
	public class CodeDomSerializer : CodeDomSerializerBase
	{

		public CodeDomSerializer ()
		{
		}


		public object SerializeAbsolute (IDesignerSerializationManager manager, object value)
		{
			if (value == null)
				throw new ArgumentNullException ("value");
			if (manager == null)
				throw new ArgumentNullException ("manager");

			SerializeAbsoluteContext context = new SerializeAbsoluteContext ();
			manager.Context.Push (context);
			object result = this.Serialize (manager, value);
			manager.Context.Pop ();
			return result;
		}

		public virtual object Serialize (IDesignerSerializationManager manager, object value)
		{
			if (value == null)
				throw new ArgumentNullException ("value");
			if (manager == null)
				throw new ArgumentNullException ("manager");

			bool isComplete = true;
			CodeStatementCollection statements = new CodeStatementCollection ();
			ExpressionContext context = manager.Context[typeof (ExpressionContext)] as ExpressionContext;
			object serialized = null;

			if (context != null && context.PresetValue == value) {
				string varName = base.GetUniqueName (manager, value);
				CodeVariableDeclarationStatement statement = new CodeVariableDeclarationStatement (value.GetType (), varName); // declare
				statement.InitExpression = base.SerializeCreationExpression (manager, value, out isComplete); // initialize
				base.SetExpression (manager, value, statement.InitExpression);
				statements.Add (statement);
				serialized = statement;
			} else {
				string name = manager.GetName (value);
				if (name == null)
					name = base.GetUniqueName (manager, value);
				serialized = GetFieldReference (manager, name);
			}

			base.SerializeProperties (manager, statements, value, new Attribute[0]);
			base.SerializeEvents (manager, statements, value, new Attribute[0]);

			return serialized;
		}


		private CodeExpression GetFieldReference (IDesignerSerializationManager manager, string componentName)
		{
			RootContext rootContext = manager.Context[typeof (RootContext)] as RootContext;

			if (rootContext != null)
				return new CodeFieldReferenceExpression (rootContext.Expression , componentName);
			else
				return new CodeFieldReferenceExpression (new CodeThisReferenceExpression () , componentName);
		}

		// I am not sure what this does, but the only name I can think of this can get is a variable name from 
		// the expression
		public virtual string GetTargetComponentName (CodeStatement statement, CodeExpression expression, Type targetType)
		{
			if (expression is CodeFieldReferenceExpression)
				return ((CodeFieldReferenceExpression) expression).FieldName;
			else if (expression is CodeVariableReferenceExpression)
				return ((CodeVariableReferenceExpression) expression).VariableName;
			return null;
		}

		public virtual CodeStatementCollection SerializeMember (IDesignerSerializationManager manager, 
																object owningobject, MemberDescriptor member)
		{
			if (member == null)
				throw new ArgumentNullException ("member");
			if (owningobject == null)
				throw new ArgumentNullException ("owningobject");
			if (manager == null)
				throw new ArgumentNullException ("manager");

			CodeStatementCollection statements = new CodeStatementCollection ();
			if (base.GetExpression (manager, owningobject) == null)
				base.SetExpression (manager, owningobject, new CodeVariableReferenceExpression (base.GetUniqueName (manager, owningobject)));

			if (member is PropertyDescriptor)
				base.SerializeProperty (manager, statements, owningobject, (PropertyDescriptor) member);
			if (member is EventDescriptor)
				base.SerializeEvent (manager, statements, owningobject, (EventDescriptor) member);

			return statements;
		}

		public virtual CodeStatementCollection SerializeMemberAbsolute (IDesignerSerializationManager manager, 
																		object owningobject, MemberDescriptor member)
		{
			if (member == null)
				throw new ArgumentNullException ("member");
			if (owningobject == null)
				throw new ArgumentNullException ("owningobject");
			if (manager == null)
				throw new ArgumentNullException ("manager");

			SerializeAbsoluteContext context = new SerializeAbsoluteContext (member);
			manager.Context.Push (context);
			CodeStatementCollection result = this.SerializeMember (manager, owningobject, member);
			manager.Context.Pop ();
			return result;
		}


		// TODO
		public virtual object Deserialize (IDesignerSerializationManager manager, object codeobject)
		{
			throw new NotImplementedException ();
		}

		// TODO
		protected object DeserializeStatementToInstance (IDesignerSerializationManager manager, CodeStatement statement)
		{
			throw new NotImplementedException ();
		}
	}
}
#endif
