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
		public void DrawingGlyph() {
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
			Assert.AreEqual(((DrawingGroup)d).Children.Count, 1, "2 4");
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