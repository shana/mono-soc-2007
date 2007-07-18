using Mono.WindowsPresentationFoundation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#if Implementation
namespace Mono.Microsoft.Windows.Themes {
#else
namespace Microsoft.Windows.Themes {
#endif
	public sealed class BulletChrome : FrameworkElement {
		#region Public Fields
		#region Dependency Properties
		static public readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(BulletChrome), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(BulletChrome), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(BulletChrome), new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsRender), ValidateBorderThickness);
		static public readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?), typeof(BulletChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty IsRoundProperty = DependencyProperty.Register("IsRound", typeof(bool), typeof(BulletChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty RenderMouseOverProperty = DependencyProperty.Register("RenderMouseOver", typeof(bool), typeof(BulletChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty RenderPressedProperty = DependencyProperty.Register("RenderPressed", typeof(bool), typeof(BulletChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		#endregion
		#endregion

		#region Public Constructors
		public BulletChrome() {
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

		public bool? IsChecked {
			get { return (bool?)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}

		public bool IsRound {
			get { return (bool)GetValue(IsRoundProperty); }
			set { SetValue(IsRoundProperty, value); }
		}

		public bool RenderMouseOver {
			get { return (bool)GetValue(RenderMouseOverProperty); }
			set { SetValue(RenderMouseOverProperty, value); }
		}

		public bool RenderPressed {
			get { return (bool)GetValue(RenderPressedProperty); }
			set { SetValue(RenderPressedProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override Size MeasureOverride(Size availableSize) {
			if (IsRound) {
				const int RoundSize = 13;
				return Utility.GetSize(RoundSize, availableSize);
			} else {
				const int SquareSize = 11;
				return Utility.GetSize(SquareSize, BorderThickness, availableSize);
			}
		}

		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			double actual_width = ActualWidth;
			double actual_height = ActualHeight;
			if (IsRound) {
				#region Render radio button bullet
				double radius_x = actual_width / 2;
				double radius_y = actual_height / 2;
				Point center = new Point(radius_x, radius_y);
				const double BackgroundPadding = 1;
				if (RenderPressed)
					drawingContext.DrawEllipse(new LinearGradientBrush(new GradientStopCollection(new GradientStop[] {
						new GradientStop(Color.FromArgb(0xFF, 0xB2, 0xB2, 0xA9), 0),
						new GradientStop(Color.FromArgb(0xFF, 0xEB, 0xEA, 0xDA), 1)
					}), new Point(0, 0), new Point(1, 1)), null, center, radius_x - BackgroundPadding, radius_y - BackgroundPadding);
				else
					if (Background != null)
						drawingContext.DrawEllipse(Background, null, center, radius_x - BackgroundPadding, radius_y - BackgroundPadding);
				if (RenderMouseOver && !RenderPressed) {
					const double MouseOverPadding = 2;
					drawingContext.DrawEllipse(null, new Pen(new LinearGradientBrush(new GradientStopCollection(new GradientStop[] {
						new GradientStop(Color.FromArgb(0xFF, 0xFE, 0xDF, 0x9C), 0),
						new GradientStop(Color.FromArgb(0xFF, 0xF9, 0xBB, 0x43), 1)
					}), new Point(0, 0), new Point(1, 1)), 2), center, radius_x - MouseOverPadding, radius_y - MouseOverPadding);
				}
				if (IsChecked.Value) {
					const double CheckPadding = 4;
					drawingContext.DrawEllipse(new LinearGradientBrush(new GradientStopCollection(new GradientStop[] {
						new GradientStop(Color.FromArgb(0xFF, 0x60, 0xCF, 0x5D), 0),
						new GradientStop(Color.FromArgb(0xFF, 0xAC, 0xEF, 0xAA), 0.302469134),
						new GradientStop(Color.FromArgb(0xFF, 0x13, 0x92, 0x10), 1)
					}), new Point(0, 0), new Point(1, 1)), null, center, radius_x - CheckPadding, radius_y - CheckPadding);
				}
				if (BorderBrush != null) {
					const double BorderPadding = 0.5;
					drawingContext.DrawEllipse(null, new Pen(BorderBrush, 1), center, radius_x - BorderPadding, radius_y - BorderPadding);
				}
				#endregion
			} else {
				#region Render check box bullet
				Thickness border_thickness = BorderThickness;
				const double MinimumSize = 2;
				//FIXME: This is most likely wrong.
				const double BorderSizeWhenBehaviorChanges = 100;
				const double MinimumSizeWhenBehaviorChanges = 3;

				double minimum_size_computed_using_border = border_thickness.Left + border_thickness.Right + 1;
				double minimum_size;
				if (minimum_size_computed_using_border >= BorderSizeWhenBehaviorChanges)
					minimum_size = MinimumSizeWhenBehaviorChanges;
				else
					minimum_size = Math.Max(MinimumSize, minimum_size_computed_using_border);
				if (actual_width <= minimum_size)
					return;

				minimum_size_computed_using_border = border_thickness.Top + border_thickness.Bottom + 1;
				if (minimum_size_computed_using_border >= BorderSizeWhenBehaviorChanges)
					minimum_size = MinimumSizeWhenBehaviorChanges;
				else
					minimum_size = Math.Max(MinimumSize, minimum_size_computed_using_border);
				if (actual_width <= minimum_size)
					return;

				bool render_pressed = RenderPressed;
				Brush background = Background;
				double width;
				double height;
				bool fill_displayed;
				if (render_pressed || background != null) {
					width = actual_width - border_thickness.Left - border_thickness.Right;
					height = actual_height - border_thickness.Top - border_thickness.Bottom;
					if (fill_displayed = (width >= 0 && height >= 0)) {
						Brush fill_brush;
						if (render_pressed)
							fill_brush = new LinearGradientBrush(new GradientStopCollection(new GradientStop[] {
								new GradientStop(Color.FromArgb(0xFF, 0xB2, 0xB2, 0xA9), 0),
								new GradientStop(Color.FromArgb(0xFF, 0xEB, 0xEA, 0xDA), 1)
							}), new Point(0, 0), new Point(1, 1));
						else
							fill_brush = background;
						drawingContext.DrawRectangle(fill_brush, null, new Rect(border_thickness.Left, border_thickness.Top, width, height));
					}
				} else
					fill_displayed = false;
				if (fill_displayed)
					if (RenderMouseOver && !render_pressed)
						if ((width = actual_width - border_thickness.Left - border_thickness.Right - 2) >= 0 && (height = actual_height - border_thickness.Top - border_thickness.Bottom - 2) >= 0)
							drawingContext.DrawRectangle(null, new Pen(new LinearGradientBrush(new GradientStopCollection(new GradientStop[] {
								new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xF0, 0xCF), 0),
								new GradientStop(Color.FromArgb(0xFF, 0xF8, 0xB3, 0x30), 1)
							}), new Point(0, 0), new Point(1, 1)), 2), new Rect(border_thickness.Left + 1, border_thickness.Top + 1, width, height));
				bool has_uniform_border_thickness = border_thickness.Bottom == border_thickness.Left && border_thickness.Bottom == BorderThickness.Right && border_thickness.Bottom == BorderThickness.Top;
				if (fill_displayed)
					if (IsChecked.Value) {
						if (!has_uniform_border_thickness)
							drawingContext.PushTransform(new TranslateTransform(border_thickness.Left - 1, border_thickness.Top - 1));
						StreamGeometry geometry = new StreamGeometry();
						using (StreamGeometryContext context = geometry.Open()) {
							context.BeginFigure(new Point(3, 5), true, true);
							context.LineTo(new Point(3, 7.8), false, false);
							context.LineTo(new Point(5.5, 10.4), false, false);
							context.LineTo(new Point(10.1, 5.8), false, false);
							context.LineTo(new Point(10.1, 3), false, false);
							context.LineTo(new Point(5.5, 7.6), false, false);
						}
						geometry.Freeze();
						drawingContext.DrawGeometry(new SolidColorBrush(render_pressed ? Color.FromArgb(0xFF, 0x1A, 0x7E, 0x18) : Color.FromArgb(0xFF, 0x21, 0xA1, 0x21)), null, geometry);
						if (!has_uniform_border_thickness)
							drawingContext.Pop();
					}
				if (BorderBrush != null) {
					if (has_uniform_border_thickness) {
						double border_thickness_size = border_thickness.Bottom;
						if ((width = actual_width - 2 * border_thickness_size + 1) >= 0 && (height = actual_height - 2 * border_thickness_size + 1) >= 0)
							drawingContext.DrawRectangle(null, new Pen(BorderBrush, border_thickness.Bottom), new Rect(border_thickness_size - 0.5, border_thickness_size - 0.5, width, height));
					} else {
						drawingContext.DrawGeometry(BorderBrush, null, new PathGeometry(new PathFigure[] {
							new PathFigure(new Point(0, 0), new PathSegment[] {
								new LineSegment(new Point(actual_width, 0), false),
								new LineSegment(new Point(actual_width, actual_height), false),
								new LineSegment(new Point(0, actual_height), false)
							}, true),
							new PathFigure(new Point(border_thickness.Left, border_thickness.Top), new PathSegment[] {
								new LineSegment(new Point((width = actual_width - border_thickness.Right) >= 0 ? width : border_thickness.Left, border_thickness.Top), false),
								new LineSegment(new Point((width = actual_width - border_thickness.Right) >= 0 ? width : border_thickness.Left, (height = actual_height - border_thickness.Bottom) >= 0 ? height : border_thickness.Top), false),
								new LineSegment(new Point(border_thickness.Left, (height = actual_height - border_thickness.Bottom) >= 0 ? height : border_thickness.Top), false)
							}, true)
						}));
					}
				}
				#endregion
			}
		}
		#endregion

		#region Private Methods
		static bool ValidateBorderThickness(object value) {
			Thickness thickness = (Thickness)value;
			return thickness.Left >= 0 && thickness.Right >= 0 && thickness.Top >= 0 && thickness.Bottom >= 0;
		}
		#endregion
	}
}