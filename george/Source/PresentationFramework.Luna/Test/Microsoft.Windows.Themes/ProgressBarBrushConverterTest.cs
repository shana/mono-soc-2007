using NUnit.Framework;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
#if Implementation
namespace Mono.Microsoft.Windows.Themes
{
#else
namespace Microsoft.Windows.Themes {
#endif
	[TestFixture]
	public class ProgressBarBrushConverterTest
	{
		[Test]
		public void Test ()
		{
			ProgressBarBrushConverter p = new ProgressBarBrushConverter ();
			object result = p.Convert (new object [] { Brushes.Red, false, 100, 100, 100 }, null, null, null);
			Assert.IsNull (result, "1");
			result = p.Convert (new object [] { Brushes.Red, false, 100, 100, 100 }, typeof (Brush), null, null);
			Assert.IsNull (result, "2");
			result = p.Convert (new object [] { Brushes.Red, false, 100, 100, 100 }, typeof (Brush), null, CultureInfo.CurrentUICulture);
			Assert.IsNull (result, "3");
			result = p.Convert (new object [] { Brushes.Red, false, 100, 100, 100 }, typeof (Brush), null, CultureInfo.InstalledUICulture);
			Assert.IsNull (result, "4");
			result = p.Convert (new object [] { Brushes.Red, false, 100, 100, 100 }, typeof (Brush), null, new CultureInfo ("en-US"));
			Assert.IsNull (result, "5");
			result = p.Convert (new object [] { Brushes.Red, false, 100D, 100D, 100D }, null, null, null);
			Assert.IsTrue (result is DrawingBrush, "6");
			DrawingBrush drawing_brush = (DrawingBrush)result;
			Assert.AreEqual (drawing_brush.AlignmentX, AlignmentX.Center, "7");
			Assert.AreEqual (drawing_brush.AlignmentY, AlignmentY.Center, "8");
			Assert.AreEqual (drawing_brush.Stretch, Stretch.None, "9");
			Assert.AreEqual (drawing_brush.TileMode, TileMode.None, "10");
			Assert.AreEqual (drawing_brush.Transform, Transform.Identity, "11");
			Assert.AreEqual (drawing_brush.Viewbox, new Rect (0, 0, 100, 100), "12");
			Assert.AreEqual (drawing_brush.Viewport, new Rect (0, 0, 100, 100), "13");
			DrawingGroup drawing = (DrawingGroup)drawing_brush.Drawing;
			Assert.AreEqual (drawing.Bounds, new Rect (0, 0, 100, 100), "14");
			Assert.AreEqual (drawing.Children.Count, 13, "15");
			Assert.AreEqual (drawing.Children [12].GetType (), typeof (GeometryDrawing), "16");
			GeometryDrawing geometry_draing_13 = (GeometryDrawing)drawing.Children [12];
			Assert.AreEqual (geometry_draing_13.Brush, Brushes.Red, "17");
			Assert.AreEqual (geometry_draing_13.Geometry.GetType (), typeof (RectangleGeometry), "18");
			RectangleGeometry geometry_13 = (RectangleGeometry)geometry_draing_13.Geometry;
			Assert.AreEqual (geometry_13.Rect, new Rect (12 * 8, 0, 4, 100), "19");
			Assert.AreEqual (((RectangleGeometry)((GeometryDrawing)drawing.Children [11]).Geometry).Rect, new Rect (11 * 8, 0, 6, 100), "20");
		}
	}
}