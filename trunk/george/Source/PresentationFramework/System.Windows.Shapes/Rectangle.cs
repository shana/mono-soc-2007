using System.ComponentModel;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Shapes {
#else
namespace System.Windows.Shapes {
#endif
	public sealed class Rectangle : global::System.Windows.Shapes.Shape {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty RadiusXProperty = DependencyProperty.Register("RadiusX", typeof(double), typeof(Rectangle), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty RadiusYProperty = DependencyProperty.Register("RadiusY", typeof(double), typeof(Rectangle), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		#endregion
		#endregion

		#region Private Fields
		Geometry rendered_geometry;
		#endregion

		#region Public Constructors
		public Rectangle() {
			Stretch = Stretch.Fill;
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[TypeConverter(typeof(LengthConverter))]
		public double RadiusX {
			get { return (double)GetValue(RadiusXProperty); }
			set { SetValue(RadiusXProperty, value); }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double RadiusY {
			get { return (double)GetValue(RadiusYProperty); }
			set { SetValue(RadiusYProperty, value); }
		}
		#endregion

		public override Transform GeometryTransform {
			get {
				return base.GeometryTransform;
			}
		}

		public override Geometry RenderedGeometry {
			get {
				return rendered_geometry ?? new RectangleGeometry();
			}
		}
		#endregion

		#region Protected Properties
		protected override Geometry DefiningGeometry {
			get {
				double width;
				double height;
				width = ActualWidth;
				height = ActualHeight;
				double stroke_thickness = Stroke == null ? 0 : StrokeThickness;
				if (width >= stroke_thickness && height > stroke_thickness)
					return new RectangleGeometry(new Rect(stroke_thickness / 2, stroke_thickness / 2, width - stroke_thickness, height - stroke_thickness), RadiusX, RadiusY);
				else
					return new RectangleGeometry();
			}
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize) {
			return finalSize;
		}

		protected override Size MeasureOverride(Size constraint) {
			return new Size(ZeroIfInfinite(constraint.Width), ZeroIfInfinite(constraint.Height));
		}

		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			rendered_geometry = DefiningGeometry;
			Brush fill = Fill;
			if (fill != null || Stroke != null) {
				//drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight)));
				drawingContext.DrawGeometry(fill, CreatePen(), DefiningGeometry);
				//drawingContext.Pop();
			}
		}
		#endregion
		
		#region Private Methods
		Pen CreatePen() {
			Pen result = new Pen(Stroke, StrokeThickness);
			result.DashStyle = new DashStyle(StrokeDashArray, StrokeDashOffset);
			result.DashCap = StrokeDashCap;
			result.StartLineCap = StrokeStartLineCap;
			result.EndLineCap = StrokeEndLineCap;
			result.LineJoin = StrokeLineJoin;
			result.MiterLimit = StrokeMiterLimit;
			return result;
		}

		static double ZeroIfInfinite(double value) {
			return double.IsPositiveInfinity(value) ? 0 : value;
		}
		#endregion
	}
}