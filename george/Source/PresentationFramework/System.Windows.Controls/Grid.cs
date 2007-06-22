using System.Collections;
using System.Windows.Markup;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public class Grid : Panel {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty ShowGridLinesProperty = DependencyProperty.Register("ShowGridLines", typeof(bool), typeof(Grid), new FrameworkPropertyMetadata());
		#region Attached Properties
		public static readonly DependencyProperty ColumnProperty = DependencyProperty.RegisterAttached("Column", typeof(int), typeof(Grid), new FrameworkPropertyMetadata(InvalidateGridMeasure), ValidateRowColumn);
		public static readonly DependencyProperty ColumnSpanProperty = DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), typeof(Grid), new FrameworkPropertyMetadata(1, InvalidateGridMeasure), ValidateSpan);
		public static readonly DependencyProperty IsSharedSizeScopeProperty = DependencyProperty.RegisterAttached("IsSharedSizeScope", typeof(bool), typeof(Grid), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty RowProperty = DependencyProperty.RegisterAttached("Row", typeof(int), typeof(Grid), new FrameworkPropertyMetadata(InvalidateGridMeasure), ValidateRowColumn);
		public static readonly DependencyProperty RowSpanProperty = DependencyProperty.RegisterAttached("RowSpan", typeof(int), typeof(Grid), new FrameworkPropertyMetadata(1, InvalidateGridMeasure), ValidateSpan);
		#endregion
		#endregion
		#endregion

		#region Private Fields
		ColumnDefinitionCollection column_definitions;
		RowDefinitionCollection row_definitions;
		bool measure_called;
		GridLinesRenderer grid_lines_renderer;
		#endregion

		#region Public Constructors
		public Grid() {
			column_definitions = new ColumnDefinitionCollection(this);
			row_definitions = new RowDefinitionCollection(this);
		}
		#endregion

		#region Public Properties
		public ColumnDefinitionCollection ColumnDefinitions {
			get { return column_definitions; }
		}

		public RowDefinitionCollection RowDefinitions {
			get { return row_definitions; }
		}

		#region Dependency Properties
		public bool ShowGridLines {
			get { return (bool)GetValue(ShowGridLinesProperty); }
			set { SetValue(ShowGridLinesProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Properties
		protected override IEnumerator LogicalChildren {
			get {
				return base.LogicalChildren;
			}
		}

		protected override int VisualChildrenCount {
			get {
				return base.VisualChildrenCount + (grid_lines_renderer == null ? 0 : 1);
			}
		}
		#endregion

		#region Public Methods
		#region Attached Properties
		[AttachedPropertyBrowsableForChildren]
		public static int GetColumn(UIElement element) {
			return (int)element.GetValue(ColumnProperty);
		}

		[AttachedPropertyBrowsableForChildren]
		public static int GetColumnSpan(UIElement element) {
			return (int)element.GetValue(ColumnSpanProperty);
		}

		public static bool GetIsSharedSizeScope(UIElement element) {
			return (bool)element.GetValue(IsSharedSizeScopeProperty);
		}

		[AttachedPropertyBrowsableForChildren]
		public static int GetRow(UIElement element) {
			return (int)element.GetValue(RowProperty);
		}

		[AttachedPropertyBrowsableForChildren]
		public static int GetRowSpan(UIElement element) {
			return (int)element.GetValue(RowSpanProperty);
		}

		public static void SetColumn(UIElement element, int value) {
			element.SetValue(ColumnProperty, value);
		}

		public static void SetColumnSpan(UIElement element, int value) {
			element.SetValue(ColumnSpanProperty, value);
		}

		public static void SetIsSharedSizeScope(UIElement element, bool value) {
			element.SetValue(IsSharedSizeScopeProperty, value);
		}

		public static void SetRow(UIElement element, int value) {
			element.SetValue(RowProperty, value);
		}

		public static void SetRowSpan(UIElement element, int value) {
			element.SetValue(RowSpanProperty, value);
		}
		#endregion

		public bool ShouldSerializeColumnDefinitions() {
			return column_definitions.Count != 0;
		}

		public bool ShouldSerializeRowDefinitions() {
			return row_definitions.Count != 0;
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize) {
			bool has_row_definitions = row_definitions.Count != 0;
			bool has_column_definitions = column_definitions.Count != 0;
			if ((has_row_definitions || has_column_definitions) && !measure_called)
				throw new NullReferenceException();
			int row_count = GetRowColumnCount(row_definitions.Count);
			int column_count = GetRowColumnCount(column_definitions.Count);
			double[] row_heights = new double[row_count];
			double[] column_widths = new double[column_count];
			int index;
			#region Compute initial row and column size
			if (has_row_definitions)
				for (index = 0; index < row_count; index++) {
					GridLength row_definition_height = row_definitions[index].Height;
					row_heights[index] = row_definition_height.IsAbsolute ? row_definition_height.Value : double.PositiveInfinity;
				} 
			else
				row_heights[0] = finalSize.Height;
			if (has_column_definitions)
				for (index = 0; index < column_count; index++) {
					GridLength column_definition_width = column_definitions[index].Width;
					column_widths[index] = column_definition_width.IsAbsolute ? column_definition_width.Value : double.PositiveInfinity;
				} 
			else
				column_widths[0] = finalSize.Width;
			#endregion
			if (has_row_definitions || has_column_definitions) {
				double[] desired_row_heights = new double[row_count];
				double[] desired_column_widths = new double[column_count];
				foreach (UIElement child in InternalChildren) {
					if (has_row_definitions) {
						int child_row = GetRow(child);
						if (!row_definitions[child_row].Height.IsAbsolute && GetRowSpan(child) == 1)
							desired_row_heights[child_row] = Math.Max(desired_row_heights[child_row], child.DesiredSize.Height);
					}
					if (has_column_definitions) {
						int child_column = GetColumn(child);
						if (!column_definitions[child_column].Width.IsAbsolute && GetColumnSpan(child) == 1)
							desired_column_widths[child_column] = Math.Max(desired_column_widths[child_column], child.DesiredSize.Width);
					}
				}
				if (has_row_definitions)
					for (index = 0; index < row_count; index++)
						if (row_definitions[index].Height.IsAuto)
							row_heights[index] = desired_row_heights[index];
				if (has_column_definitions)
					for (index = 0; index < column_count; index++)
						if (column_definitions[index].Width.IsAuto)
							column_widths[index] = desired_column_widths[index];
				#region Apply definition minimum and maximum
				if (has_row_definitions)
					for (index = 0; index < row_count; index++) {
						RowDefinition row_definition = row_definitions[index];
						row_heights[index] = AdjustToBeInRange(row_heights[index], row_definition.MinHeight, row_definition.MaxHeight);
					}
				if (has_column_definitions)
					for (index = 0; index < column_count; index++) {
						ColumnDefinition column_definition = column_definitions[index];
						column_widths[index] = AdjustToBeInRange(column_widths[index], column_definition.MinWidth, column_definition.MaxWidth);
					}
				#endregion
				double total_star;
				double remaining_lenght;
				GridLength lenght;
				double star_ratio;
				double offset;
				if (has_row_definitions) {
					total_star = 0;
					remaining_lenght = finalSize.Height;
					for (index = 0; index < row_count; index++) {
						lenght = row_definitions[index].Height;
						if (lenght.IsStar)
							total_star += lenght.Value;
						else
							remaining_lenght -= row_heights[index];
					}
					if (remaining_lenght > 0 && total_star != 0) {
						star_ratio = remaining_lenght / total_star;
						for (index = 0; index < row_count; index++) {
							lenght = row_definitions[index].Height;
							if (lenght.IsStar)
								row_heights[index] = Math.Max(lenght.Value * star_ratio, desired_row_heights[index]);
						}
					}
					remaining_lenght = finalSize.Height;
					offset = 0;
					for (index = 0; index < row_count; index++) {
						if (row_heights[index] > remaining_lenght)
							row_heights[index] = remaining_lenght;
						remaining_lenght -= row_heights[index];
						RowDefinition definition = row_definitions[index];
						definition.Offset = offset;
						offset += definition.ActualHeight = row_heights[index];
					}
				}
				if (has_column_definitions) {
					total_star = 0;
					remaining_lenght = finalSize.Width;
					for (index = 0; index < column_count; index++) {
						lenght = column_definitions[index].Width;
						if (lenght.IsStar)
							total_star += lenght.Value;
						else
							remaining_lenght -= column_widths[index];
					}
					if (remaining_lenght > 0 && total_star != 0) {
						star_ratio = remaining_lenght / total_star;
						for (index = 0; index < column_count; index++) {
							lenght = column_definitions[index].Width;
							if (lenght.IsStar)
								column_widths[index] = Math.Max(lenght.Value * star_ratio, desired_column_widths[index]);
							column_definitions[index].ActualWidth = column_widths[index];
						}
					}
					remaining_lenght = finalSize.Width;
					offset = 0;
					for (index = 0; index < column_count; index++) {
						if (column_widths[index] > remaining_lenght)
							column_widths[index] = remaining_lenght;
						remaining_lenght -= column_widths[index];
						ColumnDefinition definition = column_definitions[index];
						definition.Offset = offset;
						offset += definition.ActualWidth = column_widths[index];
					}
				}
			}
			foreach (UIElement child in InternalChildren) {
				Rect rect = new Rect();
				int row_column = GetRow(child);
				for (index = 0; index < row_column; index++)
					rect.Y += row_heights[index];
				int maximum = Math.Min(row_column + GetRowSpan(child), row_count);
				for (index = row_column; index < maximum; index++)
					rect.Height += row_heights[index];
				row_column = GetColumn(child);
				for (index = 0; index < row_column; index++)
					rect.X += column_widths[index];
				maximum = Math.Min(row_column + GetColumnSpan(child), column_count);
				for (index = row_column; index < maximum; index++)
					rect.Width += column_widths[index];
				child.Arrange(rect);
			}
			if (grid_lines_renderer == null && ShowGridLines && (has_row_definitions || has_column_definitions))
				grid_lines_renderer = new GridLinesRenderer(this);
			if (grid_lines_renderer != null)
				grid_lines_renderer.Rebuild();
			return finalSize;
		}

		protected override Visual GetVisualChild(int index) {
			if (grid_lines_renderer != null && index == InternalChildren.Count)
				return grid_lines_renderer;
			return base.GetVisualChild(index);
		}

		protected override Size MeasureOverride(Size availableSize) {
			measure_called = true;
			bool has_row_definitions = row_definitions.Count != 0;
			bool has_column_definitions = column_definitions.Count != 0;
			int row_count = GetRowColumnCount(row_definitions.Count);
			int column_count = GetRowColumnCount(column_definitions.Count);
			double[] row_heights = new double[row_count];
			double[] column_widths = new double[column_count];
			int index;
			#region Compute initial row and column size
			if (has_row_definitions)
				for (index = 0; index < row_definitions.Count; index++) {
					GridLength row_definition_height = row_definitions[index].Height;
					row_heights[index] = row_definition_height.IsAbsolute ? row_definition_height.Value : double.PositiveInfinity;
				} 
			else
				row_heights[0] = double.PositiveInfinity;
			if (has_column_definitions)
				for (index = 0; index < column_definitions.Count; index++) {
					GridLength column_definition_width = column_definitions[index].Width;
					column_widths[index] = column_definition_width.IsAbsolute ? column_definition_width.Value : double.PositiveInfinity;
				} 
			else
				column_widths[0] = double.PositiveInfinity;
			#endregion
			double[] desired_row_heights = new double[row_count];
			double[] desired_column_widths = new double[column_count];
			foreach (UIElement child in InternalChildren) {
				int child_row = GetRow(child);
				int child_column = GetColumn(child);
				int child_row_span = GetRowSpan(child);
				int child_column_span = GetColumnSpan(child);
				Size child_constraint = new Size();
				int maximum;
				if (has_row_definitions) {
					maximum = Math.Min(child_row + child_row_span, row_count);
					for (index = child_row; index < maximum; index++)
						child_constraint.Height += row_heights[index];
					if (double.IsPositiveInfinity(child_constraint.Height) && child_row_span == 1 && row_definitions[child_row].Height.IsStar)
						child_constraint.Height = availableSize.Height;
				} else
					child_constraint.Height = availableSize.Height;
				if (has_column_definitions) {
					maximum = Math.Min(child_column + child_column_span, column_count);
					for (index = child_column; index < maximum; index++)
						child_constraint.Width += column_widths[index];
					if (double.IsPositiveInfinity(child_constraint.Width) && child_column_span == 1 && column_definitions[child_column].Width.IsStar)
						child_constraint.Width = availableSize.Width;
				} else
					child_constraint.Width = availableSize.Width;
				child.Measure(child_constraint);
				if (child_row_span == 1)
					desired_row_heights[child_row] = Math.Max(desired_row_heights[child_row], child.DesiredSize.Height);
				if (child_column_span == 1)
					desired_column_widths[child_column] = Math.Max(desired_column_widths[child_column], child.DesiredSize.Width);
			}
			for (index = 0; index < row_count; index++)
				if (double.IsPositiveInfinity(row_heights[index]))
					row_heights[index] = desired_row_heights[index];
			for (index = 0; index < column_count; index++)
				if (double.IsPositiveInfinity(column_widths[index]))
					column_widths[index] = desired_column_widths[index];
			#region Apply definition minimum and maximum
			if (has_row_definitions)
				for (index = 0; index < row_count; index++) {
					RowDefinition row_definition = row_definitions[index];
					row_heights[index] = AdjustToBeInRange(row_heights[index], row_definition.MinHeight, row_definition.MaxHeight);
				}
			if (has_column_definitions)
				for (index = 0; index < column_count; index++) {
					ColumnDefinition column_definition = column_definitions[index];
					column_widths[index] = AdjustToBeInRange(column_widths[index], column_definition.MinWidth, column_definition.MaxWidth);
				}
			#endregion
			double remaining_lenght;
			if (has_row_definitions) {
				remaining_lenght = availableSize.Height;
				for (index = 0; index < row_count; index++) {
					if (row_heights[index] > remaining_lenght)
						row_heights[index] = remaining_lenght;
					remaining_lenght -= row_heights[index];
				}
			}
			if (has_column_definitions) {
				remaining_lenght = availableSize.Width;
				for (index = 0; index < column_count; index++) {
					if (column_widths[index] > remaining_lenght)
						column_widths[index] = remaining_lenght;
					remaining_lenght -= column_widths[index];
				}
			}
			Size result = new Size();
			for (index = 0; index < row_count; index++)
				result.Height += row_heights[index];
			for (index = 0; index < column_count; index++)
				result.Width += column_widths[index];
			return result;
		}
		
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
		}
		#endregion

		#region Private Methods
		static int GetRowColumnCount(int definitionsCount) {
			return definitionsCount == 0 ? 1 : definitionsCount;
		}

		static double AdjustToBeInRange(double value, double minimum, double maximum) {
			if (value < minimum)
				return minimum;
			if (value > maximum)
				return maximum;
			return value;
		}

		static void InvalidateGridMeasure(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Grid grid = VisualTreeHelper.GetParent(d) as Grid;
			if (grid == null)
				return;
			grid.InvalidateMeasure();
		}

		static bool ValidateRowColumn(object value) {
			return (double)value >= 0;
		}

		static bool ValidateSpan(object value) {
			return (double)value > 0;
		}
		#endregion

		#region Classes
		class GridLinesRenderer : DrawingVisual {
			Grid grid;

			public GridLinesRenderer(Grid grid) {
				this.grid = grid;
			}

			public void Rebuild() {
				DrawingContext drawing_context = RenderOpen();
				int definition_index;
				for (definition_index = 1; definition_index < grid.row_definitions.Count; definition_index++)
					DrawLine(drawing_context, grid.row_definitions[definition_index].Offset, true);
				for (definition_index = 1; definition_index < grid.column_definitions.Count; definition_index++)
					DrawLine(drawing_context, grid.column_definitions[definition_index].Offset, false);
				drawing_context.Close();
				grid.InvalidateVisual();
			}

			void DrawLine(DrawingContext drawingContext, double position, bool horizontal) {
				//FIXME
				drawingContext.DrawLine(new Pen(Brushes.Black, 1), new Point(horizontal ? 0 : position, horizontal ? position : 0), new Point(horizontal ? grid.ActualWidth : position, horizontal ? position : grid.ActualHeight));
			}
		}
		#endregion
	}
}