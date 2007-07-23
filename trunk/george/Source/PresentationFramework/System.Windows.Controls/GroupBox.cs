using System.Windows.Input;
using System.Windows.Automation.Peers;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
	public class GroupBox : HeaderedContentControl {
		#region Static Constructor
		static GroupBox() {
#if Implementation
			Theme.Load();
#endif
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupBox), new FrameworkPropertyMetadata(typeof(GroupBox)));
		}
		#endregion

		#region Public Constructors
		public GroupBox() {
		}
		#endregion

		#region Protected Methods
		protected override void OnAccessKey(AccessKeyEventArgs e) {
			base.OnAccessKey(e);
			//WDTDH
		}

		protected override AutomationPeer OnCreateAutomationPeer() {
#if Implementation
			return null;
#else
			return new GroupBoxAutomationPeer(this);
#endif
		}
		#endregion
	}
}