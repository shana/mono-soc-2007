//
// Measurement.Launcher class
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

namespace Measurement {
	public class Launcher {
		private int limit;
		private string assembly;

		private string GetNext (string[] args, int index) 
		{
			if ((args == null) || (index < 0) || (index >= args.Length))
				return String.Empty;
			return args[index];
		}
		
		private bool ParseOptions (string[] args) 
		{
			for (int index = 0; index < args.Length; index++) {
				switch (args [index]) {
					case "--limit":
						limit = Int32.Parse (GetNext (args, ++index));
						break;
					default:
						assembly = args[index];
						break;
				}
			}
			return true;
		}
	
		private void PrintHelp () 
		{
			Console.WriteLine ("Usage: measure [--limit limit] [assembly]");
			Console.WriteLine ("Where:");
			Console.WriteLine ("  --limit limit\t\tSpecify an upper limit for the measure");
			Console.WriteLine ("  assembly\t\tSpecify the assembly to measure");
			Console.WriteLine ();
		}

		public string Assembly {
			get {
				return assembly;
			}
		}

		public int Limit {
			get {
				return limit;
			}
		}

		public static int Main (string[] args) 
		{
			Launcher launcher = new Launcher ();

			if (!launcher.ParseOptions (args)) {
				launcher.PrintHelp ();
				return 1;
			}
		
			new MeasureRunner (AssemblyFactory.GetAssembly (launcher.Assembly), launcher.Limit).ApplyMeasures ();

			return 0;
		}
	}
}
