//
// ThreeDElement.cs: A <see cref="Decorator"/> that draws a 3D border 
// around its content.
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Mono.WindowsPresentationFoundation
{
	/// <summary>
	/// A <see cref="Decorator"/> that draws a 3D border around its content.
	/// </summary>
	public class ThreeDElement : Decorator
	{
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty BackgroundBrushProperty = DependencyProperty.Register ("BackgroundBrush", typeof (Brush), typeof (ThreeDElement), new FrameworkPropertyMetadata (VisualParameters.ThreeDBackgroundBrush, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty BorderDarkBrushProperty = DependencyProperty.Register ("BorderDarkBrush", typeof (Brush), typeof (ThreeDElement), new FrameworkPropertyMetadata (VisualParameters.ThreeDBorderDarkBrush, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty BorderLightBrushProperty = DependencyProperty.Register ("BorderLightBrush", typeof (Brush), typeof (ThreeDElement), new FrameworkPropertyMetadata (VisualParameters.ThreeDBorderLightBrush, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register ("BorderThickness", typeof (double), typeof (ThreeDElement), new FrameworkPropertyMetadata (VisualParameters.ThreeDBorderThickness, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty FlatBorderBrushProperty = DependencyProperty.Register ("FlatBorderBrush", typeof (Brush), typeof (ThreeDElement), new FrameworkPropertyMetadata (VisualParameters.ThreeFlatBorderBrush, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty HighlightBorderBrushProperty = DependencyProperty.Register ("HighlightBorderBrush", typeof (Brush), typeof (ThreeDElement), new FrameworkPropertyMetadata (null, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty ThreeDStyleProperty = DependencyProperty.Register ("ThreeDStyle", typeof (ThreeDStyle), typeof (ThreeDElement), new FrameworkPropertyMetadata (ThreeDStyle.Raised, FrameworkPropertyMetadataOptions.AffectsRender));
		#endregion
		#endregion

		#region Public Properties
		#region Dependency Properties
		public Brush BackgroundBrush {
			get { return (Brush)GetValue (BackgroundBrushProperty); }
			set { SetValue (BackgroundBrushProperty, value); }
		}

		public Brush BorderDarkBrush {
			get { return (Brush)GetValue (BorderDarkBrushProperty); }
			set { SetValue (BorderDarkBrushProperty, value); }
		}

		public Brush BorderLightBrush {
			get { return (Brush)GetValue (BorderLightBrushProperty); }
			set { SetValue (BorderLightBrushProperty, value); }
		}

		public double BorderThickness {
			get { return (double)GetValue (BorderThicknessProperty); }
			set { SetValue (BorderThicknessProperty, value); }
		}

		public Brush FlatBorderBrush {
			get { return (Brush)GetValue (FlatBorderBrushProperty); }
			set { SetValue (FlatBorderBrushProperty, value); }
		}

		public Brush HighlightBorderBrush {
			get { return (Brush)GetValue (HighlightBorderBrushProperty); }
			set { SetValue (HighlightBorderBrushProperty, value); }
		}

		public ThreeDStyle ThreeDStyle {
			get { return (ThreeDStyle)GetValue (ThreeDStyleProperty); }
			set { SetValue (ThreeDStyleProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride (Size arrangeSize)
		{
			return Utility.ArrangeDecoratorChild (Child, arrangeSize, 2 * BorderThickness);
		}

		protected override Size MeasureOverride (Size constraint)
		{
			return Utility.MeasureDecoratorChild (Child, constraint, 2 * BorderThickness);
		}

		protected override void OnRender (DrawingContext drawingContext)
		{
			base.OnRender (drawingContext);
			double actual_width = ActualWidth;
			double actual_height = ActualHeight;
			double actual_border_thickness = Utility.GetActualDecoratorBorderThickness (BorderThickness, actual_width, actual_height);
			#region Border
			Brush top_left_border_brush;
			Brush bottom_right_border_brush;
			bool draw_three_d_border;
			switch (ThreeDStyle) {
			case ThreeDStyle.Flat:
				Brush flat_border_brush = FlatBorderBrush;
				if (flat_border_brush != null)
					drawingContext.DrawRectangle (null, new Pen (flat_border_brush, actual_border_thickness), new Rect (actual_border_thickness / 2, actual_border_thickness / 2, actual_width - actual_border_thickness, actual_height - actual_border_thickness));
				top_left_border_brush = null;
				bottom_right_border_brush = null;
				draw_three_d_border = false;
				break;
			case ThreeDStyle.Raised:
				top_left_border_brush = BorderLightBrush;
				bottom_right_border_brush = BorderDarkBrush;
				draw_three_d_border = true;
				break;
			default:
				top_left_border_brush = BorderDarkBrush;
				bottom_right_border_brush = BorderLightBrush;
				draw_three_d_border = true;
				break;
			}
			if (draw_three_d_border) {
				StreamGeometry stream_geometry;
				StreamGeometryContext border_geometry_context;
				if (top_left_border_brush != null) {
					stream_geometry = new StreamGeometry ();
					using (border_geometry_context = stream_geometry.Open ()) {
						border_geometry_context.BeginFigure (new Point (), true, true);
						border_geometry_context.LineTo (new Point (actual_width, 0), false, false);
						border_geometry_context.LineTo (new Point (actual_width - actual_border_thickness, actual_border_thickness), false, false);
						border_geometry_context.LineTo (new Point (actual_border_thickness, actual_border_thickness), false, false);
						border_geometry_context.LineTo (new Point (actual_border_thickness, actual_height - actual_border_thickness), false, false);
						border_geometry_context.LineTo (new Point (0, actual_height), false, false);
						border_geometry_context.LineTo (new Point (), false, false);
					}
					stream_geometry.Freeze ();
					drawingContext.DrawGeometry (top_left_border_brush, null, stream_geometry);
				}
				if (bottom_right_border_brush != null) {
					stream_geometry = new StreamGeometry ();
					using (border_geometry_context = stream_geometry.Open ()) {
						border_geometry_context.BeginFigure (new Point (actual_width, 0), true, true);
						border_geometry_context.LineTo (new Point (actual_width, actual_height), false, false);
						border_geometry_context.LineTo (new Point (0, actual_height), false, false);
						border_geometry_context.LineTo (new Point (actual_border_thickness, actual_height - actual_border_thickness), false, false);
						border_geometry_context.LineTo (new Point (actual_width - actual_border_thickness, actual_height - actual_border_thickness), false, false);
						border_geometry_context.LineTo (new Point (actual_width - actual_border_thickness, actual_border_thickness), false, false);
						border_geometry_context.LineTo (new Point (actual_width, 0), false, false);
					}
					stream_geometry.Freeze ();
					drawingContext.DrawGeometry (bottom_right_border_brush, null, stream_geometry);
				}
			}
			#endregion
			#region Backround
			Brush background_brush = BackgroundBrush;
			Rect background_rect = Utility.GetArrangeRectToPassToDecoratorChild (new Size (actual_width, actual_height), actual_border_thickness);
			if (background_brush != null)
				drawingContext.DrawRectangle (background_brush, null, background_rect);
			#endregion
			#region Highlight border
			Brush highlight_border_brush = HighlightBorderBrush;
			if (highlight_border_brush != null) {
				Rect highlight_border_rect = background_rect;
				double highlight_border_thickness = Utility.GetActualDecoratorBorderThickness (BorderThickness, highlight_border_rect.Width, highlight_border_rect.Height);
				highlight_border_rect.Inflate (-highlight_border_thickness / 2, -highlight_border_thickness / 2);
				drawingContext.DrawRectangle (null, new Pen (highlight_border_brush, highlight_border_thickness), highlight_border_rect);
			}
			#endregion
		}
		#endregion
	}
}