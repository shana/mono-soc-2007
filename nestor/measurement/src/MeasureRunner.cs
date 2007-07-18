//
// Measurement.MeasureRunner class
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

namespace Measurement {
	public class MeasureRunner {
		private AssemblyDefinition assembly;
		private int limit;

		public MeasureRunner (AssemblyDefinition assembly, int limit)
		{
			this.assembly = assembly;
			this.limit = limit;
		}

		public void ApplyMeasures () 
		{
			CalculateLinesPerMethodPerType ();
			CalculateParametersAveragePerType ();

			CalculateLinesPerMethodPerAssembly ();
			CalculateParametersAveragePerAssembly ();
		}

		private void CalculateLinesPerMethodPerAssembly () 
		{
			Console.WriteLine ("Applying Calculate Lines Per Method Per Assembly");
			int totalLines = 0;
			int totalMethods = 0;
			float linesPerMethod = 0;
			foreach (TypeDefinition type in assembly.MainModule.Types) {
				foreach (MethodDefinition method in type.Methods) {
					if (method.Body != null)
						totalLines += method.Body.Instructions.Count;
				}
				totalMethods += type.Methods.Count;
			}
			linesPerMethod = (float) totalLines / totalMethods;
			
			Console.WriteLine ("The assembly {0} total lines per method {1}", assembly, linesPerMethod);
		}

		private void CalculateParametersAveragePerAssembly () 
		{
			Console.WriteLine ("Applying Calculate Parameters Per Method Per Assembly");
			int totalParameters = 0;
			int totalMethods = 0;
			float parametersPerMethod = 0;
			foreach (TypeDefinition type in assembly.MainModule.Types) {
				foreach (MethodDefinition method in type.Methods) {
					totalParameters += method.Parameters.Count;
				}
				totalMethods += type.Methods.Count;
			}
			parametersPerMethod = (float) totalParameters / totalMethods;
			
			Console.WriteLine ("The assembly {0} total parameters per method {1}", assembly, parametersPerMethod);
		}

		private void CalculateLinesPerMethodPerType () 
		{
			Console.WriteLine ("Applying Calculate Lines Per Method Per Type");
			foreach (TypeDefinition type in assembly.MainModule.Types) {
				if (type.Methods.Count != 0) {
					int totalLines = 0;
					float linesPerMethod = 0;
					foreach (MethodDefinition method in type.Methods) {
						if (method.Body != null)
							totalLines+= method.Body.Instructions.Count;				
					}
					linesPerMethod = (float) totalLines / type.Methods.Count;
					if (linesPerMethod >= limit)
						Console.WriteLine ("The type {0} has an average of lines per method {1}",type.FullName, linesPerMethod);
				}
			}
			Console.WriteLine ();
		}

		private void CalculateParametersAveragePerType () 
		{
			Console.WriteLine ("Applying Calculate parameters average");
			foreach (TypeDefinition type in assembly.MainModule.Types) {
				if (type.Methods.Count != 0) {
					int totalParameters = 0;
					float parametersPerMethod = 0;
					foreach (MethodDefinition method in type.Methods) {
						totalParameters += method.Parameters.Count;
					}
					parametersPerMethod = (float) totalParameters / type.Methods.Count;
					if  (parametersPerMethod >= limit)
						Console.WriteLine ("The type {0} has an average of parameters per method {1}", type.FullName, parametersPerMethod);
				}
			}
			Console.WriteLine ();
		}
	}
}
