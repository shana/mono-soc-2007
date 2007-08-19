//
// ItemContainerGenerator.cs
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
using Mono.System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls
{
#else
using System.Windows.Controls.Primitives;
namespace System.Windows.Controls {
#endif
	public sealed class ItemContainerGenerator : IItemContainerGenerator, IWeakEventListener
	{
		#region Private Fields
		GeneratorStatus status;
		#endregion

		#region Internal Constructor
		internal ItemContainerGenerator ()
		{
		}
		#endregion

		#region Public Properties
		public GeneratorStatus Status {
			get { return status; }
		}
		#endregion

		#region Public Methods
		public DependencyObject ContainerFromIndex (int index)
		{
			//WDTDH
			return null;
		}

		public DependencyObject ContainerFromItem (object item)
		{
			//WDTDH
			return null;
		}

		public int IndexFromContainer (DependencyObject container)
		{
			//WDTDH
			return 0;
		}

		public object ItemFromContainer (DependencyObject container)
		{
			//WDTDH
			return null;
		}
		#endregion

		#region Public Events
		public event ItemsChangedEventHandler ItemsChanged;
		public event EventHandler StatusChanged;
		#endregion

		#region Explicit Interface Implementations
		#region IItemContainerGenerator
		DependencyObject IItemContainerGenerator.GenerateNext ()
		{
			throw new Exception ("The method or operation is not implemented.");
		}

		DependencyObject IItemContainerGenerator.GenerateNext (out bool isNewlyRealized)
		{
			throw new Exception ("The method or operation is not implemented.");
		}

		GeneratorPosition IItemContainerGenerator.GeneratorPositionFromIndex (int itemIndex)
		{
			throw new Exception ("The method or operation is not implemented.");
		}

		ItemContainerGenerator IItemContainerGenerator.GetItemContainerGeneratorForPanel (Panel panel)
		{
			throw new Exception ("The method or operation is not implemented.");
		}

		int IItemContainerGenerator.IndexFromGeneratorPosition (GeneratorPosition position)
		{
			throw new Exception ("The method or operation is not implemented.");
		}

		void IItemContainerGenerator.PrepareItemContainer (DependencyObject container)
		{
			throw new Exception ("The method or operation is not implemented.");
		}

		void IItemContainerGenerator.Remove (GeneratorPosition position, int count)
		{
			throw new Exception ("The method or operation is not implemented.");
		}

		void IItemContainerGenerator.RemoveAll ()
		{
			throw new Exception ("The method or operation is not implemented.");
		}

		IDisposable IItemContainerGenerator.StartAt (GeneratorPosition position, GeneratorDirection direction)
		{
			throw new Exception ("The method or operation is not implemented.");
		}

		IDisposable IItemContainerGenerator.StartAt (GeneratorPosition position, GeneratorDirection direction, bool allowStartAtRealizedItem)
		{
			throw new Exception ("The method or operation is not implemented.");
		}
		#endregion

		#region IWeakEventListener
		bool IWeakEventListener.ReceiveWeakEvent (Type managerType, object sender, EventArgs e)
		{
			throw new Exception ("The method or operation is not implemented.");
		}
		#endregion
		#endregion
	}
}