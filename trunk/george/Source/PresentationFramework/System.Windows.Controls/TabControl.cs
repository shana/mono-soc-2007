using Mono.WindowsPresentationFoundation;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Automation.Peers;
using System.Windows.Input;
#if Implementation
using System.Windows;
using System.Windows.Controls;
using Mono.System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls {
#else
using System.Windows.Controls.Primitives;
namespace System.Windows.Controls {
#endif
	[StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(TabItem))]
	[TemplatePart(Name = "PART_SelectedContentHost", Type = typeof(ContentPresenter))]
	public class TabControl : global::System.Windows.Controls.Primitives.Selector {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(TabControl), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(TabControl), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty SelectedContentProperty = DependencyProperty.Register("SelectedContent", typeof(object), typeof(TabControl), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty SelectedContentTemplateProperty = DependencyProperty.Register("SelectedContentTemplate", typeof(DataTemplate), typeof(TabControl), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty SelectedContentTemplateSelectorProperty = DependencyProperty.Register("SelectedContentTemplateSelector", typeof(DataTemplateSelector), typeof(TabControl), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty TabStripPlacementProperty = DependencyProperty.Register("TabStripPlacement", typeof(Dock), typeof(TabControl), new FrameworkPropertyMetadata(Dock.Top, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TabControl)d).UpdateTabStripPlacement();
		}));
		#endregion
		#endregion

		#region Static Constructor
		static TabControl() {
#if Implementation
			Theme.Load();
#endif
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControl), new FrameworkPropertyMetadata(typeof(TabControl)));
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
			ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
		}

		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			EnsureATabItemIsSelected();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// This is not where selecting the tab in response to the keys happens.
		/// </remarks>
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if (Items.Count < 2)
				return;
			if (e.Key == Key.Tab && e.KeyboardDevice.Modifiers == ModifierKeys.Control) {
				SelectedIndex = SelectedIndex == Items.Count - 1 ? 0 : SelectedIndex + 1;
				e.Handled = true;
			} else if (e.Key == Key.Tab && e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift)) {
				SelectedIndex = SelectedIndex == 0 ? Items.Count - 1 : SelectedIndex - 1;
				e.Handled = true;
			} else if (e.Key == Key.Home && e.KeyboardDevice.Modifiers == ModifierKeys.None) {
				SelectedIndex = 0;
				e.Handled = true;
			} else if (e.Key == Key.End && e.KeyboardDevice.Modifiers == ModifierKeys.None) {
				SelectedIndex = Items.Count - 1;
				e.Handled = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// If the user clicks a tab, this method is called. The content area is updated here.
		/// </remarks>
		protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			base.OnSelectionChanged(e);
			TabItem selected_tab_item = GetTabItemForItem(SelectedItem);
			//FIXME: I should not do this here.
			if (selected_tab_item != null)
				selected_tab_item.IsSelected = true;
			SelectedContent = selected_tab_item == null ? null : selected_tab_item.Content;
			SelectedContentTemplate = selected_tab_item == null ? ContentTemplate : selected_tab_item.ContentTemplate ?? ContentTemplate;
			SelectedContentTemplateSelector = selected_tab_item == null ? ContentTemplateSelector : selected_tab_item.ContentTemplateSelector ?? ContentTemplateSelector;
		}
		#endregion

		#region Internal Methods
		internal object GetItemForTabItem(TabItem tab_item) {
			object item_from_container = ItemContainerGenerator.ItemFromContainer(tab_item);
			if (item_from_container == DependencyProperty.UnsetValue)
				return tab_item;
			else
				return item_from_container;
		}
		#endregion

		#region Private Methods
		void OnGeneratorStatusChanged(object sender, EventArgs e) {
			EnsureATabItemIsSelected();
			UpdateTabStripPlacement();
		}

		void EnsureATabItemIsSelected() {
			if (ItemContainerGenerator.Status != global::System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
				return;
			if (Items.Count == 0)
				return;
			for (int item_index = 0; item_index < Items.Count; item_index++)
				if (GetTabItemForItemAtIndex(item_index).IsSelected)
					return;
			GetTabItemForItemAtIndex(0).IsSelected = true;
		}

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