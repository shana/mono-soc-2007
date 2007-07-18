using NUnit.Framework;
using System;
using System.Windows;
using System.Windows.Media;
#if Implementation
using Microsoft.Windows.Themes;
namespace Mono.Microsoft.Windows.Themes {
#else
namespace Microsoft.Windows.Themes {
#endif
	[TestFixture]
	public class BulletChromeTest {
		[Test]
		public void StaticPropertiesDefaultValues() {
			Assert.IsNull(BulletChrome.BackgroundProperty.DefaultMetadata.DefaultValue, "Background");
			Assert.IsNull(BulletChrome.BorderBrushProperty.DefaultMetadata.DefaultValue, "BorderBrush");
			Assert.AreEqual(BulletChrome.BorderThicknessProperty.DefaultMetadata.DefaultValue, new Thickness(0), "BorderThickness");
			Assert.IsFalse((bool)BulletChrome.IsCheckedProperty.DefaultMetadata.DefaultValue, "IsChecked");
			Assert.IsFalse((bool)BulletChrome.IsRoundProperty.DefaultMetadata.DefaultValue, "IsRound");
			Assert.IsFalse((bool)BulletChrome.RenderMouseOverProperty.DefaultMetadata.DefaultValue, "RenderMouseOver");
			Assert.IsFalse((bool)BulletChrome.RenderPressedProperty.DefaultMetadata.DefaultValue, "RenderPressed");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void BorderThickness() {
			BulletChrome b = new BulletChrome();
			Thickness t = new Thickness(-1);
			b.BorderThickness = t;
		}

		[Test]
		public void Creation() {
			BulletChrome b = new BulletChrome();
			Assert.IsFalse(b.SnapsToDevicePixels, "SnapsToDevicePixels");
		}

		[Test]
		public void Size() {
			BulletChrome b = new BulletChrome();
			b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(b.DesiredSize.Width, 11, "Width");
			Assert.AreEqual(b.DesiredSize.Height, 11, "Height");
			b.Measure(new Size(1, 1));
			Assert.AreEqual(b.DesiredSize.Width, 1, "Width with constraint");
			Assert.AreEqual(b.DesiredSize.Height, 1, "Height with constraint");
			b.Measure(new Size(1, 50));
			Assert.AreEqual(b.DesiredSize.Width, 1, "Width with constraint 2");
			Assert.AreEqual(b.DesiredSize.Height, 11, "Height with constraint 2");
			b.BorderThickness = new Thickness(1, 2, 3, 4);
			b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(b.DesiredSize.Width, 15, "Width, border");
			Assert.AreEqual(b.DesiredSize.Height, 17, "Height, border");
		}

		[Test]
		public void RoundSize() {
			BulletChrome b = new BulletChrome();
			b.IsRound = true;
			b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(b.DesiredSize.Width, 13, "Width");
			Assert.AreEqual(b.DesiredSize.Height, 13, "Height");
			b.Measure(new Size(1, 1));
			Assert.AreEqual(b.DesiredSize.Width, 1, "Width with constraint");
			Assert.AreEqual(b.DesiredSize.Height, 1, "Height with constraint");
			b.Measure(new Size(1, 50));
			Assert.AreEqual(b.DesiredSize.Width, 1, "Width with constraint 2");
			Assert.AreEqual(b.DesiredSize.Height, 13, "Height with constraint 2");
			b.BorderThickness = new Thickness(1, 2, 3, 4);
			b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(b.DesiredSize.Width, 13, "Width, border");
			Assert.AreEqual(b.DesiredSize.Height, 13, "Height, border");
			b.BorderThickness = new Thickness(10);
			b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(b.DesiredSize.Width, 13, "Width, border 2");
			Assert.AreEqual(b.DesiredSize.Height, 13, "Height, border 2");
		}

		[Test]
		public void MeasureOverride() {
			BulletChrome b = new BulletChrome();
			b.Measure(new Size(double.PositiveInfinity, 10));
			Assert.AreEqual(b.DesiredSize.Width, 11, "Width");
			Assert.AreEqual(b.DesiredSize.Height, 10, "Height");
		}

		[Test]
		public void Drawing() {
			BulletChrome c = new BulletChrome();
			Window w = new Window();
			w.Content = c;
			w.Show();
			Assert.IsNull(VisualTreeHelper.GetDrawing(c));
		}

		[Test]
		public void DrawingBackgroundBorderBrushBorderThicknessIsChecked() {
			BulletChrome c = new BulletChrome();
			c.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.BorderThickness = new Thickness(1);
			c.IsChecked = true;
			c.Width = 100;
			c.Height = 100;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(drawing_group.Children.Count, 3, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)c.Background).Color, "2");
			Assert.IsNull(gd.Pen, "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1, 1, 98, 98), "4");
			Assert.AreEqual(rg.RadiusX, 0, "5");
			Assert.AreEqual(rg.RadiusY, 0, "6");

			gd = (GeometryDrawing)drawing_group.Children[1];

			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, Color.FromArgb(0xFF, 0x21, 0xA1, 0x21), "7");
			Assert.IsNull(gd.Pen, "8");
			StreamGeometry sg = (StreamGeometry)gd.Geometry;
			Assert.AreEqual(sg.ToString(), "M3;5L3;7,8 5,5;10,4 10,1;5,8 10,1;3 5,5;7,6z", "9");

