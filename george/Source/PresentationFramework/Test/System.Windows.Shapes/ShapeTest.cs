using NUnit.Framework;
using System.Windows.Controls;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Shapes
{
#else
namespace System.Windows.Shapes {
#endif
	[TestFixture]
	public class ShapeTest
	{
		[Test]
		public void Properties ()
		{
			Assert.AreEqual (Utility.GetOptions (((FrameworkPropertyMetadata)Shape.StretchProperty.GetMetadata (typeof (Shape)))), FrameworkPropertyMetadataOptions.AffectsArrange, "1");
			Assert.AreEqual (Utility.GetOptions (((FrameworkPropertyMetadata)Shape.StrokeDashArrayProperty.GetMetadata (typeof (Shape)))), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, "2");
			Assert.AreEqual (Utility.GetOptions (((FrameworkPropertyMetadata)Shape.StrokeDashCapProperty.GetMetadata (typeof (Shape)))), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, "3");
			Assert.AreEqual (Utility.GetOptions (((FrameworkPropertyMetadata)Shape.StrokeDashOffsetProperty.GetMetadata (typeof (Shape)))), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, "4");
			Assert.AreEqual (Utility.GetOptions (((FrameworkPropertyMetadata)Shape.StrokeEndLineCapProperty.GetMetadata (typeof (Shape)))), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, "5");
			Assert.AreEqual (Utility.GetOptions (((FrameworkPropertyMetadata)Shape.StrokeLineJoinProperty.GetMetadata (typeof (Shape)))), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, "6");
			Assert.AreEqual (Utility.GetOptions (((FrameworkPropertyMetadata)Shape.StrokeMiterLimitProperty.GetMetadata (typeof (Shape)))), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, "7");
			Assert.AreEqual (Utility.GetOptions (((FrameworkPropertyMetadata)Shape.StrokeStartLineCapProperty.GetMetadata (typeof (Shape)))), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, "8");
			Assert.AreEqual (Utility.GetOptions (((FrameworkPropertyMetadata)Shape.StrokeThicknessProperty.GetMetadata (typeof (Shape)))), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, "9");

			Assert.AreEqual (new TestShape ().Stretch, Stretch.None, "10");
			Assert.AreEqual (new TestShape ().StrokeDashArray, new DoubleCollection (), "11");
			Assert.AreEqual (new TestShape ().StrokeDashCap, PenLineCap.Flat, "12");
			Assert.AreEqual (new TestShape ().StrokeDashOffset, 0, "13");
			Assert.AreEqual (new TestShape ().StrokeEndLineCap, PenLineCap.Flat, "14");
			Assert.AreEqual (new TestShape ().StrokeLineJoin, PenLineJoin.Miter, "15");
			Assert.AreEqual (new TestShape ().StrokeMiterLimit, 10, "16");
			Assert.AreEqual (new TestShape ().StrokeStartLineCap, PenLineCap.Flat, "17");
			Assert.AreEqual (new TestShape ().StrokeThickness, 1, "18");
		}

		[Test]
		public void GeometryTransform ()
		{
			TestShape e = new TestShape ();
			Assert.AreSame (e.GeometryTransform, Transform.Identity);
		}

		[Test]
		public void RenderedGeometry ()
		{
			TestShape e = new TestShape ();
			Geometry g = e.RenderedGeometry;
			StreamGeometry stream_geometry = (StreamGeometry)g;
			Assert.AreEqual (stream_geometry.ToString (), "");
		}

		class TestShape : Shape
		{
			protected override Geometry DefiningGeometry {
				get { throw new global::System.Exception ("The method or operation is not implemented."); }
			}
		}

		#region MeasureOverrideCallsDefiningGeometry
		[Test]
		public void MeasureOverrideCallsDefiningGeometry ()
		{
			new MeasureOverrideCallsDefiningGeometryShape ();
		}

		class MeasureOverrideCallsDefiningGeometryShape : Shape
		{
			int calls;

			public MeasureOverrideCallsDefiningGeometryShape ()
			{
				Assert.AreEqual (calls, 0, "1");
				MeasureOverride (new Size (100, 100));
				Assert.AreEqual (calls, 1, "2");
			}

			protected override Geometry DefiningGeometry  {
				get {
					calls++;
					return new RectangleGeometry ();
				}
			}
		}
		#endregion

		#region MeasureOverrideCallsDefiningGeometryException
		[Test]
		[ExpectedException (typeof (NullReferenceException))]
		public void MeasureOverrideCallsDefiningGeometryException ()
		{
			new MeasureOverrideCallsDefiningGeometryExceptionShape ();
		}

		class MeasureOverrideCallsDefiningGeometryExceptionShape : Shape
		{
			public MeasureOverrideCallsDefiningGeometryExceptionShape ()
			{
				MeasureOverride (new Size (100, 100));
			}

			protected override Geometry DefiningGeometry {
				get {
					return null;
				}
			}
		}
		#endregion

		#region ArrangeOverrideCallsRenderedGeometry
		[Test]
		public void ArrangeOverrideCallsRenderedGeometry ()
		{
			new ArrangeOverrideCallsRenderedGeometryShape ();
		}

		class ArrangeOverrideCallsRenderedGeometryShape : Shape
		{
			int calls;

			public ArrangeOverrideCallsRenderedGeometryShape ()
			{
				Assert.AreEqual (calls, 0, "1");
				ArrangeOverride (new Size (100, 100));
				Assert.AreEqual (calls, 0, "2");
			}

			public override Geometry RenderedGeometry {
				get {
					calls++;
					return base.RenderedGeometry;
				}
			}

			protected override Geometry DefiningGeometry {
				get {
					return new RectangleGeometry ();
				}
			}
		}
		#endregion

		#region ArrangeOverrideCallsDefiningGeometry
		[Test]
		public void ArrangeOverrideCallsDefiningGeometry ()
		{
			new ArrangeOverrideCallsDefiningGeometryShape ();
		}

		class ArrangeOverrideCallsDefiningGeometryShape : Shape
		{
			int calls;

			public ArrangeOverrideCallsDefiningGeometryShape ()
			{
				Assert.AreEqual (calls, 0, "1");
				ArrangeOverride (new Size (100, 100));
				Assert.AreEqual (calls, 0, "2");
			}

			protected override Geometry DefiningGeometry {
				get {
					calls++;
					return new RectangleGeometry ();
				}
			}
		}
		#endregion

		#region ArrangeOverrideCallsDefiningGeometryStretchFill
		[Test]
		public void ArrangeOverrideCallsDefiningGeometryStretchFill ()
		{
			new ArrangeOverrideCallsDefiningGeometryStretchFillShape ();
		}

		class ArrangeOverrideCallsDefiningGeometryStretchFillShape : Shape
		{
			int calls;

			public ArrangeOverrideCallsDefiningGeometryStretchFillShape ()
			{
				Stretch = Stretch.Fill;
				Assert.AreEqual (calls, 0, "1");
				ArrangeOverride (new Size (100, 100));
				Assert.AreEqual (calls, 1, "2");
			}

			protected override Geometry DefiningGeometry {
				get {
					calls++;
					return new RectangleGeometry ();
				}
			}
		}
		#endregion

		#region Layout
		[Test]
		public void Layout ()
		{
			new LayoutShape ();
		}

		class LayoutShape : Shape
		{
			public LayoutShape ()
			{
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, double.PositiveInfinity)), new Size (0, 0), "1");
				Assert.AreEqual (MeasureOverride (new Size (100, double.PositiveInfinity)), new Size (0, 0), "2");
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, 100)), new Size (0, 0), "3");
				Assert.AreEqual (MeasureOverride (new Size (100, 100)), new Size (0, 0), "4");
				Assert.AreEqual (ArrangeOverride (new Size (100, 100)), new Size (100, 100), "5");
			}

			protected override Geometry DefiningGeometry {
				get { return new RectangleGeometry (); }
			}
		}
		#endregion

		#region Layout2
		[Test]
		public void Layout2 ()
		{
			new Layout2Shape ();
		}

		class Layout2Shape : Shape
		{
			public Layout2Shape ()
			{
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, double.PositiveInfinity)), new Size (0, 0), "1");
				Assert.AreEqual (MeasureOverride (new Size (100, double.PositiveInfinity)), new Size (0, 0), "2");
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, 100)), new Size (0, 0), "3");
				Assert.AreEqual (MeasureOverride (new Size (100, 100)), new Size (0, 0), "4");
				Assert.AreEqual (ArrangeOverride (new Size (100, 100)), new Size (100, 100), "5");
			}

			protected override Geometry DefiningGeometry {
				get { return new EllipseGeometry (new Rect (0, 0, ActualWidth, ActualHeight)); }
			}
		}
		#endregion

		#region LayoutStretchFill
		[Test]
		public void LayoutStretchFill ()
		{
			new LayoutStretchFillShape ();
		}

		class LayoutStretchFillShape : Shape
		{
			public LayoutStretchFillShape ()
			{
				Stretch = Stretch.Fill;
				Width = 100;
				Height = 100;
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, double.PositiveInfinity)), new Size (0, 0), "1");
				Assert.AreEqual (MeasureOverride (new Size (100, double.PositiveInfinity)), new Size (0, 0), "2");
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, 100)), new Size (0, 0), "3");
				Assert.AreEqual (MeasureOverride (new Size (100, 100)), new Size (0, 0), "4");
				Assert.AreEqual (ArrangeOverride (new Size (100, 100)), new Size (0, 0), "5");
			}

			protected override Geometry DefiningGeometry {
				get { return new EllipseGeometry (new Rect (0, 0, ActualWidth, ActualHeight)); }
			}
		}
		#endregion

		#region LayoutDifferentWidthHeightStretchNone
		[Test]
		public void LayoutDifferentWidthHeightStretchNone ()
		{
			new LayoutDifferentWidthHeightStretchNoneShape ();
		}

		class LayoutDifferentWidthHeightStretchNoneShape : Shape
		{
			public LayoutDifferentWidthHeightStretchNoneShape ()
			{
				Stretch = Stretch.None;
				Width = 40;
				Height = 30;
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, double.PositiveInfinity)), new Size (0, 0), "1");
				Assert.AreEqual (MeasureOverride (new Size (100, double.PositiveInfinity)), new Size (0, 0), "2");
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, 100)), new Size (0, 0), "3");
				Assert.AreEqual (MeasureOverride (new Size (100, 100)), new Size (0, 0), "4");
				Assert.AreEqual (ArrangeOverride (new Size (100, 100)), new Size (100, 100), "5");
			}

			protected override Geometry DefiningGeometry {
				get { return new EllipseGeometry (new Rect (0, 0, ActualWidth, ActualHeight)); }
			}
		}
		#endregion

		#region LayoutDifferentWidthHeightStretchFill
		[Test]
		public void LayoutDifferentWidthHeightStretchFill ()
		{
			new LayoutDifferentWidthHeightStretchFillShape ();
		}

		class LayoutDifferentWidthHeightStretchFillShape : Shape
		{
			public LayoutDifferentWidthHeightStretchFillShape ()
			{
				Stretch = Stretch.Fill;
				Width = 40;
				Height = 30;
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, double.PositiveInfinity)), new Size (0, 0), "1");
				Assert.AreEqual (MeasureOverride (new Size (100, double.PositiveInfinity)), new Size (0, 0), "2");
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, 100)), new Size (0, 0), "3");
				Assert.AreEqual (MeasureOverride (new Size (100, 100)), new Size (0, 0), "4");
				Assert.AreEqual (ArrangeOverride (new Size (100, 100)), new Size (0, 0), "5");
			}

			protected override Geometry DefiningGeometry {
				get { return new EllipseGeometry (new Rect (0, 0, ActualWidth, ActualHeight)); }
			}
		}
		#endregion

		#region LayoutDifferentWidthHeightStretchUniform
		[Test]
		public void LayoutDifferentWidthHeightStretchUniform ()
		{
			new LayoutDifferentWidthHeightStretchUniformShape ();
		}

		class LayoutDifferentWidthHeightStretchUniformShape : Shape
		{
			public LayoutDifferentWidthHeightStretchUniformShape ()
			{
				Stretch = Stretch.Uniform;
				Width = 40;
				Height = 30;
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, double.PositiveInfinity)), new Size (0, 0), "1");
				Assert.AreEqual (MeasureOverride (new Size (100, double.PositiveInfinity)), new Size (0, 0), "2");
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, 100)), new Size (0, 0), "3");
				Assert.AreEqual (MeasureOverride (new Size (100, 100)), new Size (0, 0), "4");
				Assert.AreEqual (ArrangeOverride (new Size (100, 100)), new Size (0, 0), "5");
			}

			protected override Geometry DefiningGeometry {
				get { return new EllipseGeometry (new Rect (0, 0, ActualWidth, ActualHeight)); }
			}
		}
		#endregion

		#region LayoutDifferentWidthHeightStretchUniformToFill
		[Test]
		public void LayoutDifferentWidthHeightStretchUniformToFill ()
		{
			new LayoutDifferentWidthHeightStretchUniformToFillShape ();
		}

		class LayoutDifferentWidthHeightStretchUniformToFillShape : Shape
		{
			public LayoutDifferentWidthHeightStretchUniformToFillShape ()
			{
				Stretch = Stretch.UniformToFill;
				Width = 40;
				Height = 30;
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, double.PositiveInfinity)), new Size (0, 0), "1");
				Assert.AreEqual (MeasureOverride (new Size (100, double.PositiveInfinity)), new Size (0, 0), "2");
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, 100)), new Size (0, 0), "3");
				Assert.AreEqual (MeasureOverride (new Size (100, 100)), new Size (0, 0), "4");
				Assert.AreEqual (ArrangeOverride (new Size (100, 100)), new Size (0, 0), "5");
			}

			protected override Geometry DefiningGeometry {
				get { return new EllipseGeometry (new Rect (0, 0, ActualWidth, ActualHeight)); }
			}
		}
		#endregion

		#region RendersDefiningGeometry
		[Test]
		public void RendersDefiningGeometry ()
		{
			new RendersDefiningGeometryShape ();
		}

		class RendersDefiningGeometryShape : Shape
		{
			public RendersDefiningGeometryShape ()
			{
				Fill = Brushes.Red;
				Width = 100;
				Height = 100;
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				DrawingGroup drawing_group = VisualTreeHelper.GetDrawing (this);
				Assert.IsNotNull (drawing_group, "1");
				Assert.AreEqual (drawing_group.Children.Count, 1, "2");
				GeometryDrawing geometry_drawing = (GeometryDrawing)drawing_group.Children [0];
				Assert.AreEqual (geometry_drawing.Bounds, new Rect (0, 0, 100, 100), "3");
				Assert.IsTrue (geometry_drawing.Geometry is EllipseGeometry, "4");
			}

			protected override Geometry DefiningGeometry {
				get {
					return new EllipseGeometry (new Rect (0, 0, ActualWidth, ActualHeight));
				}
			}
		}
		#endregion

		#region RendersDefiningGeometry2
		[Test]
		public void RendersDefiningGeometry2 ()
		{
			new RendersDefiningGeometry2Shape ();
		}

		class RendersDefiningGeometry2Shape : Shape
		{
			public RendersDefiningGeometry2Shape ()
			{
				Fill = Brushes.Red;
				Width = 100;
				Height = 200;
				Stroke = Brushes.Green;
				StrokeThickness = 30;
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				DrawingGroup drawing_group = VisualTreeHelper.GetDrawing (this);
				Assert.IsNotNull (drawing_group, "1");
				Assert.AreEqual (drawing_group.Children.Count, 1, "2");
				GeometryDrawing geometry_drawing = (GeometryDrawing)drawing_group.Children [0];
				Assert.AreEqual (geometry_drawing.Bounds, new Rect (-15, -15, 130, 230), "3");
				Assert.IsTrue (geometry_drawing.Geometry is EllipseGeometry, "4");
				Assert.AreEqual (geometry_drawing.Geometry.Bounds, new Rect (0, 0, 100, 200), "5");
			}

			protected override Geometry DefiningGeometry {
				get {
					return new EllipseGeometry (new Rect (0, 0, ActualWidth, ActualHeight));
				}
			}
		}
		#endregion

		#region RendersDefiningGeometryStretchFill
		[Test]
		public void RendersDefiningGeometryStretchFill ()
		{
			new RendersDefiningGeometryStretchFillShape ();
		}

		class RendersDefiningGeometryStretchFillShape : Shape
		{
			Size measure_result;
			Size arrange_result;

			public RendersDefiningGeometryStretchFillShape ()
			{
				Fill = Brushes.Red;
				Width = 100;
				Height = 200;
				Stroke = Brushes.Green;
				StrokeThickness = 30;
				Stretch = Stretch.Fill;
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				Assert.AreEqual (DesiredSize, new Size (100, 200), "0");
				Assert.AreEqual (measure_result, new Size (30, 30), "0 1");
				Assert.AreEqual (arrange_result, new Size (30, 30), "0 2");
				DrawingGroup drawing_group = VisualTreeHelper.GetDrawing (this);
				Assert.IsNotNull (drawing_group, "1");
				Assert.AreEqual (drawing_group.Children.Count, 1, "2");
				GeometryDrawing geometry_drawing = (GeometryDrawing)drawing_group.Children [0];
				Assert.AreEqual (geometry_drawing.Bounds, new Rect (0, 0, 60, 60), "3");
				Assert.IsTrue (geometry_drawing.Geometry is EllipseGeometry, "4");
				Assert.AreEqual (geometry_drawing.Geometry.Bounds, new Rect (15, 15, 30, 30), "5");
				Assert.AreEqual (((MatrixTransform)geometry_drawing.Geometry.Transform).Matrix, new Matrix (1, 0, 0, 1, 15, 15), "6");
			}

			protected override Geometry DefiningGeometry {
				get {
					return new EllipseGeometry (new Rect (0, 0, ActualWidth, ActualHeight));
				}
			}

			protected override Size MeasureOverride (Size availableSize)
			{
				return measure_result = base.MeasureOverride (availableSize);
			}

			protected override Size ArrangeOverride (Size finalSize)
			{
				return arrange_result = base.ArrangeOverride (finalSize);
			}
		}
		#endregion
	}
}