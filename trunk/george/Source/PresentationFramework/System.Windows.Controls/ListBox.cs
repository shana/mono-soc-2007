//
// ListBox.cs
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
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Automation.Peers;
using System.Windows.Input;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	[StyleTypedProperty (Property = "ItemContainerStyle", StyleTargetType = typeof (ListBoxItem))]
	[Localizability (LocalizationCategory.ListBox)]
	public class ListBox : global::System.Windows.Controls.Primitives.Selector
	{
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterReadOnly ("SelectedItems", typeof (IList), typeof (ListBox), new FrameworkPropertyMetadata (new ObservableCollection<object> ())).DependencyProperty;
		public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register ("SelectionMode", typeof (SelectionMode), typeof (ListBox), new FrameworkPropertyMetadata ());
		#endregion
		#endregion

		#region Static Constructor
		static ListBox ()
		{
#if Implementation
			Theme.Load ();
#endif
			DefaultStyleKeyProperty.OverrideMetadata (typeof (ListBox), new FrameworkPropertyMetadata (typeof (ListBox)));
		}
		#endregion

		#region Public Constructors
		public ListBox ()
		{
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[Bindable (true)]
		public IList SelectedItems {
			get { return (IList)GetValue (SelectedItemsProperty); }
		}

		public SelectionMode SelectionMode {
			get { return (SelectionMode)GetValue (SelectionModeProperty); }
			set { SetValue (SelectionModeProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override bool HandlesScrolling {
			get { return true; }
		}
		#endregion

		#region Public Methods
		public void ScrollIntoView (object item)
		{
		}

		public void SelectAll ()
		{
			if (SelectionMode == SelectionMode.Single)
				throw new NotSupportedException ();
		}

		public void UnselectAll ()
		{
		}
		#endregion

		#region Protected Methods
		protected override DependencyObject GetContainerForItemOverride ()
		{
			return new ListBoxItem ();
		}

		protected override bool IsItemItsOwnContainerOverride (object item)
		{
			return item is ListBoxItem;
		}

		protected override AutomationPeer OnCreateAutomationPeer ()
		{
#if Implementation
			return null;
#else
			return new ListBoxAutomationPeer(this);
#endif
		}

		protected override void OnIsMouseCapturedChanged (DependencyPropertyChangedEventArgs e)
		{
			base.OnIsMouseCapturedChanged (e);
		}

		protected override void OnKeyDown (KeyEventArgs e)
		{
			base.OnKeyDown (e);
		}

		protected override void OnMouseMove (MouseEventArgs e)
		{
			base.OnMouseMove (e);
		}

		protected override void OnSelectionChanged (SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged (e);
		}

		protected override void PrepareContainerForItemOverride (DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride (element, item);
		}

		protected bool SetSelectedItems (IEnumerable selectedItems)
		{
			return false;
		}
		#endregion

		#region Internal Methods
		internal ListBoxItem GetListBoxItemForItem (object item)
		{
			return (ListBoxItem)ItemContainerGenerator.ContainerFromItem (item);
		}
		#endregion
	}
}