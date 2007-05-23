using System;
using System.Windows.Input;
using System.Windows.Automation.Peers;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public class Label : ContentControl {
		#region Dependency Property Fields
		public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(UIElement), typeof(Label));
		#endregion

		#region Static Constructor
		static Label() {
			Theme.Load();
		}
		#endregion

		#region Public Constructors
		public Label() {
			IsTabStop = false;
            AccessKeyManager.AddAccessKeyPressedHandler(this, delegate(object sender, AccessKeyPressedEventArgs e) {
                UIElement target = Target;
                if (target != null)
                    target.Focus();
            });
		}
		#endregion

		#region Public Properties
		public UIElement Target {
			get { return (UIElement)GetValue(TargetProperty); }
			set { SetValue(TargetProperty, value); }
		}
		#endregion

		#region Protected Methods
		protected override AutomationPeer OnCreateAutomationPeer() {
#if Implementation
			return null;
#else
			return new LabelAutomationPeer(this);
#endif
		}
		#endregion
	}
}