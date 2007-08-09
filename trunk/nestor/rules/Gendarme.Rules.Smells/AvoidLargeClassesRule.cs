//
// Gendarme.Rules.Smells.AvoidLargeClassesRule class
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

using Mono.Cecil;
using Gendarme.Framework;

namespace Gendarme.Rules.Smells {
	
	public class AvoidLargeClassesRule : ITypeRule {

		private MessageCollection messageCollection;
		private int maxFields = 25;

		public int MaxFields {
			get {
				return maxFields;
			}
			set {
				maxFields = value;
			}
		}

		private bool IsTooLarge (TypeDefinition type) 
		{
			return type.Fields.Count >= MaxFields;
		}
		
		private void CheckForClassFields (TypeDefinition type) 
		{
			if (IsTooLarge (type)) 
				AddMessage (type.Name, "This class contains a lot of fields.  This is a sign for the Large Class Smell", MessageType.Error);
		}

		private void AddMessage (string typeName, string summary, MessageType messageType) 
		{
			Location location = new Location (typeName, String.Empty, 0);
			Message message = new Message (summary, location, messageType);
			messageCollection.Add (message);
		}

		private bool ExitsCommonPrefixes (TypeDefinition type) 
		{
			return false;
		}

		private void CheckForCommonPrefixesInFields (TypeDefinition type) 
		{
			if (ExitsCommonPrefixes (type)) 
				AddMessage (type.Name, "This class contains some fields with the same prefix.  This is sign for the Large Class Smell", MessageType.Error);
		}


		public MessageCollection CheckType (TypeDefinition type, Runner runner) 
		{
			messageCollection = new MessageCollection ();
			
			CheckForClassFields (type);
			CheckForCommonPrefixesInFields (type);

			if (messageCollection.Count != 0)
				return messageCollection;
			return null;
		}
	}
}
