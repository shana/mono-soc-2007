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
		public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(SystemDropShadowChrome), new FrameworkPropertyMetadata());
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
			drawingContext.PushTransform(new TranslateTransform(5, 5));
			//FIXME
			drawingContext.DrawRoundedRectangle(new SolidColorBrush(Color), null, new Rect(0, 0, ActualWidth, ActualHeight), CornerRadius.BottomLeft, CornerRadius.TopLeft);
		}
		#endregion
	}
}