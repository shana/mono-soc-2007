using System;
using System.Reflection;

using Gendarme.Framework;
using Gendarme.Rules.Naming;
using Mono.Cecil;
using NUnit.Framework;

namespace Test.Rules.Naming {

	public class CorrectAttribute : Attribute {
	}

	public class Incorrect : Attribute {
	}

	public class OtherAttribute : CorrectAttribute {
	}
	
	public class Other : CorrectAttribute {
	}
	
	public class CorrectContextStaticAttribute : ContextStaticAttribute {
	}
	
	[TestFixture]
	public class AttributeEndsWithAttributeSuffixTest {
		private ITypeRule rule;
		private AssemblyDefinition assembly;
		private TypeDefinition type;
	
		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			rule = new AttributeEndsWithAttributeSuffixRule ();
		}
		
		[Test]
		public void TestOneLevelInheritanceIncorrectName () 
		{
			type = assembly.MainModule.Types ["Test.Rules.Naming.Incorrect"];
			Assert.IsNotNull (rule.CheckType (type, new MinimalRunner ()));
			Assert.AreEqual (rule.CheckType (type, new MinimalRunner ()).Count, 1);
		}
		
		[Test]
		public void TestOneLevelInheritanceCorrectName () 
		{
			type = assembly.MainModule.Types ["Test.Rules.Naming.CorrectAttribute"];
			Assert.IsNull (rule.CheckType (type, new MinimalRunner ()));
		}
		
		[Test]
		public void TestVariousLevelInheritanceCorrectName () {
			type = assembly.MainModule.Types ["Test.Rules.Naming.OtherAttribute"];
			Assert.IsNull (rule.CheckType (type, new MinimalRunner ()));
		}
		
		[Test]
		public void TestVariousLevelInheritanceIncorrectName () 
		{
			type = assembly.MainModule.Types ["Test.Rules.Naming.Other"];
			Assert.IsNotNull (rule.CheckType (type, new MinimalRunner ()));
			Assert.AreEqual (rule.CheckType (type, new MinimalRunner ()).Count, 1);
		}
		
		[Test]
		public void TestVariousLevelInheritanceExternalTypeCorrectName () 
		{
		    type = assembly.MainModule.Types ["Test.Rules.Naming.CorrectContextStaticAttribute"];
		    Assert.IsNull (rule.CheckType (type, new MinimalRunner ()));
		}
	}
}