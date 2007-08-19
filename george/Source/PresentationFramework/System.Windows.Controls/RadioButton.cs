//
// RadioButton.cs
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
using System.Windows.Automation.Peers;
using System.Windows.Input;
#if Implementation
using System.Windows;
using System.Windows.Controls;
using Mono.System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls
{
#else
using System.Windows.Controls.Primitives;
namespace System.Windows.Controls {
#endif
	[Localizability (LocalizationCategory.RadioButton)]
	public class RadioButton : ToggleButton
	{
		#region Public Fields
		#region Dependency Properties
		static public readonly DependencyProperty GroupNameProperty = DependencyProperty.Register ("GroupName", typeof (string), typeof (RadioButton), new FrameworkPropertyMetadata (string.Empty));
		#endregion
		#endregion

		#region Static Contstructor
		static RadioButton ()
		{
#if Implementation
			Theme.Load ();
#endif
			DefaultStyleKeyProperty.OverrideMetadata (typeof (RadioButton), new FrameworkPropertyMetadata (typeof (RadioButton)));
		}
		#endregion

		#region Public Constructors
		public RadioButton ()
		{
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[Localizability (LocalizationCategory.NeverLocalize)]
		public string GroupName {
			get { return (string)GetValue (GroupNameProperty); }
			set { SetValue (GroupNameProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override void OnAccessKey (AccessKeyEventArgs e)
		{
			//WDTDH
			base.OnAccessKey (e);
		}

		protected override void OnChecked (RoutedEventArgs e)
		{
			base.OnChecked (e);
			DependencyObject parent = LogicalTreeHelper.GetParent (this);
			if (parent != null) {
				string group_name = GroupName;
				foreach (object child in LogicalTreeHelper.GetChildren (parent)) {
					RadioButton radio_button = child as RadioButton;
					if (radio_button != null && radio_button != this && radio_button.GroupName == group_name)
						radio_button.IsChecked = false;
				}
			}
		}

		protected override AutomationPeer OnCreateAutomationPeer ()
		{
#if Implementation
			return null;
#else
			return new RadioButtonAutomationPeer(this);
#endif
		}

		protected override void OnToggle ()
		{
			IsChecked = true;
		}
		#endregion
	}
}