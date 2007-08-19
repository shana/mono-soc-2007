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
	public class EllipseTest
	{
		[Test]
		public void StretchFill ()
		{
			Assert.AreEqual (new Ellipse ().Stretch, Stretch.Fill);
		}

		[Test]
		public void GeometryTransform ()
		{
			Ellipse e = new Ellipse ();
			Assert.AreSame (e.GeometryTransform, Transform.Identity);
		}

		[Test]
		public void RenderedGeometry ()
		{
			Ellipse e = new Ellipse ();
			Geometry g = e.RenderedGeometry;
			StreamGeometry stream_geometry = (StreamGeometry)g;
			Assert.AreEqual (stream_geometry.ToString (), "");
		}

		[Test]
		public void RenderedGeometryInWindow ()
		{
			Window w = new Window ();
			Ellipse e = new Ellipse ();
			e.Width = 100;
			e.Height = 100;
			w.Content = e;
			w.Show ();
			Geometry g = e.RenderedGeometry;
			EllipseGeometry ellipse_geometry = (EllipseGeometry)g;
			Assert.AreEqual (ellipse_geometry.Bounds.Width, 100, "1");
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing (e);
			Assert.IsNull (drawing_group, "2");
		}

		[Test]
		public void Clipping ()
		{
			Ellipse e = new Ellipse ();
			Assert.IsNull (e.Clip, "1");
			Assert.IsFalse (e.ClipToBounds, "2");
		}

		[Test]
		public void RenderedGeometryInWindowFillStroke ()
		{
			Window w = new Window ();
			Ellipse e = new Ellipse ();
			e.Fill = Brushes.Red;
			e.Stroke = Brushes.Blue;
			e.Width = 100;
			e.Height = 100;
			w.Content = e;
			w.Show ();

			EllipseGeometry rendered_geometry = (EllipseGeometry)e.RenderedGeometry;
			Assert.AreEqual (rendered_geometry.Bounds, new Rect (0.5, 0.5, 99, 99), "1");

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing (e);
			Assert.AreEqual (drawing_group.Children.Count, 1, "2");
			EllipseGeometry drawing_geometry = (EllipseGeometry)((GeometryDrawing)drawing_group.Children [0]).Geometry;
			Assert.AreNotSame (rendered_geometry, drawing_geometry, "3");
			Assert.AreEqual (drawing_geometry.Bounds, new Rect (0.5, 0.5, 99, 99), "4");
		}
	}
}