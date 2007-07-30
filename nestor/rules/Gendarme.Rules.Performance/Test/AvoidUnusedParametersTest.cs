//
// Unit Test for AvoidUnusedParameters Rule.
//
// Authors:
//      Néstor Salceda <nestor.salceda@gmail.com>
//
//      (C) 2007 Néstor Salceda
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
using System.Reflection;

using Mono.Cecil;
using NUnit.Framework;
using Gendarme.Framework;
using Gendarme.Rules.Performance;

namespace Test.Rules.Performance {

	[TestFixture]
	public class AvoidUnusedParametersTest {
		
		private IMethodRule rule;
		private AssemblyDefinition assembly;
		private MethodDefinition method;
		private MessageCollection messageCollection;

		public void PrintBannerUsingParameter (Version version) 
		{
			Console.WriteLine ("Welcome to the foo program {0}", version);
		}

		public void PrintBannerUsingAssembly (Version version)
		{
			Console.WriteLine ("Welcome to the foo program {0}", Assembly.GetExecutingAssembly ().GetName ().Version);
		}

		public void PrintBannerWithoutParameters () 
		{
			Console.WriteLine ("Welcome to the foo program {0}", Assembly.GetExecutingAssembly ().GetName ().Version);
		}


		[TestFixtureSetUp]
		public void FixtureSetUp () 
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			rule = new AvoidUnusedParametersRule ();
			messageCollection = null;
		}

		[Test]
		public void PrintBannerUsingParameterTest () 
		{
			method = GetMethodForTest ("PrintBannerUsingParameter");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}

		[Test]
		public void PrintBannerUsingAssemblyTest ()
		{
			method = GetMethodForTest ("PrintBannerUsingAssembly");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}

		[Test]
		public void PrintBannerWithoutParametersTest () 
		{
			method = GetMethodForTest ("PrintBannerWithoutParameter");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}

		private MethodDefinition GetMethodForTest (string methodName) 
		{
			return GetMethodForTestFrom ("Test.Rules.Performance.AvoidUnusedParametersTest", methodName);
		}

		private MethodDefinition GetMethodForTestFrom (string fullTypeName, string methodName) 
		{
			TypeDefinition type =  assembly.MainModule.Types[fullTypeName];
			if (type != null) {
				foreach (MethodDefinition method in type.Methods) {
				if (method.Name == methodName)
					return method;
				}
			}
			return null;
		}


	}
}
