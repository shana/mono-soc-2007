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
		public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(Panel));
		public static readonly DependencyProperty IsItemsHostProperty = DependencyProperty.Register("IsItemsHost", typeof(bool), typeof(Panel));
		#region Attached Properties
		public static readonly DependencyProperty ZIndexProperty = DependencyProperty.RegisterAttached("ZIndex", typeof(int), typeof(Panel));
		#endregion
		#endregion
		#endregion

		#region Protected Constructors
		protected Panel() {
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
				return null;
			}
		}
		#endregion

		#region Protected Properties
		protected override int VisualChildrenCount {
			get {
				return base.VisualChildrenCount;
			}
		}
		#endregion

		#region Protected Internal Properties
		protected internal virtual bool HasLogicalOrientation {
			get {
				return false;
			}
		}

		protected internal UIElementCollection InternalChildren {
			get {
				return null;
			}
		}

		protected override IEnumerator LogicalChildren {
			get {
				return base.LogicalChildren;
			}
		}

		protected internal virtual Orientation LogicalOrientation {
			get {
				return Orientation.Horizontal;
			}
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
			return false;
		}
		#endregion

		#region Protected Methods
		protected virtual UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent) {
			return null;
		}

		protected override Visual GetVisualChild(int index) {
			return base.GetVisualChild(index);
		}

		protected virtual void OnIsItemsHostChanged(bool oldIsItemsHost, bool newIsItemsHost) {
		}

		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
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