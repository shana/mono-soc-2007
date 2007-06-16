using System.Collections.Generic;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	public class TabPanel : global::System.Windows.Controls.Panel {
		#region Public Constructors
		public TabPanel() {
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize) {
			//FIXME: The Microsoft implementation distributes the elements on rows a little better.
			bool horizontal = GetHorizontal();
			double row_height = 0;
			double used_width = 0;
			double total_width = 0;
			int rows = 0;
			double available_width = horizontal ? finalSize.Width : finalSize.Height;
			for (int child_index = 0; child_index < InternalChildren.Count; child_index++) {
				UIElement child = InternalChildren[child_index];
				row_height = Math.Max(row_height, horizontal ? child.DesiredSize.Height : child.DesiredSize.Width);
				double child_width  = GetDesiredChildWidth(child);
				total_width += child_width;
				used_width += child_width;
				if (child_index == InternalChildren.Count - 1 || used_width > available_width) {
					used_width = 0;
					rows++;
				}
			}
			bool should_expand = rows != 1;
			double used_height = 0;
			List<UIElement> current_row_elements = new List<UIElement>();
			double average_width = total_width / rows;
			for (int child_index = 0; child_index < InternalChildren.Count; child_index++) {
				UIElement child = InternalChildren[child_index];
				double child_width = GetDesiredChildWidth(child);
				used_width += child_width;
				current_row_elements.Add(child);
				if (child_index == InternalChildren.Count - 1 || used_width > average_width || (used_width + GetDesiredChildWidth(InternalChildren[child_index + 1]) > available_width && used_width != 0)) {
					double width_ratio;
					if (should_expand)
						width_ratio = available_width / used_width;
					else
						width_ratio = 0;
					used_width = 0;
					foreach (UIElement current_row_element in current_row_elements) {
						child_width = GetDesiredChildWidth(current_row_element);
						if (should_expand)
							child_width *= width_ratio;
						current_row_element.Arrange(horizontal ? new Rect(used_width, used_height, child_width, row_height) : new Rect(used_height, used_width, row_height, child_width));
						used_width += child_width;
					}
					current_row_elements.Clear();
					used_width = 0;
					used_height += row_height;
				}
			}
			return finalSize;
		}

		protected override Geometry GetLayoutClip(Size layoutSlotSize) {
			return null;
		}

		protected override Size MeasureOverride(Size availableSize) {
			bool horizontal = GetHorizontal();
			double available_width = horizontal ? availableSize.Width : availableSize.Height;
			double width = 0;
			double height = 0;
			int rows = 0;
			bool limited_width = !double.IsPositiveInfinity(available_width) && double.IsPositiveInfinity(horizontal ? availableSize.Height : availableSize.Width);
			for (int child_index = 0; child_index < InternalChildren.Count; child_index++) {
				UIElement child = InternalChildren[child_index];
				child.Measure(availableSize);
				width += horizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
				if (limited_width && (width >= available_width || child_index == InternalChildren.Count - 1)) {
					width = 0;
					rows++;
				}
				height = Math.Max(height, horizontal ? child.DesiredSize.Height : child.DesiredSize.Width);
			}
			if (limited_width) {
				width = available_width;
				height *= rows;
			}
			return horizontal ? new Size(width, height) : new Size(height, width);
		}
		#endregion

		#region Private Methods
		bool GetHorizontal() {
			TabControl tab_control = TemplatedParent as TabControl;
			return tab_control == null || tab_control.TabStripPlacement == Dock.Top || tab_control.TabStripPlacement == Dock.Bottom;
		}

		double GetDesiredChildWidth(UIElement element) {
			return GetHorizontal() ? element.DesiredSize.Width : element.DesiredSize.Height;
		}
		#endregion
	}
}