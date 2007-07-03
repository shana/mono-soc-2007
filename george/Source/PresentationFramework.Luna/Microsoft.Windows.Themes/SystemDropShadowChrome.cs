using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#if Implementation
namespace Mono.Microsoft.Windows.Themes {
#else
namespace Microsoft.Windows.Themes {
#endif
	public sealed class SystemDropShadowChrome : Decorator {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(SystemDropShadowChrome), new FrameworkPropertyMetadata(Color.FromArgb(0x71, 0, 0, 0)));
		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(SystemDropShadowChrome), new FrameworkPropertyMetadata());
		#endregion
		#endregion

		#region Public Constructors
		public SystemDropShadowChrome() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public Color Color {
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}

		public CornerRadius CornerRadius {
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			const double BorderSize = 5;
			drawingContext.PushTransform(new TranslateTransform(0, 0));
			RadialGradientBrush radial_gradient_brush;
			radial_gradient_brush = new RadialGradientBrush(color, Colors.Transparent);
			radial_gradient_brush.Center = new Point(1, 1);
			radial_gradient_brush.RadiusX = 1;
			radial_gradient_brush.RadiusX = 2;
			drawingContext.DrawRectangle(radial_gradient_brush, null, new Rect(BorderSize, BorderSize, BorderSize, BorderSize));
			drawingContext.DrawRectangle(brush, null, new Rect(2 * BorderSize, BorderSize, ActualHeight - 2 * BorderSize, BorderSize));
			drawingContext.DrawRectangle(brush, null, new Rect(ActualWidth, BorderSize, BorderSize, BorderSize));
			drawingContext.DrawRectangle(brush, null, new Rect(BorderSize, 2 * BorderSize, BorderSize, ActualHeight - 2 * BorderSize));
			drawingContext.DrawRectangle(brush, null, new Rect(ActualWidth, 2 * BorderSize, BorderSize, ActualHeight - 2 * BorderSize));
			drawingContext.DrawRectangle(brush, null, new Rect(BorderSize, ActualHeight, BorderSize, BorderSize));
			drawingContext.DrawRectangle(brush, null, new Rect(2 * BorderSize, ActualHeight, ActualWidth - 2 * BorderSize, BorderSize));
			drawingContext.DrawRectangle(brush, null, new Rect(ActualWidth, ActualHeight, BorderSize, BorderSize));
			drawingContext.DrawRectangle(new SolidColorBrush(Color), null, new Rect(2 * BorderSize, 2 * BorderSize, ActualWidth - 2 * BorderSize, ActualHeight - 2 * BorderSize));
			drawingContext.Pop();
		}
		#endregion

		#region Private Methods
		Brush CreateRadialBrush(double centerX, double centerY, double radiusX, double radiu
		#endregion
	}
}