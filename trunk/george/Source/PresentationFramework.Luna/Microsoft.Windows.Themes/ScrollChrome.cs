using Mono.WindowsPresentationFoundation;
using System;
using System.Windows;
using System.Windows.Media;
#if Implementation
using Microsoft.Windows.Themes;
namespace Mono.Microsoft.Windows.Themes {
#else
namespace Microsoft.Windows.Themes {
#endif
	public sealed class ScrollChrome : FrameworkElement {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty HasOuterBorderProperty = DependencyProperty.Register("HasOuterBorder", typeof(bool), typeof(ScrollChrome), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(ScrollChrome), new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty RenderMouseOverProperty = DependencyProperty.Register("RenderMouseOver", typeof(bool), typeof(ScrollChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty RenderPressedProperty = DependencyProperty.Register("RenderPressed", typeof(bool), typeof(ScrollChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty ScrollGlyphProperty = DependencyProperty.RegisterAttached("ScrollGlyph", typeof(ScrollGlyph), typeof(ScrollChrome), new FrameworkPropertyMetadata(ScrollGlyph.None, FrameworkPropertyMetadataOptions.AffectsRender));
		//TODO: Implement support for the other values.
		public static readonly DependencyProperty ThemeColorProperty = DependencyProperty.Register("ThemeColor", typeof(ThemeColor), typeof(ScrollChrome), new FrameworkPropertyMetadata(ThemeColor.NormalColor, FrameworkPropertyMetadataOptions.AffectsRender));
		#endregion
		#endregion

		//#region Private Fields
		//const double GripperLines = 4;
		//const double GripperLenght = 5;
		//#endregion

		#region Public Constructors
		public ScrollChrome() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public bool HasOuterBorder {
			get { return (bool)GetValue(HasOuterBorderProperty); }
			set { SetValue(HasOuterBorderProperty, value); }
		}

		public Thickness Padding {
			get { return (Thickness)GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}

		public bool RenderMouseOver {
			get { return (bool)GetValue(RenderMouseOverProperty); }
			set { SetValue(RenderMouseOverProperty, value); }
		}

		public bool RenderPressed {
			get { return (bool)GetValue(RenderPressedProperty); }
			set { SetValue(RenderPressedProperty, value); }
		}

		public ThemeColor ThemeColor {
			get { return (ThemeColor)GetValue(ThemeColorProperty); }
			set { SetValue(ThemeColorProperty, value); }
		}
		#endregion
		#endregion

		#region Public Methods
		#region Attached properties
		public static ScrollGlyph GetScrollGlyph(DependencyObject element) {
			return (ScrollGlyph)element.GetValue(ScrollGlyphProperty);
		}

		public static void SetScrollGlyph(DependencyObject element, ScrollGlyph value) {
			element.SetValue(ScrollGlyphProperty, value);
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize) {
			return finalSize;
		}

		protected override Size MeasureOverride(Size availableSize) {
			return new Size(0, 0);
		}

		protected override void OnRender(DrawingContext drawingContext) {
			double actual_width_without_padding = ActualWidth - Padding.Left - Padding.Right;
			if (actual_width_without_padding < 3)
				return;
			double actual_height_without_padding = ActualHeight - Padding.Top - Padding.Bottom;
			ScrollGlyph glyph = GetScrollGlyph(this);
			double glyph_one_pixel_horizontal_offset = glyph == ScrollGlyph.None ? 0 : 1;
			bool has_outer_border = HasOuterBorder;
			if (has_outer_border) {
				if (actual_height_without_padding < 3)
					return;
				drawingContext.DrawRoundedRectangle(null, new Pen(new LinearGradientBrush(new GradientStopCollection(new GradientStop[] { 
				new GradientStop(Color.FromArgb(0, 0xA0, 0xB5, 0xD3), 0), 
				new GradientStop(Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), 0.5), 
				new GradientStop(Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), 1) 
			}), new Point(0, 0), new Point(0, 1)), 1), new Rect(0.5 + Padding.Left + glyph_one_pixel_horizontal_offset, 2.5 + Padding.Top, actual_width_without_padding - 1 - glyph_one_pixel_horizontal_offset, actual_height_without_padding - 3), 3, 3);
			}
			if (actual_width_without_padding < 4 || actual_height_without_padding < 4)
				return;
			double fill_extra_padding = has_outer_border ? 2 : 1;
			double fill_radius = has_outer_border ? 2 : 1.5;
			bool render_pressed = RenderPressed;
			bool render_mouse_over = RenderMouseOver && !RenderPressed;
			drawingContext.DrawRoundedRectangle(new LinearGradientBrush(new GradientStopCollection(render_mouse_over ? new GradientStop[] {
				new GradientStop(Color.FromArgb(0xFF, 0xFD, 0xFF, 0xFF), 0), 
				new GradientStop(Color.FromArgb(0xFF, 0xE2, 0xF3, 0xFD), 0.25), 
				new GradientStop(Color.FromArgb(0xFF, 0xB9, 0xDA, 0xFB), 1) 
			} : new GradientStop[] { 
				new GradientStop(render_pressed ? Color.FromArgb(0xFF, 0x6E, 0x8E, 0xF1) : Color.FromArgb(0xFF, 0xE1, 0xEA, 0xFE), 0), 
				new GradientStop(render_pressed ? Color.FromArgb(0xFF, 0x80, 0x9D, 0xF1) : Color.FromArgb(0xFF, 0xC3, 0xD3, 0xFD), 0.29999999999999999), 
				new GradientStop(render_pressed ? Color.FromArgb(0xFF, 0xAF, 0xBF, 0xED) : Color.FromArgb(0xFF, 0xC3, 0xD3, 0xFD), render_pressed ? 0.69999999999999996 : 0.59999999999999998),
				new GradientStop(render_pressed ? Color.FromArgb(0xFF, 0xD2, 0xDE, 0xEB) : Color.FromArgb(0xFF, 0xBB, 0xCD, 0xF9), 1)
			}), new Point(0, 0), new Point(1, 1)), new Pen(has_outer_border ? Brushes.White : new SolidColorBrush(Color.FromArgb(0xFF, 0xB4, 0xC8, 0xF6)), 1), new Rect(0.5 + Padding.Left + glyph_one_pixel_horizontal_offset, 0.5 + Padding.Top, actual_width_without_padding - fill_extra_padding - glyph_one_pixel_horizontal_offset, actual_height_without_padding - fill_extra_padding), fill_radius, fill_radius);
			if (has_outer_border) {
				if (actual_width_without_padding < 6 || actual_height_without_padding < 6)
					return;
				drawingContext.DrawRoundedRectangle(null, new Pen(new SolidColorBrush(render_pressed ? Color.FromArgb(0xFF, 0x83, 0x8F, 0xDA) : (render_mouse_over ? Color.FromArgb(0xFF, 0x98, 0xB1, 0xE4) : Color.FromArgb(0xFF, 0xB4, 0xC8, 0xF6))), 1), new Rect(1.5 + Padding.Left + glyph_one_pixel_horizontal_offset, 1.5 + Padding.Top, actual_width_without_padding - 4 - glyph_one_pixel_horizontal_offset, actual_height_without_padding - 4), 1.5, 1.5);
			}
			if (glyph != ScrollGlyph.None)
				drawingContext.DrawRectangle(Brushes.Red, null, new Rect(0, 0, 1, 1));
			/*
			bool padded;
			if (Padding.Left != 0 || Padding.Top != 0) {
				padded = true;
				drawingContext.PushTransform(new TranslateTransform(Padding.Left, Padding.Top));
			} else
				padded = false;
			double actual_width_without_padding = ActualWidth - Padding.Left - Padding.Right;
			if (actual_width_without_padding <= 0)
				return;
			double actual_height_without_padding = ActualHeight - Padding.Top - Padding.Bottom;
			ScrollGlyph glyph = GetScrollGlyph(this);
			double glyph_one_pixel_horizontal_offset = glyph == ScrollGlyph.None ? 0 : 1;
			if (actual_height_without_padding <= 0)
				return;
			if (HasOuterBorder) {
				#region Shadow
				double shadow_width = actual_width_without_padding - glyph_one_pixel_horizontal_offset;
				if (shadow_width <= 0)
					return;
				Brush shadow_brush = new LinearGradientBrush(Colors.White, Colors.LightBlue, 45);
				drawingContext.DrawRoundedRectangle(shadow_brush, null, new Rect(glyph_one_pixel_horizontal_offset, 0, shadow_width, actual_height_without_padding), 3, 3);
				#endregion
				#region White
				double white_width = actual_width_without_padding - 1 - glyph_one_pixel_horizontal_offset;
				if (white_width <= 0)
					return;
				double white_height = actual_height_without_padding - 1;
				if (white_height <= 0)
					return;
				drawingContext.DrawRoundedRectangle(Brushes.White, null, new Rect(glyph_one_pixel_horizontal_offset, 0, white_width, white_height), 3, 3);
				#endregion
			}
			#region Interior
			Color interior_color;
			if (RenderPressed)
				interior_color = Colors.Blue;
			else if (RenderMouseOver)
				interior_color = Colors.Aqua;
			else
				interior_color = Colors.LightBlue;

			Brush interior_brush = new LinearGradientBrush(Colors.White, interior_color, 45);
			double interior_width = actual_width_without_padding - 1;
			double interior_height = actual_height_without_padding - 1;
			double outer_border_offset;
			if (HasOuterBorder) {
				interior_width -= 3;
				interior_height -= 3;
				outer_border_offset = 1;
			} else
				outer_border_offset = 0;
			interior_width -= glyph_one_pixel_horizontal_offset;
			if (interior_width <= 0 || interior_height <= 0)
				return;
			drawingContext.DrawRoundedRectangle(interior_brush, RenderPressed ? null : new Pen(Brushes.LightBlue, 1), new Rect(outer_border_offset + 0.5 + glyph_one_pixel_horizontal_offset, outer_border_offset + 0.5, interior_width, interior_height), 1, 1);
			#endregion
			#region Glyph
			const double BorderSize = 4;
			switch (glyph) {
				case ScrollGlyph.UpArrow:
				case ScrollGlyph.DownArrow:
				case ScrollGlyph.LeftArrow:
				case ScrollGlyph.RightArrow:
					const double ArrowWidth = 8;
					const double ArrowLenght = 6;
					double usable_width = actual_width_without_padding - BorderSize;
					double usable_height = actual_height_without_padding - BorderSize;
					double width;
					double height;
					switch (glyph) {
						case ScrollGlyph.LeftArrow:
						case ScrollGlyph.RightArrow:
							width = Math.Min(usable_width, ArrowLenght);
							height = Math.Min(usable_height, ArrowWidth);
							break;
						default:
							width = Math.Min(usable_width, ArrowWidth);
							height = Math.Min(usable_height, ArrowLenght);
							break;
					}
					if (width > 1 && height > 1) {
						width -= 1;
						height -= 1;
						DrawArrow(drawingContext, glyph, new Rect((actual_width_without_padding - width) / 2, (actual_height_without_padding - height) / 2, width, height));
					}
					break;
				case ScrollGlyph.HorizontalGripper:
				case ScrollGlyph.VerticalGripper:
					bool horizontal = glyph == ScrollGlyph.HorizontalGripper;
					if ((horizontal ? actual_width_without_padding : actual_height_without_padding) - 4 > 2 * GripperLines) {
						double used_width, used_height;
						if (horizontal) {
							used_width = 2 * GripperLines;
							used_height = GripperLenght;
						} else {
							used_width = GripperLenght;
							used_height = 2 * GripperLines;
						}
						DrawGripper(drawingContext, horizontal, new Point((actual_width_without_padding - used_width) / 2, (actual_height_without_padding - used_height) / 2));
					}
					break;
			}
			#endregion
			if (padded)
				drawingContext.Pop();
			*/
		}
		#endregion

		//#region Private Methods
		//void DrawArrow(DrawingContext drawingContext, ScrollGlyph glyph, Rect rect) {
		//    Pen pen = new Pen(Brushes.Black, 2);
		//    Point start, middle, end;
		//    switch (glyph) {
		//        case ScrollGlyph.UpArrow:
		//            start = rect.BottomRight;
		//            middle = new Point((rect.Left + rect.Right) / 2, rect.Top);
		//            end = rect.BottomLeft;
		//            break;
		//        default:
		//            start = rect.TopRight;
		//            middle = new Point((rect.Left + rect.Right) / 2, rect.Bottom);
		//            end = rect.TopLeft;
		//            break;
		//        case ScrollGlyph.LeftArrow:
		//            start = rect.TopRight;
		//            middle = new Point(rect.Left, (rect.Top + rect.Bottom) / 2);
		//            end = rect.BottomRight;
		//            break;
		//        case ScrollGlyph.RightArrow:
		//            start = rect.TopLeft;
		//            middle = new Point(rect.Right, (rect.Top + rect.Bottom) / 2);
		//            end = rect.BottomLeft;
		//            break;
		//    }
		//    drawingContext.DrawLine(pen, start, middle);
		//    drawingContext.DrawLine(pen, middle, end);

		//}

		//void DrawGripper(DrawingContext drawingContext, bool horizontal, Point location) {
		//    Pen pen = new Pen(Brushes.White, 1);
		//    Pen shadow_pen = new Pen(Brushes.DarkBlue, 1);
		//    Point shadow_location = new Point(location.X + 1, location.Y + 1);
		//    for (int line = 0; line < GripperLines; line++) {
		//        Utility.DrawLine(drawingContext, !horizontal, pen, location, GripperLenght);
		//        Utility.DrawLine(drawingContext, !horizontal, shadow_pen, shadow_location, GripperLenght);
		//        if (horizontal) {
		//            location.X += 2;
		//            shadow_location.X += 2;
		//        } else {
		//            location.Y += 2;
		//            shadow_location.Y += 2;
		//        }
		//    }
		//}
		//#endregion
	}
}