//
// Utility.cs
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
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
namespace Mono.WindowsPresentationFoundation
{
	static class Utility
	{
		static public Size GetSize (Size desired, Size constraint)
		{
			return new Size (
				GetSize (desired.Width, constraint.Width),
				GetSize (desired.Height, constraint.Height)
			);
		}

		static double GetSize (double desired, double constraint)
		{
			return constraint == double.PositiveInfinity ? desired : Math.Min (desired, constraint);
		}

		static public Size GetSize (double desired, Size constraint)
		{
			return GetSize (new Size (desired, desired), constraint);
		}

		static Size GetSize (Size desired, Thickness border)
		{
			return new Size (
				desired.Width + border.Left + border.Right,
				desired.Height + border.Top + border.Bottom
			);
		}

		static Size GetSize (double desired, Thickness border)
		{
			return GetSize (new Size (desired, desired), border);
		}

		static public Size GetSize (Size desired, Thickness border, Size constraint)
		{
			return GetSize (GetSize (desired, border), constraint);
		}

		static public Size GetSize (Size desired, double border, Size constraint)
		{
			return GetSize (GetSize (desired, new Thickness (border)), constraint);
		}

		static public Size GetSize (double desired, Thickness border, Size constraint)
		{
			return GetSize (GetSize (desired, border), constraint);
		}

		static public Thickness Add (Thickness value1, Thickness value2)
		{
			return new Thickness (
				value1.Left + value2.Left,
				value1.Top + value2.Top,
				value1.Right + value2.Right,
				value1.Bottom + value2.Bottom
			);
		}

		static public Rect GetBounds (FrameworkElement element)
		{
			return new Rect (0, 0, element.ActualWidth, element.ActualHeight);
		}

		static public bool IsVoid (CornerRadius value)
		{
			return value.TopLeft == 0 && value.TopRight == 0 && value.BottomLeft == 0 && value.BottomRight == 0;
		}

		static public int GetSystemDelay ()
		{
			return 250 * (SystemParameters.KeyboardDelay + 1);
		}

		static public int GetSystemInterval ()
		{
			const double minimum_rate = 2.5;
			const double maximum_rate = 30;
			return (int)(1000 / (minimum_rate + (maximum_rate - minimum_rate) / 31 * SystemParameters.KeyboardSpeed));
		}

		static public double GetAdjustedSize (double size)
		{
			return size >= 0 ? size : 0;
		}

		static public void SetBinding (FrameworkElement target, DependencyProperty targetProperty, object source, string sourceProperty)
		{
			Binding binding = new Binding (sourceProperty);
			binding.Source = source;
			target.SetBinding (targetProperty, binding);
		}

		static public void DrawLine (DrawingContext drawingContext, bool horizontal, Pen pen, Point location, double lenght)
		{
			double x = Math.Floor (location.X);
			double y = Math.Floor (location.Y);
			double offset = pen.Thickness / 2;
			if (horizontal)
				drawingContext.DrawLine (pen, new Point (x, y + offset), new Point (x + lenght, y + offset));
			else
				drawingContext.DrawLine (pen, new Point (x + offset, y), new Point (x + offset, y + lenght));
		}

		static public void Hang ()
		{
			//TODO Prevent optimization.
			while (true) {
			}
		}

		/// <summary>
		/// To be called by tests for Luna theme when running on Microsoft classes on Windows Vista. (Implemented classes will load the Luna theme on Windows Vista automatically).
		/// </summary>
		public static void LoadLunaTheme ()
		{
			const int WindowsVistaMajorVersion = 6;
			if (Environment.OSVersion.Version.Major == WindowsVistaMajorVersion)
				Application.Current.Resources.MergedDictionaries.Add (Application.LoadComponent (new Uri ("PresentationFramework.Luna;V3.0.0.0;31bf3856ad364e35;component\\themes/luna.normalcolor.xaml", UriKind.Relative)) as ResourceDictionary);
		}

		public static bool IsInVisibleWindow (DependencyObject visual)
		{
			DependencyObject parent = visual;
			for (; ; ) {
				parent = VisualTreeHelper.GetParent (parent);
				Window window = parent as Window;
				if (window != null)
					return window.Visibility == Visibility.Visible;
				if (parent == null)
					return false;
			}
		}

		public static bool IsUniform (Thickness thickness)
		{
			return thickness.Bottom == thickness.Left && thickness.Bottom == thickness.Right && thickness.Bottom == thickness.Top;
		}

		#region Decorator layout
		static Size GetDesiredDecoratorSize (Size desiredChildSize, double borderThickness)
		{
			return new Size (desiredChildSize.Width + 2 * borderThickness, desiredChildSize.Height + 2 * borderThickness);
		}

		static double GetMeasureSizeToPassToDecoratorChild (double constraint, double borderThickness)
		{
			if (double.IsPositiveInfinity (constraint))
				return double.PositiveInfinity;
			return Math.Max (constraint - 2 * borderThickness, 0);
		}

		static Size GetMeasureSizeToPassToDecoratorChild (Size constraint, double borderThickness)
		{
			return new Size (GetMeasureSizeToPassToDecoratorChild (constraint.Width, borderThickness), GetMeasureSizeToPassToDecoratorChild (constraint.Height, borderThickness));
		}

		public static Size MeasureDecoratorChild (UIElement child, Size constraint, double borderThickness)
		{
			if (child == null)
				return Size.Empty;
			child.Measure (GetMeasureSizeToPassToDecoratorChild (constraint, borderThickness));
			return GetDesiredDecoratorSize (child.DesiredSize, borderThickness);
		}

		static double GetArrangeSizeToPassToDecoratorChild (double constraint, double borderThickness)
		{
			return Math.Max (constraint - 2 * borderThickness, 0);
		}

		public static Rect GetArrangeRectToPassToDecoratorChild (Size constraint, double borderThickness)
		{
			return new Rect (borderThickness, borderThickness, GetArrangeSizeToPassToDecoratorChild (constraint.Width, borderThickness), GetArrangeSizeToPassToDecoratorChild (constraint.Height, borderThickness));
		}

		public static Size ArrangeDecoratorChild (UIElement child, Size constraint, double borderThickness)
		{
			if (child != null) {
				double actualBorderThickness = Utility.GetActualDecoratorBorderThickness (borderThickness, constraint.Width, constraint.Height);
				child.Arrange (GetArrangeRectToPassToDecoratorChild (constraint, actualBorderThickness));
			}
			return constraint;
		}

		static double GetActualDecoratorBorderThickness (double borderThickness, double size)
		{
			return Math.Min (size / 2, borderThickness);
		}

		public static double GetActualDecoratorBorderThickness (double borderThickness, double width, double height)
		{
			return Math.Min (GetActualDecoratorBorderThickness (borderThickness, width), GetActualDecoratorBorderThickness (borderThickness, height));
		}
		#endregion
	}
}