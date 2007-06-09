using System.ComponentModel;
using System.Windows.Input;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	[Localizability(LocalizationCategory.Button)]
	public abstract class ButtonBase : ContentControl, ICommandSource {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty ClickModeProperty = DependencyProperty.Register("ClickMode", typeof(ClickMode), typeof(ButtonBase), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ButtonBase), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ButtonBase), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(ButtonBase), new FrameworkPropertyMetadata());
		public static readonly DependencyPropertyKey IsPressedPropertyKey = DependencyProperty.RegisterReadOnly("IsPressed", typeof(bool), typeof(ButtonBase), new FrameworkPropertyMetadata(delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ButtonBase)d).OnIsPressedChanged(e);
		}));
		public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;
		#endregion

		#region Routed Events
		public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ButtonBase));
		#endregion
		#endregion

		#region Private Fields
		bool should_set_is_pressed_when_the_mouse_is_over_it;
		#endregion

		#region Public Constructors
		protected ButtonBase() {
			//FIXME
			/*
			AccessKeyManager.AddAccessKeyPressedHandler(this, delegate(object sender, AccessKeyPressedEventArgs e) {
				MessageBox.Show(Environment.StackTrace);
				OnClick();
			});*/
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[Bindable(true)]
		public ClickMode ClickMode {
			get { return (ClickMode)GetValue(ClickModeProperty); }
			set { SetValue(ClickModeProperty, value); }
		}

		[Bindable(true)]
		[Localizability(LocalizationCategory.NeverLocalize)]
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		[Localizability(LocalizationCategory.NeverLocalize)]
		[Bindable(true)]
		public object CommandParameter {
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		[Bindable(true)]
		public IInputElement CommandTarget {
			get { return (IInputElement)GetValue(CommandTargetProperty); }
			set { SetValue(CommandTargetProperty, value); }
		}

		public bool IsPressed {
			get { return (bool)GetValue(IsPressedProperty); }
		}
		#endregion
		#endregion

		#region Protected Properties
		protected override bool IsEnabledCore {
			get { return IsEnabled; }
		}
		#endregion

		#region Protected Methods
		protected override void OnAccessKey(AccessKeyEventArgs e) {
			base.OnAccessKey(e);
			OnClick();
		}

		protected virtual void OnClick() {
			RaiseEvent(new RoutedEventArgs(ClickEvent, this));
			//FIXME: Execute only for CommandTarget.
			if (Command != null && Command.CanExecute(CommandParameter))
				Command.Execute(CommandParameter);
		}

		protected virtual void OnIsPressedChanged(DependencyPropertyChangedEventArgs e) {
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if (IsButtonKey(e)) {
				IsPressedInternal = true;
				if (ClickMode == ClickMode.Press)
					OnClick();
			}
		}

		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			IsPressedInternal = false;
			if (IsButtonKey(e))
				if (ClickMode == ClickMode.Release)
					OnClick();
		}

		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnLostKeyboardFocus(e);
			should_set_is_pressed_when_the_mouse_is_over_it = false;
		}

		protected override void OnLostMouseCapture(MouseEventArgs e) {
			base.OnLostMouseCapture(e);
			should_set_is_pressed_when_the_mouse_is_over_it = false;
			IsPressedInternal = false;
		}

		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			if (ClickMode == ClickMode.Hover) {
				IsPressedInternal = true;
				OnClick();
			}
		}

		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			if (ClickMode == ClickMode.Hover)
				IsPressedInternal = false;
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			if (e == null)
				throw new NullReferenceException();
			if (e.RoutedEvent == null)
				throw new InvalidOperationException();
			base.OnMouseLeftButtonDown(e);
			Focus();
			if (e.ButtonState == MouseButtonState.Pressed) {
				should_set_is_pressed_when_the_mouse_is_over_it = true;
				CaptureMouse();
				if (ClickMode == ClickMode.Press)
					OnClick();
			}
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			ReleaseMouseCapture();
			if (ClickMode == ClickMode.Release)
				OnClick();
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if (should_set_is_pressed_when_the_mouse_is_over_it)
				IsPressedInternal = RectangleContainsPoint(0, 0, ActualWidth, ActualHeight, e.GetPosition(this));
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			//WDTDH
			base.OnRenderSizeChanged(sizeInfo);
		}
		#endregion

		#region Public Events
		#region Routed Events
		public event RoutedEventHandler Click {
			add { AddHandler(ClickEvent, value); }
			remove { RemoveHandler(ClickEvent, value); }
		}
		#endregion
		#endregion

		#region Private Properties
		bool IsPressedInternal {
			set { SetValue(IsPressedPropertyKey, value); }
		}
		#endregion

		#region Private Methods
		static bool RectangleContainsPoint(double x, double y, double width, double height, Point point) {
			return point.X >= x && point.X <= x + width && point.Y >= y && point.Y <= y + height;
		}
		#endregion

		#region Internal Methods
		static internal bool IsButtonKey(KeyEventArgs e) {
			return e.Key == Key.Enter || e.Key == Key.Space;
		}
		#endregion
	}
}