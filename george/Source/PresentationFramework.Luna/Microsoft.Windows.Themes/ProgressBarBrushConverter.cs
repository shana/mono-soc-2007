//
// ProgressBarBrushConverter.cs
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
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
#if Implementation
namespace Mono.Microsoft.Windows.Themes
{
#else
namespace Microsoft.Windows.Themes {
#endif
	public class ProgressBarBrushConverter : IMultiValueConverter
	{
		#region Public Methods
		public object Convert (object [] values, Type targetType, object parameter, CultureInfo culture)
		{
			Brush foreground;
			//FIXME: How is it used?
			bool is_indeterminate;
			double indicator_lenght_in_orientation_direction;
			double indicator_lenght_in_other_direction;
			double track_lenght_in_orientation_direction;
			try {
				foreground = (Brush)values [0];
				is_indeterminate = (bool)values [1];
				indicator_lenght_in_orientation_direction = (double)values [2];
				indicator_lenght_in_other_direction = (double)values [3];
				track_lenght_in_orientation_direction = (double)values [4];
			} catch (InvalidCastException) {
				return null;
			}
			const double LineWidth = 6;
			const double LineSpacing = 2;
			DrawingGroup drawing = new DrawingGroup ();
			DrawingContext drawing_context = drawing.Open ();
			int lines = (int)Math.Ceiling (indicator_lenght_in_orientation_direction / (LineWidth + LineSpacing));
			int line_index;
			for (line_index = 0; line_index < lines - 1; line_index++)
				drawing_context.DrawRectangle (foreground, null, new Rect (line_index * (LineWidth + LineSpacing), 0, LineWidth, indicator_lenght_in_other_direction));
			drawing_context.DrawRectangle (foreground, null, new Rect (line_index * (LineWidth + LineSpacing), 0, indicator_lenght_in_orientation_direction - (lines - 1) * (LineWidth + LineSpacing), indicator_lenght_in_other_direction));
			drawing_context.Close ();
			DrawingBrush result = new DrawingBrush (drawing);
			result.Stretch = Stretch.None;
			result.Viewbox = new Rect (new Size (indicator_lenght_in_orientation_direction, indicator_lenght_in_other_direction));
			result.Viewport = result.Viewbox;
			return result;
		}

		public object [] ConvertBack (object value, Type [] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
		#endregion
	}
}