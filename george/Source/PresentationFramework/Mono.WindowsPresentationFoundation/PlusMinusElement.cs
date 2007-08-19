//
// PlusMinusElement.cs: An element that displays a plus/minus sign and 
// represents expanding/collapsing content.
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
using System.Windows;
using System.Windows.Controls;
namespace Mono.WindowsPresentationFoundation
{
	/// <summary>
	/// An element that displays a plus/minus sign and represents expanding/collapsing content.
	/// </summary>
	public class PlusMinusElement : ThreeDElement
	{
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty ShowsMinusProperty = DependencyProperty.Register ("ShowsMinus", typeof (bool), typeof (PlusMinusElement), new PropertyMetadata (delegate (DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((PlusMinusElement)d).SetSign ();
		}));
		#endregion
		#endregion

		#region Private Fields
		TextBlock sign_block = new TextBlock ();
		#endregion

		#region Public Constructors
		public PlusMinusElement ()
		{
			Width = Height = 20;
			sign_block.HorizontalAlignment = HorizontalAlignment.Center;
			sign_block.VerticalAlignment = VerticalAlignment.Center;
			Child = sign_block;
			SetSign ();
		}
		#endregion

		#region Public Fields
		#region Dependency Properties
		public bool ShowsMinus {
			get { return (bool)GetValue (ShowsMinusProperty); }
			set { SetValue (ShowsMinusProperty, value); }
		}
		#endregion
		#endregion

		#region Private Methods
		void SetSign ()
		{
			sign_block.Text = ShowsMinus ? "-" : "+";
		}
		#endregion
	}
}