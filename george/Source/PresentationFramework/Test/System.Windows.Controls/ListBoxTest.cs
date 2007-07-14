using NUnit.Framework;
using System.Collections;
using System.Collections.ObjectModel;
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
	public class ListBoxTest {
		[Test]
		public void SelectedItemsDefaultValue() {
			IList selected_items = new ListBox().SelectedItems;
			Assert.IsNotNull(selected_items, "1");
			Assert.AreEqual(selected_items.Count, 0, "2");
			Assert.AreEqual(selected_items.GetType(), typeof(ObservableCollection<object>), "3");
		}

		#region HandlesScrolling
		[Test]
		public void HandlesScrolling() {
			new HandlesScrollingListBox();
		}

		class HandlesScrollingListBox : ListBox {
			public HandlesScrollingListBox() {
				Assert.IsTrue(HandlesScrolling);
			}
		}
		#endregion

		#region IsItemItsOwnContainerOverride
		[Test]
		public void IsItemItsOwnContainerOverride() {
			new IsItemItsOwnContainerOverrideListBox();
		}

		class IsItemItsOwnContainerOverrideListBox : ListBox {
			public IsItemItsOwnContainerOverrideListBox() {
				Assert.IsFalse(IsItemItsOwnContainerOverride(null), "1");
				Assert.IsFalse(IsItemItsOwnContainerOverride(string.Empty), "2");
				Assert.IsFalse(IsItemItsOwnContainerOverride(new object()), "3");
				Assert.IsFalse(IsItemItsOwnContainerOverride(new Button()), "4");
				Assert.IsTrue(IsItemItsOwnContainerOverride(new ListBoxItem()), "5");
			}
		}
		#endregion

		#region GetContainerForItemOverride
		[Test]
		public void GetContainerForItemOverride() {
			new GetContainerForItemOverrideListBox();
		}

		class GetContainerForItemOverrideListBox : ListBox {
			public GetContainerForItemOverrideListBox() {
				Assert.IsTrue(GetContainerForItemOverride() is ListBoxItem);
			}
		}
		#endregion

		[Test]
		public void SelectionModeSingle() {
			ListBox l = new ListBox();
			Assert.AreEqual(l.SelectionMode, SelectionMode.Single, "1");
			ListBoxItem i1 = new ListBoxItem();
			i1.IsSelected = true;
			l.Items.Add(i1);
			ListBoxItem i2 = new ListBoxItem();
			i2.IsSelected = true;
			l.Items.Add(i2);
			Assert.IsFalse(i1.IsSelected, "2");
			Assert.IsTrue(i2.IsSelected, "3");
		}
	}
}