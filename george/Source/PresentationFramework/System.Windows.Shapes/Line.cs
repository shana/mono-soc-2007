using System.ComponentModel;
using System.Windows.Media;
#if Implementation
using System.Windows;
namespace Mono.System.Windows.Shapes {
#else
namespace System.Windows.Shapes {
#endif
	public sealed class Line : global::System.Windows.Shapes.Shape {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof(double), typeof(Line), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(double), typeof(Line), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(double), typeof(Line), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(double), typeof(Line), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		#endregion
		#endregion

		#region Public Constructors
		public Line() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[TypeConverter(typeof(LengthConverter))]
		public double X1 {
			get { return (double)GetValue(X1Property); }
			set { SetValue(X1Property, value); }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double X2 {
			get { return (double)GetValue(X2Property); }
			set { SetValue(X2Property, value); }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double Y1 {
			get { return (double)GetValue(Y1Property); }
			set { SetValue(Y1Property, value); }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double Y2 {
			get { return (double)GetValue(Y2Property); }
			set { SetValue(Y2Property, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override Geometry DefiningGeometry {
			get {
				return new LineGeometry(new Point(X1, Y1), new Point(X2, Y2));
			}
		}
		#endregion
	}
}