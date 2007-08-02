using System.Collections;
using System.Windows.Markup;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[ContentProperty("Child")]
	[Localizability(LocalizationCategory.Ignore, Readability = Readability.Unreadable)]
	public class Decorator : FrameworkElement, IAddChild {
		#region Private Fields
		UIElement child;
		#endregion

		#region Public Constructors
		public Decorator() {
		}
		#endregion

		#region Public Properties
		public virtual UIElement Child {
			get { return child; }
			set {
				if (child == value)
					return;
				if (child != null) {
					RemoveLogicalChild(child);
					RemoveVisualChild(child);
				}
				child = value;
				AddLogicalChild(child);
				AddVisualChild(child);
				InvalidateVisual();
			}
		}
		#endregion

		#region Protected Properties
		protected override IEnumerator LogicalChildren {
			get {
				return (child == null ? new object[] { } : new object[] { child }).GetEnumerator();
			}
		}

		protected override int VisualChildrenCount {
			get {
				return child == null ? 0 : 1;
			}
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize) {
			if (child == null)
				return finalSize;
			child.Arrange(new Rect(new Point(0, 0), finalSize));
			return finalSize;
		}

		protected override Visual GetVisualChild(int index) {
			return (index == 0 && child != null) ? child : base.GetVisualChild(-1);
		}

		protected override Size MeasureOverride(Size availableSize) {
			if (child == null)
				return new Size(0, 0);
			child.Measure(availableSize);
			return child.DesiredSize;
		}
		#endregion

		#region Explicit Interface Implementations
		#region IAddChild
		void IAddChild.AddChild(object value) {
			UIElement ui_element = value as UIElement;
			if (ui_element == null)
				if (value == null)
					throw new NullReferenceException();
				else
					throw new ArgumentException(string.Format("Parameter is unexpected type '{0}'. Expected type is 'System.Windows.UIElement'.\r\nParameter name: value", value.GetType().FullName));
			else
				if (Child == null)
					Child = ui_element;
				else
					throw new ArgumentException(string.Format("'System.Windows.Controls.Decorator' already has a child and cannot add '{0}'. 'System.Windows.Controls.Decorator' can accept only one child.", value.GetType().FullName));
		}

		void IAddChild.AddText(string text) {
			throw new ArgumentException(string.Format("'{0}' text cannot be added because text is not valid in this element.", text));
		}
		#endregion
		#endregion
	}
}