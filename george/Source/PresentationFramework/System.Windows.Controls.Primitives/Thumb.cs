//
// Thumb.cs
//
// Author:
//   George Giolfan (georgegiolfan@yahoo.com)
//
// Copyright (C) 2007 George Giolfan
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System.ComponentModel;
using System.Windows.Automation.Peers;
using System.Windows.Input;
#if Implementation
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives
{
#else
namespace System.Windows.Controls.Primitives {
#endif
	[Localizability (LocalizationCategory.NeverLocalize)]
	public class Thumb : Control
	{
		#region Public Fields
		#region Dependency Properties
		static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly ("IsDragging", typeof (bool), typeof (Thumb), new FrameworkPropertyMetadata (delegate (DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((Thumb)d).OnDraggingChanged (e);
		}));
		public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;
		#endregion

		#region Routed Events
		public static readonly RoutedEvent DragCompletedEvent = EventManager.RegisterRoutedEvent ("DragCompleted", RoutingStrategy.Bubble, typeof (DragCompletedEventHandler), typeof (Thumb));
		public static readonly RoutedEvent DragDeltaEvent = EventManager.RegisterRoutedEvent ("DragDelta", RoutingStrategy.Bubble, typeof (DragDeltaEventHandler), typeof (Thumb));
		public static readonly RoutedEvent DragStartedEvent = EventManager.RegisterRoutedEvent ("DragStarted", RoutingStrategy.Bubble, typeof (DragStartedEventHandler), typeof (Thumb));
		#endregion
		#endregion

		#region Private Fields
		double initial_position_x, initial_position_y;
		double last_delta_position_x, last_delta_position_y;
		bool ignore_mouse_move;
		#endregion

		#region Static Constructor
		static Thumb ()
		{
#if Implementation
			Theme.Load ();
#endif
			DefaultStyleKeyProperty.OverrideMetadata (typeof (Thumb), new FrameworkPropertyMetadata (typeof (Thumb)));
		}
		#endregion

		#region Public Constructors
		public Thumb ()
		{
			Focusable = false;
		}
		#endregion

		#region Public Properties
		[Bindable (true)]
		public bool IsDragging {
			get { return (bool)GetValue (IsDraggingProperty); }
			private set { SetValue (IsDraggingPropertyKey, value); }
		}

		#endregion

		#region Public Methods
		public void CancelDrag ()
		{
			RaiseEvent (new DragCompletedEventArgs (0, 0, true));
		}
		#endregion

		#region Protected Methods
		protected override AutomationPeer OnCreateAutomationPeer ()
		{
#if Implementation
			return null;
#else
			return new ThumbAutomationPeer(this);
#endif
		}

		protected virtual void OnDraggingChanged (DependencyPropertyChangedEventArgs e)
		{
		}

		protected override void OnMouseLeftButtonDown (MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown (e);
			Focus ();
			ignore_mouse_move = true;
			CaptureMouse ();
			ignore_mouse_move = false;
			IsDragging = true;
			Point mouse_position = e.MouseDevice.GetPosition (this);
			initial_position_x = mouse_position.X;
			initial_position_y = mouse_position.Y;
			DragStartedEventArgs event_args = new DragStartedEventArgs (initial_position_x, initial_position_y);
			event_args.RoutedEvent = DragStartedEvent;
			RaiseEvent (event_args);
			e.Handled = true;
		}

		protected override void OnMouseLeftButtonUp (MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonUp (e);
			ReleaseMouseCapture ();
			IsDragging = false;
			DragCompletedEventArgs event_args = new DragCompletedEventArgs (last_delta_position_x, last_delta_position_y, false);
			event_args.RoutedEvent = DragCompletedEvent;
			RaiseEvent (event_args);
			e.Handled = true;
		}

		protected override void OnMouseMove (MouseEventArgs e)
		{
			base.OnMouseMove (e);
			if (ignore_mouse_move)
				return;
			if (IsMouseCaptured) {
				Point mouse_position = e.MouseDevice.GetPosition (this);
				last_delta_position_x = mouse_position.X - initial_position_x;
				last_delta_position_y = mouse_position.Y - initial_position_y;
				DragDeltaEventArgs event_args = new DragDeltaEventArgs (last_delta_position_x, last_delta_position_y);
				event_args.RoutedEvent = DragDeltaEvent;
				RaiseEvent (event_args);
				e.Handled = true;
			}
		}
		#endregion

		#region Public Events
		public event DragCompletedEventHandler DragCompleted
		{
			add { AddHandler (DragCompletedEvent, value); }
			remove { RemoveHandler (DragCompletedEvent, value); }
		}

		public event DragDeltaEventHandler DragDelta
		{
			add { AddHandler (DragDeltaEvent, value); }
			remove { RemoveHandler (DragDeltaEvent, value); }
		}

		public event DragStartedEventHandler DragStarted
		{
			add { AddHandler (DragStartedEvent, value); }
			remove { RemoveHandler (DragStartedEvent, value); }
		}
		#endregion
	}
}