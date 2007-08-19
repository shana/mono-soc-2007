//
// ProgressBar.cs
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
using System.Windows.Automation.Peers;
using System.Windows.Media;
using System.Windows.Shapes;
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
	[TemplatePart (Name = "PART_Track", Type = typeof (FrameworkElement))]
	[TemplatePart (Name = "PART_Indicator", Type = typeof (FrameworkElement))]
	public class ProgressBar : RangeBase
	{
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register ("IsIndeterminate", typeof (bool), typeof (ProgressBar), new FrameworkPropertyMetadata ());
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register ("Orientation", typeof (Orientation), typeof (ProgressBar), new FrameworkPropertyMetadata ());
		#endregion
		#endregion

		#region Private Fields
		FrameworkElement _indicator;
		FrameworkElement _track;
		#endregion

		#region Static Constructor
		static ProgressBar ()
		{
#if Implementation
			Theme.Load ();
#endif
			DefaultStyleKeyProperty.OverrideMetadata (typeof (ProgressBar), new FrameworkPropertyMetadata (typeof (ProgressBar)));
		}
		#endregion

		#region Public Constructors
		public ProgressBar ()
		{
			Maximum = 100;
			SizeChanged += delegate (object sender, SizeChangedEventArgs e)
			{
				SetIndicatorPosition ();
			};
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public bool IsIndeterminate {
			get { return (bool)GetValue (IsIndeterminateProperty); }
			set { SetValue (IsIndeterminateProperty, value); }
		}

		public Orientation Orientation {
			get { return (Orientation)GetValue (OrientationProperty); }
			set { SetValue (OrientationProperty, value); }
		}
		#endregion
		#endregion

		#region Public Methods
		public override void OnApplyTemplate ()
		{
			base.OnApplyTemplate ();
			_track = (FrameworkElement)GetTemplateChild ("PART_Track");
			_indicator = (FrameworkElement)GetTemplateChild ("PART_Indicator");
		}
		#endregion

		#region Protected Methods
		protected override AutomationPeer OnCreateAutomationPeer ()
		{
#if Implementation
			return null;
#else
			return new ProgressBarAutomationPeer(this);
#endif
		}

		protected override void OnValueChanged (double oldValue, double newValue)
		{
			base.OnValueChanged (oldValue, newValue);
			SetIndicatorPosition ();
		}
		#endregion

		#region Private Methods
		void SetIndicatorPosition ()
		{
			if (_track == null)
				return;
			double ratio = Value / Maximum;
			if (Orientation == Orientation.Horizontal)
				_indicator.Width = _track.ActualWidth * ratio;
			else
				_indicator.Height = _track.ActualHeight * ratio;
		}
		#endregion
	}
}