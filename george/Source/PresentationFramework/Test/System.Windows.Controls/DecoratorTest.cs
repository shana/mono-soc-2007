using NUnit.Framework;
using System.Collections;
using System.Windows.Markup;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class DecoratorTest {
		#region Child
		[Test]
		public void Child() {
			new ChildDecorator();
		}

		class ChildDecorator : Decorator {
			public ChildDecorator() {
				Assert.IsNull(Child, "1");
				IEnumerator logical_children = LogicalChildren;
				Assert.IsFalse(logical_children.MoveNext(), "1 1");
				Assert.AreEqual(VisualChildrenCount, 0, "1 2");
				Button b = new Button();
				Child = b;
				Assert.AreSame(Child, b, "2");
				logical_children = LogicalChildren;
				Assert.IsTrue(logical_children.MoveNext(), "2 1");
				Assert.AreSame(logical_children.Current, b, "2 2");
				Assert.IsFalse(logical_children.MoveNext(), "2 3");
				Assert.AreEqual(VisualChildrenCount, 1, "2 4");
				Assert.AreSame(GetVisualChild(0), b, "2 5");
			}
		}
		#endregion

		#region Layout
		[Test]
		public void Layout() {
			new LayoutDecorator();
		}

		class LayoutDecorator : Decorator {
			static Size arrange_parameter;
			static Size measure_parameter;

			public LayoutDecorator() {
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(0, 0), "1");
				Assert.AreEqual(MeasureOverride(new Size(100, double.PositiveInfinity)), new Size(0, 0), "2");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, 100)), new Size(0, 0), "3");
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(0, 0), "4");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(100, 100), "5");
				Child = new TestFrameworkElement();
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(10, 10), "6");
				Assert.AreEqual(measure_parameter, new Size(10, 10), "6 1");
				Assert.AreEqual(MeasureOverride(new Size(100, double.PositiveInfinity)), new Size(10, 10), "7");
				Assert.AreEqual(measure_parameter, new Size(10, 10), "7 1");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, 100)), new Size(10, 10), "8");
				Assert.AreEqual(measure_parameter, new Size(10, 10), "8 1");
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(10, 10), "9");
				Assert.AreEqual(measure_parameter, new Size(10, 10), "9 1");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(100, 100), "10");
				Assert.AreEqual(arrange_parameter, new Size(10, 10), "10 1");
			}

			class TestFrameworkElement : FrameworkElement {
				public TestFrameworkElement() {
					Width = 10;
					Height = 10;
				}

				protected override Size ArrangeOverride(Size finalSize) {
					arrange_parameter = finalSize;
					return base.ArrangeOverride(finalSize);
				}

				protected override Size MeasureOverride(Size availableSize) {
					measure_parameter = availableSize;
					return base.MeasureOverride(availableSize);
				}
			}
		}
		#endregion

		#region Layout2
		[Test]
		public void Layout2() {
			new Layout2Decorator();
		}

		class Layout2Decorator : Decorator {
			static Size arrange_parameter;
			static Size measure_parameter;

			public Layout2Decorator() {
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(0, 0), "1");
				Assert.AreEqual(MeasureOverride(new Size(100, double.PositiveInfinity)), new Size(0, 0), "2");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, 100)), new Size(0, 0), "3");
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(0, 0), "4");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(100, 100), "5");
				Child = new TestFrameworkElement();
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity)), new Size(0, 0), "6");
				Assert.AreEqual(measure_parameter, new Size(double.PositiveInfinity, double.PositiveInfinity), "6 1");
				Assert.AreEqual(MeasureOverride(new Size(100, double.PositiveInfinity)), new Size(0, 0), "7");
				Assert.AreEqual(measure_parameter, new Size(100, double.PositiveInfinity), "7 1");
				Assert.AreEqual(MeasureOverride(new Size(double.PositiveInfinity, 100)), new Size(0, 0), "8");
				Assert.AreEqual(measure_parameter, new Size(double.PositiveInfinity, 100), "8 1");
				Assert.AreEqual(MeasureOverride(new Size(100, 100)), new Size(0, 0), "9");
				Assert.AreEqual(measure_parameter, new Size(100, 100), "9 1");
				Assert.AreEqual(ArrangeOverride(new Size(100, 100)), new Size(100, 100), "10");
				Assert.AreEqual(arrange_parameter, new Size(100, 100), "10 1");
			}

			class TestFrameworkElement : FrameworkElement {
				protected override Size ArrangeOverride(Size finalSize) {
					arrange_parameter = finalSize;
					return base.ArrangeOverride(finalSize);
				}

				protected override Size MeasureOverride(Size availableSize) {
					measure_parameter = availableSize;
					return base.MeasureOverride(availableSize);
				}
			}
		}
		#endregion

		[Test]
		public void IAddChildTest() {
			Decorator d = new Decorator();
			IAddChild add_child = (IAddChild)d;
			UIElement b = new UIElement();
			add_child.AddChild(b);
			Assert.AreSame(d.Child, b, "1");
			try {
				add_child.AddChild(new UIElement());
				Assert.Fail("2");
			} catch (ArgumentException ex) {
				Assert.AreEqual(ex.Message, "'System.Windows.Controls.Decorator' already has a child and cannot add 'System.Windows.UIElement'. 'System.Windows.Controls.Decorator' can accept only one child.", "3");
			}
			d.Child = null;
			try {
				add_child.AddChild(new DrawingVisual());
				Assert.Fail("4");
			} catch (ArgumentException ex) {
				Assert.AreEqual(ex.Message, "Parameter is unexpected type 'System.Windows.Media.DrawingVisual'. Expected type is 'System.Windows.UIElement'.\r\nParameter name: value", "5");
			}
			d.Child = new UIElement();
			try {
				add_child.AddChild(new DrawingVisual());
				Assert.Fail("5");
			} catch (ArgumentException ex) {
				Assert.AreEqual(ex.Message, "Parameter is unexpected type 'System.Windows.Media.DrawingVisual'. Expected type is 'System.Windows.UIElement'.\r\nParameter name: value", "6");
			}
			d.Child = null;
			try {
				add_child.AddChild(null);
				Assert.Fail("7");
			} catch (NullReferenceException) {
			}
			try {
				add_child.AddText("1");
				Assert.Fail("8");
			} catch (ArgumentException ex) {
				Assert.AreEqual(ex.Message, "'1' text cannot be added because text is not valid in this element.", "9");
			}
		}

		#region AddChildCallsChild
		[Test]
		public void AddChildCallsChild() {
			new AddChildCallsChildDecorator();
		}

		class AddChildCallsChildDecorator : Decorator {
			int get_calls;
			int set_calls;

			public AddChildCallsChildDecorator() {
				Assert.AreEqual(get_calls, 0, "1");
				Assert.AreEqual(set_calls, 0, "2");
				((IAddChild)this).AddChild(new UIElement());
				Assert.AreEqual(get_calls, 1, "3");
				Assert.AreEqual(set_calls, 1, "4");
			}

			public override UIElement Child {
				get {
					get_calls++;
					return base.Child;
				}
				set {
					set_calls++;
					base.Child = value;
				}
			}
		}
		#endregion

		[Test]
		public void ChildAddsVisualChild() {
			Decorator d = new Decorator();
			d.Child = new UIElement();
			Assert.AreEqual(VisualTreeHelper.GetChildrenCount(d), 1);
		}

		#region ChildAddsVisualChild2
		[Test]
		public void ChildAddsVisualChild2() {
			new ChildAddsVisualChild2Decorator();
		}

		class ChildAddsVisualChild2Decorator : Decorator {
			static int calls;

			public ChildAddsVisualChild2Decorator() {
				Window w = new Window();
				w.Content = this;
				w.Show();
				Child = new TestFrameworkElement();
				Assert.AreEqual(calls, 0);
			}

			class TestFrameworkElement : FrameworkElement {
				protected override void OnRender(DrawingContext drawingContext) {
					calls++;
					base.OnRender(drawingContext);
				}

				protected override Size MeasureOverride(Size availableSize) {
					calls++;
					return base.MeasureOverride(availableSize);
				}
			}
		}
		#endregion
	}
}