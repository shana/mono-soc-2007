using System.Windows.Media;
#if Implementation
using System.Windows;
namespace Mono.System.Windows.Shapes {
#else
namespace System.Windows.Shapes {
#endif
	public sealed class Path : global::System.Windows.Shapes.Shape {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Geometry), typeof(Path), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		#endregion
		#endregion

		#region Public Constructors
		public Path() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public Geometry Data {
			get { return (Geometry)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override Geometry DefiningGeometry {
			get { return Data; }
		}
		#endregion
	}
}