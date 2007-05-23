using NUnit.Framework;
using System.Collections;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	[TestFixture]
	public class TickBarTest {
		[Test]
		public void Creation() {
			TickBar p = new TickBar();
			Assert.AreEqual(p.Minimum, 0, "Minimum");
			Assert.AreEqual(p.Maximum, 100, "Maximum");
			Assert.AreEqual(p.Placement, TickBarPlacement.Top, "Placement");
			Assert.AreEqual(p.TickFrequency, 1, "Placement");
			Assert.IsTrue(p.IsEnabled, "IsEnabled");
			Assert.IsTrue(p.SnapsToDevicePixels, "SnapsToDevicePixels");
		}
	}
}