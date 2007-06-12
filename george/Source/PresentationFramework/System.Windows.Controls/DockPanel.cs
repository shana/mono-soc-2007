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
		public static readonly DependencyProperty LastChildFillProperty = DependencyProperty.Register("LastChildFill", typeof(bool), typeof(DockPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange));
		#region Attached Properties
		public static readonly DependencyProperty DockProperty = DependencyProperty.RegisterAttached("Dock", typeof(Dock), typeof(DockPanel), new FrameworkPropertyMetadata(Dock.Left, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FrameworkElement framework_element = d as FrameworkElement;
			if (framework_element == null)
				return;
			DockPanel dock_panel = framework_element.Parent as DockPanel;
			if (dock_panel == null)
				return;
			dock_panel.InvalidateArrange();
		}));
		#endregion
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
			if (InternalChildren.Count != 0) {
				Rect remaining_rect = new Rect(new Point(0, 0), finalSize);
				int children_to_lay_out_normally = InternalChildren.Count - (LastChildFill ? 1 : 0);
				for (int child_index = 0; child_index < children_to_lay_out_normally; child_index++) {
					UIElement element = InternalChildren[child_index];
					switch (GetDock(element)) {
					case Dock.Left:
						element.Arrange(new Rect(remaining_rect.Left, remaining_rect.Top, element.DesiredSize.Width, remaining_rect.Height));
						remaining_rect.X += element.DesiredSize.Width;
						remaining_rect.Width -= element.DesiredSize.Width;
						break;
					case Dock.Top:
						element.Arrange(new Rect(remaining_rect.Left, remaining_rect.Top, remaining_rect.Width, element.DesiredSize.Height));
						remaining_rect.Y += element.DesiredSize.Height;
						remaining_rect.Height -= element.DesiredSize.Height;
						break;
					case Dock.Right:
						element.Arrange(new Rect(remaining_rect.Right - element.DesiredSize.Width, remaining_rect.Top, element.DesiredSize.Width, remaining_rect.Height));
						remaining_rect.Width -= element.DesiredSize.Width;
						break;
					default:
						element.Arrange(new Rect(remaining_rect.Left, remaining_rect.Bottom - element.DesiredSize.Height, remaining_rect.Width, element.DesiredSize.Height));
						remaining_rect.Height -= element.DesiredSize.Height;
						break;
					}
				}
				if (LastChildFill)
					InternalChildren[InternalChildren.Count - 1].Arrange(remaining_rect);
			}
			return finalSize;
		}

		protected override Size MeasureOverride(Size availableSize) {
			if (InternalChildren.Count == 0)
				return new Size(0, 0);
			if (double.IsInfinity(availableSize.Width)) {
				Size desired_size = new Size();
				UIElement element;
				for (int child_index = 0; child_index < InternalChildren.Count - 1; child_index++) {
					element = InternalChildren[child_index];
					element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
					switch (GetDock(element)) {
					case Dock.Left:
					case Dock.Right:
						desired_size.Width += element.DesiredSize.Width;
						break;
					default:
						desired_size.Height += element.DesiredSize.Height;
						break;
					}
				}
				element = InternalChildren[InternalChildren.Count - 1];
				element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				desired_size.Width += element.DesiredSize.Width;
				desired_size.Height += element.DesiredSize.Height;
				return desired_size;
			} else {
				Size remaining_size = new Size(availableSize.Width, availableSize.Height);
				foreach (UIElement element in InternalChildren) {
					element.Measure(remaining_size);
					switch (GetDock(element)) {
					case Dock.Left:
					case Dock.Right:
						remaining_size.Width -= element.DesiredSize.Width;
						break;
					default:
						remaining_size.Height -= element.DesiredSize.Height;
						break;
					}
				}
				return new Size(availableSize.Width - remaining_size.Width, availableSize.Height - remaining_size.Height);
			}
		}
		#endregion
	}
}