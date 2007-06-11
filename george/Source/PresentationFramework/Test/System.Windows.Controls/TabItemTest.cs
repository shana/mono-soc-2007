using NUnit.Framework;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class TabItemTest {
		#region Selected
		[Test]
		public void Selected() {
			new SelectedTabItem();
		}

		class SelectedTabItem : TabItem {
			bool on_selected_called;
			bool on_unselected_called;
			RoutedEventArgs on_selected_event_args;
			RoutedEventArgs on_unselected_event_args;

			public SelectedTabItem() {
				Assert.IsFalse(IsSelected, "1");
				Assert.IsFalse(on_selected_called, "2");
				Assert.IsFalse(on_unselected_called, "3");
				IsSelected = false;
				Assert.IsFalse(on_selected_called, "4");
				Assert.IsFalse(on_unselected_called, "5");
				IsSelected = true;
				Assert.IsTrue(on_selected_called, "6");
				on_selected_called = false;
				Assert.IsFalse(on_unselected_called, "7");
				IsSelected = false;
				Assert.IsFalse(on_selected_called, "8");
				Assert.IsTrue(on_unselected_called, "9");
				Assert.AreEqual(on_unselected_event_args.Source, this, "10");
				RoutedEvent unselected_event = on_unselected_event_args.RoutedEvent;
				Assert.AreEqual(unselected_event.Name, "Unselected", "11");
				Assert.AreEqual(unselected_event.RoutingStrategy, RoutingStrategy.Bubble, "12");
				IsSelected = true;
				IsSelected = false;
				Assert.AreSame(on_unselected_event_args.RoutedEvent, unselected_event, "13");
			}

			protected override void OnSelected(RoutedEventArgs e) {
				on_selected_called = true;
				on_selected_event_args = e;
				base.OnSelected(e);
			}

			protected override void OnUnselected(RoutedEventArgs e) {
				on_unselected_called = true;
				on_unselected_event_args = e;
				base.OnUnselected(e);
			}
		}
		#endregion

		[Test]
		public void TabStripPlacement() {
			TabItem tab_item = new TabItem();
			Assert.AreEqual(tab_item.TabStripPlacement, Dock.Top, "1");
		}
	}
}