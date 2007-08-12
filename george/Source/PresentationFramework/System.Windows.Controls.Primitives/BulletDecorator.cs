using System.Collections;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	public class BulletDecorator : Decorator {
		#region Public Fields
		#region Dependency Property Fields
		static public readonly DependencyProperty BackgroundProperty = Panel.BackgroundProperty;
		#endregion
		#endregion

		#region Private Fields
		UIElement bullet;
		static IEnumerator empty_enumerator = new ArrayList().GetEnumerator();
		#endregion

		#region Static Constructor
		static BulletDecorator() {
			BackgroundProperty.OverrideMetadata(typeof(BulletDecorator), new FrameworkPropertyMetadata());
		}
		#endregion

		#region Public Constructors
		public BulletDecorator() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public Brush Background {
			get { return (Brush)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}
		#endregion

		public UIElement Bullet {
			get { return bullet; }
			set {
				if (bullet != null) {
					RemoveLogicalChild(bullet);
					RemoveVisualChild(bullet);
				}
				bullet = value;
				if (bullet != null) {
					AddLogicalChild(bullet);
					AddVisualChild(bullet);
				}
			}
		}
		#endregion

		#region Protected Properties
		protected override IEnumerator LogicalChildren {
			get {
				if (Bullet == null && Child == null)
					return empty_enumerator;
				ArrayList list = new ArrayList(2);
				if (Bullet != null)
					list.Add(Bullet);
				if (Child != null)
					list.Add(Child);
				return list.GetEnumerator();
			}
		}

		protected override int VisualChildrenCount {
			get {
				return (Bullet == null ? 0 : 1) + (Child == null ? 0 : 1);
			}
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size arrangeSize) {
			double used_width;
			if (Bullet != null) {
				//HACK
				double bullet_top;
				if (Child == null)
					bullet_top = 0;
				else {
					TextBlock text_block = ((ContentPresenter)Child).Content as TextBlock;
					double height_to_center_to = text_block == null ? Child.DesiredSize.Height : text_block.FontSize;
					bullet_top = height_to_center_to < Bullet.DesiredSize.Height ? 0 : ((height_to_center_to - Bullet.DesiredSize.Height) / 2);
				}
				Bullet.Arrange(new Rect(new Point(0, bullet_top), Bullet.DesiredSize));
				used_width = Bullet.DesiredSize.Width;
			} else
				used_width = 0;
			if (Child != null)
				Child.Arrange(new Rect(new Point(used_width, 0), Child.DesiredSize));
			return arrangeSize;
		}

		protected override Visual GetVisualChild(int index) {
			int visual_children_count = VisualChildrenCount;
			if (visual_children_count == 0 || index < 0 || index >= visual_children_count)
				throw new ArgumentOutOfRangeException("index", index, "Specified index is out of range or child at index is null. Do not call this method if VisualChildrenCount returns zero, indicating that the Visual has no children.");
			return index == 0 ? (Bullet == null ? Child : Bullet) : Child;
		}

		protected override Size MeasureOverride(Size constraint) {
			Size result;
			if (Bullet != null) {
				Bullet.Measure(constraint);
				result = Bullet.DesiredSize;
			} else
				result = new Size();
			if (Child != null) {
				Child.Measure(constraint);
				result.Width += Child.DesiredSize.Width;
				result.Height = Math.Max(result.Height, Child.DesiredSize.Height);
			}
			return result;
		}

		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			if (Background != null)
				drawingContext.DrawRectangle(Background, null, new Rect(0, 0, ActualWidth, ActualHeight));
		}
		#endregion
	}
}