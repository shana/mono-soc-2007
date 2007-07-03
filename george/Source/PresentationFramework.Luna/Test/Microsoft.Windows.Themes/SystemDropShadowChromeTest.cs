using NUnit.Framework;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
#if Implementation
namespace Mono.Microsoft.Windows.Themes {
#else
namespace Microsoft.Windows.Themes {
#endif
	[TestFixture]
	public class SystemDropShadowChromeTest {
		[Test]
		public void Drawing() {
			SystemDropShadowChrome s = new SystemDropShadowChrome();
			Assert.AreEqual(s.Color, Color.FromArgb(0x71, 0, 0, 0), "Color");
			Assert.AreEqual(s.CornerRadius.BottomLeft, 0, "CornerRadius 1");
			Assert.AreEqual(s.CornerRadius.BottomRight, 0, "CornerRadius 2");
			Assert.AreEqual(s.CornerRadius.TopLeft, 0, "CornerRadius 3");
			Assert.AreEqual(s.CornerRadius.TopRight, 0, "CornerRadius 4");
			s.Width = 100;
			s.Height = 100;
			Window w = new Window();
			w.Content = s;
			w.Show();
			DrawingGroup g = VisualTreeHelper.GetDrawing(s);
			Assert.AreEqual(g.Children.Count, 1, "1");
			g = (DrawingGroup)g.Children[0];
			Assert.AreEqual(g.Children.Count, 9, "2");
			Assert.AreEqual(((RectangleGeometry)((GeometryDrawing)g.Children[0]).Geometry).Rect, new Rect(5, 5, 5, 5), "3");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[0]).Brush).Center, new Point(1, 1), "3 brush center");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[0]).Brush).GradientOrigin, new Point(1, 1), "3 gradient origin");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[0]).Brush).RadiusX, 1, "3 radius x");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[0]).Brush).RadiusY, 1, "3 radius y");
			
			Assert.AreEqual(((RectangleGeometry)((GeometryDrawing)g.Children[1]).Geometry).Rect, new Rect(10, 5, 90, 5), "4");
			Assert.AreEqual(((LinearGradientBrush)((GeometryDrawing)g.Children[1]).Brush).StartPoint, new Point(0, 1), "4 brush start point");
			Assert.AreEqual(((LinearGradientBrush)((GeometryDrawing)g.Children[1]).Brush).EndPoint, new Point(0, 0), "4 brush end point");

			Assert.AreEqual(((RectangleGeometry)((GeometryDrawing)g.Children[2]).Geometry).Rect, new Rect(100, 5, 5, 5), "5");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[2]).Brush).Center, new Point(0, 1), "5 brush center");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[2]).Brush).GradientOrigin, new Point(0, 1), "5 gradient origin");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[2]).Brush).RadiusX, 1, "5 radius x");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[2]).Brush).RadiusY, 1, "5 radius y");
			
			Assert.AreEqual(((RectangleGeometry)((GeometryDrawing)g.Children[3]).Geometry).Rect, new Rect(5, 10, 5, 90), "6");
			Assert.AreEqual(((LinearGradientBrush)((GeometryDrawing)g.Children[3]).Brush).StartPoint, new Point(1, 0), "6 brush start point");
			Assert.AreEqual(((LinearGradientBrush)((GeometryDrawing)g.Children[3]).Brush).EndPoint, new Point(0, 0), "6 brush end point");

			Assert.AreEqual(((RectangleGeometry)((GeometryDrawing)g.Children[4]).Geometry).Rect, new Rect(100, 10, 5, 90), "7");
			Assert.AreEqual(((LinearGradientBrush)((GeometryDrawing)g.Children[4]).Brush).StartPoint, new Point(0, 0), "7 brush start point");
			Assert.AreEqual(((LinearGradientBrush)((GeometryDrawing)g.Children[4]).Brush).EndPoint, new Point(1, 0), "7 brush end point");

			Assert.AreEqual(((RectangleGeometry)((GeometryDrawing)g.Children[5]).Geometry).Rect, new Rect(5, 100, 5, 5), "8");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[5]).Brush).Center, new Point(1, 0), "8 brush center");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[5]).Brush).GradientOrigin, new Point(1, 0), "8 gradient origin");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[5]).Brush).RadiusX, 1, "8 radius x");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[5]).Brush).RadiusY, 1, "8 radius y");

			Assert.AreEqual(((RectangleGeometry)((GeometryDrawing)g.Children[6]).Geometry).Rect, new Rect(10, 100, 90, 5), "9");
			Assert.AreEqual(((LinearGradientBrush)((GeometryDrawing)g.Children[6]).Brush).StartPoint, new Point(0, 0), "9 brush start point");
			Assert.AreEqual(((LinearGradientBrush)((GeometryDrawing)g.Children[6]).Brush).EndPoint, new Point(0, 1), "9 brush end point");

			Assert.AreEqual(((RectangleGeometry)((GeometryDrawing)g.Children[7]).Geometry).Rect, new Rect(100, 100, 5, 5), "10");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[7]).Brush).Center, new Point(0, 0), "10 brush center");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[7]).Brush).GradientOrigin, new Point(0, 0), "10 gradient origin");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[7]).Brush).RadiusX, 1, "10 radius x");
			Assert.AreEqual(((RadialGradientBrush)((GeometryDrawing)g.Children[7]).Brush).RadiusY, 1, "10 radius y");

			Assert.AreEqual(((RectangleGeometry)((GeometryDrawing)g.Children[8]).Geometry).Rect, new Rect(10, 10, 90, 90), "11");
			Assert.AreEqual(((SolidColorBrush)((GeometryDrawing)g.Children[8]).Brush).Color, s.Color, "10 color");
		}
	}
}