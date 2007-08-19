//
// Polyline.cs
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
	public sealed class Polyline : global::System.Windows.Shapes.Shape
	{
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty FillRuleProperty = DependencyProperty.Register ("FillRule", typeof (FillRule), typeof (Polyline), new FrameworkPropertyMetadata (FillRule.EvenOdd, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty PointsProperty = DependencyProperty.Register ("Points", typeof (PointCollection), typeof (Polyline), new FrameworkPropertyMetadata (null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		#endregion
		#endregion

		#region Public Constructors
		public Polyline ()
		{
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public FillRule FillRule {
			get { return (FillRule)GetValue (FillRuleProperty); }
			set { SetValue (FillRuleProperty, value); }
		}

		public PointCollection Points {
			get { return (PointCollection)GetValue (PointsProperty); }
			set { SetValue (PointsProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override Geometry DefiningGeometry {
			get {
				PathGeometry result = new PathGeometry ();
				result.FillRule = FillRule;
				PathFigure path_figure = new PathFigure ();
				PointCollection points = Points;
				if (points.Count > 0) {
					path_figure.StartPoint = points [0];
					for (int point_index = 1; point_index < points.Count; point_index++)
						path_figure.Segments.Add (new LineSegment (points [point_index], true));
				}
				result.Figures.Add (path_figure);
				return result;
			}
		}
		#endregion
	}
}