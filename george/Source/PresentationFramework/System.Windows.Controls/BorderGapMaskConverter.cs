//
// BorderGapMaskConverter.cs
//
// Author:
//   George Giolfan (georgegiolfan@yahoo.com)
//
// Copyright (C) 2007 George Giolfan
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	public class BorderGapMaskConverter : IMultiValueConverter
	{
		#region Public Constructors
		public BorderGapMaskConverter ()
		{
		}
		#endregion

		#region Public Methods
		public object Convert (object [] values, Type targetType, object parameter, CultureInfo culture)
		{
			try {
				double header_width = (double)values [0];
				double width = (double)values [1];
				double height = (double)values [2];
				double visible_line_width;
				if (parameter is double)
					visible_line_width = (double)parameter;
				else {
					string parameter_string = parameter as string;
					if (parameter == null)
						return DependencyProperty.UnsetValue;
					visible_line_width = double.Parse (parameter_string);
				}
				Grid grid = new Grid ();
				grid.Height = height;
				grid.Width = width;
				ColumnDefinition column_definition = new ColumnDefinition ();
				column_definition.Width = new GridLength (visible_line_width);
				grid.ColumnDefinitions.Add (column_definition);
				column_definition = new ColumnDefinition ();
				column_definition.Width = new GridLength (header_width);
				grid.ColumnDefinitions.Add (column_definition);
				grid.ColumnDefinitions.Add (new ColumnDefinition ());
				RowDefinition row_definition = new RowDefinition ();
				row_definition.Height = new GridLength (width);
				grid.RowDefinitions.Add (row_definition);
				grid.RowDefinitions.Add (new RowDefinition ());
				Rectangle rectangle = new Rectangle ();
				rectangle.Fill = Brushes.Black;
				Grid.SetRowSpan (rectangle, 2);
				grid.Children.Add (rectangle);
				rectangle = new Rectangle ();
				rectangle.Fill = Brushes.Black;
				Grid.SetColumn (rectangle, 1);
				Grid.SetRow (rectangle, 1);
				grid.Children.Add (rectangle);
				rectangle = new Rectangle ();
				rectangle.Fill = Brushes.Black;
				Grid.SetColumn (rectangle, 2);
				Grid.SetRowSpan (rectangle, 2);
				grid.Children.Add (rectangle);
				return new VisualBrush (grid);
			} catch {
				return DependencyProperty.UnsetValue;
			}
		}

		public object [] ConvertBack (object value, Type [] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException ();
		}
		#endregion
	}
}