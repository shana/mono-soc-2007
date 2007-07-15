using NUnit.Framework;
using System.Windows;
using System.Windows.Media;
#if Implementation
using Microsoft.Windows.Themes;
namespace Mono.Microsoft.Windows.Themes {
#else
namespace Microsoft.Windows.Themes {
#endif
	[TestFixture]
	public class ButtonChromeTest {
		[Test]
		public void StaticProperties() {
			TestProperty(ButtonChrome.BorderBrushProperty);
			TestProperty(ButtonChrome.FillProperty);
			TestProperty(ButtonChrome.RenderDefaultedProperty);
			TestProperty(ButtonChrome.RenderMouseOverProperty);
			TestProperty(ButtonChrome.RenderPressedProperty);
		}

		void TestProperty(DependencyProperty property) {
			Assert.IsNull(property.ValidateValueCallback, property.Name + " ValidateValueCallback");
			PropertyMetadata metadata = property.GetMetadata(typeof(ButtonChrome));
			Assert.IsInstanceOfType(typeof(FrameworkPropertyMetadata), metadata, property.Name + " metadata type");
			Assert.IsTrue(((FrameworkPropertyMetadata)metadata).AffectsRender, property.Name + " AffectsRender");
		}

		[Test]
		public void Creation() {
			ButtonChrome button_chrome = new ButtonChrome();
			Assert.IsNull(button_chrome.BorderBrush, "button_chrome.BorderBrush");
			Assert.IsNull(button_chrome.Fill, "button_chrome.Fill");
			Assert.IsFalse(button_chrome.RenderDefaulted, "button_chrome.RenderDefaulted");
			Assert.IsFalse(button_chrome.RenderPressed, "button_chrome.RenderPressed");
			Assert.IsFalse(button_chrome.RenderMouseOver, "button_chrome.RenderMouseOver");
			Assert.AreEqual(button_chrome.ThemeColor, ThemeColor.NormalColor, "button_chrome.ThemeColor");
			Assert.IsNull(button_chrome.Clip, "button_chrome.Clip");
		}

		[Test]
		public void Alignment() {
			ButtonChrome b = new ButtonChrome();
			Assert.AreEqual(b.HorizontalAlignment, HorizontalAlignment.Stretch, "HorizontalAlignment");
			Assert.AreEqual(b.VerticalAlignment, VerticalAlignment.Stretch, "VerticalAlignment");
		}

		[Test]
		public void Measure() {
			ButtonChrome b = new ButtonChrome();
			b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(b.DesiredSize.Width, 8, "Width");
			Assert.AreEqual(b.DesiredSize.Height, 8, "Height");
			b.Measure(new Size(double.PositiveInfinity, 10));
			Assert.AreEqual(b.DesiredSize.Width, 8, "Width 2");
			Assert.AreEqual(b.DesiredSize.Height, 8, "Height 2");
			b.Measure(new Size(double.PositiveInfinity, 7));
			Assert.AreEqual(b.DesiredSize.Width, 8, "Width 3");
			Assert.AreEqual(b.DesiredSize.Height, 7, "Height 3");
			b.Measure(new Size(7, 7));
			Assert.AreEqual(b.DesiredSize.Width, 7, "Width 4");
			Assert.AreEqual(b.DesiredSize.Height, 7, "Height 4");

			FrameworkElement x = new FrameworkElement();
			x.Width = 10;
			x.Height = 10;
			b.Child = x;
			b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(b.DesiredSize.Width, 18, "Width 5");
			Assert.AreEqual(b.DesiredSize.Height, 18, "Height 5");
			b.Measure(new Size(double.PositiveInfinity, 0));
			Assert.AreEqual(b.DesiredSize.Width, 18, "Width 6");
			Assert.AreEqual(b.DesiredSize.Height, 0, "Height 6");
			b.Measure(new Size(0, 0));
			Assert.AreEqual(b.DesiredSize.Width, 0, "Width 7");
			Assert.AreEqual(b.DesiredSize.Height, 0, "Height 7");
			b.Measure(new Size(double.PositiveInfinity, 17));
			Assert.AreEqual(b.DesiredSize.Width, 18, "Width 8");
			Assert.AreEqual(b.DesiredSize.Height, 17, "Height 8");

			b = new ButtonChrome();
			b.Measure(new Size(100, 100));
			Assert.AreEqual(b.DesiredSize.Width, 8, "1");
			Window w = new Window();
			w.Content = b;
			w.Show();
			b.Measure(new Size(100, 100));
			Assert.AreEqual(b.DesiredSize.Width, 8, "2");
		}

		[Test]
		public void Drawing() {
			ButtonChrome c = new ButtonChrome();
			c.Width = 100;
			c.Height = 100;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup g = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(g.Children.Count, 3, "1");

			GeometryDrawing gd = (GeometryDrawing)g.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(gd.Pen.Thickness, 1,3333333332999999, "3");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "4");
			Assert.AreEqual(lgb.EndPoint, new Point(0.4, 1), "5");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "6");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0x20, 0x00, 0x00, 0x00), "7");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "8");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "9");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "10");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0x80, 0xFF, 0xFF, 0xFF), "11");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "12");
			Assert.AreEqual(lgb.MappingMode, BrushMappingMode.RelativeToBoundingBox, "13");
			Assert.AreEqual(lgb.ColorInterpolationMode, ColorInterpolationMode.SRgbLinearInterpolation, "14");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 3, "15");
			Assert.AreEqual(rg.RadiusY, 3, "16");
			Assert.AreEqual(rg.Rect, new Rect(2D / 3, 2D / 3, 98 + 2D / 3, 98 + 2D / 3), "17");

			gd = (GeometryDrawing)g.Children[1];
			Assert.IsNull(gd.Pen, "18");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.GradientStops.Count, 2, "19");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "20");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0.5, "21");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0x35, 0x59, 0x2F, 0x00), "22");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "23");
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 0), "24");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 1), "25");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "26");
			Assert.AreEqual(rg.RadiusY, 4, "27");
			Assert.AreEqual(rg.Rect, new Rect(1.05, 92.95, 97.9, 6), "28");

			gd = (GeometryDrawing)g.Children[2];
			Assert.IsNull(gd.Pen, "29");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0.5), "30");
			Assert.AreEqual(lgb.EndPoint, new Point(1, 0.5), "31");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "32");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0x00, 0x59, 0x2F, 0x00), "33");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0.5, "34");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0x28, 0x59, 0x2F, 0x00), "35");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "36");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "37");
			Assert.AreEqual(rg.RadiusY, 4, "38");
			Assert.AreEqual(rg.Rect, new Rect(92.95, 1.05, 6, 97.9), "39");
		}
	}
}