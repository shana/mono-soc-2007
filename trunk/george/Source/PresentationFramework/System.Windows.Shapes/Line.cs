//
// Line.cs
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
using System.Windows.Media;
#if Implementation
using System.Windows;
namespace Mono.System.Windows.Shapes
{
#else
namespace System.Windows.Shapes {
#endif
	public sealed class Line : global::System.Windows.Shapes.Shape
	{
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty X1Property = DependencyProperty.Register ("X1", typeof (double), typeof (Line), new FrameworkPropertyMetadata (0D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty X2Property = DependencyProperty.Register ("X2", typeof (double), typeof (Line), new FrameworkPropertyMetadata (0D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty Y1Property = DependencyProperty.Register ("Y1", typeof (double), typeof (Line), new FrameworkPropertyMetadata (0D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty Y2Property = DependencyProperty.Register ("Y2", typeof (double), typeof (Line), new FrameworkPropertyMetadata (0D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		#endregion
		#endregion

		#region Public Constructors
		public Line ()
		{
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[TypeConverter (typeof (LengthConverter))]
		public double X1 {
			get { return (double)GetValue (X1Property); }
			set { SetValue (X1Property, value); }
		}

		[TypeConverter (typeof (LengthConverter))]
		public double X2 {
			get { return (double)GetValue (X2Property); }
			set { SetValue (X2Property, value); }
		}

		[TypeConverter (typeof (LengthConverter))]
		public double Y1 {
			get { return (double)GetValue (Y1Property); }
			set { SetValue (Y1Property, value); }
		}

		[TypeConverter (typeof (LengthConverter))]
		public double Y2 {
			get { return (double)GetValue (Y2Property); }
			set { SetValue (Y2Property, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override Geometry DefiningGeometry {
			get {
				return new LineGeometry (new Point (X1, Y1), new Point (X2, Y2));
			}
		}
		#endregion
	}
}