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
	
	class ExpressionFiller : BaseCodeVisitor {
		private IList expressionContainer;
		private Expression currentExpression;

		public ExpressionFiller () : base () {}
	
		public override void VisitMethodBody (MethodBody methodBody) 
		{
			expressionContainer = new ArrayList ();
			currentExpression = null;
		}
		
		private bool IsAcceptable (Instruction instruction) 
		{
			return instruction.OpCode.FlowControl == FlowControl.Call || 
				instruction.OpCode.FlowControl == FlowControl.Branch || 
				instruction.OpCode.FlowControl == FlowControl.Cond_Branch;
		}

		private void CreateExpressionAndAddToExpressionContainer () 
		{
			currentExpression = new Expression ();
			expressionContainer.Add (currentExpression);
		}

		private bool IsDelimiter (Instruction instruction) 
		{
			return instruction.OpCode.Name == "ldarg.0" ||
				instruction.OpCode.FlowControl == FlowControl.Branch;
		}

		private void AddToExpression (Instruction instruction) 
		{
			if (currentExpression == null)
				CreateExpressionAndAddToExpressionContainer ();
			currentExpression.Add (instruction);
		}

		public override void VisitInstructionCollection (InstructionCollection instructionCollection) 
		{
			foreach (Instruction instruction in instructionCollection) {
				if (IsDelimiter (instruction)) 
					CreateExpressionAndAddToExpressionContainer ();
				if (IsAcceptable (instruction)) 
					AddToExpression (instruction);
			}
		}

		public ICollection Expressions {
			get {
				return expressionContainer;
			}
		}
	}

	class Expression : CollectionBase {

		public Expression () : base () {}

		public void Add (Instruction instruction) 
		{
			InnerList.Add (instruction);
		}

		public Instruction this[int index] {
			get {
				return (Instruction) InnerList[index];
			}
		}
				
		protected override void OnValidate (object value) 
		{
			if (!(value is Instruction))
				throw new ArgumentException ("You should use this class with Mono.Cecil.Cil.Instruction", "value");
		}

		public override bool Equals (object value) 
		{
			if (!(value is Expression))
				throw new ArgumentException ("The value argument should be an Expression", "value");
						
			Expression targetExpression = (Expression) value;
						
			if (HasSameSize (targetExpression)) 
				return CompareInstructionsInOrder (targetExpression);
						
			return false;
		}

		private bool HasSameSize (Expression expression) 
		{
			return Count == expression.Count;
		}

		private bool CompareInstructionsInOrder (Expression targetExpression) 
		{
			bool equality = true;
			for (int index = 0; index < Count; index++) {
				Instruction instruction = this[index];
				Instruction targetInstruction = targetExpression[index];
										
				if (CheckEqualityForOpCodes (instruction, targetInstruction)) {
					if (instruction.OpCode.FlowControl == FlowControl.Call) {
						equality = equality & (instruction.Operand == targetInstruction.Operand);
					}
				}
				else
					return false;;
			}
			return equality;
		}
				
		private bool CheckEqualityForOpCodes (Instruction currentInstruction, Instruction targetInstruction) 
		{
			if (currentInstruction.OpCode.Name == targetInstruction.OpCode.Name)
				return true;
			else {
				if (currentInstruction.OpCode.Name == "call" && targetInstruction.OpCode.Name == "callvirt")
					return true;
				else if (currentInstruction.OpCode.Name == "callvirt" && targetInstruction.OpCode.Name == "call")
					return true;
				else
					return false;
			}
		}

		public override int GetHashCode () 
		{
			return base.GetHashCode ();
		}

		public override string ToString () 
		{
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.Append ("\tFor the expression:");
			stringBuilder.Append (Environment.NewLine);
			foreach (Instruction instruction in InnerList) {
				stringBuilder.Append (String.Format ("\t\tInstruction: {0} {1}", instruction.OpCode.Name, instruction.Operand));
				stringBuilder.Append (Environment.NewLine);
			}
			return stringBuilder.ToString ();
			}

		}
	
	
	public class DetectCodeDuplicatedInSameClassRule : ITypeRule {
		private StringCollection checkedMethods;
		private MessageCollection messageCollection;

		private bool ExistsExpressionsReplied (ICollection currentExpressions, ICollection targetExpressions)
		{
			IEnumerator currentEnumerator = currentExpressions.GetEnumerator ();
			IEnumerator targetEnumerator = targetExpressions.GetEnumerator ();
			bool equality = false;

			while (currentEnumerator.MoveNext () & targetEnumerator.MoveNext ()) {
				Expression currentExpression = (Expression) currentEnumerator.Current;
				Expression targetExpression = (Expression) targetEnumerator.Current;

				if (equality && currentExpression.Equals (targetExpression))
					return true;
				else {
					equality = currentExpression.Equals (targetExpression);
				}
			}
			return false;
		}

		private ICollection GetExpressionsFrom (MethodBody methodBody) 
		{
			ExpressionFiller expressionFiller = new ExpressionFiller ();
			methodBody.Accept (expressionFiller);
			return expressionFiller.Expressions;
		}

		private bool CanCompareMethods (MethodDefinition currentMethod, MethodDefinition targetMethod) 
		{
			return currentMethod.HasBody && targetMethod.HasBody &&
				!checkedMethods.Contains (targetMethod.Name) && 
				currentMethod != targetMethod;
		}

		private bool ContainsDuplicatedCode (MethodDefinition currentMethod, MethodDefinition targetMethod) 
		{
			if (CanCompareMethods (currentMethod, targetMethod)) {
				ICollection currentExpressions = GetExpressionsFrom (currentMethod.Body);
				ICollection targetExpressions = GetExpressionsFrom (targetMethod.Body);
										
				return ExistsExpressionsReplied (currentExpressions, targetExpressions);
			}
			return false;
		}

		private void CompareMethodAgainstTypeMethods (MethodDefinition currentMethod, TypeDefinition targetTypeDefinition) 
		{
			foreach (MethodDefinition targetMethod in targetTypeDefinition.Methods) {
				if (ContainsDuplicatedCode (currentMethod, targetMethod)) {
					Location location = new Location (currentMethod.DeclaringType.Name, currentMethod.Name, 0);
					Message message = new Message (String.Format ("Exists code duplicated with {0}.{1}", targetTypeDefinition.Name, targetMethod.Name), location, MessageType.Error);
					messageCollection.Add (message);
				}
			}
		}

		public MessageCollection CheckType (TypeDefinition typeDefinition, Runner runner) 
		{
			checkedMethods = new StringCollection ();
			messageCollection = new MessageCollection ();
			foreach (MethodDefinition currentMethod in typeDefinition.Methods) {
				CompareMethodAgainstTypeMethods (currentMethod, typeDefinition);
				checkedMethods.Add (currentMethod.Name);
			}
			
			if (messageCollection.Count == 0)
				return null;
			return messageCollection;
		}
	}
}
