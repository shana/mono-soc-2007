using NUnit.Framework;
#if Implementation
using System;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	[TestFixture]
	public class UniformGridTest {
		[Test]
		public void NegativeValues() {
			UniformGrid g = new UniformGrid();
			try {
				g.Columns = -1;
				Assert.Fail("1");
			} catch (ArgumentException ex) {
				Assert.AreEqual(ex.Message, "'-1' is not a valid value for property 'Columns'.", "2");
			}
			try {
				g.FirstColumn = -1;
				Assert.Fail("3");
			} catch (ArgumentException ex) {
				Assert.AreEqual(ex.Message, "'-1' is not a valid value for property 'FirstColumn'.", "4");
			}
			try {
				g.Rows = -1;
				Assert.Fail("5");
			} catch (ArgumentException ex) {
				Assert.AreEqual(ex.Message, "'-1' is not a valid value for property 'Rows'.", "6");
			}
		}
	}
}