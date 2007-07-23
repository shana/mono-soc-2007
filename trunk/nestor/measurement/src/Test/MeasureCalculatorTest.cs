using System;
using System.Collections;
using Mono.Cecil;
using NUnit.Framework;

using Measures;

namespace Test.Measures {
	[TestFixture]	
	public class MeasureCalculatorTest {
		MeasureCalculator measureCalculator;
		AssemblyDefinition assembly;

		[TestFixtureSetUp]
		public void FixtureSetUp () 
		{
			assembly = AssemblyFactory.GetAssembly ("Test.Assembly.dll");
			measureCalculator = new MeasureCalculator ();
		}

		[Test]
		public void NotEmptyMeasuresTest () 
		{
			Assert.IsNotNull (measureCalculator.ProcessMeasures (assembly));
		}
	}
}
