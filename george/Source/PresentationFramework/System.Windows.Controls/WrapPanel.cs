using System.Collections.Generic;
using System.ComponentModel;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public class WrapPanel : Panel {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(WrapPanel), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(WrapPanel), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(WrapPanel), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));
		#endregion
		#endregion

		#region Public Constructors
		public WrapPanel() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[TypeConverter(typeof(LengthConverter))]
		public double ItemHeight {
			get { return (double)GetValue(ItemHeightProperty); }
			set { SetValue(ItemHeightProperty, value); }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double ItemWidth {
			get { return (double)GetValue(ItemWidthProperty); }
			set { SetValue(ItemWidthProperty, value); }
		}

		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize) {
			// In the following code the meaning of width, height, row, top in local variable names is height, width, column, left when the Orientation is Vertical. 
			bool horizontal = Orientation == Orientation.Horizontal;
			double used_width = 0;
			double used_height = 0;
			double available_row_width = horizontal ? finalSize.Width : finalSize.Height;
			double current_row_height = 0;
			List<UIElement> current_row_elements = new List<UIElement>();
			for(int child_index = 0; child_index < Children.Count; child_index++) {
				UIElement child = Children[child_index];
				Size desired_child_size = GetDesiredChildSize(child);
				current_row_elements.Add(child);
				used_width += desired_child_size.Width;
				current_row_height = Math.Max(current_row_height, desired_child_size.Height);
				if ((used_width + desired_child_size.Width > available_row_width && used_width != 0) || child_index == Children.Count - 1) {
					used_width = 0;
					foreach (UIElement current_row_element in current_row_elements) {
						desired_child_size = GetDesiredChildSize(current_row_element);
						double top = used_height;
						FrameworkElement framework_element = current_row_element as FrameworkElement;
						if (framework_element != null)
							if (horizontal)
								switch (framework_element.VerticalAlignment) {
								case VerticalAlignment.Top:
									break;
								case VerticalAlignment.Bottom:
									top += current_row_height - desired_child_size.Height;
									break;
								case VerticalAlignment.Center:
									top += (current_row_height - desired_child_size.Height) / 2;
									break;
								default:
									desired_child_size.Height = Math.Min(current_row_height, finalSize.Height);
									break;
								} else
								switch (framework_element.HorizontalAlignment) {
								case HorizontalAlignment.Left:
									break;
								case HorizontalAlignment.Right:
									top += current_row_height - desired_child_size.Height;
									break;
								case HorizontalAlignment.Center:
									top += (current_row_height - desired_child_size.Height) / 2;
									break;
								default:
									desired_child_size.Height = Math.Min(current_row_height, finalSize.Width);
									break;
								}
						current_row_element.Arrange(horizontal ? new Rect(used_width, top, desired_child_size.Width, desired_child_size.Height) : new Rect(top, used_width, desired_child_size.Height, desired_child_size.Width));
						used_width += desired_child_size.Width;
					}
					current_row_elements.Clear();
					used_width = 0;
					used_height += current_row_height;
					current_row_height = 0;
				}
			}
			return finalSize;
		}

		protected override Size MeasureOverride(Size availableSize) {
			if (double.IsPositiveInfinity(availableSize.Width) || double.IsPositiveInfinity(availableSize.Height)) {
				double width = 0;
				double height = 0;
				foreach (UIElement child in Children) {
					child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
					Size desired_size = GetDesiredChildSize(child);
					width += desired_size.Width;
					height = Math.Max(height, desired_size.Height);
				}
				return (Orientation == Orientation.Horizontal) ? new Size(width, height) : new Size(height, width);
			} else {
				foreach (UIElement child in Children)
					child.Measure(availableSize);
				return availableSize;
			}
		}
		#endregion

		#region Private Methods
		Size GetDesiredChildSize(UIElement element) {
			Size result = new Size(element.DesiredSize.Width, element.DesiredSize.Height);
			FrameworkElement framework_element = element as FrameworkElement;
			if (framework_element != null) {
				if (!double.IsNaN(ItemWidth) && double.IsNaN(framework_element.Width)) 
					result.Width = ItemWidth;
				if (!double.IsNaN(ItemHeight) && double.IsNaN(framework_element.Height)) 
					result.Height = ItemHeight;
			}
			if (Orientation == Orientation.Vertical)
				result = new Size(result.Height, result.Width);
			return result;
		}
		#endregion
	}
}