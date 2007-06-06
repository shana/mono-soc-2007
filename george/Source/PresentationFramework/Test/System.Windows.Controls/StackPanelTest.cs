using NUnit.Framework;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class StackPanelTest {
		[Test]
		public void DefaultValueIsStretchForAlignment() {
			StackPanel stack_panel = new StackPanel();
			Assert.AreEqual(stack_panel.HorizontalAlignment, HorizontalAlignment.Stretch, "1");
			Assert.AreEqual(stack_panel.VerticalAlignment, VerticalAlignment.Stretch, "2");
			Button button = new Button();
			button.HorizontalAlignment = HorizontalAlignment.Left;
			stack_panel.Children.Add(button);
			Assert.AreEqual(button.HorizontalAlignment, HorizontalAlignment.Left, "3");
		}

		[Test]
		public void Focusable() {
			Assert.IsFalse(new StackPanel().Focusable);
		}

		#region HasLogicalOrientation
		[Test]
		public void HasLogicalOrientation() {
			new HasLogicalOrientationStackPanel();
		}

		class HasLogicalOrientationStackPanel : StackPanel {
			public HasLogicalOrientationStackPanel() {
				Assert.IsTrue(HasLogicalOrientation);
			}
		}
		#endregion
		
		#region LogicalOrientation
		[Test]
		public void LogicalOrientation() {
			new LogicalOrientationStackPanel();
		}

		class LogicalOrientationStackPanel : StackPanel {
			public LogicalOrientationStackPanel() {
				Assert.AreEqual(Orientation, Orientation.Vertical, "1");
				Assert.AreEqual(LogicalOrientation, Orientation.Vertical, "2");
				Orientation = Orientation.Horizontal;
				Assert.AreEqual(LogicalOrientation, Orientation.Horizontal, "3");
			}
		}
		#endregion

		#region Measure
		[Test]
		public void Measure() {
			new MeasureStackPanel();
		}

		class MeasureStackPanel : StackPanel {
			static bool called;
			static Size measure_constraint;

			public MeasureStackPanel() {
				Size result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 0, "1");
				Assert.AreEqual(result.Height, 0, "2");
				TestButton button = new TestButton();
				Children.Add(button);
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, Utility.GetEmptyButtonSize(), "3");
				Assert.AreEqual(result.Height, Utility.GetEmptyButtonSize(), "4");
				Assert.IsTrue(called, "5");
				called = false;
				Assert.IsTrue(double.IsPositiveInfinity(measure_constraint.Width), "6");
				Assert.IsTrue(double.IsPositiveInfinity(measure_constraint.Height), "7");
			}
			
			class TestButton : global::System.Windows.Controls.Button {
				protected override Size MeasureOverride(Size constraint) {
					called = true;
					return base.MeasureOverride(measure_constraint = constraint);
				}
			}
		}
		#endregion

		#region Alignment
		[Test]
		public void Alignment() {
			new AlignmentStackPanel();
		}

		class AlignmentStackPanel : StackPanel {
			public AlignmentStackPanel() {
				Window window = new Window();
				global::System.Windows.Controls.DockPanel dock_panel = new global::System.Windows.Controls.DockPanel();
				window.Content = dock_panel;
				window.Show();
				dock_panel.Children.Add(this);
				HorizontalAlignment = HorizontalAlignment.Center;
				Size result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 0, "1");
				Assert.AreEqual(result.Height, 0, "2");
				Children.Add(new global::System.Windows.Controls.Button());
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, Utility.GetEmptyButtonSize(), "3");
				Assert.AreEqual(result.Height, Utility.GetEmptyButtonSize(), "4");
				Assert.AreEqual(ActualWidth, Utility.GetEmptyButtonSize(), "5");
				Assert.AreEqual(ActualHeight, dock_panel.ActualHeight, "6");
			}
		}
		#endregion

		[Test]
		public void AlignmentMetadata() {
			PropertyMetadata panel_metadata = FrameworkElement.HorizontalAlignmentProperty.GetMetadata(typeof(Panel));
			PropertyMetadata stack_panel_metadata = FrameworkElement.HorizontalAlignmentProperty.GetMetadata(typeof(StackPanel));
			Assert.AreSame(panel_metadata, stack_panel_metadata);
		}
	}
}