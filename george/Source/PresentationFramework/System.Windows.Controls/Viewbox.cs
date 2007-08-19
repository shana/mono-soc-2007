//
// Viewbox.cs
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
using System.Collections;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	public class Viewbox : Decorator
	{
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register ("StretchDirection", typeof (StretchDirection), typeof (Viewbox), new FrameworkPropertyMetadata (StretchDirection.Both, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty StretchProperty = DependencyProperty.Register ("Stretch", typeof (Stretch), typeof (Viewbox), new FrameworkPropertyMetadata (Stretch.Uniform, FrameworkPropertyMetadataOptions.AffectsMeasure));
		#endregion
		#endregion

		#region Private Fields
		ContainerVisual container_visual = new ContainerVisual ();
		UIElement child;
		#endregion

		#region Public Constructors
		public Viewbox ()
		{
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public StretchDirection StretchDirection {
			get { return (StretchDirection)GetValue (StretchDirectionProperty); }
			set { SetValue (StretchDirectionProperty, value); }
		}

		public Stretch Stretch {
			get { return (Stretch)GetValue (StretchProperty); }
			set { SetValue (StretchProperty, value); }
		}
		#endregion

		public override UIElement Child {
			get { return child; }
			set {
				if (child != null)
					container_visual.Children.Remove (child);
				child = value;
				container_visual.Children.Add (value);
			}
		}
		#endregion

		#region Protected Properties
		protected override IEnumerator LogicalChildren {
			get {
				return (child == null ? new object [] { } : new object [] { child }).GetEnumerator ();
			}
		}

		protected override int VisualChildrenCount {
			get { return 1; }
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride (Size arrangeSize)
		{
			if (child == null)
				return arrangeSize;
			Size child_desired_size = child.DesiredSize;
			child.Arrange (new Rect (new Point (0, 0), child_desired_size));
			if (child_desired_size.Width == 0 && child_desired_size.Height == 0)
				return child_desired_size;
			double scale_x;
			double scale_y;
			switch (Stretch) {
			case Stretch.None:
				scale_x = scale_y = 1;
				break;
			case Stretch.Fill:
				scale_x = arrangeSize.Width / child_desired_size.Width;
				scale_y = arrangeSize.Height / child_desired_size.Height;
				break;
			case Stretch.Uniform:
				scale_x = scale_y = Math.Min (arrangeSize.Width / child_desired_size.Width, arrangeSize.Height / child_desired_size.Height);
				break;
			default:
				scale_x = scale_y = Math.Max (arrangeSize.Width / child_desired_size.Width, arrangeSize.Height / child_desired_size.Height);
				break;
			}
			switch (StretchDirection) {
			case StretchDirection.DownOnly:
				if (scale_x > 1 || scale_y > 1)
					scale_x = scale_y = 1;
				break;
			case StretchDirection.UpOnly:
				if (scale_x < 1 || scale_y < 1)
					scale_x = scale_y = 1;
				break;
			}
			container_visual.Transform = new ScaleTransform (scale_x, scale_y);
			return new Size (child_desired_size.Width * scale_x, child_desired_size.Height * scale_y);
		}

		protected override Visual GetVisualChild (int index)
		{
			if (index == 0)
				return container_visual;
			return base.GetVisualChild (-1);
		}

		protected override Size MeasureOverride (Size constraint)
		{
			if (child == null)
				return new Size (0, 0);
			child.Measure (new Size (double.PositiveInfinity, double.PositiveInfinity));
			Size child_desired_size = child.DesiredSize;
			switch (Stretch) {
			case Stretch.None:
				return child_desired_size;
			case Stretch.Fill:
				if (child_desired_size.Width == 0 && child_desired_size.Height == 0)
					return child_desired_size;
				Size result = new Size (double.IsPositiveInfinity (constraint.Width) ? child_desired_size.Width : constraint.Width, double.IsPositiveInfinity (constraint.Height) ? child_desired_size.Height : constraint.Height);
				switch (StretchDirection) {
				case StretchDirection.DownOnly:
					if (result.Width > child_desired_size.Width)
						result.Width = child_desired_size.Width;
					if (result.Height > child_desired_size.Height)
						result.Height = child_desired_size.Height;
					break;
				case StretchDirection.UpOnly:
					if (result.Width < child_desired_size.Width)
						result.Width = child_desired_size.Width;
					if (result.Height < child_desired_size.Height)
						result.Height = child_desired_size.Height;
					break;
				}
				return result;
			default:
				double scale_x = child_desired_size.Width == 0 ? double.NaN : double.IsPositiveInfinity (constraint.Width) ? double.NaN : constraint.Width / child_desired_size.Width;
				double scale_y = child_desired_size.Height == 0 ? double.NaN : double.IsPositiveInfinity (constraint.Height) ? double.NaN : constraint.Height / child_desired_size.Height;
				double scale;
				if (double.IsNaN (scale_x))
					if (double.IsNaN (scale_y))
						scale = 1;
					else
						scale = scale_y;
				else
					if (double.IsNaN (scale_y))
						scale = scale_x;
					else
						if (Stretch == Stretch.Uniform)
							scale = Math.Min (scale_x, scale_y);
						else
							scale = Math.Max (scale_x, scale_y);
				switch (StretchDirection) {
				case StretchDirection.DownOnly:
					if (scale > 1)
						scale = 1;
					break;
				case StretchDirection.UpOnly:
					if (scale < 1)
						scale = 1;
					break;
				}
				return new Size (child_desired_size.Width * scale, child_desired_size.Height * scale);
			}
		}
		#endregion
	}
}