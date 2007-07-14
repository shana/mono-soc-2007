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

			Drawing d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 2.5, 99, 97), "4");
			Assert.AreEqual(rg.RadiusX, 3, "5");
			Assert.AreEqual(rg.RadiusY, 3, "6");
			Assert.IsNull(gd.Brush, "7");
			Assert.IsNotNull(gd.Pen, "8");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19");

			d = drawing_group.Children[1];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 2");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 2");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 0.5, 98, 98), "4 2");
			Assert.AreEqual(rg.RadiusX, 2, "5 2");
			Assert.AreEqual(rg.RadiusY, 2, "6 2");
			Assert.IsNotNull(gd.Brush, "7 2");
			Assert.IsNotNull(gd.Pen, "8 2");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 2");
			SolidColorBrush scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Colors.White, "9 2 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(1, 1), "10 2");
			Assert.AreEqual(lgb.GradientStops.Count, 4, "11 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xE1, 0xEA, 0xFE), "12 2");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13 2");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xC3, 0xD3, 0xFD), "14 2");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.29999999999999999, "15 2");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0XC3, 0xD3, 0xFD), "16 2");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 0.59999999999999998, "17 2");
			Assert.AreEqual(lgb.GradientStops[3].Color, Color.FromArgb(0xFF, 0XBB, 0xCD, 0xF9), "16 2 1");
			Assert.AreEqual(lgb.GradientStops[3].Offset, 1, "17 2 1");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18 2");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 2");

			d = drawing_group.Children[2];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 3");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 3");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 1.5, 96, 96), "4 3");
			Assert.AreEqual(rg.RadiusX, 1.5, "5 3");
			Assert.AreEqual(rg.RadiusY, 1.5, "6 3");
			Assert.IsNull(gd.Brush, "7 3");
			Assert.IsNotNull(gd.Pen, "8 3");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 3");
			scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xB4, 0xC8, 0xF6), "9 3 1");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 3");
		}

		[Test]
		public void DrawingSmallSize() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 2.99;
			s.Height = 2.99;
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.IsNull(drawing_group);
			return;
		}

		[Test]
		public void DrawingSmallSize2() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 3;
			s.Height = 3;
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.IsNotNull(drawing_group);
			return;
		}

		[Test]
		public void DrawingSmallSize3() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 6;
			s.Height = 6;
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 3, "1");

			Drawing d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 2.5, 5, 3), "4");
			Assert.AreEqual(rg.RadiusX, 3, "5");
			Assert.AreEqual(rg.RadiusY, 3, "6");
			Assert.IsNull(gd.Brush, "7");
			Assert.IsNotNull(gd.Pen, "8");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19");

			d = drawing_group.Children[1];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 2");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 2");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 0.5, 4, 4), "4 2");
			Assert.AreEqual(rg.RadiusX, 2, "5 2");
			Assert.AreEqual(rg.RadiusY, 2, "6 2");
			Assert.IsNotNull(gd.Brush, "7 2");
			Assert.IsNotNull(gd.Pen, "8 2");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 2");
			SolidColorBrush scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Colors.White, "9 2 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(1, 1), "10 2");
			Assert.AreEqual(lgb.GradientStops.Count, 4, "11 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xE1, 0xEA, 0xFE), "12 2");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13 2");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xC3, 0xD3, 0xFD), "14 2");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.29999999999999999, "15 2");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0XC3, 0xD3, 0xFD), "16 2");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 0.59999999999999998, "17 2");
			Assert.AreEqual(lgb.GradientStops[3].Color, Color.FromArgb(0xFF, 0XBB, 0xCD, 0xF9), "16 2 1");
			Assert.AreEqual(lgb.GradientStops[3].Offset, 1, "17 2 1");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18 2");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 2");

			d = drawing_group.Children[2];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 3");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 3");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 1.5, 2, 2), "4 3");
			Assert.AreEqual(rg.RadiusX, 1.5, "5 3");
			Assert.AreEqual(rg.RadiusY, 1.5, "6 3");
			Assert.IsNull(gd.Brush, "7 3");
			Assert.IsNotNull(gd.Pen, "8 3");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 3");
			scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xB4, 0xC8, 0xF6), "9 3 1");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 3");
		}

		[Test]
		public void DrawingSmallSize4() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 5.5;
			s.Height = 5.5;
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 2, "1");

			Drawing d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 2.5, 4.5, 2.5), "4");
			Assert.AreEqual(rg.RadiusX, 3, "5");
			Assert.AreEqual(rg.RadiusY, 3, "6");
			Assert.IsNull(gd.Brush, "7");
			Assert.IsNotNull(gd.Pen, "8");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19");

			d = drawing_group.Children[1];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 2");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 2");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 0.5, 3.5, 3.5), "4 2");
			Assert.AreEqual(rg.RadiusX, 2, "5 2");
			Assert.AreEqual(rg.RadiusY, 2, "6 2");
			Assert.IsNotNull(gd.Brush, "7 2");
			Assert.IsNotNull(gd.Pen, "8 2");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 2");
			SolidColorBrush scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Colors.White, "9 2 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(1, 1), "10 2");
			Assert.AreEqual(lgb.GradientStops.Count, 4, "11 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xE1, 0xEA, 0xFE), "12 2");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13 2");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xC3, 0xD3, 0xFD), "14 2");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.29999999999999999, "15 2");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0XC3, 0xD3, 0xFD), "16 2");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 0.59999999999999998, "17 2");
			Assert.AreEqual(lgb.GradientStops[3].Color, Color.FromArgb(0xFF, 0XBB, 0xCD, 0xF9), "16 2 1");
			Assert.AreEqual(lgb.GradientStops[3].Offset, 1, "17 2 1");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18 2");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 2");
		}

		[Test]
		public void DrawingSmallSize5() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 3.5;
			s.Height = 3.5;
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 1, "1");

			Drawing d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 2.5, 2.5, 0.5), "4");
			Assert.AreEqual(rg.RadiusX, 3, "5");
			Assert.AreEqual(rg.RadiusY, 3, "6");
			Assert.IsNull(gd.Brush, "7");
			Assert.IsNotNull(gd.Pen, "8");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19");
		}

		[Test]
		public void DrawingHasOuterBorder() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 100;
			s.Height = 100;
			s.HasOuterBorder = false;
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 1, "1");

			Drawing d;
			GeometryDrawing gd;
			RectangleGeometry rg;
			LinearGradientBrush lgb;

			d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 2");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 2");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 0.5, 99, 99), "4 2");
			Assert.AreEqual(rg.RadiusX, 1.5, "5 2");
			Assert.AreEqual(rg.RadiusY, 1.5, "6 2");
			Assert.IsNotNull(gd.Brush, "7 2");
			Assert.IsNotNull(gd.Pen, "8 2");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 2");
			SolidColorBrush scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xB4, 0xC8, 0xF6), "9 2 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(1, 1), "10 2");
			Assert.AreEqual(lgb.GradientStops.Count, 4, "11 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xE1, 0xEA, 0xFE), "12 2");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13 2");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xC3, 0xD3, 0xFD), "14 2");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.29999999999999999, "15 2");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0XC3, 0xD3, 0xFD), "16 2");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 0.59999999999999998, "17 2");
			Assert.AreEqual(lgb.GradientStops[3].Color, Color.FromArgb(0xFF, 0XBB, 0xCD, 0xF9), "16 2 1");
			Assert.AreEqual(lgb.GradientStops[3].Offset, 1, "17 2 1");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18 2");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 2");
		}

		[Test]
		public void DrawingGlyphDownArrow() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 100;
			s.Height = 100;
			ScrollChrome.SetScrollGlyph(s, ScrollGlyph.DownArrow);
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 4, "1");

			Drawing d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 2.5, 98, 97), "4");
			Assert.AreEqual(rg.RadiusX, 3, "5");
			Assert.AreEqual(rg.RadiusY, 3, "6");
			Assert.IsNull(gd.Brush, "7");
			Assert.IsNotNull(gd.Pen, "8");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19");

			d = drawing_group.Children[1];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 2");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 2");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 0.5, 97, 98), "4 2");
			Assert.AreEqual(rg.RadiusX, 2, "5 2");
			Assert.AreEqual(rg.RadiusY, 2, "6 2");
			Assert.IsNotNull(gd.Brush, "7 2");
			Assert.IsNotNull(gd.Pen, "8 2");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 2");
			SolidColorBrush scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Colors.White, "9 2 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(1, 1), "10 2");
			Assert.AreEqual(lgb.GradientStops.Count, 4, "11 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xE1, 0xEA, 0xFE), "12 2");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13 2");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xC3, 0xD3, 0xFD), "14 2");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.29999999999999999, "15 2");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0XC3, 0xD3, 0xFD), "16 2");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 0.59999999999999998, "17 2");
			Assert.AreEqual(lgb.GradientStops[3].Color, Color.FromArgb(0xFF, 0XBB, 0xCD, 0xF9), "16 2 1");
			Assert.AreEqual(lgb.GradientStops[3].Offset, 1, "17 2 1");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18 2");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 2");

			d = drawing_group.Children[2];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 3");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 3");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(2.5, 1.5, 95, 96), "4 3");
			Assert.AreEqual(rg.RadiusX, 1.5, "5 3");
			Assert.AreEqual(rg.RadiusY, 1.5, "6 3");
			Assert.IsNull(gd.Brush, "7 3");
			Assert.IsNotNull(gd.Pen, "8 3");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 3");
			scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xB4, 0xC8, 0xF6), "9 3 1");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 3");

			d = drawing_group.Children[3];
			Assert.AreEqual(d.GetType(), typeof(DrawingGroup), "1 4");
			Assert.AreEqual(((MatrixTransform)((DrawingGroup)d).Transform).Matrix, new Matrix(1, 0, 0, 1, 45.5, 45.5), "2 4");
			Assert.AreEqual(((DrawingGroup)d).Children.Count, 1, "3 4");
			gd = (GeometryDrawing)((DrawingGroup)d).Children[0];
			Assert.AreEqual(gd.Brush.GetType(), typeof(SolidColorBrush), "4 4");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, Color.FromArgb(0xFF, 0x4D, 0x61, 0x85), "5 4");
			Assert.IsNull(gd.Pen, "6 4");
			Assert.AreEqual(gd.Geometry.GetType(), typeof(PathGeometry), "7 4");
			PathGeometry pg = (PathGeometry)gd.Geometry;
			Assert.AreEqual(pg.FillRule, FillRule.EvenOdd, "8 4");
			Assert.AreEqual(pg.Figures.Count, 1, "9 4");
			PathFigure pf = pg.Figures[0];
			Assert.AreEqual(pf.StartPoint, new Point(0, 3.5), "10 4");
			Assert.IsTrue(pf.IsClosed, "11 4");
			Assert.AreEqual(pf.Segments.Count, 5, "12 4");
			TestLineSegment(pf.Segments[0], 4.5, 8, "S 1");
			TestLineSegment(pf.Segments[1], 9, 3.5, "S 2");
			TestLineSegment(pf.Segments[2], 7.5, 2, "S 3");
			TestLineSegment(pf.Segments[3], 4.5, 5, "S 4");
			TestLineSegment(pf.Segments[4], 1.5, 2, "S 5");
		}

		[Test]
		public void DrawingGlyphUpArrow() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 100;
			s.Height = 100;
			ScrollChrome.SetScrollGlyph(s, ScrollGlyph.UpArrow);
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 4, "1");

			Drawing d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 2.5, 98, 97), "4");
			Assert.AreEqual(rg.RadiusX, 3, "5");
			Assert.AreEqual(rg.RadiusY, 3, "6");
			Assert.IsNull(gd.Brush, "7");
			Assert.IsNotNull(gd.Pen, "8");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19");

			d = drawing_group.Children[1];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 2");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 2");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 0.5, 97, 98), "4 2");
			Assert.AreEqual(rg.RadiusX, 2, "5 2");
			Assert.AreEqual(rg.RadiusY, 2, "6 2");
			Assert.IsNotNull(gd.Brush, "7 2");
			Assert.IsNotNull(gd.Pen, "8 2");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 2");
			SolidColorBrush scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Colors.White, "9 2 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(1, 1), "10 2");
			Assert.AreEqual(lgb.GradientStops.Count, 4, "11 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xE1, 0xEA, 0xFE), "12 2");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13 2");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xC3, 0xD3, 0xFD), "14 2");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.29999999999999999, "15 2");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0XC3, 0xD3, 0xFD), "16 2");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 0.59999999999999998, "17 2");
			Assert.AreEqual(lgb.GradientStops[3].Color, Color.FromArgb(0xFF, 0XBB, 0xCD, 0xF9), "16 2 1");
			Assert.AreEqual(lgb.GradientStops[3].Offset, 1, "17 2 1");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18 2");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 2");

			d = drawing_group.Children[2];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 3");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 3");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(2.5, 1.5, 95, 96), "4 3");
			Assert.AreEqual(rg.RadiusX, 1.5, "5 3");
			Assert.AreEqual(rg.RadiusY, 1.5, "6 3");
			Assert.IsNull(gd.Brush, "7 3");
			Assert.IsNotNull(gd.Pen, "8 3");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 3");
			scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xB4, 0xC8, 0xF6), "9 3 1");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 3");

			d = drawing_group.Children[3];
			Assert.AreEqual(d.GetType(), typeof(DrawingGroup), "1 4");
			Assert.AreEqual(((MatrixTransform)((DrawingGroup)d).Transform).Matrix, new Matrix(1, 0, 0, 1, 45.5, 45.5), "2 4");
			Assert.AreEqual(((DrawingGroup)d).Children.Count, 1, "3 4");
			gd = (GeometryDrawing)((DrawingGroup)d).Children[0];
			Assert.AreEqual(gd.Brush.GetType(), typeof(SolidColorBrush), "4 4");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, Color.FromArgb(0xFF, 0x4D, 0x61, 0x85), "5 4");
			Assert.IsNull(gd.Pen, "6 4");
			Assert.AreEqual(gd.Geometry.GetType(), typeof(PathGeometry), "7 4");
			PathGeometry pg = (PathGeometry)gd.Geometry;
			Assert.AreEqual(pg.FillRule, FillRule.EvenOdd, "8 4");
			Assert.AreEqual(pg.Figures.Count, 1, "9 4");
			PathFigure pf = pg.Figures[0];
			Assert.AreEqual(pf.StartPoint, new Point(0, 4.5), "10 4");
			Assert.IsTrue(pf.IsClosed, "11 4");
			Assert.AreEqual(pf.Segments.Count, 5, "12 4");
			TestLineSegment(pf.Segments[0], 4.5, 0, "S 1");
			TestLineSegment(pf.Segments[1], 9, 4.5, "S 2");
			TestLineSegment(pf.Segments[2], 7.5, 6, "S 3");
			TestLineSegment(pf.Segments[3], 4.5, 3, "S 4");
			TestLineSegment(pf.Segments[4], 1.5, 6, "S 5");
		}

		[Test]
		public void DrawingGlyphLeftArrow() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 100;
			s.Height = 100;
			ScrollChrome.SetScrollGlyph(s, ScrollGlyph.LeftArrow);
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 4, "1");

			Drawing d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 3.5, 99, 96), "4");
			Assert.AreEqual(rg.RadiusX, 3, "5");
			Assert.AreEqual(rg.RadiusY, 3, "6");
			Assert.IsNull(gd.Brush, "7");
			Assert.IsNotNull(gd.Pen, "8");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19");

			d = drawing_group.Children[1];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 2");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 2");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 1.5, 98, 97), "4 2");
			Assert.AreEqual(rg.RadiusX, 2, "5 2");
			Assert.AreEqual(rg.RadiusY, 2, "6 2");
			Assert.IsNotNull(gd.Brush, "7 2");
			Assert.IsNotNull(gd.Pen, "8 2");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 2");
			SolidColorBrush scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Colors.White, "9 2 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(1, 1), "10 2");
			Assert.AreEqual(lgb.GradientStops.Count, 4, "11 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xE1, 0xEA, 0xFE), "12 2");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13 2");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xC3, 0xD3, 0xFD), "14 2");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.29999999999999999, "15 2");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0XC3, 0xD3, 0xFD), "16 2");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 0.59999999999999998, "17 2");
			Assert.AreEqual(lgb.GradientStops[3].Color, Color.FromArgb(0xFF, 0XBB, 0xCD, 0xF9), "16 2 1");
			Assert.AreEqual(lgb.GradientStops[3].Offset, 1, "17 2 1");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18 2");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 2");

			d = drawing_group.Children[2];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 3");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 3");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 2.5, 96, 95), "4 3");
			Assert.AreEqual(rg.RadiusX, 1.5, "5 3");
			Assert.AreEqual(rg.RadiusY, 1.5, "6 3");
			Assert.IsNull(gd.Brush, "7 3");
			Assert.IsNotNull(gd.Pen, "8 3");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 3");
			scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xB4, 0xC8, 0xF6), "9 3 1");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 3");

			d = drawing_group.Children[3];
			Assert.AreEqual(d.GetType(), typeof(DrawingGroup), "1 4");
			Assert.AreEqual(((MatrixTransform)((DrawingGroup)d).Transform).Matrix, new Matrix(1, 0, 0, 1, 45.5, 45.5), "2 4");
			Assert.AreEqual(((DrawingGroup)d).Children.Count, 1, "3 4");
			gd = (GeometryDrawing)((DrawingGroup)d).Children[0];
			Assert.AreEqual(gd.Brush.GetType(), typeof(SolidColorBrush), "4 4");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, Color.FromArgb(0xFF, 0x4D, 0x61, 0x85), "5 4");
			Assert.IsNull(gd.Pen, "6 4");
			Assert.AreEqual(gd.Geometry.GetType(), typeof(PathGeometry), "7 4");
			PathGeometry pg = (PathGeometry)gd.Geometry;
			Assert.AreEqual(pg.FillRule, FillRule.EvenOdd, "8 4");
			Assert.AreEqual(pg.Figures.Count, 1, "9 4");
			PathFigure pf = pg.Figures[0];
			Assert.AreEqual(pf.StartPoint, new Point(4.5, 0), "10 4");
			Assert.IsTrue(pf.IsClosed, "11 4");
			Assert.AreEqual(pf.Segments.Count, 5, "12 4");
			TestLineSegment(pf.Segments[0], 0, 4.5, "S 1");
			TestLineSegment(pf.Segments[1], 4.5, 9, "S 2");
			TestLineSegment(pf.Segments[2], 6, 7.5, "S 3");
			TestLineSegment(pf.Segments[3], 3, 4.5, "S 4");
			TestLineSegment(pf.Segments[4], 6, 1.5, "S 5");
		}

		[Test]
		public void DrawingGlyphRightArrow() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 100;
			s.Height = 100;
			ScrollChrome.SetScrollGlyph(s, ScrollGlyph.RightArrow);
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 4, "1");

			Drawing d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 3.5, 99, 96), "4");
			Assert.AreEqual(rg.RadiusX, 3, "5");
			Assert.AreEqual(rg.RadiusY, 3, "6");
			Assert.IsNull(gd.Brush, "7");
			Assert.IsNotNull(gd.Pen, "8");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19");

			d = drawing_group.Children[1];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 2");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 2");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 1.5, 98, 97), "4 2");
			Assert.AreEqual(rg.RadiusX, 2, "5 2");
			Assert.AreEqual(rg.RadiusY, 2, "6 2");
			Assert.IsNotNull(gd.Brush, "7 2");
			Assert.IsNotNull(gd.Pen, "8 2");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 2");
			SolidColorBrush scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Colors.White, "9 2 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(1, 1), "10 2");
			Assert.AreEqual(lgb.GradientStops.Count, 4, "11 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xE1, 0xEA, 0xFE), "12 2");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13 2");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xC3, 0xD3, 0xFD), "14 2");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.29999999999999999, "15 2");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0XC3, 0xD3, 0xFD), "16 2");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 0.59999999999999998, "17 2");
			Assert.AreEqual(lgb.GradientStops[3].Color, Color.FromArgb(0xFF, 0XBB, 0xCD, 0xF9), "16 2 1");
			Assert.AreEqual(lgb.GradientStops[3].Offset, 1, "17 2 1");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18 2");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 2");

			d = drawing_group.Children[2];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 3");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 3");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 2.5, 96, 95), "4 3");
			Assert.AreEqual(rg.RadiusX, 1.5, "5 3");
			Assert.AreEqual(rg.RadiusY, 1.5, "6 3");
			Assert.IsNull(gd.Brush, "7 3");
			Assert.IsNotNull(gd.Pen, "8 3");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 3");
			scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xB4, 0xC8, 0xF6), "9 3 1");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 3");

			d = drawing_group.Children[3];
			Assert.AreEqual(d.GetType(), typeof(DrawingGroup), "1 4");
			Assert.AreEqual(((MatrixTransform)((DrawingGroup)d).Transform).Matrix, new Matrix(1, 0, 0, 1, 45.5, 45.5), "2 4");
			Assert.AreEqual(((DrawingGroup)d).Children.Count, 1, "3 4");
			gd = (GeometryDrawing)((DrawingGroup)d).Children[0];
			Assert.AreEqual(gd.Brush.GetType(), typeof(SolidColorBrush), "4 4");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, Color.FromArgb(0xFF, 0x4D, 0x61, 0x85), "5 4");
			Assert.IsNull(gd.Pen, "6 4");
			Assert.AreEqual(gd.Geometry.GetType(), typeof(PathGeometry), "7 4");
			PathGeometry pg = (PathGeometry)gd.Geometry;
			Assert.AreEqual(pg.FillRule, FillRule.EvenOdd, "8 4");
			Assert.AreEqual(pg.Figures.Count, 1, "9 4");
			PathFigure pf = pg.Figures[0];
			Assert.AreEqual(pf.StartPoint, new Point(3.5, 0), "10 4");
			Assert.IsTrue(pf.IsClosed, "11 4");
			Assert.AreEqual(pf.Segments.Count, 5, "12 4");
			TestLineSegment(pf.Segments[0], 8, 4.5, "S 1");
			TestLineSegment(pf.Segments[1], 3.5, 9, "S 2");
			TestLineSegment(pf.Segments[2], 2, 7.5, "S 3");
			TestLineSegment(pf.Segments[3], 5, 4.5, "S 4");
			TestLineSegment(pf.Segments[4], 2, 1.5, "S 5");
		}

		[Test]
		public void DrawingGlyphHorizontalGripper() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 100;
			s.Height = 100;
			ScrollChrome.SetScrollGlyph(s, ScrollGlyph.HorizontalGripper);
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 11, "1");

			Drawing d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 3.5, 99, 96), "4");
			Assert.AreEqual(rg.RadiusX, 3, "5");
			Assert.AreEqual(rg.RadiusY, 3, "6");
			Assert.IsNull(gd.Brush, "7");
			Assert.IsNotNull(gd.Pen, "8");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19");

			d = drawing_group.Children[1];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 2");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 2");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 1.5, 98, 97), "4 2");
			Assert.AreEqual(rg.RadiusX, 2, "5 2");
			Assert.AreEqual(rg.RadiusY, 2, "6 2");
			Assert.IsNotNull(gd.Brush, "7 2");
			Assert.IsNotNull(gd.Pen, "8 2");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 2");
			SolidColorBrush scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Colors.White, "9 2 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10 2");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xC9, 0xD8, 0xFC), "12 2");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13 2");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xC2, 0xD3, 0xFC), "14 2");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.65000000000000002, "15 2");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0xB6, 0xCD, 0xFB), "16 2");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17 2");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18 2");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 2");

			d = drawing_group.Children[2];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 3");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 3");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 2.5, 96, 95), "4 3");
			Assert.AreEqual(rg.RadiusX, 1.5, "5 3");
			Assert.AreEqual(rg.RadiusY, 1.5, "6 3");
			Assert.IsNull(gd.Brush, "7 3");
			Assert.IsNotNull(gd.Pen, "8 3");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 3");
			scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xB4, 0xC8, 0xF6), "9 3 1");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 3");
			
			int drawing_index;
			for (drawing_index = 3; drawing_index <= 10; drawing_index += 2) {
				gd = (GeometryDrawing)drawing_group.Children[drawing_index];
				scb = (SolidColorBrush)gd.Brush;
				Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xEE, 0xF4, 0xFE), "G 1 " + drawing_index);
				Assert.IsNull(gd.Pen, "G 2 " + drawing_index);
				rg = (RectangleGeometry)gd.Geometry;
				Assert.AreEqual(rg.RadiusX, 0, "G 3 " + drawing_index);
				Assert.AreEqual(rg.RadiusY, 0, "G 4 " + drawing_index);
				Assert.AreEqual(rg.Rect, new Rect(45.5 + drawing_index - 3, 47, 1, 5), "G 5 " + drawing_index);
			}
			for (drawing_index = 4; drawing_index <= 10; drawing_index += 2) {
				gd = (GeometryDrawing)drawing_group.Children[drawing_index];
				scb = (SolidColorBrush)gd.Brush;
				Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0x8C, 0xB0, 0xF8), "G2 1 " + drawing_index);
				Assert.IsNull(gd.Pen, "G2 2 " + drawing_index);
				rg = (RectangleGeometry)gd.Geometry;
				Assert.AreEqual(rg.RadiusX, 0, "G2 3 " + drawing_index);
				Assert.AreEqual(rg.RadiusY, 0, "G2 4 " + drawing_index);
				Assert.AreEqual(rg.Rect, new Rect(46.5 + drawing_index - 4, 48, 1, 5), "G2 5 " + drawing_index);
			}
		}

		[Test]
		public void DrawingGlyphVerticalGripper() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 100;
			s.Height = 100;
			ScrollChrome.SetScrollGlyph(s, ScrollGlyph.VerticalGripper);
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 11, "1");

			Drawing d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 2.5, 98, 97), "4");
			Assert.AreEqual(rg.RadiusX, 3, "5");
			Assert.AreEqual(rg.RadiusY, 3, "6");
			Assert.IsNull(gd.Brush, "7");
			Assert.IsNotNull(gd.Pen, "8");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19");

			d = drawing_group.Children[1];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 2");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 2");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 0.5, 97, 98), "4 2");
			Assert.AreEqual(rg.RadiusX, 2, "5 2");
			Assert.AreEqual(rg.RadiusY, 2, "6 2");
			Assert.IsNotNull(gd.Brush, "7 2");
			Assert.IsNotNull(gd.Pen, "8 2");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 2");
			SolidColorBrush scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Colors.White, "9 2 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(1, 0), "10 2");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xC9, 0xD8, 0xFC), "12 2");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13 2");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xC2, 0xD3, 0xFC), "14 2");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.65000000000000002, "15 2");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0xB6, 0xCD, 0xFB), "16 2");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17 2");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18 2");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 2");

			d = drawing_group.Children[2];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 3");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 3");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(2.5, 1.5, 95, 96), "4 3");
			Assert.AreEqual(rg.RadiusX, 1.5, "5 3");
			Assert.AreEqual(rg.RadiusY, 1.5, "6 3");
			Assert.IsNull(gd.Brush, "7 3");
			Assert.IsNotNull(gd.Pen, "8 3");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 3");
			scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xB4, 0xC8, 0xF6), "9 3 1");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 3");

			int drawing_index;
			for (drawing_index = 3; drawing_index <= 10; drawing_index += 2) {
				gd = (GeometryDrawing)drawing_group.Children[drawing_index];
				scb = (SolidColorBrush)gd.Brush;
				Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xEE, 0xF4, 0xFE), "G 1 " + drawing_index);
				Assert.IsNull(gd.Pen, "G 2 " + drawing_index);
				rg = (RectangleGeometry)gd.Geometry;
				Assert.AreEqual(rg.RadiusX, 0, "G 3 " + drawing_index);
				Assert.AreEqual(rg.RadiusY, 0, "G 4 " + drawing_index);
				Assert.AreEqual(rg.Rect, new Rect(47, 45.5 + drawing_index - 3, 5, 1), "G 5 " + drawing_index);
			}
			for (drawing_index = 4; drawing_index <= 10; drawing_index += 2) {
				gd = (GeometryDrawing)drawing_group.Children[drawing_index];
				scb = (SolidColorBrush)gd.Brush;
				Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0x8C, 0xB0, 0xF8), "G2 1 " + drawing_index);
				Assert.IsNull(gd.Pen, "G2 2 " + drawing_index);
				rg = (RectangleGeometry)gd.Geometry;
				Assert.AreEqual(rg.RadiusX, 0, "G2 3 " + drawing_index);
				Assert.AreEqual(rg.RadiusY, 0, "G2 4 " + drawing_index);
				Assert.AreEqual(rg.Rect, new Rect(48, 46.5 + drawing_index - 4, 5, 1), "G2 5 " + drawing_index);
			}
		}

		[Test]
		public void DrawingGlyphHorizontalGripperRenderPressed() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 100;
			s.Height = 100;
			ScrollChrome.SetScrollGlyph(s, ScrollGlyph.HorizontalGripper);
			s.RenderPressed = true;
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 11, "1");

			Drawing d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 3.5, 99, 96), "4");
			Assert.AreEqual(rg.RadiusX, 3, "5");
			Assert.AreEqual(rg.RadiusY, 3, "6");
			Assert.IsNull(gd.Brush, "7");
			Assert.IsNotNull(gd.Pen, "8");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19");

			d = drawing_group.Children[1];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 2");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 2");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 1.5, 98, 97), "4 2");
			Assert.AreEqual(rg.RadiusX, 2, "5 2");
			Assert.AreEqual(rg.RadiusY, 2, "6 2");
			Assert.IsNotNull(gd.Brush, "7 2");
			Assert.IsNotNull(gd.Pen, "8 2");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 2");
			SolidColorBrush scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Colors.White, "9 2 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10 2");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xA8, 0xBE, 0xF5), "12 2");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13 2");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA1, 0xBD, 0xFA), "14 2");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.65000000000000002, "15 2");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0x98, 0xB0, 0xEE), "16 2");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17 2");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18 2");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 2");

			d = drawing_group.Children[2];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 3");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 3");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 2.5, 96, 95), "4 3");
			Assert.AreEqual(rg.RadiusX, 1.5, "5 3");
			Assert.AreEqual(rg.RadiusY, 1.5, "6 3");
			Assert.IsNull(gd.Brush, "7 3");
			Assert.IsNotNull(gd.Pen, "8 3");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 3");
			scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0x83, 0x8F, 0xDA), "9 3 1");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 3");

			int drawing_index;
			for (drawing_index = 3; drawing_index <= 10; drawing_index += 2) {
				gd = (GeometryDrawing)drawing_group.Children[drawing_index];
				scb = (SolidColorBrush)gd.Brush;
				Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xCF, 0xDD, 0xFD), "G 1 " + drawing_index);
				Assert.IsNull(gd.Pen, "G 2 " + drawing_index);
				rg = (RectangleGeometry)gd.Geometry;
				Assert.AreEqual(rg.RadiusX, 0, "G 3 " + drawing_index);
				Assert.AreEqual(rg.RadiusY, 0, "G 4 " + drawing_index);
				Assert.AreEqual(rg.Rect, new Rect(45.5 + drawing_index - 3, 47, 1, 5), "G 5 " + drawing_index);
			}
			for (drawing_index = 4; drawing_index <= 10; drawing_index += 2) {
				gd = (GeometryDrawing)drawing_group.Children[drawing_index];
				scb = (SolidColorBrush)gd.Brush;
				Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0x83, 0x9E, 0xD8), "G2 1 " + drawing_index);
				Assert.IsNull(gd.Pen, "G2 2 " + drawing_index);
				rg = (RectangleGeometry)gd.Geometry;
				Assert.AreEqual(rg.RadiusX, 0, "G2 3 " + drawing_index);
				Assert.AreEqual(rg.RadiusY, 0, "G2 4 " + drawing_index);
				Assert.AreEqual(rg.Rect, new Rect(46.5 + drawing_index - 4, 48, 1, 5), "G2 5 " + drawing_index);
			}
		}

		[Test]
		public void DrawingGlyphHorizontalGripperRenderMouseOver() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 100;
			s.Height = 100;
			ScrollChrome.SetScrollGlyph(s, ScrollGlyph.HorizontalGripper);
			s.RenderMouseOver = true;
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 11, "1");

			Drawing d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 3.5, 99, 96), "4");
			Assert.AreEqual(rg.RadiusX, 3, "5");
			Assert.AreEqual(rg.RadiusY, 3, "6");
			Assert.IsNull(gd.Brush, "7");
			Assert.IsNotNull(gd.Pen, "8");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19");

			d = drawing_group.Children[1];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 2");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 2");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 1.5, 98, 97), "4 2");
			Assert.AreEqual(rg.RadiusX, 2, "5 2");
			Assert.AreEqual(rg.RadiusY, 2, "6 2");
			Assert.IsNotNull(gd.Brush, "7 2");
			Assert.IsNotNull(gd.Pen, "8 2");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 2");
			SolidColorBrush scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Colors.White, "9 2 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10 2");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xDA, 0xE9, 0xFF), "12 2");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13 2");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xD4, 0xE6, 0xFF), "14 2");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.65000000000000002, "15 2");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0xCA, 0xE0, 0xFF), "16 2");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17 2");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18 2");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 2");

			d = drawing_group.Children[2];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 3");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 3");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 2.5, 96, 95), "4 3");
			Assert.AreEqual(rg.RadiusX, 1.5, "5 3");
			Assert.AreEqual(rg.RadiusY, 1.5, "6 3");
			Assert.IsNull(gd.Brush, "7 3");
			Assert.IsNotNull(gd.Pen, "8 3");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 3");
			scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xAC, 0xCE, 0xFF), "9 3 1");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 3");

			int drawing_index;
			for (drawing_index = 3; drawing_index <= 10; drawing_index += 2) {
				gd = (GeometryDrawing)drawing_group.Children[drawing_index];
				scb = (SolidColorBrush)gd.Brush;
				Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xFC, 0xFD, 0xFF), "G 1 " + drawing_index);
				Assert.IsNull(gd.Pen, "G 2 " + drawing_index);
				rg = (RectangleGeometry)gd.Geometry;
				Assert.AreEqual(rg.RadiusX, 0, "G 3 " + drawing_index);
				Assert.AreEqual(rg.RadiusY, 0, "G 4 " + drawing_index);
				Assert.AreEqual(rg.Rect, new Rect(45.5 + drawing_index - 3, 47, 1, 5), "G 5 " + drawing_index);
			}
			for (drawing_index = 4; drawing_index <= 10; drawing_index += 2) {
				gd = (GeometryDrawing)drawing_group.Children[drawing_index];
				scb = (SolidColorBrush)gd.Brush;
				Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0x9C, 0xC5, 0xFF), "G2 1 " + drawing_index);
				Assert.IsNull(gd.Pen, "G2 2 " + drawing_index);
				rg = (RectangleGeometry)gd.Geometry;
				Assert.AreEqual(rg.RadiusX, 0, "G2 3 " + drawing_index);
				Assert.AreEqual(rg.RadiusY, 0, "G2 4 " + drawing_index);
				Assert.AreEqual(rg.Rect, new Rect(46.5 + drawing_index - 4, 48, 1, 5), "G2 5 " + drawing_index);
			}
		}

		[Test]
		public void DrawingGlyphHorizontalGripperSmallSize() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 14;
			s.Height = 14;
			ScrollChrome.SetScrollGlyph(s, ScrollGlyph.HorizontalGripper);
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 3, "1");
		}
		
		[Test]
		public void DrawingGlyphHorizontalGripperSmallSize3() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 14.5;
			s.Height = 9.5;
			ScrollChrome.SetScrollGlyph(s, ScrollGlyph.HorizontalGripper);
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 11, "1");
		}

		[Test]
		public void DrawingGlyphHorizontalGripperSmallSize2() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 14.5;
			s.Height = 14.5;
			ScrollChrome.SetScrollGlyph(s, ScrollGlyph.HorizontalGripper);
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 11, "1");

			Drawing d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 3.5, 13.5, 10.5), "4");
			Assert.AreEqual(rg.RadiusX, 3, "5");
			Assert.AreEqual(rg.RadiusY, 3, "6");
			Assert.IsNull(gd.Brush, "7");
			Assert.IsNotNull(gd.Pen, "8");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19");

			d = drawing_group.Children[1];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 2");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 2");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 1.5, 12.5, 11.5), "4 2");
			Assert.AreEqual(rg.RadiusX, 2, "5 2");
			Assert.AreEqual(rg.RadiusY, 2, "6 2");
			Assert.IsNotNull(gd.Brush, "7 2");
			Assert.IsNotNull(gd.Pen, "8 2");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 2");
			SolidColorBrush scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Colors.White, "9 2 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10 2");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xC9, 0xD8, 0xFC), "12 2");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13 2");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xC2, 0xD3, 0xFC), "14 2");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.65000000000000002, "15 2");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0xB6, 0xCD, 0xFB), "16 2");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17 2");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18 2");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 2");

			d = drawing_group.Children[2];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 3");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 3");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 2.5, 10.5, 9.5), "4 3");
			Assert.AreEqual(rg.RadiusX, 1.5, "5 3");
			Assert.AreEqual(rg.RadiusY, 1.5, "6 3");
			Assert.IsNull(gd.Brush, "7 3");
			Assert.IsNotNull(gd.Pen, "8 3");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 3");
			scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xB4, 0xC8, 0xF6), "9 3 1");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 3");

			int drawing_index;
			for (drawing_index = 3; drawing_index <= 10; drawing_index += 2) {
				gd = (GeometryDrawing)drawing_group.Children[drawing_index];
				scb = (SolidColorBrush)gd.Brush;
				Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0xEE, 0xF4, 0xFE), "G 1 " + drawing_index);
				Assert.IsNull(gd.Pen, "G 2 " + drawing_index);
				rg = (RectangleGeometry)gd.Geometry;
				Assert.AreEqual(rg.RadiusX, 0, "G 3 " + drawing_index);
				Assert.AreEqual(rg.RadiusY, 0, "G 4 " + drawing_index);
				Assert.AreEqual(rg.Rect, new Rect(2.75 + drawing_index - 3, 4.25, 1, 5), "G 5 " + drawing_index);
			}
			for (drawing_index = 4; drawing_index <= 10; drawing_index += 2) {
				gd = (GeometryDrawing)drawing_group.Children[drawing_index];
				scb = (SolidColorBrush)gd.Brush;
				Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0x8C, 0xB0, 0xF8), "G2 1 " + drawing_index);
				Assert.IsNull(gd.Pen, "G2 2 " + drawing_index);
				rg = (RectangleGeometry)gd.Geometry;
				Assert.AreEqual(rg.RadiusX, 0, "G2 3 " + drawing_index);
				Assert.AreEqual(rg.RadiusY, 0, "G2 4 " + drawing_index);
				Assert.AreEqual(rg.Rect, new Rect(3.75 + drawing_index - 4, 5.25, 1, 5), "G2 5 " + drawing_index);
			}
		}

		void TestLineSegment(PathSegment segment, double x, double y, string message) {
			LineSegment ls = (LineSegment)segment;
			Assert.AreEqual(ls.Point, new Point(x, y), message + " 1");
			Assert.IsTrue(ls.IsStroked, message + " 2");
		}

		[Test]
		public void DrawingRenderPressed() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 100;
			s.Height = 100;
			s.RenderPressed = true;
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 3, "1");

			Drawing d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 2.5, 99, 97), "4");
			Assert.AreEqual(rg.RadiusX, 3, "5");
			Assert.AreEqual(rg.RadiusY, 3, "6");
			Assert.IsNull(gd.Brush, "7");
			Assert.IsNotNull(gd.Pen, "8");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19");

			d = drawing_group.Children[1];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 2");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 2");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 0.5, 98, 98), "4 2");
			Assert.AreEqual(rg.RadiusX, 2, "5 2");
			Assert.AreEqual(rg.RadiusY, 2, "6 2");
			Assert.IsNotNull(gd.Brush, "7 2");
			Assert.IsNotNull(gd.Pen, "8 2");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 2");
			SolidColorBrush scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Colors.White, "9 2 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(1, 1), "10 2");
			Assert.AreEqual(lgb.GradientStops.Count, 4, "11 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0x6E, 0x8E, 0xF1), "12 2");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13 2");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0x80, 0x9D, 0xF1), "14 2");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.29999999999999999, "15 2");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0xAF, 0xBF, 0xED), "16 2");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 0.69999999999999996, "17 2");
			Assert.AreEqual(lgb.GradientStops[3].Color, Color.FromArgb(0xFF, 0xD2, 0xDE, 0xEB), "16 2 1");
			Assert.AreEqual(lgb.GradientStops[3].Offset, 1, "17 2 1");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18 2");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 2");

			d = drawing_group.Children[2];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 3");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 3");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 1.5, 96, 96), "4 3");
			Assert.AreEqual(rg.RadiusX, 1.5, "5 3");
			Assert.AreEqual(rg.RadiusY, 1.5, "6 3");
			Assert.IsNull(gd.Brush, "7 3");
			Assert.IsNotNull(gd.Pen, "8 3");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 3");
			scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0x83, 0x8F, 0xDA), "9 3 1");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 3");
		}

		[Test]
		public void DrawingRenderMouseOver() {
			ScrollChrome s = new ScrollChrome();
			s.Width = 100;
			s.Height = 100;
			s.RenderMouseOver = true;
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(drawing_group.Children.Count, 3, "1");

			Drawing d = drawing_group.Children[0];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2");
			GeometryDrawing gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 2.5, 99, 97), "4");
			Assert.AreEqual(rg.RadiusX, 3, "5");
			Assert.AreEqual(rg.RadiusY, 3, "6");
			Assert.IsNull(gd.Brush, "7");
			Assert.IsNotNull(gd.Pen, "8");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(LinearGradientBrush), "9");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(0, 1), "10");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0, 0xA0, 0xB5, 0xD3), "12");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), "14");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.5, "15");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0x7C, 0x9F, 0xD3), "16");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19");

			d = drawing_group.Children[1];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 2");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 2");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 0.5, 98, 98), "4 2");
			Assert.AreEqual(rg.RadiusX, 2, "5 2");
			Assert.AreEqual(rg.RadiusY, 2, "6 2");
			Assert.IsNotNull(gd.Brush, "7 2");
			Assert.IsNotNull(gd.Pen, "8 2");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 2");
			SolidColorBrush scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Colors.White, "9 2 1");
			lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.EndPoint, new Point(1, 1), "10 2");
			Assert.AreEqual(lgb.GradientStops.Count, 3, "11 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xFD, 0xFF, 0xFF), "12 2");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "13 2");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xE2, 0xF3, 0xFD), "14 2");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 0.25, "15 2");
			Assert.AreEqual(lgb.GradientStops[2].Color, Color.FromArgb(0xFF, 0xB9, 0xDA, 0xFB), "16 2");
			Assert.AreEqual(lgb.GradientStops[2].Offset, 1, "17 2");
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "18 2");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 2");

			d = drawing_group.Children[2];
			Assert.AreEqual(d.GetType(), typeof(GeometryDrawing), "2 3");
			gd = (GeometryDrawing)d;
			Assert.AreEqual(gd.Geometry.GetType(), typeof(RectangleGeometry), "3 3");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1.5, 1.5, 96, 96), "4 3");
			Assert.AreEqual(rg.RadiusX, 1.5, "5 3");
			Assert.AreEqual(rg.RadiusY, 1.5, "6 3");
			Assert.IsNull(gd.Brush, "7 3");
			Assert.IsNotNull(gd.Pen, "8 3");
			Assert.AreEqual(gd.Pen.Brush.GetType(), typeof(SolidColorBrush), "9 3");
			scb = (SolidColorBrush)gd.Pen.Brush;
			Assert.AreEqual(scb.Color, Color.FromArgb(0xFF, 0x98, 0xB1, 0xE4), "9 3 1");
			Assert.AreEqual(gd.Pen.Thickness, 1, "19 3");
		}
	}
}