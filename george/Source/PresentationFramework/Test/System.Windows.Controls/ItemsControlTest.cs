using NUnit.Framework;
using System.Collections;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class ItemsControlTest {
		[Test]
		public void ItemContainerGenerator() {
			Assert.IsNotNull(new ItemsControl().ItemContainerGenerator);
		}

		[Test]
		public void Items() {
			Assert.IsNotNull(new ItemsControl().Items, "1");
			Assert.AreEqual(new ItemsControl().Items.GetType(), typeof(ItemCollection), "2");
		}

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
	}
}