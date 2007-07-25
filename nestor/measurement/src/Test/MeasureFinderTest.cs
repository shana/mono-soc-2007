using System;
using System.Collections;
using NUnit.Framework;
using Mono.Cecil;

using Measures;

namespace Test.Measures {
	[TestFixture]
	public class MeasureFinderTest {
		MeasureFinder measureFinder;
		IEnumerable measures;
		AssemblyDefinition assembly;

		[TestFixtureSetUp]
		public void TestFixtureSetUp () 
		{
			assembly = AssemblyFactory.GetAssembly ("Test.Assembly.dll");
			measureFinder = new MeasureFinder ();
			measures = new MeasureCalculator ().ProcessMeasures (assembly);
		}

		[Test]
		public void MethodWithOneParameterTest () 
		{
			IEnumerable results = measureFinder.FindByNumberOfParameters (measures, 1);
			IEnumerator resultEnumerator = results.GetEnumerator ();
			Assert.IsNotNull (results);
			Assert.AreEqual (1, ((ICollection) results).Count);
			Assert.IsTrue (resultEnumerator.MoveNext ());
			TypeMeasure typeMeasure = (TypeMeasure) resultEnumerator.Current;
			Assert.AreEqual (3, ((ICollection) typeMeasure.MethodMeasures).Count);
		}

		[Test]
		public void MethodWithTwoParameterTest () 
		{
			IEnumerable results = measureFinder.FindByNumberOfParameters (measures, 2);
			IEnumerator resultEnumerator = results.GetEnumerator ();
			Assert.IsNotNull (results);
			Assert.AreEqual (1, ((ICollection) results).Count);
			Assert.IsTrue (resultEnumerator.MoveNext ());
			TypeMeasure typeMeasure = (TypeMeasure) resultEnumerator.Current;
			Assert.AreEqual (2, ((ICollection) typeMeasure.MethodMeasures).Count);
		}

		[Test]
		public void MethodWithThreeparameterTest () 
		{
			IEnumerable results = measureFinder.FindByNumberOfParameters (measures, 3);
			IEnumerator resultEnumerator = results.GetEnumerator ();
			Assert.IsNotNull (results);
			Assert.AreEqual (1, ((ICollection) results).Count);
			Assert.IsTrue (resultEnumerator.MoveNext ());
			TypeMeasure typeMeasure = (TypeMeasure) resultEnumerator.Current;
			Assert.AreEqual (1, ((ICollection) typeMeasure.MethodMeasures).Count);
		}
	}
}
