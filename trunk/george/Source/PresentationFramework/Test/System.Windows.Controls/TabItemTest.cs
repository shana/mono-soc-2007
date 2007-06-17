using NUnit.Framework;
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
			i1.IsSelected = false;
			Assert.IsFalse(i1.IsSelected, "6");
			Assert.IsNull(t.SelectedItem, "6 1");
			TabItem i2 = new TabItem();
			t.Items.Add(i2);
			i2.IsSelected = true;
			Assert.IsTrue(i2.IsSelected, "7");
			Assert.AreSame(t.SelectedItem, i2, "7 1");
			i1.IsSelected = true;
			Assert.IsTrue(i1.IsSelected, "8");
			Assert.AreSame(t.SelectedItem, i1, "8 1");

			i1.IsSelected = false;
			i2.IsSelected = false;
			Window w = new Window();
			w.Content = t;
			w.Show();
			Assert.IsTrue(i1.IsSelected, "9");
			Assert.IsFalse(i2.IsSelected, "10");
			Assert.AreSame(t.SelectedItem, i1, "10 1");
			i1.IsSelected = true;
			Assert.IsTrue(i1.IsSelected, "11");
			Assert.IsFalse(i2.IsSelected, "12");
			Assert.AreSame(t.SelectedItem, i1, "12 1");
			i2.IsSelected = true;
			Assert.IsFalse(i1.IsSelected, "13");
			Assert.IsTrue(i2.IsSelected, "14");
			Assert.AreSame(t.SelectedItem, i2, "14 1");
			i1.IsSelected = false;
			Assert.IsFalse(i1.IsSelected, "15");
			Assert.IsTrue(i2.IsSelected, "16");
			Assert.AreSame(t.SelectedItem, i2, "16 1");
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

		[Test]
		public void Focusable() {
			Assert.IsTrue(new TabItem().Focusable);
		}

		[Test]
		public void TabStripPlacementBinding() {
			Assert.IsNull(BindingOperations.GetBinding(new TabItem(), TabItem.TabStripPlacementProperty));
		}
	}
}