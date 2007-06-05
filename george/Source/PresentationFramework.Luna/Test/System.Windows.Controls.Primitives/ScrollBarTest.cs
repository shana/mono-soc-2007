using NUnit.Framework;
#if Implementation
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
    [TestFixture]
    public class ScrollBarTest {
		[Test]
		public void PartBounds() {
			Window w = new Window();
			ScrollBar s = new ScrollBar();
			w.Content = s;
			w.Show();
			s.Value = s.Maximum;
			Assert.AreEqual(s.Track.Thumb.ActualWidth, 17, "Track.Thumb.ActualWidth");
			Assert.AreEqual(s.Track.Thumb.ActualHeight, 8, "Track.Thumb.ActualHeight");
		}

		[Test]
		public void OnApplyTemplate() {
			ScrollBar p = new ScrollBar();
			Window w = new Window();
			w.Content = p;
			p.OnApplyTemplate();
			w.Show();
			Assert.IsTrue(p.Track.IsEnabled, "Track.IsEnabled");
			Assert.IsTrue(p.Track.IncreaseRepeatButton.IsEnabled, "Track.IncreaseRepeatButton.IsEnabled");
		}
	}
}