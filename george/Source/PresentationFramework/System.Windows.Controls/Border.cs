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
		#region Dependency Property Fields
		public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(Border), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
		public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(Border), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
		public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(Border), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Border), new FrameworkPropertyMetadata(new CornerRadius(), FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(Border), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure));
		#endregion

		#region Public Constructors
		public Border() {
			//WDTDH
		}
		#endregion

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