//FIXME: When in a tool bar, it does not display keyboard focus cues.
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
	public class Button : ButtonBase {
		#region Private Fields
		bool in_tool_bar;
		#endregion

		#region Dependency Property Fields
		static public readonly DependencyProperty IsCancelProperty = DependencyProperty.Register("IsCancel", typeof(bool), typeof(Button));
		static public readonly DependencyProperty IsDefaultedProperty = DependencyProperty.RegisterReadOnly("IsDefaulted", typeof(bool), typeof(Button), new PropertyMetadata()).DependencyProperty;
		static public readonly DependencyProperty IsDefaultProperty = DependencyProperty.Register("IsDefault", typeof(bool), typeof(Button));
		#endregion

		#region Static Constructor
		static Button() {
			Theme.Load();
		}
		#endregion

		#region Public Constructors
		public Button() {
			//FIXME: Do this when the parent changes.
			LayoutUpdated += delegate(object sender, EventArgs e) {
				bool new_in_tool_bar = Parent is ToolBar;
				if (new_in_tool_bar != in_tool_bar) {
					in_tool_bar = new_in_tool_bar;
					//FIXME: Check.
					Style = in_tool_bar ? (Style)Application.Current.FindResource(ToolBar.ButtonStyleKey) : null;
				}
			};
		}
		#endregion

		#region Public Properties
		public bool IsCancel { 
			get { return (bool)GetValue(IsCancelProperty); }
			set { SetValue(IsCancelProperty, value); }
		}

		public bool IsDefault {
			get { return (bool)GetValue(IsDefaultProperty); }
			set { SetValue(IsDefaultProperty, value); }
		}

		public bool IsDefaulted {
			get { return (bool)GetValue(IsDefaultedProperty); }
		}
		#endregion

		#region Protected Methods
		protected override void OnClick() {
			//WDTDH
			base.OnClick();
			if (in_tool_bar)
				Keyboard.Focus(null);
		}

		protected override AutomationPeer OnCreateAutomationPeer() {
#if Implementation
			return null;
#else
			return new ButtonAutomationPeer(this);
#endif
		}
		#endregion
	}
}