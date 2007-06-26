using NUnit.Framework;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class ListBoxItemTest {
		#region OnSelected
		[Test]
		public void OnSelected() {
			new OnSelectedListBoxItem();
		}

		class OnSelectedListBoxItem : ListBoxItem {
			int on_selected_calls;
			int on_unselected_calls;
			int selected_calls;
			int unselected_calls;
			int order_counter;

			public OnSelectedListBoxItem() {
				Selected += new RoutedEventHandler(OnSelectedListBoxItem_Selected);
				Unselected += new RoutedEventHandler(OnSelectedListBoxItem_Unselected);
				Assert.AreEqual(IsSelected, false, "1");
				IsSelected = true;
				Assert.AreEqual(IsSelected, true, "2");
				Assert.AreEqual(on_selected_calls, 1, "3");
				Assert.AreEqual(on_unselected_calls, 0, "4");
				Assert.AreEqual(selected_calls, 1, "5");
				Assert.AreEqual(unselected_calls, 0, "6");
				Assert.AreEqual(order_counter, 3, "7");
			}

			void OnSelectedListBoxItem_Unselected(object sender, RoutedEventArgs e) {
				unselected_calls++;
			}

			void OnSelectedListBoxItem_Selected(object sender, RoutedEventArgs e) {
				selected_calls++;
				Assert.AreEqual(order_counter, 1, "8");
				order_counter++;
			}

			protected override void OnSelected(RoutedEventArgs e) {
				Assert.AreSame(e.Source, this, "9");
				Assert.AreSame(e.RoutedEvent, SelectedEvent, "10");
				on_selected_calls++;
				Assert.AreEqual(order_counter, 0, "11");
				order_counter++;
				base.OnSelected(e);
				Assert.AreEqual(order_counter, 2, "12");
				order_counter++;
			}

			protected override void OnUnselected(RoutedEventArgs e) {
				Assert.AreSame(e.Source, this, "13");
				Assert.AreSame(e.RoutedEvent, UnselectedEvent, "14");
				on_unselected_calls++;
				base.OnUnselected(e);
			}
		}
		#endregion
	}
}