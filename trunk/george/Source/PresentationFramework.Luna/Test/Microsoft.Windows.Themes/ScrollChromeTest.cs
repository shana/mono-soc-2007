using NUnit.Framework;
using System;
using System.Windows;
#if Implementation
using Microsoft.Windows.Themes;
namespace Mono.Microsoft.Windows.Themes {
#else
namespace Microsoft.Windows.Themes {
#endif
	[TestFixture]
	public class ScrollChromeTest {
		[Test]
		public void StaticPropertiesDefaultValues() {
			Assert.IsInstanceOfType(typeof(FrameworkPropertyMetadata), ScrollChrome.HasOuterBorderProperty.GetMetadata(typeof(ScrollChrome)));
		}

		[Test]
		public void Creation() {
			ScrollChrome c = new ScrollChrome();
			Assert.IsTrue(c.HasOuterBorder, "HasOuterBorder");
		}

		[Test]
		public void MeasureOverride() {
			ScrollChrome s = new ScrollChrome();
			s.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(s.DesiredSize.Width, 0, "DesiredSize.Width");
			Assert.AreEqual(s.DesiredSize.Height, 0, "DesiredSize.Height");
		}

		[Test]
		public void ArrangeOverride() {
			ScrollChrome s = new ScrollChrome();
			s.Arrange(new Rect(0, 0, 100, 100));
			Assert.AreEqual(s.DesiredSize.Width, 0, "DesiredSize.Width");
			Assert.AreEqual(s.DesiredSize.Height, 0, "DesiredSize.Height");
			Assert.AreEqual(s.Width, double.NaN, "Width");
			Assert.AreEqual(s.Height, double.NaN, "Height");
		}
	}
}