using Mono.WindowsPresentationFoundation;
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
	partial class TabItem {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(TabItem), new FrameworkPropertyMetadata(delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TabItem i = (TabItem)d;
			if ((bool)e.NewValue) {
				TabControl tab_control = i.GetTabControl();
				i.OnSelected(new RoutedEventArgs(global::System.Windows.Controls.Primitives.Selector.SelectedEvent, i));
				if (tab_control != null) {
					tab_control.ExecuteStrangeCaseSelectFirstItemInWeirdConditions();
					tab_control.SetSelectedProperties();
				}
			} else {
				TabControl tab_control = i.GetTabControl();
				if (tab_control != null) {
					if (tab_control.SelectedItem == tab_control.GetItemForTabItem(i))
						tab_control.SelectedItem = null;
				}
				i.OnUnselected(new RoutedEventArgs(global::System.Windows.Controls.Primitives.Selector.UnselectedEvent, i));
			}
			#region Invalidate TabPanel arrange
			TabPanel tab_panel = i.VisualParent as TabPanel;
			if (tab_panel != null)
				tab_panel.InvalidateArrange();
			#endregion
		}));
		static readonly DependencyPropertyKey TabStripPlacementPropertyKey = DependencyProperty.RegisterReadOnly("TabStripPlacement", typeof(Dock), typeof(TabItem), new FrameworkPropertyMetadata(Dock.Top));
		public static readonly DependencyProperty TabStripPlacementProperty = TabStripPlacementPropertyKey.DependencyProperty; 
		#endregion
		#endregion

		#region Public Constructors
		public TabItem() {
			//TabControl.AddSelectedHandler(this, delegate(object sender, RoutedEventArgs e) {
			//    IsSelected = true;
			//});
		}
		#endregion

		#region Protected Methods
		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);
			//WDTDH
		}

		protected override void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate) {
			base.OnContentTemplateChanged(oldContentTemplate, newContentTemplate);
			//WDTDH
		}

		protected override void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector) {
			base.OnContentTemplateSelectorChanged(oldContentTemplateSelector, newContentTemplateSelector);
			//WDTDH
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// Disabling this causes Ctrl+Tab not to work.
		/// </remarks>
		protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnPreviewGotKeyboardFocus(e);
			//WDTDH
		}

		protected virtual void OnSelected(RoutedEventArgs e) {
			TabControl tab_control = GetTabControl();
			if (tab_control == null)
				return;
			int index_of_current_instance = tab_control.ItemContainerGenerator.IndexFromContainer(this);
			if (index_of_current_instance != -1) {
				for (int item_index = 0; item_index < tab_control.Items.Count; item_index++) {
					TabItem tab_item = (TabItem)tab_control.ItemContainerGenerator.ContainerFromIndex(item_index);
					if (tab_item != null && tab_item != this)
						tab_item.IsSelected = false;
				}
				tab_control.SelectedItem = tab_control.Items[index_of_current_instance];
			} else {
				foreach (object item in tab_control.Items) {
					TabItem tab_item = item as TabItem;
					if (tab_item != null && tab_item != this)
						tab_item.IsSelected = false;
				}
				tab_control.SelectedItem = this;
			}
		}

		protected virtual void OnUnselected(RoutedEventArgs e) {
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Does whatever it is supposed to do when the TabItem is selected through the UI.
		/// </summary>
		void HandleUIAction() {
			TabControl tab_control = GetTabControl();
			if (tab_control != null && Utility.IsInVisibleWindow(tab_control))
				IsSelected = true;
		}
		#endregion
	}
}