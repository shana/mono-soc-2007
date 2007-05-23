using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
namespace Mono.WindowsPresentationFoundation {
	static class Utility {
		static public Size GetSize(Size desired, Size constraint) {
			return new Size(
				GetSize(desired.Width, constraint.Width),
				GetSize(desired.Height, constraint.Height)
			);
		}

		static double GetSize(double desired, double constraint) {
			return constraint == double.PositiveInfinity ? desired : Math.Min(desired, constraint);
		}

		static public Size GetSize(double desired, Size constraint) {
			return GetSize(new Size(desired, desired), constraint);
		}

		static Size GetSize(Size desired, Thickness border) {
			return new Size(
				desired.Width + border.Left + border.Right,
				desired.Height + border.Top + border.Bottom
			);
		}

		static Size GetSize(double desired, Thickness border) {
			return GetSize(new Size(desired, desired), border);
		}

		static public Size GetSize(Size desired, Thickness border, Size constraint) {
			return GetSize(GetSize(desired, border), constraint);
		}

		static public Size GetSize(Size desired, double border, Size constraint) {
			return GetSize(GetSize(desired, new Thickness(border)), constraint);
		}

		static public Size GetSize(double desired, Thickness border, Size constraint) {
			return GetSize(GetSize(desired, border), constraint);
		}

		static public Thickness Add(Thickness value1, Thickness value2) {
			return new Thickness(
				value1.Left + value2.Left,
				value1.Top + value2.Top,
				value1.Right + value2.Right,
				value1.Bottom + value2.Bottom
			);
		}

		static public Rect GetBounds(FrameworkElement element) {
			return new Rect(0, 0, element.ActualWidth, element.ActualHeight);
		}

		static public void DrawBox(DrawingContext drawingContext, double containerWidth, double containerHeight, Brush brush, Thickness thickness) {
			double x = thickness.Left / 2;
			double y = containerHeight;
			drawingContext.DrawLine(new Pen(brush, thickness.Left), new Point(x, 0), new Point(x, y));
			x = containerWidth - thickness.Right / 2;
			drawingContext.DrawLine(new Pen(brush, thickness.Right), new Point(x, 0), new Point(x, y));
			x = containerWidth;
			y = thickness.Top / 2;
			drawingContext.DrawLine(new Pen(brush, thickness.Top), new Point(0, y), new Point(x, y));
			y = containerHeight - thickness.Bottom / 2;
			drawingContext.DrawLine(new Pen(brush, thickness.Bottom), new Point(0, y), new Point(x, y));
		}

		static public void DrawRoundBox(DrawingContext drawingContext, double containerWidth, double containerHeight, Brush brush, Thickness thickness, CornerRadius cornerRadius) {
			//FIXME: The corner line thickness should transition between the thinkness values of the adjacent borders.
			//FIXME: Corner scaling.
			drawingContext.DrawLine(
				new Pen(brush, thickness.Top),
				new Point(cornerRadius.TopLeft, thickness.Top / 2),
				new Point(containerWidth - cornerRadius.TopRight, thickness.Top / 2)
			);
			drawingContext.DrawLine(
				new Pen(brush, thickness.Right),
				new Point(containerWidth - thickness.Right / 2, cornerRadius.TopRight),
				new Point(containerWidth - thickness.Right / 2, containerHeight - cornerRadius.BottomRight)
			);
			drawingContext.DrawLine(
				new Pen(brush, thickness.Bottom),
				new Point(cornerRadius.BottomLeft, containerHeight - thickness.Bottom / 2),
				new Point(containerWidth - cornerRadius.BottomRight, containerHeight - thickness.Bottom / 2)
			);
			drawingContext.DrawLine(
				new Pen(brush, thickness.Left),
				new Point(thickness.Left / 2, cornerRadius.TopLeft),
				new Point(thickness.Left / 2, containerHeight - cornerRadius.BottomLeft)
			);
			drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, cornerRadius.TopLeft, cornerRadius.TopLeft)));
			drawingContext.DrawEllipse(
				null,
				new Pen(brush, (thickness.Left + thickness.Top) / 2),
				new Point(cornerRadius.TopLeft, cornerRadius.TopLeft),
				cornerRadius.TopLeft - thickness.Left / 2, cornerRadius.TopLeft - thickness.Top / 2
			);
			drawingContext.Pop();
			drawingContext.PushClip(new RectangleGeometry(new Rect(containerWidth - cornerRadius.TopRight, 0, cornerRadius.TopRight, cornerRadius.TopRight)));
			drawingContext.DrawEllipse(
				null,
				new Pen(brush, (thickness.Right + thickness.Top) / 2),
				new Point(containerWidth - cornerRadius.TopRight, cornerRadius.TopRight),
				cornerRadius.TopRight - thickness.Right / 2, cornerRadius.TopRight - thickness.Top / 2
			);
			drawingContext.Pop();
			drawingContext.PushClip(new RectangleGeometry(new Rect(0, containerHeight - cornerRadius.BottomLeft, cornerRadius.BottomLeft, cornerRadius.BottomLeft)));
			drawingContext.DrawEllipse(
				null,
				new Pen(brush, (thickness.Left + thickness.Bottom) / 2),
				new Point(cornerRadius.BottomLeft, containerHeight - cornerRadius.BottomLeft),
				cornerRadius.BottomLeft - thickness.Left / 2, cornerRadius.BottomLeft - thickness.Bottom / 2
			);
			drawingContext.Pop();
			drawingContext.PushClip(new RectangleGeometry(new Rect(containerWidth - cornerRadius.BottomRight, containerHeight - cornerRadius.BottomRight, cornerRadius.BottomRight, cornerRadius.BottomRight)));
			drawingContext.DrawEllipse(
				null,
				new Pen(brush, (thickness.Right + thickness.Bottom) / 2),
				new Point(containerWidth - cornerRadius.BottomRight, containerHeight - cornerRadius.BottomRight),
				cornerRadius.BottomRight - thickness.Right / 2, cornerRadius.BottomRight - thickness.Bottom / 2
			);
			drawingContext.Pop();
		}

		static public bool IsVoid(CornerRadius value) {
			return value.TopLeft == 0 && value.TopRight == 0 && value.BottomLeft == 0 && value.BottomRight == 0;
		}

		static public int GetSystemDelay() {
			return 250 * (SystemParameters.KeyboardDelay + 1);
		}

		static public int GetSystemInterval() {
			const double minimum_rate = 2.5;
			const double maximum_rate = 30;
			return (int)(1000 / (minimum_rate + (maximum_rate - minimum_rate) / 31 * SystemParameters.KeyboardSpeed));
		}
		
		static public double GetAdjustedSize(double size) {
			return size >= 0 ? size : 0;
		}

		static public void SetBinding(FrameworkElement target, DependencyProperty targetProperty, object source, string sourceProperty) {
			Binding binding = new Binding(sourceProperty);
			binding.Source = source;
			target.SetBinding(targetProperty, binding);
		}

		static public void DrawLine(DrawingContext drawingContext, bool horizontal, Pen pen, Point location, double lenght) {
			double x = Math.Floor(location.X);
			double y = Math.Floor(location.Y);
			double offset = pen.Thickness/ 2;
			if (horizontal)
				drawingContext.DrawLine(pen, new Point(x, y + offset), new Point(x + lenght, y + offset));
			else
				drawingContext.DrawLine(pen, new Point(x + offset, y), new Point(x + offset, y + lenght));
		}
	}
}