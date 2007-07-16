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
			b.Measure(new Size(100, 100));
			Assert.AreEqual(b.DesiredSize.Width, 8, "Width 4 1");
			Assert.AreEqual(b.DesiredSize.Height, 8, "Height 4 1");
			Assert.AreEqual(b.ActualWidth, 0, "1");
			b.Arrange(new Rect(0, 0, 100, 100));
			Assert.AreEqual(b.ActualWidth, 100, "2");

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
		public void Measure2() {
			ButtonChrome c = new ButtonChrome();
			Window w = new Window();
			System.Windows.Controls.StackPanel p = new System.Windows.Controls.StackPanel();
			p.Children.Add(c);
			w.Content = p;
			w.Show();
			Assert.AreEqual(c.ActualWidth, p.ActualWidth);
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
			Assert.AreEqual(gd.Pen.Thickness, 1.3333333332999999, "3");
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

		[Test]
		public void DrawingFill() {
			ButtonChrome c = new ButtonChrome();
			c.Width = 100;
			c.Height = 100;
			c.Fill = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11)); ;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup g = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(g.Children.Count, 4, "1");

			GeometryDrawing gd = (GeometryDrawing)g.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(gd.Pen.Thickness, 1.3333333332999999, "3");
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

			gd = (GeometryDrawing)g.Children[2];
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

			gd = (GeometryDrawing)g.Children[3];
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

			gd = (GeometryDrawing)g.Children[1];
			Assert.IsNull(gd.Pen, "40");
			Assert.AreSame(gd.Brush, c.Fill, "41");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.75, 0.75, 98.5, 98.5), "42");
			Assert.AreEqual(rg.RadiusX, 4, "43");
			Assert.AreEqual(rg.RadiusY, 4, "44");
		}

		[Test]
		public void DrawingFillBorderBrush() {
			ButtonChrome c = new ButtonChrome();
			c.Width = 100;
			c.Height = 100;
			c.Fill = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup g = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(g.Children.Count, 5, "1");

			GeometryDrawing gd = (GeometryDrawing)g.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(gd.Pen.Thickness, 1.3333333332999999, "3");
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

			gd = (GeometryDrawing)g.Children[2];
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

			gd = (GeometryDrawing)g.Children[3];
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

			gd = (GeometryDrawing)g.Children[1];
			Assert.IsNull(gd.Pen, "40");
			Assert.AreSame(gd.Brush, c.Fill, "41");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.75, 0.75, 98.5, 98.5), "42");
			Assert.AreEqual(rg.RadiusX, 4, "43");
			Assert.AreEqual(rg.RadiusY, 4, "44");

			gd = (GeometryDrawing)g.Children[4];
			Assert.IsNull(gd.Brush, "45");
			Assert.AreEqual(((SolidColorBrush)gd.Pen.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "46");
			Assert.AreEqual(gd.Pen.Thickness, 1, "47");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.25, 1.25, 97.5, 97.5), "48");
			Assert.AreEqual(rg.RadiusX, 3, "49");
			Assert.AreEqual(rg.RadiusY, 3, "50");
		}

		[Test]
		public void DrawingFillBorderBrushRenderPressed() {
			ButtonChrome c = new ButtonChrome();
			c.Width = 100;
			c.Height = 100;
			c.Fill = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.RenderPressed = true;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup g = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(g.Children.Count, 6, "1");

			GeometryDrawing gd = (GeometryDrawing)g.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(gd.Pen.Thickness, 1.3333333332999999, "3");
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

			gd = (GeometryDrawing)g.Children[2];
			Assert.IsNull(gd.Pen, "18");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.GradientStops.Count, 2, "19");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0x97, 0x8B, 0x72), "20");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 1, "21");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "22");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.6, "23");
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 1), "24");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 0), "25");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "26");
			Assert.AreEqual(rg.RadiusY, 4, "27");
			Assert.AreEqual(rg.Rect, new Rect(1.05, 1.05, 97.9, 6), "28");

			gd = (GeometryDrawing)g.Children[3];
			Assert.IsNull(gd.Pen, "29");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 0), "30");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 1), "31");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "32");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "33");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0.59999999999999998, "34");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), "35");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "36");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "37");
			Assert.AreEqual(rg.RadiusY, 4, "38");
			Assert.AreEqual(rg.Rect, new Rect(1.05, 92.95, 97.9, 6), "39");

			gd = (GeometryDrawing)g.Children[4];
			Assert.IsNull(gd.Pen, "29 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(1, 0.5), "30 1");
			Assert.AreEqual(lgb.EndPoint, new Point(0, 0.5), "31 1");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "32 1");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xAA, 0x9D, 0x87), "33 1");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 1, "34 1");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "35 1");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.59999999999999998, "36 1");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "37 1");
			Assert.AreEqual(rg.RadiusY, 4, "38 1");
			Assert.AreEqual(rg.Rect, new Rect(1.05, 1.05, 6, 97.9), "39 1");

			gd = (GeometryDrawing)g.Children[1];
			Assert.IsNull(gd.Pen, "40");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 1), "41 1");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 0), "41 2");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "41 3");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xE6, 0xE6, 0xE0), "41 4");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "41 5");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xE2, 0xE2, 0xDA), "41 6");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "41 7");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.75, 0.75, 98.5, 98.5), "42");
			Assert.AreEqual(rg.RadiusX, 4, "43");
			Assert.AreEqual(rg.RadiusY, 4, "44");

			gd = (GeometryDrawing)g.Children[5];
			Assert.IsNull(gd.Brush, "45");
			Assert.AreEqual(((SolidColorBrush)gd.Pen.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "46");
			Assert.AreEqual(gd.Pen.Thickness, 1, "47");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.25, 1.25, 97.5, 97.5), "48");
			Assert.AreEqual(rg.RadiusX, 3, "49");
			Assert.AreEqual(rg.RadiusY, 3, "50");
		}

		[Test]
		public void DrawingBorderBrushRenderPressed() {
			ButtonChrome c = new ButtonChrome();
			c.Width = 100;
			c.Height = 100;
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.RenderPressed = true;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup g = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(g.Children.Count, 6, "1");

			GeometryDrawing gd = (GeometryDrawing)g.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(gd.Pen.Thickness, 1.3333333332999999, "3");
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

			gd = (GeometryDrawing)g.Children[2];
			Assert.IsNull(gd.Pen, "18");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.GradientStops.Count, 2, "19");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0x97, 0x8B, 0x72), "20");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 1, "21");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "22");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.6, "23");
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 1), "24");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 0), "25");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "26");
			Assert.AreEqual(rg.RadiusY, 4, "27");
			Assert.AreEqual(rg.Rect, new Rect(1.05, 1.05, 97.9, 6), "28");

			gd = (GeometryDrawing)g.Children[3];
			Assert.IsNull(gd.Pen, "29");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 0), "30");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 1), "31");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "32");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "33");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0.59999999999999998, "34");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), "35");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "36");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "37");
			Assert.AreEqual(rg.RadiusY, 4, "38");
			Assert.AreEqual(rg.Rect, new Rect(1.05, 92.95, 97.9, 6), "39");

			gd = (GeometryDrawing)g.Children[4];
			Assert.IsNull(gd.Pen, "29 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(1, 0.5), "30 1");
			Assert.AreEqual(lgb.EndPoint, new Point(0, 0.5), "31 1");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "32 1");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xAA, 0x9D, 0x87), "33 1");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 1, "34 1");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "35 1");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.59999999999999998, "36 1");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "37 1");
			Assert.AreEqual(rg.RadiusY, 4, "38 1");
			Assert.AreEqual(rg.Rect, new Rect(1.05, 1.05, 6, 97.9), "39 1");

			gd = (GeometryDrawing)g.Children[1];
			Assert.IsNull(gd.Pen, "40");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 1), "41 1");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 0), "41 2");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "41 3");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xE6, 0xE6, 0xE0), "41 4");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "41 5");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xE2, 0xE2, 0xDA), "41 6");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "41 7");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.75, 0.75, 98.5, 98.5), "42");
			Assert.AreEqual(rg.RadiusX, 4, "43");
			Assert.AreEqual(rg.RadiusY, 4, "44");

			gd = (GeometryDrawing)g.Children[5];
			Assert.IsNull(gd.Brush, "45");
			Assert.AreEqual(((SolidColorBrush)gd.Pen.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "46");
			Assert.AreEqual(gd.Pen.Thickness, 1, "47");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.25, 1.25, 97.5, 97.5), "48");
			Assert.AreEqual(rg.RadiusX, 3, "49");
			Assert.AreEqual(rg.RadiusY, 3, "50");
		}

		[Test]
		public void DrawingBorderBrushRenderPressedRenderDefaultedRenderMouseOver() {
			ButtonChrome c = new ButtonChrome();
			c.Width = 100;
			c.Height = 100;
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.RenderPressed = true;
			c.RenderDefaulted = true;
			c.RenderMouseOver = true;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup g = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(g.Children.Count, 6, "1");

			GeometryDrawing gd = (GeometryDrawing)g.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(gd.Pen.Thickness, 1.3333333332999999, "3");
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

			gd = (GeometryDrawing)g.Children[2];
			Assert.IsNull(gd.Pen, "18");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.GradientStops.Count, 2, "19");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0x97, 0x8B, 0x72), "20");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 1, "21");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "22");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.6, "23");
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 1), "24");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 0), "25");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "26");
			Assert.AreEqual(rg.RadiusY, 4, "27");
			Assert.AreEqual(rg.Rect, new Rect(1.05, 1.05, 97.9, 6), "28");

			gd = (GeometryDrawing)g.Children[3];
			Assert.IsNull(gd.Pen, "29");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 0), "30");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 1), "31");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "32");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "33");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0.59999999999999998, "34");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), "35");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "36");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "37");
			Assert.AreEqual(rg.RadiusY, 4, "38");
			Assert.AreEqual(rg.Rect, new Rect(1.05, 92.95, 97.9, 6), "39");

			gd = (GeometryDrawing)g.Children[4];
			Assert.IsNull(gd.Pen, "29 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(1, 0.5), "30 1");
			Assert.AreEqual(lgb.EndPoint, new Point(0, 0.5), "31 1");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "32 1");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xAA, 0x9D, 0x87), "33 1");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 1, "34 1");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "35 1");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.59999999999999998, "36 1");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "37 1");
			Assert.AreEqual(rg.RadiusY, 4, "38 1");
			Assert.AreEqual(rg.Rect, new Rect(1.05, 1.05, 6, 97.9), "39 1");

			gd = (GeometryDrawing)g.Children[1];
			Assert.IsNull(gd.Pen, "40");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 1), "41 1");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 0), "41 2");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "41 3");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xE6, 0xE6, 0xE0), "41 4");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "41 5");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xE2, 0xE2, 0xDA), "41 6");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "41 7");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.75, 0.75, 98.5, 98.5), "42");
			Assert.AreEqual(rg.RadiusX, 4, "43");
			Assert.AreEqual(rg.RadiusY, 4, "44");

			gd = (GeometryDrawing)g.Children[5];
			Assert.IsNull(gd.Brush, "45");
			Assert.AreEqual(((SolidColorBrush)gd.Pen.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "46");
			Assert.AreEqual(gd.Pen.Thickness, 1, "47");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.25, 1.25, 97.5, 97.5), "48");
			Assert.AreEqual(rg.RadiusX, 3, "49");
			Assert.AreEqual(rg.RadiusY, 3, "50");
		}

		[Test]
		public void DrawingFillBorderBrushRenderDefaulted() {
			ButtonChrome c = new ButtonChrome();
			c.Width = 100;
			c.Height = 100;
			c.Fill = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.RenderDefaulted = true;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup g = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(g.Children.Count, 6, "1");

			GeometryDrawing gd = (GeometryDrawing)g.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(gd.Pen.Thickness, 1.3333333332999999, "3");
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

			gd = (GeometryDrawing)g.Children[2];
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

			gd = (GeometryDrawing)g.Children[3];
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

			gd = (GeometryDrawing)g.Children[1];
			Assert.IsNull(gd.Pen, "40");
			Assert.AreSame(gd.Brush, c.Fill, "41");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.75, 0.75, 98.5, 98.5), "42");
			Assert.AreEqual(rg.RadiusX, 4, "43");
			Assert.AreEqual(rg.RadiusY, 4, "44");

			gd = (GeometryDrawing)g.Children[5];
			Assert.IsNull(gd.Brush, "45");
			Assert.AreEqual(((SolidColorBrush)gd.Pen.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "46");
			Assert.AreEqual(gd.Pen.Thickness, 1, "47");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.25, 1.25, 97.5, 97.5), "48");
			Assert.AreEqual(rg.RadiusX, 3, "49");
			Assert.AreEqual(rg.RadiusY, 3, "50");

			gd = (GeometryDrawing)g.Children[4];
			Assert.IsNull(gd.Brush, "51");
			lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(gd.Pen.Thickness, 2.6666666666999999, "52");
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 0), "53");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 1), "54");
			Assert.AreEqual(lgb.GradientStops.Count, 4, "55");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xCE, 0xE7, 0xFF), "56");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "57");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xBC, 0xD4, 0xF6), "58");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.3, "50");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0x89, 0xAD, 0xE4), "60");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 0.97, "61");
			Assert.AreEqual(lgb.GradientStops[3].Color, Color.FromArgb(0xFF, 0x69, 0x82, 0xEE), "62");
			Assert.AreEqual(lgb.GradientStops[3].Offset, 1, "63");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 3, "64");
			Assert.AreEqual(rg.RadiusY, 3, "65");
			Assert.AreEqual(rg.Rect.Left, 2.083333333333333, "66");
			Assert.AreEqual(rg.Rect.Top, 2.083333333333333, "67");
			Assert.AreEqual(rg.Rect.Width, 95.833333333333329, "68");
			Assert.AreEqual(rg.Rect.Height, 95.833333333333329, "69");
		}

		[Test]
		public void DrawingFillBorderBrushRenderMouseOver() {
			ButtonChrome c = new ButtonChrome();
			c.Width = 100;
			c.Height = 100;
			c.Fill = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.RenderMouseOver = true;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup g = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(g.Children.Count, 6, "1");

			GeometryDrawing gd = (GeometryDrawing)g.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(gd.Pen.Thickness, 1.3333333332999999, "3");
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

			gd = (GeometryDrawing)g.Children[2];
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

			gd = (GeometryDrawing)g.Children[3];
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

			gd = (GeometryDrawing)g.Children[1];
			Assert.IsNull(gd.Pen, "40");
			Assert.AreSame(gd.Brush, c.Fill, "41");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.75, 0.75, 98.5, 98.5), "42");
			Assert.AreEqual(rg.RadiusX, 4, "43");
			Assert.AreEqual(rg.RadiusY, 4, "44");

			gd = (GeometryDrawing)g.Children[5];
			Assert.IsNull(gd.Brush, "45");
			Assert.AreEqual(((SolidColorBrush)gd.Pen.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "46");
			Assert.AreEqual(gd.Pen.Thickness, 1, "47");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.25, 1.25, 97.5, 97.5), "48");
			Assert.AreEqual(rg.RadiusX, 3, "49");
			Assert.AreEqual(rg.RadiusY, 3, "50");

			gd = (GeometryDrawing)g.Children[4];
			Assert.IsNull(gd.Brush, "51");
			lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(gd.Pen.Thickness, 2.6666666666999999, "52");
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 0), "53");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 1), "54");
			Assert.AreEqual(lgb.GradientStops.Count, 4, "55");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xFF, 0xF0, 0xCF), "56");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "57");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xFC, 0xD2, 0x79), "58");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.03, "50");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0xF8, 0xB7, 0x3B), "60");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 0.75, "61");
			Assert.AreEqual(lgb.GradientStops[3].Color, Color.FromArgb(0xFF, 0xE5, 0x97, 0x00), "62");
			Assert.AreEqual(lgb.GradientStops[3].Offset, 1, "63");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 3, "64");
			Assert.AreEqual(rg.RadiusY, 3, "65");
			Assert.AreEqual(rg.Rect.Left, 2.083333333333333, "66");
			Assert.AreEqual(rg.Rect.Top, 2.083333333333333, "67");
			Assert.AreEqual(rg.Rect.Width, 95.833333333333329, "68");
			Assert.AreEqual(rg.Rect.Height, 95.833333333333329, "69");
		}

		[Test]
		public void DrawingFillBorderBrushRenderMouseOverRenderDefaulted() {
			ButtonChrome c = new ButtonChrome();
			c.Width = 100;
			c.Height = 100;
			c.Fill = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.RenderMouseOver = true;
			c.RenderDefaulted = true;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup g = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(g.Children.Count, 6, "1");

			GeometryDrawing gd = (GeometryDrawing)g.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(gd.Pen.Thickness, 1.3333333332999999, "3");
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

			gd = (GeometryDrawing)g.Children[2];
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

			gd = (GeometryDrawing)g.Children[3];
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

			gd = (GeometryDrawing)g.Children[1];
			Assert.IsNull(gd.Pen, "40");
			Assert.AreSame(gd.Brush, c.Fill, "41");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.75, 0.75, 98.5, 98.5), "42");
			Assert.AreEqual(rg.RadiusX, 4, "43");
			Assert.AreEqual(rg.RadiusY, 4, "44");

			gd = (GeometryDrawing)g.Children[5];
			Assert.IsNull(gd.Brush, "45");
			Assert.AreEqual(((SolidColorBrush)gd.Pen.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "46");
			Assert.AreEqual(gd.Pen.Thickness, 1, "47");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.25, 1.25, 97.5, 97.5), "48");
			Assert.AreEqual(rg.RadiusX, 3, "49");
			Assert.AreEqual(rg.RadiusY, 3, "50");

			gd = (GeometryDrawing)g.Children[4];
			Assert.IsNull(gd.Brush, "51");
			lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(gd.Pen.Thickness, 2.6666666666999999, "52");
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 0), "53");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 1), "54");
			Assert.AreEqual(lgb.GradientStops.Count, 4, "55");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xFF, 0xF0, 0xCF), "56");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "57");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xFC, 0xD2, 0x79), "58");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.03, "50");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0xF8, 0xB7, 0x3B), "60");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 0.75, "61");
			Assert.AreEqual(lgb.GradientStops[3].Color, Color.FromArgb(0xFF, 0xE5, 0x97, 0x00), "62");
			Assert.AreEqual(lgb.GradientStops[3].Offset, 1, "63");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 3, "64");
			Assert.AreEqual(rg.RadiusY, 3, "65");
			Assert.AreEqual(rg.Rect.Left, 2.083333333333333, "66");
			Assert.AreEqual(rg.Rect.Top, 2.083333333333333, "67");
			Assert.AreEqual(rg.Rect.Width, 95.833333333333329, "68");
			Assert.AreEqual(rg.Rect.Height, 95.833333333333329, "69");
		}

		[Test]
		public void DrawingSmallSize() {
			ButtonChrome c = new ButtonChrome();
			c.Width = c.Height = 1 + 1D / 3 - 0.00000001;
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.RenderPressed = true;
			c.RenderDefaulted = true;
			c.RenderMouseOver = true;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup g = VisualTreeHelper.GetDrawing(c);
			Assert.IsNull(g);
		}

		[Test]
		public void DrawingSmallSize2() {
			ButtonChrome c = new ButtonChrome();
			c.Width = c.Height = 1 + 1D / 3;
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.RenderPressed = true;
			c.RenderDefaulted = true;
			c.RenderMouseOver = true;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup g = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(g.Children.Count, 1, "1");

			GeometryDrawing gd = (GeometryDrawing)g.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(gd.Pen.Thickness, 1.3333333332999999, "3");
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
			Assert.AreEqual(rg.Rect, new Rect(2D / 3, 2D / 3, 0, 0), "17");
		}

		[Test]
		public void DrawingSmallSize3() {
			ButtonChrome c = new ButtonChrome();
			c.Width = c.Height = 1.5;
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.RenderPressed = true;
			c.RenderDefaulted = true;
			c.RenderMouseOver = true;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup g = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(g.Children.Count, 2, "1");

			GeometryDrawing gd = (GeometryDrawing)g.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(gd.Pen.Thickness, 1.3333333332999999, "3");
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
			Assert.AreEqual(rg.Rect.Width, 0.16666666666666674, "17");

			gd = (GeometryDrawing)g.Children[1];
			Assert.IsNull(gd.Pen, "40");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 1), "41 1");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 0), "41 2");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "41 3");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xE6, 0xE6, 0xE0), "41 4");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "41 5");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xE2, 0xE2, 0xDA), "41 6");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "41 7");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.75, 0.75, 0, 0), "42");
			Assert.AreEqual(rg.RadiusX, 4, "43");
			Assert.AreEqual(rg.RadiusY, 4, "44");
		}

		[Test]
		public void DrawingSmallSize4() {
			ButtonChrome c = new ButtonChrome();
			c.Width = c.Height = 2.1;
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.RenderPressed = true;
			c.RenderDefaulted = true;
			c.RenderMouseOver = true;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup g = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(g.Children.Count, 5, "1");

			GeometryDrawing gd = (GeometryDrawing)g.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(gd.Pen.Thickness, 1.3333333332999999, "3");
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
			Assert.AreEqual(rg.Rect.Width, 0.76666666666666683, "17");

			gd = (GeometryDrawing)g.Children[2];
			Assert.IsNull(gd.Pen, "18");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.GradientStops.Count, 2, "19");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0x97, 0x8B, 0x72), "20");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 1, "21");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "22");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.6, "23");
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 1), "24");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 0), "25");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "26");
			Assert.AreEqual(rg.RadiusY, 4, "27");

			gd = (GeometryDrawing)g.Children[3];
			Assert.IsNull(gd.Pen, "29");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 0), "30");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 1), "31");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "32");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "33");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0.59999999999999998, "34");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), "35");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "36");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "37");
			Assert.AreEqual(rg.RadiusY, 4, "38");
			Assert.AreEqual(rg.Rect.Top, -4.9499999999999993, "39");

			gd = (GeometryDrawing)g.Children[4];
			Assert.IsNull(gd.Pen, "29 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(1, 0.5), "30 1");
			Assert.AreEqual(lgb.EndPoint, new Point(0, 0.5), "31 1");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "32 1");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xAA, 0x9D, 0x87), "33 1");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 1, "34 1");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "35 1");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.59999999999999998, "36 1");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "37 1");
			Assert.AreEqual(rg.RadiusY, 4, "38 1");

			gd = (GeometryDrawing)g.Children[1];
			Assert.IsNull(gd.Pen, "40");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 1), "41 1");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 0), "41 2");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "41 3");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xE6, 0xE6, 0xE0), "41 4");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "41 5");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xE2, 0xE2, 0xDA), "41 6");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "41 7");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "43");
			Assert.AreEqual(rg.RadiusY, 4, "44");
		}

		[Test]
		public void DrawingSmallSize5() {
			ButtonChrome c = new ButtonChrome();
			c.Width = c.Height = 2.5;
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.RenderPressed = true;
			c.RenderDefaulted = true;
			c.RenderMouseOver = true;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup g = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(g.Children.Count, 6, "1");

			GeometryDrawing gd = (GeometryDrawing)g.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(gd.Pen.Thickness, 1.3333333332999999, "3");
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

			gd = (GeometryDrawing)g.Children[2];
			Assert.IsNull(gd.Pen, "18");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.GradientStops.Count, 2, "19");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0x97, 0x8B, 0x72), "20");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 1, "21");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "22");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.6, "23");
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 1), "24");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 0), "25");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "26");
			Assert.AreEqual(rg.RadiusY, 4, "27");

			gd = (GeometryDrawing)g.Children[3];
			Assert.IsNull(gd.Pen, "29");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 0), "30");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 1), "31");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "32");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "33");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0.59999999999999998, "34");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), "35");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "36");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "37");
			Assert.AreEqual(rg.RadiusY, 4, "38");

			gd = (GeometryDrawing)g.Children[4];
			Assert.IsNull(gd.Pen, "29 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(1, 0.5), "30 1");
			Assert.AreEqual(lgb.EndPoint, new Point(0, 0.5), "31 1");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "32 1");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xAA, 0x9D, 0x87), "33 1");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 1, "34 1");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), "35 1");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.59999999999999998, "36 1");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "37 1");
			Assert.AreEqual(rg.RadiusY, 4, "38 1");

			gd = (GeometryDrawing)g.Children[1];
			Assert.IsNull(gd.Pen, "40");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0.5, 1), "41 1");
			Assert.AreEqual(lgb.EndPoint, new Point(0.5, 0), "41 2");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "41 3");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xE6, 0xE6, 0xE0), "41 4");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "41 5");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xE2, 0xE2, 0xDA), "41 6");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "41 7");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 4, "43");
			Assert.AreEqual(rg.RadiusY, 4, "44");

			gd = (GeometryDrawing)g.Children[5];
			Assert.IsNull(gd.Brush, "45");
			Assert.AreEqual(((SolidColorBrush)gd.Pen.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "46");
			Assert.AreEqual(gd.Pen.Thickness, 1, "47");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.RadiusX, 3, "49");
			Assert.AreEqual(rg.RadiusY, 3, "50");
		}
	}
}