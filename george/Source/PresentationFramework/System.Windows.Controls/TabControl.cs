#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[StyleTypedProperty(Property="ItemContainerStyle", StyleTargetType=typeof(TabItem))]
	[TemplatePart(Name="PART_SelectedContentHost", Type=typeof(ContentPresenter))]
	public class TabControl : global::System.Windows.Controls.Primitives.Selector {
		public TabControl() {
		}
	}
}