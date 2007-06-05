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
	}
}