//
// System.ComponentModel.Design.Serialization.ComponentCodeDomSerializer
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
	// A serializer for the IComponent Type, supplied by the CodeDomSerializationProvider,
	// added as a provider by the RootComponentCodeDomSerializer
	//
	internal class ComponentCodeDomSerializer : CodeDomSerializer
	{

		public ComponentCodeDomSerializer ()
		{
		}

		public override object Serialize (IDesignerSerializationManager manager, object value)
		{
			if (value == null)
				throw new ArgumentNullException ("value");
			if (manager == null)
				throw new ArgumentNullException ("manager");

			object serialized = null;
			string name = manager.GetName (value);

			ExpressionContext exprContext = manager.Context[typeof (ExpressionContext)] as ExpressionContext;
			if (exprContext != null && exprContext.PresetValue == value) {
				CodeStatementCollection statements = new CodeStatementCollection ();
				bool isComplete = true;

				statements.Add (new CodeCommentStatement (String.Empty));
				statements.Add (new CodeCommentStatement (name));
				statements.Add (new CodeCommentStatement (String.Empty));
				statements.Add (new CodeAssignStatement (GetFieldReference (manager, name), 
														 base.SerializeCreationExpression (manager, value, out isComplete)));
				base.SerializeProperties (manager, statements, value, new Attribute[0]);
				base.SerializeEvents (manager, statements, value);
				serialized = statements;
			} else {
				serialized = base.Serialize (manager, value);
			}

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
	}
}
#endif
