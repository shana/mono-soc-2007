using NUnit.Framework;
using System.Windows.Data;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class TabControlTest {
		#region SelectedContent
		[Test]
		public void SelectedContent() {
			new SelectedContentTabControl();
		}

		class SelectedContentTabControl : TabControl {
			public SelectedContentTabControl() {
				Assert.IsNull(SelectedContent, "1");
				Items.Add("Test");
				Assert.IsNull(SelectedContent, "2");
				try {
					OnSelectionChanged(new SelectionChangedEventArgs(SelectionChangedEvent, null, new object[] { Items[0] }));
					Assert.Fail("3");
				} catch (ArgumentException) {
				}
				OnSelectionChanged(new SelectionChangedEventArgs(SelectionChangedEvent, new object[] { }, new object[] { Items[0] }));
				Assert.IsNull(SelectedContent, "4");
				Items.Clear();
				TabItem tab_item = new TabItem();
				tab_item.Content = "Test";
				Items.Add(tab_item);
				Assert.IsNull(SelectedContent, "5");
				OnSelectionChanged(new SelectionChangedEventArgs(SelectionChangedEvent, new object[] { }, new object[] { tab_item }));
				Assert.IsNull(SelectedContent, "6");
				SelectionChangedEventArgs e = new SelectionChangedEventArgs(SelectionChangedEvent, new object[] { }, new object[] { tab_item });
				e.Source = this;
				OnSelectionChanged(e);
				Assert.IsNull(SelectedContent, "7");
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.AreEqual((string)SelectedContent, "Test", "8");
			}
		}
		#endregion

		#region GetContainerForItemOverride
		[Test]
		public void GetContainerForItemOverride() {
			new GetContainerForItemOverrideTabControl();
		}

		class GetContainerForItemOverrideTabControl : TabControl {
			public GetContainerForItemOverrideTabControl() {
				object result = GetContainerForItemOverride();
				Assert.AreEqual(result.GetType(), typeof(TabItem));
			}
		}
		#endregion

		#region IsItemItsOwnContainerOverride
		[Test]
		public void IsItemItsOwnContainerOverride() {
			new IsItemItsOwnContainerOverrideTabControl();
		}

		class IsItemItsOwnContainerOverrideTabControl : TabControl {
			public IsItemItsOwnContainerOverrideTabControl() {
				Assert.IsTrue(IsItemItsOwnContainerOverride(new TabItem()), "1");
				Assert.IsFalse(IsItemItsOwnContainerOverride(null), "2");
				Assert.IsFalse(IsItemItsOwnContainerOverride(string.Empty), "3");
				Assert.IsFalse(IsItemItsOwnContainerOverride("False"), "4");
				Assert.IsFalse(IsItemItsOwnContainerOverride(new object()), "5");
			}
		}
		#endregion

		[Test]
		public void TabStripPlacementBinding() {
			Assert.IsNull(BindingOperations.GetBinding(new TabControl(), TabControl.TabStripPlacementProperty));
		}
	}
}