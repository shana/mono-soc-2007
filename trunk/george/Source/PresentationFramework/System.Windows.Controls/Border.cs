using Mono.WindowsPresentationFoundation;
using System.Windows.Media;
#if Implementation
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
			} else {
				StreamGeometry geometry;
				StreamGeometryContext context;
				if (border_brush != null) {
					geometry = new StreamGeometry();
					using (context = geometry.Open()) {
						context.BeginFigure(new Point(0, 0), true, true);
						context.LineTo(new Point(actual_width, 0), false, false);
						context.LineTo(new Point(actual_width, actual_height), false, false);
						context.LineTo(new Point(0, actual_height), false, false);
						context.LineTo(new Point(0, 0), false, true);
						context.BeginFigure(new Point(border_thickness.Left, border_thickness.Top), true, false);
						context.LineTo(new Point(actual_width - border_thickness.Right, border_thickness.Top), false, false);
						context.LineTo(new Point(actual_width - border_thickness.Right, ActualHeight - border_thickness.Bottom), false, false);
						context.LineTo(new Point(border_thickness.Left, ActualHeight - border_thickness.Bottom), false, false);
						context.LineTo(new Point(border_thickness.Left, border_thickness.Top), false, false);
					}
					geometry.Freeze();
					drawingContext.DrawGeometry(border_brush, null, geometry);
				}
				if (Background != null) {
					geometry = new StreamGeometry();
					using (context = geometry.Open()) {
						context.BeginFigure(new Point(border_thickness.Left, border_thickness.Top), true, false);
						context.LineTo(new Point(actual_width - border_thickness.Right, border_thickness.Top), false, false);
						context.LineTo(new Point(actual_width - border_thickness.Right, ActualHeight - border_thickness.Bottom), false, false);
						context.LineTo(new Point(border_thickness.Left, ActualHeight - border_thickness.Bottom), false, false);
						context.LineTo(new Point(border_thickness.Left, border_thickness.Top), false, false);
					}
					geometry.Freeze();
					drawingContext.DrawGeometry(Background, null, geometry);
				}
			}

			return;
			//HACK
			if (Utility.IsVoid(CornerRadius)) {
				if (Background != null)
					drawingContext.DrawRectangle(Background, null, Utility.GetBounds(this));
				if (BorderBrush != null)
					Utility.DrawBox(drawingContext, ActualWidth, ActualHeight, BorderBrush, BorderThickness);
			} else {
				if (Background == null) {
					if (BorderBrush != null)
						Utility.DrawRoundBox(drawingContext, ActualWidth, ActualHeight, BorderBrush, BorderThickness, CornerRadius);
				} else {
					//FIXME: Respect BorderThickness.
					double top_left_radius_x = CornerRadius.TopLeft;
					double top_left_radius_y = top_left_radius_x;
					double bottom_left_radius_x = CornerRadius.BottomLeft;
					double bottom_left_radius_y = bottom_left_radius_x;
					double top_right_radius_x = CornerRadius.TopRight;
					double top_right_radius_y = top_right_radius_x;
					double bottom_right_radius_x = CornerRadius.BottomRight;
					double bottom_right_radius_y = bottom_right_radius_x;
					double scale_factor = ActualHeight / (CornerRadius.TopLeft + CornerRadius.BottomLeft);
					if (scale_factor < 1) {
						top_left_radius_y *= scale_factor;
						bottom_left_radius_y *= scale_factor;
					}
					scale_factor = ActualHeight / (CornerRadius.TopRight + CornerRadius.BottomRight);
					if (scale_factor < 1) {
						top_right_radius_y *= scale_factor;
						bottom_right_radius_y *= scale_factor;
					}
					scale_factor = ActualWidth / (CornerRadius.TopLeft + CornerRadius.TopRight);
					if (scale_factor < 1) {
						top_left_radius_x *= scale_factor;
						top_right_radius_x *= scale_factor;
					}
					scale_factor = ActualWidth / (CornerRadius.BottomLeft + CornerRadius.BottomRight);
					if (scale_factor < 1) {
						bottom_left_radius_x *= scale_factor;
						bottom_right_radius_x *= scale_factor;
					}
					double top_left_x = top_left_radius_x;
					double top_left_y = BorderThickness.Top / 2;
					double top_x = ActualWidth - top_right_radius_x;
					double top_y = top_left_y;
					double top_right_x = ActualWidth - BorderThickness.Right / 2;
					double top_right_y = top_right_radius_y;
					double right_x = top_right_x;
					double right_y = ActualHeight - bottom_right_radius_y;
					double bottom_right_x = ActualWidth - bottom_right_radius_x;
					double bottom_right_y = ActualHeight - BorderThickness.Bottom / 2;
					double bottom_x = bottom_left_radius_x;
					double bottom_y = bottom_right_y;
					double bottom_left_x = BorderThickness.Left / 2;
					double bottom_left_y = ActualHeight - bottom_left_radius_y;
					double left_x = bottom_left_x;
					double left_y = top_left_radius_y;
					StreamGeometry geometry = new StreamGeometry();
					using (StreamGeometryContext context = geometry.Open()) {
						context.BeginFigure(new Point(top_left_x, top_left_y), true, true);
						context.LineTo(new Point(top_x, top_y), true, true);
						context.ArcTo(new Point(top_right_x, top_right_y), new Size(top_right_radius_x, top_right_radius_y), 0, false, SweepDirection.Clockwise, true, true);
						context.LineTo(new Point(right_x, right_y), true, true);
						context.ArcTo(new Point(bottom_right_x, bottom_right_y), new Size(bottom_right_radius_x, bottom_right_radius_y), 0, false, SweepDirection.Clockwise, true, true);
						context.LineTo(new Point(bottom_x, bottom_y), true, true);
						context.ArcTo(new Point(bottom_left_x, bottom_left_y), new Size(bottom_left_radius_x, bottom_left_radius_y), 0, false, SweepDirection.Clockwise, true, true);
						context.LineTo(new Point(left_x, left_y), true, true);
						context.ArcTo(new Point(top_left_x, top_left_y), new Size(top_left_radius_x, top_left_radius_y), 0, false, SweepDirection.Clockwise, true, true);
					}
					geometry.Freeze();
					drawingContext.DrawGeometry(Background, new Pen(BorderBrush, BorderThickness.Left), geometry);
				}
			}
		}
		#endregion
	}
}