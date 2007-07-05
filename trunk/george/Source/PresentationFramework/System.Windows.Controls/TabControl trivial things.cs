using System.ComponentModel;
using System.Windows.Automation.Peers;
#if Implementation
using System.Windows;
using System.Windows.Controls;
using Mono.System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(TabItem))]
	[TemplatePart(Name = "PART_SelectedContentHost", Type = typeof(ContentPresenter))]
	public partial class TabControl : Selector {
		#region Static Constructor
		static TabControl() {
#if Implementation
			Theme.Load();
#endif
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControl), new FrameworkPropertyMetadata(typeof(TabControl)));
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}

		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}

		public object SelectedContent {
			get { return GetValue(SelectedContentProperty); }
			set { SetValue(SelectedContentProperty, value); }
		}

		public DataTemplate SelectedContentTemplate {
			get { return (DataTemplate)GetValue(SelectedContentTemplateProperty); }
			set { SetValue(SelectedContentTemplateProperty, value); }
		}

		public DataTemplateSelector SelectedContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(SelectedContentTemplateSelectorProperty); }
			set { SetValue(SelectedContentTemplateSelectorProperty, value); }
		}

		[Bindable(true)]
		public Dock TabStripPlacement {
			get { return (Dock)GetValue(TabStripPlacementProperty); }
			set { SetValue(TabStripPlacementProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override DependencyObject GetContainerForItemOverride() {
			return new TabItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is TabItem;
		}

		protected override AutomationPeer OnCreateAutomationPeer() {
#if Implementation
			return null;
#else
			return new TabControlAutomationPeer(this);
#endif
		}
		#endregion

		#region Private Methods
		TabItem GetTabItemForItem(object item) {
			TabItem tab_item = item as TabItem;
			if (tab_item != null)
				return tab_item;
			return (TabItem)ItemContainerGenerator.ContainerFromItem(item);
		}

		TabItem GetTabItemForItemAtIndex(int index) {
			return (TabItem)ItemContainerGenerator.ContainerFromIndex(index);
		}
		
		void UpdateTabStripPlacement() {
			for (int item_index = 0; item_index < Items.Count; item_index++) {
				TabItem tab_item = GetTabItemForItemAtIndex(item_index);
				if (tab_item != null)
					tab_item.TabStripPlacement = TabStripPlacement;
			}
		}
		#endregion
	}
}