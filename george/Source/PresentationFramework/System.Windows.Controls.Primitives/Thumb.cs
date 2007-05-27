#if Implementation
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	[Localizability(LocalizationCategory.NeverLocalize)]
	public class Thumb : Control {
		#region Public Fields
		#region Routed Events
		public static readonly RoutedEvent DragCompletedEvent = EventManager.RegisterRoutedEvent("DragCompleted", RoutingStrategy.Bubble, typeof(DragCompletedEventHandler), typeof(Thumb));
		public static readonly RoutedEvent DragDeltaEvent = EventManager.RegisterRoutedEvent("DragDelta", RoutingStrategy.Bubble, typeof(DragDeltaEventHandler), typeof(Thumb));
		public static readonly RoutedEvent DragStartedEvent = EventManager.RegisterRoutedEvent("DragStarted", RoutingStrategy.Bubble, typeof(DragStartedEventHandler), typeof(Thumb));
		#endregion

		#region Dependency Properties
		static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly("IsDragging", typeof(bool), typeof(Thumb), new PropertyMetadata(delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Thumb)d).OnDraggingChanged(e);
		}));
		public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;
		#endregion
		#endregion

		#region Private Fields
		double initial_position_x, initial_position_y;
		double last_delta_position_x, last_delta_position_y;
		bool ignore_mouse_move;
		#endregion

		#region Static Constructor
		static Thumb() {
			Theme.Load();
		}
		#endregion

		#region Public Constructors
		public Thumb() {
			Focusable = false;
			//FIXME
			//Style = (Style)FindResource(typeof(Thumb));
		}
		#endregion

		#region Public Properties
		[Bindable(true)]
		public bool IsDragging {
			get { return (bool)GetValue(IsDraggingProperty); }
		}

		#endregion

		#region Public Methods
		public void CancelDrag() {
			RaiseEvent(new DragCompletedEventArgs(0, 0, true));
		}
		#endregion

		#region Protected Methods
		protected override AutomationPeer OnCreateAutomationPeer() {
#if Implementation
			return null;
#else
			return new ThumbAutomationPeer(this);
#endif
		}

		protected virtual void OnDraggingChanged(DependencyPropertyChangedEventArgs e) {
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			Focus();
			ignore_mouse_move = true;
			CaptureMouse();
			ignore_mouse_move = false;
			IsDraggingInternal = true;
			Point mouse_position = e.MouseDevice.GetPosition(this);
			initial_position_x = mouse_position.X;
			initial_position_y = mouse_position.Y;
			DragStartedEventArgs event_args = new DragStartedEventArgs(initial_position_x, initial_position_y);
			event_args.RoutedEvent = DragStartedEvent;
			RaiseEvent(event_args);
			e.Handled = true;
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			ReleaseMouseCapture();
			IsDraggingInternal = false;
			DragCompletedEventArgs event_args = new DragCompletedEventArgs(last_delta_position_x, last_delta_position_y, false);
			event_args.RoutedEvent = DragCompletedEvent;
			RaiseEvent(event_args);
			e.Handled = true;
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if (ignore_mouse_move)
				return;
			if (IsFocused && IsMouseCaptured) {
				Point mouse_position = e.MouseDevice.GetPosition(this);
				last_delta_position_x = mouse_position.X - initial_position_x;
				last_delta_position_y = mouse_position.Y - initial_position_y;
				DragDeltaEventArgs event_args = new DragDeltaEventArgs(last_delta_position_x, last_delta_position_y);
				event_args.RoutedEvent = DragDeltaEvent;
				RaiseEvent(event_args);
				e.Handled = true;
			}
		}
		#endregion

		#region Public Events
		public event DragCompletedEventHandler DragCompleted {
			add { AddHandler(DragCompletedEvent, value); }
			remove { RemoveHandler(DragCompletedEvent, value); }
		}

		public event DragDeltaEventHandler DragDelta {
			add { AddHandler(DragDeltaEvent, value); }
			remove { RemoveHandler(DragDeltaEvent, value); }
		}

		public event DragStartedEventHandler DragStarted {
			add { AddHandler(DragStartedEvent, value); }
			remove { RemoveHandler(DragStartedEvent, value); }
		}
		#endregion

		#region Private Properties
		bool IsDraggingInternal {
			set { SetValue(IsDraggingPropertyKey, value); }
		}
		#endregion
	}
}