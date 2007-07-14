using NUnit.Framework;
using System.Collections;
using System.Windows.Markup;
using System.Windows.Media;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
#if Implementation
	[Ignore("Not implementable now")]
#endif
	public class ItemsControlTest {
		#region GroupStyle
		[Test]
		public void GroupStyle() {
			Assert.IsNotNull(new ItemsControl().GroupStyle);
		}
		#endregion

		#region ItemContainerGenerator
		[Test]
		public void ItemContainerGenerator() {
			Assert.IsNotNull(new ItemsControl().ItemContainerGenerator);
		}
		#endregion

		#region Items
		[Test]
		public void Items() {
			Assert.IsNotNull(new ItemsControl().Items, "1");
			Assert.AreEqual(new ItemsControl().Items.GetType(), typeof(ItemCollection), "2");
		}
		#endregion

		#region ItemsPanel
		[Test]
		public void ItemsPanelDefaultValue() {
			object default_value = ItemsControl.ItemsPanelProperty.GetMetadata(typeof(ItemsControl)).DefaultValue;
			Assert.IsNotNull(default_value, "1");
			Assert.AreEqual(default_value.GetType(), typeof(ItemsPanelTemplate), "2");
			ItemsPanelTemplate items_panel_template_default_value = (ItemsPanelTemplate)default_value;
			Assert.AreEqual(items_panel_template_default_value.VisualTree.Type, typeof(StackPanel), "3");

			ItemsPanelTemplate i = new ItemsControl().ItemsPanel;
			Assert.AreEqual(i, items_panel_template_default_value, "4");
		}
		#endregion

		#region LogicalChildren
		[Test]
		public void LogicalChildren() {
			new LogicalChildrenItemsControl();
		}

		class LogicalChildrenItemsControl : ItemsControl {
			public LogicalChildrenItemsControl() {
				Assert.IsNotNull(LogicalChildren, "1");
				Items.Add(1);
				IEnumerator logical_children = LogicalChildren;
				Assert.IsNotNull(logical_children, "2");
				Assert.IsTrue(logical_children.MoveNext(), "3");
				Assert.AreEqual(logical_children.Current, 1, "4");
				Assert.IsFalse(logical_children.MoveNext(), "5");
			}
		}
		#endregion

		#region ItemsSource
		[Test]
		public void ItemsCount() {
			ItemsControl c = new ItemsControl();
			ItemCollection i = c.Items;
			object[] o = new object[] {"1"};
			c.ItemsSource = o;
			Assert.AreSame(i.SourceCollection, o, "1");
			Assert.AreEqual(i.Count, 1, "2");
			c.ItemsSource = null;
			Assert.AreSame(i.SourceCollection, i, "3");
			Assert.AreEqual(i.Count, 0, "4");
		}

		[Test]
		public void ItemsSource() {
			Assert.IsNull(new ItemsControl().ItemsSource);
		}

		[Test]
		public void WhenItemsSourceIsSetItemsIsReadOnlyAndFixedSize() {
			ItemsControl c = new ItemsControl();
			ItemCollection i = c.Items;
			IList i_list_items = (IList)i;
			Assert.IsFalse(i_list_items.IsReadOnly, "1");
			Assert.IsFalse(i_list_items.IsFixedSize, "2");
			c.ItemsSource = new object[] { };
			Assert.IsTrue(i_list_items.IsReadOnly, "3");
			Assert.IsTrue(i_list_items.IsFixedSize, "4");
			c.ItemsSource = null;
			Assert.IsFalse(i_list_items.IsReadOnly, "5");
			Assert.IsFalse(i_list_items.IsFixedSize, "6");
		}
		#endregion

		#region ShouldSerializeGroupStyle
		[Test]
		public void ShouldSerializeGroupStyle() {
			ItemsControl c = new ItemsControl();
			Assert.IsFalse(c.ShouldSerializeGroupStyle(), "1");
			c.GroupStyle.Add(new GroupStyle());
			Assert.IsTrue(c.ShouldSerializeGroupStyle(), "2");
		}
		#endregion

		#region ShouldSerializeItems
		[Test]
		public void ShouldSerializeItems() {
			ItemsControl c = new ItemsControl();
			Assert.IsFalse(c.ShouldSerializeItems(), "1");
			c.Items.Add(1);
			Assert.IsTrue(c.ShouldSerializeItems(), "2");
		}
		#endregion

		#region ToString
		[Test]
		public void ToString() {
			ItemsControl c = new ItemsControl();
			Assert.AreEqual(c.ToString(), "System.Windows.Controls.ItemsControl Items.Count:0", "1");
			c.Items.Add(1);
			Assert.AreEqual(c.ToString(), "System.Windows.Controls.ItemsControl Items.Count:1", "2");
		}
		#endregion

		#region AddChild
		[Test]
		public void AddChild() {
			ItemsControl c = new ItemsControl();
			((IAddChild)c).AddChild(1);
			Assert.AreEqual(c.Items[0], 1);
		}
		#endregion

		#region AddText
		[Test]
		public void AddText() {
			ItemsControl c = new ItemsControl();
			((IAddChild)c).AddText("1");
			Assert.AreEqual(c.Items[0], "1");
		}
		#endregion

		#region GetContainerForItemOverride
		[Test]
		public void GetContainerForItemOverride() {
			new GetContainerForItemOverrideItemsControl();
		}

		class GetContainerForItemOverrideItemsControl : ItemsControl {
			public GetContainerForItemOverrideItemsControl() {
				Assert.IsTrue(GetContainerForItemOverride().GetType() == typeof(ContentPresenter));
			}
		}
		#endregion

		#region IsItemItsOwnContainerOverride
		[Test]
		public void IsItemItsOwnContainerOverride() {
			new IsItemItsOwnContainerOverrideItemsControl();
		}

		class IsItemItsOwnContainerOverrideItemsControl : ItemsControl {
			public IsItemItsOwnContainerOverrideItemsControl() {
				Assert.IsFalse(IsItemItsOwnContainerOverride(null), "1");
				Assert.IsFalse(IsItemItsOwnContainerOverride(new DrawingVisual()), "2");
				Assert.IsTrue(IsItemItsOwnContainerOverride(new UIElement()), "3");
			}
		}
		#endregion

		#region PrepareContainerForItemOverride
		[Test]
		public void PrepareContainerForItemOverride() {
			new PrepareContainerForItemOverrideItemsControl();
		}

		class PrepareContainerForItemOverrideItemsControl : ItemsControl {
			public PrepareContainerForItemOverrideItemsControl() {
				ContentPresenter c = new ContentPresenter();
				PrepareContainerForItemOverride(c, 1);
				Assert.AreEqual(c.Content, 1, "1");
				PrepareContainerForItemOverride(new DependencyObject(), 2);
				Grid g = new Grid();
				PrepareContainerForItemOverride(g, 3);
				Assert.AreEqual(g.Children.Count, 0, "2");
				ContentControl c2 = new ContentControl();
				PrepareContainerForItemOverride(c2, 4);
				Assert.AreEqual(c2.Content, 4, "3");
				TestClass c3 = new TestClass();
				PrepareContainerForItemOverride(c3, 5);
				Assert.IsNull(c3.Content, "4");
				PrepareContainerForItemOverride(null, null);
			}

			class TestClass : DependencyObject {
				object content;

				public object Content {
					get { return content; }
					set { content = value; }
				}
	
			}
		}
		#endregion

		#region IAddChild.AddChild, IAddChild.AddText
		[Test]
		public void IAddChild() {
			new IAddChildItemsControl();
		}

		class IAddChildItemsControl : ItemsControl {
			int add_child_calls;
			int add_text_calls;

			public IAddChildItemsControl() {
				Assert.AreEqual(add_child_calls, 0, "1");
				((IAddChild)this).AddChild(1);
				Assert.AreEqual(add_child_calls, 1, "2");
				Assert.AreEqual(add_text_calls, 0, "3");
				((IAddChild)this).AddText("2");
				Assert.AreEqual(add_text_calls, 1, "4");
			}

			protected override void AddChild(object value) {
				add_child_calls++;
				Assert.AreEqual(value, 1, "5");
				base.AddChild(value);
			}

			protected override void AddText(string text) {
				add_text_calls++;
				Assert.AreEqual(text, "2", "6");
				base.AddText(text);
			}
		}
		#endregion
	}
}