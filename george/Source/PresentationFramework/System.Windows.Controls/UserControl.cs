//FIXME?: Is this all it does?
using System.Windows.Automation.Peers;
#if Implementation
using System.Windows;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public class UserControl : ContentControl {
		#region Static Constructor
		static UserControl() {
#if Implementation
			Theme.Load();
#endif
			DefaultStyleKeyProperty.OverrideMetadata(typeof(UserControl), new FrameworkPropertyMetadata(typeof(UserControl)));
		}
		#endregion

		#region Public Constructord
		public UserControl() {
		}
		#endregion

		#region Protected Methods
		protected override AutomationPeer OnCreateAutomationPeer() {
#if Implementation
			return null;
#else
			return new UserControlAutomationPeer(this);
#endif
		}
		#endregion
	}
}