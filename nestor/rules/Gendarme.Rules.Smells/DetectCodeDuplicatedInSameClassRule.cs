//
// Gendarme.Rules.Smells.DetectCodeDuplicatedInSameClassRule class
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
using System.Collections.Specialized;
using System.Text;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.Smells {
	//TODO: Ask if I can create new files that contains classes for this rule.

	class InstructionFillerVisitor : BaseCodeVisitor {
		private IList instructionPairContainer;
		private InstructionPair currentPair;
		
		public	InstructionFillerVisitor () : base () {
		}
		
		public override void VisitMethodBody (MethodBody methodBody) 
		{
			instructionPairContainer = new ArrayList ();
			currentPair = new InstructionPair ();
		}
		
		private bool IsAcceptable (Instruction instruction) 
		{
			return instruction.OpCode.FlowControl == FlowControl.Call 
				|| IsABranchInstruction (instruction);
		}
		
		private bool IsABranchInstruction (Instruction instruction) 
		{
			return instruction.OpCode.FlowControl == FlowControl.Branch ||
				instruction.OpCode.FlowControl == FlowControl.Cond_Branch;
		}
		
		private void FillInstructionPair (Instruction instruction) {
			if (currentPair.First != null) {
				currentPair.Second = instruction;
				instructionPairContainer.Add (currentPair);
				currentPair = new InstructionPair ();
				currentPair.First = instruction;
			}
			else {
				currentPair.First = instruction;
			}
		}
		
		public override void VisitInstructionCollection (InstructionCollection instructionCollection) 
		{
			foreach (Instruction instruction in instructionCollection) {
				if (IsAcceptable (instruction)) {
					if (IsABranchInstruction (instruction.Next)) 
						FillInstructionPair (instruction.Next);
					FillInstructionPair (instruction);
				}
			}
		}
		
		public IList InstructionPairContainer {
			get {
				return instructionPairContainer;
			}
		}
	}
	
	class InstructionPair {
		private Instruction firstInstruction;
		private Instruction secondInstruction;
		
		public InstructionPair () {}
		
		public InstructionPair (Instruction firstInstruction, Instruction secondInstruction) {
			this.firstInstruction = firstInstruction;
			this.secondInstruction = secondInstruction;
		}
		
		public Instruction First {
			get {
				return firstInstruction;
			}
			set {
				firstInstruction = value;
			}
		}
		
		public Instruction Second {
			get {
				return secondInstruction;
			}
			set {
				secondInstruction = value;
			}
		}
		
		private bool IsABranchInstruction (Instruction instruction) 
		{
			return instruction.OpCode.FlowControl == FlowControl.Branch ||
				instruction.OpCode.FlowControl == FlowControl.Cond_Branch;
		}
		
		public override bool Equals (object obj) {
			if (obj is InstructionPair) {
				InstructionPair targetInstructionPair = (InstructionPair) obj;
				
				if (IsABranchInstruction (firstInstruction) && 
					IsABranchInstruction (targetInstructionPair.First)) {
					return firstInstruction.OpCode == targetInstructionPair.First.OpCode &&
						secondInstruction.OpCode == targetInstructionPair.Second.OpCode &&
						secondInstruction.Operand == targetInstructionPair.Second.Operand;
				}
				
				if (IsABranchInstruction (secondInstruction) && 
					IsABranchInstruction (targetInstructionPair.Second)) {
					
					return firstInstruction.OpCode == targetInstructionPair.First.OpCode &&
						firstInstruction.Operand == targetInstructionPair.First.Operand &&
						secondInstruction.OpCode == targetInstructionPair.Second.OpCode;
				}
				
				return  firstInstruction.OpCode == targetInstructionPair.First.OpCode &&
						firstInstruction.Operand == targetInstructionPair.First.Operand &&
						secondInstruction.OpCode == targetInstructionPair.Second.OpCode &&
						secondInstruction.Operand == targetInstructionPair.Second.Operand
						;
			}
			return false;
		}
		
		public override int GetHashCode () {
			return base.GetHashCode ();
		}
		
		public override string ToString () {
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.Append ("Instruction Pair:" + Environment.NewLine);
			stringBuilder.Append (String.Format ("\tFirst: {0} {1}{2}", firstInstruction != null? firstInstruction.OpCode.Name : "null", firstInstruction != null? firstInstruction.Operand : "null", Environment.NewLine));
			stringBuilder.Append (String.Format ("\tSecond: {0} {1}{2}", secondInstruction != null? secondInstruction.OpCode.Name : "null", secondInstruction != null ? secondInstruction.Operand : "null", Environment.NewLine));
			return stringBuilder.ToString ();
		}
	}
	
	public class DetectCodeDuplicatedInSameClassRule : ITypeRule {
		private StringCollection checkedMethods;

		private IList FillInstructionPairContainerFrom (MethodBody methodBody) {
			InstructionFillerVisitor instructionFillerVisitor = new InstructionFillerVisitor ();
			methodBody.Accept (instructionFillerVisitor);
			return instructionFillerVisitor.InstructionPairContainer;
		}
		
		private bool ExistsRepliedInstructions (IList currentInstructionSet, IList targetInstructionSet) {
			bool existsRepliedInstructions = false;

			foreach (InstructionPair currentInstructionPair in currentInstructionSet) {
				foreach (InstructionPair targetInstructionPair in targetInstructionSet) {
					existsRepliedInstructions = existsRepliedInstructions || currentInstructionPair.Equals (targetInstructionPair);
					//if (currentInstructionPair.Equals (targetInstructionPair))
					//Console.WriteLine ("Checking: {0} aganist {1}",currentInstructionPair, targetInstructionPair);
				}
			}
			return existsRepliedInstructions;
		}
	
		private bool ContainsDuplicatedCode (MethodDefinition currentMethod, MethodDefinition targetMethod) {
			if (currentMethod.HasBody & targetMethod.HasBody & !checkedMethods.Contains (targetMethod.Name) & currentMethod != targetMethod) {
				//Console.WriteLine ("Checking: {0} against {1}", currentMethod.Name, targetMethod.Name);
				IList currentInstructionPairContainer = FillInstructionPairContainerFrom (currentMethod.Body);
				IList targetInstructionPairContainer = FillInstructionPairContainerFrom (targetMethod.Body);
				
				return ExistsRepliedInstructions (currentInstructionPairContainer, targetInstructionPairContainer);
			}
			return false;
		}
	
		public MessageCollection CheckType (TypeDefinition typeDefinition, Runner runner) 
		{
			checkedMethods = new StringCollection ();
			MessageCollection messageCollection = new MessageCollection ();
			foreach (MethodDefinition currentMethodDefinition in typeDefinition.Methods) {
				foreach (MethodDefinition targetMethodDefinition in typeDefinition.Methods) {
					
					if (ContainsDuplicatedCode (currentMethodDefinition, targetMethodDefinition)) {
						Location location = new Location (typeDefinition.Name, currentMethodDefinition.Name, 0);
						Message message = new Message (String.Format ("Exists code duplicated in: {0}.{1} and in {0}.{2}", typeDefinition.Name, currentMethodDefinition.Name, targetMethodDefinition.Name), location, MessageType.Error);
						messageCollection.Add (message);
					}
				}
				checkedMethods.Add (currentMethodDefinition.Name);
			}
			
			if (messageCollection.Count == 0)
				return null;
			return messageCollection;
		}
	}
}