using NUnit.Framework;
using System;
using System.Windows;
using System.Windows.Media;
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
			Assert.AreEqual(c.Padding, new Thickness(0), "Padding");
			Assert.AreEqual(c.Margin, new Thickness(0), "Margin");
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

		[Test]
		public void Drawing() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 100;
			s.Height = 100;
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 3, "1");

			Drawing d1 = drawing_group.Children[0];
			Assert.AreEqual(d1.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd1 = (GeometryDrawing)d1;
			Assert.AreEqual(gd1.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg1 = (RectangleGeometry)gd1.Geometry;
			Assert.AreEqual(rg1.Rect, new Rect(0.5, 2.5, 99, 97), "4");
			Assert.AreEqual(rg1.RadiusX, 3, "5");
			Assert.AreEqual(rg1.RadiusY, 3, "6");
			Assert.IsNull(gd1.Brush, "7");
			Assert.IsNotNull(gd1.Pen, "8");
			Assert.AreEqual(gd1.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush pen_lgb1 = (LinearGradientBrush)gd1.Pen.Brush;
			Assert.AreEqual(pen_lgb1.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(pen_lgb1.GradientStops.Count, 3, "11");
			Assert.AreEqual(pen_lgb1.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(pen_lgb1.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(pen_lgb1.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(pen_lgb1.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(pen_lgb1.GradientStops[2].Color, Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(pen_lgb1.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(pen_lgb1.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd1.Pen.Thickness, 1, "19");
		}
	}
}