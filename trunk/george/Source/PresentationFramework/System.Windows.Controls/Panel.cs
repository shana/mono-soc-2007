using System.Collections;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Media;
#if Implementation
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
		public static readonly DependencyProperty IsItemsHostProperty = DependencyProperty.Register("IsItemsHost", typeof(bool), typeof(Panel), new PropertyMetadata(delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Panel)d).OnIsItemsHostChanged((bool)e.OldValue, (bool)e.NewValue);
		}));
		#region Attached Properties
		public static readonly DependencyProperty ZIndexProperty = DependencyProperty.RegisterAttached("ZIndex", typeof(int), typeof(Panel));
		#endregion
		#endregion
		#endregion

		#region Private Fields
		UIElementCollection children;
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
			get { return children; }
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
			return children[index];
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
			//FIXME: Recalculate ZIndex here.
		}
		#endregion

		#region Explicit Interface Implementations
		#region IAddChild
		void IAddChild.AddChild(object value) {
		}

		void IAddChild.AddText(string text) {
		}
		#endregion
		#endregion
	}
}