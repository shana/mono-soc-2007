using System.ComponentModel;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public class ColumnDefinition : DefinitionBase {
		#region Public Fields
		#region Dependency Properties
		[TypeConverter("System.Windows.LengthConverter, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, Custom=null")]
		public static readonly DependencyProperty MaxWidthProperty = DependencyProperty.Register("MaxWidth", typeof(double), typeof(ColumnDefinition), new FrameworkPropertyMetadata(double.PositiveInfinity));
		[TypeConverter("System.Windows.LengthConverter, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, Custom=null")]
		public static readonly DependencyProperty MinWidthProperty = DependencyProperty.Register("MinWidth", typeof(double), typeof(ColumnDefinition), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(GridLength), typeof(ColumnDefinition), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnDefinition)d).InvalidateGridMeasure();
		}));
		#endregion
		#endregion

		#region Private Fields
		double actual_width;
		double offset;
		Grid grid;
		#endregion

		#region Public Constructors
		public ColumnDefinition() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[TypeConverter(typeof(LengthConverter))]
		public double MaxWidth {
			get { return (double)GetValue(MaxWidthProperty); }
			set { SetValue(MaxWidthProperty, value); } 
		}

		[TypeConverter(typeof(LengthConverter))]
		public double MinWidth {
			get { return (double)GetValue(MinWidthProperty); }
			set { SetValue(MinWidthProperty, value); }
		}

		public GridLength Width {
			get { return (GridLength)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		#endregion

		public double ActualWidth {
			get { return actual_width; }
			internal set { actual_width = value; }
		}

		public double Offset {
			get { return offset; }
			internal set { offset = value; }
		}
		#endregion

		#region Internal Properties
		internal Grid Grid {
			get { return grid; }
			set { grid = value; }
		}
		#endregion

		#region Private Methods
		void InvalidateGridMeasure() {
			if (grid != null)
				grid.InvalidateMeasure();
		}
		#endregion
	}
}