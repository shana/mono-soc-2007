using Mono.WindowsPresentationFoundation;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public class StackPanel : Panel, IScrollInfo {
		#region Public Fields
		#region Dependency properties
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StackPanel), new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));
		#endregion
		#endregion

		#region Private Fields
		ScrollViewer scroll_owner;
		#endregion

		#region Static Constructor
		static StackPanel() {
			//FIXME: I should not do this.
			HorizontalAlignmentProperty.OverrideMetadata(typeof(StackPanel), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsMeasure));
			VerticalAlignmentProperty.OverrideMetadata(typeof(StackPanel), new FrameworkPropertyMetadata(VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsMeasure));
		}
		#endregion

		#region Public Constructors
		public StackPanel() {
		}
		#endregion

		#region Public Properties
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		#endregion

		#region Protected Properties
		protected internal override bool HasLogicalOrientation {
			get { return true; }
		}

		protected internal override Orientation LogicalOrientation {
			get { return Orientation; }
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize) {
			double used_size = 0;
			bool horizontal = Orientation == Orientation.Horizontal;
			foreach (UIElement child in Children) {
				child.Arrange(horizontal ? new Rect(used_size, 0, Math.Min(child.DesiredSize.Width, finalSize.Width - used_size), DesiredSize.Height) : new Rect(0, used_size, DesiredSize.Width, Math.Min(child.DesiredSize.Height, finalSize.Height - used_size)));
				used_size += horizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
			}
			return finalSize;
		}

		protected override Size MeasureOverride(Size availableSize) {
			double size_in_orientation_direction = 0;
			double size_in_other_direction = 0;
			bool horizontal = Orientation == Orientation.Horizontal;
			foreach (UIElement child in Children) {
				child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				size_in_orientation_direction += horizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
				size_in_other_direction = Math.Max(size_in_other_direction, horizontal ? child.DesiredSize.Height : child.DesiredSize.Width);
			}
			Size result = horizontal ? new Size(size_in_orientation_direction, size_in_other_direction) : new Size(size_in_other_direction, size_in_orientation_direction);
			if (!(double.IsPositiveInfinity(availableSize.Width) || double.IsPositiveInfinity(availableSize.Height))) {
				if (HorizontalAlignment == HorizontalAlignment.Stretch)
					result.Width = availableSize.Width;
				else
					if (result.Width > availableSize.Width)
						result.Width = availableSize.Width;
				if (VerticalAlignment == VerticalAlignment.Stretch)
					result.Height = availableSize.Height;
				else
					if (result.Height > availableSize.Height)
						result.Height = availableSize.Height;
			}
			return result;
		}
		#endregion

		#region IScrollInfo implementation
		public bool CanHorizontallyScroll {
			get {
				throw new global::System.Exception("The method or operation is not implemented.");
			}
			set { }
		}

		public bool CanVerticallyScroll {
			get {
				throw new global::System.Exception("The method or operation is not implemented.");
			}
			set { }
		}

		public double ExtentHeight {
			get { throw new global::System.Exception("The method or operation is not implemented."); }
		}

		public double ExtentWidth {
			get { throw new global::System.Exception("The method or operation is not implemented."); }
		}

		public double HorizontalOffset {
			get { throw new global::System.Exception("The method or operation is not implemented."); }
		}

		public void LineDown() {
			throw new global::System.Exception("The method or operation is not implemented.");
		}

		public void LineLeft() {
			throw new global::System.Exception("The method or operation is not implemented.");
		}

		public void LineRight() {
			throw new global::System.Exception("The method or operation is not implemented.");
		}

		public void LineUp() {
			throw new global::System.Exception("The method or operation is not implemented.");
		}

		public Rect MakeVisible(global::System.Windows.Media.Visual visual, Rect rectangle) {
			throw new global::System.Exception("The method or operation is not implemented.");
		}

		public void MouseWheelDown() {
			throw new global::System.Exception("The method or operation is not implemented.");
		}

		public void MouseWheelLeft() {
			throw new global::System.Exception("The method or operation is not implemented.");
		}

		public void MouseWheelRight() {
			throw new global::System.Exception("The method or operation is not implemented.");
		}

		public void MouseWheelUp() {
			throw new global::System.Exception("The method or operation is not implemented.");
		}

		public void PageDown() {
			throw new global::System.Exception("The method or operation is not implemented.");
		}

		public void PageLeft() {
			throw new global::System.Exception("The method or operation is not implemented.");
		}

		public void PageRight() {
			throw new global::System.Exception("The method or operation is not implemented.");
		}

		public void PageUp() {
			throw new global::System.Exception("The method or operation is not implemented.");
		}

		public ScrollViewer ScrollOwner {
			get { return scroll_owner; }
			set { scroll_owner = value; }
		}

		public void SetHorizontalOffset(double offset) {
			throw new global::System.Exception("The method or operation is not implemented.");
		}

		public void SetVerticalOffset(double offset) {
			throw new global::System.Exception("The method or operation is not implemented.");
		}

		public double VerticalOffset {
			get { throw new global::System.Exception("The method or operation is not implemented."); }
		}

		public double ViewportHeight {
			get { throw new global::System.Exception("The method or operation is not implemented."); }
		}

		public double ViewportWidth {
			get { throw new global::System.Exception("The method or operation is not implemented."); }
		}
		#endregion
	}
}