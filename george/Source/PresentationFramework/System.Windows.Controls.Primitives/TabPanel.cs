using System.Windows.Media;
#if Implementation
using System.Windows;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	public class TabPanel : Panel {
		#region Public Constructors
		public TabPanel() {
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize) {
			return base.ArrangeOverride(finalSize);
		}
		protected override Geometry GetLayoutClip(Size layoutSlotSize) {
			return base.GetLayoutClip(layoutSlotSize);
		}

		protected override Size MeasureOverride(Size availableSize) {
			return base.MeasureOverride(availableSize);
		}
		#endregion
	}
}