using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	[Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
	public abstract class Selector : ItemsControl {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty = DependencyProperty.Register("IsSynchronizedWithCurrentItem", typeof(bool?), typeof(Selector), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Selector), new FrameworkPropertyMetadata(-1));
		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(Selector), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty SelectedValuePathProperty = DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(Selector), new FrameworkPropertyMetadata(string.Empty));
		public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(object), typeof(Selector), new FrameworkPropertyMetadata());

		#region Attached Properties
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(Selector), new FrameworkPropertyMetadata());
		static readonly DependencyPropertyKey IsSelectionActivePropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsSelectionActive", typeof(bool), typeof(Selector), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty IsSelectionActiveProperty = IsSelectionActivePropertyKey.DependencyProperty;
		#endregion
		#endregion

		#region Routed Events
		public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Selector));
		public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(Selector));
		public static readonly RoutedEvent UnselectedEvent = EventManager.RegisterRoutedEvent("Unselected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Selector));
		#endregion
		#endregion

		#region Protected Constructors
		protected Selector() {
		}
		#endregion

		#region Public Properties
		[Localizability(LocalizationCategory.NeverLocalize)]
		[Bindable(true)]
		[TypeConverter("System.Windows.NullableBoolConverter, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, Custom=null")]
		public bool? IsSynchronizedWithCurrentItem {
			get { return (bool?)GetValue(IsSynchronizedWithCurrentItemProperty); }
			set { SetValue(IsSynchronizedWithCurrentItemProperty, value); }
		}

		[Bindable(true)]
		[Localizability(LocalizationCategory.NeverLocalize)]
		public int SelectedIndex {
			get { return (int)GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}

		[Bindable(true)]
		public object SelectedItem {
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		[Localizability(LocalizationCategory.NeverLocalize)]
		[Bindable(true)]
		public object SelectedValue {
			get { return GetValue(SelectedValueProperty); }
			set { SetValue(SelectedValueProperty, value); }
		}

		[Bindable(true)]
		[Localizability(LocalizationCategory.NeverLocalize)]
		public string SelectedValuePath {
			get { return (string)GetValue(SelectedValuePathProperty); }
			set { SetValue(SelectedValuePathProperty, value); }
		}
		#endregion

		#region Public Methods
		#region Attached Properties
		[AttachedPropertyBrowsableForChildren]
		public static bool GetIsSelected(DependencyObject element) {
			return (bool)element.GetValue(IsSelectedProperty);
		}

		public static bool GetIsSelectionActive(DependencyObject element) {
			return (bool)element.GetValue(IsSelectionActiveProperty);
		}

		public static void SetIsSelected(DependencyObject element, bool isSelected) {
			element.SetValue(IsSelectedProperty, isSelected);
		}
		#endregion

		#region Attached Events
		public static void AddSelectedHandler(DependencyObject element, RoutedEventHandler handler) {
			((UIElement)element).AddHandler(SelectedEvent, handler);
		}

		public static void AddUnselectedHandler(DependencyObject element, RoutedEventHandler handler) {
			((UIElement)element).AddHandler(UnselectedEvent, handler);
		}

		public static void RemoveSelectedHandler(DependencyObject element, RoutedEventHandler handler) {
			((UIElement)element).RemoveHandler(SelectedEvent, handler);
		}

		public static void RemoveUnselectedHandler(DependencyObject element, RoutedEventHandler handler) {
			((UIElement)element).RemoveHandler(UnselectedEvent, handler);
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
		}

		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
		}

		protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) {
			base.OnItemsSourceChanged(oldValue, newValue);
		}

		protected virtual void OnSelectionChanged(SelectionChangedEventArgs e) {
		}
		#endregion
	}
}