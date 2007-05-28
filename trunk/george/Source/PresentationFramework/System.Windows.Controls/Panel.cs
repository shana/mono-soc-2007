using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Media;
#if Implementation
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[Localizability(LocalizationCategory.Ignore)]
	[ContentProperty("Children")]
	public abstract class Panel : FrameworkElement, IAddChild {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(Panel));
		public static readonly DependencyProperty IsItemsHostProperty = DependencyProperty.Register("IsItemsHost", typeof(bool), typeof(Panel));
		#region Attached Properties
		public static readonly DependencyProperty ZIndexProperty = DependencyProperty.RegisterAttached("ZIndex", typeof(int), typeof(Panel));
		#endregion
		#endregion
		#endregion

		#region Protected Constructors
		protected Panel() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public Brush Background {
			get { return (Brush)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}
		
		[Bindable(false)]
		public bool IsItemsHost {
			get { return (bool)GetValue(IsItemsHostProperty); }
			set { SetValue(IsItemsHostProperty, value); }
		}
		#endregion

		public UIElementCollection Children {
			get {
				return null;
			}
		}

	}
}