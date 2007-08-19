//
// DragCompletedEventArgs.cs
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
/*
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls.Primitives
{
#else
namespace System.Windows.Controls.Primitives {
#endif
	public class DragCompletedEventArgs : RoutedEventArgs
	{
		#region Private Fields
		bool canceled;
		double horizontalChange;
		double verticalChange;
		#endregion

		#region Public Constructors
		public DragCompletedEventArgs (double horizontalChange, double verticalChange, bool canceled)
		{
			this.canceled = canceled;
			this.horizontalChange = horizontalChange;
			this.verticalChange = verticalChange;
		}
		#endregion

		#region Public Properties
		public bool Canceled {
			get { return canceled; }
		}

		public double HorizontalChange {
			get { return horizontalChange; }
		}

		public double VerticalChange {
			get { return verticalChange; }
		}
		#endregion

		#region Protected Methods
		protected override void InvokeEventHandler (Delegate genericHandler, object genericTarget)
		{
			((DragCompletedEventHandler)genericHandler).Invoke (genericTarget, this);
		}
		#endregion
	}
}
*/