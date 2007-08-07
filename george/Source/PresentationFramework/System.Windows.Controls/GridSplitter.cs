using System.Windows.Input;
using System.Windows.Automation.Peers;
#if Implementation
using System.Windows;
using Mono.System.Windows.Controls.Primitives;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
using System.Windows.Controls.Primitives;
namespace System.Windows.Controls {
#endif
	[StyleTypedProperty(Property = "PreviewStyle", StyleTargetType = typeof(Control))]
	public class GridSplitter : global::System.Windows.Controls.Primitives.Thumb {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty DragIncrementProperty = DependencyProperty.Register("DragIncrement", typeof(double), typeof(GridSplitter), new FrameworkPropertyMetadata(1D));
		public static readonly DependencyProperty KeyboardIncrementProperty = DependencyProperty.Register("KeyboardIncrement", typeof(double), typeof(GridSplitter), new FrameworkPropertyMetadata(10D));
		public static readonly DependencyProperty PreviewStyleProperty = DependencyProperty.Register("PreviewStyle", typeof(Style), typeof(GridSplitter), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty ResizeBehaviorProperty = DependencyProperty.Register("ResizeBehavior", typeof(GridResizeBehavior), typeof(GridSplitter), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty ResizeDirectionProperty = DependencyProperty.Register("ResizeDirection", typeof(GridResizeDirection), typeof(GridSplitter), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty ShowsPreviewProperty = DependencyProperty.Register("ShowsPreview", typeof(bool), typeof(GridSplitter), new FrameworkPropertyMetadata());
		#endregion
		#endregion

		#region Static Constructor
		static GridSplitter() {
#if Implementation
			Theme.Load();
#endif
			HorizontalAlignmentProperty.AddOwner(typeof(GridSplitter), new FrameworkPropertyMetadata(HorizontalAlignment.Right));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GridSplitter), new FrameworkPropertyMetadata(typeof(GridSplitter)));
		}
		#endregion

		#region Public Constructors
		public GridSplitter() {
		}
		#endregion

		#region Public Fields
		#region Dependency Properties
		public double DragIncrement {
			get { return (double)GetValue(DragIncrementProperty); }
			set { SetValue(DragIncrementProperty, value); }
		}

		public double KeyboardIncrement {
			get { return (double)GetValue(KeyboardIncrementProperty); }
			set { SetValue(KeyboardIncrementProperty, value); }
		}

		public Style PreviewStyle {
			get { return (Style)GetValue(PreviewStyleProperty); }
			set { SetValue(PreviewStyleProperty, value); }
		}

		public GridResizeBehavior ResizeBehavior {
			get { return (GridResizeBehavior)GetValue(ResizeBehaviorProperty); }
			set { SetValue(ResizeBehaviorProperty, value); }
		}

		public GridResizeDirection ResizeDirection {
			get { return (GridResizeDirection)GetValue(ResizeDirectionProperty); }
			set { SetValue(ResizeDirectionProperty, value); }
		}

		public bool ShowsPreview {
			get { return (bool)GetValue(ShowsPreviewProperty); }
			set { SetValue(ShowsPreviewProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override AutomationPeer OnCreateAutomationPeer() {
#if Implementation
			return null;
#else
			return new GridSplitterAutomationPeer(this);
#endif
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
		}

		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnLostKeyboardFocus(e);
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			base.OnRenderSizeChanged(sizeInfo);
			Cursor = GetActualResizeDirection() == GridResizeDirection.Rows ? Cursors.SizeNS : Cursors.SizeWE;
		}
		#endregion

		GridResizeDirection GetActualResizeDirection() {
			GridResizeDirection value = ResizeDirection;
			if (value == GridResizeDirection.Auto)
				if (HorizontalAlignment != HorizontalAlignment.Stretch)
					return GridResizeDirection.Columns;
				else if (VerticalAlignment != VerticalAlignment.Stretch)
					return GridResizeDirection.Rows;
				else
					return ActualHeight >= ActualWidth ? GridResizeDirection.Columns : GridResizeDirection.Rows;
			else
				return value;
		}
	}
}