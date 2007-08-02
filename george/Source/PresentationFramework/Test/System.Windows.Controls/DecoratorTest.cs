using NUnit.Framework;
using System.Collections;
using System.Windows.Markup;
#if Implementation
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
			Button b = new Button();
			add_child.AddChild(b);
			Assert.AreSame(d.Child, b, "1");
		}
	}
}