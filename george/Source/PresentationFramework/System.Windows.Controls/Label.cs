//
// Label.cs
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
using System;
using System.Windows.Input;
using System.Windows.Automation.Peers;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	public class Label : ContentControl
	{
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty TargetProperty = DependencyProperty.Register ("Target", typeof (UIElement), typeof (Label), new FrameworkPropertyMetadata ());
		#endregion
		#endregion

		#region Static Constructor
		static Label ()
		{
#if Implementation
			Theme.Load ();
#endif
			DefaultStyleKeyProperty.OverrideMetadata (typeof (Label), new FrameworkPropertyMetadata (typeof (Label)));
		}
		#endregion

		#region Public Constructors
		public Label ()
		{
			IsTabStop = false;

			AccessKeyManager.AddAccessKeyPressedHandler (this, delegate (object sender, AccessKeyPressedEventArgs e)
			{
				UIElement target = Target;
				if (target != null)
					target.Focus ();
			});
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public UIElement Target
		{
			get { return (UIElement)GetValue (TargetProperty); }
			set { SetValue (TargetProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override AutomationPeer OnCreateAutomationPeer ()
		{
#if Implementation
			return null;
#else
			return new LabelAutomationPeer(this);
#endif
		}
		#endregion
	}
}