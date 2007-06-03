using Mono.WindowsPresentationFoundation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#if Implementation
using Microsoft.Windows.Themes;
namespace Mono.Microsoft.Windows.Themes {
#else
namespace Microsoft.Windows.Themes {
#endif
	public sealed class ButtonChrome : Decorator {
		#region Private Constants
		const int BorderSize = 4;
		#endregion

		#region Dependency Property Fields
		static public readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(ButtonChrome), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(ButtonChrome), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty RenderDefaultedProperty = DependencyProperty.Register("RenderDefaulted", typeof(bool), typeof(ButtonChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty RenderMouseOverProperty = DependencyProperty.Register("RenderMouseOver", typeof(bool), typeof(ButtonChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty RenderPressedProperty = DependencyProperty.Register("RenderPressed", typeof(bool), typeof(ButtonChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		static public readonly DependencyProperty ThemeColorProperty = DependencyProperty.Register("ThemeColor", typeof(ThemeColor), typeof(ButtonChrome));
		#endregion

		#region Public Constructors
		public ButtonChrome() {
			//WDTDH
		}
		#endregion

		#region Public Properties
		public Brush BorderBrush {
			get { return (Brush)GetValue(BorderBrushProperty); }
			set { SetValue(BorderBrushProperty, value); }
		}

		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}

		public bool RenderDefaulted {
			get { return (bool)GetValue(RenderDefaultedProperty); }
			set { SetValue(RenderDefaultedProperty, value); }
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

		#region Protected Methods
		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			const double BorderThickness = 1.1;
			const double BorderPosition = 1.3;
			const double BorderRoundCorderRadius = BorderThickness + BorderPosition;
			const double VisualCueThickness = 2;
			double width = ActualWidth - 2 * BorderPosition;
			if (width < 0)
				return;
			double height = ActualHeight - 2 * BorderPosition;
			if (height < 0)
				return;
			Rect border_rect = new Rect(BorderPosition, BorderPosition, width, height);
			drawingContext.DrawRoundedRectangle(RenderPressed ? SystemColors.ControlLightBrush : Fill, null, border_rect, BorderRoundCorderRadius, BorderRoundCorderRadius);
			if (!RenderPressed && (RenderDefaulted || RenderMouseOver)) {
				width = border_rect.Width - 2 * BorderThickness;
				if (width > 0) {
					height = border_rect.Height - 2 * BorderThickness;
					if (height > 0)
						drawingContext.DrawRoundedRectangle(null, new Pen(RenderMouseOver ? Brushes.Orange : Brushes.LightBlue, VisualCueThickness), new Rect(border_rect.Left + BorderThickness, border_rect.Top + BorderThickness, width, height), BorderRoundCorderRadius, BorderRoundCorderRadius);
				}
			}
			drawingContext.DrawRoundedRectangle(null, new Pen(BorderBrush, BorderThickness), border_rect, BorderRoundCorderRadius, BorderRoundCorderRadius);
		}

		protected override Size ArrangeOverride(Size arrangeSize) {
			if (Child != null)
				Child.Arrange(GetArrangedRect(arrangeSize, Child.DesiredSize, new Thickness(BorderSize), HorizontalAlignment, VerticalAlignment));
			return arrangeSize;
		}

		//FIXME?: This probably needs more work.
		protected override Size MeasureOverride(Size constraint) {
			if (TemplatedParent != null && ((ContentControl)TemplatedParent).Content == null)
				return new Size(0, 0);
			if (double.IsInfinity(constraint.Width) || double.IsInfinity(constraint.Height)) {
				return Utility.GetSize(base.MeasureOverride(constraint), BorderSize, constraint);
			} else {
				if (Child != null)
					Child.Measure(new Size(Utility.GetAdjustedSize(constraint.Width - 2 * BorderSize), Utility.GetAdjustedSize(constraint.Height - 2 * BorderSize)));
				return new Size(2 * BorderSize, 2 * BorderSize);
			}
		}
		#endregion

		#region Private Methods
		static Rect GetArrangedRect(Size containerSize, Size objectSize, Thickness borderSize, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment) {
			double available_width = Utility.GetAdjustedSize(containerSize.Width - borderSize.Left - borderSize.Right);
			double available_height = Utility.GetAdjustedSize(containerSize.Height - borderSize.Top - borderSize.Bottom);
			double width = Math.Min(available_width, objectSize.Width);
			double height = Math.Min(available_height, objectSize.Height);
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
			return new Rect(left + borderSize.Left, top + borderSize.Top, width, height);
		}
        #endregion
	}
}