			gd = (GeometryDrawing)drawing_group.Children[2];
			Assert.IsNull(gd.Brush, "10");
			Assert.AreEqual(gd.Pen.Thickness, 1, "11");
			Assert.AreEqual(((SolidColorBrush)gd.Pen.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "12");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(0.5, 0.5, 99, 99), "13");
			Assert.AreEqual(rg.RadiusX, 0, "14");
			Assert.AreEqual(rg.RadiusY, 0, "15");
		}

		[Test]
		public void DrawingBackgroundBorderBrushNonUniformBorderThicknessIsChecked() {
			BulletChrome c = new BulletChrome();
			c.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.BorderThickness = new Thickness(1, 2, 3, 4);
			c.IsChecked = true;
			c.Width = 100;
			c.Height = 100;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(drawing_group.Children.Count, 3, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)c.Background).Color, "2");
			Assert.IsNull(gd.Pen, "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1, 2, 96, 94), "4");
			Assert.AreEqual(rg.RadiusX, 0, "5");
			Assert.AreEqual(rg.RadiusY, 0, "6");

			Assert.AreEqual(((DrawingGroup)drawing_group.Children[1]).Children.Count, 1, "6 1");
			Assert.AreEqual(((TranslateTransform)((DrawingGroup)drawing_group.Children[1]).Transform).Value, new Matrix(1, 0, 0, 1, 0, 1), "6 2");
			gd = (GeometryDrawing)((DrawingGroup)drawing_group.Children[1]).Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, Color.FromArgb(0xFF, 0x21, 0xA1, 0x21), "7");
			Assert.IsNull(gd.Pen, "8");
			StreamGeometry sg = (StreamGeometry)gd.Geometry;
			Assert.AreEqual(sg.ToString(), "M3;5L3;7,8 5,5;10,4 10,1;5,8 10,1;3 5,5;7,6z", "9");

