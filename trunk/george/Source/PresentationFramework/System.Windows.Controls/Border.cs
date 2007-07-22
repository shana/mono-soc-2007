using Mono.WindowsPresentationFoundation;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public class Border : Decorator {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(Border), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
		public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(Border), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
		public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(Border), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Border), new FrameworkPropertyMetadata(new CornerRadius(), FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(Border), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure));
		#endregion
		#endregion

		#region Public Constructors
		public Border() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public Brush Background {
			get { return (Brush)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}

		public Brush BorderBrush {
			get { return (Brush)GetValue(BorderBrushProperty); }
			set { SetValue(BorderBrushProperty, value); }
		}

		public Thickness BorderThickness {
			get { return (Thickness)GetValue(BorderThicknessProperty); }
			set { SetValue(BorderThicknessProperty, value); }
		}

		public CornerRadius CornerRadius {
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}

		public Thickness Padding {
			get { return (Thickness)GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size arrangeSize) {
			if (Child != null)
				Child.Arrange(new Rect(
					BorderThickness.Left + Padding.Left,
					BorderThickness.Top + Padding.Top,
					Utility.GetAdjustedSize(arrangeSize.Width - BorderThickness.Left - BorderThickness.Right - Padding.Left - Padding.Right),
					Utility.GetAdjustedSize(arrangeSize.Height - BorderThickness.Top - BorderThickness.Bottom - Padding.Top - Padding.Bottom)
				));
			return arrangeSize;
		}

		protected override Size MeasureOverride(Size constraint) {
			return Utility.GetSize(base.MeasureOverride(constraint), Utility.Add(BorderThickness, Padding), constraint);
		}

		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			Thickness border_thickness = BorderThickness;
			CornerRadius corner_radius = CornerRadius;
			double actual_width = ActualWidth;
			double actual_height = ActualHeight;
			double uniform_corner_radius = corner_radius.BottomLeft;
			Brush border_brush = BorderBrush;
			StreamGeometry geometry;
			StreamGeometryContext stream_geometry_context;
			CreateBackgroundShape create_background_shape;
			if (Utility.IsUniform(border_thickness) && uniform_corner_radius == corner_radius.BottomRight && uniform_corner_radius == corner_radius.TopLeft && uniform_corner_radius == corner_radius.TopRight) {
				double uniform_border_thickness = border_thickness.Bottom;
				if (border_brush != null)
					drawingContext.DrawRoundedRectangle(null, new Pen(border_brush, uniform_border_thickness), new Rect(uniform_border_thickness / 2, uniform_border_thickness / 2, actual_width - uniform_border_thickness, actual_height - uniform_border_thickness), uniform_corner_radius, uniform_corner_radius);
				if (Background != null) {
					double background_radius = uniform_corner_radius - 0.5;
					if (background_radius < 0)
						background_radius = 0;
					drawingContext.DrawRoundedRectangle(Background, null, new Rect(uniform_border_thickness, uniform_border_thickness, actual_width - 2 * uniform_border_thickness, actual_height - 2 * uniform_border_thickness), background_radius, background_radius);
				}
			} else if (Utility.IsVoid(corner_radius) && border_brush is SolidColorBrush && ((SolidColorBrush)border_brush).Color.A == 0xFF) {
				drawingContext.DrawLine(new Pen(border_brush, border_thickness.Left), new Point(border_thickness.Left / 2, 0), new Point(border_thickness.Left / 2, actual_height));
				drawingContext.DrawLine(new Pen(border_brush, border_thickness.Right), new Point(actual_width - border_thickness.Right / 2, 0), new Point(actual_width - border_thickness.Right / 2, actual_height));
				drawingContext.DrawLine(new Pen(border_brush, border_thickness.Top), new Point(0, border_thickness.Top / 2), new Point(actual_width, border_thickness.Top / 2));
				drawingContext.DrawLine(new Pen(border_brush, border_thickness.Bottom), new Point(0, actual_height - border_thickness.Bottom / 2), new Point(actual_width, actual_height - border_thickness.Bottom / 2));
				if (Background != null)
					drawingContext.DrawRectangle(Background, null, new Rect(border_thickness.Left, border_thickness.Top, actual_width - border_thickness.Left - border_thickness.Right, actual_height - border_thickness.Top - border_thickness.Bottom));
			} else if (Utility.IsVoid(corner_radius)) {
				create_background_shape = delegate(StreamGeometryContext context) {
					context.BeginFigure(new Point(border_thickness.Left, border_thickness.Top), true, true);
					context.LineTo(new Point(actual_width - border_thickness.Right, border_thickness.Top), false, false);
					context.LineTo(new Point(actual_width - border_thickness.Right, actual_height - border_thickness.Bottom), false, false);
					context.LineTo(new Point(border_thickness.Left, actual_height - border_thickness.Bottom), false, false);
					context.LineTo(new Point(border_thickness.Left, border_thickness.Top), false, false);
				};
				if (border_brush != null) {
					geometry = new StreamGeometry();
					using (stream_geometry_context = geometry.Open()) {
						stream_geometry_context.BeginFigure(new Point(0, 0), true, true);
						stream_geometry_context.LineTo(new Point(actual_width, 0), false, false);
						stream_geometry_context.LineTo(new Point(actual_width, actual_height), false, false);
						stream_geometry_context.LineTo(new Point(0, actual_height), false, false);
						stream_geometry_context.LineTo(new Point(0, 0), false, false);
						create_background_shape(stream_geometry_context);
					}
					geometry.Freeze();
					drawingContext.DrawGeometry(border_brush, null, geometry);
				}
				if (Background != null) {
					geometry = new StreamGeometry();
					using (stream_geometry_context = geometry.Open()) {
						create_background_shape(stream_geometry_context);
					}
					geometry.Freeze();
					drawingContext.DrawGeometry(Background, null, geometry);
				}
			} else {
				//FIXME: This needs to be clipped.
				//FIXME: I am pretty sure this needs more work.
				actual_width = Math.Max(actual_width, border_thickness.Left + border_thickness.Right);
				actual_height = Math.Max(actual_height, border_thickness.Top + border_thickness.Bottom);

				double top_left_radius_x = corner_radius.TopLeft;
				double top_left_radius_y = top_left_radius_x;
				double bottom_left_radius_x = corner_radius.BottomLeft;
				double bottom_left_radius_y = bottom_left_radius_x;
				double top_right_radius_x = corner_radius.TopRight;
				double top_right_radius_y = top_right_radius_x;
				double bottom_right_radius_x = corner_radius.BottomRight;
				double bottom_right_radius_y = bottom_right_radius_x;

				//double inner_top_left_arc_area_x = ZeroIfNegative(top_left_radius_x - border_thickness.Left);
				//double inner_top_left_arc_area_y = ZeroIfNegative(top_left_radius_y - border_thickness.Top);
				//double inner_bottom_left_arc_area_x = ZeroIfNegative(bottom_left_radius_x - border_thickness.Left);
				//double inner_bottom_left_arc_area_y = ZeroIfNegative(bottom_left_radius_y - border_thickness.Bottom);
				//double inner_top_right_arc_area_x = ZeroIfNegative(top_right_radius_x - border_thickness.Right);
				//double inner_top_right_arc_area_y = ZeroIfNegative(top_right_radius_y - border_thickness.Top);
				//double inner_bottom_right_arc_area_x = ZeroIfNegative(bottom_right_radius_x - border_thickness.Right);
				//double inner_bottom_right_arc_area_y = ZeroIfNegative(bottom_right_radius_y - border_thickness.Bottom);

				double inner_width = ZeroIfNegative(actual_width - border_thickness.Left - border_thickness.Right);
				double inner_height = ZeroIfNegative(actual_height - border_thickness.Top - border_thickness.Bottom);

				//Scale(inner_width, ref inner_top_left_arc_area_x, ref inner_top_right_arc_area_x);
				//Scale(inner_width, ref inner_bottom_left_arc_area_x, ref inner_bottom_right_arc_area_x);
				//Scale(inner_height, ref inner_top_left_arc_area_y, ref inner_bottom_left_arc_area_y);
				//Scale(inner_height, ref inner_top_right_arc_area_y, ref inner_bottom_right_arc_area_y);

				//double inner_left_straight_section_lenght = inner_height - inner_top_left_arc_area_y - inner_bottom_left_arc_area_y;
				//double inner_right_straight_section_lenght = inner_height - inner_top_right_arc_area_y - inner_bottom_right_arc_area_y;
				//double inner_top_straight_section_lenght = inner_width - inner_top_left_arc_area_x - inner_top_right_arc_area_x;
				//double inner_bottom_straight_section_lenght = inner_width - inner_bottom_left_arc_area_x - inner_bottom_right_arc_area_x;

				//double top_left_arc_area_x = inner_top_left_arc_area_x + border_thickness.Left;
				//double top_left_arc_area_y = inner_top_left_arc_area_y + border_thickness.Top;
				//double bottom_left_arc_area_x = inner_bottom_left_arc_area_x + border_thickness.Left;
				//double bottom_left_arc_area_y = inner_bottom_left_arc_area_y + border_thickness.Bottom;
				//double top_right_arc_area_x = inner_top_right_arc_area_x + border_thickness.Right;
				//double top_right_arc_area_y = inner_top_right_arc_area_y + border_thickness.Top;
				//double bottom_right_arc_area_x = inner_bottom_right_arc_area_x + border_thickness.Right;
				//double bottom_right_arc_area_y = inner_bottom_right_arc_area_y + border_thickness.Bottom;

				double left_straight_section_lenght = ZeroIfNegative(actual_height - top_left_radius_y - bottom_left_radius_y);
				double right_straight_section_lenght = ZeroIfNegative(actual_height - top_right_radius_y - bottom_right_radius_y);
				double top_straight_section_lenght = ZeroIfNegative(actual_width - top_left_radius_x - top_right_radius_x);
				double bottom_straight_section_lenght = ZeroIfNegative(actual_width - bottom_left_radius_x - bottom_right_radius_x);

				create_background_shape = delegate(StreamGeometryContext context) {
					context.BeginFigure(new Point(top_left_radius_x + border_thickness.Left / 2, border_thickness.Top), true, true);
					double width = top_right_radius_x - border_thickness.Right / 2;
					double height = top_right_radius_y - border_thickness.Top / 2;
					context.LineTo(new Point(actual_width - (width > 0 ? top_right_radius_x + border_thickness.Right / 2 : border_thickness.Right), border_thickness.Top), false, false);
					if (width >= 0 && height >= 0)
						context.ArcTo(new Point(actual_width - border_thickness.Right, top_right_radius_y + border_thickness.Top / 2), new Size(width, height), 0, false, SweepDirection.Clockwise, false, false);
					width = bottom_right_radius_x - border_thickness.Right / 2;
					height = bottom_right_radius_y - border_thickness.Bottom / 2;
					context.LineTo(new Point(actual_width - border_thickness.Right, actual_height - (height > 0 ? bottom_right_radius_y + border_thickness.Bottom / 2 : border_thickness.Bottom)), false, false);
					if (width >= 0 && height >= 0)
						context.ArcTo(new Point(actual_width - bottom_right_radius_x - border_thickness.Right / 2, actual_height - border_thickness.Bottom), new Size(width, height), 0, false, SweepDirection.Clockwise, false, false);
					width = bottom_left_radius_x - border_thickness.Left / 2;
					height = bottom_left_radius_y - border_thickness.Bottom / 2;
					context.LineTo(new Point(width > 0 ? bottom_left_radius_x + border_thickness.Left / 2 : border_thickness.Left, actual_height - border_thickness.Bottom), false, false);
					context.ArcTo(new Point(border_thickness.Left, actual_height - (height > 0 ? bottom_left_radius_y + border_thickness.Bottom / 2 : border_thickness.Bottom)), new Size(width > 0 ? width : 0, height > 0 ? height : 0), 0, false, SweepDirection.Clockwise, false, false);
					width = top_left_radius_x - border_thickness.Left / 2;
					height = top_left_radius_y - border_thickness.Top / 2;
					context.LineTo(new Point(border_thickness.Left, height > 0 ? top_left_radius_y + border_thickness.Top / 2 : border_thickness.Top), false, false);
					context.ArcTo(new Point(top_left_radius_x + border_thickness.Left / 2, border_thickness.Top), new Size(width > 0 ? width : 0, height > 0 ? height : 0), 0, false, SweepDirection.Clockwise, false, false);
				};
				if (border_brush != null) {
					geometry = new StreamGeometry();
					using (stream_geometry_context = geometry.Open()) {
						stream_geometry_context.BeginFigure(new Point(top_left_radius_x + border_thickness.Left / 2, 0), true, true);
						stream_geometry_context.LineTo(new Point(top_left_radius_x + top_straight_section_lenght - border_thickness.Right / 2, 0), false, false);
						stream_geometry_context.ArcTo(new Point(top_left_radius_x + top_straight_section_lenght + top_right_radius_x, top_right_radius_y + border_thickness.Top / 2), new Size(top_right_radius_x + border_thickness.Right / 2, top_right_radius_y + border_thickness.Top / 2), 0, false, SweepDirection.Clockwise, false, false);
						stream_geometry_context.LineTo(new Point(bottom_left_radius_x + bottom_straight_section_lenght + bottom_right_radius_x, top_right_radius_y + right_straight_section_lenght - border_thickness.Bottom / 2), false, false);
						stream_geometry_context.ArcTo(new Point(bottom_left_radius_x + bottom_straight_section_lenght - border_thickness.Right / 2, top_right_radius_y + right_straight_section_lenght + bottom_right_radius_y), new Size(bottom_right_radius_x + border_thickness.Right / 2, bottom_right_radius_y + border_thickness.Bottom / 2), 0, false, SweepDirection.Clockwise, false, false);
						stream_geometry_context.LineTo(new Point(bottom_left_radius_x + border_thickness.Left / 2, top_left_radius_y + left_straight_section_lenght + bottom_left_radius_y), false, false);
						stream_geometry_context.ArcTo(new Point(0, top_left_radius_y + left_straight_section_lenght - border_thickness.Bottom / 2), new Size(bottom_left_radius_x + border_thickness.Left / 2, bottom_left_radius_y + border_thickness.Bottom / 2), 0, false, SweepDirection.Clockwise, false, false);
						stream_geometry_context.LineTo(new Point(0, top_left_radius_y + border_thickness.Top / 2), false, false);
						stream_geometry_context.ArcTo(new Point(top_left_radius_x + border_thickness.Left / 2, 0), new Size(top_left_radius_x + border_thickness.Left / 2, top_left_radius_y + border_thickness.Top / 2), 0, false, SweepDirection.Clockwise, false, false);
						if (inner_width > 0 && inner_height > 0) {
							create_background_shape(stream_geometry_context);
						}
					}
					geometry.Freeze();
					drawingContext.DrawGeometry(border_brush, null, geometry);
				}
				if (Background != null) {
					if (inner_width > 0 && inner_height > 0) {
						geometry = new StreamGeometry();
						using (stream_geometry_context = geometry.Open()) {
							create_background_shape(stream_geometry_context);
						}
						geometry.Freeze();
						drawingContext.DrawGeometry(Background, null, geometry);
					}
				}
			}
		}
		
		delegate void CreateBackgroundShape(StreamGeometryContext context);

		static double ZeroIfNegative(double value) {
			return value < 0 ? 0 : value;
		}

		static void Scale(double availableSize, ref double size1, ref double size2) {
			double scale_ratio = availableSize / (size1 + size2);
			if (scale_ratio < 1) {
				size1 *= scale_ratio;
				size2 *= scale_ratio;
			}
		}
		#endregion
	}
}