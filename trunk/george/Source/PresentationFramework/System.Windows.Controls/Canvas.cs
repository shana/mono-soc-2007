using System.Collections;
using System.ComponentModel;
using System.Windows.Media;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public class Canvas : Panel {
		#region Public Fields
		#region Dependency Properties
		#region Attached Properties
		public static readonly DependencyProperty BottomProperty = DependencyProperty.RegisterAttached("Bottom", typeof(double), typeof(Canvas), new FrameworkPropertyMetadata(double.NaN));
		public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached("Left", typeof(double), typeof(Canvas), new FrameworkPropertyMetadata(double.NaN));
		public static readonly DependencyProperty RightProperty = DependencyProperty.RegisterAttached("Right", typeof(double), typeof(Canvas), new FrameworkPropertyMetadata(double.NaN));
		public static readonly DependencyProperty TopProperty = DependencyProperty.RegisterAttached("Top", typeof(double), typeof(Canvas), new FrameworkPropertyMetadata(double.NaN));
		#endregion
		#endregion
		#endregion

		#region Public Constructors
		public Canvas() {
		}
		#endregion

		#region Public Methods
		[TypeConverter("System.Windows.LengthConverter, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, Custom=null")]
		[AttachedPropertyBrowsableForChildren]
		public static double GetBottom(UIElement element) {
			return (double)element.GetValue(BottomProperty);
		}

		[TypeConverter("System.Windows.LengthConverter, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, Custom=null")]
		[AttachedPropertyBrowsableForChildren]
		public static double GetLeft(UIElement element) {
			return (double)element.GetValue(LeftProperty);
		}

		[TypeConverter("System.Windows.LengthConverter, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, Custom=null")]
		[AttachedPropertyBrowsableForChildren]
		public static double GetRight(UIElement element) {
			return (double)element.GetValue(RightProperty);
		}

		[TypeConverter("System.Windows.LengthConverter, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, Custom=null")]
		[AttachedPropertyBrowsableForChildren]
		public static double GetTop(UIElement element) {
			return (double)element.GetValue(TopProperty);
		}

		public static void SetBottom(UIElement element, double length) {
			element.SetValue(BottomProperty, length);
		}

		public static void SetLeft(UIElement element, double length) {
			element.SetValue(LeftProperty, length);
		}

		public static void SetRight(UIElement element, double length) {
			element.SetValue(RightProperty, length);
		}

		public static void SetTop(UIElement element, double length) {
			element.SetValue(TopProperty, length);
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize) {
			//FIXME: Implement Z order.
			foreach (UIElement element in Children) {
				Point position = new Point();
				Size size = element.DesiredSize;
				double value;
				value = GetLeft(element);
				if (double.IsNaN(value)) {
					value = GetRight(element);
					if (!double.IsNaN(value))
						position.X = value - size.Width;
				} else
					position.X = value;
				value = GetTop(element);
				if (double.IsNaN(value)) {
					value = GetBottom(element);
					if (!double.IsNaN(value))
						position.Y = value - size.Height;
				} else
					position.Y = value;
				element.Arrange(new Rect(position, size));
			}
			return base.ArrangeOverride(finalSize);
		}

		protected override Geometry GetLayoutClip(Size layoutSlotSize) {
			return base.GetLayoutClip(layoutSlotSize);
		}

		protected override Size MeasureOverride(Size availableSize) {
			foreach (UIElement element in Children)
				element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			return base.MeasureOverride(availableSize);
		}
		#endregion
	}
}