			gd = (GeometryDrawing)drawing_group.Children[2];
			Assert.IsNull(gd.Pen, "10");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "12");
			Assert.AreEqual(gd.Geometry.ToString(), "M0;0L100;0L100;100L0;100z M1;2L97;2L97;96L1;96z", "13");
		}

		[Test]
		public void DrawingBackgroundBorderBrushNonUniformBorderThicknessIsChecked2() {
			BulletChrome c = new BulletChrome();
			c.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.BorderThickness = new Thickness(10, 20, 30, 40);
			c.IsChecked = true;
			c.Width = 100;
			c.Height = 100;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(drawing_group.Children.Count, 3, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)c.Background).Color, "2");
			Assert.IsNull(gd.Pen, "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(10, 20, 60, 40), "4");
			Assert.AreEqual(rg.RadiusX, 0, "5");
			Assert.AreEqual(rg.RadiusY, 0, "6");

			Assert.AreEqual(((DrawingGroup)drawing_group.Children[1]).Children.Count, 1, "6 1");
			Assert.AreEqual(((TranslateTransform)((DrawingGroup)drawing_group.Children[1]).Transform).Value, new Matrix(1, 0, 0, 1, 9, 19), "6 2");
			gd = (GeometryDrawing)((DrawingGroup)drawing_group.Children[1]).Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, Color.FromArgb(0xFF, 0x21, 0xA1, 0x21), "7");
			Assert.IsNull(gd.Pen, "8");
			StreamGeometry sg = (StreamGeometry)gd.Geometry;
			Assert.AreEqual(sg.ToString(), "M3;5L3;7,8 5,5;10,4 10,1;5,8 10,1;3 5,5;7,6z", "9");

			gd = (GeometryDrawing)drawing_group.Children[2];
			Assert.IsNull(gd.Pen, "10");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "12");
			Assert.AreEqual(gd.Geometry.ToString(), "M0;0L100;0L100;100L0;100z M10;20L70;20L70;60L10;60z", "13");
		}

		[Test]
		public void DrawingBackgroundBorderBrushNonUniformBorderThicknessIsCheckedRenderPressed() {
			BulletChrome c = new BulletChrome();
			c.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.BorderThickness = new Thickness(1, 2, 3, 4);
			c.IsChecked = true;
			c.RenderPressed = true;
			c.Width = 100;
			c.Height = 100;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(drawing_group.Children.Count, 3, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "2");
			Assert.AreEqual(lgb.EndPoint, new Point(1, 1), "2 1");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "2 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xB2, 0xB2, 0xA9), "2 3");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "2 4");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xEB, 0xEA, 0xDA), "2 5");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "2 6");
			Assert.IsNull(gd.Pen, "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1, 2, 96, 94), "4");
			Assert.AreEqual(rg.RadiusX, 0, "5");
			Assert.AreEqual(rg.RadiusY, 0, "6");

			Assert.AreEqual(((DrawingGroup)drawing_group.Children[1]).Children.Count, 1, "6 1");
			Assert.AreEqual(((TranslateTransform)((DrawingGroup)drawing_group.Children[1]).Transform).Value, new Matrix(1, 0, 0, 1, 0, 1), "6 2");
			gd = (GeometryDrawing)((DrawingGroup)drawing_group.Children[1]).Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, Color.FromArgb(0xFF, 0x1A, 0x7E, 0x18), "7");
			Assert.IsNull(gd.Pen, "8");
			StreamGeometry sg = (StreamGeometry)gd.Geometry;
			Assert.AreEqual(sg.ToString(), "M3;5L3;7,8 5,5;10,4 10,1;5,8 10,1;3 5,5;7,6z", "9");

			gd = (GeometryDrawing)drawing_group.Children[2];
			Assert.IsNull(gd.Pen, "10");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "12");
			Assert.AreEqual(gd.Geometry.ToString(), "M0;0L100;0L100;100L0;100z M1;2L97;2L97;96L1;96z", "13");
		}
		
		[Test]
		public void DrawingBackgroundBorderBrushNonUniformBorderThicknessIsCheckedRenderPressedRenderMouseOver() {
			BulletChrome c = new BulletChrome();
			c.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.BorderThickness = new Thickness(1, 2, 3, 4);
			c.IsChecked = true;
			c.RenderPressed = true;
			c.RenderMouseOver = true;
			c.Width = 100;
			c.Height = 100;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(drawing_group.Children.Count, 3, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "2");
			Assert.AreEqual(lgb.EndPoint, new Point(1, 1), "2 1");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "2 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xB2, 0xB2, 0xA9), "2 3");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "2 4");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xEB, 0xEA, 0xDA), "2 5");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "2 6");
			Assert.IsNull(gd.Pen, "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1, 2, 96, 94), "4");
			Assert.AreEqual(rg.RadiusX, 0, "5");
			Assert.AreEqual(rg.RadiusY, 0, "6");

			Assert.AreEqual(((DrawingGroup)drawing_group.Children[1]).Children.Count, 1, "6 1");
			Assert.AreEqual(((TranslateTransform)((DrawingGroup)drawing_group.Children[1]).Transform).Value, new Matrix(1, 0, 0, 1, 0, 1), "6 2");
			gd = (GeometryDrawing)((DrawingGroup)drawing_group.Children[1]).Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, Color.FromArgb(0xFF, 0x1A, 0x7E, 0x18), "7");
			Assert.IsNull(gd.Pen, "8");
			StreamGeometry sg = (StreamGeometry)gd.Geometry;
			Assert.AreEqual(sg.ToString(), "M3;5L3;7,8 5,5;10,4 10,1;5,8 10,1;3 5,5;7,6z", "9");

			gd = (GeometryDrawing)drawing_group.Children[2];
			Assert.IsNull(gd.Pen, "10");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "12");
			Assert.AreEqual(gd.Geometry.ToString(), "M0;0L100;0L100;100L0;100z M1;2L97;2L97;96L1;96z", "13");
		}

		[Test]
		public void DrawingBackgroundBorderBrushNonUniformBorderThicknessIsCheckedRenderMouseOver() {
			BulletChrome c = new BulletChrome();
			c.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.BorderThickness = new Thickness(1, 2, 3, 4);
			c.IsChecked = true;
			c.RenderMouseOver = true;
			c.Width = 100;
			c.Height = 100;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(drawing_group.Children.Count, 4, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)c.Background).Color, "2");
			Assert.IsNull(gd.Pen, "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1, 2, 96, 94), "4");
			Assert.AreEqual(rg.RadiusX, 0, "5");
			Assert.AreEqual(rg.RadiusY, 0, "6");

			gd = (GeometryDrawing)drawing_group.Children[1];
			Assert.IsNull(gd.Brush, "6 2 2");
			Assert.AreEqual(gd.Pen.Thickness, 2, "6 3");
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Pen.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "6 4");
			Assert.AreEqual(lgb.EndPoint, new Point(1, 1), "6 5");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "6 6");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xFF, 0xF0, 0xCF), "6 7");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "6 8");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xF8, 0xB3, 0x30), "6 9");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "6 10");
			rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(2, 3, 94, 92), "6 11");
			Assert.AreEqual(rg.RadiusX, 0, "6 12");
			Assert.AreEqual(rg.RadiusY, 0, "6 13");
			
			Assert.AreEqual(((DrawingGroup)drawing_group.Children[2]).Children.Count, 1, "6 1");
			Assert.AreEqual(((TranslateTransform)((DrawingGroup)drawing_group.Children[2]).Transform).Value, new Matrix(1, 0, 0, 1, 0, 1), "6 2");
			gd = (GeometryDrawing)((DrawingGroup)drawing_group.Children[2]).Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, Color.FromArgb(0xFF, 0x21, 0xA1, 0x21), "7");
			Assert.IsNull(gd.Pen, "8");
			StreamGeometry sg = (StreamGeometry)gd.Geometry;
			Assert.AreEqual(sg.ToString(), "M3;5L3;7,8 5,5;10,4 10,1;5,8 10,1;3 5,5;7,6z", "9");

			gd = (GeometryDrawing)drawing_group.Children[3];
			Assert.IsNull(gd.Pen, "10");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "12");
			Assert.AreEqual(gd.Geometry.ToString(), "M0;0L100;0L100;100L0;100z M1;2L97;2L97;96L1;96z", "13");
		}

		[Test]
		public void DrawingBorderBrushNonUniformBorderThicknessIsCheckedRenderPressed() {
			BulletChrome c = new BulletChrome();
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.BorderThickness = new Thickness(1, 2, 3, 4);
			c.IsChecked = true;
			c.RenderPressed = true;
			c.Width = 100;
			c.Height = 100;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(drawing_group.Children.Count, 3, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			LinearGradientBrush lgb = (LinearGradientBrush)gd.Brush;
			Assert.AreEqual(lgb.StartPoint, new Point(0, 0), "2");
			Assert.AreEqual(lgb.EndPoint, new Point(1, 1), "2 1");
			Assert.AreEqual(lgb.GradientStops.Count, 2, "2 2");
			Assert.AreEqual(lgb.GradientStops[0].Color, Color.FromArgb(0xFF, 0xB2, 0xB2, 0xA9), "2 3");
			Assert.AreEqual(lgb.GradientStops[0].Offset, 0, "2 4");
			Assert.AreEqual(lgb.GradientStops[1].Color, Color.FromArgb(0xFF, 0xEB, 0xEA, 0xDA), "2 5");
			Assert.AreEqual(lgb.GradientStops[1].Offset, 1, "2 6");
			Assert.IsNull(gd.Pen, "3");
			RectangleGeometry rg = (RectangleGeometry)gd.Geometry;
			Assert.AreEqual(rg.Rect, new Rect(1, 2, 96, 94), "4");
			Assert.AreEqual(rg.RadiusX, 0, "5");
			Assert.AreEqual(rg.RadiusY, 0, "6");

			Assert.AreEqual(((DrawingGroup)drawing_group.Children[1]).Children.Count, 1, "6 1");
			Assert.AreEqual(((TranslateTransform)((DrawingGroup)drawing_group.Children[1]).Transform).Value, new Matrix(1, 0, 0, 1, 0, 1), "6 2");
			gd = (GeometryDrawing)((DrawingGroup)drawing_group.Children[1]).Children[0];
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, Color.FromArgb(0xFF, 0x1A, 0x7E, 0x18), "7");
			Assert.IsNull(gd.Pen, "8");
			StreamGeometry sg = (StreamGeometry)gd.Geometry;
			Assert.AreEqual(sg.ToString(), "M3;5L3;7,8 5,5;10,4 10,1;5,8 10,1;3 5,5;7,6z", "9");

			gd = (GeometryDrawing)drawing_group.Children[2];
			Assert.IsNull(gd.Pen, "10");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "12");
			Assert.AreEqual(gd.Geometry.ToString(), "M0;0L100;0L100;100L0;100z M1;2L97;2L97;96L1;96z", "13");
		}

		[Test]
		public void DrawingBackgroundBorderBrushNonUniformLargeBorderThicknessIsCheckedRenderPressed() {
			BulletChrome c = new BulletChrome();
			c.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.BorderThickness = new Thickness(100, 200, 300, 400);
			c.IsChecked = true;
			c.RenderPressed = true;
			c.Width = 100;
			c.Height = 100;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(drawing_group.Children.Count, 1, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.IsNull(gd.Pen, "1");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "2");
			Assert.AreEqual(gd.Geometry.ToString(), "M0;0L100;0L100;100L0;100z M100;200L100;200L100;200L100;200z", "3");
		}

		[Test]
		public void DrawingBackgroundBorderBrushNonUniformVeryLargeBorderThicknessIsCheckedRenderPressed() {
			BulletChrome c = new BulletChrome();
			c.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.BorderThickness = new Thickness(1000, 2000, 3000, 4000);
			c.IsChecked = true;
			c.RenderPressed = true;
			c.Width = 100;
			c.Height = 100;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(c);
			Assert.AreEqual(drawing_group.Children.Count, 1, "1");

			GeometryDrawing gd = (GeometryDrawing)drawing_group.Children[0];
			Assert.IsNull(gd.Pen, "1");
			Assert.AreEqual(((SolidColorBrush)gd.Brush).Color, ((SolidColorBrush)c.BorderBrush).Color, "2");
			Assert.AreEqual(gd.Geometry.ToString(), "M0;0L100;0L100;100L0;100z M1000;2000L1000;2000L1000;2000L1000;2000z", "3");
		}

		[Test]
		public void DrawingBackgroundBorderBrushBorderThicknessIsCheckedSmallSize() {
			BulletChrome c = new BulletChrome();
			c.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.BorderThickness = new Thickness(1);
			c.IsChecked = true;
			c.Width = 1;
			c.Height = 1;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(c);
			Assert.IsNull(drawing_group);
		}

		[Test]
		public void DrawingBackgroundBorderBrushBorderThicknessIsCheckedSmallSize2() {
			BulletChrome c = new BulletChrome();
			c.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.BorderThickness = new Thickness(1);
			c.IsChecked = true;
			c.Width = 2;
			c.Height = 2;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(c);
			Assert.IsNull(drawing_group);
		}

		[Test]
		public void DrawingBackgroundBorderBrushBorderThicknessIsCheckedSmallSize3() {
			BulletChrome c = new BulletChrome();
			c.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.BorderThickness = new Thickness(1);
			c.IsChecked = true;
			c.Width = 2.1;
			c.Height = 2.1;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(c);
			Assert.IsNull(drawing_group);
		}

		[Test]
		public void DrawingBackgroundBorderBrushIsCheckedSmallSize() {
			BulletChrome c = new BulletChrome();
			c.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.IsChecked = true;
			c.Width = 2;
			c.Height = 2;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(c);
			Assert.IsNull(drawing_group);
		}

		[Test]
		public void DrawingBackgroundBorderBrushIsCheckedSmallSize2() {
			BulletChrome c = new BulletChrome();
			c.Background = new SolidColorBrush(Color.FromArgb(0x11, 0x11, 0x11, 0x11));
			c.BorderBrush = new SolidColorBrush(Color.FromArgb(0x22, 0x22, 0x22, 0x22));
			c.IsChecked = true;
			c.Width = 2.1;
			c.Height = 2.1;
			Window w = new Window();
			w.Content = c;
			w.Show();
			DrawingGroup drawing_group = VisualTreeHelper.GetDrawing(c);
			Assert.IsNotNull(drawing_group);
		}
	}
}