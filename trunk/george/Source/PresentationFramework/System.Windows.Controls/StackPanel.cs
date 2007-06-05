#if Implementation
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
			return base.ArrangeOverride(finalSize);
		}

		protected override Size MeasureOverride(Size availableSize) {
			return base.MeasureOverride(availableSize);
		}
		#endregion

		#region IScrollInfo implementation
		public bool CanHorizontallyScroll {
			get {
				throw new global::System.Exception("The method or operation is not implemented.");
			}
			set {
				throw new global::System.Exception("The method or operation is not implemented.");
			}
		}

		public bool CanVerticallyScroll {
			get {
				throw new global::System.Exception("The method or operation is not implemented.");
			}
			set {
				throw new global::System.Exception("The method or operation is not implemented.");
			}
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
			get {
				throw new global::System.Exception("The method or operation is not implemented.");
			}
			set {
				throw new global::System.Exception("The method or operation is not implemented.");
			}
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