using NUnit.Framework;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class DockPanelTest {
		#region MeasureOverride
		[Test]
		public void MeasureOverride() {
			new MeasureOverrideDockPanel();
		}

		class MeasureOverrideDockPanel : DockPanel {
			public MeasureOverrideDockPanel() {
				Size result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 0, "1");
				result = MeasureOverride(new Size(100, 100));
				Assert.AreEqual(result.Width, 0, "2");
				Window w = new Window();
				w.Content = this;
				w.Show();
				result = MeasureOverride(new Size(100, 100));
				Assert.AreEqual(result.Width, 0, "3");
				Children.Add(new TestButton());
				should_record = true;
				result = MeasureOverride(new Size(100, 100));
				should_record = false;
				Assert.AreEqual(measure_constraint.Width, 100, "4 1");
				Assert.AreEqual(measure_constraint.Height, 100, "4 1 2");
				Assert.IsTrue(called, "4 2");
				Assert.AreEqual(measure_result.Width, 8, "4 3");
				Assert.AreEqual(result.Width, 8, "4");
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 8, "5");
			}
			static bool should_record;
			static bool called;
			static Size measure_constraint;
			static Size measure_result;
			class TestButton : Button {
				protected override Size MeasureOverride(Size constraint) {
					if (should_record) {
						called = true;
						return measure_result = base.MeasureOverride(measure_constraint = constraint);
					} else
						return base.MeasureOverride(constraint);
				}
			}
		}
		#endregion

		#region ButtonMeasureOverride
		[Test]
		public void ButtonMeasureOverride() {
			new ButtonMeasureOverrideButton();
		}

		class ButtonMeasureOverrideButton : Button {
			public ButtonMeasureOverrideButton() {
				Assert.AreEqual(MeasureOverride(new Size(100, 100)).Width, 0, "1");
				DockPanel p = new DockPanel();
				p.Children.Add(this);
				Assert.AreEqual(MeasureOverride(new Size(100, 100)).Width, 0, "2");
				p.Children.Remove(this);
				Window w = new Window();
				w.Show();
				w.Content = this;
				Assert.AreEqual(MeasureOverride(new Size(100, 100)).Width, 0, "2");
			}
		}
		#endregion
	}
}