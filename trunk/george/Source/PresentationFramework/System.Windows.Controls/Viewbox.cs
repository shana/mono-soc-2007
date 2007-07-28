using System.Collections;
using System.Windows.Media;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public class Viewbox : Decorator {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register("StretchDirection", typeof(StretchDirection), typeof(Viewbox), new FrameworkPropertyMetadata(StretchDirection.Both, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(Viewbox), new FrameworkPropertyMetadata(Stretch.Uniform, FrameworkPropertyMetadataOptions.AffectsMeasure));
		#endregion
		#endregion

		#region Public Constructors
		public Viewbox() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public StretchDirection StretchDirection {
			get { return (StretchDirection)GetValue(StretchDirectionProperty); }
			set { SetValue(StretchDirectionProperty, value); }
		}
		
		public Stretch Stretch {
			get { return (Stretch)GetValue(StretchProperty); }
			set { SetValue(StretchProperty, value); }
		}
		#endregion

		public override UIElement Child {
			get { 
				return base.Child;
			}
			set {
				base.Child = value;
			}
		}
		#endregion

		#region Protected Properties
		protected override IEnumerator LogicalChildren {
			get {
				return base.LogicalChildren;
			}
		}

		protected override int VisualChildrenCount {
			get { return 1; }
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size arrangeSize) {
			return base.ArrangeOverride(arrangeSize);
		}

		protected override Visual GetVisualChild(int index) {
			return base.GetVisualChild(index);
		}

		protected override Size MeasureOverride(Size constraint) {
			return base.MeasureOverride(constraint);
		}
		#endregion
	}
}