//
// RowDefinition.cs
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
using System.ComponentModel;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	public class RowDefinition : DefinitionBase
	{
		#region Public Fields
		#region Dependency Properties
		[TypeConverter ("System.Windows.LengthConverter, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, Custom=null")]
		public static readonly DependencyProperty MaxHeightProperty = DependencyProperty.Register ("MaxHeight", typeof (double), typeof (RowDefinition), new FrameworkPropertyMetadata (double.PositiveInfinity, InvalidateGridMeasure));
		[TypeConverter ("System.Windows.LengthConverter, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, Custom=null")]
		public static readonly DependencyProperty MinHeightProperty = DependencyProperty.Register ("MinHeight", typeof (double), typeof (RowDefinition), new FrameworkPropertyMetadata (InvalidateGridMeasure));
		public static readonly DependencyProperty HeightProperty = DependencyProperty.Register ("Height", typeof (GridLength), typeof (RowDefinition), new FrameworkPropertyMetadata (new GridLength (1, GridUnitType.Star), InvalidateGridMeasure));
		#endregion
		#endregion

		#region Private Fields
		double actual_height;
		double offset;
		Grid grid;
		#endregion

		#region Public Constructors
		public RowDefinition ()
		{
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[TypeConverter (typeof (LengthConverter))]
		public double MaxHeight {
			get { return (double)GetValue (MaxHeightProperty); }
			set { SetValue (MaxHeightProperty, value); }
		}

		[TypeConverter (typeof (LengthConverter))]
		public double MinHeight {
			get { return (double)GetValue (MinHeightProperty); }
			set { SetValue (MinHeightProperty, value); }
		}

		public GridLength Height {
			get { return (GridLength)GetValue (HeightProperty); }
			set { SetValue (HeightProperty, value); }
		}
		#endregion

		public double ActualHeight {
			get { return actual_height; }
			internal set { actual_height = value; }
		}

		public double Offset {
			get { return offset; }
			internal set { offset = value; }
		}
		#endregion

		#region Internal Properties
		internal Grid Grid {
			get { return grid; }
			set { grid = value; }
		}
		#endregion

		#region Private Methods
		static void InvalidateGridMeasure (DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Grid grid = ((RowDefinition)d).grid;
			if (grid != null)
				grid.InvalidateMeasure ();
		}
		#endregion
	}
}