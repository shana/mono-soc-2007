#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	//TODO: Fix when integrating. (FrameworkTemplate contains internal abstract members.)
	public class ItemsPanelTemplate : global::System.Windows.Controls.ItemsPanelTemplate {
		#region Public Constructors
		public ItemsPanelTemplate() {
		}

		public ItemsPanelTemplate(FrameworkElementFactory root)
			: base(root) {
		}
		#endregion

		#region Protected Methods
		protected override void ValidateTemplatedParent(FrameworkElement templatedParent) {
			if (templatedParent == null)
				throw new ArgumentNullException("templatedParent");
			//LAMESPEC?
			if (!(templatedParent is ItemsPresenter))
				throw new ArgumentException("'ItemsPresenter' ControlTemplate TargetType does not match templated type 'FrameworkElement'.");
		}
		#endregion
	}
}