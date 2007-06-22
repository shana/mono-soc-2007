using System.ComponentModel;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public class RowDefinition : DefinitionBase {
		#region Public Fields
		#region Dependency Properties
		[TypeConverter("System.Windows.LengthConverter, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, Custom=null")]
		public static readonly DependencyProperty MaxHeightProperty = DependencyProperty.Register("MaxHeight", typeof(double), typeof(RowDefinition), new FrameworkPropertyMetadata(double.PositiveInfinity));
		[TypeConverter("System.Windows.LengthConverter, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, Custom=null")]
		public static readonly DependencyProperty MinHeightProperty = DependencyProperty.Register("MinHeight", typeof(double), typeof(RowDefinition), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(GridLength), typeof(RowDefinition), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RowDefinition)d).InvalidateGridMeasure();
		}));
		#endregion
		#endregion

		#region Private Fields
		double actual_height;
		double offset;
		Grid grid;
		#endregion

		#region Public Constructors
		public RowDefinition() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[TypeConverter(typeof(LengthConverter))]
		public double MaxHeight {
			get { return (double)GetValue(MaxHeightProperty); }
			set { SetValue(MaxHeightProperty, value); } 
		}

		[TypeConverter(typeof(LengthConverter))]
		public double MinHeight {
			get { return (double)GetValue(MinHeightProperty); }
			set { SetValue(MinHeightProperty, value); }
		}

		public GridLength Height {
			get { return (GridLength)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		#endregion

		public double ActualHeight {
			get { return actual_height; }
			internal set { actual_height = value; }
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