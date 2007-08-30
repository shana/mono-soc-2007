//
// Gendarme.Rules.Security.AvoidUnsafeSQLQueries class
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

namespace Gendarme.Rules.Security {
	
	public class AvoidUnsafeSQLQueriesRule : IMethodRule {
		private MessageCollection messageCollection;
	
		private bool IsCallingInstruction (Instruction instruction)
		{
			return ((instruction.OpCode.Code == Code.Call) || (instruction.OpCode.Code == Code.Callvirt));
		}

		private bool SetSqlQueryInstruction (Instruction instruction) 
		{
			//This only works with calls to IDbCommand, I will add
			//the checking with different providers.
			return String.Compare (instruction.Operand.ToString () ,"System.Void System.Data.IDbCommand::set_CommandText(System.String)") == 0;
		}

		private Instruction GetSetCommandTextInstruction (MethodDefinition method) 
		{
			if (method.HasBody) {
				foreach (Instruction instruction in method.Body.Instructions) {
					if (IsCallingInstruction (instruction) && SetSqlQueryInstruction (instruction)) 
						return instruction;
				}
			}
			return null;
		}

		private bool QueryIsConcatenatedWithArguments (MethodDefinition method, Instruction instruction) 
		{
			Instruction iterationInstruction = instruction;
			while (iterationInstruction.Previous != null) {
				iterationInstruction = iterationInstruction.Previous;
				
				if (IsCallingInstruction (iterationInstruction)) {
					if (iterationInstruction.Operand.ToString ().StartsWith ("System.String System.String::Concat"))
						return true;
				}
				
				if (iterationInstruction.OpCode.Code == Code.Ldstr)
					return false;

			}
			return false;
		}

		private bool ContainsPotentialInjectionCode (MethodDefinition method) 
		{
			Instruction setDbQuery = GetSetCommandTextInstruction (method);
			if (setDbQuery != null) 
				return QueryIsConcatenatedWithArguments (method, setDbQuery);
			return false;
		}
		
		private void CheckForUnsafeQueries (MethodDefinition method) 
		{
			if (ContainsPotentialInjectionCode (method)) {
				Location location = new Location (method.DeclaringType.Name, method.Name, 0);
				Message message = new Message ("The method contains potential SQL injection code.",location, MessageType.Error);
				messageCollection.Add (message);
			}
		}

		public MessageCollection CheckMethod (MethodDefinition method, Runner runner) 
		{
			messageCollection = new MessageCollection ();
			
			CheckForUnsafeQueries (method);

			if (messageCollection.Count == 0)
				return null;
			return messageCollection;
		}
	}
}
