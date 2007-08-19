//
// ButtonChrome.cs
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
using Mono.WindowsPresentationFoundation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#if Implementation
namespace Mono.Microsoft.Windows.Themes
{
#else
namespace Microsoft.Windows.Themes {
#endif
	public sealed class ButtonChrome : Decorator
	{
		#region Private Constants
		const int BorderSize = 4;
		#endregion

		#region Public Fields
		#region Dependency Properties
		static public readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register ("BorderBrush", typeof (Brush), typeof (ButtonChrome), new FrameworkPropertyMetadata (null, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty FillProperty = DependencyProperty.Register ("Fill", typeof (Brush), typeof (ButtonChrome), new FrameworkPropertyMetadata (null, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty RenderDefaultedProperty = DependencyProperty.Register ("RenderDefaulted", typeof (bool), typeof (ButtonChrome), new FrameworkPropertyMetadata (false, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty RenderMouseOverProperty = DependencyProperty.Register ("RenderMouseOver", typeof (bool), typeof (ButtonChrome), new FrameworkPropertyMetadata (false, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty RenderPressedProperty = DependencyProperty.Register ("RenderPressed", typeof (bool), typeof (ButtonChrome), new FrameworkPropertyMetadata (false, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty ThemeColorProperty = DependencyProperty.Register ("ThemeColor", typeof (ThemeColor), typeof (ButtonChrome));
		#endregion
		#endregion

		#region Public Constructors
		public ButtonChrome ()
		{
		}
		#endregion

		#region Public Properties
		public Brush BorderBrush {
			get { return (Brush)GetValue (BorderBrushProperty); }
			set { SetValue (BorderBrushProperty, value); }
		}

		public Brush Fill {
			get { return (Brush)GetValue (FillProperty); }
			set { SetValue (FillProperty, value); }
		}

		public bool RenderDefaulted {
			get { return (bool)GetValue (RenderDefaultedProperty); }
			set { SetValue (RenderDefaultedProperty, value); }
		}

		public bool RenderMouseOver {
			get { return (bool)GetValue (RenderMouseOverProperty); }
			set { SetValue (RenderMouseOverProperty, value); }
		}

		public bool RenderPressed {
			get { return (bool)GetValue (RenderPressedProperty); }
			set { SetValue (RenderPressedProperty, value); }
		}

		public ThemeColor ThemeColor {
			get { return (ThemeColor)GetValue (ThemeColorProperty); }
			set { SetValue (ThemeColorProperty, value); }
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride (Size arrangeSize)
		{
			if (Child != null)
				Child.Arrange (GetArrangedRect (arrangeSize, Child.DesiredSize, new Thickness (BorderSize), HorizontalAlignment, VerticalAlignment));
			return arrangeSize;
		}

		protected override void OnRender (DrawingContext drawingContext)
		{
			base.OnRender (drawingContext);
			//FIXME? Is this correct?
			double actual_width = double.IsNaN (Width) ? ActualWidth : Width;
			double actual_height = double.IsNaN (Height) ? ActualHeight : Height;
			#region Top left corner shadow
			const double TopLeftCornerShadowPadding = 2D / 3;
			const double TopLeftCornerShadowRadius = 3;
			double width = actual_width - 2 * TopLeftCornerShadowPadding;
			if (width < 0)
				return;
			double height = actual_height - 2 * TopLeftCornerShadowPadding;
			if (height < 0)
				return;
			drawingContext.DrawRoundedRectangle (null, new Pen (new LinearGradientBrush (new GradientStopCollection (new GradientStop [] {
				new GradientStop(Color.FromArgb(0x20, 0x00, 0x00, 0x00), 0),
				new GradientStop(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), 0.5),
				new GradientStop(Color.FromArgb(0x80, 0xFF, 0xFF, 0xFF), 1)
			}), new Point (0, 0), new Point (0.4, 1)), 1.3333333332999999), new Rect (TopLeftCornerShadowPadding, TopLeftCornerShadowPadding, width, height), TopLeftCornerShadowRadius, TopLeftCornerShadowRadius);
			#endregion
			const double FillRadius = 4;
			const double FillPadding = 0.75;
			#region Fill
			Brush fill = Fill;
			if (fill != null || RenderPressed) {
				if ((width = actual_width - 2 * FillPadding) < 0)
					return;
				if ((height = actual_height - 2 * FillPadding) < 0)
					return;
				drawingContext.DrawRoundedRectangle (RenderPressed ? new LinearGradientBrush (new GradientStopCollection (new GradientStop [] {
					new GradientStop(Color.FromArgb(0xFF, 0xE6, 0xE6, 0xE0), 0),
					new GradientStop(Color.FromArgb(0xFF, 0xE2, 0xE2, 0xDA), 1)
				}), new Point (0.5, 1), new Point (0.5, 0)) : fill, null, new Rect (FillPadding, FillPadding, width, height), FillRadius, FillRadius);
			}
			#endregion
			if (RenderPressed) {
				if ((width = actual_width - 2.1) < 0)
					return;
				drawingContext.DrawRoundedRectangle (new LinearGradientBrush (new GradientStopCollection (new GradientStop [] {
					new GradientStop(Color.FromArgb(0xFF, 0x97, 0x8B, 0x72), 1),
					new GradientStop(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), 0.6)
				}), new Point (0.5, 1), new Point (0.5, 0)), null, new Rect (1.05, 1.05, width, 6), FillRadius, FillRadius);
				drawingContext.DrawRoundedRectangle (new LinearGradientBrush (new GradientStopCollection (new GradientStop [] {
					new GradientStop(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), 0.6),
					new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), 1)
				}), new Point (0.5, 0), new Point (0.5, 1)), null, new Rect (1.05, actual_height - 7.05, width, 6), FillRadius, FillRadius);
				drawingContext.DrawRoundedRectangle (new LinearGradientBrush (new GradientStopCollection (new GradientStop [] {
					new GradientStop(Color.FromArgb(0xFF, 0xAA, 0x9D, 0x87), 1),
					new GradientStop(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), 0.6)
				}), new Point (1, 0.5), new Point (0, 0.5)), null, new Rect (1.05, 1.05, 6, actual_height - 2.1), FillRadius, FillRadius);
			} else {
				#region Bottom right corner shadow
				const double BottomRightCornerShadowRadius = 4;
				if ((width = actual_width - 2.1) < 0)
					return;
				if ((height = actual_height - 7.05) < 0)
					return;
				drawingContext.DrawRoundedRectangle (new LinearGradientBrush (new GradientStopCollection (new GradientStop [] {
					new GradientStop(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), 0.5),
					new GradientStop(Color.FromArgb(0x35, 0x59, 0x2F, 0x00), 1)
				}), new Point (0.5, 0), new Point (0.5, 1)), null, new Rect (1.05, height, width, 6), BottomRightCornerShadowRadius, BottomRightCornerShadowRadius);
				if ((width = actual_width - 7.05) < 0)
					return;
				if ((height = actual_height - 2.1) < 0)
					return;
				drawingContext.DrawRoundedRectangle (new LinearGradientBrush (new GradientStopCollection (new GradientStop [] {
					new GradientStop(Color.FromArgb(0x00, 0x59, 0x2F, 0x00), 0.5),
					new GradientStop(Color.FromArgb(0x28, 0x59, 0x2F, 0x00), 1)
				}), new Point (0, 0.5), new Point (1, 0.5)), null, new Rect (width, 1.05, 6, height), BottomRightCornerShadowRadius, BottomRightCornerShadowRadius);
				#endregion
				if (RenderMouseOver || RenderDefaulted) {
					const double MouseOverDefaultedCornerRadius = 3;
					const double MouseOverDefaultedPadding = 2.083333333333333;
					if ((width = actual_width - 2 * MouseOverDefaultedPadding) < 0)
						return;
					if ((height = actual_height - 2 * MouseOverDefaultedPadding) < 0)
						return;
					drawingContext.DrawRoundedRectangle (null, new Pen (new LinearGradientBrush (new GradientStopCollection (RenderMouseOver ? new GradientStop [] {
						new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xF0, 0xCF), 0),
						new GradientStop(Color.FromArgb(0xFF, 0xFC, 0xD2, 0x79), 0.03),
						new GradientStop(Color.FromArgb(0xFF, 0xF8, 0xB7, 0x3B), 0.75),
						new GradientStop(Color.FromArgb(0xFF, 0xE5, 0x97, 0x00), 1)
					} : new GradientStop [] {
						new GradientStop(Color.FromArgb(0xFF, 0xCE, 0xE7, 0xFF), 0),
						new GradientStop(Color.FromArgb(0xFF, 0xBC, 0xD4, 0xF6), 0.3),
						new GradientStop(Color.FromArgb(0xFF, 0x89, 0xAD, 0xE4), 0.97),
						new GradientStop(Color.FromArgb(0xFF, 0x69, 0x82, 0xEE), 1)
					}), new Point (0.5, 0), new Point (0.5, 1)), 2.6666666666999999), new Rect (MouseOverDefaultedPadding, MouseOverDefaultedPadding, width, height), MouseOverDefaultedCornerRadius, MouseOverDefaultedCornerRadius);
				}
			}
			#region Border
			Brush border_brush = BorderBrush;
			if (border_brush != null) {
				const double BorderPadding = 1.25;
				const double BorderRadius = 3;
				if ((width = actual_width - 2 * BorderPadding) < 0)
					return;
				if ((height = actual_height - 2 * BorderPadding) < 0)
					return;
				drawingContext.DrawRoundedRectangle (null, new Pen (border_brush, 1), new Rect (BorderPadding, BorderPadding, width, height), BorderRadius, BorderRadius);
			}
			#endregion
		}

		protected override Size MeasureOverride (Size constraint)
		{
			if (Child == null)
				return new Size (2 * BorderSize, 2 * BorderSize);
			if (double.IsInfinity (constraint.Width) || double.IsInfinity (constraint.Height)) {
				Child.Measure (constraint);
				return Utility.GetSize (Child.DesiredSize, BorderSize, constraint);
			} else {
				Child.Measure (new Size (Utility.GetAdjustedSize (constraint.Width - 2 * BorderSize), Utility.GetAdjustedSize (constraint.Height - 2 * BorderSize)));
				return constraint;
			}
		}
		#endregion

		#region Private Methods
		static Rect GetArrangedRect (Size containerSize, Size objectSize, Thickness borderSize, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
		{
			double available_width = Utility.GetAdjustedSize (containerSize.Width - borderSize.Left - borderSize.Right);
			double available_height = Utility.GetAdjustedSize (containerSize.Height - borderSize.Top - borderSize.Bottom);
			double width = Math.Min (available_width, objectSize.Width);
			double height = Math.Min (available_height, objectSize.Height);
			double left;
			switch (horizontalAlignment) {
			case HorizontalAlignment.Left: left = 0; break;
			case HorizontalAlignment.Center: left = (available_width - width) / 2; break;
			case HorizontalAlignment.Right: left = available_width - width; break;
			default: left = 0; width = available_width; break;
			}
			double top;
			switch (verticalAlignment) {
			case VerticalAlignment.Top: top = 0; break;
			case VerticalAlignment.Center: top = (available_height - height) / 2; break;
			case VerticalAlignment.Bottom: top = available_height - height; break;
			default: top = 0; height = available_height; break;
			}
			return new Rect (left + borderSize.Left, top + borderSize.Top, width, height);
		}
		#endregion
	}
}