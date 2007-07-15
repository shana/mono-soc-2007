//
// System.ComponentModel.Design.Serialization.CollectionCodeDomSerializer
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
	public class CollectionCodeDomSerializer : CodeDomSerializer
	{

		public CollectionCodeDomSerializer ()
		{
		}

		// FIXME: What is this supposed to do?
		protected bool MethodSupportsSerialization (MethodInfo method)
		{
			return true;
		}

		public override object Serialize (IDesignerSerializationManager manager, object value)
		{
			if (value == null)
				throw new ArgumentNullException ("value");
			if (manager == null)
				throw new ArgumentNullException ("manager");

			ICollection originalCollection = value as ICollection;
			if (originalCollection == null)
				return null;

			CodeExpression targetExpression = null;
			Type targetType = value.GetType ();

			ExpressionContext exprContext = manager.Context[typeof (ExpressionContext)] as ExpressionContext;
			if (exprContext != null && exprContext.PresetValue == value) {
				targetExpression = exprContext.Expression;
				targetType = exprContext.ExpressionType;
			}

			ArrayList valuesToSerialize = new ArrayList ();
			foreach (object o in originalCollection)
				valuesToSerialize.Add (o);

			return this.SerializeCollection (manager, targetExpression, targetType, originalCollection, valuesToSerialize);
		}

		protected virtual object SerializeCollection (IDesignerSerializationManager manager, CodeExpression targetExpression, 
													  Type targetType, ICollection originalCollection, ICollection valuesToSerialize)
		{
			if (valuesToSerialize == null)
				throw new ArgumentNullException ("valuesToSerialize");
			if (originalCollection == null)
				throw new ArgumentNullException ("originalCollection");
			if (targetType == null)
				throw new ArgumentNullException ("targetType");
			if (manager == null)
				throw new ArgumentNullException ("manager");

			MethodInfo method = targetType.GetMethod ("Add", BindingFlags.Public | BindingFlags.Instance);
			if (method == null)
				return null;

			CodeStatementCollection statements = new CodeStatementCollection ();

			foreach (object value in valuesToSerialize) {
				statements.Add (new CodeMethodInvokeExpression (targetExpression, "Add", 
																new CodeExpression[] { base.SerializeToExpression (manager, value) }));
			}

			return statements;
		}

	}
}
#endif
