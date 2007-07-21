using NUnit.Framework;
using System.Windows.Media;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class BorderTest {
		[Test]
		public void Creation() {
			Border b = new Border();
			Assert.AreEqual(b.BorderThickness, new Thickness(0), "BorderThickness");
			Assert.AreEqual(b.Padding, new Thickness(0), "Padding");
			Assert.IsFalse(b.SnapsToDevicePixels, "SnapsToDevicePixels");
		}

		#region MeasureOverride
		[Test]
		public void MeasureOverride() {
			new MeasureOverrideBorder();
		}

		class MeasureOverrideBorder : Border {
			public MeasureOverrideBorder() {
				Size result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 0, "Width");
				Assert.AreEqual(result.Height, 0, "Height");
				
				Control child = new Control();
				child.Width = 100;
				child.Height = 100;
				Child = child;
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 100, "Width 1");
				Assert.AreEqual(result.Height, 100, "Height 1");

				Padding = new Thickness(1, 2, 3, 4);
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 104, "Width 2");
				Assert.AreEqual(result.Height, 106, "Height 2");

				BorderThickness = new Thickness(1, 2, 3, 4);
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 108, "Width 3");
				Assert.AreEqual(result.Height, 112, "Height 3");

				Padding = new Thickness(0);
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 104, "Width 4");
				Assert.AreEqual(result.Height, 106, "Height 4");

				Child = null;
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 4, "Width 5");
				Assert.AreEqual(result.Height, 6, "Height 5");

				Padding = new Thickness(1, 2, 3, 4);
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 8, "Width 6");
				Assert.AreEqual(result.Height, 12, "Height 6");

				Padding = new Thickness(0);
				BorderThickness = new Thickness(0);
				child = new Control();
				Child = child;
				MeasureOverride(new Size(100, 100));
				Assert.AreEqual(child.ActualWidth, 0, "Width 7");
				Assert.AreEqual(child.ActualHeight, 0, "Height 7");
				Assert.AreEqual(child.DesiredSize.Width, 0, "Width 8");
				Assert.AreEqual(child.DesiredSize.Height, 0, "Height 8");
				Window window = new Window();
				window.Show();
				Canvas canvas = new Canvas();
				window.Content = canvas;
				canvas.Children.Add(this);
				result = MeasureOverride(new Size(100, 100));
				Assert.AreEqual(child.ActualWidth, 0, "Width 9");
				Assert.AreEqual(child.ActualHeight, 0, "Height 9");
				Assert.AreEqual(child.DesiredSize.Width, 0, "Width 10");
				Assert.AreEqual(child.DesiredSize.Height, 0, "Height 10");
				Assert.AreEqual(result.Width, 0, "Width 11");
				Assert.AreEqual(result.Height, 0, "Height 11");
				Measure(new Size(100, 100));
				Assert.AreEqual(child.ActualWidth, 0, "Width 12");
				Assert.AreEqual(child.ActualHeight, 0, "Height 12");
				Assert.AreEqual(child.DesiredSize.Width, 0, "Width 13");
				Assert.AreEqual(child.DesiredSize.Height, 0, "Height 13");
				ArrangeOverride(new Size(100, 100));
				Assert.AreEqual(child.ActualWidth, 100, "Width 14");
				Assert.AreEqual(child.ActualHeight, 100, "Height 14");
				Assert.AreEqual(child.DesiredSize.Width, 0, "Width 15");
				Assert.AreEqual(child.DesiredSize.Height, 0, "Height 15");
			}
		}
		#endregion

		#region ChildMeasureCalled
		[Test]
		public void ChildMeasureCalled() {
			new ChildMeasureCalledBorder();
		}

		class ChildMeasureCalledBorder : Border {
			public ChildMeasureCalledBorder() {
				Child = new ChildBorder();
				ChildBorder.SetMeasureOverrideCalled = true;
				MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				ChildBorder.SetMeasureOverrideCalled = false;
				Assert.IsTrue(ChildBorder.MeasureOverrideCalled);
			}

			class ChildBorder : Border {
				static public bool SetMeasureOverrideCalled;
				static public bool MeasureOverrideCalled;
				
				protected override Size MeasureOverride(Size constraint) {
					if (SetMeasureOverrideCalled)
						MeasureOverrideCalled = true;
					return new Size(0, 0);
				}
			}
		}
		#endregion

		#region Drawing
		[Test]
		public void Drawing() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			b.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			b.BorderThickness = new Thickness(1);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 2, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(((SolidColorBrush)gd.Pen.Brush).Color, ((SolidColorBrush)b.BorderBrush).Color, "3");
			Assert.AreEqual(gd.Pen.Thickness, 1, "4");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 0.5, 99, 99), "5");
			Assert.AreEqual(rg.RadiusX, 0, "6");
			Assert.AreEqual(rg.RadiusY, 0, "7");

			gd = (GeometryDrawing)drawing_group.Children[1];
			Assert.IsNull(gd.Pen, "8");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.Background).Color, "9");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1, 1, 98, 98), "10");
			Assert.AreEqual(rg.RadiusX, 0, "11");
			Assert.AreEqual(rg.RadiusY, 0, "12");
		}

		[Test]
		public void DrawingNoBorder() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 1, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.IsNull(gd.Pen, "2");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.Background).Color, "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0, 0, 100, 100), "5");
			Assert.AreEqual(rg.RadiusX, 0, "6");
			Assert.AreEqual(rg.RadiusY, 0, "7");
		}

		[Test]
		public void DrawingCornerRadius() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			b.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			b.BorderThickness = new Thickness(1);
			b.CornerRadius = new CornerRadius(1);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 2, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(((SolidColorBrush)gd.Pen.Brush).Color, ((SolidColorBrush)b.BorderBrush).Color, "3");
			Assert.AreEqual(gd.Pen.Thickness, 1, "4");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 0.5, 99, 99), "5");
			Assert.AreEqual(rg.RadiusX, 1, "6");
			Assert.AreEqual(rg.RadiusY, 1, "7");

			gd = (GeometryDrawing)drawing_group.Children[1];
			Assert.IsNull(gd.Pen, "8");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.Background).Color, "9");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1, 1, 98, 98), "10");
			Assert.AreEqual(rg.RadiusX, 0.5, "11");
			Assert.AreEqual(rg.RadiusY, 0.5, "12");
		}

		[Test]
		public void DrawingCornerRadius2() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			b.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			b.BorderThickness = new Thickness(1);
			b.CornerRadius = new CornerRadius(1.25);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 2, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(((SolidColorBrush)gd.Pen.Brush).Color, ((SolidColorBrush)b.BorderBrush).Color, "3");
			Assert.AreEqual(gd.Pen.Thickness, 1, "4");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 0.5, 99, 99), "5");
			Assert.AreEqual(rg.RadiusX, 1.25, "6");
			Assert.AreEqual(rg.RadiusY, 1.25, "7");

			gd = (GeometryDrawing)drawing_group.Children[1];
			Assert.IsNull(gd.Pen, "8");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.Background).Color, "9");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1, 1, 98, 98), "10");
			Assert.AreEqual(rg.RadiusX, 0.75, "11");
			Assert.AreEqual(rg.RadiusY, 0.75, "12");
		}

		[Test]
		public void DrawingCornerRadius3() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			b.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			b.BorderThickness = new Thickness(1);
			b.CornerRadius = new CornerRadius(5);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 2, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.IsNull(gd.Brush, "2");
			Assert.AreEqual(((SolidColorBrush)gd.Pen.Brush).Color, ((SolidColorBrush)b.BorderBrush).Color, "3");
			Assert.AreEqual(gd.Pen.Thickness, 1, "4");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 0.5, 99, 99), "5");
			Assert.AreEqual(rg.RadiusX, 5, "6");
			Assert.AreEqual(rg.RadiusY, 5, "7");

			gd = (GeometryDrawing)drawing_group.Children[1];
			Assert.IsNull(gd.Pen, "8");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.Background).Color, "9");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1, 1, 98, 98), "10");
			Assert.AreEqual(rg.RadiusX, 4.5, "11");
			Assert.AreEqual(rg.RadiusY, 4.5, "12");
		}
		
		[Test]
		public void DrawingNonUniformBorderBrushNonUniformCornerRadius() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			b.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			b.BorderThickness = new Thickness(1, 2, 3, 4);
			b.CornerRadius = new CornerRadius(1, 2, 3, 4);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 2, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.BorderBrush).Color, "2");
			Assert.IsNull(gd.Pen, "3");
			Assert.AreEqual(gd.Geometry.ToString(), "M1,5;0L96,5;0A3,5;3;0;0;1;100;3L100;95A4,5;5;0;0;1;95,5;100L4,5;100A4,5;6;0;0;1;0;94L0;2A1,5;2;0;0;1;1,5;0z M1,5;2L96,5;2A0,5;1;0;0;1;97;3L97;95A1,5;1;0;0;1;95,5;96L4,5;96A3,5;2;0;0;1;1;94L1;2A0,5;0;0;0;1;1,5;2z", "4");

			gd = (GeometryDrawing)drawing_group.Children[1];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.Background).Color, "5");
			Assert.IsNull(gd.Pen, "6");
			Assert.AreEqual(gd.Geometry.ToString(), "M1,5;2L96,5;2A0,5;1;0;0;1;97;3L97;95A1,5;1;0;0;1;95,5;96L4,5;96A3,5;2;0;0;1;1;94L1;2A0,5;0;0;0;1;1,5;2z", "7");
		}

		[Test]
		public void DrawingNonUniformBorderBrushCornerRadius() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			b.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			b.BorderThickness = new Thickness(1, 2, 3, 4);
			b.CornerRadius = new CornerRadius(1);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 2, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.BorderBrush).Color, "2");
			Assert.IsNull(gd.Pen, "3");
			Assert.AreEqual(gd.Geometry.ToString(), "M1,5;0L97,5;0A2,5;2;0;0;1;100;2L100;97A2,5;3;0;0;1;97,5;100L1,5;100A1,5;3;0;0;1;0;97L0;2A1,5;2;0;0;1;1,5;0z M1,5;2L97;2 97;96 1,5;96A0,5;0;0;0;1;1;96L1;2A0,5;0;0;0;1;1,5;2z", "4");

			gd = (GeometryDrawing)drawing_group.Children[1];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.Background).Color, "5");
			Assert.IsNull(gd.Pen, "6");
			Assert.AreEqual(gd.Geometry.ToString(), "M1,5;2L97;2 97;96 1,5;96A0,5;0;0;0;1;1;96L1;2A0,5;0;0;0;1;1,5;2z", "7");
		}

		[Test]
		public void DrawingNonUniformBorderBrushCornerRadius2() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.Background = Brushes.Red;
			b.BorderBrush = Brushes.Green;
			b.BorderThickness = new Thickness(1, 2, 3, 4);
			b.CornerRadius = new CornerRadius(1);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 2, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.BorderBrush).Color, "2");
			Assert.IsNull(gd.Pen, "3");
			Assert.AreEqual(gd.Geometry.ToString(), "M1,5;0L97,5;0A2,5;2;0;0;1;100;2L100;97A2,5;3;0;0;1;97,5;100L1,5;100A1,5;3;0;0;1;0;97L0;2A1,5;2;0;0;1;1,5;0z M1,5;2L97;2 97;96 1,5;96A0,5;0;0;0;1;1;96L1;2A0,5;0;0;0;1;1,5;2z", "4");

			gd = (GeometryDrawing)drawing_group.Children[1];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.Background).Color, "5");
			Assert.IsNull(gd.Pen, "6");
			Assert.AreEqual(gd.Geometry.ToString(), "M1,5;2L97;2 97;96 1,5;96A0,5;0;0;0;1;1;96L1;2A0,5;0;0;0;1;1,5;2z", "7");
		}

		[Test]
		public void DrawingNonUniformBorderBrushCornerRadius3() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.Background = Brushes.Red;
			b.BorderBrush = Brushes.Green;
			b.BorderThickness = new Thickness(1, 2, 3, 4);
			b.CornerRadius = new CornerRadius(10);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 2, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.BorderBrush).Color, "2");
			Assert.IsNull(gd.Pen, "3");
			Assert.AreEqual(gd.Geometry.ToString(), "M10,5;0L88,5;0A11,5;11;0;0;1;100;11L100;88A11,5;12;0;0;1;88,5;100L10,5;100A10,5;12;0;0;1;0;88L0;11A10,5;11;0;0;1;10,5;0z M10,5;2L88,5;2A8,5;9;0;0;1;97;11L97;88A8,5;8;0;0;1;88,5;96L10,5;96A9,5;8;0;0;1;1;88L1;11A9,5;9;0;0;1;10,5;2z", "4");

			gd = (GeometryDrawing)drawing_group.Children[1];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.Background).Color, "5");
			Assert.IsNull(gd.Pen, "6");
			Assert.AreEqual(gd.Geometry.ToString(), "M10,5;2L88,5;2A8,5;9;0;0;1;97;11L97;88A8,5;8;0;0;1;88,5;96L10,5;96A9,5;8;0;0;1;1;88L1;11A9,5;9;0;0;1;10,5;2z", "7");
		}

		[Test]
		public void DrawingNonUniformBorderBrushCornerRadius4() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.Background = Brushes.Red;
			b.BorderBrush = Brushes.Green;
			b.BorderThickness = new Thickness(1, 2, 3, 4);
			b.CornerRadius = new CornerRadius(2);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 2, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.BorderBrush).Color, "2");
			Assert.IsNull(gd.Pen, "3");
			Assert.AreEqual(gd.Geometry.ToString(), "M2,5;0L96,5;0A3,5;3;0;0;1;100;3L100;96A3,5;4;0;0;1;96,5;100L2,5;100A2,5;4;0;0;1;0;96L0;3A2,5;3;0;0;1;2,5;0z M2,5;2L96,5;2A0,5;1;0;0;1;97;3L97;96A0,5;0;0;0;1;96,5;96L2,5;96A1,5;0;0;0;1;1;96L1;3A1,5;1;0;0;1;2,5;2z", "4");

			gd = (GeometryDrawing)drawing_group.Children[1];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.Background).Color, "5");
			Assert.IsNull(gd.Pen, "6");
			Assert.AreEqual(gd.Geometry.ToString(), "M2,5;2L96,5;2A0,5;1;0;0;1;97;3L97;96A0,5;0;0;0;1;96,5;96L2,5;96A1,5;0;0;0;1;1;96L1;3A1,5;1;0;0;1;2,5;2z", "7");
		}

		[Test]
		public void DrawingBorderBrushNonUniformCornerRadius() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			b.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			b.BorderThickness = new Thickness(1);
			b.CornerRadius = new CornerRadius(1, 2, 3, 4);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 2, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.BorderBrush).Color, "2");
			Assert.IsNull(gd.Pen, "3");
			Assert.AreEqual(gd.Geometry.ToString(), "M1,5;0L97,5;0A2,5;2,5;0;0;1;100;2,5L100;96,5A3,5;3,5;0;0;1;96,5;100L4,5;100A4,5;4,5;0;0;1;0;95,5L0;1,5A1,5;1,5;0;0;1;1,5;0z M1,5;1L97,5;1A1,5;1,5;0;0;1;99;2,5L99;96,5A2,5;2,5;0;0;1;96,5;99L4,5;99A3,5;3,5;0;0;1;1;95,5L1;1,5A0,5;0,5;0;0;1;1,5;1z", "4");

			gd = (GeometryDrawing)drawing_group.Children[1];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.Background).Color, "5");
			Assert.IsNull(gd.Pen, "6");
			Assert.AreEqual(gd.Geometry.ToString(), "M1,5;1L97,5;1A1,5;1,5;0;0;1;99;2,5L99;96,5A2,5;2,5;0;0;1;96,5;99L4,5;99A3,5;3,5;0;0;1;1;95,5L1;1,5A0,5;0,5;0;0;1;1,5;1z", "7");
		}

		[Test]
		public void DrawingBorderBrushNonUniformCornerRadius2() {
			Border b = new Border();
			b.Width = 10000;
			b.Height = 10000;
			b.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			b.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			b.BorderThickness = new Thickness(1, 5, 10, 50);
			b.CornerRadius = new CornerRadius(100, 500, 1000, 5000);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 2, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.BorderBrush).Color, "2");
			Assert.IsNull(gd.Pen, "3");
			Assert.AreEqual(gd.Geometry.ToString(), "M100,5;0L9495;0A505;502,5;0;0;1;10000;502,5L10000;8975A1005;1025;0;0;1;8995;10000L5000,5;10000A5000,5;5025;0;0;1;0;4975L0;102,5A100,5;102,5;0;0;1;100,5;0z M100,5;5L9495;5A495;497,5;0;0;1;9990;502,5L9990;8975A995;975;0;0;1;8995;9950L5000,5;9950A4999,5;4975;0;0;1;1;4975L1;102,5A99,5;97,5;0;0;1;100,5;5z", "4");

			gd = (GeometryDrawing)drawing_group.Children[1];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.Background).Color, "5");
			Assert.IsNull(gd.Pen, "6");
			Assert.AreEqual(gd.Geometry.ToString(), "M100,5;5L9495;5A495;497,5;0;0;1;9990;502,5L9990;8975A995;975;0;0;1;8995;9950L5000,5;9950A4999,5;4975;0;0;1;1;4975L1;102,5A99,5;97,5;0;0;1;100,5;5z", "7");
		}

		#region DrawingBorderBrushNonUniformBorderThickness
		[Test]
		public void DrawingBorderBrushNonUniformBorderThickness() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.Background = Brushes.Red;
			b.BorderBrush = Brushes.Green;
			b.BorderThickness = new Thickness(1, 2, 3, 4);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 5, "1");
			DrawingBorderBrushNonUniformBorderThicknessTestLine(drawing_group.Children[0], 1, 0.5, 0, 0.5, 100, "1");
			DrawingBorderBrushNonUniformBorderThicknessTestLine(drawing_group.Children[1], 3, 98.5, 0, 98.5, 100, "2");
			DrawingBorderBrushNonUniformBorderThicknessTestLine(drawing_group.Children[2], 2, 0, 1, 100, 1, "3");
			DrawingBorderBrushNonUniformBorderThicknessTestLine(drawing_group.Children[3], 4, 0, 98, 100, 98, "4");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[4];
			Assert.IsNull(gd.Pen, "8");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.Background).Color, "9");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1, 2, 96, 94), "10");
			Assert.AreEqual(rg.RadiusX, 0, "11");
			Assert.AreEqual(rg.RadiusY, 0, "12");
		}

		void DrawingBorderBrushNonUniformBorderThicknessTestLine(Drawing drawing, double pen_thickness, double start_x, double start_y, double end_x, double end_y, string message) {
			GeometryDrawing gd = (GeometryDrawing)drawing;
			Assert.IsNull(gd.Brush, message + " 1");
			Assert.AreEqual(gd.Pen.Thickness, pen_thickness, message + " 2");
			Assert.AreEqual(((SolidColorBrush)gd.Pen.Brush).Color, Colors.Green, message + " 3");
			LineGeometry lg = (LineGeometry)gd.Geometry;
			Assert.AreEqual(lg.StartPoint, new Point(start_x, start_y), message + " 4");
			Assert.AreEqual(lg.EndPoint, new Point(end_x, end_y), message + " 5");
		}
		#endregion

		[Test]
		public void DrawingBorderBrushNonUniformBorderThickness2() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			b.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			b.BorderThickness = new Thickness(1, 2, 3, 4);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 2, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.BorderBrush).Color, "2");
			Assert.IsNull(gd.Pen, "3");
			Assert.AreEqual(gd.Geometry.ToString(), "M0;0L100;0 100;100 0;100 0;0z M1;2L97;2 97;96 1;96 1;2z", "4");

			gd = (GeometryDrawing)drawing_group.Children[1];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)b.Background).Color, "5");
			Assert.IsNull(gd.Pen, "6");
			Assert.AreEqual(gd.Geometry.ToString(), "M1;2L97;2 97;96 1;96 1;2z", "7");
		}

		[Test]
		public void DrawingBorderBrushNonUniformBorderThickness3() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.BorderBrush = Brushes.Green;
			b.BorderThickness = new Thickness(1, 2, 3, 4);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 4);
		}

		[Test]
		public void DrawingBorderBrushNonUniformBorderThickness4() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.BorderBrush = new SolidColorBrush(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF));
			b.BorderThickness = new Thickness(1, 2, 3, 4);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 1);
		}

		[Test]
		public void DrawingBorderBrushNonUniformBorderThickness5() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFE, 0xFF, 0xFF, 0xFF));
			b.BorderThickness = new Thickness(1, 2, 3, 4);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 1);
		}

		[Test]
		public void DrawingBorderBrushNonUniformBorderThickness6() {
			Border b = new Border();
			b.Width = 100;
			b.Height = 100;
			b.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
			b.BorderThickness = new Thickness(1, 2, 3, 4);
			Window w = new Window();
			w.Content = b;
			w.Show();

			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(b);
			Assert.AreEqual(drawing_group.Children.Count, 4);
		}
		#endregion
	}
}