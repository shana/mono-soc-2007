using NUnit.Framework;
using System.Windows.Markup;
using System.Windows.Media;
#if Implementation
using System;
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

		#region GetVisualChildOutOfRange
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetVisualChildOutOfRange() {
			new GetVisualChildOutOfRangePanel();
		}

		class GetVisualChildOutOfRangePanel : Panel {
			public GetVisualChildOutOfRangePanel() {
				GetVisualChild(0);
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

		#region ShouldSerializeChildren
		[Test]
		public void ShouldSerializeChildren() {
			new ShouldSerializeChildrenPanel();
		}

		class ShouldSerializeChildrenPanel : Panel {
			public ShouldSerializeChildrenPanel() {
				Assert.IsFalse(ShouldSerializeChildren(), "1");
				Children.Add(new Button());
				Assert.IsTrue(ShouldSerializeChildren(), "2");
			}
		}
		#endregion

		#region ZIndex
		[Test]
		public void ZIndex() {
			new ZIndexPanel();
		}

		class ZIndexPanel : Panel {
			public ZIndexPanel() {
				Button b1 = new Button();
				Assert.AreEqual(GetZIndex(b1), 0, "1");
				Children.Add(b1);
				Assert.AreEqual(GetZIndex(b1), 0, "2");
				Button b2 = new Button();
				Assert.AreEqual(GetZIndex(b2), 0, "3");
				SetZIndex(b2, -1);
				Assert.AreEqual(GetZIndex(b2), -1, "4");
				Children.Add(b2);
				Assert.AreEqual(GetZIndex(b2), -1, "5");
				Assert.AreEqual(Children[1], b2, "6");
				Assert.AreEqual(InternalChildren[1], b2, "6 1");
				Assert.AreEqual(GetVisualChild(1), b1, "6 3");
				Button b3 = new Button();
				Children.Add(b3);
				Assert.AreEqual(GetZIndex(b3), 0, "7");
			}
		}
		#endregion
		
		#region Focusable
		[Test]
		public void Focusable() {
			new FocusablePanel();
		}

		class FocusablePanel : Panel {
			public FocusablePanel() {
				Assert.IsFalse(Focusable);
			}
		}
		#endregion

		#region IAddChild
		[Test]
#if Implementation
		[Ignore("Not documented")]
#endif
		public void IAddChild() {
			new IAddChildPanel();
		}

		class IAddChildPanel : Panel {
			public IAddChildPanel() {
				((IAddChild)this).AddText(string.Empty);
				Assert.AreEqual(Children.Count, 0, "1");
				try {
					((IAddChild)this).AddText("Test");
					Assert.Fail("2");
				} catch (ArgumentException) {
				}
			}
		}
		#endregion

		[Test]
		public void AlignmentMetadata() {
			PropertyMetadata framework_element_metadata = FrameworkElement.HorizontalAlignmentProperty.GetMetadata(typeof(FrameworkElement));
			PropertyMetadata panel_metadata = FrameworkElement.HorizontalAlignmentProperty.GetMetadata(typeof(Panel));
			Assert.AreSame(framework_element_metadata, panel_metadata);
		}

		#region IsItemsHost
		[Test]
		[ExpectedException(ExceptionType = typeof(InvalidOperationException), ExpectedMessage="A panel with IsItemsHost=\"true\" is not nested in an ItemsControl. Panel must be nested in ItemsControl to get and show items.")]
		public void IsItemsHostChildren() {
			IsItemsHostPanel p = new IsItemsHostPanel();
			object dummy = p.Children;
		}
		
		[Test]
		[ExpectedException(ExceptionType = typeof(InvalidOperationException), ExpectedMessage = "A panel with IsItemsHost=\"true\" is not nested in an ItemsControl. Panel must be nested in ItemsControl to get and show items.")]
		public void IsItemsHostInternalChildren() {
			IsItemsHostPanel p = new IsItemsHostPanel();
			p.GetInternalChildren();
		}

		[Test]
		public void InItemsControl() {
			TestItemsControl items_control = new TestItemsControl();
			ControlTemplate items_control_template = new ControlTemplate(typeof(TestItemsControl));
			FrameworkElementFactory panel_factory = new FrameworkElementFactory(typeof(IsItemsHostPanel));
			panel_factory.Name = "Panel";
			items_control_template.VisualTree = panel_factory;
			items_control.Template = items_control_template;
			Button button = new Button();
			items_control.Items.Add(button);
			Assert.IsNull(items_control.GetPanel());
			items_control.ApplyTemplate();
			UIElementCollection children = items_control.GetPanel().Children;
			Assert.AreEqual(children.Count, 1, "2");
			Assert.AreSame(children, items_control.GetPanel().Children, "3");
			Assert.AreSame(children, items_control.GetPanel().GetInternalChildren(), "4");
			items_control.GetPanel().IsItemsHost = false;
			Assert.AreSame(children, items_control.GetPanel().Children, "5");
			Assert.AreSame(children, items_control.GetPanel().GetInternalChildren(), "6");
			Assert.AreEqual(items_control.GetPanel().Children.Count, 0, "7");
			items_control.GetPanel().IsItemsHost = true;
			Assert.AreSame(children, items_control.GetPanel().Children, "8");
			Assert.AreSame(children, items_control.GetPanel().GetInternalChildren(), "9");
			Assert.AreEqual(items_control.GetPanel().Children.Count, 1, "10");
			Assert.AreSame(items_control.GetPanel().Children[0], button, "11");
			items_control.GetPanel().CallOnIsItemsHostChanged = false;
			items_control.GetPanel().IsItemsHost = false;
			Assert.AreSame(children, items_control.GetPanel().Children, "12");
			Assert.AreSame(children, items_control.GetPanel().GetInternalChildren(), "13");
			Assert.AreEqual(items_control.GetPanel().Children.Count, 0, "14");
			Assert.AreEqual(children.GetType(), typeof(UIElementCollection), "15");
		}

		class IsItemsHostPanel : Panel {
			public bool CallOnIsItemsHostChanged = true;

			public IsItemsHostPanel() {
				IsItemsHost = true;
			}

			public object GetInternalChildren() {
				return InternalChildren;
			}

			protected override void OnIsItemsHostChanged(bool oldIsItemsHost, bool newIsItemsHost) {
				if (CallOnIsItemsHostChanged)
					base.OnIsItemsHostChanged(oldIsItemsHost, newIsItemsHost);
			}
		}

		class TestItemsControl : ItemsControl {
			public IsItemsHostPanel GetPanel() {
				return (IsItemsHostPanel)GetTemplateChild("Panel");
			}
		}
		#endregion
	}
}