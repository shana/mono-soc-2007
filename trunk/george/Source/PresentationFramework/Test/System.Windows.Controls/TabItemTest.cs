using NUnit.Framework;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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
#if Implementation
	[Ignore("Not implementable now")]
#endif
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
				Assert.AreSame(unselected_event, Selector.UnselectedEvent, "10 1");
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

		[Test]
		public void IsSelected() {
			TabItem i1 = new TabItem();
			Assert.IsFalse(i1.IsSelected, "1");
			i1.IsSelected = true;
			Assert.IsTrue(i1.IsSelected, "2");
			i1.IsSelected = false;
			Assert.IsFalse(i1.IsSelected, "3");
			TabControl t = new TabControl();
			t.Items.Add(i1);
			Assert.IsFalse(i1.IsSelected, "4");
			Assert.IsNull(t.SelectedItem, "4 1");
			i1.IsSelected = true;
			Assert.IsTrue(i1.IsSelected, "5");
			Assert.AreEqual(t.SelectedItem, i1, "5 1");
			Assert.AreEqual(t.SelectedIndex, 0, "5 2");
			i1.IsSelected = false;
			Assert.IsFalse(i1.IsSelected, "6");
			Assert.IsNull(t.SelectedItem, "6 1");
			Assert.AreEqual(t.SelectedIndex, -1, "6 2");
			TabItem i2 = new TabItem();
			t.Items.Add(i2);
			i2.IsSelected = true;
			Assert.IsTrue(i2.IsSelected, "7");
			Assert.AreSame(t.SelectedItem, i2, "7 1");
			Assert.AreEqual(t.SelectedIndex, 1, "7 2");
			i1.IsSelected = true;
			Assert.IsTrue(i1.IsSelected, "8");
			Assert.AreSame(t.SelectedItem, i1, "8 1");
			Assert.AreEqual(t.SelectedIndex, 0, "8 2");

			i1.IsSelected = false;
			i2.IsSelected = false;
			Window w = new Window();
			w.Content = t;
			w.Show();
			Assert.IsTrue(i1.IsSelected, "9");
			Assert.IsFalse(i2.IsSelected, "10");
			Assert.AreSame(t.SelectedItem, i1, "10 1");
			Assert.AreEqual(t.SelectedIndex, 0, "10 2");
			i1.IsSelected = true;
			Assert.IsTrue(i1.IsSelected, "11");
			Assert.IsFalse(i2.IsSelected, "12");
			Assert.AreSame(t.SelectedItem, i1, "12 1");
			Assert.AreEqual(t.SelectedIndex, 0, "12 2");
			i2.IsSelected = true;
			Assert.IsFalse(i1.IsSelected, "13");
			Assert.IsTrue(i2.IsSelected, "14");
			Assert.AreSame(t.SelectedItem, i2, "14 1");
			Assert.AreEqual(t.SelectedIndex, 1, "14 2");
			i1.IsSelected = false;
			Assert.IsFalse(i1.IsSelected, "15");
			Assert.IsTrue(i2.IsSelected, "16");
			Assert.AreSame(t.SelectedItem, i2, "16 1");
			Assert.AreEqual(t.SelectedIndex, 1, "16 2");
		}

		[Test]
		public void IsSelectedSimplified() {
			TabControl t = new TabControl();
			TabItem i1 = new TabItem();
			t.Items.Add(i1);
			Assert.AreEqual(t.ItemContainerGenerator.Status, GeneratorStatus.NotStarted, "1");
			i1.IsSelected = true;
			Assert.AreEqual(t.SelectedItem, i1, "2");
		}

		#region OnApplyTemplateSetsIsSelected
		[Test]
		public void OnApplyTemplateSetsIsSelected() {
			new OnApplyTemplateSetsIsSelectedTabControl();
		}

		class OnApplyTemplateSetsIsSelectedTabControl : TabControl {
			public OnApplyTemplateSetsIsSelectedTabControl() {
				TabItem t = new TabItem();
				Items.Add(t);
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.IsTrue(t.IsSelected);
			}

			public override void OnApplyTemplate() {
			}
		}
		#endregion

		#region OnApplyTemplateSetsIsSelected2
		[Test]
		public void OnApplyTemplateSetsIsSelected2() {
			new OnApplyTemplateSetsIsSelected2TabControl();
		}

		class OnApplyTemplateSetsIsSelected2TabControl : TabControl {
			public OnApplyTemplateSetsIsSelected2TabControl() {
				TabItem t = new TabItem();
				Items.Add(t);
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.IsTrue(t.IsSelected);
			}
		}
		#endregion


		#region OnMouseLeftButtonDown
		[Test]
		public void OnMouseLeftButtonDown() {
			new OnMouseLeftButtonDownTabItem();
		}

		class OnMouseLeftButtonDownTabItem : TabItem {
			public OnMouseLeftButtonDownTabItem() {
				MouseButtonEventArgs e = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left);
				e.RoutedEvent = MouseLeftButtonDownEvent;
				OnMouseLeftButtonDown(e);
				Assert.IsFalse(IsSelected, "1");
				Assert.IsTrue(e.Handled, "2");
				e.Handled = false;
				TabControl t = new TabControl();
				t.Items.Add(new TabItem());
				t.Items.Add(this);
				OnMouseLeftButtonDown(e);
				Assert.IsFalse(IsSelected, "3");
				Assert.IsTrue(e.Handled, "4");
				e.Handled = false;
				Window w = new Window();
				w.Content = t;
				w.Show();
				OnMouseLeftButtonDown(e);
				Assert.IsTrue(IsSelected, "5");
				Assert.IsTrue(e.Handled, "6");
			}
		}
		#endregion

		#region OnMouseLeftButtonDownInCanvas
		[Test]
		public void OnMouseLeftButtonDownInCanvas() {
			new OnMouseLeftButtonDownInCanvasTabItem();
		}

		class OnMouseLeftButtonDownInCanvasTabItem : TabItem {
			public OnMouseLeftButtonDownInCanvasTabItem() {
				MouseButtonEventArgs e = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left);
				e.RoutedEvent = MouseLeftButtonDownEvent;
				TabControl t = new TabControl();
				t.Items.Add(new TabItem());
				t.Items.Add(this);
				Canvas c = new Canvas();
				c.Children.Add(t);
				OnMouseLeftButtonDown(e);
				Assert.IsFalse(IsSelected, "1");
				Assert.IsTrue(e.Handled, "2");
			}
		}
		#endregion

		[Test]
		public void Focusable() {
			Assert.IsTrue(new TabItem().Focusable);
		}

		[Test]
		public void TabStripPlacementBinding() {
			Assert.IsNull(BindingOperations.GetBinding(new TabItem(), TabItem.TabStripPlacementProperty));
		}

		#region OnSelected
		[Test]
		public void OnSelected() {
			TabControl tab_control = new TabControl();
			OnSelectedTabItem i1 = new OnSelectedTabItem();
			tab_control.Items.Add(i1);
			OnSelectedTabItem i2 = new OnSelectedTabItem();
			tab_control.Items.Add(i2);
			i1.IsSelected = true;
			Assert.AreEqual(tab_control.SelectedIndex, -1, "1");
			i2.IsSelected = true;
			Assert.AreEqual(tab_control.SelectedIndex, -1, "2");
			Assert.IsTrue(i1.IsSelected, "3");
			Assert.IsTrue(i2.IsSelected, "4");
		}

		[Test]
		public void OnSelected2() {
			TabControl tab_control = new TabControl();
			TabItem i1 = new TabItem();
			tab_control.Items.Add(i1);
			TabItem i2 = new TabItem();
			tab_control.Items.Add(i2);
			i1.IsSelected = true;
			Assert.AreEqual(tab_control.SelectedIndex, 0, "1");
			i2.IsSelected = true;
			Assert.AreEqual(tab_control.SelectedIndex, 1, "2");
			Assert.IsFalse(i1.IsSelected, "3");
			Assert.IsTrue(i2.IsSelected, "4");
		}

		[Test]
		public void OnSelectedInWindow() {
			Window w = new Window();
			TabControl tab_control = new TabControl();
			w.Content = tab_control;
			w.Show();
			Assert.AreEqual(tab_control.SelectedIndex, -1, "1");
			OnSelectedTabItem i1 = new OnSelectedTabItem();
			tab_control.Items.Add(i1);
			Assert.AreEqual(tab_control.SelectedIndex, -1, "2");
			OnSelectedTabItem i2 = new OnSelectedTabItem();
			tab_control.Items.Add(i2);
			Assert.AreEqual(tab_control.SelectedIndex, 0, "3");
			Assert.IsTrue(i1.IsSelected, "4");
			i1.IsSelected = true;
			Assert.AreEqual(tab_control.SelectedIndex, 0, "5");
			i2.IsSelected = true;
			Assert.AreEqual(tab_control.SelectedIndex, 0, "6");
			Assert.IsTrue(i1.IsSelected, "7");
			Assert.IsTrue(i2.IsSelected, "8");
		}

		[Test]
		public void OnSelectedInWindow3() {
			Window w = new Window();
			OnSelectedTabControl tab_control = new OnSelectedTabControl();
			w.Content = tab_control;
			w.Show();
			Assert.AreEqual(tab_control.SelectedIndex, -1, "1");
			OnSelectedTabItem i1 = new OnSelectedTabItem();
			tab_control.Items.Add(i1);
			Assert.AreEqual(tab_control.SelectedIndex, -1, "2");
			OnSelectedTabItem i2 = new OnSelectedTabItem();
			tab_control.Items.Add(i2);
			Assert.AreEqual(tab_control.SelectedIndex, 0, "3");
			Assert.IsTrue(i1.IsSelected, "4");
			i1.IsSelected = true;
			Assert.AreEqual(tab_control.SelectedIndex, 0, "5");
			i2.IsSelected = true;
			Assert.AreEqual(tab_control.SelectedIndex, 0, "6");
			Assert.IsTrue(i1.IsSelected, "7");
			Assert.IsTrue(i2.IsSelected, "8");
		}

		[Test]
		public void OnSelectedInWindow4() {
			Window w = new Window();
			OnSelectedTabControl2 tab_control = new OnSelectedTabControl2();
			w.Content = tab_control;
			w.Show();
			Assert.AreEqual(tab_control.SelectedIndex, -1, "1");
			OnSelectedTabItem i1 = new OnSelectedTabItem();
			tab_control.Items.Add(i1);
			Assert.AreEqual(tab_control.SelectedIndex, -1, "2");
			OnSelectedTabItem i2 = new OnSelectedTabItem();
			tab_control.Items.Add(i2);
			Assert.AreEqual(tab_control.SelectedIndex, 0, "3");
			Assert.IsTrue(i1.IsSelected, "4");
			i1.IsSelected = true;
			Assert.AreEqual(tab_control.SelectedIndex, 0, "5");
			i2.IsSelected = true;
			Assert.AreEqual(tab_control.SelectedIndex, 0, "6");
			Assert.IsTrue(i1.IsSelected, "7");
			Assert.IsTrue(i2.IsSelected, "8");
		}

		[Test]
		public void OnSelectedInWindow5() {
			Window w = new Window();
			TabControl tab_control = new TabControl();
			w.Content = tab_control;
			w.Show();
			Assert.AreEqual(tab_control.SelectedIndex, -1, "1");
			OnSelectedTabItem2 i1 = new OnSelectedTabItem2();
			tab_control.Items.Add(i1);
			Assert.AreEqual(tab_control.SelectedIndex, -1, "2");
			OnSelectedTabItem2 i2 = new OnSelectedTabItem2();
			tab_control.Items.Add(i2);
			Assert.AreEqual(tab_control.SelectedIndex, 0, "3");
			Assert.IsTrue(i1.IsSelected, "4");
			i1.IsSelected = true;
			Assert.AreEqual(tab_control.SelectedIndex, 0, "5");
			i2.IsSelected = true;
			Assert.AreEqual(tab_control.SelectedIndex, 0, "6");
			Assert.IsTrue(i1.IsSelected, "7");
			Assert.IsTrue(i2.IsSelected, "8");
		}

		[Test]
		public void OnSelectedInWindow2() {
			TabControl tab_control = new TabControl();
			OnSelectedTabItem i1 = new OnSelectedTabItem();
			tab_control.Items.Add(i1);
			OnSelectedTabItem i2 = new OnSelectedTabItem();
			tab_control.Items.Add(i2);
			i1.IsSelected = true;
			i2.IsSelected = true;
			Window w = new Window();
			w.Content = tab_control;
			w.Show();
			Assert.AreEqual(tab_control.SelectedIndex, 0, "1");
			Assert.IsTrue(i1.IsSelected, "2");
			Assert.IsTrue(i2.IsSelected, "3");
		}

		class OnSelectedTabItem : TabItem {
			protected override void OnSelected(RoutedEventArgs e) {
			}
		}

		class OnSelectedTabItem2 : TabItem {
			protected override void OnSelected(RoutedEventArgs e) {
			}

			protected override void OnUnselected(RoutedEventArgs e) {
			}
		}

		class OnSelectedTabControl : TabControl {
			protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			}
		}

		class OnSelectedTabControl2 : TabControl {
			protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			}
		}
		#endregion

		#region OnUnselected
		[Test]
		public void OnUnselected() {
			TabControl tab_control = new TabControl();
			OnUnselectedTabItem i1 = new OnUnselectedTabItem();
			tab_control.Items.Add(i1);
			OnUnselectedTabItem i2 = new OnUnselectedTabItem();
			tab_control.Items.Add(i2);
			i1.IsSelected = true;
			Assert.AreEqual(tab_control.SelectedIndex, 0, "1");
			Assert.IsTrue(i1.IsSelected, "1 1");
			i2.IsSelected = true;
			Assert.AreEqual(tab_control.SelectedIndex, 1, "2");
			Assert.IsFalse(i1.IsSelected, "3");
			Assert.IsTrue(i2.IsSelected, "4");
		}

		class OnUnselectedTabItem : TabItem {
			protected override void OnUnselected(RoutedEventArgs e) {
			}
		}
		#endregion

		[Test]
		public void OnUnselectedSimplified() {
			TabControl tab_control = new TabControl();
			TabItem i1 = new TabItem();
			tab_control.Items.Add(i1);
			TabItem i2 = new TabItem();
			tab_control.Items.Add(i2);
			i1.IsSelected = true;
			Assert.IsTrue(i1.IsSelected, "1");
			i2.IsSelected = true;
			Assert.IsFalse(i1.IsSelected, "2");
		}

		#region StrangeCase
		[Test]
		public void StrangeCase() {
			TabControl t = new TabControl();
			Window w = new Window();
			w.Content = t;
			w.Show();
			t.Items.Add(new StrangeCaseTabItem());
			Assert.AreEqual(t.SelectedIndex, -1, "1");
			t.Items.Add(new StrangeCaseTabItem());
			Assert.AreEqual(t.SelectedIndex, 0, "2");
		}

		class StrangeCaseTabItem : TabItem {
			protected override void OnSelected(RoutedEventArgs e) {
			}
		}
		#endregion

		#region StrangeCase2
		[Test]
		public void StrangeCase2() {
			TabControl t = new TabControl();
			Window w = new Window();
			w.Content = t;
			w.Show();
			t.Items.Add(new StrangeCase2TabItem());
			Assert.AreEqual(t.SelectedIndex, -1, "1");
			t.Items.Add(new StrangeCase2TabItem());
			Assert.AreEqual(t.SelectedIndex, 0, "2");
		}

		class StrangeCase2TabItem : TabItem {
			protected override void OnSelected(RoutedEventArgs e) {
			}

			protected override void OnUnselected(RoutedEventArgs e) {
			}
		}
		#endregion

		#region StrangeCase3
		[Test]
		public void StrangeCase3() {
			TabControl t = new TabControl();
			Window w = new Window();
			w.Content = t;
			w.Show();
			t.Items.Add(new StrangeCase3TabItem());
			Assert.AreEqual(t.SelectedIndex, -1, "1");
			t.Items.Add(new StrangeCase3TabItem());
			Assert.AreEqual(t.SelectedIndex, 0, "2");
		}

		class StrangeCase3TabItem : TabItem {
			protected override void OnUnselected(RoutedEventArgs e) {
			}
		}
		#endregion

		#region StrangeCase4
		[Test]
		public void StrangeCase4() {
			TabControl t = new TabControl();
			Window w = new Window();
			w.Content = t;
			w.Show();
			Assert.AreEqual(t.SelectedIndex, -1, "0");
			TabItem i = new TabItem();
			i.Content = 1;
			t.Items.Add(i);
			Assert.AreEqual(t.SelectedIndex, -1, "1");
			Assert.IsNull(t.SelectedContent, "1 1");
			t.Items.Add(new TabItem());
			Assert.AreEqual(t.SelectedIndex, 0, "2");
		}
		#endregion

		#region StrangeCase5
		[Test]
		public void StrangeCase5() {
			TabControl t = new TabControl();
			t.Items.Add(new TabItem());
			Assert.AreEqual(t.SelectedIndex, -1, "1");
			t.Items.Add(new TabItem());
			Assert.AreEqual(t.SelectedIndex, -1, "2");
		}
		#endregion

		#region StrangeCase6
		[Test]
		public void StrangeCase6() {
			TabControl t = new TabControl();
			t.Items.Add(new TabItem());
			t.Items.Add(new TabItem());
			Window w = new Window();
			w.Content = t;
			w.Show();
			Assert.AreEqual(t.SelectedIndex, 0);
		}
		#endregion

		#region StrangeCase7
		[Test]
		public void StrangeCase7() {
			TabControl t = new TabControl();
			t.Items.Add(new TabItem());
			Window w = new Window();
			w.Content = t;
			w.Show();
			Assert.AreEqual(t.SelectedIndex, 0);
		}
		#endregion

		#region StrangeCase8
		[Test]
		public void StrangeCase8() {
			TabControl t = new TabControl();
			Window w = new Window();
			w.Content = t;
			w.Show();
			t.Items.Add(new TabItem());
			Assert.AreEqual(t.SelectedIndex, -1);
		}
		#endregion

		#region StrangeCase71
		[Test]
		public void StrangeCase71() {
			TabControl t = new TabControl();
			t.Items.Add(new object());
			Window w = new Window();
			w.Content = t;
			w.Show();
			Assert.AreEqual(t.SelectedIndex, 0);
		}
		#endregion

		#region StrangeCase81
		[Test]
		public void StrangeCase81() {
			TabControl t = new TabControl();
			Window w = new Window();
			w.Content = t;
			w.Show();
			t.Items.Add(new object());
			Assert.AreEqual(t.SelectedIndex, -1);
		}
		#endregion
	}
}