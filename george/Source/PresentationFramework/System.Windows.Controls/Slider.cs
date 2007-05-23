using Mono.WindowsPresentationFoundation;
using System.ComponentModel;
using System.Text;
using System.Windows.Automation.Peers;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using Mono.System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls {
#else
using System.Windows.Controls.Primitives;
namespace System.Windows.Controls {
#endif
	[Localizability(LocalizationCategory.Ignore)]
	[TemplatePart(Name = "PART_Track", Type = typeof(Track))]
	[TemplatePart(Name = "PART_SelectionRange", Type = typeof(FrameworkElement))]
	public class Slider : global::System.Windows.Controls.Primitives.RangeBase {
		#region Dependency Property Fields
		public static readonly DependencyProperty AutoToolTipPlacementProperty = DependencyProperty.Register("AutoToolTipPlacement", typeof(global::System.Windows.Controls.Primitives.AutoToolTipPlacement), typeof(Slider), new PropertyMetadata(delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Slider)d).SetUpAutoToolTip();
		}));
		public static readonly DependencyProperty AutoToolTipPrecisionProperty = DependencyProperty.Register("AutoToolTipPrecision", typeof(int), typeof(Slider), null, ValidateNonNegativeInteger);
		public static readonly DependencyProperty DelayProperty = DependencyProperty.Register("Delay", typeof(int), typeof(Slider), new PropertyMetadata(Utility.GetSystemDelay()), ValidateNonNegativeInteger);
		public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register("Interval", typeof(int), typeof(Slider), new PropertyMetadata(Utility.GetSystemInterval()), ValidateNonNegativeInteger);
		public static readonly DependencyProperty IsDirectionReversedProperty = DependencyProperty.Register("IsDirectionReversed", typeof(bool), typeof(Slider));
		public static readonly DependencyProperty IsMoveToPointEnabledProperty = DependencyProperty.Register("IsMoveToPointEnabled", typeof(bool), typeof(Slider));
		public static readonly DependencyProperty IsSelectionRangeEnabledProperty = DependencyProperty.Register("IsSelectionRangeEnabled", typeof(bool), typeof(Slider));
		public static readonly DependencyProperty IsSnapToTickEnabledProperty = DependencyProperty.Register("IsSnapToTickEnabled", typeof(bool), typeof(Slider));
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Slider));
		public static readonly DependencyProperty SelectionEndProperty = DependencyProperty.Register("SelectionEnd", typeof(double), typeof(Slider), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Slider instance = (Slider)d;
			if (!instance.adjusting_properties)
				instance.requested_selection_end = (double)e.NewValue;
			instance.AdjustSelectionRange();
		}), ValidateNonInfinite);
		public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register("SelectionStart", typeof(double), typeof(Slider), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Slider instance = (Slider)d;
			if (!instance.adjusting_properties)
				instance.requested_selection_start = (double)e.NewValue;
			instance.AdjustSelectionRange();
		}), ValidateNonInfinite);
		public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register("TickFrequency", typeof(double), typeof(Slider), new PropertyMetadata(1D));
		public static readonly DependencyProperty TickPlacementProperty = DependencyProperty.Register("TickPlacement", typeof(global::System.Windows.Controls.Primitives.TickPlacement), typeof(Slider));
		public static readonly DependencyProperty TicksProperty = DependencyProperty.Register("Ticks", typeof(DoubleCollection), typeof(Slider), new FrameworkPropertyMetadata());

		#endregion

		#region Private Fields
		#region Commands
		static RoutedCommand decrease_large = new RoutedCommand("DecreaseLarge", typeof(Slider));
		static RoutedCommand decrease_small = new RoutedCommand("DecreaseSmall", typeof(Slider));
		static RoutedCommand increase_large = new RoutedCommand("IncreaseLarge", typeof(Slider));
		static RoutedCommand increase_small = new RoutedCommand("IncreaseSmall", typeof(Slider));
		static RoutedCommand maximize_value = new RoutedCommand("MaximizeValue", typeof(Slider));
		static RoutedCommand minimize_value = new RoutedCommand("MinimizeValue", typeof(Slider));
		#endregion
		/// <summary>
		/// Whether properties are being adjusted.
		/// </summary>
		/// <remarks>
		/// When this is <code>true</code>, the requested property values should not be changed.
		/// </remarks>
		bool adjusting_properties;
		#region Requested property values
		double requested_selection_start;
		double requested_selection_end;
		#endregion
		ToolTip auto_tool_tip;
		Track track;
		#endregion

		#region Static Constructor
		static Slider() {
			Theme.Load();
			#region Command bindings
			Type type = typeof(Slider);
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(DecreaseLarge, delegate(object sender, ExecutedRoutedEventArgs e) {
				((Slider)sender).OnDecreaseLarge();
			}, delegate(object sender, CanExecuteRoutedEventArgs e) {
				Slider i = (Slider)sender;
				e.CanExecute = i.Value - i.LargeChange >= i.Minimum;
			}));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(DecreaseSmall, delegate(object sender, ExecutedRoutedEventArgs e) {
				((Slider)sender).OnDecreaseSmall();
			}, delegate(object sender, CanExecuteRoutedEventArgs e) {
				Slider i = (Slider)sender;
				e.CanExecute = i.Value - i.SmallChange >= i.Minimum;
			}));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(IncreaseLarge, delegate(object sender, ExecutedRoutedEventArgs e) {
				((Slider)sender).OnIncreaseLarge();
			}, delegate(object sender, CanExecuteRoutedEventArgs e) {
				Slider i = (Slider)sender;
				e.CanExecute = i.Value + i.LargeChange <= i.Maximum;
			}));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(IncreaseSmall, delegate(object sender, ExecutedRoutedEventArgs e) {
				((Slider)sender).OnIncreaseSmall();
			}, delegate(object sender, CanExecuteRoutedEventArgs e) {
				Slider i = (Slider)sender;
				e.CanExecute = i.Value + i.SmallChange <= i.Maximum;
			}));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(MaximizeValue, delegate(object sender, ExecutedRoutedEventArgs e) {
				((Slider)sender).OnMaximizeValue();
			}, delegate(object sender, CanExecuteRoutedEventArgs e) {
				e.CanExecute = true;
			}));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(MinimizeValue, delegate(object sender, ExecutedRoutedEventArgs e) {
				((Slider)sender).OnMinimizeValue();
			}, delegate(object sender, CanExecuteRoutedEventArgs e) {
				e.CanExecute = true;
			}));
			#endregion
		}
		#endregion

		#region Public Constructors
		public Slider() {
			Maximum = 10;
			//FIXME?: Should I do this using CommandManager.RegisterClassInputBinding?
			KeyDown += delegate(object sender, KeyEventArgs e) {
				if (e.KeyboardDevice.Modifiers == ModifierKeys.None) {
					if (e.Key == Key.Left || e.Key == Key.Down) {
						OnDecreaseSmall();
						e.Handled = true;
					} else if (e.Key == Key.Right || e.Key == Key.Up) {
						OnIncreaseSmall();
						e.Handled = true;
					}
				}
			};
		}
		#endregion

		#region Dependency Properties
		[Bindable(true)]
		public global::System.Windows.Controls.Primitives.AutoToolTipPlacement AutoToolTipPlacement {
			get { return (global::System.Windows.Controls.Primitives.AutoToolTipPlacement)GetValue(AutoToolTipPlacementProperty); }
			set { SetValue(AutoToolTipPlacementProperty, value); }
		}

		[Bindable(true)]
		public int AutoToolTipPrecision {
			get { return (int)GetValue(AutoToolTipPrecisionProperty); }
			set { SetValue(AutoToolTipPrecisionProperty, value); }
		}

		[Bindable(true)]
		public int Delay {
			get { return (int)GetValue(DelayProperty); }
			set { SetValue(DelayProperty, value); }
		}

		[Bindable(true)]
		public int Interval {
			get { return (int)GetValue(IntervalProperty); }
			set { SetValue(IntervalProperty, value); }
		}

		[Bindable(true)]
		public bool IsDirectionReversed {
			get { return (bool)GetValue(IsDirectionReversedProperty); }
			set { SetValue(IsDirectionReversedProperty, value); }
		}

		[Bindable(true)]
		public bool IsMoveToPointEnabled {
			get { return (bool)GetValue(IsMoveToPointEnabledProperty); }
			set { SetValue(IsMoveToPointEnabledProperty, value); }
		}

		[Bindable(true)]
		public bool IsSelectionRangeEnabled {
			get { return (bool)GetValue(IsSelectionRangeEnabledProperty); }
			set { SetValue(IsSelectionRangeEnabledProperty, value); }
		}

		[Bindable(true)]
		public bool IsSnapToTickEnabled {
			get { return (bool)GetValue(IsSnapToTickEnabledProperty); }
			set { SetValue(IsSnapToTickEnabledProperty, value); }
		}

		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}

		[Bindable(true)]
		public double SelectionEnd {
			get { return (double)GetValue(SelectionEndProperty); }
			set { SetValue(SelectionEndProperty, value); }
		}

		[Bindable(true)]
		public double SelectionStart {
			get { return (double)GetValue(SelectionStartProperty); }
			set { SetValue(SelectionStartProperty, value); }
		}

		[Bindable(true)]
		public double TickFrequency {
			get { return (double)GetValue(TickFrequencyProperty); }
			set { SetValue(TickFrequencyProperty, value); }
		}

		[Bindable(true)]
		public global::System.Windows.Controls.Primitives.TickPlacement TickPlacement {
			get { return (global::System.Windows.Controls.Primitives.TickPlacement)GetValue(TickPlacementProperty); }
			set { SetValue(TickPlacementProperty, value); }
		}

		[Bindable(true)]
		public DoubleCollection Ticks {
			get { return (DoubleCollection)GetValue(TicksProperty); }
			set { SetValue(TicksProperty, value); }
		}
		#endregion

		#region Public Static Properties
		#region Commands
		public static RoutedCommand DecreaseLarge {
			get { return decrease_large; }
		}

		public static RoutedCommand DecreaseSmall {
			get { return decrease_small; }
		}

		public static RoutedCommand IncreaseLarge {
			get { return increase_large; }
		}

		public static RoutedCommand IncreaseSmall {
			get { return increase_small; }
		}

		public static RoutedCommand MaximizeValue {
			get { return maximize_value; }
		}

		public static RoutedCommand MinimizeValue {
			get { return minimize_value; }
		}
		#endregion
		#endregion

		#region Public Override Methods
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			track = (Track)GetTemplateChild("PART_Track");
			if (track == null)
				return;
			Utility.SetBinding(track, Track.MaximumProperty, this, "Maximum");
			Utility.SetBinding(track, Track.MinimumProperty, this, "Minimum");
			Utility.SetBinding(track, Track.ValueProperty, this, "Value");
			Utility.SetBinding(track, Track.OrientationProperty, this, "Orientation");
			Utility.SetBinding(track, Track.IsDirectionReversedProperty, this, "IsDirectionReversed");
			SetSelectionRangeBounds();
			Bind(track.DecreaseRepeatButton);
			Bind(track.IncreaseRepeatButton);
			TickBar top_tick = GetTopTick();
			if (top_tick != null) {
				Bind(top_tick);
				Bind(GetBottomTick());
				track.SizeChanged += delegate(object sender, SizeChangedEventArgs e) {
					double reserved_space = Orientation == Orientation.Horizontal ? track.Thumb.ActualWidth : track.Thumb.ActualHeight;
					GetTopTick().ReservedSpace = reserved_space;
					GetBottomTick().ReservedSpace = reserved_space;
				};
			}
			global::System.Windows.Controls.Primitives.Thumb thumb = track.Thumb;
			#region Auto tool tip
			if (auto_tool_tip != null)
				SetAutoToolTipPlacementTarget(track);
			#endregion
			#region Thumb drag methods
			//FIXME: Do I need to remove these handlers?
			thumb.DragStarted += delegate(object sender, global::System.Windows.Controls.Primitives.DragStartedEventArgs e) {
				OnThumbDragStarted(e);
			};

			thumb.DragDelta += delegate(object sender, global::System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
				OnThumbDragDelta(e);
			};

			thumb.DragCompleted += delegate(object sender, global::System.Windows.Controls.Primitives.DragCompletedEventArgs e) {
				OnThumbDragCompleted(e);
			};
			#endregion
		}
		#endregion

		#region Protected Override Methods
		protected override Size ArrangeOverride(Size arrangeBounds) {
			Size result = base.ArrangeOverride(arrangeBounds);
			SetSelectionRangeBounds();
			return result;
		}

		//FIXME: I should not be overriding this.
		protected override Size MeasureOverride(Size constraint) {
			if (Parent == null)
				return new Size(0, 0);
			return base.MeasureOverride(constraint);
		}

		protected override AutomationPeer OnCreateAutomationPeer() {
#if Implementation
			return null;
#else
			return new SliderAutomationPeer(this);
#endif
		}

		protected override void OnMaximumChanged(double oldMaximum, double newMaximum) {
			base.OnMaximumChanged(oldMaximum, newMaximum);
			AdjustSelectionProperties();
		}

		protected override void OnMinimumChanged(double oldMinimum, double newMinimum) {
			base.OnMinimumChanged(oldMinimum, newMinimum);
			AdjustSelectionProperties();
		}

		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseLeftButtonDown(e);
			if (IsMoveToPointEnabled) {
				if (!track.Thumb.IsMouseOver) {
					Value = track.ValueFromPoint(e.MouseDevice.GetPosition(track));
					e.Handled = true;
				}
			}
		}

		protected override void OnValueChanged(double oldValue, double newValue) {
			//WDTDH
			base.OnValueChanged(oldValue, newValue);
		}
		#endregion

		#region Protected Methods
		#region Commands
		protected virtual void OnDecreaseLarge() {
			if (IsSnapToTickEnabled)
				MoveToNextTick(false);
			else
				Value -= LargeChange;
		}

		protected virtual void OnDecreaseSmall() {
			if (IsSnapToTickEnabled)
				MoveToNextTick(false);
			else
				Value -= SmallChange;
		}

		protected virtual void OnIncreaseLarge() {
			if (IsSnapToTickEnabled)
				MoveToNextTick(true);
			else
				Value += LargeChange;
		}

		protected virtual void OnIncreaseSmall() {
			if (IsSnapToTickEnabled)
				MoveToNextTick(true);
			else
				Value += SmallChange;
		}

		protected virtual void OnMaximizeValue() {
			Value = Maximum;
		}

		protected virtual void OnMinimizeValue() {
			Value = Minimum;
		}
		#endregion

		protected virtual void OnThumbDragCompleted(global::System.Windows.Controls.Primitives.DragCompletedEventArgs e) {
			if (auto_tool_tip != null)
				auto_tool_tip.IsOpen = false;
		}

		protected virtual void OnThumbDragDelta(global::System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
			if (e.Source == null)
				return;
			double value_from_distance = track.ValueFromDistance(e.HorizontalChange, e.VerticalChange);
			if (double.IsNaN(value_from_distance))
				return;
			Value = GetValueAdjustedForTicks(Value + value_from_distance);
			//FIXME: Auto tool tip does not move. I suspect this is done with an internal member.
			if (auto_tool_tip != null)
				SetAutoToolTipContent();
		}

		protected virtual void OnThumbDragStarted(global::System.Windows.Controls.Primitives.DragStartedEventArgs e) {
			if (auto_tool_tip != null) {
				auto_tool_tip.IsOpen = true;
				SetAutoToolTipContent();
			}
		}
		#endregion

		#region Private Methods
		static bool ValidateNonNegativeInteger(object value) {
			return (int)value >= 0;
		}

		static bool ValidateNonInfinite(object value) {
			return !double.IsInfinity((double)value);
		}

		void Bind(TickBar tickBar) {
			Utility.SetBinding(tickBar, TickBar.IsDirectionReversedProperty, this, "IsDirectionReversed");
			Utility.SetBinding(tickBar, TickBar.IsSelectionRangeEnabledProperty, this, "IsSelectionRangeEnabled");
			Utility.SetBinding(tickBar, TickBar.MaximumProperty, this, "Maximum");
			Utility.SetBinding(tickBar, TickBar.MinimumProperty, this, "Minimum");
			Utility.SetBinding(tickBar, TickBar.SelectionEndProperty, this, "SelectionEnd");
			Utility.SetBinding(tickBar, TickBar.SelectionStartProperty, this, "SelectionStart");
			Utility.SetBinding(tickBar, TickBar.TickFrequencyProperty, this, "TickFrequency");
			Utility.SetBinding(tickBar, TickBar.TicksProperty, this, "Ticks");
		}

		void Bind(RepeatButton repeatButton) {
			Utility.SetBinding(repeatButton, RepeatButton.DelayProperty, this, "Delay");
			//FIXME?: Should I do this? Track documentation only mentions Delay.
			Utility.SetBinding(repeatButton, RepeatButton.IntervalProperty, this, "Interval");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// Called when <see cref="Minimum"/>, <see cref="Maximum"/>, <see cref="SelectionStart"/> or <see cref="SelectionEnd"/> change.
		/// </remarks>
		void AdjustSelectionProperties() {
			double adjusted_selection_start = requested_selection_start;
			double adjusted_selection_end = requested_selection_end;
			if (adjusted_selection_start < Minimum)
				adjusted_selection_start = Minimum;
			else if (adjusted_selection_start > Maximum)
				adjusted_selection_start = Maximum;
			if (adjusted_selection_end < Minimum)
				adjusted_selection_end = Minimum;
			else if (adjusted_selection_end > Maximum)
				adjusted_selection_end = Maximum;
			if (adjusted_selection_end < adjusted_selection_start)
				adjusted_selection_end = adjusted_selection_start;
			adjusting_properties = true;
			if (SelectionStart != adjusted_selection_start)
				SelectionStart = adjusted_selection_start;
			if (SelectionEnd != adjusted_selection_end)
				SelectionEnd = adjusted_selection_end;
			adjusting_properties = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// Called when <see cref="SelectionStart"/> or <see cref="SelectionEnd"/> change.
		/// </remarks>
		void AdjustSelectionRange() {
			AdjustSelectionProperties();
			SetSelectionRangeBounds();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// Called when <see cref="SelectionStart"/> or <see cref="SelectionEnd"/> change, or the control changes its size.
		/// </remarks>
		void SetSelectionRangeBounds() {
			//TODO: Respect IsDirectionReversed.
			FrameworkElement selection_range = GetSelectionRange();
			if (selection_range == null)
				return;
			if (IsSelectionRangeEnabled) {
				selection_range.Visibility = Visibility.Visible;
				double range_size = Maximum - Minimum;
				double size;
				double position;
				if (range_size == 0) {
					size = 0;
					position = 0;
				} else {
					double dimension;
					if (Orientation == Orientation.Horizontal)
						dimension = track.ActualWidth - track.Thumb.ActualWidth;
					else
						dimension = track.ActualHeight - track.Thumb.ActualHeight;
					size = dimension * (SelectionEnd - SelectionStart) / range_size;
					position = dimension * (SelectionStart - Minimum) / range_size;
				}
				//FIXME: Is it always in a Canvas?
				if (Orientation == Orientation.Horizontal) {
					selection_range.Width = size;
					Canvas.SetLeft(selection_range, position + track.Thumb.ActualWidth / 2);
				} else {
					selection_range.Height = size;
					Canvas.SetTop(selection_range, position + track.Thumb.ActualHeight / 2);
				}
			} else
				selection_range.Visibility = Visibility.Hidden;
		}

		void SetUpAutoToolTip() {
			if (AutoToolTipPlacement == global::System.Windows.Controls.Primitives.AutoToolTipPlacement.None) {
				if (auto_tool_tip != null) {
					auto_tool_tip.IsOpen = false;
					auto_tool_tip = null;
				}
			} else {
				if (auto_tool_tip == null) {
					auto_tool_tip = new ToolTip();
					SetAutoToolTipPlacementTarget();
				}
				switch (AutoToolTipPlacement) {
					case global::System.Windows.Controls.Primitives.AutoToolTipPlacement.TopLeft:
						auto_tool_tip.Placement = Orientation == Orientation.Horizontal ? global::System.Windows.Controls.Primitives.PlacementMode.Top : global::System.Windows.Controls.Primitives.PlacementMode.Left;
						break;
					case global::System.Windows.Controls.Primitives.AutoToolTipPlacement.BottomRight:
						auto_tool_tip.Placement = Orientation == Orientation.Horizontal ? global::System.Windows.Controls.Primitives.PlacementMode.Bottom : global::System.Windows.Controls.Primitives.PlacementMode.Right;
						break;
				}
			}
		}

		void SetAutoToolTipContent() {
			StringBuilder format = new StringBuilder("0.");
			for (int decimal_digit_index = 0; decimal_digit_index < AutoToolTipPrecision; decimal_digit_index++)
				format.Append("0");
			auto_tool_tip.Content = Value.ToString(format.ToString());
		}

		void SetAutoToolTipPlacementTarget() {
			if (track != null)
				SetAutoToolTipPlacementTarget(track);
		}

		void SetAutoToolTipPlacementTarget(Track track) {
			auto_tool_tip.PlacementTarget = track.Thumb;
		}

		#region Parts
		TickBar GetTopTick() {
			return (TickBar)GetTemplateChild("TopTick");
		}

		TickBar GetBottomTick() {
			return (TickBar)GetTemplateChild("BottomTick");
		}
		
		FrameworkElement GetSelectionRange() {
			return (FrameworkElement)GetTemplateChild("PART_SelectionRange");
		}
		#endregion

		double GetValueAdjustedForTicks(double value) {
			if (IsSnapToTickEnabled) {
				if (Ticks != null && Ticks.Count != 0) {
					double closest = Minimum;
					double distance_from_closest = Math.Abs(value - closest);
					double distance;
					foreach (double tick in Ticks) {
						distance = Math.Abs(value - tick);
						if (distance < distance_from_closest) {
							closest = tick;
							distance_from_closest = distance;
						}
					}
					distance = Math.Abs(value - Maximum);
					if (distance < distance_from_closest)
						closest = Maximum;
					value = closest;
				} else if (TickFrequency == 0)
					value = Math.Abs(value - Minimum) < Math.Abs(value - Maximum) ? Minimum : Maximum;
				else
					value = (int)Math.Round((value - Minimum) / TickFrequency) * TickFrequency;

			}
			return value;
		}

		void MoveToNextTick(bool forward) {
			if (forward) {
				if (Value == Maximum)
					return;
			} else {
				if (Value == Minimum)
					return;
			}
			if (Ticks != null && Ticks.Count != 0) {
				double closest;
				double distance;
				double distance_from_closest;
				if (Ticks.Contains(Value)) {
					closest = double.NaN;
					distance_from_closest = double.PositiveInfinity;
					for (int tick_index = 0; tick_index < Ticks.Count; tick_index++) {
						double tick = Ticks[tick_index];
						if (forward) {
							if (tick > Value)
								distance = tick - Value;
							else
								continue;
						} else {
							if (tick < Value)
								distance = Value - tick;
							else
								continue;
						}
						if (distance < distance_from_closest) {
							closest = tick;
							distance_from_closest = distance;
						}
					}
				} else {
					closest = Ticks[forward ? 0 : Ticks.Count - 1];
					distance_from_closest = Math.Abs(Value - closest);
					for (int tick_index = 1; tick_index < Ticks.Count; tick_index++) {
						double tick = Ticks[forward ? tick_index : Ticks.Count - tick_index];
						distance = Math.Abs(Value - tick);
						if (distance < distance_from_closest) {
							closest = tick;
							distance_from_closest = distance;
						}
					}
				}
				if (!double.IsNaN(closest))
					Value = closest;
			} else if (TickFrequency != 0) {
				double value = ((int)Math.Floor(Value / TickFrequency) + (forward ? 1 : -1)) * TickFrequency;
				if (value < Minimum)
					value = Minimum;
				if (value > Maximum)
					value = Maximum;
				Value = value;
			}
		}
		#endregion
	}
}