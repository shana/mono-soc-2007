//
// TabItem trivial things.cs
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
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	public partial class TabItem : HeaderedContentControl
	{
		#region Static Constructor
		static TabItem ()
		{
#if Implementation
			Theme.Load ();
#endif
			DefaultStyleKeyProperty.OverrideMetadata (typeof (TabItem), new FrameworkPropertyMetadata (typeof (TabItem)));
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[Bindable (true)]
		public bool IsSelected {
			get { return (bool)GetValue (IsSelectedProperty); }
			set { SetValue (IsSelectedProperty, value); }
		}

		public Dock TabStripPlacement {
			get { return (Dock)GetValue (TabStripPlacementProperty); }
			internal set { SetValue (TabStripPlacementPropertyKey, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override void OnAccessKey (AccessKeyEventArgs e)
		{
			base.OnAccessKey (e);
			HandleUIAction ();
		}

		protected override AutomationPeer OnCreateAutomationPeer ()
		{
#if Implementation
			return null;
#else
			return new TabItemAutomationPeer(this);
#endif
		}

		protected override void OnMouseLeftButtonDown (MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown (e);
			HandleUIAction ();
			e.Handled = true;
		}
		#endregion

		#region Private Methods
		TabControl GetTabControl ()
		{
			return ItemsControl.ItemsControlFromItemContainer (this) as TabControl;
		}
		#endregion
	}
}