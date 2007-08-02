using System.Collections;
using System.Windows.Markup;
using System.Windows.Media;
#if Implementation
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
			set { child = value; }
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
			//WDTDH
		}

		void IAddChild.AddText(string text) {
			//WDTDH
		}
		#endregion
		#endregion
	}
}