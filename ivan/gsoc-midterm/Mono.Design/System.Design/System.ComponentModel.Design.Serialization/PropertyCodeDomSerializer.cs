//
// System.ComponentModel.Design.Serialization.PropertyCodeDomSerializer
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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;

namespace Mono.Design
{
	internal class PropertyCodeDomSerializer : MemberCodeDomSerializer
	{

		public PropertyCodeDomSerializer ()
		{
		}
	
		public override void Serialize (IDesignerSerializationManager manager, object value, MemberDescriptor descriptor, CodeStatementCollection statements)
		{
			if (manager == null)
				throw new ArgumentNullException ("manager");
			if (value == null)
				throw new ArgumentNullException ("value");
			if (descriptor == null)
				throw new ArgumentNullException ("descriptor");
			if (statements == null)
				throw new ArgumentNullException ("statements");

			PropertyDescriptor property = (PropertyDescriptor) descriptor;
			if (property.Attributes.Contains (DesignerSerializationVisibilityAttribute.Visible))
				SerializeNormalProperty (manager, value, property, statements);
			else if (property.Attributes.Contains (DesignerSerializationVisibilityAttribute.Content))
				SerializeContentProperty (manager, value, property, statements);
		}


		private void SerializeNormalProperty (IDesignerSerializationManager manager, 
											  object value, PropertyDescriptor descriptor, CodeStatementCollection statements)
		{
			CodeExpression leftSide = base.SerializeToExpression (manager, value);
			CodeExpression rightSide = null;

			MemberRelationship relationship = GetRelationship (manager, value, descriptor);
			if (!relationship.IsEmpty) 
				rightSide = new CodePropertyReferenceExpression (base.SerializeToExpression (manager, relationship.Owner), 
																 relationship.Member.Name);
			else
				rightSide = base.SerializeToExpression (manager, descriptor.GetValue (value));

			statements.Add (new CodeAssignStatement (leftSide, rightSide));
		}

		// TODO
		private void SerializeContentProperty (IDesignerSerializationManager manager, object value, 
											   PropertyDescriptor descriptor, CodeStatementCollection statements)
		{
			throw new NotImplementedException ();
		}

		public override bool ShouldSerialize (IDesignerSerializationManager manager, object value, MemberDescriptor descriptor)
		{
			if (manager == null)
				throw new ArgumentNullException ("manager");
			if (value == null)
				throw new ArgumentNullException ("value");
			if (descriptor == null)
				throw new ArgumentNullException ("descriptor");

			bool result = ((PropertyDescriptor) descriptor).ShouldSerializeValue (value);

			if (!GetRelationship (manager, value, descriptor).IsEmpty)
				result = true;

			if (!result) {
				SerializeAbsoluteContext absolute = manager.Context[typeof (SerializeAbsoluteContext)] as SerializeAbsoluteContext;
				if (absolute.Member == null || absolute.Member == descriptor)
					result = true;
			}

			return result;
		}

		private MemberRelationship GetRelationship (IDesignerSerializationManager manager, object value, MemberDescriptor descriptor)
		{
			MemberRelationshipService service = manager.GetService (typeof (MemberRelationshipService)) as MemberRelationshipService;
			if (service != null)
				return service[value, descriptor];
			else
				return MemberRelationship.Empty;
		}
	}
}
#endif
