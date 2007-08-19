using NUnit.Framework;
using System.Windows;
using System.Windows.Media;
#if Implementation
namespace Mono.System.Windows.Shapes
{
#else
namespace System.Windows.Shapes {
#endif
	[TestFixture]
	public class RectangleTest
	{
		[Test]
		public void StretchFill ()
		{
			Assert.AreEqual (new Rectangle ().Stretch, Stretch.Fill);
		}

		[Test]
		public void GeometryTransform ()
		{
			Rectangle e = new Rectangle ();
			Assert.AreSame (e.GeometryTransform, Transform.Identity);
		}

		[Test]
		public void RenderedGeometry ()
		{
			Assert.IsTrue (new Rectangle ().RenderedGeometry is RectangleGeometry);
		}

		[Test]
		public void RenderedGeometryInWindow ()
		{
			Window w = new Window ();
			Rectangle e = new Rectangle ();
			e.Width = 100;
			e.Height = 100;
			w.Content = e;
			w.Show ();
			Geometry g = e.RenderedGeometry;
			RectangleGeometry rectangle_geometry = (RectangleGeometry)g;
			Assert.AreEqual (rectangle_geometry.Bounds.Width, 100, "1");
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing (e);
			Assert.IsNull (drawing_group, "2");
		}

		[Test]
		public void Clipping ()
		{
			Rectangle e = new Rectangle ();
			Assert.IsNull (e.Clip, "1");
			Assert.IsFalse (e.ClipToBounds, "2");
		}

		[Test]
		public void RenderedGeometryInWindowFillStroke ()
		{
			Window w = new Window ();
			Rectangle e = new Rectangle ();
			e.Fill = Brushes.Red;
			e.Stroke = Brushes.Blue;
			e.Width = 100;
			e.Height = 100;
			w.Content = e;
			w.Show ();

			RectangleGeometry rendered_geometry = (RectangleGeometry)e.RenderedGeometry;
			Assert.AreEqual (rendered_geometry.Bounds, new Rect (0.5, 0.5, 99, 99), "1");

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing (e);
			Assert.AreEqual (drawing_group.Children.Count, 1, "2");
			RectangleGeometry drawing_geometry = (RectangleGeometry)((GeometryDrawing)drawing_group.Children [0]).Geometry;
			Assert.AreNotSame (rendered_geometry, drawing_geometry, "3");
			Assert.AreEqual (drawing_geometry.Bounds, new Rect (0.5, 0.5, 99, 99), "4");
		}
	}
}