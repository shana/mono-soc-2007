using NUnit.Framework;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class ButtonTest {
		[Test]
		public void StaticProperties() {
			Assert.AreEqual(Button.IsCancelProperty.Name, "IsCancel", "IsCancelProperty.Name");
			Assert.AreEqual(Button.IsCancelProperty.OwnerType, typeof(Button), "IsCancelProperty.OwnerType");
			Assert.AreEqual(Button.IsCancelProperty.PropertyType, typeof(bool), "IsCancelProperty.PropertyType");

			Assert.AreEqual(Button.IsDefaultedProperty.Name, "IsDefaulted", "IsDefaultedProperty.Name");
			Assert.AreEqual(Button.IsDefaultedProperty.OwnerType, typeof(Button), "IsDefaultedProperty.OwnerType");
			Assert.AreEqual(Button.IsDefaultedProperty.PropertyType, typeof(bool), "IsDefaultedProperty.PropertyType");
			Assert.AreEqual(Button.IsDefaultedProperty.ReadOnly, true, "IsDefaultedProperty.ReadOnly");

			Assert.AreEqual(Button.IsDefaultProperty.Name, "IsDefault", "IsDefaultProperty.Name");
			Assert.AreEqual(Button.IsDefaultProperty.OwnerType, typeof(Button), "IsDefaultProperty.OwnerType");
			Assert.AreEqual(Button.IsDefaultProperty.PropertyType, typeof(bool), "IsDefaultProperty.PropertyType");
		}

		[Test]
		public void Creation() {
			Button button = new Button();
			Assert.IsFalse(button.IsCancel, "button.IsCancel");
			Assert.IsFalse(button.IsDefaulted, "button.IsDefaulted");
			Assert.IsFalse(button.IsDefault, "button.IsDefault");
		}

#if !Implementation
		[Test]
		public void AutomationPeer() {
			new AutomationPeerButton();
		}
		class AutomationPeerButton : Button {
			public AutomationPeerButton() {
				AutomationPeer automationPeer = OnCreateAutomationPeer();
				Assert.IsInstanceOfType(typeof(ButtonAutomationPeer), automationPeer, "automationPeer Type");
			}
		}
#endif

		#region Focus
		[Test]
		public void Focus() {
			new FocusButton();
		}

		class FocusButton : Button {
			public FocusButton() {
				Focus();
				OnClick();
				Assert.IsTrue(IsFocused);
			}
		}
		#endregion

		#region FocusInToolBar
		[Test]
		public void FocusInToolBar() {
			new FocusInToolBarButton();
		}

		class FocusInToolBarButton : Button {
			public FocusInToolBarButton() {
				ToolBar t = new ToolBar();
				t.Items.Add(this);
				Assert.IsTrue(IsTabStop, "IsTabStop");
				Focus();
				OnClick();
				Assert.IsTrue(IsFocused, "IsFocused");
			}
		}
		#endregion

		#region FocusInToolBarInWindow
		[Test]
		public void FocusInToolBarInWindow() {
			new FocusInToolBarInWindowButton();
		}

		class FocusInToolBarInWindowButton : Button {
			public FocusInToolBarInWindowButton() {
				Window w = new Window();
				ToolBar t = new ToolBar();
				t.Items.Add(this);
				w.Content = t;
				w.Show();
				Focus();
				OnClick();
				Assert.IsTrue(IsFocused);
			}
		}
		#endregion

		#region FocusInToolBarInWindowMouseLeftButton
		[Test]
		public void FocusInToolBarInWindowMouseLeftButton() {
			new FocusInToolBarInWindowMouseLeftButtonButton();
		}

		class FocusInToolBarInWindowMouseLeftButtonButton : Button {
			public FocusInToolBarInWindowMouseLeftButtonButton() {
				Window w = new Window();
				ToolBar t = new ToolBar();
				t.Items.Add(this);
				w.Content = t;
				w.Show();
				Focus();
				MouseButtonEventArgs e = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left);
				e.RoutedEvent = MouseLeftButtonDownEvent;
				OnMouseLeftButtonDown(e);
				e.RoutedEvent = MouseLeftButtonUpEvent;
				OnMouseLeftButtonUp(e);
				Assert.IsTrue(IsFocused);
			}
		}
		#endregion

		#region DockPanel
		[Test]
		public void DockPanel() {
			new DockPanelButton();
		}

		class DockPanelButton : Button {
			public DockPanelButton() {
				Window w = new Window();
				DockPanel p = new DockPanel();
				w.Content = p;
				p.Children.Add(this);
				w.Show();
				Assert.AreEqual(result.Width, Utility.GetEmptyButtonSize());
			}
			Size result;
			protected override Size MeasureOverride(Size constraint) {
				return result = base.MeasureOverride(constraint);
			}
		}
		#endregion
	}
}