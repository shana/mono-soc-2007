using NUnit.Framework;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using Mono.System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls {
#else
using System.Windows.Controls.Primitives;
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
				TabItem tab_item = result as TabItem;
				Assert.IsNotNull(tab_item, "1");
				Assert.IsNull(tab_item.Parent, "2");
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

		#region Items
		[Test]
		public void Items() {
			new ItemsTabControl();
		}

		class ItemsTabControl : TabControl {
			public ItemsTabControl() {
				Window window = new Window();
				window.Content = this;
				window.Show();
				TabItem tab_item = new TabItem();
				Items.Add(tab_item);
				Assert.AreSame(HeaderPanel.Children[0], tab_item);
			}

			TabPanel HeaderPanel {
				get {
					return (TabPanel)GetTemplateChild("HeaderPanel");
				}
			}
		}
		#endregion

		#region Template
		[Test]
		public void Template() {
			new TemplateTabControl();
		}

		class TemplateTabControl : TabControl {
			public TemplateTabControl() {
				Window window = new Window();
				window.Content = this;
				window.Show();
				Assert.IsNotNull(HeaderPanel);
			}

			TabPanel HeaderPanel {
				get {
					return (TabPanel)GetTemplateChild("HeaderPanel");
				}
			}
		}
		#endregion

		#region OnInitializedCallsOnApplyTemplate
		[Test]
		public void OnInitializedCallsOnApplyTemplate() {
			new OnInitializedCallsOnApplyTemplateTabControl();
		}

		class OnInitializedCallsOnApplyTemplateTabControl : TabControl {
			bool should_set_called;
			bool called;

			public OnInitializedCallsOnApplyTemplateTabControl() {
				Window window = new Window();
				window.Content = this;
				window.Show();
				Assert.IsFalse(called);
			}

			protected override void OnInitialized(EventArgs e) {
				should_set_called = true;
				base.OnInitialized(e);
				should_set_called = false;
			}

			public override void OnApplyTemplate() {
				if (should_set_called)
				called = true;
				base.OnApplyTemplate();
			}
		}
		#endregion

		#region OnApplyTemplateUpdatesSelectedProperties
		[Test]
		public void OnApplyTemplateUpdatesSelectedProperties() {
			new OnApplyTemplateUpdatesSelectedPropertiesTabControl();
		}

		class OnApplyTemplateUpdatesSelectedPropertiesTabControl : TabControl {
			bool call;

			public OnApplyTemplateUpdatesSelectedPropertiesTabControl() {
				Window w = new Window();
				w.Content = this;
				w.Show();
				TabItem t = new TabItem();
				t.Content = "Test";
				Items.Add(t);
				Assert.AreNotEqual((string)SelectedContent, "Test", "1");
				call = true;
				OnApplyTemplate();
				Assert.AreNotEqual((string)SelectedContent, "Test", "2");
			}

			public override void OnApplyTemplate() {
				if (call)
					base.OnApplyTemplate();
			}
		}
		#endregion

		#region OnApplyTemplateUpdatesSelectedProperties2
		[Test]
		public void OnApplyTemplateUpdatesSelectedProperties2() {
			new OnApplyTemplateUpdatesSelectedProperties2TabControl();
		}

		class OnApplyTemplateUpdatesSelectedProperties2TabControl : TabControl {
			public OnApplyTemplateUpdatesSelectedProperties2TabControl() {
				TabItem t = new TabItem();
				t.Content = "Test";
				Items.Add(t);
				Assert.AreNotEqual((string)SelectedContent, "Test", "1");
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.AreEqual((string)SelectedContent, "Test", "2");
			}
		}
		#endregion

		#region OnApplyTemplateUpdatesSelectedProperties3
		[Test]
		public void OnApplyTemplateUpdatesSelectedProperties3() {
			new OnApplyTemplateUpdatesSelectedProperties3TabControl();
		}

		class OnApplyTemplateUpdatesSelectedProperties3TabControl : TabControl {
			public OnApplyTemplateUpdatesSelectedProperties3TabControl() {
				TabItem t = new TabItem();
				t.Content = "Test";
				Items.Add(t);
				Assert.AreEqual(SelectedIndex, -1, "1 1");
				Assert.AreNotEqual((string)SelectedContent, "Test", "1");
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.AreEqual(SelectedIndex, 0, "2 1");
				Assert.AreEqual((string)SelectedContent, "Test", "2");
			}

			public override void OnApplyTemplate() {
			}
		}
		#endregion

		#region OnApplyTemplateUpdatesSelectedProperties4
		[Test]
		public void OnApplyTemplateUpdatesSelectedProperties4() {
			new OnApplyTemplateUpdatesSelectedProperties4TabControl();
		}

		class OnApplyTemplateUpdatesSelectedProperties4TabControl : TabControl {
			public OnApplyTemplateUpdatesSelectedProperties4TabControl() {
				TabItem t = new TabItem();
				t.Content = "Test";
				Items.Add(t);
				Assert.AreNotEqual((string)SelectedContent, "Test", "1");
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.AreEqual((string)SelectedContent, "Test", "2");
			}

			protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			}
		}
		#endregion
		
		#region OnApplyTemplateUpdatesSelectedProperties7
		[Test]
		public void OnApplyTemplateUpdatesSelectedProperties7() {
			new OnApplyTemplateUpdatesSelectedProperties7TabControl();
		}

		class OnApplyTemplateUpdatesSelectedProperties7TabControl : TabControl {
			public OnApplyTemplateUpdatesSelectedProperties7TabControl() {
				TestTabItem t = new TestTabItem();
				t.Content = "Test";
				Items.Add(t);
				Assert.AreNotEqual((string)SelectedContent, "Test", "1");
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.AreEqual(SelectedIndex, 0, "2");
				Assert.AreEqual((string)SelectedContent, "Test", "3");
				Assert.AreEqual(SelectedIndex, 0, "4");
			}

			protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			}

			class TestTabItem : TabItem {
				protected override void OnSelected(RoutedEventArgs e) {
				}
			}
		}
		#endregion

		#region OnApplyTemplateUpdatesSelectedProperties8
		[Test]
		public void OnApplyTemplateUpdatesSelectedProperties8() {
			new OnApplyTemplateUpdatesSelectedProperties8TabControl();
		}

		class OnApplyTemplateUpdatesSelectedProperties8TabControl : TabControl {
			public OnApplyTemplateUpdatesSelectedProperties8TabControl() {
				TestTabItem t = new TestTabItem();
				t.Content = "Test";
				Items.Add(t);
				Assert.AreNotEqual((string)SelectedContent, "Test", "1");
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.AreEqual(SelectedIndex, 0, "2");
				Assert.AreEqual((string)SelectedContent, "Test", "3");
				Assert.AreEqual(SelectedIndex, 0, "4");
			}

			protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			}

			public override void OnApplyTemplate() {
			}

			class TestTabItem : TabItem {
				protected override void OnSelected(RoutedEventArgs e) {
				}
			}
		}
		#endregion
		
		#region OnApplyTemplateUpdatesSelectedProperties5
		[Test]
		public void OnApplyTemplateUpdatesSelectedProperties5() {
			new OnApplyTemplateUpdatesSelectedProperties5TabControl();
		}

		class OnApplyTemplateUpdatesSelectedProperties5TabControl : TabControl {
			public OnApplyTemplateUpdatesSelectedProperties5TabControl() {
				TabItem t = new TabItem();
				t.Content = "Test";
				Items.Add(t);
				Assert.AreNotEqual((string)SelectedContent, "Test", "1");
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.AreEqual((string)SelectedContent, "Test", "2");
			}

			protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			}
		}
		#endregion

		#region OnApplyTemplateUpdatesSelectedProperties6
		[Test]
		public void OnApplyTemplateUpdatesSelectedProperties6() {
			new OnApplyTemplateUpdatesSelectedProperties6TabControl();
		}

		class OnApplyTemplateUpdatesSelectedProperties6TabControl : TabControl {
			public OnApplyTemplateUpdatesSelectedProperties6TabControl() {
				TabItem t = new TabItem();
				t.Content = "Test";
				Items.Add(t);
				Assert.AreNotEqual((string)SelectedContent, "Test", "1");
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.AreNotEqual((string)SelectedContent, "Test", "2");
			}

			protected override void OnInitialized(EventArgs e) {
			}
		}
		#endregion

		#region OnSelectionChangedCalled
		[Test]
		public void OnSelectionChangedCalled() {
			new OnSelectionChangedCalledTabControl();
		}

		class OnSelectionChangedCalledTabControl : TabControl {
			bool called;

			public OnSelectionChangedCalledTabControl() {
				Window w = new Window();
				w.Content = this;
				w.Show();
				TabItem t = new TabItem();
				t.Content = "Test";
				Items.Add(t);
				Assert.IsFalse(called);
			}

			protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
				called = true;
				base.OnSelectionChanged(e);
			}
		}
		#endregion

		#region ItemIsSelectedSet
		[Test]
		public void ItemIsSelectedSet() {
			new ItemIsSelectedSetTabControl();
		}

		class ItemIsSelectedSetTabControl : TabControl {
			public ItemIsSelectedSetTabControl() {
				Window w = new Window();
				w.Content = this;
				w.Show();
				TabItem t = new TabItem();
				t.Content = "Test";
				Items.Add(t);
				Assert.IsFalse(t.IsSelected, "1");
				Assert.AreEqual(SelectedIndex, -1, "1 2");
				OnApplyTemplate();
				Assert.IsFalse(t.IsSelected, "2");
				Assert.AreEqual(SelectedIndex, -1, "2 2");
			}
		}
		#endregion

		#region ItemIsSelectedSetItemContainerGeneratorSetStatus
		[Test]
		public void ItemIsSelectedSetItemContainerGeneratorSetStatus() {
			new ItemIsSelectedSetItemContainerGeneratorSetStatusTabControl();
		}

		class ItemIsSelectedSetItemContainerGeneratorSetStatusTabControl : ItemContainerGeneratorStatusChangedTabControl {
			bool called;

			public ItemIsSelectedSetItemContainerGeneratorSetStatusTabControl() {
				Window w = new Window();
				w.Content = this;
				w.Show();
				TabItem t = new TabItem();
				t.Content = "Test";
				ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
				Assert.AreEqual(GetHandlerCount(), 3, "1");
				Items.Add(t);
				Assert.IsTrue(called, "2");
			}

			void ItemContainerGenerator_StatusChanged(object sender, EventArgs e) {
				called = true;
			}
		}
		#endregion

		#region ItemIsSelectedSet2
		[Test]
		public void ItemIsSelectedSet2() {
			new ItemIsSelectedSet2TabControl();
		}

		class ItemIsSelectedSet2TabControl : TabControl {
			public ItemIsSelectedSet2TabControl() {
				TabItem t = new TabItem();
				t.Content = "Test";
				Items.Add(t);
				Window w = new Window();
				w.Content = this;
				w.Show();
				Assert.IsTrue(t.IsSelected);
			}
		}
		#endregion

		#region Style
		[Test]
		public void Style() {
			new StyleTabControl();
		}

		class StyleTabControl : TabControl {
			public StyleTabControl() {
				Assert.AreEqual(DefaultStyleKey, typeof(TabControl), "1");
				Assert.AreSame(DefaultStyleKeyProperty.GetMetadata(this), DefaultStyleKeyProperty.GetMetadata(typeof(TabControl)), "2");

			}
		}
		#endregion

		#region OnApplyTemplateAddsItemsToTabPanel
		[Test]
		public void OnApplyTemplateAddsItemsToTabPanel() {
			new OnApplyTemplateAddsItemsToTabPanelTabControl();
		}

		class OnApplyTemplateAddsItemsToTabPanelTabControl : TabControl {
			public OnApplyTemplateAddsItemsToTabPanelTabControl() {
				Window window = new Window();
				window.Content = this;
				window.Show();
				Items.Add(new TabItem());
				Assert.AreEqual(Items.Count, 1, "1");
				Assert.AreEqual(HeaderPanel.Children.Count, 1, "2");
			}

			TabPanel HeaderPanel {
				get {
					return (TabPanel)GetTemplateChild("HeaderPanel");
				}
			}
		}
		#endregion

		[Test]
		public void Focusable() {
			TabControl t = new TabControl();
			Assert.IsTrue(t.Focusable);
		}

		[Test]
		public void ItemsStoresThings() {
			TabControl t = new TabControl();
			t.Items.Add("Test");
			Assert.AreEqual(t.Items[0], "Test");
		}

		[Test]
		public void GeneratedTabItem() {
			TabControl t = new TabControl();
			t.Items.Add("Test");
			TabItem tab_item = (TabItem)t.ItemContainerGenerator.ContainerFromIndex(0);
			Assert.IsNull(tab_item);
		}

		[Test]
		public void TabStripPlacementPropertyChangedCallback() {
			Assert.IsNull(TabControl.TabStripPlacementProperty.DefaultMetadata.PropertyChangedCallback);
		}

		#region OnSelectionChanged
		[Test]
		public void OnSelectionChanged() {
			new OnSelectionChangedTabControl();
		}

		class OnSelectionChangedTabControl : TabControl {
			public OnSelectionChangedTabControl() {
				TabItem i1 = new TabItem();
				Items.Add(i1);
				TabItem i2 = new TabItem();
				Items.Add(i2);
				SelectedItem = i2;
				Assert.IsTrue(i2.IsSelected);
			}

			protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			}
		}
		#endregion

		[Test]
		public void IsSynchronizedWithCurrentItem() {
			Assert.IsNull(new TabControl().IsSynchronizedWithCurrentItem);
		}

		[Test]
		public void SelectedIndex() {
			PropertyMetadata default_metadata = Selector.SelectedIndexProperty.DefaultMetadata;
			PropertyMetadata selector_metadata = Selector.SelectedIndexProperty.GetMetadata(typeof(Selector));
			PropertyMetadata tab_control_metadata = Selector.SelectedIndexProperty.GetMetadata(typeof(TabControl));
			Assert.AreNotSame(default_metadata, tab_control_metadata, "1");
			Assert.AreSame(selector_metadata, tab_control_metadata, "2");
			Assert.AreEqual(tab_control_metadata.DefaultValue, -1, "3");
		}

		[Test]
		public void SelectedItem() {
			PropertyMetadata default_metadata = Selector.SelectedItemProperty.DefaultMetadata;
			PropertyMetadata selector_metadata = Selector.SelectedItemProperty.GetMetadata(typeof(Selector));
			PropertyMetadata tab_control_metadata = Selector.SelectedItemProperty.GetMetadata(typeof(TabControl));
			Assert.AreNotSame(default_metadata, tab_control_metadata, "1");
			Assert.AreSame(selector_metadata, tab_control_metadata, "2");
			Assert.AreEqual(tab_control_metadata.DefaultValue, null, "3");
		}

		[Test]
		public void SelectedValue() {
			PropertyMetadata default_metadata = Selector.SelectedValueProperty.DefaultMetadata;
			PropertyMetadata selector_metadata = Selector.SelectedValueProperty.GetMetadata(typeof(Selector));
			PropertyMetadata tab_control_metadata = Selector.SelectedValueProperty.GetMetadata(typeof(TabControl));
			Assert.AreNotSame(default_metadata, tab_control_metadata, "1");
			Assert.AreSame(selector_metadata, tab_control_metadata, "2");
			Assert.AreEqual(tab_control_metadata.DefaultValue, null, "3");
		}

		[Test]
		public void OnSelectionChangedSimplified() {
			TabControl tab_control = new TabControl();
			TabItem i1 = new TabItem();
			tab_control.Items.Add(i1);
			TabItem i2 = new TabItem();
			tab_control.Items.Add(i2);
			tab_control.SelectedItem = i2;
			Assert.IsTrue(i2.IsSelected);
		}

		#region ItemContainerGeneratorStatusChanged
		[Test]
		public void ItemContainerGeneratorStatusChanged() {
			ItemContainerGeneratorStatusChangedTabControl t = new ItemContainerGeneratorStatusChangedTabControl();
			Assert.AreEqual(t.GetHandlerCount(), 1, "1");
			Window w = new Window();
			w.Content = t;
			w.Show();
			Assert.AreEqual(t.GetHandlerCount(), 2, "2");
		}

		[Test]
		public void ItemContainerGeneratorStatusChanged2() {
			ItemContainerGeneratorStatusChangedTabControl t = new ItemContainerGeneratorStatusChangedTabControl();
			t.ApplyTemplate();
			Assert.AreEqual(t.GetHandlerCount(), 1);
		}

		[Test]
		public void ItemContainerGeneratorStatusChanged3() {
			new ItemContainerGeneratorStatusChangedTabControl3ItemContainerGeneratorStatusChangedTabControl();
		}

		class ItemContainerGeneratorStatusChangedTabControl3ItemContainerGeneratorStatusChangedTabControl : ItemContainerGeneratorStatusChangedTabControl {
			public ItemContainerGeneratorStatusChangedTabControl3ItemContainerGeneratorStatusChangedTabControl() {
				Window w = new Window();
				w.Content = this;
				w.Show();
			}

			public override void OnApplyTemplate() {
				Assert.AreEqual(GetHandlerCount(), 2, "1");
				Assert.AreEqual(GetItemsChangedHandlerCount(), 0, "1 1");
				base.OnApplyTemplate();
				Assert.AreEqual(GetHandlerCount(), 2, "2");
				Assert.AreEqual(GetItemsChangedHandlerCount(), 0, "2 1");
			}

			protected override void OnInitialized(EventArgs e) {
				Assert.AreEqual(GetHandlerCount(), 1, "3");
				Assert.AreEqual(GetItemsChangedHandlerCount(), 0, "3 1");
				base.OnInitialized(e);
				Assert.AreEqual(GetHandlerCount(), 2, "4");
				Assert.AreEqual(GetItemsChangedHandlerCount(), 0, "4 1");
			}
		}

		class ItemContainerGeneratorStatusChangedTabControl : TabControl {
			public int GetHandlerCount() {
				EventHandler handler = (EventHandler)typeof(ItemContainerGenerator).GetField("StatusChanged", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ItemContainerGenerator);
				if (handler == null)
					return 0;
				return handler.GetInvocationList().GetLength(0);
			}

			public int GetItemsChangedHandlerCount() {
				EventHandler handler = (EventHandler)typeof(ItemContainerGenerator).GetField("ItemsChanged", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ItemContainerGenerator);
				if (handler == null)
					return 0;
				return handler.GetInvocationList().GetLength(0);
			}
		}
		#endregion

		[Test]
		public void Theme() {
			TabControl t = new TabControl();
			Window w = new Window();
			w.Content = t;
			w.Show();
			Assert.IsNotNull(t.Template);
		}

		#region OnItemsChangedCalledAfterOnGeneratorStatusChanged
		[Test]
		public void OnItemsChangedCalledAfterOnGeneratorStatusChanged() {
			new OnItemsChangedCalledAfterOnGeneratorStatusChangedTabControl();
		}

		class OnItemsChangedCalledAfterOnGeneratorStatusChangedTabControl : TabControl {
			int order;

			public OnItemsChangedCalledAfterOnGeneratorStatusChangedTabControl() {
				Window w = new Window();
				w.Content = this;
				w.Show();
				Items.Add("1");
				Assert.AreEqual(order, 2, "3");
			}

			protected override void OnInitialized(EventArgs e) {
				base.OnInitialized(e);
				ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
			}

			void OnGeneratorStatusChanged(object sender, EventArgs e) {
				if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated && Items.Count == 1) {
					Assert.AreEqual(order, 0, "1");
					order++;
				}
			}

			protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
				base.OnItemsChanged(e);
				Assert.AreEqual(order, 1, "2");
				order++;
			}
		}
		#endregion

		#region OnGeneratorStatusChangedCalled
		[Test]
		public void OnGeneratorStatusChangedCalled() {
			new OnGeneratorStatusChangedCalledTabControl();
		}

		class OnGeneratorStatusChangedCalledTabControl : TabControl {
			int calls;

			public OnGeneratorStatusChangedCalledTabControl() {
				Assert.AreEqual(calls, 0, "1");
				Items.Add(new object());
				Assert.AreEqual(calls, 0, "2");
				ApplyTemplate();
				Assert.AreEqual(calls, 0, "3");
				Window w = new Window();
				w.Content = this;
				Assert.AreEqual(calls, 0, "4");
				w.Show();
				Assert.AreEqual(calls, 2, "5");
				Items.Add(new object());
				Assert.AreEqual(calls, 4, "6");
			}

			protected override void OnInitialized(EventArgs e) {
				base.OnInitialized(e);
				ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
			}

			void OnGeneratorStatusChanged(object sender, EventArgs e) {
				calls++;
			}
		}
		#endregion

		#region OnGeneratorStatusChangedCalled2
		[Test]
		public void OnGeneratorStatusChangedCalled2() {
			new OnGeneratorStatusChangedCalled2TabControl();
		}

		class OnGeneratorStatusChangedCalled2TabControl : TabControl {
			bool called;
			bool check_items;
			public OnGeneratorStatusChangedCalled2TabControl() {
				Window w = new Window();
				w.Content = this;
				w.Show();
				check_items = true;
				Items.Add(new object());
				called = true;
			}

			protected override void OnInitialized(EventArgs e) {
				base.OnInitialized(e);
				ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
			}

			void OnGeneratorStatusChanged(object sender, EventArgs e) {
				if (check_items)
					Assert.AreEqual(Items.Count, 1, "1");
				Assert.IsFalse(called, "2");
			}
		}
		#endregion

		#region OnItemsChangedCalled
		[Test]
		public void OnItemsChangedCalled() {
			new OnItemsChangedCalledTabControl();
		}

		class OnItemsChangedCalledTabControl : TabControl {
			int calls;

			public OnItemsChangedCalledTabControl() {
				Assert.AreEqual(calls, 0, "1");
				Items.Add(new object());
				Assert.AreEqual(calls, 1, "2");
				ApplyTemplate();
				Assert.AreEqual(calls, 1, "3");
				Window w = new Window();
				w.Content = this;
				Assert.AreEqual(calls, 1, "4");
				w.Show();
				Assert.AreEqual(calls, 1, "5");
				Items.Add(new object());
				Assert.AreEqual(calls, 2, "6");
			}

			protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
				base.OnItemsChanged(e);
				calls++;
			}
		}
		#endregion
	}
}