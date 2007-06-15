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
		public static readonly DependencyProperty TabStripPlacementProperty = DependencyProperty.Register("TabStripPlacement", typeof(Dock), typeof(TabControl), new FrameworkPropertyMetadata(Dock.Top));
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
			ItemContainerGenerator.StatusChanged += delegate(object sender, EventArgs e) {
				EnsureATabItemIsSelected();
			};
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
			if (
				(e.Key == Key.Tab && e.KeyboardDevice.Modifiers == ModifierKeys.Control) ||
				(e.Key == Key.Tab && e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift)) ||
				(e.Key == Key.Home && e.KeyboardDevice.Modifiers == ModifierKeys.None) ||
				(e.Key == Key.End && e.KeyboardDevice.Modifiers == ModifierKeys.None))
				e.Handled = true;
			base.OnKeyDown(e);
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
			TabItem selectedTabItem = GetTabItemForItem(SelectedItem);
			SelectedContent = selectedTabItem == null ? null : selectedTabItem.Content;
			SelectedContentTemplate = selectedTabItem == null ? ContentTemplate : selectedTabItem.ContentTemplate ?? ContentTemplate;
			SelectedContentTemplateSelector = selectedTabItem == null ? ContentTemplateSelector : selectedTabItem.ContentTemplateSelector ?? ContentTemplateSelector;
		}
		#endregion

		#region Private Methods
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
			return (TabItem)ItemContainerGenerator.ContainerFromItem(item);
		}

		TabItem GetTabItemForItemAtIndex(int index) {
			return (TabItem)ItemContainerGenerator.ContainerFromIndex(index);
		}
		#endregion
	}
}