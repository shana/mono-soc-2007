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
		public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register("LargeChange", typeof(double), typeof(RangeBase), new PropertyMetadata(1D));
		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(RangeBase), new PropertyMetadata(1D, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RangeBase)d).OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
		}));
		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(RangeBase), new PropertyMetadata(0D, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RangeBase)d).OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
		}));
		public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register("SmallChange", typeof(double), typeof(RangeBase), new PropertyMetadata(0.1D));
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(RangeBase), new PropertyMetadata(0D, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RangeBase)d).OnValueChanged((double)e.OldValue, (double)e.NewValue);
		}));
		#endregion

		#region Routed Events
		public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(RangeBase));
		#endregion
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
	}
}