//
// ItemsPresenter.cs
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
	[Localizability(LocalizationCategory.NeverLocalize)]
	public class ItemsPresenter : FrameworkElement
	{
		#region Public Constructors
		public ItemsPresenter()
		{
		}
		#endregion

		#region Public Methods
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			GetVisualChild(0);
			throw new InvalidOperationException("VisualTree of ItemsPanelTemplate must be a single element.");
			//WDTDH
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize)
		{
			return base.ArrangeOverride(finalSize);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			return base.MeasureOverride(availableSize);
		}

		protected virtual void OnTemplateChanged(ItemsPanelTemplate oldTemplate, ItemsPanelTemplate newTemplate)
		{
		}
		#endregion
	}
}