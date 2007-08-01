#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	public class UniformGrid : Panel {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(UniformGrid), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure), ValidateNonNegativeInteger);
		public static readonly DependencyProperty FirstColumnProperty = DependencyProperty.Register("FirstColumn", typeof(int), typeof(UniformGrid), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure), ValidateNonNegativeInteger);
		public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(UniformGrid), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure), ValidateNonNegativeInteger);
		#endregion
		#endregion

		#region Public Constructors
		public UniformGrid() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public int Columns {
			get { return (int)GetValue(ColumnsProperty); }
			set { SetValue(ColumnsProperty, value); }
		}

		public int FirstColumn {
			get { return (int)GetValue(FirstColumnProperty); }
			set { SetValue(FirstColumnProperty, value); }
		}

		public int Rows {
			get { return (int)GetValue(RowsProperty); }
			set { SetValue(RowsProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize) {
			int children_count = Children.Count;
			if (children_count == 0)
				return finalSize;

			int actual_columns;
			int actual_first_column;
			int actual_rows;
			ComputeActualParameters(children_count, out actual_columns, out actual_first_column, out actual_rows);

			double child_width = finalSize.Width / actual_columns;
			double child_height = finalSize.Height / actual_rows;
			for (int child_index = 0; child_index < children_count; child_index++) {
				int left;
				int top = Math.DivRem(child_index + actual_first_column, actual_columns, out left);
				Children[child_index].Arrange(new Rect(left * child_width, top * child_height, child_width, child_height));
			}
			return finalSize;
		}

		protected override Size MeasureOverride(Size availableSize) {
			int children_count = Children.Count;
			if (children_count == 0)
				return new Size(0, 0);

			int actual_columns;
			int actual_first_column;
			int actual_rows;
			ComputeActualParameters(children_count, out actual_columns, out actual_first_column, out actual_rows);

			Size child_measure_size = new Size(double.IsPositiveInfinity(availableSize.Width) ? double.PositiveInfinity : availableSize.Width / actual_columns, double.IsPositiveInfinity(availableSize.Height) ? double.PositiveInfinity : availableSize.Height / actual_rows);
			foreach (UIElement child in Children)
				child.Measure(child_measure_size);
			return new Size(0, 0);
		}
		#endregion

		#region Private Methods
		static bool ValidateNonNegativeInteger(object value) {
			return (int)value >= 0;
		}

		void ComputeActualParameters(int childrenCount, out int actualColumns, out int actualFirstColumn, out int actualRows) {
			actualColumns = Columns;
			actualFirstColumn = FirstColumn;
			actualRows = Rows;
			if (actualColumns == 0) {
				actualFirstColumn = 0;
				if (actualRows == 0)
					actualColumns = actualRows = (int)Math.Ceiling(Math.Sqrt(childrenCount));
				else
					actualColumns = (int)Math.Ceiling((decimal)childrenCount / actualRows);
			} else {
				if (actualFirstColumn >= actualColumns)
					actualFirstColumn = 0;
				if (actualRows == 0)
					actualRows = (int)Math.Ceiling((decimal)(childrenCount + actualFirstColumn) / actualColumns);
			}
		}
		#endregion
	}
}