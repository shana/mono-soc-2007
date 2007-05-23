using Mono.WindowsPresentationFoundation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#if Implementation
using Microsoft.Windows.Themes;
namespace Mono.Microsoft.Windows.Themes {
#else
namespace Microsoft.Windows.Themes {
#endif
	public sealed class BulletChrome : FrameworkElement {
		#region Dependency Property Fields
		static public readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(BulletChrome), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(BulletChrome), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(BulletChrome), new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsRender), ValidateBorderThickness);
		static public readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?), typeof(BulletChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty IsRoundProperty = DependencyProperty.Register("IsRound", typeof(bool), typeof(BulletChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty RenderMouseOverProperty = DependencyProperty.Register("RenderMouseOver", typeof(bool), typeof(BulletChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty RenderPressedProperty = DependencyProperty.Register("RenderPressed", typeof(bool), typeof(BulletChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		#endregion

		static Brush pressed_brush = new LinearGradientBrush(Colors.Gray, Colors.LightGray, 45);
		static Brush mouse_over_brush = new LinearGradientBrush(Colors.Yellow, Colors.Orange, 45);
		static Pen check_box_check_pen = new Pen(Brushes.Green, 2);
		static Brush radio_button_check_brush = new LinearGradientBrush(Colors.LightGreen, Colors.Green, 45);

		#region Public Constructors
		public BulletChrome() {
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
			//FIXME:
			if (IsRound) {
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
				if (Background != null)
					drawingContext.DrawRectangle(Background, null, new Rect(0, 0, ActualWidth, ActualHeight));
				if (RenderPressed)
					drawingContext.DrawRectangle(pressed_brush, null, new Rect(0, 0, ActualWidth, ActualHeight));
				if (BorderBrush != null)
					Utility.DrawBox(drawingContext, ActualWidth, ActualHeight, BorderBrush, BorderThickness);
				if (IsChecked.Value) {
					drawingContext.PushTransform(new TranslateTransform(BorderThickness.Left, BorderThickness.Top));
					drawingContext.PushClip(new RectangleGeometry(new Rect(2, 2, 8, 10)));
					drawingContext.DrawLine(check_box_check_pen, new Point(2, 5), new Point(5, 8));
					drawingContext.DrawLine(check_box_check_pen, new Point(5, 8), new Point(10, 3));
					drawingContext.Pop();
					drawingContext.Pop();
				}
				if (RenderMouseOver && !RenderPressed)
					drawingContext.DrawRectangle(null, new Pen(mouse_over_brush, 2), new Rect(1 + BorderThickness.Left, 1 + BorderThickness.Top, ActualWidth - 2 - BorderThickness.Left - BorderThickness.Right, ActualHeight - 2 - BorderThickness.Top - BorderThickness.Bottom));
				#endregion
			}
		}
		#endregion

		#region Static Private Methods
		static bool ValidateBorderThickness(object value) {
			Thickness thickness = (Thickness)value;
			return thickness.Left >= 0 && thickness.Right >= 0 && thickness.Top >= 0 && thickness.Bottom >= 0;
		}
		#endregion
	}
}