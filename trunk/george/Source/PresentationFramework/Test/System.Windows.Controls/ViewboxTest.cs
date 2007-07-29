using NUnit.Framework;
using System.Collections;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class ViewboxTest {
		#region GetVisualChild
		[Test]
		public void GetVisualChild() {
			new GetVisualChildViewbox();
		}

		class GetVisualChildViewbox : Viewbox {
			public GetVisualChildViewbox() {
				IEnumerator logical_children = LogicalChildren;
				Assert.IsFalse(logical_children.MoveNext(), "0");
				Assert.AreEqual(VisualChildrenCount, 1, "1");
				Assert.IsTrue(GetVisualChild(0) is ContainerVisual, "2");
				try {
					GetVisualChild(1);
					Assert.Fail("3");
				} catch (ArgumentOutOfRangeException) {
				}
				try {
					GetVisualChild(-1);
					Assert.Fail("4");
				} catch (ArgumentOutOfRangeException) {
				}
			}
		}
		#endregion

		#region Child
		[Test]
		public void Child() {
			ChildViewbox v = new ChildViewbox();
			ContainerVisual container_visual = v.GetVisualChild();
			Assert.IsNull(v.Child, "1");
			Assert.AreEqual(container_visual.Children.Count, 0, "1 1");
			UIElement u1 = new UIElement();
			v.Child = u1;
			Assert.AreSame(v.Child, u1, "2");
			UIElement u2 = new UIElement();
			v.Child = u2;
			Assert.AreSame(v.Child, u2, "3");
			Assert.AreEqual(container_visual.Children.Count, 1, "4");
			Assert.AreSame(container_visual.Children[0], u2, "5");
			IEnumerator logical_children = v.GetLogicalChildren();
			Assert.IsTrue(logical_children.MoveNext(), "6");
			Assert.AreSame(logical_children.Current, u2, "7");
			Assert.IsFalse(logical_children.MoveNext(), "8");
		}

		class ChildViewbox : Viewbox {
			public ContainerVisual GetVisualChild() {
				return (ContainerVisual)GetVisualChild(0);
			}
			public IEnumerator GetLogicalChildren() {
				return LogicalChildren;
			}
		}
		#endregion

		#region Layout
		[Test]
		public void Layout() {
			new LayoutViewbox();
		}

		class LayoutViewbox : Viewbox {
			public LayoutViewbox() {
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(0, 0), "1");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(0, 0), "2");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(100, 100), "2 1");
				Child = new UIElement();
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(0, 0), "3");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(0, 0), "3 1");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(0, 0), "4");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(0, 0), "4 1");
				Child = new Button();
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(100, 100), "5");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(Utility.GetEmptyButtonSize(), Utility.GetEmptyButtonSize()), "6");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(100, 100), "6 1");
				Window w = new Window();
				FrameworkElement e = new FrameworkElement();
				e.Width = 10;
				e.Height = 20;
				Width = 200;
				Height = 100;
				w.Content = this;
				Child = e;
				w.Show();
				Assert.AreEqual(e.ActualWidth, 10, "7");
				Assert.AreEqual(e.ActualHeight, 20, "8");
				ContainerVisual container_visual = (ContainerVisual)GetVisualChild(0);
				Assert.IsNotNull(container_visual.Transform, "9");
				Assert.IsTrue(container_visual.Transform is ScaleTransform, "10");
				ScaleTransform scale_transform = (ScaleTransform)container_visual.Transform;
				Assert.AreEqual(scale_transform.CenterX, 0, "11");
				Assert.AreEqual(scale_transform.CenterY, 0, "12");
				Assert.AreEqual(scale_transform.ScaleX, 5, "13");
				Assert.AreEqual(scale_transform.ScaleX, 5, "14");
			}
		}
		#endregion

		#region LayoutStretchNone
		[Test]
		public void LayoutStretchNone() {
			new LayoutStretchNoneViewbox();
		}

		class LayoutStretchNoneViewbox : Viewbox {
			public LayoutStretchNoneViewbox() {
				Stretch = Stretch.None;
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(0, 0), "1");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(0, 0), "2");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(100, 100), "2 1");
				Child = new UIElement();
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(0, 0), "3");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(0, 0), "3 1");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(0, 0), "4");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(0, 0), "4 1");
				Child = new Button();
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(Utility.GetEmptyButtonSize(), Utility.GetEmptyButtonSize()), "5");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(Utility.GetEmptyButtonSize(), Utility.GetEmptyButtonSize()), "6");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(Utility.GetEmptyButtonSize(), Utility.GetEmptyButtonSize()), "6 1");
				Window w = new Window();
				FrameworkElement e = new FrameworkElement();
				e.Width = 10;
				e.Height = 20;
				Width = 200;
				Height = 100;
				w.Content = this;
				Child = e;
				w.Show();
				Assert.AreEqual(e.ActualWidth, 10, "7");
				Assert.AreEqual(e.ActualHeight, 20, "8");
				ContainerVisual container_visual = (ContainerVisual)GetVisualChild(0);
				Assert.IsNotNull(container_visual.Transform, "9");
				Assert.IsTrue(container_visual.Transform is ScaleTransform, "10");
				ScaleTransform scale_transform = (ScaleTransform)container_visual.Transform;
				Assert.AreEqual(scale_transform.CenterX, 0, "11");
				Assert.AreEqual(scale_transform.CenterY, 0, "12");
				Assert.AreEqual(scale_transform.ScaleX, 1, "13");
				Assert.AreEqual(scale_transform.ScaleX, 1, "14");
			}
		}
		#endregion

		#region LayoutStretchNone2
		[Test]
		public void LayoutStretchNone2() {
			new LayoutStretchNone2Viewbox();
		}

		class LayoutStretchNone2Viewbox : Viewbox {
			public LayoutStretchNone2Viewbox() {
				Stretch = Stretch.None;
				Child = new Button();
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(0, 0));
			}
		}
		#endregion

		#region LayoutStretchNone3
		[Test]
		public void LayoutStretchNone3() {
			new LayoutStretchNone3Viewbox();
		}

		class LayoutStretchNone3Viewbox : Viewbox {
			public LayoutStretchNone3Viewbox() {
				Stretch = Stretch.None;
				Child = new Button();
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(Utility.GetEmptyButtonSize(), Utility.GetEmptyButtonSize()), "1");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(Utility.GetEmptyButtonSize(), Utility.GetEmptyButtonSize()), "2");
			}
		}
		#endregion

		#region LayoutStretchFill
		[Test]
		public void LayoutStretchFill() {
			new LayoutStretchFillViewbox();
		}

		class LayoutStretchFillViewbox : Viewbox {
			public LayoutStretchFillViewbox() {
				Stretch = Stretch.Fill;
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(0, 0), "1");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(0, 0), "2");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(100, 100), "2 1");
				Child = new UIElement();
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(0, 0), "3");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(0, 0), "3 1");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(0, 0), "4");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(0, 0), "4 1");
				Child = new Button();
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(100, 100), "5");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(Utility.GetEmptyButtonSize(), Utility.GetEmptyButtonSize()), "6");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(100, 100), "6 1");
				Window w = new Window();
				FrameworkElement e = new FrameworkElement();
				e.Width = 10;
				e.Height = 20;
				Width = 200;
				Height = 100;
				w.Content = this;
				Child = e;
				w.Show();
				Assert.AreEqual(e.ActualWidth, 10, "7");
				Assert.AreEqual(e.ActualHeight, 20, "8");
				ContainerVisual container_visual = (ContainerVisual)GetVisualChild(0);
				Assert.IsNotNull(container_visual.Transform, "9");
				Assert.IsTrue(container_visual.Transform is ScaleTransform, "10");
				ScaleTransform scale_transform = (ScaleTransform)container_visual.Transform;
				Assert.AreEqual(scale_transform.CenterX, 0, "11");
				Assert.AreEqual(scale_transform.CenterY, 0, "12");
				Assert.AreEqual(scale_transform.ScaleX, 20, "13");
				Assert.AreEqual(scale_transform.ScaleX, 20, "14");
			}
		}
		#endregion

		#region LayoutStretchFill2
		[Test]
		public void LayoutStretchFill2() {
			new LayoutStretchFill2Viewbox();
		}

		class LayoutStretchFill2Viewbox : Viewbox {
			public LayoutStretchFill2Viewbox() {
				Stretch = Stretch.Fill;
				Child = new Button();
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(0, 0));
			}
		}
		#endregion

		#region LayoutStretchFill3
		[Test]
		public void LayoutStretchFill3() {
			new LayoutStretchFill3Viewbox();
		}

		class LayoutStretchFill3Viewbox : Viewbox {
			public LayoutStretchFill3Viewbox() {
				Stretch = Stretch.Fill;
				Child = new Button();
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(100, 100), "1");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(100, 100), "2");
			}
		}
		#endregion

		#region LayoutStretchUniformToFill
		[Test]
		public void LayoutStretchUniformToFill() {
			new LayoutStretchUniformToFillViewbox();
		}

		class LayoutStretchUniformToFillViewbox : Viewbox {
			public LayoutStretchUniformToFillViewbox() {
				Stretch = Stretch.UniformToFill;
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(0, 0), "1");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(0, 0), "2");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(100, 100), "2 1");
				Child = new UIElement();
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(0, 0), "3");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(0, 0), "3 1");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(0, 0), "4");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(0, 0), "4 1");
				Child = new Button();
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(100, 100), "5");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(Utility.GetEmptyButtonSize(), Utility.GetEmptyButtonSize()), "6");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(100, 100), "6 1");
				Window w = new Window();
				FrameworkElement e = new FrameworkElement();
				e.Width = 10;
				e.Height = 20;
				Width = 200;
				Height = 100;
				w.Content = this;
				Child = e;
				w.Show();
				Assert.AreEqual(e.ActualWidth, 10, "7");
				Assert.AreEqual(e.ActualHeight, 20, "8");
				ContainerVisual container_visual = (ContainerVisual)GetVisualChild(0);
				Assert.IsNotNull(container_visual.Transform, "9");
				Assert.IsTrue(container_visual.Transform is ScaleTransform, "10");
				ScaleTransform scale_transform = (ScaleTransform)container_visual.Transform;
				Assert.AreEqual(scale_transform.CenterX, 0, "11");
				Assert.AreEqual(scale_transform.CenterY, 0, "12");
				Assert.AreEqual(scale_transform.ScaleX, 20, "13");
				Assert.AreEqual(scale_transform.ScaleX, 20, "14");
			}
		}
		#endregion

		#region LayoutStretchUniformToFill2
		[Test]
		public void LayoutStretchUniformToFill2() {
			new LayoutStretchUniformToFill2Viewbox();
		}

		class LayoutStretchUniformToFill2Viewbox : Viewbox {
			public LayoutStretchUniformToFill2Viewbox() {
				Stretch = Stretch.UniformToFill;
				Child = new Button();
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(0, 0));
			}
		}
		#endregion

		#region LayoutStretchUniformToFill3
		[Test]
		public void LayoutStretchUniformToFill3() {
			new LayoutStretchUniformToFill3Viewbox();
		}

		class LayoutStretchUniformToFill3Viewbox : Viewbox {
			public LayoutStretchUniformToFill3Viewbox() {
				Stretch = Stretch.UniformToFill;
				Child = new Button();
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(100, 100), "1");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(100, 100), "2");
			}
		}
		#endregion

		#region ChildMeasure
		[Test]
		public void ChildMeasure() {
			new ChildMeasureViewbox();
		}

		class ChildMeasureViewbox : Viewbox {
			static Size measure_size;

			public ChildMeasureViewbox() {
				Child = new TestFrameworkElement();
				Window w = new Window();
				w.Content = this;
				Width = 100;
				Height = 200;
				w.Show();
				Assert.AreEqual(measure_size, new Size(double.PositiveInfinity, double.PositiveInfinity));
			}

			class TestFrameworkElement : FrameworkElement {
				protected override Size MeasureOverride(Size availableSize) {
					return base.MeasureOverride(measure_size = availableSize);
				}
			}
		}
		#endregion

		#region ChildMeasureStretchNone
		[Test]
		public void ChildMeasureStretchNone() {
			new ChildMeasureStretchNoneViewbox();
		}

		class ChildMeasureStretchNoneViewbox : Viewbox {
			static Size measure_size;

			public ChildMeasureStretchNoneViewbox() {
				Stretch = Stretch.None;
				Child = new TestFrameworkElement();
				Window w = new Window();
				w.Content = this;
				Width = 100;
				Height = 200;
				w.Show();
				Assert.AreEqual(measure_size, new Size(double.PositiveInfinity, double.PositiveInfinity));
			}

			class TestFrameworkElement : FrameworkElement {
				protected override Size MeasureOverride(Size availableSize) {
					return base.MeasureOverride(measure_size = availableSize);
				}
			}
		}
		#endregion

		#region ChildMeasureStretchFill
		[Test]
		public void ChildMeasureStretchFill() {
			new ChildMeasureStretchFillViewbox();
		}

		class ChildMeasureStretchFillViewbox : Viewbox {
			static Size measure_size;

			public ChildMeasureStretchFillViewbox() {
				Stretch = Stretch.Fill;
				Child = new TestFrameworkElement();
				Window w = new Window();
				w.Content = this;
				Width = 100;
				Height = 200;
				w.Show();
				Assert.AreEqual(measure_size, new Size(double.PositiveInfinity, double.PositiveInfinity));
			}

			class TestFrameworkElement : FrameworkElement {
				protected override Size MeasureOverride(Size availableSize) {
					return base.MeasureOverride(measure_size = availableSize);
				}
			}
		}
		#endregion

		#region ChildMeasureStretchUniformToFill
		[Test]
		public void ChildMeasureStretchUniformToFill() {
			new ChildMeasureStretchUniformToFillViewbox();
		}

		class ChildMeasureStretchUniformToFillViewbox : Viewbox {
			static Size measure_size;

			public ChildMeasureStretchUniformToFillViewbox() {
				Stretch = Stretch.UniformToFill;
				Child = new TestFrameworkElement();
				Window w = new Window();
				w.Content = this;
				Width = 100;
				Height = 200;
				w.Show();
				Assert.AreEqual(measure_size, new Size(double.PositiveInfinity, double.PositiveInfinity));
			}

			class TestFrameworkElement : FrameworkElement {
				protected override Size MeasureOverride(Size availableSize) {
					return base.MeasureOverride(measure_size = availableSize);
				}
			}
		}
		#endregion

		#region Image
		[Test]
		public void Image() {
			new ImageViewbox();
		}

		class ImageViewbox : Viewbox {
			Size arrange_parameter;
			Size arrange_result;
			Size measure_parameter;
			Size measure_result;

			public ImageViewbox() {
				global::System.Windows.Controls.Image image = new global::System.Windows.Controls.Image();
				image.Source = new BitmapImage(new Uri("Test.png", UriKind.Relative));
				Child = image;
				MaxWidth = 100;
				MaxHeight = 200;
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.AreEqual(arrange_parameter, new Size(100, 200), "1");
				Assert.AreEqual(arrange_result.Width, 100, "2");
				Assert.AreEqual(arrange_result.Height, 61.764705882352942, "2 1");
				Assert.AreEqual(measure_parameter, new Size(100, 200), "3");
				Assert.AreEqual(measure_result, new Size(100, 61.764705882352942), "4");
			}

			protected override Size ArrangeOverride(Size arrangeSize) {
				return arrange_result = base.ArrangeOverride(arrange_parameter = arrangeSize);
			}

			protected override Size MeasureOverride(Size constraint) {
				return measure_result = base.MeasureOverride(measure_parameter = constraint);
			}
		}
		#endregion

		#region ImageStretchNone
		[Test]
		public void ImageStretchNone() {
			new ImageStretchNoneViewbox();
		}

		class ImageStretchNoneViewbox : Viewbox {
			Size arrange_parameter;
			Size arrange_result;
			Size measure_parameter;
			Size measure_result;

			public ImageStretchNoneViewbox() {
				global::System.Windows.Controls.Image image = new global::System.Windows.Controls.Image();
				image.Source = new BitmapImage(new Uri("Test.png", UriKind.Relative));
				Stretch = Stretch.None;
				Child = image;
				MaxWidth = 100;
				MaxHeight = 200;
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.AreEqual(arrange_parameter, new Size(100, 200), "1");
				Assert.AreEqual(arrange_result, new Size(34, 21), "2");
				Assert.AreEqual(measure_parameter, new Size(100, 200), "3");
				Assert.AreEqual(measure_result, new Size(34, 21), "4");
			}

			protected override Size ArrangeOverride(Size arrangeSize) {
				return arrange_result = base.ArrangeOverride(arrange_parameter = arrangeSize);
			}

			protected override Size MeasureOverride(Size constraint) {
				return measure_result = base.MeasureOverride(measure_parameter = constraint);
			}
		}
		#endregion

		#region ImageStretchFill
		[Test]
		public void ImageStretchFill() {
			new ImageStretchFillViewbox();
		}

		class ImageStretchFillViewbox : Viewbox {
			Size arrange_parameter;
			Size arrange_result;
			Size measure_parameter;
			Size measure_result;

			public ImageStretchFillViewbox() {
				global::System.Windows.Controls.Image image = new global::System.Windows.Controls.Image();
				image.Source = new BitmapImage(new Uri("Test.png", UriKind.Relative));
				Stretch = Stretch.Fill;
				Child = image;
				MaxWidth = 100;
				MaxHeight = 200;
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.AreEqual(arrange_parameter, new Size(100, 200), "1");
				Assert.AreEqual(arrange_result, new Size(100, 200), "2");
				Assert.AreEqual(measure_parameter, new Size(100, 200), "3");
				Assert.AreEqual(measure_result, new Size(100, 200), "4");
			}

			protected override Size ArrangeOverride(Size arrangeSize) {
				return arrange_result = base.ArrangeOverride(arrange_parameter = arrangeSize);
			}

			protected override Size MeasureOverride(Size constraint) {
				return measure_result = base.MeasureOverride(measure_parameter = constraint);
			}
		}
		#endregion

		#region ImageStretchUniformToFill
		[Test]
		public void ImageStretchUniformToFill() {
			new ImageStretchUniformToFillViewbox();
		}

		class ImageStretchUniformToFillViewbox : Viewbox {
			Size arrange_parameter;
			Size arrange_result;
			Size measure_parameter;
			Size measure_result;

			public ImageStretchUniformToFillViewbox() {
				global::System.Windows.Controls.Image image = new global::System.Windows.Controls.Image();
				image.Source = new BitmapImage(new Uri("Test.png", UriKind.Relative));
				Stretch = Stretch.UniformToFill;
				Child = image;
				MaxWidth = 100;
				MaxHeight = 200;
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.AreEqual(arrange_parameter.Width, 323.8095238095238, "1");
				Assert.AreEqual(arrange_parameter.Height, 200, "1 1");
				Assert.AreEqual(arrange_result, new Size(323.8095238095238, 200), "2");
				Assert.AreEqual(measure_parameter, new Size(100, 200), "3");
				Assert.AreEqual(measure_result, new Size(323.8095238095238, 200), "4");
			}

			protected override Size ArrangeOverride(Size arrangeSize) {
				return arrange_result = base.ArrangeOverride(arrange_parameter = arrangeSize);
			}

			protected override Size MeasureOverride(Size constraint) {
				return measure_result = base.MeasureOverride(measure_parameter = constraint);
			}
		}
		#endregion
	}
}