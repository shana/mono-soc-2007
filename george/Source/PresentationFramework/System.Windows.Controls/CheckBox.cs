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
		#region Public Constructors
		public CheckBox() {
			ThemeStyle = Theme.GetStyle(typeof(CheckBox));
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
		
		#region Internal Properties
		//FIXME: This should be used by lower-level classes when they are implemented.
		internal Style ThemeStyle {
			set {
				Style = value;
			}
		}
		#endregion
	}
}