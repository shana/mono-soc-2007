using System.Windows.Automation.Peers;
using System.ComponentModel;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
	public class Expander : HeaderedContentControl {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty ExpandDirectionProperty = DependencyProperty.Register("ExpandDirection", typeof(ExpandDirection), typeof(Expander), new FrameworkPropertyMetadata(ExpandDirection.Down, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(Expander), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Expander i = (Expander)d;
			if ((bool)e.NewValue)
				i.OnExpanded();
			else
				i.OnCollapsed();
				
		}));
		#endregion

		#region Routed Events
		public static readonly RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent("Collapsed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Expander));
		public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent("Expanded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Expander));
		#endregion
		#endregion

		#region Static Constructor
		static Expander() {
#if Implementation
			Theme.Load();
#endif
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Expander), new FrameworkPropertyMetadata(typeof(Expander)));
		}
		#endregion

		#region Public Properties
		[Bindable(true)]
		public ExpandDirection ExpandDirection {
			get { return (ExpandDirection)GetValue(ExpandDirectionProperty); }
			set { SetValue(ExpandDirectionProperty, value); }
		}

		[Bindable(true)]
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		#endregion

		#region Protected Methods
		protected virtual void OnCollapsed() {
			RoutedEventArgs e = new RoutedEventArgs();
			e.RoutedEvent = CollapsedEvent;
			RaiseEvent(e);
		}

		protected override AutomationPeer OnCreateAutomationPeer() {
#if Implementation
			
			return null;
#else
			return new ExpanderAutomationPeer(this);
#endif
		}

		protected virtual void OnExpanded() {
			RoutedEventArgs e = new RoutedEventArgs();
			e.RoutedEvent = ExpandedEvent;
			RaiseEvent(e);
		}
		#endregion

		#region Public Events
		public event RoutedEventHandler Collapsed {
			add { AddHandler(CollapsedEvent, value); }
			remove { RemoveHandler(CollapsedEvent, value); }
		}
		
		public event RoutedEventHandler Expanded {
			add { AddHandler(ExpandedEvent, value); }
			remove { RemoveHandler(ExpandedEvent, value); }
		}
		#endregion
	}
}