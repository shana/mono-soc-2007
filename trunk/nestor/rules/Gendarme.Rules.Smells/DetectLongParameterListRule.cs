//
// Gendarme.Rules.Smells.DetectLongParameterListRule class
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
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.Smells {
	
	//SUGGESTION: You *may* want offer a discount if the method is overloaded with a
	//subset of parameters.
	//SUGGESTION: Setting all required properties in a constructor isn't
	//uncommon.
	//SUGGESTION: Different value for public / private / protected methods *may*
	//be useful.
	public class DetectLongParameterListRule : IMethodRule {

		public const int MaxParameters = 10;

		private bool HasLongParameterList (MethodDefinition method) 
		{
			return method.Parameters.Count >= MaxParameters;
		}

		public MessageCollection CheckMethod (MethodDefinition method, Runner runner) 
		{
			MessageCollection messageCollection = new MessageCollection ();
			
			if (HasLongParameterList (method)) {
				Location location = new Location (method.DeclaringType.Name, method.Name, 0);		
				Message message = new Message ("The method contains a long parameter list.",location, MessageType.Error);
				messageCollection.Add (message);
			}

			if (messageCollection.Count == 0)
				return null;
			return messageCollection;
		}
	}
}
