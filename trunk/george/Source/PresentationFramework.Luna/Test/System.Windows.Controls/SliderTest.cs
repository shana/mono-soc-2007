using NUnit.Framework;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using Mono.System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls {
#else
using System.Windows.Controls.Primitives;
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class SliderTest {
		[Test]
		public void Creation() {
			Slider p = new Slider();
			p.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(p.DesiredSize.Width, 0, "DesiredSize.Width");
			Assert.AreEqual(p.DesiredSize.Height, 0, "DesiredSize.Height");
		}

		#region Layout
		[Test]
		public void Layout() {
			new LayoutSlider();
		}

		class LayoutSlider : Slider {
			public LayoutSlider() {
				Window w = new Window();
				w.Show();
				w.Content = this;
				TickPlacement =  global::System.Windows.Controls.Primitives.TickPlacement.Both;
				ApplyTemplate();
				Width = 100;
				SelectionStart = 4;
				SelectionEnd = 9;
				Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Arrange(new Rect(new Point(), DesiredSize));
				TickBar bottom_tick = (TickBar)GetTemplateChild("BottomTick");
				Assert.AreEqual(bottom_tick.ActualWidth, 100, "bottom_tick.ActualWidth");
				FrameworkElement selection_range = (FrameworkElement)GetTemplateChild("PART_SelectionRange");
				Assert.AreEqual(selection_range.ActualWidth, 1, "selection_range.ActualWidth");
				Assert.AreEqual(Canvas.GetLeft(selection_range), 41.100000000000001d, "Canvas.GetLeft(selection_range)");
			}
		}
		#endregion
	}
}