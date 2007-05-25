//FIXME: Bind to RangeBase, ScrollBar, Slider properties here (as documented), not in those classes.
using Mono.WindowsPresentationFoundation;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	[Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
	public class Track : FrameworkElement {
		#region Public Fields
		#region Dependency Properties
		static public readonly DependencyProperty IsDirectionReversedProperty = DependencyProperty.Register("IsDirectionReversed", typeof(bool), typeof(Track));
		static public readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(Track), new FrameworkPropertyMetadata(1D, FrameworkPropertyMetadataOptions.AffectsArrange));
		static public readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(Track), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsArrange));
		//LAMESPEC: In Windows SDK Feb 2007 the default value is Orientation.Vertical. I have confirmation that it is wrong.
		static public readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Track), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));
		static public readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(Track), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange));
		static public readonly DependencyProperty ViewportSizeProperty = DependencyProperty.Register("ViewportSize", typeof(double), typeof(Track), new PropertyMetadata(double.NaN));

		#endregion
		#endregion

		#region Internal Fields
		internal bool in_scroll_bar;
		#endregion

		#region Private Fields
		#region Parts
		RepeatButton decrease_repeat_button;
		RepeatButton increase_repeat_button;
		Thumb thumb;
		#endregion
		List<Visual> visual_children = new List<Visual>(3);
		#endregion

		#region Public Constructors
		public Track() {
		}

		#endregion

		#region Public Properties
		#region Dependency Properties
		public bool IsDirectionReversed {
			get { return (bool)GetValue(IsDirectionReversedProperty); }
			set { SetValue(IsDirectionReversedProperty, value); }
		}

		public double Maximum {
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}

		public double Minimum {
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}

		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}

		public double Value {
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public double ViewportSize {
			get { return (double)GetValue(ViewportSizeProperty); }
			set { SetValue(ViewportSizeProperty, value); }
		}
		#endregion

		#region Parts
		public RepeatButton DecreaseRepeatButton {
			get { return decrease_repeat_button; }
			set {
				if (decrease_repeat_button == value)
					return;
				if (decrease_repeat_button != null)
					if (in_scroll_bar)
						throw new IndexOutOfRangeException();
					else
						Utility.Hang();
				decrease_repeat_button = value;
				visual_children.Add(decrease_repeat_button);
				AddLogicalChild(decrease_repeat_button);
				AddVisualChild(decrease_repeat_button);
			}
		}

		public RepeatButton IncreaseRepeatButton {
			get { return increase_repeat_button; }
			set {
				if (increase_repeat_button == value)
					return;
				if (increase_repeat_button != null)
					if (in_scroll_bar)
						throw new IndexOutOfRangeException();
					else
						Utility.Hang();
				increase_repeat_button = value;
				visual_children.Add(increase_repeat_button);
				AddLogicalChild(increase_repeat_button);
				AddVisualChild(increase_repeat_button);
			}
		}

		public Thumb Thumb {
			get { return thumb; }
			set {
				if (thumb == value)
					return;
				if (thumb != null)
					if (in_scroll_bar)
						throw new IndexOutOfRangeException();
					else
						Utility.Hang();
				thumb = value;
				visual_children.Add(thumb);
				AddLogicalChild(thumb);
				AddVisualChild(thumb);
			}
		}
		#endregion
		#endregion

		#region Protected Properties
		protected override int VisualChildrenCount {
			get { return visual_children.Count; }
		}
		#endregion

		#region Public Methods
		public virtual double ValueFromDistance(double horizontal, double vertical) {
			bool is_horizontal = Orientation == Orientation.Horizontal;
			double delta = is_horizontal ? horizontal : vertical;
			if (delta == 0)
				return double.NaN;
			double adjustments = (is_horizontal ? 1 : -1) * (IsDirectionReversed ? -1 : 1); 
			if (thumb == null && Parent != null)
				return double.PositiveInfinity * adjustments;
			double available_size;
			if (is_horizontal)
				available_size = ActualWidth - (thumb == null ? new Thumb().Width : thumb.ActualWidth);
			else
				available_size = ActualHeight - (thumb == null ? new Thumb().Height : thumb.ActualHeight);
			return delta / available_size * (Maximum - Minimum) * adjustments;
		}

		public virtual double ValueFromPoint(Point pt) {
			Point mouse_position = Mouse.GetPosition(this);
			double position;
			double track_size;
			if (Orientation == Orientation.Horizontal) {
				position = mouse_position.X;
				track_size = ActualWidth;
			} else {
				position = mouse_position.Y;
				track_size = ActualHeight;
			}
			return position / track_size * (Maximum - Minimum) + Minimum;
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize) {
			if (thumb != null) {
				bool horizontal = Orientation == Orientation.Horizontal;
				#region Ratio
				double ratio = (Value - Minimum) / (Maximum - Minimum);
				if (!horizontal)
					ratio = 1 - ratio;
				if (IsDirectionReversed)
					ratio = 1 - ratio;
				if (double.IsNaN(ratio) || double.IsInfinity(ratio))
					ratio = 0;
				#endregion
				#region Thumb size
				double thumb_size;
				if (TemplatedParent is ScrollBar) {
					if (double.IsNaN(ViewportSize))
						thumb_size = SystemParameters.HorizontalScrollBarThumbWidth / 2D;
					else
						thumb_size = Math.Max((horizontal ? finalSize.Width : finalSize.Height) * ViewportSize / (Maximum - Minimum + ViewportSize), SystemParameters.HorizontalScrollBarThumbWidth / 2D);
				} else
					thumb_size = horizontal ? thumb.DesiredSize.Width : thumb.DesiredSize.Height;
				if (double.IsNaN(thumb_size) || double.IsInfinity(thumb_size))
					thumb_size = horizontal ? thumb.DesiredSize.Width : thumb.DesiredSize.Height;
				Rect thumb_rect;
				#endregion
				if (horizontal)
					thumb_rect = new Rect(ratio * Utility.GetAdjustedSize(finalSize.Width - thumb_size), 0, thumb_size, finalSize.Height);
				else
					thumb_rect = new Rect(0, ratio * Utility.GetAdjustedSize(finalSize.Height - thumb_size), finalSize.Width, thumb_size);
				thumb.Arrange(thumb_rect);
				#region Arrange repeat buttons
				if (horizontal) {
					Rect left_rect = new Rect(0, 0, thumb_rect.Left, finalSize.Height);
					Rect right_rect = new Rect(thumb_rect.Right, 0, finalSize.Width - thumb_rect.Right, finalSize.Height);
					Arrange(decrease_repeat_button, IsDirectionReversed ? right_rect : left_rect);
					Arrange(increase_repeat_button, IsDirectionReversed ? left_rect : right_rect);
				} else {
					Rect top_rect = new Rect(0, 0, finalSize.Width, thumb_rect.Top);
					Rect bottom_rect = new Rect(0, thumb_rect.Bottom, finalSize.Width, finalSize.Height - thumb_rect.Bottom);
					Arrange(decrease_repeat_button, IsDirectionReversed ? top_rect : bottom_rect);
					Arrange(increase_repeat_button, IsDirectionReversed ? bottom_rect: top_rect);
				}
				#endregion
			}
			return finalSize;
		}

		protected override Visual GetVisualChild(int index) {
			if (index < 0 || index > VisualChildrenCount - 1)
				throw new ArgumentOutOfRangeException("index");
			return visual_children[index];
		}

		protected override Size MeasureOverride(Size availableSize) {
			if (thumb == null)
				return new Size(0, 0);
			else {
				thumb.Measure(availableSize);
				if (Orientation == Orientation.Horizontal) {
					return new Size(double.IsInfinity(availableSize.Width) ? thumb.DesiredSize.Width : availableSize.Width, thumb.DesiredSize.Height);
				} else {
					return new Size(thumb.DesiredSize.Width, double.IsInfinity(availableSize.Height) ? thumb.DesiredSize.Height : availableSize.Height);
				}
			}
		}
		#endregion

		#region Private Methods
		void Arrange(RepeatButton repeatButton, Rect rect) {
			if (repeatButton != null)
				repeatButton.Arrange(rect);
		}
		#endregion
	}
}