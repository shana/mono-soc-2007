//
// Path.cs
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
using System.Windows.Media;
#if Implementation
using System.Windows;
namespace Mono.System.Windows.Shapes
{
#else
namespace System.Windows.Shapes {
#endif
	public sealed class Path : global::System.Windows.Shapes.Shape
	{
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register ("Data", typeof (Geometry), typeof (Path), new FrameworkPropertyMetadata (null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		#endregion
		#endregion

		#region Public Constructors
		public Path ()
		{
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public Geometry Data {
			get { return (Geometry)GetValue (DataProperty); }
			set { SetValue (DataProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override Geometry DefiningGeometry {
			get { return Data; }
		}
		#endregion
	}
}