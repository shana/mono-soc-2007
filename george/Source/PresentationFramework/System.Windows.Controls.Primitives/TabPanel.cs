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
	public class TabPanel : Panel {
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
			for (int child_index = 0; child_index < Children.Count; child_index++) {
				UIElement child = Children[child_index];
				row_height = Math.Max(row_height, horizontal ? child.DesiredSize.Height : child.DesiredSize.Width);
				double child_width  = GetDesiredChildWidth(child);
				total_width += child_width;
				used_width += child_width;
				if (child_index == Children.Count - 1 || used_width > available_width) {
					used_width = 0;
					rows++;
				}
			}
			double used_height = 0;
			List<UIElement> current_row_elements = new List<UIElement>();
			double average_width = total_width / rows;
			for (int child_index = 0; child_index < Children.Count; child_index++) {
				UIElement child = Children[child_index];
				double child_width = GetDesiredChildWidth(child);
				used_width += child_width;
				current_row_elements.Add(child);
				if (child_index == Children.Count - 1 || used_width > average_width || (used_width + GetDesiredChildWidth(Children[child_index + 1]) > available_width && used_width != 0)) {
					double width_ratio = available_width / used_width;
					used_width = 0;
					foreach (UIElement current_row_element in current_row_elements) {
						child_width = GetDesiredChildWidth(current_row_element) * width_ratio;
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
			double used_width = 0;
			double maximum_height = 0;
			foreach (UIElement child in Children) {
				child.Measure(availableSize);
				used_width += horizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
				maximum_height = Math.Max(maximum_height, horizontal ? child.DesiredSize.Height : child.DesiredSize.Width);
			}
			return horizontal ? new Size(used_width, maximum_height) : new Size(maximum_height, used_width);
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