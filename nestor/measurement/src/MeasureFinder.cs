//
// Measures.MeasureFinder class
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

namespace Measures {

	public class MeasureFinder {
		
		public IEnumerable FindByLinesPerMethod (IEnumerable measures, int minimumValue) 
		{
			ArrayList arrayList = new ArrayList ();
			foreach (TypeMeasure typeMeasure in measures) {
				if (minimumValue <= typeMeasure.LinesPerMethod) {
					arrayList.Add (typeMeasure);
				}
			}
			return arrayList;
		}

		public IEnumerable FindByParametersPerMethod (IEnumerable measures, int minimumValue) 
		{
			ArrayList arrayList = new ArrayList ();
			foreach (TypeMeasure typeMeasure in measures) {
				if (minimumValue <= typeMeasure.ParametersPerMethod)
					arrayList.Add (typeMeasure);
			}
			return arrayList;
		}

		public IEnumerable FindByNumberOfLines (IEnumerable measures, int minimumValue) 
		{
			ArrayList arrayList = new ArrayList ();
			foreach (TypeMeasure typeMeasure in measures) {
				ArrayList methodMeasures = new ArrayList ();
				foreach (MethodMeasure methodMeasure in typeMeasure.MethodMeasures) {
					if (minimumValue <= methodMeasure.TotalLines) {
						methodMeasures.Add (methodMeasure);
					}
				}
				if (methodMeasures.Count != 0) {
					typeMeasure.MethodMeasures = methodMeasures;
					arrayList.Add (typeMeasure);
				}
			}
			return arrayList;
		}

		public IEnumerable FindByNumberOfParameters (IEnumerable measures, int minimumValue) 
		{
			ArrayList arrayList = new ArrayList ();
			foreach (TypeMeasure typeMeasure in measures) {
				ArrayList methodMeasures = new ArrayList ();
				foreach (MethodMeasure methodMeasure in typeMeasure.MethodMeasures) {
					if (minimumValue <= methodMeasure.Parameters) {
						methodMeasures.Add (methodMeasure);
					}
				}
				if (methodMeasures.Count != 0) {
					typeMeasure.MethodMeasures = methodMeasures;
					arrayList.Add (typeMeasure);
				}
			}
			return arrayList;
		}
	}
}
