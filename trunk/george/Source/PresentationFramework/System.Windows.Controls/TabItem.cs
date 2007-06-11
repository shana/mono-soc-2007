using System.ComponentModel;
using System.Windows.Automation.Peers;
using System.Windows.Input;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public class TabItem : HeaderedContentControl {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(TabItem), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty TabStripPlacementProperty = DependencyProperty.RegisterReadOnly("TabStripPlacement", typeof(Dock), typeof(TabItem)).DependencyProperty; 
		#endregion
		#endregion

		#region Static Constructor
		static TabItem() {
			Theme.Load();
		}
		#endregion

		#region Public Constructors
		public TabItem() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[Bindable(true)]
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		public Dock TabStripPlacement {
			get { return (Dock)GetValue(TabStripPlacementProperty); }
		}
		#endregion
		#endregion

		#region Protected Methods
		protected override void OnAccessKey(AccessKeyEventArgs e) {
			base.OnAccessKey(e);
		}

		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);
		}

		protected override void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate) {
			base.OnContentTemplateChanged(oldContentTemplate, newContentTemplate);
		}

		protected override void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector) {
			base.OnContentTemplateSelectorChanged(oldContentTemplateSelector, newContentTemplateSelector);
		}

		protected override AutomationPeer OnCreateAutomationPeer() {
			return base.OnCreateAutomationPeer();
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
		}

		protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnPreviewGotKeyboardFocus(e);
		}

		protected virtual void OnSelected(RoutedEventArgs e) {
		}

		protected virtual void OnUnselected(RoutedEventArgs e) {
		}
		#endregion
	}
}