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
	[Localizability(LocalizationCategory.RadioButton)]
	public class RadioButton : ToggleButton {
		#region Dependency Property Fields
		static public readonly DependencyProperty GroupNameProperty = DependencyProperty.Register("GroupName", typeof(string), typeof(RadioButton), new FrameworkPropertyMetadata(string.Empty));
		#endregion

		#region Static Contstructor
		static RadioButton() {
#if Implementation
			Theme.Load();
#endif
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioButton), new FrameworkPropertyMetadata(typeof(RadioButton)));
		}
		#endregion

		#region Public Constructors
		public RadioButton() {
		}
		#endregion

		#region Public Properties
		[Localizability(LocalizationCategory.NeverLocalize)]
		public string GroupName {
			get { return (string)GetValue(GroupNameProperty); }
			set { SetValue(GroupNameProperty, value); }
		}
		#endregion

		#region Protected Methods
		protected override void OnAccessKey(AccessKeyEventArgs e) {
			//WDTDH
			base.OnAccessKey(e);
		}

		protected override void OnChecked(RoutedEventArgs e) {
			base.OnChecked(e);
			DependencyObject parent = LogicalTreeHelper.GetParent(this);
			if (parent != null) {
				string group_name = GroupName;
				foreach (object child in LogicalTreeHelper.GetChildren(parent)) {
					RadioButton radio_button = child as RadioButton;
					if (radio_button != null && radio_button != this && radio_button.GroupName == group_name)
						radio_button.IsChecked = false;
				}
			}
		}

		protected override AutomationPeer OnCreateAutomationPeer() {
#if Implementation
			return null;
#else
			return new RadioButtonAutomationPeer(this);
#endif
		}

		protected override void OnToggle() {
			IsChecked = true;
		}
		#endregion
	}
}