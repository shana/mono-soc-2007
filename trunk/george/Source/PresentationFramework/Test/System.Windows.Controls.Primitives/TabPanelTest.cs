using NUnit.Framework;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	[TestFixture]
	public class TabPanelTest {
		#region MeasureAuto
		[Test]
		public void MeasureAuto() {
			new MeasureAutoTabPanel();
		}

		class MeasureAutoTabPanel : TabPanel {
			static bool called;
			static Size measure_constraint;
			static Size measure_result;

			public MeasureAutoTabPanel() {
				Children.Add(new TestTabItem());
				Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.IsTrue(called, "1");
				Assert.IsTrue(double.IsPositiveInfinity(measure_constraint.Width), "2");
				Assert.IsTrue(double.IsPositiveInfinity(measure_constraint.Height), "3");
				Assert.AreEqual(measure_result.Width, 27, "4");
				Assert.AreEqual(measure_result.Height, 5, "5");
				Assert.AreEqual(DesiredSize.Width, 27, "6");
				Assert.AreEqual(DesiredSize.Height, 5, "7");
			}

			class TestTabItem : global::System.Windows.Controls.TabItem {
				protected override Size MeasureOverride(Size constraint) {
					called = true;
					return measure_result = base.MeasureOverride(measure_constraint = constraint);
				}
			}
		}
		#endregion

		#region MeasureAutoEmpty
		[Test]
		public void MeasureAutoEmpty() {
			new MeasureAutoEmptyTabPanel();
		}

		class MeasureAutoEmptyTabPanel : TabPanel {
			public MeasureAutoEmptyTabPanel() {
				Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(DesiredSize.Width, 0, "1");
				Assert.AreEqual(DesiredSize.Height, 0, "2");
			}
		}
		#endregion

		#region Measure
		[Test]
		public void Measure() {
			new MeasureTabPanel();
		}

		class MeasureTabPanel : TabPanel {
			static bool called;
			static Size measure_constraint;
			static Size measure_result;

			public MeasureTabPanel() {
				Children.Add(new TestTabItem());
				Measure(new Size(100, 100));
				Assert.IsTrue(called, "1");
				Assert.AreEqual(measure_constraint.Width, 100, "2");
				Assert.AreEqual(measure_constraint.Height, 100, "3");
				Assert.AreEqual(measure_result.Width, 27, "4");
				Assert.AreEqual(measure_result.Height, 5, "5");
				Assert.AreEqual(DesiredSize.Width, 27, "6");
				Assert.AreEqual(DesiredSize.Height, 5, "7");
			}

			class TestTabItem : global::System.Windows.Controls.TabItem {
				protected override Size MeasureOverride(Size constraint) {
					called = true;
					return measure_result = base.MeasureOverride(measure_constraint = constraint);
				}
			}
		}
		#endregion

		#region MeasureEmpty
		[Test]
		public void MeasureEmpty() {
			new MeasureEmptyTabPanel();
		}

		class MeasureEmptyTabPanel : TabPanel {
			public MeasureEmptyTabPanel() {
				Measure(new Size(100, 100));
				Assert.AreEqual(DesiredSize.Width, 0, "1");
				Assert.AreEqual(DesiredSize.Height, 0, "2");
			}
		}
		#endregion

		#region GetLayoutClip
		[Test]
		public void GetLayoutClip() {
			new GetLayoutClipTabPanel();
		}

		class GetLayoutClipTabPanel : TabPanel {
			public GetLayoutClipTabPanel() {
				Assert.IsNull(GetLayoutClip(new Size(double.PositiveInfinity, double.PositiveInfinity)), "1");
				Assert.IsNull(GetLayoutClip(new Size(100, 100)), "2");
				Children.Add(new TabItem());
				Assert.IsNull(GetLayoutClip(new Size(double.PositiveInfinity, double.PositiveInfinity)), "3");
				Assert.IsNull(GetLayoutClip(new Size(100, 100)), "4");
			}
		}
		#endregion

		[Test]
		public void LimitedWidth() {
			TabPanel t = new TabPanel();
			for (int i = 0; i < 5; i++) {
				TabItem tab_item = new TabItem();
				tab_item.Width = 100;
				t.Children.Add(tab_item);
			}
			t.Measure(new Size(100, double.PositiveInfinity));
			Assert.AreEqual(t.DesiredSize.Width, 100, "1");
			Assert.AreEqual(t.DesiredSize.Height, 25, "2");
		}
	}
}