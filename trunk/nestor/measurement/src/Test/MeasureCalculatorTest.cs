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
		IEnumerable measures;

		[TestFixtureSetUp]
		public void FixtureSetUp () 
		{
			assembly = AssemblyFactory.GetAssembly ("Test.Assembly.dll");
			measureCalculator = new MeasureCalculator ();
		}
		
		[Test]
		public void NotEmptyMeasuresTest () 
		{
			measures = measureCalculator.ProcessMeasures (assembly);
			Assert.IsNotNull (measures);
		}

		[Test]
		public void CountTypeMeasuresTest () 
		{
			measures = measureCalculator.ProcessMeasures (assembly);
			if (measures is ICollection)
				Assert.AreEqual (assembly.MainModule.Types.Count, ((ICollection) measures).Count);
		}
		
		[Test]
		public void CountMethodMeasureTest () 
		{
			measures = measureCalculator.ProcessMeasures (assembly);
			IEnumerator measureEnumerator = measures.GetEnumerator ();
			IEnumerator typeEnumerator = assembly.MainModule.Types.GetEnumerator ();
			while (measureEnumerator.MoveNext () && typeEnumerator.MoveNext ()) {
				TypeDefinition type = (TypeDefinition) typeEnumerator.Current;
				TypeMeasure typeMeasure = (TypeMeasure) measureEnumerator.Current;
				Assert.AreEqual (type.Methods.Count, ((ICollection) typeMeasure.MethodMeasures).Count);
			}
		}
	}
}
