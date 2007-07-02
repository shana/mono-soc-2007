using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
#if Implementation
namespace Mono.Microsoft.Windows.Themes {
#else
namespace Microsoft.Windows.Themes {
#endif
	public class ProgressBarBrushConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			Brush foreground = (Brush)values[0];
			bool is_indeterminate = (bool)values[1];
			double indicator_lenght_in_orientation_direction = (double)values[2];
			double indicator_lenght_in_other_direction = (double)values[3];
			double track_lenght_in_orientation_direction = (double)values[4];
			const double LineWidth = 6;
			//FIXME
			return foreground;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			return null;
		}
	}
}