//
// ListBoxItem.cs
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
using Mono.WindowsPresentationFoundation;
using System.ComponentModel;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Media;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	public class ListBoxItem : ContentControl
	{
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register ("IsSelected", typeof (bool), typeof (ListBoxItem), new FrameworkPropertyMetadata (delegate (DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ListBoxItem i = (ListBoxItem)d;
			ListBox list_box = i.GetListBox ();
			if (list_box != null) {
				switch (list_box.SelectionMode) {
				case SelectionMode.Single:
					object selected_item = list_box.SelectedItem;
					if (selected_item != null) {
						ListBoxItem selected_list_box_item = list_box.GetListBoxItemForItem (selected_item);
						if (selected_list_box_item != i)
							selected_list_box_item.IsSelected = false;
					}
					break;
				}
			}
			if ((bool)e.NewValue) {
				i.OnSelected (new RoutedEventArgs (SelectedEvent, i));
				if (list_box != null)
					list_box.SelectedItem = list_box.ItemContainerGenerator.ItemFromContainer (i);
			} else
				i.OnUnselected (new RoutedEventArgs (UnselectedEvent, i));

		}));
		#endregion

		#region Routed Events
		public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent ("Selected", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (ListBoxItem));
		public static readonly RoutedEvent UnselectedEvent = EventManager.RegisterRoutedEvent ("Unselected", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (ListBoxItem));
		#endregion
		#endregion

		#region Static Constructor
		static ListBoxItem ()
		{
#if Implementation
			Theme.Load ();
#endif
			DefaultStyleKeyProperty.OverrideMetadata (typeof (ListBoxItem), new FrameworkPropertyMetadata (typeof (ListBoxItem)));
		}
		#endregion

		#region Public Constructors
		public ListBoxItem ()
		{
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[Bindable (true)]
		public bool IsSelected {
			get { return (bool)GetValue (IsSelectedProperty); }
			set { SetValue (IsSelectedProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override AutomationPeer OnCreateAutomationPeer ()
		{
#if Implementation
			return null;
#else
			return new ListBoxItemAutomationPeer(this);
#endif
		}

		protected override void OnMouseEnter (MouseEventArgs e)
		{
			base.OnMouseEnter (e);
		}

		protected override void OnMouseLeave (MouseEventArgs e)
		{
			base.OnMouseLeave (e);
		}

		protected override void OnMouseLeftButtonDown (MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown (e);
			if (IsInListBoxInShownWindow ())
				IsSelected = true;
		}

		protected override void OnMouseRightButtonDown (MouseButtonEventArgs e)
		{
			base.OnMouseRightButtonDown (e);
		}

		protected virtual void OnSelected (RoutedEventArgs e)
		{
			RaiseEvent (e);
		}

		protected virtual void OnUnselected (RoutedEventArgs e)
		{
			RaiseEvent (e);
		}

		protected override void OnVisualParentChanged (DependencyObject oldParent)
		{
			base.OnVisualParentChanged (oldParent);
			ListBox old_list_box = oldParent as ListBox;
			if (IsSelected) {
				ListBox list_box = GetListBox ();
				if (list_box != null && list_box.SelectionMode == SelectionMode.Single) {
					object selected_item = list_box.SelectedItem;
					if (selected_item != null)
						list_box.GetListBoxItemForItem (selected_item).IsSelected = false;
				}
			}
		}
		#endregion

		#region Public Events
		#region Routed Events
		public event RoutedEventHandler Selected
		{
			add { AddHandler (SelectedEvent, value); }
			remove { RemoveHandler (SelectedEvent, value); }
		}

		public event RoutedEventHandler Unselected
		{
			add { AddHandler (UnselectedEvent, value); }
			remove { RemoveHandler (UnselectedEvent, value); }
		}
		#endregion
		#endregion

		#region Private Methods
		ListBox GetListBox ()
		{
			return Parent as ListBox;
		}

		bool IsInListBoxInShownWindow ()
		{
			ListBox list_box = GetListBox ();
			if (list_box == null)
				return false;
			return Utility.IsInVisibleWindow (list_box);
		}
		#endregion
	}
}