//
// HeaderedContentControl.cs
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
using System.ComponentModel;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	[Localizability (LocalizationCategory.Text)]
	public class HeaderedContentControl : ContentControl
	{
		#region Public Fields
		#region Dependency Properties
		static readonly DependencyPropertyKey HasHeaderPropertyKey = DependencyProperty.RegisterReadOnly ("HasHeader", typeof (bool), typeof (HeaderedContentControl), new FrameworkPropertyMetadata ());
		public static readonly DependencyProperty HasHeaderProperty = HasHeaderPropertyKey.DependencyProperty;
		public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register ("Header", typeof (object), typeof (HeaderedContentControl), new FrameworkPropertyMetadata (null, delegate (DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((HeaderedContentControl)d).OnHeaderChanged (e.OldValue, e.NewValue);
		}));
		public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register ("HeaderTemplate", typeof (DataTemplate), typeof (HeaderedContentControl), new FrameworkPropertyMetadata (null, delegate (DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((HeaderedContentControl)d).OnHeaderTemplateChanged ((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
		}));
		public static readonly DependencyProperty HeaderTemplateSelectorProperty = DependencyProperty.Register ("HeaderTemplateSelector", typeof (DataTemplateSelector), typeof (HeaderedContentControl), new FrameworkPropertyMetadata (null, delegate (DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((HeaderedContentControl)d).OnHeaderTemplateSelectorChanged ((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue);
		}));
		#endregion
		#endregion

		#region Static Constructor
		static HeaderedContentControl ()
		{
#if Implementation
			Theme.Load ();
#endif
			DefaultStyleKeyProperty.OverrideMetadata (typeof (HeaderedContentControl), new FrameworkPropertyMetadata (typeof (HeaderedContentControl)));
		}
		#endregion

		#region Public Constructors
		public HeaderedContentControl ()
		{
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[Bindable (false)]
		public bool HasHeader {
			get { return (bool)GetValue (HasHeaderProperty); }
			private set { SetValue (HasHeaderPropertyKey, value); }
		}

		[Localizability (LocalizationCategory.Label)]
		[Bindable (true)]
		public object Header {
			get { return GetValue (HeaderProperty); }
			set { SetValue (HeaderProperty, value); }
		}

		[Bindable (true)]
		public DataTemplate HeaderTemplate {
			get { return (DataTemplate)GetValue (HeaderTemplateProperty); }
			set { SetValue (HeaderTemplateProperty, value); }
		}

		[Bindable (true)]
		public DataTemplateSelector HeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue (HeaderTemplateSelectorProperty); }
			set { SetValue (HeaderTemplateSelectorProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Properties
		protected override IEnumerator LogicalChildren {
			get {
				ArrayList result = new ArrayList (2);
				if (Header != null)
					result.Add (Header);
				if (Content != null)
					result.Add (Content);
				return result.GetEnumerator ();
			}
		}
		#endregion

		#region Public Methods
		public override string ToString ()
		{
			return string.Format ("System.Windows.Controls.HeaderedContentControl Header:{0} Content:{1}", Header, Content);
		}
		#endregion

		#region Protected Methods
		protected virtual void OnHeaderChanged (object oldHeader, object newHeader)
		{
		}

		protected virtual void OnHeaderTemplateChanged (DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
		{
		}

		protected virtual void OnHeaderTemplateSelectorChanged (DataTemplateSelector oldHeaderTemplateSelector, DataTemplateSelector newHeaderTemplateSelector)
		{
		}
		#endregion
	}
}