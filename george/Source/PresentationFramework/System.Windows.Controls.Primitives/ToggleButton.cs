using System;
using System.ComponentModel;
using System.Windows.Automation.Peers;
#if Implementation
using System.Windows;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	public class ToggleButton : ButtonBase {
		#region Public Fields
		#region Dependency Properties
		static public readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?), typeof(ToggleButton), new FrameworkPropertyMetadata(false, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            bool? old_value = (bool?)e.OldValue;
            bool? new_value = (bool?)e.NewValue;
            if (new_value != old_value)
                ((ToggleButton)d).RaiseIsCheckedValueEvent(new_value);
		}));

		static public readonly DependencyProperty IsThreeStateProperty = DependencyProperty.Register("IsThreeState", typeof(bool), typeof(ToggleButton), new FrameworkPropertyMetadata());
		#endregion

		#region Routed Events
		static public readonly RoutedEvent CheckedEvent = EventManager.RegisterRoutedEvent("Checked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToggleButton));
		static public readonly RoutedEvent IndeterminateEvent = EventManager.RegisterRoutedEvent("Indeterminate", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToggleButton));
		static public readonly RoutedEvent UncheckedEvent = EventManager.RegisterRoutedEvent("Unchecked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToggleButton));
		#endregion
		#endregion

		#region Static Constructor
		static ToggleButton() {
#if Implementation
			Theme.Load();
#endif
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButton), new FrameworkPropertyMetadata(typeof(ToggleButton)));
		}
		#endregion

		#region Public Constructors
		public ToggleButton() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[TypeConverter(typeof(NullableBoolConverter))]
		[Localizability(LocalizationCategory.None, Readability=Readability.Unreadable)]
		public bool? IsChecked {
			get { return (bool?)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}
		
		[Bindable(true)]
		public bool IsThreeState {
			get { return (bool)GetValue(IsThreeStateProperty); }
			set { SetValue(IsThreeStateProperty, value); }
		}
		#endregion
		#endregion

		#region Public Methods
		public override string ToString() {
			return base.ToString() + " Content:" + Content + " IsChecked:" + IsChecked;
		}
		#endregion

		#region Protected Methods
		protected virtual void OnChecked(RoutedEventArgs e) {
			RaiseEvent(e);
		}

		protected override void OnClick() {
			base.OnClick();
			OnToggle();
		}

		protected override AutomationPeer OnCreateAutomationPeer() {
#if Implementation
			return null;
#else
			return new ToggleButtonAutomationPeer(this);
#endif
		}

		protected virtual void OnIndeterminate(RoutedEventArgs e) {
			RaiseEvent(e);
		}

		protected virtual void OnToggle() {
			switch (IsChecked) {
			case false:
				IsChecked = true; 
				break;
			case true:
				IsChecked = IsThreeState ? null : (bool?)false; 
				break;
			case null:
				IsChecked = false; 
				break;
			}
		}

		protected virtual void OnUnchecked(RoutedEventArgs e) {
			RaiseEvent(e);
		}
		#endregion

		#region Public Events
		#region Routed Events
		public event RoutedEventHandler Checked {
			add { AddHandler(CheckedEvent, value); }
			remove { RemoveHandler(CheckedEvent, value); }
		}

		public event RoutedEventHandler Indeterminate {
			add { AddHandler(IndeterminateEvent, value); }
			remove { RemoveHandler(IndeterminateEvent, value); }
		}

		public event RoutedEventHandler Unchecked {
			add { AddHandler(UncheckedEvent, value); }
			remove { RemoveHandler(UncheckedEvent, value); }
		}
		#endregion
		#endregion

		#region Private Methods
		void RaiseIsCheckedValueEvent(bool? new_value) {
			switch (new_value) {
			case null:
				OnIndeterminate(new RoutedEventArgs(IndeterminateEvent, this));
				break;
			case false:
				OnUnchecked(new RoutedEventArgs(UncheckedEvent, this));
				break;
			case true:
				OnChecked(new RoutedEventArgs(CheckedEvent, this));
				break;
			}
		}
		#endregion
	}
}