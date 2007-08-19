using NUnit.Framework;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using Mono.System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls
{
#else
using System.Windows.Controls.Primitives;
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class SliderTest
	{
		[Test]
		public void StaticProperties ()
		{
			Assert.IsTrue (((FrameworkPropertyMetadata)Slider.SelectionStartProperty.GetMetadata (typeof (Slider))).BindsTwoWayByDefault, "BindsTwoWayByDefault");
			Assert.AreEqual (Slider.DecreaseLarge.GetType (), typeof (RoutedCommand), "DecreaseLarge.GetType()");
			Assert.AreEqual (Slider.DecreaseLarge.Name, "DecreaseLarge", "DecreaseLarge.Name");
			Assert.AreEqual (Slider.DecreaseLarge.OwnerType, typeof (Slider), "DecreaseLarge.OwnerType");
			Assert.AreEqual (Slider.DecreaseLarge.InputGestures.Count, 0, "DecreaseLarge.InputGestures.Count");

			Assert.IsNull (Slider.TicksProperty.ValidateValueCallback, "TicksProperty.ValidateValueCallback");
			Assert.AreEqual (Slider.TicksProperty.GetMetadata (typeof (Slider)).GetType (), typeof (FrameworkPropertyMetadata), "TicksProperty metadata type");
			FrameworkPropertyMetadata ticks_metadata = (FrameworkPropertyMetadata)Slider.TicksProperty.GetMetadata (typeof (Slider));
			Assert.IsNull (ticks_metadata.PropertyChangedCallback, "ticks_metadata.PropertyChangedCallback");
		}

		[Test]
		public void Creation ()
		{
			Slider p = new Slider ();
			Assert.AreEqual (p.Value, 0, "Value");
			Assert.AreEqual (p.Minimum, 0, "Minimum");
			Assert.AreEqual (p.Maximum, 10, "Maximum");
			Assert.AreEqual (p.SmallChange, 0.1, "SmallChange");
			Assert.AreEqual (p.LargeChange, 1, "LargeChange");
			Assert.AreEqual (p.IsSelectionRangeEnabled, false, "IsSelectionRangeEnabled");
			Assert.AreEqual (p.SelectionStart, 0, "SelectionStart");
			Assert.AreEqual (p.SelectionEnd, 0, "SelectionEnd");
			Assert.AreEqual (p.TickFrequency, 1, "TickFrequency");
			Assert.AreEqual (p.CommandBindings.Count, 0, "CommandBindings.Count");
			Assert.IsTrue (Slider.MinimizeValue.CanExecute (null, p), "MinimizeValue.CanExecute");
			Assert.IsTrue (Slider.MaximizeValue.CanExecute (null, p), "MaximizeValue.CanExecute");
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void NegativePrecision ()
		{
			new Slider ().AutoToolTipPrecision = -1;
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void NegativeDelay ()
		{
			new Slider ().Delay = -1;
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void NegativeInterval ()
		{
			new Slider ().Interval = -1;
		}

		[Test]
		public void IncreaseLarge ()
		{
			Slider s = new Slider ();
			Slider.IncreaseLarge.Execute (null, s);
			Assert.AreEqual (s.Value, s.LargeChange);
		}

		#region OnIncreaseLarge
		[Test]
		public void OnIncreaseLarge ()
		{
			new OnIncreaseLargeSlider ();
		}

		class OnIncreaseLargeSlider : Slider
		{
			public OnIncreaseLargeSlider ()
			{
				OnIncreaseLarge ();
				Assert.AreEqual (Value, LargeChange);
			}
		}
		#endregion

		#region CommandCallsMethod
		[Test]
		public void CommandCallsMethod ()
		{
			new CommandCallsMethodSlider ();
		}

		class CommandCallsMethodSlider : Slider
		{
			public CommandCallsMethodSlider ()
			{
				should_set_on_increase_large_called = true;
				IncreaseLarge.Execute (null, this);
				Assert.IsTrue (on_increase_large_called);
			}
			bool on_increase_large_called;
			bool should_set_on_increase_large_called;
			protected override void OnIncreaseLarge ()
			{
				base.OnIncreaseLarge ();
				if (should_set_on_increase_large_called)
					on_increase_large_called = true;
			}
		}
		#endregion

		[Test]
		public void SelectionPropertiesAreKeptSane ()
		{
			Slider s = new Slider ();
			s.SelectionStart = 10;
			Assert.AreEqual (s.SelectionStart, 10, "Start 1");
			Assert.AreEqual (s.SelectionEnd, 10, "End 1");
			s.SelectionEnd = 5;
			Assert.AreEqual (s.SelectionStart, 10, "Start 2");
			Assert.AreEqual (s.SelectionEnd, 10, "End 2");
			s.SelectionEnd = 0;
			Assert.AreEqual (s.SelectionStart, 10, "Start 3");
			Assert.AreEqual (s.SelectionEnd, 10, "End 3");
			s.SelectionStart = 10;
			Assert.AreEqual (s.SelectionStart, 10, "Start 4");
			Assert.AreEqual (s.SelectionEnd, 10, "End 4");
			s.SelectionEnd = 0;
			Assert.AreEqual (s.SelectionStart, 10, "Start 5");
			Assert.AreEqual (s.SelectionEnd, 10, "End 5");
			s.SelectionStart = 0;
			Assert.AreEqual (s.SelectionStart, 0, "Start 6");
			Assert.AreEqual (s.SelectionEnd, 0, "End 6");
			s.SelectionEnd = 10;
			Assert.AreEqual (s.SelectionStart, 0, "Start 7");
			Assert.AreEqual (s.SelectionEnd, 10, "End 7");
		}

		[Test]
		public void SelectionPropertiesRememberedSimplified ()
		{
			Slider s = new Slider ();
			s.SelectionStart = 10;
			Assert.AreEqual (s.SelectionEnd, 10, "Before");
			s.SelectionStart = 0;
			Assert.AreEqual (s.SelectionEnd, 0, "After");
		}

		[Test]
		public void SelectionPropertiesAreKeptSaneInAWindow ()
		{
			Window w = new Window ();
			w.Show ();
			Slider s = new Slider ();
			w.Content = s;
			s.SelectionStart = 10;
			s.SelectionEnd = 5;
			Assert.AreEqual (s.SelectionStart, 10);
			Assert.AreEqual (s.SelectionEnd, 10);
		}

		[Test]
		public void InvalidSelectionStartLessThanMinimum ()
		{
			Slider s = new Slider ();
			s.Minimum = 10;
			s.SelectionStart = 9;
		}

		[Test]
		public void InvalidSelectionStartNegative ()
		{
			new Slider ().SelectionStart = -1;
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void InvalidSelectionStartNegativeInfinity ()
		{
			new Slider ().SelectionStart = double.NegativeInfinity;
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void InvalidSelectionStartPositiveInfinity ()
		{
			new Slider ().SelectionStart = double.PositiveInfinity;
		}

		[Test]
		public void InvalidMinimumNegative ()
		{
			new Slider ().Minimum = -1;
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void InvalidMinimumNegativeInfinity ()
		{
			new Slider ().Minimum = double.NegativeInfinity;
		}

		#region SelectionStartCallsApplyTemplate
		[Test]
		public void SelectionStartCallsApplyTemplate ()
		{
			new SelectionStartCallsApplyTemplateSlider ();
		}

		class SelectionStartCallsApplyTemplateSlider : Slider
		{
			bool called;
			bool should_set_called;
			public SelectionStartCallsApplyTemplateSlider ()
			{
				should_set_called = true;
				SelectionStart = 1;
				Assert.IsFalse (called);
			}
			public override void OnApplyTemplate ()
			{
				base.OnApplyTemplate ();
				if (should_set_called)
					called = true;
			}
		}
		#endregion

		#region ConstructorCallsApplyTemplate
		[Test]
		public void ConstructorCallsApplyTemplate ()
		{
			new ConstructorCallsApplyTemplateSlider ();
		}

		class ConstructorCallsApplyTemplateSlider : Slider
		{
			bool called;
			public ConstructorCallsApplyTemplateSlider ()
			{
				Assert.IsFalse (called);
			}
			public override void OnApplyTemplate ()
			{
				base.OnApplyTemplate ();
				called = true;
			}
		}
		#endregion

		#region OnThumbDragDelta
		[Test]
		public void OnThumbDragDelta ()
		{
			new OnThumbDragDeltaSlider ();
		}

		class OnThumbDragDeltaSlider : Slider
		{
			public OnThumbDragDeltaSlider ()
			{
				global::System.Windows.Controls.Primitives.DragDeltaEventArgs e = new global::System.Windows.Controls.Primitives.DragDeltaEventArgs (10, 0);
				OnThumbDragDelta (e);
				Assert.AreEqual (Value, 0);
			}
		}
		#endregion

		#region OnThumbDragDelta2
		[Test]
		public void OnThumbDragDelta2 ()
		{
			new OnThumbDragDelta2Slider ();
		}

		class OnThumbDragDelta2Slider : Slider
		{
			public OnThumbDragDelta2Slider ()
			{
				OnThumbDragStarted (new global::System.Windows.Controls.Primitives.DragStartedEventArgs (0, 0));
				global::System.Windows.Controls.Primitives.DragDeltaEventArgs e = new global::System.Windows.Controls.Primitives.DragDeltaEventArgs (10, 0);
				OnThumbDragDelta (e);
				Assert.AreEqual (Value, 0);
				e.Source = GetTemplateChild ("Thumb");
				OnThumbDragDelta (e);
				Assert.AreEqual (Value, 0);
			}
		}
		#endregion

		[Test]
		public void SelectionStartIsAdjustedToMinimum ()
		{
			Slider s = new Slider ();
			s.Minimum = 2;
			s.SelectionStart = 1;
			Assert.AreEqual (s.SelectionStart, 2);
		}

		[Test]
		public void SelectionStartIsAdjustedToMinimumRemembered ()
		{
			Slider s = new Slider ();
			s.Minimum = 2;
			s.SelectionStart = 1;
			s.Minimum = 0;
			Assert.AreEqual (s.SelectionStart, 1);
		}

		#region TrackExistAtCreation
		[Test]
		public void TrackExistAtCreation ()
		{
			new TrackExistAtCreationSlider ();
		}

		class TrackExistAtCreationSlider : Slider
		{
			public TrackExistAtCreationSlider ()
			{
				Assert.IsNull (GetTemplateChild ("PART_Track"));
			}
		}
		#endregion

		#region OrientationCallsApplyTemplate
		[Test]
		public void OrientationCallsApplyTemplate ()
		{
			new OrientationCallsApplyTemplateSlider ();
		}

		class OrientationCallsApplyTemplateSlider : Slider
		{
			bool called;
			bool should_set_called;
			public OrientationCallsApplyTemplateSlider ()
			{
				should_set_called = true;
				Orientation = Orientation.Vertical;
				Assert.IsFalse (called);
			}
			public override void OnApplyTemplate ()
			{
				base.OnApplyTemplate ();
				if (should_set_called)
					called = true;
			}
		}
		#endregion

		#region BindingsSlider
		[Test]
		public void Bindings ()
		{
			new BindingsSlider ();
		}

		class BindingsSlider : Slider
		{
			public BindingsSlider ()
			{
				Window w = new Window ();
				w.Show ();
				w.Content = this;
				ApplyTemplate ();
				TickBar bottom_tick = (TickBar)GetTemplateChild ("BottomTick");
				Assert.IsNull (bottom_tick.DataContext);
			}
		}
		#endregion

		[Test]
		public void Ticks ()
		{
			Slider s = new Slider ();
			s.Ticks = new DoubleCollection (new double [] { 2, 1 });
			Assert.AreEqual (s.Ticks [0], 2, "Keeps order");
			s.Ticks = new DoubleCollection (new double [] { s.Maximum + 1 });
			Assert.AreEqual (s.Ticks [0], s.Maximum + 1, "Value outside of range");
			s.Ticks = new DoubleCollection ();
			Assert.AreEqual (s.Ticks.Count, 0, "No values");
		}

		#region CommandsRespectTicks
		[Test]
		public void CommandsRespectTicks ()
		{
			new CommandsRespectTicksSlider ();
		}

		class CommandsRespectTicksSlider : Slider
		{
			public CommandsRespectTicksSlider ()
			{
				IsSnapToTickEnabled = true;
				Ticks = new DoubleCollection (new double [] { 3 });
				OnIncreaseSmall ();
				Assert.AreEqual (Value, 3);
			}
		}
		#endregion

		#region ValueRespectsTicks
		[Test]
		public void ValueRespectsTicks ()
		{
			new ValueRespectsTicksSlider ();
		}

		class ValueRespectsTicksSlider : Slider
		{
			public ValueRespectsTicksSlider ()
			{
				IsSnapToTickEnabled = true;
				Ticks = new DoubleCollection (new double [] { 3 });
				Value = 1;
				Assert.AreEqual (Value, 1);
			}
		}
		#endregion

		#region OnValueChangedRespectsTicks
		[Test]
		public void OnValueChangedRespectsTicks ()
		{
			new OnValueChangedRespectsTicksSlider ();
		}

		class OnValueChangedRespectsTicksSlider : Slider
		{
			public OnValueChangedRespectsTicksSlider ()
			{
				IsSnapToTickEnabled = true;
				Ticks = new DoubleCollection (new double [] { 3 });
				OnValueChanged (0, 1);
				Assert.AreEqual (Value, 0, "1");
				Value = 1;
				OnValueChanged (0, 1);
				Assert.AreEqual (Value, 1, "2");
			}
		}
		#endregion

		#region CommandsRespectRange
		[Test]
		public void CommandsRespectRange ()
		{
			new CommandsRespectRangeSlider ();
		}

		class CommandsRespectRangeSlider : Slider
		{
			public CommandsRespectRangeSlider ()
			{
				OnDecreaseLarge ();
				Assert.AreEqual (Value, 0);
			}
		}
		#endregion

		#region TickBar
		[Test]
		public void TickBar ()
		{
			new TickBarSlider ();
		}

		class TickBarSlider : Slider
		{
			public TickBarSlider ()
			{
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				OnApplyTemplate ();
				TickBar top_tick = (TickBar)GetTemplateChild ("TopTick");
				Track track = (Track)GetTemplateChild ("PART_Track");
				Assert.IsTrue (top_tick.SnapsToDevicePixels, "SnapsToDevicePixels");
				Assert.AreEqual (top_tick.ActualWidth, 0, "ActualWidth");
				TickPlacement = TickPlacement.Both;
				Assert.AreEqual (top_tick.ActualWidth, 0, "ActualWidth 2");
				Assert.AreEqual (top_tick.Width, double.NaN, "Width 2");
				Assert.AreEqual (top_tick.Margin.Right, 0, "Margin.Right");
				Assert.AreEqual (top_tick.Placement, TickBarPlacement.Top, "TopTick Placement 2");
				TickBar bottom_tick = (TickBar)GetTemplateChild ("BottomTick");
				Assert.AreEqual (bottom_tick.Placement, TickBarPlacement.Bottom, "BottomTick Placement 2");
				Assert.AreEqual (top_tick.ReservedSpace, 0, "ReservedSpace");
			}
		}
		#endregion

		#region TickBarReservedSpace
		[Test]
		public void TickBarReservedSpace ()
		{
			new TickBarReservedSpaceSlider ();
		}

		class TickBarReservedSpaceSlider : Slider
		{
			public TickBarReservedSpaceSlider ()
			{
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				TickPlacement = TickPlacement.Both;
				Assert.AreEqual (((TickBar)GetTemplateChild ("TopTick")).ReservedSpace, 0);
			}
		}
		#endregion

		#region TickBarReservedSpace2
		[Test]
		public void TickBarReservedSpace2 ()
		{
			new TickBarReservedSpace2Slider ();
		}

		class TickBarReservedSpace2Slider : Slider
		{
			public TickBarReservedSpace2Slider ()
			{
				TickPlacement = TickPlacement.Both;
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				Assert.AreNotEqual (((TickBar)GetTemplateChild ("TopTick")).ReservedSpace, 0);
			}
		}
		#endregion

		[Test]
		public void WindowsSdkFeb2007LameSpec ()
		{
			Slider s = new Slider ();
			Assert.AreEqual (s.Value, 0);
			Slider.IncreaseLarge.Execute (null, s);
			Assert.AreEqual (s.SelectionStart, new Slider ().SelectionStart);
			Assert.AreEqual (s.Value, s.LargeChange);
		}

		[Test]
		public void CanCallOnApplyTemplateWhenever ()
		{
			new Slider ().OnApplyTemplate ();
		}

		#region HasTrack
		[Test]
		public void HasTrack ()
		{
			new HasTrackSlider ();
		}

		class HasTrackSlider : Slider
		{
			public HasTrackSlider ()
			{
				Assert.IsNull (GetTemplateChild ("PART_Track"), "1");
				ApplyTemplate ();
				Assert.IsNull (GetTemplateChild ("PART_Track"), "2");
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				Assert.IsNotNull (Template, "3");
				Assert.IsNotNull (GetTemplateChild ("PART_Track"), "4");
			}
		}
		#endregion

		#region Delay
		[Test]
		public void Delay ()
		{
			new DelaySlider ();
		}

		class DelaySlider : Slider
		{
			public DelaySlider ()
			{
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				Delay += 1;
				Assert.AreEqual (Delay, ((Track)GetTemplateChild ("PART_Track")).IncreaseRepeatButton.Delay);
			}
		}
		#endregion

		#region Interval
		[Test]
		public void Interval ()
		{
			new IntervalSlider ();
		}

		class IntervalSlider : Slider
		{
			public IntervalSlider ()
			{
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				Interval += 1;
				Assert.AreEqual (Interval, ((Track)GetTemplateChild ("PART_Track")).IncreaseRepeatButton.Interval);
			}
		}
		#endregion
	}
}