//
// Measures.Calculator class
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

namespace Measures {

	public class MeasureCalculator {
		private int CalculateTotalLines (MethodDefinition method) 
		{
			return method.HasBody ? method.Body.Instructions.Count : 0;
		}

		private int CalculateParameters (MethodDefinition method) 
		{
			return method.Parameters.Count;
		}

		public MethodMeasure MeasureMethod (MethodDefinition method) 
		{
			MethodMeasure methodMeasure = new MethodMeasure (method);
			methodMeasure.TotalLines = CalculateTotalLines (method);
			methodMeasure.Parameters = CalculateParameters (method);
			return methodMeasure;
		}

		private float CalculateLinesPerMethod (TypeDefinition type) 
		{
			if (type.Methods.Count != 0) {
				int totalLines = 0;
				foreach (MethodDefinition method in type.Methods) {
					totalLines += CalculateTotalLines (method);	
				}

				return (float) totalLines / type.Methods.Count;
			}
			return 0;
		}

		private float CalculateParametersPerMethod (TypeDefinition type) 
		{
			if (type.Methods.Count != 0) {
				int totalParameters = 0;
				foreach (MethodDefinition method in type.Methods) {
					totalParameters += CalculateParameters (method);
				}
				return (float) totalParameters / type.Methods.Count;
			}
			return 0;
		}

		public TypeMeasure MeasureType (TypeDefinition type) 
		{
			TypeMeasure typeMeasure = new TypeMeasure (type);
			typeMeasure.LinesPerMethod = CalculateLinesPerMethod (type);
			typeMeasure.ParametersPerMethod = CalculateParametersPerMethod (type);
			return typeMeasure;
		}

		public IEnumerable ProcessMeasures (AssemblyDefinition assembly) 
		{
			ArrayList arrayList = new ArrayList ();
			foreach (TypeDefinition type in assembly.MainModule.Types) {
				TypeMeasure typeMeasure = MeasureType (type);
				ArrayList methodMeasureList = new ArrayList ();
				foreach (MethodDefinition method in type.Methods) {
					methodMeasureList.Add (MeasureMethod (method));
				}
				typeMeasure.MethodMeasures = methodMeasureList;
				arrayList.Add (typeMeasure);
			}
			return arrayList;
		}
	}
}
