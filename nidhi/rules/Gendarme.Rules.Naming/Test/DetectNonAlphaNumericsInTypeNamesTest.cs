//Copyright (c) <2007> <Nidhi Rawal>
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

using System;
using System.Reflection;

using Gendarme.Framework;
using Gendarme.Rules.Naming;
using Mono.Cecil;
using NUnit.Framework;

namespace Test.Rules.Naming {

	[TestFixture]
	public class DetectNonAlphaNumericsInTypeNamesTest {
			
		public class ClassContainingProperty {
			int i=0;
			public int Property {
				get {
					return i;
				}
				set {
					i = value;
				}
			}
		}
		
		public class ClassContainingEvent
		{
			public event EventHandler MyEvent
			{
				add { } 
				remove { }
			}
		}
	
		class ClassContainingConversionOperator 
		{ 
			private string FullName = "";
			public ClassContainingConversionOperator (string str) 
			{ 
				FullName = str; 
			} 
			public static implicit operator ClassContainingConversionOperator (string str) 
			{ 
				return new ClassContainingConversionOperator (str); 
			}
			public static explicit operator ClassContainingConversionOperator (string str) 
			{ 
				return new ClassContainingConversionOperator (str); 
			}
		}
			
		class ClassContainingPrivateMethodWithUnderscore 
		{
			private void my_method ()
			{
			}
		}
		
		class ClassContainingPublicMethodWithUnderscore
		{
			public void methot_test ()
			{
			}
		}
			
		private class PrivateClassName_WithUnderscore
		{
		}
		
		public class PublicClassName_WithUnderscore
		{
		}
					
		private interface PrivateInterface_WithUnderscore
		{
		}
			
		public interface PublicInterface_WithUnderscore
		{
		}
			
		class DefaultPrivate_Class
		{
			void DefaultPrivate_Method ()
			{
			}
		}
		
		public class ClassWithoutUnderscore 
		{
			public void methodWithoutUnderscore ()
			{
			}
		}
		
		private IMethodRule methodRule;
		private ITypeRule typeRule;
		private AssemblyDefinition assembly;
		private ModuleDefinition module;
		private TypeDefinition type;

		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			module = assembly.MainModule;
			type = assembly.MainModule.Types["Test.Rules.Naming.DetectNonAlphaNumericsInTypeNamesTest/TestCase"];
			methodRule = new DetectNonAlphaNumericsInTypeNamesRule();
			typeRule = new DetectNonAlphaNumericsInTypeNamesRule();
		}
		
		private TypeDefinition GetTest (string name)
		{
			string fullname = "Test.Rules.Naming.DetectNonAlphaNumericsInTypeNamesTest/" + name;
			return assembly.MainModule.Types[fullname];
		}
		
		[Test]
		public void propertyTest ()
		{
			TypeDefinition type = GetTest ("ClassContainingProperty");
			foreach (MethodDefinition method in type.Methods) {
				Assert.IsNull (methodRule.CheckMethod (method, new MinimalRunner ()));
			}
		}
			
		[Test]
		public void eventTest ()
		{
			TypeDefinition type = GetTest ("ClassContainingEvent");
			foreach (MethodDefinition method in type.Methods) {
				Assert.IsNull (methodRule.CheckMethod (method, new MinimalRunner ()));
			}
		}
			
		[Test]
		public void conversionOperatorTest ()
		{
			TypeDefinition type = GetTest ("ClassContainingConversionOperator");
			foreach (MethodDefinition method in type.Methods) {
				Assert.IsNull (methodRule.CheckMethod (method, new MinimalRunner ()));
			}
		}
		
		[Test]
		public void privateMethodWithUnderscoreTest ()
		{
			TypeDefinition type = GetTest ("ClassContainingPrivateMethodWithUnderscore");
			foreach (MethodDefinition method in type.Methods) {
				Assert.IsNull (methodRule.CheckMethod (method, new MinimalRunner ()));
			}
		}
		
		[Test]
		public void publicMethodWithUnderscoreTest ()
		{
			TypeDefinition type = GetTest ("ClassContainingPublicMethodWithUnderscore");
			foreach (MethodDefinition method in type.Methods) {
				Assert.IsNull (methodRule.CheckMethod (method, new MinimalRunner ()));
			}
		}
		
		[Test]
		public void privateClassWithUnderscoreTest ()
		{
			TypeDefinition type = GetTest ("PrivateClassName_WithUnderscore");
			Assert.IsNull (typeRule.CheckType (type, new MinimalRunner ()));
		}
		
		[Test]
		public void publicClassWithUnderscoreTest ()
		{
			TypeDefinition type = GetTest ("PublicClassName_WithUnderscore");
			Assert.IsNull (typeRule.CheckType (type, new MinimalRunner ()));
		}
		
		[Test]
		public void privateInterfaceWithUnderscoreTest ()
		{
			TypeDefinition type = GetTest ("PrivateInterface_WithUnderscore");
			Assert.IsNull (typeRule.CheckType (type, new MinimalRunner ()));
		}
		
		[Test]
		public void publicInterfaceWithUnderscoreTest ()
		{
			TypeDefinition type = GetTest ("PublicInterface_WithUnderscore");
			Assert.IsNull (typeRule.CheckType (type, new MinimalRunner ()));
		}
		
		[Test]
		public void defaultPrivateClassTest ()
		{
			TypeDefinition type = GetTest ("DefaultPrivate_Class");
			Assert.IsNull (typeRule.CheckType (type, new MinimalRunner ()));
		}
		
		[Test]
		public void defaultPrivateMethodTest ()
		{
			TypeDefinition type = GetTest ("DefaultPrivate_Class");
			foreach (MethodDefinition method in type.Methods) {
				Assert.IsNull (methodRule.CheckMethod (method, new MinimalRunner ()));
			}
		}
		
		[Test]
		public void classWithoutUnderscoreTest ()
		{
			TypeDefinition type = GetTest ("ClassWithoutUnderscore");
			Assert.IsNull (typeRule.CheckType (type, new MinimalRunner ()));
		}
		
		[Test]
		public void methodWithoutUnderscoreTest ()
		{
			TypeDefinition type = GetTest ("ClassWithoutUnderscore");
			foreach (MethodDefinition method in type.Methods) {
				Assert.IsNull (methodRule.CheckMethod (method, new MinimalRunner ()));
			}
		}
	}
}