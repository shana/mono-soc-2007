using System;
using System.Windows;
using System.Windows.Controls;

namespace grid_getset_methods
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : Window
	{
		private void setCol0(object sender, RoutedEventArgs e) 
		{
            Mono.System.Windows.Controls.Grid.SetColumn(rect1, 0);
			txt1.Text = "Rectangle is in Column " + Mono.System.Windows.Controls.Grid.GetColumn(rect1).ToString();

		}
        private void setCol1(object sender, RoutedEventArgs e)
        {
			Mono.System.Windows.Controls.Grid.SetColumn(rect1, 1);
			txt1.Text = "Rectangle is in Column " + Mono.System.Windows.Controls.Grid.GetColumn(rect1).ToString();
        }
        private void setCol2(object sender, RoutedEventArgs e)
        {
			Mono.System.Windows.Controls.Grid.SetColumn(rect1, 2);
			txt1.Text = "Rectangle is in Column " + Mono.System.Windows.Controls.Grid.GetColumn(rect1).ToString();
        }
		private void setRow0(object sender, RoutedEventArgs e)
		{
			Mono.System.Windows.Controls.Grid.SetRow(rect1, 0);
			txt2.Text = "Rectangle is in Row " + Mono.System.Windows.Controls.Grid.GetRow(rect1).ToString();
		}
        private void setRow1(object sender, RoutedEventArgs e)
        {
			Mono.System.Windows.Controls.Grid.SetRow(rect1, 1);
			txt2.Text = "Rectangle is in Row " + Mono.System.Windows.Controls.Grid.GetRow(rect1).ToString();
        }
        private void setRow2(object sender, RoutedEventArgs e)
        {
			Mono.System.Windows.Controls.Grid.SetRow(rect1, 2);
			txt2.Text = "Rectangle is in Row " + Mono.System.Windows.Controls.Grid.GetRow(rect1).ToString();
        }
        private void setColspan(object sender, RoutedEventArgs e)
		{
			Mono.System.Windows.Controls.Grid.SetColumnSpan(rect1, 3);
			txt3.Text = "ColumnSpan is set to " + Mono.System.Windows.Controls.Grid.GetColumnSpan(rect1).ToString();
		}
		private void setRowspan(object sender, RoutedEventArgs e)
		{
			Mono.System.Windows.Controls.Grid.SetRowSpan(rect1, 3);
			txt4.Text = "RowSpan is set to " + Mono.System.Windows.Controls.Grid.GetRowSpan(rect1).ToString();
		}
        private void clearAll(object sender, RoutedEventArgs e)
        {
			Mono.System.Windows.Controls.Grid.SetColumn(rect1, 0);
			Mono.System.Windows.Controls.Grid.SetRow(rect1, 0);
			Mono.System.Windows.Controls.Grid.SetColumnSpan(rect1, 1);
			Mono.System.Windows.Controls.Grid.SetRowSpan(rect1, 1);
            txt1.Text = "Rectangle is in Column 0";
            txt2.Text = "Rectangle is in Row 0";
            txt3.Text = "ColumnSpan is set to 1 (default)";
            txt4.Text = "RowSpan is set to 1 (default)";
        }

	}
}