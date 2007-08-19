using NUnit.Framework;
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls.Primitives
{
#else
namespace System.Windows.Controls.Primitives {
#endif
	[TestFixture]
	public class UniformGridTest
	{
		[Test]
		public void NegativeValues ()
		{
			UniformGrid g = new UniformGrid ();
			try {
				g.Columns = -1;
				Assert.Fail ("1");
			} catch (ArgumentException ex) {
				Assert.AreEqual (ex.Message, "'-1' is not a valid value for property 'Columns'.", "2");
			}
			try {
				g.FirstColumn = -1;
				Assert.Fail ("3");
			} catch (ArgumentException ex) {
				Assert.AreEqual (ex.Message, "'-1' is not a valid value for property 'FirstColumn'.", "4");
			}
			try {
				g.Rows = -1;
				Assert.Fail ("5");
			} catch (ArgumentException ex) {
				Assert.AreEqual (ex.Message, "'-1' is not a valid value for property 'Rows'.", "6");
			}
		}

		#region Layout
		[Test]
		public void Layout ()
		{
			new LayoutUniformGrid ();
		}

		class LayoutUniformGrid : UniformGrid
		{
			public LayoutUniformGrid ()
			{
				Assert.AreEqual (MeasureOverride (new Size (100, 100)), new Size (0, 0), "1");
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, 100)), new Size (0, 0), "2");
				Assert.AreEqual (MeasureOverride (new Size (100, double.PositiveInfinity)), new Size (0, 0), "3");
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, double.PositiveInfinity)), new Size (0, 0), "4");
				Assert.AreEqual (ArrangeOverride (new Size (100, 100)), new Size (100, 100), "5");
				TestFrameworkElement e = new TestFrameworkElement ();
				Children.Add (e);
				Assert.AreEqual (MeasureOverride (new Size (100, 100)), new Size (0, 0), "6");
				Assert.AreEqual (e.measure_parameter, new Size (100, 100), "7");
				e.measure_parameter = Size.Empty;
				TestFrameworkElement e2 = new TestFrameworkElement ();
				Children.Add (e2);
				Assert.AreEqual (MeasureOverride (new Size (100, 100)), new Size (0, 0), "8");
				Assert.AreEqual (e.measure_parameter, new Size (50, 50), "9");
				e.measure_parameter = Size.Empty;
				Assert.AreEqual (e2.measure_parameter, new Size (50, 50), "10");
				e2.measure_parameter = Size.Empty;
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, 100)), new Size (0, 0), "11");
				Assert.AreEqual (e.measure_parameter, new Size (double.PositiveInfinity, 50), "12");
				e.measure_parameter = Size.Empty;
				Assert.AreEqual (e2.measure_parameter, new Size (double.PositiveInfinity, 50), "13");
				e2.measure_parameter = Size.Empty;
				Assert.AreEqual (MeasureOverride (new Size (100, double.PositiveInfinity)), new Size (0, 0), "14");
				Assert.AreEqual (e.measure_parameter, new Size (50, double.PositiveInfinity), "15");
				e.measure_parameter = Size.Empty;
				Assert.AreEqual (e2.measure_parameter, new Size (50, double.PositiveInfinity), "16");
				e2.measure_parameter = Size.Empty;
				Assert.AreEqual (MeasureOverride (new Size (double.PositiveInfinity, double.PositiveInfinity)), new Size (0, 0), "17");
				Assert.AreEqual (e.measure_parameter, new Size (double.PositiveInfinity, double.PositiveInfinity), "18");
				e.measure_parameter = Size.Empty;
				Assert.AreEqual (e2.measure_parameter, new Size (double.PositiveInfinity, double.PositiveInfinity), "19");
				e2.measure_parameter = Size.Empty;
				Assert.AreEqual (ArrangeOverride (new Size (100, 100)), new Size (100, 100), "20");
				Assert.AreEqual (e.arrange_parameter, new Size (50, 50), "21");
				e.arrange_parameter = Size.Empty;
				Assert.AreEqual (e2.arrange_parameter, new Size (50, 50), "22");
				e2.arrange_parameter = Size.Empty;
			}

			class TestFrameworkElement : FrameworkElement
			{
				public Size arrange_parameter;
				public Size measure_parameter;

				protected override Size ArrangeOverride (Size finalSize)
				{
					arrange_parameter = finalSize;
					return base.ArrangeOverride (finalSize);
				}

				protected override Size MeasureOverride (Size availableSize)
				{
					measure_parameter = availableSize;
					return base.MeasureOverride (availableSize);
				}
			}
		}
		#endregion
	}
}