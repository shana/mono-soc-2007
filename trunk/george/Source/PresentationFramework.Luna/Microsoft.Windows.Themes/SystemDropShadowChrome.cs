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
			// I think the Microsoft implementation does a translate transform here (possibly 5, 5).
			drawingContext.PushTransform(Transform.Identity);
			drawingContext.DrawRectangle(CreateRadialBrush(1, 1, 1, 1), null, new Rect(BorderSize, BorderSize, BorderSize, BorderSize));
			drawingContext.DrawRectangle(CreateLinearBrush(0, 1, 0, 0), null, new Rect(2 * BorderSize, BorderSize, ActualHeight - 2 * BorderSize, BorderSize));
			drawingContext.DrawRectangle(CreateRadialBrush(0, 1, 1, 1), null, new Rect(ActualWidth, BorderSize, BorderSize, BorderSize));
			drawingContext.DrawRectangle(CreateLinearBrush(1, 0, 0, 0), null, new Rect(BorderSize, 2 * BorderSize, BorderSize, ActualHeight - 2 * BorderSize));
			drawingContext.DrawRectangle(CreateLinearBrush(0, 0, 1, 0), null, new Rect(ActualWidth, 2 * BorderSize, BorderSize, ActualHeight - 2 * BorderSize));
			drawingContext.DrawRectangle(CreateRadialBrush(1, 0, 1, 1), null, new Rect(BorderSize, ActualHeight, BorderSize, BorderSize));
			drawingContext.DrawRectangle(CreateLinearBrush(0, 0, 0, 1), null, new Rect(2 * BorderSize, ActualHeight, ActualWidth - 2 * BorderSize, BorderSize));
			drawingContext.DrawRectangle(CreateRadialBrush(0, 0, 1, 1), null, new Rect(ActualWidth, ActualHeight, BorderSize, BorderSize));
			drawingContext.DrawRectangle(new SolidColorBrush(Color), null, new Rect(2 * BorderSize, 2 * BorderSize, ActualWidth - 2 * BorderSize, ActualHeight - 2 * BorderSize));
			drawingContext.Pop();
		}
		#endregion

		#region Private Methods
		Brush CreateRadialBrush(double centerX, double centerY, double radiusX, double radiusY) {
			RadialGradientBrush result = new RadialGradientBrush(Color, Colors.Transparent);
			result.Center = new Point(centerX, centerY);
			result.GradientOrigin = result.Center;
			result.RadiusX = radiusX;
			result.RadiusY = radiusY;
			return result;
		}

		Brush CreateLinearBrush(double startPointX, double startPointY, double endPointX, double endPointY) {
			LinearGradientBrush result = new LinearGradientBrush(Color, Colors.Transparent, 0);
			result.StartPoint = new Point(startPointX, startPointY);
			result.EndPoint = new Point(endPointX, endPointY);
			return result;
		}
		#endregion
	}
}