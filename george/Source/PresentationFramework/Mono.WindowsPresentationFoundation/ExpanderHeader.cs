using System.Windows;
namespace Mono.WindowsPresentationFoundation {
	public class ExpanderHeader : Mono.System.Windows.Controls.Primitives.ToggleButton {
		#region Public Constructors
		static ExpanderHeader() {
#if Implementation
			Theme.Load();
#endif
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpanderHeader), new FrameworkPropertyMetadata(typeof(ExpanderHeader)));
		}
		#endregion
	}
}