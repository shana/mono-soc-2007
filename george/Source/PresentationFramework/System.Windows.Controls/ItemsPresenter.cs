#if Implementation
using System.Windows;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[Localizability(LocalizationCategory.NeverLocalize)]
	public class ItemsPresenter : FrameworkElement {
		#region Public Constructors
		public ItemsPresenter() {
		}
		#endregion

		#region Public Methods
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			//WDTDH
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize) {
			return base.ArrangeOverride(finalSize);
		}

		protected override Size MeasureOverride(Size availableSize) {
			return base.MeasureOverride(availableSize);
		}

		protected virtual void OnTemplateChanged(ItemsPanelTemplate oldTemplate, ItemsPanelTemplate newTemplate) {
		}
		#endregion
	}
}