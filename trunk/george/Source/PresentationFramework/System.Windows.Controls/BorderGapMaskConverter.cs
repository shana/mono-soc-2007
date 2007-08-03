using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;namespace Mono.System.Windows.Controls {
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
				double visible_line_width = (double)parameter;
				VisualBrush result = new VisualBrush();
				return result;
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
