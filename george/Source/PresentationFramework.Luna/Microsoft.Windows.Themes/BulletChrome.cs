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

		static Brush pressed_brush = new LinearGradientBrush(Colors.Gray, Colors.LightGray, 45);
		static Brush mouse_over_brush = new LinearGradientBrush(Colors.Yellow, Colors.Orange, 45);
		static Pen check_box_check_pen = new Pen(Brushes.Green, 2);
		static Brush radio_button_check_brush = new LinearGradientBrush(Colors.LightGreen, Colors.Green, 45);

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
			if (IsRound) {
				//FIXME:
				#region Render radio button bullet
				double radius_x = ActualWidth / 2 - .5;
				double radius_y = ActualHeight / 2 - .5;
				Point center = new Point(radius_x + .5, radius_y + .5);
				if (Background != null)
					drawingContext.DrawEllipse(Background, null, center, radius_x - 1, radius_y - 1);
				if (RenderPressed)
					drawingContext.DrawEllipse(pressed_brush, null, center, radius_x - 1, radius_y - 1);
				if (BorderBrush != null)
					drawingContext.DrawEllipse(null, new Pen(BorderBrush, 1), center, radius_x, radius_y);
				if (IsChecked.Value)
					drawingContext.DrawEllipse(radio_button_check_brush, null, center, radius_x - 3.5, radius_y - 3.5);
				if (RenderMouseOver && !RenderPressed)
					drawingContext.DrawEllipse(null, new Pen(mouse_over_brush, 2), center, radius_x - 2, radius_y - 2);
				#endregion
			} else {
				#region Render check box bullet
				Thickness border_thickness = BorderThickness;
				bool render_pressed = RenderPressed;
				Brush background = Background;
				double width;
				double height;
				bool fill_displayed;
				if (render_pressed || background != null) {
					width = ActualWidth - border_thickness.Left - border_thickness.Right;
					height = ActualHeight - border_thickness.Top - border_thickness.Bottom;
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
						if ((width = ActualWidth - border_thickness.Left - border_thickness.Right - 2) >= 0 && (height = ActualHeight - border_thickness.Top - border_thickness.Bottom - 2) >= 0)
							drawingContext.DrawRectangle(null, new Pen(new LinearGradientBrush(new GradientStopCollection(new GradientStop[] {
								new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xF0, 0xCF), 0),
								new GradientStop(Color.FromArgb(0xFF, 0xF8, 0xB3, 0x30), 1)
							}), new Point(0, 0), new Point(1, 1)), 2), new Rect(border_thickness.Left + 1, border_thickness.Top + 1, width, height));
				bool has_uniform_border_thickness = border_thickness.Bottom == border_thickness.Left && border_thickness.Bottom == BorderThickness.Right && border_thickness.Bottom == BorderThickness.Top;
				if (fill_displayed)
					if (IsChecked.Value) {
						if (!has_uniform_border_thickness)
							drawingContext.PushTransform(new TranslateTransform(border_thickness.Left - 1, border_thickness.Top - 1));
						StreamGeometry stream_geometry = new StreamGeometry();
						StreamGeometryContext context = stream_geometry.Open();
						context.BeginFigure(new Point(3, 5), true, true);
						context.LineTo(new Point(3, 7.8), false, false);
						context.LineTo(new Point(5.5, 10.4), false, false);
						context.LineTo(new Point(10.1, 5.8), false, false);
						context.LineTo(new Point(10.1, 3), false, false);
						context.LineTo(new Point(5.5, 7.6), false, false);
						context.Close();
						drawingContext.DrawGeometry(new SolidColorBrush(render_pressed ? Color.FromArgb(0xFF, 0x1A, 0x7E, 0x18) : Color.FromArgb(0xFF, 0x21, 0xA1, 0x21)), null, stream_geometry);
						if (!has_uniform_border_thickness)
							drawingContext.Pop();
					}
				if (BorderBrush != null) {
					if (has_uniform_border_thickness) {
						double border_thickness_size = border_thickness.Bottom;
						if ((width = ActualWidth - 2 * border_thickness_size + 1) >= 0 && (height = ActualHeight - 2 * border_thickness_size + 1) >= 0)
							drawingContext.DrawRectangle(null, new Pen(BorderBrush, border_thickness.Bottom), new Rect(border_thickness_size - 0.5, border_thickness_size - 0.5, width, height));
					} else {
						drawingContext.DrawGeometry(BorderBrush, null, new PathGeometry(new PathFigure[] {
							new PathFigure(new Point(0, 0), new PathSegment[] {
								new LineSegment(new Point(ActualWidth, 0), false),
								new LineSegment(new Point(ActualWidth, ActualHeight), false),
								new LineSegment(new Point(0, ActualHeight), false)
							}, true),
							new PathFigure(new Point(border_thickness.Left, border_thickness.Top), new PathSegment[] {
								new LineSegment(new Point((width = ActualWidth - border_thickness.Right) >= 0 ? width : border_thickness.Left, border_thickness.Top), false),
								new LineSegment(new Point((width = ActualWidth - border_thickness.Right) >= 0 ? width : border_thickness.Left, (height = ActualHeight - border_thickness.Bottom) >= 0 ? height : border_thickness.Top), false),
								new LineSegment(new Point(border_thickness.Left, (height = ActualHeight - border_thickness.Bottom) >= 0 ? height : border_thickness.Top), false)
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