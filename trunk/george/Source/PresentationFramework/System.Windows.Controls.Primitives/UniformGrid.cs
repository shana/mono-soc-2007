#if Implementation
using System.Windows;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	public class UniformGrid : Panel {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(UniformGrid), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure), ValidateNonNegativeInteger);
		public static readonly DependencyProperty FirstColumnProperty = DependencyProperty.Register("FirstColumn", typeof(int), typeof(UniformGrid), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure), ValidateNonNegativeInteger);
		public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(UniformGrid), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure), ValidateNonNegativeInteger);
		#endregion
		#endregion

		#region Public Constructors
		public UniformGrid() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public int Columns {
			get { return (int)GetValue(ColumnsProperty); }
			set { SetValue(ColumnsProperty, value); }
		}

		public int FirstColumn {
			get { return (int)GetValue(FirstColumnProperty); }
			set { SetValue(FirstColumnProperty, value); }
		}

		public int Rows {
			get { return (int)GetValue(RowsProperty); }
			set { SetValue(RowsProperty, value); }
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

		#region Private Methods
		static bool ValidateNonNegativeInteger(object value) {
			return (int)value >= 0;
		}
		#endregion
	}
}