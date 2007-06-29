using Mono.WindowsPresentationFoundation;
using System.Collections.Specialized;
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
	partial class TabControl {
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

		#region Public Constructors
		public TabControl() {
		}
		#endregion

		#region Public Methods
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (Items.Count != 0)
				SelectedIndex = 0;
		}
		#endregion

		#region Protected Methods
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
		}

		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			//if (IsInitialized)
			//    return;
			if (ItemContainerGenerator.Status != global::System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
				return;
			if (Items.Count == 0)
				return;
			for (int item_index = 0; item_index < Items.Count; item_index++)
				if (GetTabItemForItemAtIndex(item_index).IsSelected)
					return;
			GetTabItemForItemAtIndex(0).IsSelected = true;
			// This needs to be done now but even if the user overrides this and does not call the base implementation.
			ExecuteStrangeCaseSelectFirstItemInWeirdConditions();
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
			SetSelectedProperties();
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
		
		internal void SetSelectedProperties() {
			TabItem selected_tab_item = GetTabItemForItem(SelectedItem);
			//FIXME: I should not do this here.
			if (selected_tab_item != null)
				selected_tab_item.IsSelected = true;
			SelectedContent = selected_tab_item == null ? null : selected_tab_item.Content;
			SelectedContentTemplate = selected_tab_item == null ? ContentTemplate : selected_tab_item.ContentTemplate ?? ContentTemplate;
			SelectedContentTemplateSelector = selected_tab_item == null ? ContentTemplateSelector : selected_tab_item.ContentTemplateSelector ?? ContentTemplateSelector;
		}

		internal void ExecuteStrangeCaseSelectFirstItemInWeirdConditions() {
			if (SelectedIndex == -1 && Utility.IsInVisibleWindow(this) && Items.Count > 1)
				SelectedIndex = 0;
		}
		#endregion

		#region Private Methods
		void OnGeneratorStatusChanged(object sender, EventArgs e) {
			//if (IsInitialized)
			//    return;
			if (ItemContainerGenerator.Status != global::System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
				return;
			if (Items.Count == 0)
				return;
			for (int item_index = 0; item_index < Items.Count; item_index++)
				if (GetTabItemForItemAtIndex(item_index).IsSelected)
					return;
			GetTabItemForItemAtIndex(0).IsSelected = true;
			UpdateTabStripPlacement();
		}
		#endregion
	}
}