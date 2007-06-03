using NUnit.Framework;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class DockPanelTest {
		[Test]
		public void MeasureOverride() {
			new MeasureOverrideDockPanel();
		}

		class MeasureOverrideDockPanel : DockPanel {
			public MeasureOverrideDockPanel() {
				Size result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 0, "1");
				result = MeasureOverride(new Size(100, 100));
				Assert.AreEqual(result.Width, 0, "2");
				Window w = new Window();
				w.Content = this;
				w.Show();
				result = MeasureOverride(new Size(100, 100));
				Assert.AreEqual(result.Width, 0, "3");
			}
		}
	}
}