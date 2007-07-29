using System.Collections;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public class Viewbox : Decorator {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register("StretchDirection", typeof(StretchDirection), typeof(Viewbox), new FrameworkPropertyMetadata(StretchDirection.Both, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(Viewbox), new FrameworkPropertyMetadata(Stretch.Uniform, FrameworkPropertyMetadataOptions.AffectsMeasure));
		#endregion
		#endregion

		#region Private Fields
		ContainerVisual container_visual = new ContainerVisual();
		UIElement child;
		#endregion

		#region Public Constructors
		public Viewbox() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public StretchDirection StretchDirection {
			get { return (StretchDirection)GetValue(StretchDirectionProperty); }
			set { SetValue(StretchDirectionProperty, value); }
		}

		public Stretch Stretch {
			get { return (Stretch)GetValue(StretchProperty); }
			set { SetValue(StretchProperty, value); }
		}
		#endregion

		public override UIElement Child {
			get { return child; }
			set {
				if (child != null)
					container_visual.Children.Remove(child);
				child = value;
				container_visual.Children.Add(value);
			}
		}
		#endregion

		#region Protected Properties
		protected override IEnumerator LogicalChildren {
			get {
				return (child == null ? new object[] { } : new object[] { child }).GetEnumerator();
			}
		}

		protected override int VisualChildrenCount {
			get { return 1; }
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size arrangeSize) {
			if (child == null)
				return arrangeSize;
			Size child_desired_size = child.DesiredSize;
			child.Arrange(new Rect(new Point(0, 0), child_desired_size));
			if (child_desired_size.Width == 0 && child_desired_size.Height == 0)
				return child_desired_size;
			double scale_x;
			double scale_y;
			switch (Stretch) {
			case Stretch.None:
				scale_x = scale_y = 1;
				break;
			case Stretch.Fill:
				scale_x = arrangeSize.Width / child_desired_size.Width;
				scale_y = arrangeSize.Height / child_desired_size.Height;
				break;
			case Stretch.Uniform:
				scale_x = scale_y = Math.Min(arrangeSize.Width / child_desired_size.Width, arrangeSize.Height / child_desired_size.Height);
				break;
			default:
				scale_x = scale_y = Math.Max(arrangeSize.Width / child_desired_size.Width, arrangeSize.Height / child_desired_size.Height);
				break;
			}
			switch (StretchDirection) {
			case StretchDirection.DownOnly:
				if (scale_x > 1 || scale_y > 1)
					scale_x = scale_y = 1;
				break;
			case StretchDirection.UpOnly:
				if (scale_x < 1 || scale_y < 1)
					scale_x = scale_y = 1;
				break;
			}
			container_visual.Transform = new ScaleTransform(scale_x, scale_y);
			return arrangeSize;
		}

		protected override Visual GetVisualChild(int index) {
			if (index == 0)
				return container_visual;
			return base.GetVisualChild(-1);
		}

		protected override Size MeasureOverride(Size constraint) {
			if (child == null)
				return new Size(0, 0);
			child.Measure(constraint);
			Size child_desired_size = child.DesiredSize;
			if (child_desired_size.Width == 0 && child_desired_size.Height == 0)
				return child_desired_size;
			return new Size(double.IsPositiveInfinity(constraint.Width) ? child.DesiredSize.Width : constraint.Width, double.IsPositiveInfinity(constraint.Height) ? child.DesiredSize.Height : constraint.Height);
		}
		#endregion
	}
}