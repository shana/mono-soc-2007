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
	public class PanelTest {
		#region ChildrenNotNull
		[Test]
		public void ChildrenNotNull() {
			new ChildrenNotNullPanel();	
		}

		class ChildrenNotNullPanel : Panel {
			public ChildrenNotNullPanel() {
				Assert.IsNotNull(Children, "Value");
				Assert.AreEqual(Children.GetType().FullName, "System.Windows.Controls.UIElementCollection", "Type");
			}
		}
		#endregion

		#region CreateUIElementCollection
		[Test]
		public void CreateUIElementCollection() {
			new CreateUIElementCollectionPanel();
		}

		class CreateUIElementCollectionPanel : Panel {
			public CreateUIElementCollectionPanel() {
				UIElementCollection value = CreateUIElementCollection(this);
				Assert.AreEqual(value.GetType().FullName, "System.Windows.Controls.UIElementCollection", "Type");
			}
		}
		#endregion

		#region VisualChildrenCount
		[Test]
		public void VisualChildrenCount() {
			new VisualChildrenCountPanel();
		}

		class VisualChildrenCountPanel : Panel {
			public VisualChildrenCountPanel() {
				Assert.AreEqual(VisualChildrenCount, 0, "Before");
				Button b = new Button();
				Children.Add(b);
				Assert.AreEqual(VisualChildrenCount, 1, "After");
				Assert.AreEqual(VisualTreeHelper.GetParent(b), this, "Visual parent");
				Assert.AreEqual(LogicalTreeHelper.GetParent(b), this, "Logical parent");
			}
		}
		#endregion

		#region GetVisualChild
		[Test]
		public void GetVisualChild() {
			new GetVisualChildPanel();
		}

		class GetVisualChildPanel : Panel {
			public GetVisualChildPanel() {
				Button b = new Button();
				Children.Add(b);
				Assert.AreEqual(GetVisualChild(0), b);
			}
		}
		#endregion


		#region HasLogicalOrientation
		[Test]
		public void HasLogicalOrientation() {
			new HasLogicalOrientationPanel();
		}

		class HasLogicalOrientationPanel : Panel {
			public HasLogicalOrientationPanel() {
				Assert.IsFalse(HasLogicalOrientation, "HasLogicalOrientation");
				Assert.AreEqual(LogicalOrientation, Orientation.Vertical, "LogicalOrientation");
			}
		}
		#endregion
	}
}