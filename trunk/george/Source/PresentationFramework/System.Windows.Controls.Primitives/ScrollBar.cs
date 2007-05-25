//FIXME: The Thumb and the RepeatButtons are displayed only when there is enough room.
using Mono.WindowsPresentationFoundation;
using System.Windows.Automation.Peers;
using System.Windows.Input;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	[Localizability(LocalizationCategory.NeverLocalize)]
	[TemplatePart(Name="PART_Track", Type=typeof(Track))]
	public class ScrollBar : RangeBase {
		#region Public Fields
		#region Commands
		public static readonly RoutedCommand LineDownCommand = new RoutedCommand("LineDown", typeof(ScrollBar));
		public static readonly RoutedCommand LineLeftCommand = new RoutedCommand("LineLeft", typeof(ScrollBar));
		public static readonly RoutedCommand LineRightCommand = new RoutedCommand("LineRight", typeof(ScrollBar));
		public static readonly RoutedCommand LineUpCommand = new RoutedCommand("LineUp", typeof(ScrollBar));
		public static readonly RoutedCommand PageDownCommand = new RoutedCommand("PageDown", typeof(ScrollBar));
		public static readonly RoutedCommand PageLeftCommand = new RoutedCommand("PageLeft", typeof(ScrollBar));
		public static readonly RoutedCommand PageRightCommand = new RoutedCommand("PageRight", typeof(ScrollBar));
		public static readonly RoutedCommand PageUpCommand = new RoutedCommand("PageUp", typeof(ScrollBar));
		public static readonly RoutedCommand ScrollHereCommand = new RoutedCommand("ScrollHere", typeof(ScrollBar));
		public static readonly RoutedCommand ScrollToBottomCommand = new RoutedCommand("ScrollToBottom", typeof(ScrollBar));
		public static readonly RoutedCommand ScrollToEndCommand = new RoutedCommand("ScrollToEnd", typeof(ScrollBar));
		public static readonly RoutedCommand ScrollToHomeCommand = new RoutedCommand("ScrollToHome", typeof(ScrollBar));
		public static readonly RoutedCommand ScrollToHorizontalOffsetCommand = new RoutedCommand("ScrollToHorizontalOffset", typeof(ScrollBar));
		public static readonly RoutedCommand ScrollToLeftEndCommand = new RoutedCommand("ScrollToLeftEnd", typeof(ScrollBar));
		public static readonly RoutedCommand ScrollToRightEndCommand = new RoutedCommand("ScrollToRightEnd", typeof(ScrollBar));
		public static readonly RoutedCommand ScrollToTopCommand = new RoutedCommand("ScrollToTop", typeof(ScrollBar));
		public static readonly RoutedCommand ScrollToVerticalOffsetCommand = new RoutedCommand("ScrollToVerticalOffset", typeof(ScrollBar));
		#endregion
		
		#region Dependency Properties
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ScrollBar));
		public static readonly DependencyProperty ViewportSizeProperty = DependencyProperty.Register("ViewportSize", typeof(double), typeof(ScrollBar));
		#endregion
		
		#region Events
		public static readonly RoutedEvent ScrollEvent = EventManager.RegisterRoutedEvent("Scroll", RoutingStrategy.Bubble, typeof(ScrollEventHandler), typeof(ScrollBar));
		#endregion
		#endregion

		#region Private Fields
		Track track;
		double scroll_here_value;
		static ScrollBarContextMenu context_menu;
		#endregion

		#region Static Constructor
		static ScrollBar() {
			Theme.Load();
			#region Command bindings
			Type type = typeof(ScrollBar);
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(LineDownCommand, ExecuteIncreaseSmall, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(LineLeftCommand, ExecuteDecreaseSmall, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(LineRightCommand, ExecuteIncreaseSmall, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(LineUpCommand, ExecuteDecreaseSmall, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(PageDownCommand, ExecuteIncreaseLarge, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(PageLeftCommand, ExecuteDecreaseLarge, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(PageRightCommand, ExecuteIncreaseLarge, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(PageUpCommand, ExecuteDecreaseLarge, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(ScrollHereCommand, delegate(object sender, ExecutedRoutedEventArgs e) {
				ScrollBar i = (ScrollBar)sender;
				i.Value = i.scroll_here_value;
			}, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(ScrollToBottomCommand, ExecuteSetToMaximum, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(ScrollToEndCommand, ExecuteSetToMaximum, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(ScrollToHomeCommand, ExecuteSetToMinimum, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(ScrollToHorizontalOffsetCommand, ExecuteScrollToOffset, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(ScrollToLeftEndCommand, ExecuteSetToMinimum, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(ScrollToRightEndCommand, ExecuteSetToMaximum, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(ScrollToTopCommand, ExecuteSetToMaximum, True));
			CommandManager.RegisterClassCommandBinding(type, new CommandBinding(ScrollToVerticalOffsetCommand, ExecuteScrollToOffset, True));
			#endregion
		}
		#endregion

		#region Public Constructors
		public ScrollBar() {
			Maximum = 1;
			Orientation = Orientation.Vertical;
			Focusable = false;
		}
		#endregion
		
		#region Public Methods
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			track = (Track)GetTemplateChild("PART_Track");
			if (track == null)
				return;
			track.in_scroll_bar = true;
			//FIXME: Move binding to Track.
			Utility.SetBinding(track, Track.MaximumProperty, this, "Maximum");
			Utility.SetBinding(track, Track.MinimumProperty, this, "Minimum");
			Utility.SetBinding(track, Track.ValueProperty, this, "Value");
			Utility.SetBinding(track, Track.OrientationProperty, this, "Orientation");
			Utility.SetBinding(track, Track.ViewportSizeProperty, this, "ViewportSize");
			track.Thumb.DragDelta += delegate(object sender, DragDeltaEventArgs e) {
				double value_from_distance = track.ValueFromDistance(e.HorizontalChange, e.VerticalChange);
				if (double.IsNaN(value_from_distance))
					return;
				Value += value_from_distance;
			};
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}

		public double ViewportSize {
			get { return (double)GetValue(ViewportSizeProperty); }
			set { SetValue(ViewportSizeProperty, value); }
		}
		#endregion

		public Track Track {
			get { return track; }
		}
		#endregion

		#region Protected Properties
		//FIXME: ScrollBar should be grayed when Maximum equals Minimum.
		protected override bool IsEnabledCore {
			get {
				return base.IsEnabledCore && Maximum > Minimum;
			}
		}
		#endregion

		#region Protected Methods
		protected override void OnContextMenuClosing(ContextMenuEventArgs e) {
			base.OnContextMenuClosing(e);

		}

		protected override void OnContextMenuOpening(ContextMenuEventArgs e) {
			base.OnContextMenuOpening(e);
			//FIXME: If you right-click on the scroll bar when the menu is open, it is shown in the new position. It should happen only after a certain amount of time.
			if (context_menu == null)
				context_menu = new ScrollBarContextMenu();
			context_menu.Owner = this;
			context_menu.IsOpen = true;
		}

		protected override AutomationPeer OnCreateAutomationPeer() {
#if Implementation
			return null;
#else
			return new ScrollBarAutomationPeer(this);
#endif
		}

		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseLeftButtonDown(e);
			if (Keyboard.PrimaryDevice.Modifiers == ModifierKeys.Shift) {
				Value = GetValueFromPoint(e);
				e.Handled = true;
			}
		}

		protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e) {
			base.OnPreviewMouseRightButtonUp(e);
			scroll_here_value = GetValueFromPoint(e);
		}

		#endregion

		#region Public Events
		//FIXME: Raise
		public event ScrollEventHandler Scroll {
			add { AddHandler(ScrollEvent, value); }
			remove { RemoveHandler(ScrollEvent, value); }
		}
		#endregion

		#region Private Methods
		double GetValueFromPoint(MouseButtonEventArgs e) {
			return track.ValueFromPoint(e.MouseDevice.GetPosition(track));
		}

		void DecreaseSmall() {
			Value = Math.Max(Value - SmallChange, Minimum);
		}

		static void ExecuteDecreaseSmall(object sender, ExecutedRoutedEventArgs e) {
			((ScrollBar)sender).DecreaseSmall();		
		}

		void IncreaseSmall() {
			Value = Math.Min(Value + SmallChange, Maximum);
		}

		static void ExecuteIncreaseSmall(object sender, ExecutedRoutedEventArgs e) {
			((ScrollBar)sender).IncreaseSmall();
		}

		void DecreaseLarge() {
			Value = Math.Max(Value - LargeChange, Minimum);
		}

		static void ExecuteDecreaseLarge(object sender, ExecutedRoutedEventArgs e) {
			((ScrollBar)sender).DecreaseLarge();
		}

		void IncreaseLarge() {
			Value = Math.Min(Value + LargeChange, Maximum);
		}

		static void ExecuteIncreaseLarge(object sender, ExecutedRoutedEventArgs e) {
			((ScrollBar)sender).IncreaseLarge();
		}

		void SetToMinimum() {
			Value = Minimum;
		}

		static void ExecuteSetToMinimum(object sender, ExecutedRoutedEventArgs e) {
			((ScrollBar)sender).SetToMinimum();
		}

		void SetToMaximum() {
			Value = Maximum;
		}

		static void ExecuteSetToMaximum(object sender, ExecutedRoutedEventArgs e) {
			((ScrollBar)sender).SetToMaximum();
		}

		static void ExecuteScrollToOffset(object sender, ExecutedRoutedEventArgs e) {
			((ScrollBar)sender).Value = (double)e.Parameter;
		}

		static void True(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}
		#endregion

		#region Private Classes
		class ScrollBarContextMenu : ContextMenu {
			MenuItem scroll_here = new MenuItem();
			MenuItem top_left_edge = new MenuItem();
			MenuItem bottom_right_edge = new MenuItem();
			MenuItem page_up_left = new MenuItem();
			MenuItem page_down_right = new MenuItem();
			MenuItem scroll_up_left = new MenuItem();
			MenuItem scroll_down_right = new MenuItem();

			public ScrollBarContextMenu() {
				scroll_here.Header = "Scroll Here";
				scroll_here.Command = ScrollHereCommand;
				Items.Add(scroll_here);
				Items.Add(new Separator());
				Items.Add(top_left_edge);
				Items.Add(bottom_right_edge);
				Items.Add(new Separator());
				Items.Add(page_up_left);
				Items.Add(page_down_right);
				Items.Add(new Separator());
				Items.Add(scroll_up_left);
				Items.Add(scroll_down_right);
			}
			
			public ScrollBar Owner {
				set {
					scroll_here.CommandTarget = value;
					top_left_edge.CommandTarget = value;
					bottom_right_edge.CommandTarget = value;
					page_up_left.CommandTarget = value;
					page_down_right.CommandTarget = value;
					scroll_up_left.CommandTarget = value;
					scroll_down_right.CommandTarget = value;
					if (value.Orientation == Orientation.Vertical) {
						top_left_edge.Header = "Top";
						bottom_right_edge.Header = "Bottom";
						page_up_left.Header = "Page Up";
						page_down_right.Header = "Page Down";
						scroll_up_left.Header = "Scroll Up";
						scroll_down_right.Header = "Scroll Down";

						top_left_edge.Command = ScrollToTopCommand;
						bottom_right_edge.Command = ScrollToBottomCommand;
						page_up_left.Command = PageUpCommand;
						page_down_right.Command = PageDownCommand;
						scroll_up_left.Command = LineUpCommand;
						scroll_down_right.Command = LineDownCommand;
					} else {
						top_left_edge.Header = "Left Edge";
						bottom_right_edge.Header = "Right Edge";
						page_up_left.Header = "Page Left";
						page_down_right.Header = "Page Right";
						scroll_up_left.Header = "Scroll Left";
						scroll_down_right.Header = "Scroll Right";
						
						top_left_edge.Command = ScrollToLeftEndCommand;
						bottom_right_edge.Command = ScrollToRightEndCommand;
						page_up_left.Command = PageLeftCommand;
						page_down_right.Command = PageRightCommand;
						scroll_up_left.Command = LineLeftCommand;
						scroll_down_right.Command = LineRightCommand;
					}
				}
			}
		}
		#endregion
	}
}