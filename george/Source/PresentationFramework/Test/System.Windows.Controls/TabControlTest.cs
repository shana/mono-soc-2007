using NUnit.Framework;
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

		#region Items
		[Test]
		public void Items() {
			new ItemsTabControl();
		}

		class ItemsTabControl : TabControl{
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
	}
}