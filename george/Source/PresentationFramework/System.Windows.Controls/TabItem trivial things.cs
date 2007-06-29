using System.ComponentModel;
using System.Windows.Automation.Peers;
using System.Windows.Input;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public partial class TabItem : HeaderedContentControl {
		#region Static Constructor
		static TabItem() {
#if Implementation
			Theme.Load();
#endif
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TabItem), new FrameworkPropertyMetadata(typeof(TabItem)));
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[Bindable(true)]
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		public Dock TabStripPlacement {
			get { return (Dock)GetValue(TabStripPlacementProperty); }
			internal set { SetValue(TabStripPlacementPropertyKey, value); }
		}
		#endregion
		#endregion
		
		#region Protected Methods
		protected override void OnAccessKey(AccessKeyEventArgs e) {
			base.OnAccessKey(e);
			HandleUIAction();
		}

		protected override AutomationPeer OnCreateAutomationPeer() {
#if Implementation
			return null;
#else
			return new TabItemAutomationPeer(this);
#endif
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			HandleUIAction();
			e.Handled = true;
		}
		#endregion

		#region Private Methods
		TabControl GetTabControl() {
			return ItemsControl.ItemsControlFromItemContainer(this) as TabControl;
		}
		#endregion
	}
}