using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Markup;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[ContentProperty("Items")]
	[Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
	[StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(FrameworkElement))]
	public class ItemsControl : Control, IAddChild {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(ItemsControl), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty GroupStyleSelectorProperty = DependencyProperty.Register("GroupStyleSelector", typeof(GroupStyleSelector), typeof(ItemsControl), new FrameworkPropertyMetadata());
		public static readonly DependencyPropertyKey HasItemsPropertyKey = DependencyProperty.RegisterReadOnly("HasItems", typeof(bool), typeof(ItemsControl), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty HasItemsProperty = HasItemsPropertyKey.DependencyProperty;
		static readonly DependencyPropertyKey IsGroupingPropertyKey = DependencyProperty.RegisterReadOnly("IsGrouping", typeof(bool), typeof(ItemsControl), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty IsGroupingProperty = IsGroupingPropertyKey.DependencyProperty;
		public static readonly DependencyProperty IsTextSearchEnabledProperty = DependencyProperty.Register("IsTextSearchEnabled", typeof(bool), typeof(ItemsControl), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(ItemsControl), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty ItemContainerStyleSelectorProperty = DependencyProperty.Register("ItemContainerStyleSelector", typeof(StyleSelector), typeof(ItemsControl), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty ItemsPanelProperty = DependencyProperty.Register("ItemsPanel", typeof(ItemsPanelTemplate), typeof(ItemsControl), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ItemsControl), new FrameworkPropertyMetadata(delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ItemsControl)d).OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
		}));
		public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ItemsControl), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(ItemsControl), new FrameworkPropertyMetadata());
		#endregion
		#endregion

		#region Internal Fields
		internal ArrayList items_data = new ArrayList();
		#endregion

		#region Private Fields
		ItemContainerGenerator item_container_generator = new ItemContainerGenerator();
		ItemCollection items;
		#endregion

		#region Static Constructor
		static ItemsControl() {
#if Implementation
			Theme.Load();
#endif
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemsControl), new FrameworkPropertyMetadata(typeof(ItemsControl)));
		}
		#endregion

		#region Public Constructors
		public ItemsControl() {
			items = new ItemCollection(this);
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[Bindable(true)]
		public string DisplayMemberPath {
			get { return (string)GetValue(DisplayMemberPathProperty); }
			set { SetValue(DisplayMemberPathProperty, value); }
		}

		[Bindable(true)]
		public GroupStyleSelector GroupStyleSelector {
			get { return (GroupStyleSelector)GetValue(GroupStyleSelectorProperty); }
			set { SetValue(GroupStyleSelectorProperty, value); }
		}

		[Bindable(false)]
		public bool HasItems {
			get { return (bool)GetValue(HasItemsProperty); }
			private set { SetValue(HasItemsPropertyKey, value); }
		}

		[Bindable(false)]
		public bool IsGrouping {
			get { return (bool)GetValue(IsGroupingProperty); }
			private set { SetValue(IsGroupingPropertyKey, value); }
		}

		public bool IsTextSearchEnabled {
			get { return (bool)GetValue(IsTextSearchEnabledProperty); }
			set { SetValue(IsTextSearchEnabledProperty, value); }
		}

		[Bindable(true)]
		public Style ItemContainerStyle {
			get { return (Style)GetValue(ItemContainerStyleProperty); }
			set { SetValue(ItemContainerStyleProperty, value); }
		}

		[Bindable(true)]
		public StyleSelector ItemContainerStyleSelector {
			get { return (StyleSelector)GetValue(ItemContainerStyleSelectorProperty); }
			set { SetValue(ItemContainerStyleSelectorProperty, value); }
		}

		[Bindable(false)]
		public ItemsPanelTemplate ItemsPanel {
			get { return (ItemsPanelTemplate)GetValue(ItemsPanelProperty); }
			set { SetValue(ItemsPanelProperty, value); }
		}

		[Bindable(true)]
		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		[Bindable(true)]
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}

		[Bindable(true)]
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		#endregion
		public ObservableCollection<GroupStyle> GroupStyle { 
			get {
				//WDTDH
				return null;
			}
		}

		[Bindable(false)]
		public ItemContainerGenerator ItemContainerGenerator {
			get { return item_container_generator; }
		}

		[Bindable(true)]
		public ItemCollection Items {
			get { return items; }
		}
		#endregion

		#region Protected Properties
		protected override IEnumerator LogicalChildren {
			get {
				//WDTDH
				return base.LogicalChildren;
			}
		}
		#endregion

		#region Public Methods
		public override void BeginInit() {
			//WDTDH
			base.BeginInit();
		}

		public DependencyObject ContainerFromElement(DependencyObject element) {
			//WDTDH
			return null;
		}

		public static DependencyObject ContainerFromElement(ItemsControl itemsControl, DependencyObject element) {
			return itemsControl.ContainerFromElement(element);
		}

		public override void EndInit() {
			//WDTDH
			base.EndInit();
		}

		public static ItemsControl GetItemsOwner(DependencyObject element) {
			//WDTDH
			return null;
		}

		public static ItemsControl ItemsControlFromItemContainer(DependencyObject container) {
			//WDTDH
			return null;
		}

		public bool ShouldSerializeGroupStyle() {
			//WDTDH
			return false;
		}

		public bool ShouldSerializeItems() {
			//WDTDH
			return false;
		}

		public override string ToString() {
			//WDTDH
			return base.ToString();
		}
		#endregion

		#region Protected Methods
		protected virtual void AddChild(object value) {
			//WDTDH
		}

		protected virtual void AddText(string text) {
			//WDTDH
		}

		protected virtual void ClearContainerForItemOverride(DependencyObject element, object item) {
			//WDTDH
		}

		protected virtual DependencyObject GetContainerForItemOverride() {
			//WDTDH
			return null;
		}

		protected virtual bool IsItemItsOwnContainerOverride(object item) {
			//WDTDH
			return false;
		}

		protected virtual void OnDisplayMemberPathChanged(string oldDisplayMemberPath, string newDisplayMemberPath) {
			//WDTDH
		}

		protected virtual void OnGroupStyleSelectorChanged(GroupStyleSelector oldGroupStyleSelector, GroupStyleSelector newGroupStyleSelector) {
			//WDTDH
		}

		protected virtual void OnItemContainerStyleChanged(Style oldItemContainerStyle, Style newItemContainerStyle) {
			//WDTDH
		}

		protected virtual void OnItemContainerStyleSelectorChanged(StyleSelector oldItemContainerStyleSelector, StyleSelector newItemContainerStyleSelector) {
			//WDTDH
		}

		protected virtual void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			//WDTDH
		}

		protected virtual void OnItemsPanelChanged(ItemsPanelTemplate oldItemsPanel, ItemsPanelTemplate newItemsPanel) {
			//WDTDH
		}

		protected virtual void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) {
			items.SetItemsSource(newValue);
		}

		protected virtual void OnItemTemplateChanged(DataTemplate oldItemTemplate, DataTemplate newItemTemplate) {
			//WDTDH
		}

		protected virtual void OnItemTemplateSelectorChanged(DataTemplateSelector oldItemTemplateSelector, DataTemplateSelector newItemTemplateSelector) {
			//WDTDH
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			//WDTDH
		}

		protected override void OnTextInput(TextCompositionEventArgs e) {
			base.OnTextInput(e);
			//WDTDH
		}

		protected virtual void PrepareContainerForItemOverride(DependencyObject element, object item) {
			//WDTDH
		}

		protected virtual bool ShouldApplyItemContainerStyle(DependencyObject container, object item) {
			//WDTDH
			return false;
		}
		#endregion

		#region Explicit Interface Implementations
		#region IAddChild
		void IAddChild.AddChild(object value) {
			AddChild(value);
		}

		void IAddChild.AddText(string text) {
			AddText(text);
		}
		#endregion
		#endregion
	}
}