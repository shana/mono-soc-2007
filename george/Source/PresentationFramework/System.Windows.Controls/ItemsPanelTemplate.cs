//
// ItemsPanelTemplate.cs
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
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	//TODO: Fix when integrating. (FrameworkTemplate contains internal abstract members.)
	public class ItemsPanelTemplate : global::System.Windows.Controls.ItemsPanelTemplate
	{
		#region Public Constructors
		public ItemsPanelTemplate ()
		{
		}

		public ItemsPanelTemplate (FrameworkElementFactory root)
			: base (root)
		{
		}
		#endregion

		#region Protected Methods
		protected override void ValidateTemplatedParent (FrameworkElement templatedParent)
		{
			if (templatedParent == null)
				throw new ArgumentNullException ("templatedParent");
			//LAMESPEC?
			if (!(templatedParent is ItemsPresenter))
				throw new ArgumentException ("'ItemsPresenter' ControlTemplate TargetType does not match templated type 'FrameworkElement'.");
		}
		#endregion
	}
}