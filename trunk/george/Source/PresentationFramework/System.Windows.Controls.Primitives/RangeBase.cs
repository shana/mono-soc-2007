using System.ComponentModel;
#if Implementation
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	public abstract class RangeBase : Control {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register("LargeChange", typeof(double), typeof(RangeBase), new FrameworkPropertyMetadata(1D));
		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(RangeBase), new FrameworkPropertyMetadata(1D, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RangeBase i = (RangeBase)d;
			double new_value = (double)e.NewValue;
			if (i.Value > new_value) {
				i.ignore_value_change = true;
				i.Value = new_value;
				i.ignore_value_change = false;
			}
			i.OnMaximumChanged((double)e.OldValue, new_value);
		}), Validate);
		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(RangeBase), new FrameworkPropertyMetadata(0D, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RangeBase i = (RangeBase)d;
			double new_value = (double)e.NewValue;
			if (i.Value < new_value) {
				i.ignore_value_change = true;
				i.Value = new_value;
				i.ignore_value_change = false;
			}
			i.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
		}), Validate);
		public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register("SmallChange", typeof(double), typeof(RangeBase), new FrameworkPropertyMetadata(0.1D));
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(RangeBase), new FrameworkPropertyMetadata(0D, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RangeBase i = (RangeBase)d;
			if (i.ignore_value_change)
				return;
			double new_value = (double)e.NewValue;
			if (new_value < i.Minimum)
				i.Value = i.Minimum;
			else if (new_value > i.Maximum)
				i.Value = i.Maximum;
			else
				i.OnValueChanged((double)e.OldValue, new_value);
		}), Validate);
		#endregion

		#region Routed Events
		public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(RangeBase));
		#endregion
		#endregion

		#region Private Fields
		bool ignore_value_change;
		#endregion

		#region Protected Constructors
		protected RangeBase() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[Bindable(true)]
		public double LargeChange {
			get { return (double)GetValue(LargeChangeProperty); }
			set { SetValue(LargeChangeProperty, value); }
		}

		[Bindable(true)]
		public double Maximum {
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}

		[Bindable(true)]
		public double Minimum {
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}

		[Bindable(true)]
		public double SmallChange {
			get { return (double)GetValue(SmallChangeProperty); }
			set { SetValue(SmallChangeProperty, value); }
		}

		[Bindable(true)]
		public double Value {
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		#endregion
		#endregion

		#region Public Methods
		public override string ToString() {
			return base.ToString();
		}
		#endregion

		#region Protected Methods
		protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum) { 
		}

		protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum) {
		}

		protected virtual void OnValueChanged(double oldValue, double newValue) { 
		}
		#endregion
		
		#region Public Events
		#region Routed Events
		public event RoutedPropertyChangedEventHandler<double> ValueChanged {
			add { AddHandler(ValueChangedEvent, value); }
			remove { RemoveHandler(ValueChangedEvent, value); }
		}
		#endregion
		#endregion

		#region Private Methods
		static bool Validate(object value) {
			return !double.IsInfinity((double)value); 
		}
		#endregion
	}
}