using System;
using System.Windows.Automation.Peers;
using System.Windows.Input;
#if Implementation
using System.Windows;
using System.Windows.Controls;
using Mono.System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls {
#else
using System.Windows.Controls.Primitives;
namespace System.Windows.Controls {
#endif
	[Localizability(LocalizationCategory.CheckBox)]
	public class CheckBox : ToggleButton {
		#region Static Constructor
		static CheckBox() {
#if Implementation
			Theme.Load();
#endif
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBox), new FrameworkPropertyMetadata(typeof(CheckBox)));
		}
		#endregion

		#region Public Constructors
		public CheckBox() {
		}
		#endregion

		#region Protected Methods
		protected override void OnAccessKey(AccessKeyEventArgs e) {
			if (e == null)
				throw new NullReferenceException();
			base.OnAccessKey(e);
			Focus();
		}

		protected override AutomationPeer OnCreateAutomationPeer() {
#if Implementation
			return null;
#else
			return new CheckBoxAutomationPeer(this);
#endif
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			//WDTDH
			base.OnKeyDown(e);
		}
		#endregion
	}
}