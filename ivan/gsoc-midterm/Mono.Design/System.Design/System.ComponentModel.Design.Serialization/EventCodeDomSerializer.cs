//
// System.ComponentModel.Design.Serialization.EventCodeDomSerializer
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
	internal class EventCodeDomSerializer : MemberCodeDomSerializer
	{

		private CodeThisReferenceExpression _thisReference;

		public EventCodeDomSerializer ()
		{
			// don't waste memory on something that is constant when generating the
			// event codedom code - keep it as a field.
			_thisReference = new CodeThisReferenceExpression ();
		}


		public override void Serialize (IDesignerSerializationManager manager, object value, MemberDescriptor descriptor, CodeStatementCollection statements)
		{
			if (statements == null)
				throw new ArgumentNullException ("statements");
			if (manager == null)
				throw new ArgumentNullException ("manager");
			if (value == null)
				throw new ArgumentNullException ("value");
			if (descriptor == null)
				throw new ArgumentNullException ("descriptor");

			IEventBindingService service = manager.GetService (typeof (IEventBindingService)) as IEventBindingService;
			if (service != null) {
				// In the propertygrid the events are represented by PropertyDescriptors and the value is a string
				// which contains the method name to bind to. The propertydescriptors are managed and created by the 
				// IEventBindingService
				// 
				EventDescriptor ev = (EventDescriptor) descriptor;
				string methodName = (string) service.GetEventProperty (ev).GetValue (value);
				CodeDelegateCreateExpression listener = new CodeDelegateCreateExpression (new CodeTypeReference (ev.EventType), _thisReference, methodName);
				CodeExpression targetObject = base.SerializeToExpression (manager, value);
				CodeEventReferenceExpression eventRef = new CodeEventReferenceExpression (targetObject, ev.Name);
				statements.Add (new CodeAttachEventStatement (eventRef, listener));
			}
		}

		public override bool ShouldSerialize (IDesignerSerializationManager manager, object value, MemberDescriptor descriptor)
		{
			return true;
		}
	}
}
#endif
