//
// ItemsChangedEventArgs.cs
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
using System.Collections.Specialized;
#if Implementation
using System;
namespace Mono.System.Windows.Controls.Primitives
{
#else
namespace System.Windows.Controls.Primitives {
#endif
	public class ItemsChangedEventArgs : EventArgs
	{
		#region Private Fields
		NotifyCollectionChangedAction action;
		int item_count;
		int item_ui_count;
		GeneratorPosition old_position;
		GeneratorPosition position;
		#endregion

		#region Internal Constructors
		internal ItemsChangedEventArgs (NotifyCollectionChangedAction action, int itemCount, int itemUICount, GeneratorPosition oldPosition, GeneratorPosition position)
		{
			this.action = action;
			item_count = itemCount;
			item_ui_count = itemUICount;
			old_position = oldPosition;
			this.position = position;
		}
		#endregion

		#region Public Properties
		public NotifyCollectionChangedAction Action {
			get { return action; }
		}

		public int ItemCount {
			get { return item_count; }
		}

		public int ItemUICount {
			get { return item_ui_count; }
		}

		public GeneratorPosition OldPosition {
			get { return old_position; }
		}

		public GeneratorPosition Position {
			get { return position; }
		}
		#endregion
	}
}
