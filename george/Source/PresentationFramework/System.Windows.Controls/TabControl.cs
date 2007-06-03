// It does not work. Maybe some of the logic is in TabItem, TabPanel, etc.
using Mono.WindowsPresentationFoundation;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[StyleTypedProperty(Property="ItemContainerStyle", StyleTargetType=typeof(TabItem))]
	[TemplatePart(Name="PART_SelectedContentHost", Type=typeof(ContentPresenter))]
	public class TabControl : global::System.Windows.Controls.Primitives.Selector {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(TabControl));
		public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(TabControl));
		public static readonly DependencyProperty SelectedContentProperty = DependencyProperty.Register("SelectedContent", typeof(object), typeof(TabControl));
		public static readonly DependencyProperty SelectedContentTemplateProperty = DependencyProperty.Register("SelectedContentTemplate", typeof(DataTemplate), typeof(TabControl));
		public static readonly DependencyProperty SelectedContentTemplateSelectorProperty = DependencyProperty.Register("SelectedContentTemplateSelector", typeof(DataTemplateSelector), typeof(TabControl));
		public static readonly DependencyProperty TabStripPlacementProperty = DependencyProperty.Register("TabStripPlacement", typeof(Dock), typeof(TabControl), new PropertyMetadata(Dock.Top));
		#endregion
		#endregion

		#region Static Constructor
		static TabControl() {
			Theme.Load();
		}
		#endregion

		#region Public Constructors
		public TabControl() {
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

		#region Public Methods
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			//ContentPresenter selected_content_host = GetTemplateChild("PART_SelectedContentHost") as ContentPresenter;
			//if (selected_content_host == null)
			//    return;
			//Utility.SetBinding(selected_content_host, ContentPresenter.ContentTemplateProperty, this, "ContentTemplate");
			//Utility.SetBinding(selected_content_host, ContentPresenter.ContentTemplateSelectorProperty, this, "ContentTemplateSelector");

			if (SelectedItem != null)
				SetSelectedPropertiesValue((TabItem)SelectedItem);
		}
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

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			if (SelectedItem != null)
				SetSelectedPropertiesValue((TabItem)SelectedItem);
		}

		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			if (e.NewItems.Count >= 1) {
				TabItem tab_item = e.NewItems[0] as TabItem;
				if (tab_item != null)
					;
			}
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
		}

		protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			base.OnSelectionChanged(e);
			//if (Parent == null)
			//	return;
			if (e.AddedItems.Count == 0)
				return;
			TabItem tab_item = e.AddedItems[0] as TabItem;
			if (tab_item == null)
				return;
			SetSelectedPropertiesValue(tab_item);
		}
		#endregion

		#region Private Methods
		void SetSelectedPropertiesValue(TabItem selectedTabItem) {
			SelectedContent = selectedTabItem.Content;
			SelectedContentTemplate = selectedTabItem.ContentTemplate ?? ContentTemplate;
			SelectedContentTemplateSelector = selectedTabItem.ContentTemplateSelector ?? ContentTemplateSelector;
		}
		#endregion
	}
}