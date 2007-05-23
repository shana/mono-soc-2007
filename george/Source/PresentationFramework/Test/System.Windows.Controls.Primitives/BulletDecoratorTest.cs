using NUnit.Framework;
using System.Collections;
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
	public class BulletDecoratorTest {
		[Test]
		public void StaticProperties() {
			Assert.AreEqual(BulletDecorator.BackgroundProperty.Name, "Background", "BackgroundProperty.Name");
			Assert.AreEqual(BulletDecorator.BackgroundProperty.OwnerType, typeof(Panel), "BackgroundProperty.OwnerType");
			Assert.AreEqual(BulletDecorator.BackgroundProperty.PropertyType, typeof(Brush), "BackgroundProperty.PropertyType");
			Assert.IsNull(BulletDecorator.BackgroundProperty.DefaultMetadata.DefaultValue, "BackgroundProperty.DefaultMetadata.DefaultValue");
			Assert.AreSame(BulletDecorator.BackgroundProperty, Panel.BackgroundProperty, "Panel.BackgroundProperty");
		}

		#region Children
		[Test]
		public void Children() {
			new ChildrenBulletDecorator();
		}

		class ChildrenBulletDecorator : BulletDecorator {
			public ChildrenBulletDecorator() {
				Assert.AreSame(LogicalChildren, LogicalChildren, "Same");
				try {
					object dummy = LogicalChildren.Current;
					Assert.Fail("LogicalChildren");
				} catch (InvalidOperationException) {
				}
				Assert.AreEqual(VisualChildrenCount, 0, "VisualChildrenCount");
				object childred = LogicalChildren;
				Child = new UIElement();
				Assert.AreNotSame(childred, LogicalChildren, "Not the same");
				Assert.AreNotSame(LogicalChildren, LogicalChildren, "?");
				IEnumerator logical_children = LogicalChildren;
				logical_children.MoveNext();
				Assert.AreEqual(logical_children.Current, Child, "LogicalChildren 1");
				Assert.AreEqual(GetVisualChild(0), Child, "GetVisualChild(0)");
				Assert.AreEqual(VisualChildrenCount, 1, "VisualChildrenCount 1");
				Bullet = new UIElement();
				logical_children = LogicalChildren;
				logical_children.MoveNext();
				Assert.AreEqual(logical_children.Current, Bullet, "LogicalChildren 2");
				Assert.AreEqual(VisualChildrenCount, 2, "VisualChildrenCount 2");
				Assert.AreEqual(GetVisualChild(0), Bullet, "GetVisualChild(0) 1");
				Assert.AreEqual(GetVisualChild(1), Child, "GetVisualChild(1)");
				try {
					GetVisualChild(2);
					Assert.Fail("GetVisualChild(2)");
				} catch(ArgumentOutOfRangeException ex) {
					Assert.AreEqual(ex.Message, "Specified index is out of range or child at index is null. Do not call this method if VisualChildrenCount returns zero, indicating that the Visual has no children." + Environment.NewLine + "Parameter name: index" + Environment.NewLine + "Actual value was 2.", "ex.Message");
					Assert.AreEqual(ex.ParamName, "index", "ex.ParamName");
				}
			}
		}
		#endregion

		[Test]
		public void VerticalAlignmentTest() {
			BulletDecorator b = new BulletDecorator();
			Assert.AreEqual(b.VerticalAlignment, VerticalAlignment.Stretch);
		}
	}
}