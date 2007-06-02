using System;
using System.Collections.Specialized;
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
		public static readonly DependencyProperty TabStripPlacementProperty = DependencyProperty.Register("TabStripPlacement", typeof(Dock), typeof(TabControl));
		#endregion
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
			return base.GetContainerForItemOverride();
		}

		protected override bool IsItemItsOwnContainerOverride(object item) {
			return base.IsItemItsOwnContainerOverride(item);
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
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
		}

		protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			base.OnSelectionChanged(e);
		}
		#endregion
	}
}