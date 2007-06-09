using Mono.WindowsPresentationFoundation;
using System.Collections.Generic;
using System.Windows.Media;
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
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StackPanel), new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			StackPanel i = (StackPanel)d;
			i.extent_size = new Size();
			i.viewport_size = new Size();
		}));
		#endregion
		#endregion

		#region Private Fields
		ScrollViewer scroll_owner;
		Size extent_size;
		Size viewport_size;
		double[] physical_child_offsets;
		int logical_offset_width;
		int logical_offset_height;
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
			double available_size = horizontal ? finalSize.Width : finalSize.Height;
			int adjusted_logical_offset;
			if (ContentScroll)
				adjusted_logical_offset = GetAdjustedLogicalOffset(horizontal ? logical_offset_width : logical_offset_height, horizontal ? ExtentWidth : ExtentHeight, horizontal ? ViewportWidth : ViewportHeight);
			else
				adjusted_logical_offset = 0;
			if (horizontal)
				logical_offset_width = adjusted_logical_offset;
			else
				logical_offset_height = adjusted_logical_offset;
			for (int child_index = adjusted_logical_offset; child_index < Children.Count; child_index++) {
				UIElement child = Children[child_index];
				double child_size = horizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
				if (ScrollOwner == null)
					child_size = Math.Min(child_size, available_size - used_size);
				child.Arrange(horizontal ? new Rect(used_size, 0, child_size, finalSize.Height) : new Rect(0, used_size, finalSize.Width, child_size));
				used_size += child_size;
				if (used_size == available_size)
					break;
			}
			return finalSize;
		}

		protected override Size MeasureOverride(Size availableSize) {
			double physical_extent_size_in_orientation_direction = 0;
			double physical_extent_size_in_other_direction = 0;
			bool orientation_is_horizontal = Orientation == Orientation.Horizontal;
			const double Uninitialized = -1;
			double logical_viewport_size_in_orientation_direction = Uninitialized;
			double physical_viewport_size_in_orientation_direction = orientation_is_horizontal ? availableSize.Width : availableSize.Height;
			physical_child_offsets = new double[Children.Count];
			#region Measure children and store extent and offsets size
			for (int child_index = 0; child_index < Children.Count; child_index++) {
				physical_child_offsets[child_index] = physical_extent_size_in_orientation_direction;
				UIElement child = Children[child_index];
				child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				physical_extent_size_in_orientation_direction += orientation_is_horizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
				physical_extent_size_in_other_direction = Math.Max(physical_extent_size_in_other_direction, orientation_is_horizontal ? child.DesiredSize.Height : child.DesiredSize.Width);
				if (logical_viewport_size_in_orientation_direction == Uninitialized && physical_extent_size_in_orientation_direction > physical_viewport_size_in_orientation_direction)
					logical_viewport_size_in_orientation_direction = child_index;
			}
			#endregion
			Size result = orientation_is_horizontal ? new Size(physical_extent_size_in_orientation_direction, physical_extent_size_in_other_direction) : new Size(physical_extent_size_in_other_direction, physical_extent_size_in_orientation_direction);
			#region Compute extent and viewport sizes
			Size new_extent_size = result;
			Size new_viewport_size = availableSize;
			double logical_extent_size_in_orientation_direction = Children.Count;
			if (logical_viewport_size_in_orientation_direction == Uninitialized)
				logical_viewport_size_in_orientation_direction = Children.Count;
			if (orientation_is_horizontal) {
				new_extent_size.Width = logical_extent_size_in_orientation_direction;
				new_viewport_size.Width = logical_viewport_size_in_orientation_direction;
			} else {
				new_extent_size.Height = logical_extent_size_in_orientation_direction;
				new_viewport_size.Height = logical_viewport_size_in_orientation_direction;
			}
			bool should_invalidate_scroll_info = false;
			if (extent_size != new_extent_size) {
				extent_size = new_extent_size;
				should_invalidate_scroll_info = true;
			}
			if (viewport_size != new_viewport_size) {
				viewport_size = new_viewport_size;
				should_invalidate_scroll_info = true;
			}
			#endregion
			#region Invalidate scroll info
			if (should_invalidate_scroll_info && ScrollOwner != null)
				ScrollOwner.InvalidateScrollInfo();
			#endregion
			#region Adjust result to respect stretch alignment and available size
			if (!(double.IsPositiveInfinity(availableSize.Width) || double.IsPositiveInfinity(availableSize.Height)) && !ContentScroll) {
				if (result.Width > availableSize.Width)
					result.Width = availableSize.Width;
				if (result.Height > availableSize.Height)
					result.Height = availableSize.Height;
			}
			#endregion
			return result;
		}
		#endregion

		#region Implicit interface implementations
		#region IScrollInfo
		#region Properties
		public bool CanHorizontallyScroll {
			get { return false; }
			set { }
		}

		public bool CanVerticallyScroll {
			get {
				return ContentScroll;
			}
			set { }
		}

		public double ExtentHeight {
			get {
				if (!ContentScroll)
					return 0;
				return extent_size.Height;
			}
		}

		public double ExtentWidth {
			get {
				if (!ContentScroll)
					return 0;
				return extent_size.Width;
			}
		}

		public double HorizontalOffset {
			get {
				if (!ContentScroll)
					return 0;
				return logical_offset_width;
			}
		}

		public ScrollViewer ScrollOwner {
			get { return scroll_owner; }
			set { scroll_owner = value; }
		}

		public double VerticalOffset {
			get {
				if (!ContentScroll)
					return 0;
				return logical_offset_height;
			}
		}

		public double ViewportHeight {
			get {
				if (!ContentScroll)
					return 0;
				return viewport_size.Height;
			}
		}

		public double ViewportWidth {
			get {
				if (!ContentScroll)
					return 0;
				return viewport_size.Width;
			}
		}
		#endregion

		#region Methods
		public void LineDown() {
			SetVerticalOffset(logical_offset_height + 1);
		}

		public void LineLeft() {
			SetHorizontalOffset(logical_offset_width - 1);
		}

		public void LineRight() {
			SetHorizontalOffset(logical_offset_width + 1);
		}

		public void LineUp() {
			SetVerticalOffset(logical_offset_height - 1);
		}

		public Rect MakeVisible(Visual visual, Rect rectangle) {
			if (!ContentScroll)
				return Rect.Empty;
			int child_index = Children.IndexOf((UIElement)visual);
			bool horizontal = Orientation == Orientation.Horizontal;
			int logical_offset = horizontal ? logical_offset_width : logical_offset_height;
			if (child_index < logical_offset)
				SetOffset(horizontal, child_index);
			else {
				double viewport = horizontal ? viewport_size.Width : viewport_size.Height;
				if (child_index >= logical_offset + viewport)
					SetOffset(horizontal, child_index - viewport + 1);
			}
			return Rect.Empty;
		}

		public void MouseWheelDown() {
			SetVerticalOffset(logical_offset_height + SystemParameters.WheelScrollLines);
		}

		public void MouseWheelLeft() {
			SetHorizontalOffset(logical_offset_width - SystemParameters.WheelScrollLines);
		}

		public void MouseWheelRight() {
			SetHorizontalOffset(logical_offset_width + SystemParameters.WheelScrollLines);
		}

		public void MouseWheelUp() {
			SetVerticalOffset(logical_offset_height - SystemParameters.WheelScrollLines);
		}

		public void PageDown() {
			SetVerticalOffset(logical_offset_height + ViewportHeight);
		}

		public void PageLeft() {
			SetHorizontalOffset(logical_offset_width - ViewportWidth);
		}

		public void PageRight() {
			SetHorizontalOffset(logical_offset_width + ViewportWidth);
		}

		public void PageUp() {
			SetVerticalOffset(logical_offset_height - ViewportHeight);
		}

		public void SetHorizontalOffset(double offset) {
			SetOffset(true, offset);
		}

		public void SetVerticalOffset(double offset) {
			SetOffset(false, offset);
		}
		#endregion
		#endregion
		#endregion

		#region Private Properties
		/// <summary>
		/// Whether content scrolling is enabled and, therefore, IScrollInfo members should do their job.
		/// </summary>
		bool ContentScroll {
			get {
				return ScrollOwner != null && ScrollOwner.CanContentScroll;
			}
		}
		#endregion

		#region Private Methods
		int GetAdjustedLogicalOffsetHeight(int offset) {
			return GetAdjustedLogicalOffset(offset, ExtentHeight, ViewportHeight);
		}

		static int GetAdjustedLogicalOffset(int offset, double extent, double viewport) {
			if (offset < 0 || viewport >= extent)
				return 0;
			else
				return Math.Min(offset, (int)(extent - viewport));
		}

		void SetOffset(bool horizontal, double offset) {
			int new_logical_offset;
			if (ContentScroll) {
				new_logical_offset = GetAdjustedLogicalOffset((int)offset, horizontal ? ExtentWidth : ExtentHeight, horizontal ? ViewportWidth : ViewportHeight);
				if ((horizontal ? logical_offset_width : logical_offset_height) != new_logical_offset) {
					if (horizontal)
						logical_offset_width = new_logical_offset;
					else
						logical_offset_height = new_logical_offset;
					if (ScrollOwner != null)
						ScrollOwner.InvalidateScrollInfo();
					InvalidateArrange();
				}
			} else {
				new_logical_offset = (int)offset;
				if (horizontal)
					logical_offset_width = new_logical_offset;
				else
					logical_offset_height = new_logical_offset;
			}
		}
		#endregion
	}
}