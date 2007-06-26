using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Markup;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[Localizability(LocalizationCategory.Ignore)]
	[ContentProperty("Children")]
	public abstract class Panel : FrameworkElement, IAddChild {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(Panel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty IsItemsHostProperty = DependencyProperty.Register("IsItemsHost", typeof(bool), typeof(Panel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.NotDataBindable, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Panel i = (Panel)d;
			i.OnIsItemsHostChanged((bool)e.OldValue, (bool)e.NewValue);
			List<UIElement> current_children = new List<UIElement>();
			foreach (UIElement child in i.children)
				current_children.Add(child);
			i.children.Clear();
			if (i.children_for_the_other_is_items_host_value != null)
				foreach (UIElement child in i.children_for_the_other_is_items_host_value)
					i.children.Add(child);
			i.children_for_the_other_is_items_host_value = current_children;
		}));
		#region Attached Properties
		public static readonly DependencyProperty ZIndexProperty = DependencyProperty.RegisterAttached("ZIndex", typeof(int), typeof(Panel), new FrameworkPropertyMetadata());
		#endregion
		#endregion
		#endregion

		#region Private Fields
		UIElementCollection children;
		List<UIElement> children_for_the_other_is_items_host_value;
		#endregion

		#region Protected Constructors
		protected Panel() {
			children = CreateUIElementCollection(this);
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public Brush Background {
			get { return (Brush)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}
		
		[Bindable(false)]
		public bool IsItemsHost {
			get { return (bool)GetValue(IsItemsHostProperty); }
			set { SetValue(IsItemsHostProperty, value); }
		}
		#endregion

		public UIElementCollection Children {
			get {
				if (IsItemsHost) {
					ItemsControl items_control = TemplatedParent as ItemsControl;
					if (items_control == null)
						throw new InvalidOperationException("A panel with IsItemsHost=\"true\" is not nested in an ItemsControl. Panel must be nested in ItemsControl to get and show items.");
				}
				return children; 
			}
		}
		#endregion

		#region Protected Properties
		protected override int VisualChildrenCount {
			get { return children.Count; }
		}
		#endregion

		#region Protected Internal Properties
		protected internal virtual bool HasLogicalOrientation {
			get { return false; }
		}

		protected internal UIElementCollection InternalChildren {
			get { return Children; }
		}

		protected override IEnumerator LogicalChildren {
			get { return Children.GetEnumerator(); }
		}

		protected internal virtual Orientation LogicalOrientation {
			get { return Orientation.Vertical; }
		}
		#endregion

		#region Public Methods
		#region Attached Properties
		public static int GetZIndex(UIElement element) {
			return (int)element.GetValue(ZIndexProperty);
		}

		public static void SetZIndex(UIElement element, int value) {
			element.SetValue(ZIndexProperty, value);
		}
		#endregion

		public bool ShouldSerializeChildren() {
			return Children != null && Children.Count > 0;
		}
		#endregion

		#region Protected Methods
		protected virtual UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent) {
			return new UIElementCollection(this, logicalParent);
		}

		protected override Visual GetVisualChild(int index) {
			if (index < 0 || index >= VisualChildrenCount)
				throw new ArgumentOutOfRangeException("index");
			SortedDictionary<int, List<UIElement>> groups = new SortedDictionary<int, List<UIElement>>();
			foreach (UIElement child in Children) {
				int z_index = GetZIndex(child);
				List<UIElement> group;
				if (!groups.TryGetValue(z_index, out group)) {
					group = new List<UIElement>();
					groups.Add(z_index, group);
				}
				group.Add(child);
			}
			int children_passed_so_far = 0;
			foreach (List<UIElement> group in groups.Values) {
				int possible_index = index - children_passed_so_far;
				if (possible_index < group.Count)
					return group[possible_index];
				else
					children_passed_so_far += group.Count;
			}
			Debug.Fail("This should never be reached.");
			return null;
		}

		protected virtual void OnIsItemsHostChanged(bool oldIsItemsHost, bool newIsItemsHost) {
		}

		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			if (Background != null)
				drawingContext.DrawRectangle(Background, null, new Rect(0, 0, ActualWidth, ActualHeight));
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			//FIXME?: Recalculate ZIndex here.
		}
		#endregion

		#region Explicit Interface Implementations
		//FIXME: This is not documented.
		#region IAddChild
		void IAddChild.AddChild(object value) {
		}

		void IAddChild.AddText(string text) {
		}
		#endregion
		#endregion
	}
}