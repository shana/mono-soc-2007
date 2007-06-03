#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public class DockPanel : Panel {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty LastChildFillProperty = DependencyProperty.Register("LastChildFill", typeof(Dock), typeof(DockPanel));
		#endregion
		#region Attached Properties
		public static readonly DependencyProperty DockProperty = DependencyProperty.RegisterAttached("Dock", typeof(Dock), typeof(DockPanel));
		#endregion
		#endregion

		#region Public Constructors
		public DockPanel() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public bool LastChildFill {
			get { return (bool)GetValue(LastChildFillProperty); }
			set { SetValue(LastChildFillProperty, value); }
		}
		#endregion
		#endregion

		#region Public Methods
		#region Attached Properties
		[AttachedPropertyBrowsableForChildren]
		public static Dock GetDock(UIElement element) {
			return (Dock)element.GetValue(DockProperty);
		}

		public static void SetDock(UIElement element, Dock dock) {
			element.SetValue(DockProperty, dock);
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize) {
			return base.ArrangeOverride(finalSize);
		}

		protected override Size MeasureOverride(Size availableSize) {
			return base.MeasureOverride(availableSize);
		}
		#endregion
	}
}