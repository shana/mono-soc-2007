//
// Measures.TypeMeasure class
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
//
using System;
using System.Collections;
using Mono.Cecil;

namespace Measures {

	public class TypeMeasure {
		TypeDefinition type;
		float linesPerMethod;
		float parametersPerMethod;
		IEnumerable methodMeasures;
		int maxLinesInMethod;
		int maxParametersInMethod;

		public TypeMeasure (TypeDefinition type) 
		{
			this.type = type;
		}

		public string Name {
			get {
				return type.FullName;
			}
		}

		public float LinesPerMethod {
			get {
				return linesPerMethod;
			}
			internal set {
				linesPerMethod = value;
			}
		}

		public float ParametersPerMethod {
			get {
				return parametersPerMethod;
			}
			internal set {
				parametersPerMethod = value;
			}
		}

		public IEnumerable MethodMeasures {
			get {
				return methodMeasures;
			}
			internal set {
				methodMeasures = value;
			}
		}

		public int MaxLinesInMethod {
			get {
				return maxLinesInMethod;
			}
			internal set {
				maxLinesInMethod = value;
			}
		}

		public int MaxParametersInMethod {
			get {
				return maxParametersInMethod;
			}
			internal set {
				maxParametersInMethod = value;
			}
		}
	}
}
