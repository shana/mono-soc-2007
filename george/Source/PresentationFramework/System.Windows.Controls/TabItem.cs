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
	public class TabItem : HeaderedContentControl {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(TabItem), new FrameworkPropertyMetadata(delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TabItem i = (TabItem)d;
			if ((bool)e.NewValue)
				i.OnSelected(new RoutedEventArgs(SelectedEvent, i));
			else {
				TabControl tab_control = i.GetTabControl();
				if (tab_control != null) {
					if (tab_control.SelectedItem == tab_control.GetItemForTabItem(i))
						tab_control.SelectedItem = null;
				}
				i.OnUnselected(new RoutedEventArgs(UnselectedEvent, i));
			}
			TabPanel tab_panel = i.VisualParent as TabPanel;
			if (tab_panel != null)
				tab_panel.InvalidateArrange();
		}));
		static readonly DependencyPropertyKey TabStripPlacementPropertyKey = DependencyProperty.RegisterReadOnly("TabStripPlacement", typeof(Dock), typeof(TabItem), new FrameworkPropertyMetadata(Dock.Top));
		public static readonly DependencyProperty TabStripPlacementProperty = TabStripPlacementPropertyKey.DependencyProperty; 
		#endregion
		#endregion

		#region Private Fields
		#region Routed Events
		static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TabItem));
		static readonly RoutedEvent UnselectedEvent = EventManager.RegisterRoutedEvent("Unselected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TabItem));
		#endregion
		#endregion

		#region Static Constructor
		static TabItem() {
#if Implementation
			Theme.Load();
#endif
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TabItem), new FrameworkPropertyMetadata(typeof(TabItem)));
		}
		#endregion

		#region Public Constructors
		public TabItem() {
			//TabControl.AddSelectedHandler(this, delegate(object sender, RoutedEventArgs e) {
			//    IsSelected = true;
			//});
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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// Disabling this causes the access keys not to work.
		/// </remarks>
		protected override void OnAccessKey(AccessKeyEventArgs e) {
			base.OnAccessKey(e);
			SelectIfInTabControl();
		}

		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);
		}

		protected override void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate) {
			base.OnContentTemplateChanged(oldContentTemplate, newContentTemplate);
		}

		protected override void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector) {
			base.OnContentTemplateSelectorChanged(oldContentTemplateSelector, newContentTemplateSelector);
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
			SelectIfInTabControl();
			e.Handled = true;
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
		TabControl GetTabControl() {
			return ItemsControl.ItemsControlFromItemContainer(this) as TabControl;
		}

		void SelectIfInTabControl() {
			TabControl tab_control = GetTabControl();
			if (tab_control != null && tab_control.Parent != null)
				IsSelected = true;
		}
		#endregion
	}
}