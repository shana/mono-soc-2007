using System.Collections;
using System.Windows.Markup;
using System.Windows.Media;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public class Grid : Panel {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty ShowGridLinesProperty = DependencyProperty.Register("ShowGridLines", typeof(bool), typeof(Grid), new FrameworkPropertyMetadata());
		#region Attached Properties
		public static readonly DependencyProperty ColumnProperty = DependencyProperty.RegisterAttached("Column", typeof(int), typeof(Grid), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty ColumnSpanProperty = DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), typeof(Grid), new FrameworkPropertyMetadata(1));
		public static readonly DependencyProperty IsSharedSizeScopeProperty = DependencyProperty.RegisterAttached("IsSharedSizeScope", typeof(bool), typeof(Grid), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty RowProperty = DependencyProperty.RegisterAttached("Row", typeof(int), typeof(Grid), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty RowSpanProperty = DependencyProperty.RegisterAttached("RowSpan", typeof(int), typeof(Grid), new FrameworkPropertyMetadata(1));
		#endregion
		#endregion
		#endregion

		#region Public Constructors
		public Grid() {
		}
		#endregion

		#region Public Properties
		public ColumnDefinitionCollection ColumnDefinitions {
			get { return null; }
		}

		public RowDefinitionCollection RowDefinitions {
			get { return null; }
		}

		#region Dependency Properties
		public bool ShowGridLines {
			get { return (bool)GetValue(ShowGridLinesProperty); }
			set { SetValue(ShowGridLinesProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Properties
		protected override IEnumerator LogicalChildren {
			get {
				return base.LogicalChildren;
			}
		}

		protected override int VisualChildrenCount {
			get {
				return base.VisualChildrenCount;
			}
		}
		#endregion

		#region Public Methods
		#region Attached Properties
		[AttachedPropertyBrowsableForChildren]
		public static int GetColumn(UIElement element) {
			return (int)element.GetValue(ColumnProperty);
		}

		[AttachedPropertyBrowsableForChildren]
		public static int GetColumnSpan(UIElement element) {
			return (int)element.GetValue(ColumnSpanProperty);
		}

		public static bool GetIsSharedSizeScope(UIElement element) {
			return (bool)element.GetValue(IsSharedSizeScopeProperty);
		}

		[AttachedPropertyBrowsableForChildren]
		public static int GetRow(UIElement element) {
			return (int)element.GetValue(RowProperty);
		}

		[AttachedPropertyBrowsableForChildren]
		public static int GetRowSpan(UIElement element) {
			return (int)element.GetValue(RowSpanProperty);
		}

		public static void SetColumn(UIElement element, int value) {
			element.SetValue(ColumnProperty, value);
		}

		public static void SetColumnSpan(UIElement element, int value) {
			element.SetValue(ColumnSpanProperty, value);
		}

		public static void SetIsSharedSizeScope(UIElement element, bool value) {
			element.SetValue(IsSharedSizeScopeProperty, value);
		}

		public static void SetRow(UIElement element, int value) {
			element.SetValue(RowProperty, value);
		}

		public static void SetRowSpan(UIElement element, int value) {
			element.SetValue(RowSpanProperty, value);
		}
		#endregion

		public bool ShouldSerializeColumnDefinitions() {
			return ColumnDefinitions.Count != 0;
		}

		public bool ShouldSerializeRowDefinitions() {
			return RowDefinitions.Count != 0;
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize) {
			return base.ArrangeOverride(finalSize);
		}

		protected override Visual GetVisualChild(int index) {
			return base.GetVisualChild(index);
		}

		protected override Size MeasureOverride(Size availableSize) {
			return base.MeasureOverride(availableSize);
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
		}
		#endregion
	}
}