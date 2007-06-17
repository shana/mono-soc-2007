//
// Unit Test for DetectCodeDuplicatedInSameClass Rule.
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
using System.Reflection;

using Gendarme.Framework;
using Gendarme.Rules.Smells;
using Mono.Cecil;
using NUnit.Framework;

namespace Test.Rules.Smells {

	public class ClassWithoutCodeDuplicated {
		private IList myList;

		public ClassWithoutCodeDuplicated () 
		{
			myList = new ArrayList ();
			myList.Add ("Foo");
			myList.Add ("Bar");
			myList.Add ("Baz");
		}

		private void PrintValuesInList () 
		{
			foreach (string value in myList) {
				Console.WriteLine (value);
			}      
		}

		public void MakeStuff () 
		{
			PrintValuesInList ();
			myList.Add ("FooReplied");
		}

		public void MakeMoreStuff () 
		{
			PrintValuesInList ();
			myList.Remove ("FooReplied");
		}
	}
	
	public class ClassWithCodeDuplicated {
		private IList myList;

		public ClassWithCodeDuplicated () 
		{
			myList = new ArrayList ();
			myList.Add ("Foo");
			myList.Add ("Bar");
			myList.Add ("Baz");
		}

		public void MakeStuff () 
		{
			foreach (string value in myList) {
				Console.WriteLine (value);
			}
			myList.Add ("FooReplied");
		}

		public void MakeMoreStuff () 
		{
			foreach (string value in myList) {
				Console.WriteLine (value);              
			}
			myList.Remove ("FooReplied");
		} 
	}

	[TestFixture]
	public class DetectCodeDuplicatedInSameClassTest {
		private ITypeRule rule;
		private AssemblyDefinition assembly;
		private TypeDefinition type;
		private MessageCollection messageCollection;

		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			rule = new DetectCodeDuplicatedInSameClassRule ();
			messageCollection = null;
		}

		private void CheckMessageType (MessageCollection messageCollection, MessageType messageType) 
		{
			IEnumerator enumerator = messageCollection.GetEnumerator ();
			if (enumerator.MoveNext ()) {
				Message message = (Message) enumerator.Current;
				Assert.AreEqual (message.Type, messageType);
			}
		}
		
		private void DumpMessageCollection (MessageCollection messageCollection)
		{
			foreach (Message message in messageCollection) {
				Console.WriteLine ("{0} - {1}", message.Type, message.ToString ());
			}
		}
		
		[Test]
		public void TestClassWithoutCodeDuplicated () 
		{
			type = assembly.MainModule.Types ["Test.Rules.Smells.ClassWithoutCodeDuplicated"];
			messageCollection = rule.CheckType (type, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void TestClassWithCodeDuplicated () 
		{
			type = assembly.MainModule.Types ["Test.Rules.Smells.ClassWithCodeDuplicated"];
			messageCollection = rule.CheckType (type, new MinimalRunner ());
			DumpMessageCollection (messageCollection);
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (messageCollection.Count, 1);
			CheckMessageType (messageCollection, MessageType.Error);
		}
	}
}
