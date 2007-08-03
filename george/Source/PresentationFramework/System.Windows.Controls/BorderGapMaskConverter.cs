using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public class BorderGapMaskConverter : IMultiValueConverter {
		#region Public Constructors
		public BorderGapMaskConverter() {
		}
		#endregion

		#region Public Methods
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			try {
				double header_width = (double)values[0];
				double width = (double)values[1];
				double height = (double)values[2];
				double visible_line_width;
				if (parameter is double)
					visible_line_width = (double)parameter;
				else {
					string parameter_string = parameter as string;
					if (parameter == null)
						return DependencyProperty.UnsetValue;
					visible_line_width = double.Parse(parameter_string);
				}
				Grid grid = new Grid();
				grid.Height = height;
				grid.Width = width;
				ColumnDefinition column_definition = new ColumnDefinition();
				column_definition.Width = new GridLength(visible_line_width);
				grid.ColumnDefinitions.Add(column_definition);
				column_definition = new ColumnDefinition();
				column_definition.Width = new GridLength(header_width);
				grid.ColumnDefinitions.Add(column_definition);
				grid.ColumnDefinitions.Add(new ColumnDefinition());
				RowDefinition row_definition = new RowDefinition();
				row_definition.Height = new GridLength(width);
				grid.RowDefinitions.Add(row_definition);
				grid.RowDefinitions.Add(new RowDefinition());
				Rectangle rectangle = new Rectangle();
				rectangle.Fill = Brushes.Black;
				Grid.SetRowSpan(rectangle, 2);
				grid.Children.Add(rectangle);
				rectangle = new Rectangle();
				rectangle.Fill = Brushes.Black;
				Grid.SetColumn(rectangle, 1);
				Grid.SetRow(rectangle, 1);
				grid.Children.Add(rectangle);
				rectangle = new Rectangle();
				rectangle.Fill = Brushes.Black;
				Grid.SetColumn(rectangle, 2);
				Grid.SetRowSpan(rectangle, 2);
				grid.Children.Add(rectangle);
				return new VisualBrush(grid);
			} catch {
				return DependencyProperty.UnsetValue;
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
		#endregion
	}
}