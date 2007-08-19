//
// Ellipse.cs
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
using System.Windows.Shapes;
using System;
namespace Mono.System.Windows.Shapes
{
#else
namespace System.Windows.Shapes {
#endif
	public sealed class Ellipse : global::System.Windows.Shapes.Shape
	{
		#region Private Fields
		Geometry rendered_geometry;
		#endregion

		#region Public Constructors
		public Ellipse ()
		{
			Stretch = Stretch.Fill;
		}
		#endregion

		#region Public Properties
		public override Transform GeometryTransform {
			get {
				return base.GeometryTransform;
			}
		}

		public override Geometry RenderedGeometry {
			get { return rendered_geometry ?? base.RenderedGeometry; }
		}
		#endregion

		#region Protected Properties
		protected override Geometry DefiningGeometry {
			get {
				double width;
				double height;
				switch (Stretch) {
				case Stretch.None:
					width = height = 0;
					break;
				case Stretch.Fill:
					width = ActualWidth;
					height = ActualHeight;
					break;
				case Stretch.Uniform:
					width = height = Math.Min (ActualWidth, ActualHeight);
					break;
				default:
					width = height = Math.Max (ActualWidth, ActualHeight);
					break;
				}
				double stroke_thickness = Stroke == null ? 0 : StrokeThickness;
				if (width >= stroke_thickness && height >= stroke_thickness)
					return new EllipseGeometry (new Rect (stroke_thickness / 2, stroke_thickness / 2, width - stroke_thickness, height - stroke_thickness));
				else
					return new EllipseGeometry ();
			}
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride (Size finalSize)
		{
			return finalSize;
		}

		protected override Size MeasureOverride (Size constraint)
		{
			return new Size (ZeroIfInfinite (constraint.Width), ZeroIfInfinite (constraint.Height));
		}

		protected override void OnRender (DrawingContext drawingContext)
		{
			base.OnRender (drawingContext);
			rendered_geometry = DefiningGeometry;
			Brush fill = Fill;
			if (fill != null || Stroke != null) {
				//drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight)));
				drawingContext.DrawGeometry (fill, CreatePen (), DefiningGeometry);
				//drawingContext.Pop();
			}
		}
		#endregion

		#region Private Methods
		Pen CreatePen ()
		{
			Pen result = new Pen (Stroke, StrokeThickness);
			result.DashStyle = new DashStyle (StrokeDashArray, StrokeDashOffset);
			result.DashCap = StrokeDashCap;
			result.StartLineCap = StrokeStartLineCap;
			result.EndLineCap = StrokeEndLineCap;
			result.LineJoin = StrokeLineJoin;
			result.MiterLimit = StrokeMiterLimit;
			return result;
		}

		static double ZeroIfInfinite (double value)
		{
			return double.IsPositiveInfinity (value) ? 0 : value;
		}
		#endregion
	}
}