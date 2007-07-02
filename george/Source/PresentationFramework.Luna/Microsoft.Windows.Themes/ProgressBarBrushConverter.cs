using System;
using System.Globalization;
using System.Windows;
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
			//FIXME: How is it used?
			bool is_indeterminate = (bool)values[1];
			double indicator_lenght_in_orientation_direction = (double)values[2];
			double indicator_lenght_in_other_direction = (double)values[3];
			double track_lenght_in_orientation_direction = (double)values[4];
			const double LineWidth = 6;
			const double LineSpacing = 2;
			DrawingGroup drawing = new DrawingGroup();
			DrawingContext drawing_context = drawing.Open();
			int lines = (int)Math.Ceiling(indicator_lenght_in_orientation_direction / (LineWidth + LineSpacing));
			int line_index;
			for (line_index = 0; line_index < lines - 1; line_index++)
				drawing_context.DrawRectangle(foreground, null, new Rect(line_index * (LineWidth + LineSpacing), 0, LineWidth, indicator_lenght_in_other_direction));
			drawing_context.DrawRectangle(foreground, null, new Rect(line_index * (LineWidth + LineSpacing), 0, indicator_lenght_in_orientation_direction - (lines - 1) * (LineWidth + LineSpacing), indicator_lenght_in_other_direction));
			drawing_context.Close();
			DrawingBrush result = new DrawingBrush(drawing);
			result.Stretch = Stretch.None;
			result.AlignmentX = AlignmentX.Left;
			result.AlignmentY = AlignmentY.Top;
			return result;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			return null;
		}
	}
}