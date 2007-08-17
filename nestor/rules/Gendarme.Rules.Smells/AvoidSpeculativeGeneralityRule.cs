//
// Gendarme.Rules.Smells.AvoidSpeculativeGeneralityRule class
//
// Authors:
//	Néstor Salceda <nestor.salceda@gmail.com>
//
// 	(C) 2007 Néstor Salceda
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

using System;
using System.Collections;

using Mono.Cecil;
using Gendarme.Framework;

namespace Gendarme.Rules.Smells {
	public class AvoidSpeculativeGeneralityRule : ITypeRule {

		private MessageCollection messageCollection;
		
		private ICollection GetInheritedClassesFrom (TypeDefinition baseType)
		{
			ArrayList inheritedClasses = new ArrayList ();
			foreach (TypeDefinition type in baseType.Module.Types) {
				if ((type.BaseType != null) && type.BaseType.Equals (baseType))
					inheritedClasses.Add (type);
			}
			return inheritedClasses;
		}

		private void CheckAbstractClassWithoutResponsability (TypeDefinition type) 
		{
			if (type.IsAbstract) {
				ICollection inheritedClasses = GetInheritedClassesFrom (type);
				if (inheritedClasses.Count == 1) {
					Location location = new Location (type.Name, String.Empty, 0);
					Message message = new Message ("This abstract class only has one class inheritting from.  This is a sign for the Speculative Generality smell.",location, MessageType.Error);
					messageCollection.Add (message);
				}
			}
		}

		public MessageCollection CheckType (TypeDefinition type, Runner runner) 
		{
			messageCollection = new MessageCollection ();
			
			CheckAbstractClassWithoutResponsability (type);

			if (messageCollection.Count != 0)
				return messageCollection;
			return null;
		}
	}
}
