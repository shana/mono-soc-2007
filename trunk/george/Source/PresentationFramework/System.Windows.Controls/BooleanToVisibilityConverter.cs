using System.Globalization;
using System.Windows.Data;
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[Localizability(LocalizationCategory.NeverLocalize)]
	public sealed class BooleanToVisibilityConverter : IValueConverter {
		#region Public Constructors
		public BooleanToVisibilityConverter() {

		}
		#endregion

		#region Public Methods
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value is Visibility && (Visibility)value == Visibility.Visible;
		}
		#endregion
	}
}