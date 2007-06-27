using NUnit.Framework;
using System.Windows.Input;
#if Implementation
using System;
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
				Assert.IsFalse(IsSelected, "1");
				IsSelected = true;
				Assert.IsTrue(IsSelected, "2");
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

		#region OnMouseLeftButtonDown
		[Test]
		public void OnMouseLeftButtonDown() {
			new OnMouseLeftButtonDownListBoxItem();
		}

		class OnMouseLeftButtonDownListBoxItem : ListBoxItem {
			public OnMouseLeftButtonDownListBoxItem() {
				MouseButtonEventArgs e = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left);
				e.RoutedEvent = MouseLeftButtonDownEvent;
				OnMouseLeftButtonDown(e);
				Assert.IsFalse(IsSelected);
			}
		}
		#endregion

		#region OnMouseLeftButtonDownInListBox
		[Test]
		public void OnMouseLeftButtonDownInListBox() {
			new OnMouseLeftButtonDownInListBoxListBoxItem();
		}

		class OnMouseLeftButtonDownInListBoxListBoxItem : ListBoxItem {
			public OnMouseLeftButtonDownInListBoxListBoxItem() {
				ListBox l = new ListBox();
				l.Items.Add(this);
				MouseButtonEventArgs e = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left);
				e.RoutedEvent = MouseLeftButtonDownEvent;
				OnMouseLeftButtonDown(e);
				Assert.IsFalse(IsSelected);
			}
		}
		#endregion

		#region OnMouseLeftButtonDownInListBoxSource
		[Test]
		public void OnMouseLeftButtonDownInListBoxSource() {
			new OnMouseLeftButtonDownInListBoxSourceListBoxItem();
		}

		class OnMouseLeftButtonDownInListBoxSourceListBoxItem : ListBoxItem {
			public OnMouseLeftButtonDownInListBoxSourceListBoxItem() {
				ListBox l = new ListBox();
				l.Items.Add(this);
				MouseButtonEventArgs e = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left);
				e.RoutedEvent = MouseLeftButtonDownEvent;
				e.Source = this;
				OnMouseLeftButtonDown(e);
				Assert.IsFalse(IsSelected);
			}
		}
		#endregion

		#region OnMouseLeftButtonDownInListBoxSourceInWindow
		[Test]
		public void OnMouseLeftButtonDownInListBoxSourceInWindow() {
			new OnMouseLeftButtonDownInListBoxSourceInWindowListBoxItem();
		}

		class OnMouseLeftButtonDownInListBoxSourceInWindowListBoxItem : ListBoxItem {
			public OnMouseLeftButtonDownInListBoxSourceInWindowListBoxItem() {
				ListBox l = new ListBox();
				l.Items.Add(this);
				MouseButtonEventArgs e = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left);
				e.RoutedEvent = MouseLeftButtonDownEvent;
				e.Source = this;
				Window w = new Window();
				w.Content = l;
				w.Show();
				OnMouseLeftButtonDown(e);
				Assert.IsTrue(IsSelected);
			}
		}
		#endregion

		#region OnMouseLeftButtonDownInListBoxInWindow
		[Test]
		public void OnMouseLeftButtonDownInListBoxInWindow() {
			new OnMouseLeftButtonDownInListBoxInWindowListBoxItem();
		}

		class OnMouseLeftButtonDownInListBoxInWindowListBoxItem : ListBoxItem {
			public OnMouseLeftButtonDownInListBoxInWindowListBoxItem() {
				ListBox l = new ListBox();
				l.Items.Add(this);
				MouseButtonEventArgs e = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left);
				e.RoutedEvent = MouseLeftButtonDownEvent;
				Window w = new Window();
				w.Content = l;
				w.Show();
				OnMouseLeftButtonDown(e);
				Assert.IsTrue(IsSelected);
			}
		}
		#endregion

		#region OnMouseLeftButtonDownInListBoxInWindowNotShown
		[Test]
		public void OnMouseLeftButtonDownInListBoxInWindowNotShown() {
			new OnMouseLeftButtonDownInListBoxInWindowNotShownListBoxItem();
		}

		class OnMouseLeftButtonDownInListBoxInWindowNotShownListBoxItem : ListBoxItem {
			public OnMouseLeftButtonDownInListBoxInWindowNotShownListBoxItem() {
				ListBox l = new ListBox();
				l.Items.Add(this);
				MouseButtonEventArgs e = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left);
				e.RoutedEvent = MouseLeftButtonDownEvent;
				Window w = new Window();
				w.Content = l;
				OnMouseLeftButtonDown(e);
				Assert.IsFalse(IsSelected);
			}
		}
		#endregion


		#region OnMouseLeftButtonDownInListBoxInCanvas
		[Test]
		public void OnMouseLeftButtonDownInListBoxInCanvas() {
			new OnMouseLeftButtonDownInListBoxInCanvasListBoxItem();
		}

		class OnMouseLeftButtonDownInListBoxInCanvasListBoxItem : ListBoxItem {
			public OnMouseLeftButtonDownInListBoxInCanvasListBoxItem() {
				ListBox l = new ListBox();
				l.Items.Add(this);
				MouseButtonEventArgs e = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left);
				e.RoutedEvent = MouseLeftButtonDownEvent;
				Canvas c = new Canvas();
				c.Children.Add(l);
				OnMouseLeftButtonDown(e);
				Assert.IsFalse(IsSelected);
			}
		}
		#endregion
	}
}