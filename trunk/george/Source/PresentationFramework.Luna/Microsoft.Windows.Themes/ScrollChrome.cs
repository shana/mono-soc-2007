using Mono.WindowsPresentationFoundation;
using System;
using System.Windows;
using System.Windows.Media;
#if Implementation
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
			//TODO: Lookup colors.
			double actual_width_without_padding = ActualWidth - Padding.Left - Padding.Right;
			double actual_height_without_padding = ActualHeight - Padding.Top - Padding.Bottom;
			ScrollGlyph glyph = GetScrollGlyph(this);
			double glyph_one_pixel_horizontal_padding = (glyph == ScrollGlyph.DownArrow || glyph == ScrollGlyph.UpArrow || glyph == ScrollGlyph.VerticalGripper) ? 1 : 0;
			double glyph_one_pixel_vertical_padding = (glyph == ScrollGlyph.LeftArrow || glyph == ScrollGlyph.RightArrow || glyph == ScrollGlyph.HorizontalGripper) ? 1 : 0;
			bool has_outer_border = HasOuterBorder;
			if (has_outer_border) {
				if (actual_width_without_padding - glyph_one_pixel_horizontal_padding < 3)
					return;
				if (actual_height_without_padding - glyph_one_pixel_vertical_padding < 3)
					return;
				drawingContext.DrawRoundedRectangle(null, new Pen(new LinearGradientBrush(new GradientStopCollection(new GradientStop[] { 
					new GradientStop(Color.FromArgb(0, 0xA0, 0xB5, 0xD3), 0), 
					new GradientStop(Color.FromArgb(0xFF, 0xA0, 0xB5, 0xD3), 0.5), 
					new GradientStop(Color.FromArgb(0xFF, 0X7C, 0x9F, 0xD3), 1) 
				}), new Point(0, 0), new Point(0, 1)), 1), new Rect(0.5 + Padding.Left + glyph_one_pixel_horizontal_padding, 2.5 + Padding.Top + glyph_one_pixel_vertical_padding, actual_width_without_padding - 1 - glyph_one_pixel_horizontal_padding, actual_height_without_padding - 3 - glyph_one_pixel_vertical_padding), 3, 3);
			}
			if (actual_width_without_padding < 4 || actual_height_without_padding < 4)
				return;
			double fill_extra_padding = has_outer_border ? 2 : 1;
			double fill_radius = has_outer_border ? 2 : 1.5;
			bool render_pressed = RenderPressed;
			bool render_mouse_over = RenderMouseOver && !RenderPressed;
			drawingContext.DrawRoundedRectangle(new LinearGradientBrush(new GradientStopCollection(glyph == ScrollGlyph.HorizontalGripper || glyph == ScrollGlyph.VerticalGripper ? new GradientStop[] {
				new GradientStop(render_pressed ? Color.FromArgb(0xFF, 0xA8, 0xBE, 0xF5) : (render_mouse_over ? Color.FromArgb(0xFF, 0xDA, 0xE9, 0xFF) : Color.FromArgb(0xFF, 0xC9, 0xD8, 0xFC)), 0), 
				new GradientStop(render_pressed ? Color.FromArgb(0xFF, 0xA1, 0xBD, 0xFA) : (render_mouse_over ? Color.FromArgb(0xFF, 0xD4, 0xE6, 0xFF) : Color.FromArgb(0xFF, 0xC2, 0xD3, 0xFC)), 0.65000000000000002), 
				new GradientStop(render_pressed ? Color.FromArgb(0xFF, 0x98, 0xB0, 0xEE) : (render_mouse_over ? Color.FromArgb(0xFF, 0xCA, 0xE0, 0xFF) : Color.FromArgb(0xFF, 0xB6, 0xCD, 0xFB)), 1) 
			} : (render_mouse_over ? new GradientStop[] {
				new GradientStop(Color.FromArgb(0xFF, 0xFD, 0xFF, 0xFF), 0), 
				new GradientStop(Color.FromArgb(0xFF, 0xE2, 0xF3, 0xFD), 0.25), 
				new GradientStop(Color.FromArgb(0xFF, 0xB9, 0xDA, 0xFB), 1) 
			} : new GradientStop[] { 
				new GradientStop(render_pressed ? Color.FromArgb(0xFF, 0x6E, 0x8E, 0xF1) : Color.FromArgb(0xFF, 0xE1, 0xEA, 0xFE), 0), 
				new GradientStop(render_pressed ? Color.FromArgb(0xFF, 0x80, 0x9D, 0xF1) : Color.FromArgb(0xFF, 0xC3, 0xD3, 0xFD), 0.29999999999999999), 
				new GradientStop(render_pressed ? Color.FromArgb(0xFF, 0xAF, 0xBF, 0xED) : Color.FromArgb(0xFF, 0xC3, 0xD3, 0xFD), render_pressed ? 0.69999999999999996 : 0.59999999999999998),
				new GradientStop(render_pressed ? Color.FromArgb(0xFF, 0xD2, 0xDE, 0xEB) : Color.FromArgb(0xFF, 0xBB, 0xCD, 0xF9), 1)
			})), new Point(0, 0), new Point(glyph == ScrollGlyph.HorizontalGripper ? 0 : 1, glyph == ScrollGlyph.VerticalGripper ? 0 : 1)), new Pen(has_outer_border ? Brushes.White : new SolidColorBrush(Color.FromArgb(0xFF, 0xB4, 0xC8, 0xF6)), 1), new Rect(0.5 + Padding.Left + glyph_one_pixel_horizontal_padding, 0.5 + Padding.Top + glyph_one_pixel_vertical_padding, actual_width_without_padding - fill_extra_padding - glyph_one_pixel_horizontal_padding, actual_height_without_padding - fill_extra_padding - glyph_one_pixel_vertical_padding), fill_radius, fill_radius);
			if (has_outer_border) {
				if (actual_width_without_padding < 6 || actual_height_without_padding < 6)
					return;
				Color color;
				if ((glyph == ScrollGlyph.HorizontalGripper || glyph == ScrollGlyph.VerticalGripper) && render_mouse_over)
					color = Color.FromArgb(0xFF, 0xAC, 0xCE, 0xFF);
				else {
					if (render_pressed)
						color = Color.FromArgb(0xFF, 0x83, 0x8F, 0xDA);
					else if (render_mouse_over)
						color = Color.FromArgb(0xFF, 0x98, 0xB1, 0xE4);
					else
						color = Color.FromArgb(0xFF, 0xB4, 0xC8, 0xF6);
				}
				drawingContext.DrawRoundedRectangle(null, new Pen(new SolidColorBrush(color), 1), new Rect(1.5 + Padding.Left + glyph_one_pixel_horizontal_padding, 1.5 + Padding.Top + glyph_one_pixel_vertical_padding, actual_width_without_padding - 4 - glyph_one_pixel_horizontal_padding, actual_height_without_padding - 4 - glyph_one_pixel_vertical_padding), 1.5, 1.5);
			}
			switch (glyph) {
			case ScrollGlyph.DownArrow:
			case ScrollGlyph.LeftArrow:
			case ScrollGlyph.RightArrow:
			case ScrollGlyph.UpArrow:
				Point start_point;
				Point segment_point_1;
				Point segment_point_2;
				Point segment_point_3;
				Point segment_point_4;
				Point segment_point_5;
				switch (glyph) {
				case ScrollGlyph.DownArrow:
					start_point = new Point(0, 3.5);
					segment_point_1 = new Point(4.5, 8);
					segment_point_2 = new Point(9, 3.5);
					segment_point_3 = new Point(7.5, 2);
					segment_point_4 = new Point(4.5, 5);
					segment_point_5 = new Point(1.5, 2);
					break;
				case ScrollGlyph.LeftArrow:
					start_point = new Point(4.5, 0);
					segment_point_1 = new Point(0, 4.5);
					segment_point_2 = new Point(4.5, 9);
					segment_point_3 = new Point(6, 7.5);
					segment_point_4 = new Point(3, 4.5);
					segment_point_5 = new Point(6, 1.5);
					break;
				case ScrollGlyph.RightArrow:
					start_point = new Point(3.5, 0);
					segment_point_1 = new Point(8, 4.5);
					segment_point_2 = new Point(3.5, 9);
					segment_point_3 = new Point(2, 7.5);
					segment_point_4 = new Point(5, 4.5);
					segment_point_5 = new Point(2, 1.5);
					break;
				default:
					start_point = new Point(0, 4.5);
					segment_point_1 = new Point(4.5, 0);
					segment_point_2 = new Point(9, 4.5);
					segment_point_3 = new Point(7.5, 6);
					segment_point_4 = new Point(4.5, 3);
					segment_point_5 = new Point(1.5, 6);
					break;
				}
				drawingContext.PushTransform(new MatrixTransform(1, 0, 0, 1, (actual_width_without_padding - 9) / 2, (actual_height_without_padding - 9) / 2));
				drawingContext.DrawGeometry(new SolidColorBrush(Color.FromArgb(0xFF, 0x4D, 0x61, 0x85)), null, new PathGeometry(new PathFigure[] { new PathFigure(start_point, new PathSegment[] {
					new LineSegment(segment_point_1, true),
					new LineSegment(segment_point_2, true),
					new LineSegment(segment_point_3, true),
					new LineSegment(segment_point_4, true),
					new LineSegment(segment_point_5, true)
				}, true) }, FillRule.EvenOdd, Transform.Identity));
				drawingContext.Pop();
				break;
			case ScrollGlyph.HorizontalGripper:
			case ScrollGlyph.VerticalGripper:
				const double MinimumSizeInScrollDirection = 14;
				bool horizontal = glyph == ScrollGlyph.HorizontalGripper;
				if ((horizontal ? actual_width_without_padding : actual_height_without_padding) <= MinimumSizeInScrollDirection)
					return;
				const double MinimumSizeInOtherDirection = 9;
				if ((horizontal ? actual_height_without_padding : actual_width_without_padding) <= MinimumSizeInOtherDirection)
					return;
				Color gripper_color;
				if (render_pressed)
					gripper_color = Color.FromArgb(0xFF, 0xCF, 0xDD, 0xFD);
				else if (render_mouse_over)
					gripper_color = Color.FromArgb(0xFF, 0xFC, 0xFD, 0xFF);
				else
					gripper_color = Color.FromArgb(0xFF, 0xEE, 0xF4, 0xFE);
				Brush gripper_brush = new SolidColorBrush(gripper_color);
				Color gripper_shadow_color;
				if (render_pressed)
					gripper_shadow_color = Color.FromArgb(0xFF, 0x83, 0x9E, 0xD8);
				else if (render_mouse_over)
					gripper_shadow_color = Color.FromArgb(0xFF, 0x9C, 0xC5, 0xFF);
				else
					gripper_shadow_color = Color.FromArgb(0xFF, 0x8C, 0xB0, 0xF8);
				Brush gripper_shadow_brush = new SolidColorBrush(gripper_shadow_color);
				for (int gripper_line_index = 0; gripper_line_index < 4; gripper_line_index++) {
					drawingContext.DrawRectangle(gripper_brush, null, horizontal ? new Rect((actual_width_without_padding - 9) / 2 + gripper_line_index * 2, (actual_height_without_padding - 6) / 2, 1, 5) : new Rect((actual_width_without_padding - 6) / 2, (actual_height_without_padding - 9) / 2 + gripper_line_index * 2, 5, 1));
					drawingContext.DrawRectangle(gripper_shadow_brush, null, horizontal ? new Rect((actual_width_without_padding - 9) / 2 + gripper_line_index * 2 + 1, (actual_height_without_padding - 6) / 2 + 1, 1, 5) : new Rect((actual_width_without_padding - 6) / 2 + 1, (actual_height_without_padding - 9) / 2 + gripper_line_index * 2 + 1, 5, 1));
				}
				break;
			}
		}
		#endregion
	}
}