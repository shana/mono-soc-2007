using System.Globalization;
using System.Windows;
#if Implementation
namespace Mono.Microsoft.Windows.Themes {
#else
namespace Microsoft.Windows.Themes {
#endif
	public static class PlatformCulture {
		#region Public Properties
		public static FlowDirection FlowDirection {
			get {
				return CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
			}
		}
		#endregion
	}
}