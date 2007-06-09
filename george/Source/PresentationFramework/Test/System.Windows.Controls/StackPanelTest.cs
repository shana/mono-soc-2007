using NUnit.Framework;
using System.Windows.Media;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class StackPanelTest {
		[Test]
		public void DefaultValueIsStretchForAlignment() {
			StackPanel stack_panel = new StackPanel();
			Assert.AreEqual(stack_panel.HorizontalAlignment, HorizontalAlignment.Stretch, "1");
			Assert.AreEqual(stack_panel.VerticalAlignment, VerticalAlignment.Stretch, "2");
			Button button = new Button();
			button.HorizontalAlignment = HorizontalAlignment.Left;
			stack_panel.Children.Add(button);
			Assert.AreEqual(button.HorizontalAlignment, HorizontalAlignment.Left, "3");
		}

		[Test]
		public void Focusable() {
			Assert.IsFalse(new StackPanel().Focusable);
		}

		#region HasLogicalOrientation
		[Test]
		public void HasLogicalOrientation() {
			new HasLogicalOrientationStackPanel();
		}

		class HasLogicalOrientationStackPanel : StackPanel {
			public HasLogicalOrientationStackPanel() {
				Assert.IsTrue(HasLogicalOrientation);
			}
		}
		#endregion
		
		#region LogicalOrientation
		[Test]
		public void LogicalOrientation() {
			new LogicalOrientationStackPanel();
		}

		class LogicalOrientationStackPanel : StackPanel {
			public LogicalOrientationStackPanel() {
				Assert.AreEqual(Orientation, Orientation.Vertical, "1");
				Assert.AreEqual(LogicalOrientation, Orientation.Vertical, "2");
				Orientation = Orientation.Horizontal;
				Assert.AreEqual(LogicalOrientation, Orientation.Horizontal, "3");
			}
		}
		#endregion

		#region Measure
		[Test]
		public void Measure() {
			new MeasureStackPanel();
		}

		class MeasureStackPanel : StackPanel {
			static bool called;
			static Size measure_constraint;

			public MeasureStackPanel() {
				Size result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 0, "1");
				Assert.AreEqual(result.Height, 0, "2");
				TestButton button = new TestButton();
				Children.Add(button);
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, Utility.GetEmptyButtonSize(), "3");
				Assert.AreEqual(result.Height, Utility.GetEmptyButtonSize(), "4");
				Assert.IsTrue(called, "5");
				called = false;
				Assert.IsTrue(double.IsPositiveInfinity(measure_constraint.Width), "6");
				Assert.IsTrue(double.IsPositiveInfinity(measure_constraint.Height), "7");
			}
			
			class TestButton : global::System.Windows.Controls.Button {
				protected override Size MeasureOverride(Size constraint) {
					called = true;
					return base.MeasureOverride(measure_constraint = constraint);
				}
			}
		}
		#endregion

		#region Alignment
		[Test]
		public void Alignment() {
			new AlignmentStackPanel();
		}

		class AlignmentStackPanel : StackPanel {
			public AlignmentStackPanel() {
				Window window = new Window();
				global::System.Windows.Controls.DockPanel dock_panel = new global::System.Windows.Controls.DockPanel();
				window.Content = dock_panel;
				window.Show();
				dock_panel.Children.Add(this);
				HorizontalAlignment = HorizontalAlignment.Center;
				Size result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 0, "1");
				Assert.AreEqual(result.Height, 0, "2");
				Children.Add(new global::System.Windows.Controls.Button());
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, Utility.GetEmptyButtonSize(), "3");
				Assert.AreEqual(result.Height, Utility.GetEmptyButtonSize(), "4");
				//FIXME: Different results in Windows XP and Windows Vista.
				Assert.AreEqual(ActualWidth, Utility.GetEmptyButtonSize(), "5");
				Assert.AreEqual(ActualHeight, dock_panel.ActualHeight, "6");
			}
		}
		#endregion

		[Test]
		public void AlignmentMetadata() {
			PropertyMetadata panel_metadata = FrameworkElement.HorizontalAlignmentProperty.GetMetadata(typeof(Panel));
			PropertyMetadata stack_panel_metadata = FrameworkElement.HorizontalAlignmentProperty.GetMetadata(typeof(StackPanel));
			Assert.AreSame(panel_metadata, stack_panel_metadata);
		}

		[Test]
		public void ClipToBounds() {
			Assert.IsFalse(new StackPanel().ClipToBounds);
		}

		[Test]
		public void InScrollViewer() {
			Window window = new Window();
			ScrollViewer scroll_viewer = new ScrollViewer();
			window.Content = scroll_viewer;
			StackPanel stack_panel = new StackPanel();
			scroll_viewer.Content = stack_panel;
			global::System.Windows.Controls.Button button = new global::System.Windows.Controls.Button();
			stack_panel.Children.Add(button);
			window.Show();
			Assert.AreEqual(button.ActualWidth, scroll_viewer.ViewportWidth, "1");
			Assert.AreEqual(button.ActualWidth, stack_panel.ActualWidth, "2");
		}

		[Test]
		public void InScrollViewerCanContentScroll() {
			Window window = new Window();
			ScrollViewer scroll_viewer = new ScrollViewer();
			scroll_viewer.CanContentScroll = true;
			window.Content = scroll_viewer;
			StackPanel stack_panel = new StackPanel();
			scroll_viewer.Content = stack_panel;
			global::System.Windows.Controls.Button button = new global::System.Windows.Controls.Button();
			stack_panel.Children.Add(button);
			window.Show();
			Assert.AreEqual(button.ActualWidth, scroll_viewer.ViewportWidth, "1");
			Assert.AreEqual(button.ActualWidth, stack_panel.ActualWidth, "2");
		}

		[Test]
		public void NotInScrollViewer() {
			Window window = new Window();
			StackPanel stack_panel = new StackPanel();
			window.Content = stack_panel;
			global::System.Windows.Controls.Button button = new global::System.Windows.Controls.Button();
			stack_panel.Children.Add(button);
			window.Show();
			Assert.AreEqual(button.ActualWidth, stack_panel.ActualWidth);
		}

		#region SizeInScrollViewer
		[Test]
		public void SizeInScrollViewer() {
			Window window = new Window();
			ScrollViewer scroll_viewer = new ScrollViewer();
			window.Content = scroll_viewer;
			SizeInScrollViewerStackPanel stack_panel = new SizeInScrollViewerStackPanel();
			scroll_viewer.Content = stack_panel;
			global::System.Windows.Controls.Button button = new global::System.Windows.Controls.Button();
			stack_panel.Children.Add(button);
			window.Show();
			Assert.AreEqual(stack_panel.ActualWidth, scroll_viewer.ViewportWidth, "1");
			Assert.AreEqual(stack_panel.ActualHeight, scroll_viewer.ViewportHeight, "2");
			Assert.AreEqual(stack_panel.DesiredSize.Width, Utility.GetEmptyButtonSize(), "3");
			Assert.AreEqual(stack_panel.DesiredSize.Height, Utility.GetEmptyButtonSize(), "4");
			Assert.AreEqual(stack_panel.MeasureResult.Width, Utility.GetEmptyButtonSize(), "5");
			Assert.AreEqual(stack_panel.MeasureResult.Height, Utility.GetEmptyButtonSize(), "6");
		}

		class SizeInScrollViewerStackPanel : StackPanel {
			public Size MeasureResult;
			protected override Size MeasureOverride(Size availableSize) {
				return MeasureResult = base.MeasureOverride(availableSize);
			}
		}
		#endregion

		#region SizeInScrollViewerCanContentScroll
		[Test]
		public void SizeInScrollViewerCanContentScroll() {
			Window window = new Window();
			ScrollViewer scroll_viewer = new ScrollViewer();
			scroll_viewer.CanContentScroll = true;
			window.Content = scroll_viewer;
			SizeInScrollViewerCanContentScrollStackPanel stack_panel = new SizeInScrollViewerCanContentScrollStackPanel();
			scroll_viewer.Content = stack_panel;
			global::System.Windows.Controls.Button button = new global::System.Windows.Controls.Button();
			stack_panel.Children.Add(button);
			window.Show();
			Assert.AreEqual(stack_panel.ActualWidth, scroll_viewer.ViewportWidth, "1");
			Assert.AreEqual(stack_panel.ActualHeight, scroll_viewer.ActualHeight, "2");
			Assert.AreEqual(stack_panel.DesiredSize.Width, Utility.GetEmptyButtonSize(), "3");
			Assert.AreEqual(stack_panel.DesiredSize.Height, Utility.GetEmptyButtonSize(), "4");
			Assert.AreEqual(stack_panel.MeasureResult.Width, Utility.GetEmptyButtonSize(), "5");
			Assert.AreEqual(stack_panel.MeasureResult.Height, Utility.GetEmptyButtonSize(), "6");
		}

		class SizeInScrollViewerCanContentScrollStackPanel : StackPanel {
			public Size MeasureResult;
			protected override Size MeasureOverride(Size availableSize) {
				return MeasureResult = base.MeasureOverride(availableSize);
			}
		}
		#endregion

		[Test]
		public void CanScroll() {
			StackPanel p = new StackPanel();
			Assert.IsFalse(p.CanHorizontallyScroll, "1");
			Assert.IsFalse(p.CanVerticallyScroll, "2");
			p.Orientation = Orientation.Horizontal;
			Assert.IsFalse(p.CanHorizontallyScroll, "3");
			Assert.IsFalse(p.CanVerticallyScroll, "4");
			p.ScrollOwner = new ScrollViewer();
			p.Orientation = Orientation.Vertical;
			Assert.IsFalse(p.CanHorizontallyScroll, "5");
			Assert.IsFalse(p.CanVerticallyScroll, "6");
			p.Orientation = Orientation.Horizontal;
			Assert.IsFalse(p.CanHorizontallyScroll, "7");
			Assert.IsFalse(p.CanVerticallyScroll, "8");
			for (int i = 0; i < 10; i++)
				p.Children.Add(new global::System.Windows.Controls.Button());
			p.Orientation = Orientation.Vertical;
			Assert.IsFalse(p.CanHorizontallyScroll, "9");
			Assert.IsFalse(p.CanVerticallyScroll, "10");
			p.Orientation = Orientation.Horizontal;
			Assert.IsFalse(p.CanHorizontallyScroll, "11");
			Assert.IsFalse(p.CanVerticallyScroll, "12");
			p.ScrollOwner = null;
			ScrollViewer v = new ScrollViewer();
			v.Content = p;
			p.Orientation = Orientation.Vertical;
			Assert.IsFalse(p.CanHorizontallyScroll, "13");
			Assert.IsFalse(p.CanVerticallyScroll, "14");
			p.Orientation = Orientation.Horizontal;
			Assert.IsFalse(p.CanHorizontallyScroll, "15");
			Assert.IsFalse(p.CanVerticallyScroll, "16");
			Window w = new Window();
			w.Content = v;
			w.Width = 100;
			w.Height = 100;
			w.Show();
			p.Orientation = Orientation.Vertical;
			Assert.IsFalse(p.CanHorizontallyScroll, "17");
			Assert.IsFalse(p.CanVerticallyScroll, "18");
			p.Orientation = Orientation.Horizontal;
			Assert.IsFalse(p.CanHorizontallyScroll, "19");
			Assert.IsFalse(p.CanVerticallyScroll, "20");
			v.CanContentScroll = true;
			p.Orientation = Orientation.Vertical;
			Assert.IsFalse(p.CanHorizontallyScroll, "21");
			Assert.IsTrue(p.CanVerticallyScroll, "22");
			p.Orientation = Orientation.Horizontal;
			Assert.IsFalse(p.CanHorizontallyScroll, "23");
			Assert.IsTrue(p.CanVerticallyScroll, "24");
		}

		[Test]
		public void Extent() {
			StackPanel p = new StackPanel();
			Assert.AreEqual(p.ExtentWidth, 0, "1");
			Assert.AreEqual(p.ExtentHeight, 0, "2");
			p.Children.Add(new global::System.Windows.Controls.Button());
			Assert.AreEqual(p.ExtentWidth, 0, "3");
			Assert.AreEqual(p.ExtentHeight, 0, "4");
			Window w = new Window();
			w.Content = p;
			w.Show();
			Assert.AreEqual(p.ExtentWidth, 0, "3");
			Assert.AreEqual(p.ExtentHeight, 0, "4");
			ScrollViewer v = new ScrollViewer();
			w.Content = v;
			v.Content = p;
			Assert.AreEqual(p.ExtentWidth, 0, "5");
			Assert.AreEqual(p.ExtentHeight, 0, "6");
			v.CanContentScroll = true;
			Assert.AreEqual(p.ExtentWidth, 0, "7");
			Assert.AreEqual(p.ExtentHeight, 0, "8");
		}

		[Test]
		public void ExtentSimple() {
			Window w = new Window();
			ScrollViewer v = new ScrollViewer();
			w.Content = v;
			StackPanel p = new StackPanel();
			v.Content = p;
			global::System.Windows.Controls.Button b = new global::System.Windows.Controls.Button();
			p.Children.Add(b);
			w.Show();
			Assert.AreEqual(p.ExtentWidth, 0, "1");
			Assert.AreEqual(p.ExtentHeight, 0, "2");
		}

		[Test]
		public void ExtentSimpleCanContentScroll() {
			Window w = new Window();
			ScrollViewer v = new ScrollViewer();
			v.CanContentScroll = true;
			w.Content = v;
			StackPanel p = new StackPanel();
			v.Content = p;
			global::System.Windows.Controls.Button b = new global::System.Windows.Controls.Button();
			p.Children.Add(b);
			w.Show();
			Assert.AreEqual(p.ExtentWidth, Utility.GetEmptyButtonSize(), "1");
			Assert.AreEqual(p.ExtentHeight, 1, "2");
			p.Orientation = Orientation.Horizontal;
			Assert.AreEqual(p.ExtentWidth, 0, "3");
			Assert.AreEqual(p.ExtentHeight, 0, "4");
			w.Hide();
			w.Show();
			Assert.AreEqual(p.ExtentWidth, 0, "5");
			Assert.AreEqual(p.ExtentHeight, 0, "6");
		}

		[Test]
		public void ExtentSimpleCanContentScrollHorizontal() {
			Window w = new Window();
			ScrollViewer v = new ScrollViewer();
			v.CanContentScroll = true;
			w.Content = v;
			StackPanel p = new StackPanel();
			p.Orientation = Orientation.Horizontal;
			v.Content = p;
			global::System.Windows.Controls.Button b = new global::System.Windows.Controls.Button();
			p.Children.Add(b);
			w.Show();
			Assert.AreEqual(p.ExtentWidth, 1, "1");
			Assert.AreEqual(p.ExtentHeight, Utility.GetEmptyButtonSize(), "2");
			p.Orientation = Orientation.Vertical;
			Assert.AreEqual(p.ExtentWidth, 0, "3");
			Assert.AreEqual(p.ExtentHeight, 0, "4");
			w.Hide();
			w.Show();
			Assert.AreEqual(p.ExtentWidth, 0, "5");
			Assert.AreEqual(p.ExtentHeight, 0, "6");
		}

		#region ExtentCallsMeasureOnChildren
		[Test]
		public void ExtentCallsMeasureOnChildren() {
			StackPanel p = new StackPanel();
			p.Children.Add(new ExtentCallsMeasureOnChildrenButton());
			ExtentCallsMeasureOnChildrenButton.ShouldSetCalled = true;
			object dummy = p.ExtentWidth;
			Assert.IsFalse(ExtentCallsMeasureOnChildrenButton.Called);
		}

		class ExtentCallsMeasureOnChildrenButton : global::System.Windows.Controls.Button {
			public static bool ShouldSetCalled;
			public static bool Called;
			protected override Size MeasureOverride(Size constraint) {
				if (ShouldSetCalled)
					Called = true;
				return base.MeasureOverride(constraint);
			}
		}
		#endregion

		[Test]
		public void Offset() {
			StackPanel p = new StackPanel();
			Assert.AreEqual(p.HorizontalOffset, 0, "1");
			Assert.AreEqual(p.VerticalOffset, 0, "2");
			p.SetHorizontalOffset(1);
			Assert.AreEqual(p.HorizontalOffset, 0, "3");
			p.SetVerticalOffset(1);
			Assert.AreEqual(p.VerticalOffset, 0, "4");
			p.SetHorizontalOffset(-1);
			Assert.AreEqual(p.HorizontalOffset, 0, "5");
			p.SetVerticalOffset(-1);
			Assert.AreEqual(p.VerticalOffset, 0, "6");
		}
		
		[Test]
		public void OffsetInScrollViewer() {
			ScrollViewer v = new ScrollViewer();
			StackPanel p = new StackPanel();
			v.Content = p;
			Assert.AreEqual(p.HorizontalOffset, 0, "1");
			Assert.AreEqual(p.VerticalOffset, 0, "2");
			p.SetHorizontalOffset(1);
			Assert.AreEqual(p.HorizontalOffset, 0, "3");
			p.SetVerticalOffset(1);
			Assert.AreEqual(p.VerticalOffset, 0, "4");
			p.SetHorizontalOffset(-1);
			Assert.AreEqual(p.HorizontalOffset, 0, "5");
			p.SetVerticalOffset(-1);
			Assert.AreEqual(p.VerticalOffset, 0, "6");
		}

		[Test]
		public void OffsetInScrollViewerCanContentScroll() {
			ScrollViewer v = new ScrollViewer();
			v.CanContentScroll = true;
			StackPanel p = new StackPanel();
			v.Content = p;
			Assert.AreEqual(p.HorizontalOffset, 0, "1");
			Assert.AreEqual(p.VerticalOffset, 0, "2");
			p.SetHorizontalOffset(1);
			Assert.AreEqual(p.HorizontalOffset, 0, "3");
			p.SetVerticalOffset(1);
			Assert.AreEqual(p.VerticalOffset, 0, "4");
			p.SetHorizontalOffset(-1);
			Assert.AreEqual(p.HorizontalOffset, 0, "5");
			p.SetVerticalOffset(-1);
			Assert.AreEqual(p.VerticalOffset, 0, "6");
		}
		
		[Test]
		public void OffsetInScrollViewerCanContentScrollInWindow() {
			Window w = new Window();
			ScrollViewer v = new ScrollViewer();
			w.Content = v;
			w.Show();
			v.CanContentScroll = true;
			StackPanel p = new StackPanel();
			v.Content = p;
			Assert.AreEqual(p.HorizontalOffset, 0, "1");
			Assert.AreEqual(p.VerticalOffset, 0, "2");
			p.SetHorizontalOffset(1);
			Assert.AreEqual(p.HorizontalOffset, 0, "3");
			p.SetVerticalOffset(1);
			Assert.AreEqual(p.VerticalOffset, 0, "4");
			p.SetHorizontalOffset(-1);
			Assert.AreEqual(p.HorizontalOffset, 0, "5");
			p.SetVerticalOffset(-1);
			Assert.AreEqual(p.VerticalOffset, 0, "6");
		}

		[Test]
		public void OffsetInScrollViewerCanContentScrollInWindowWithChildren() {
			Window w = new Window();
			ScrollViewer v = new ScrollViewer();
			w.Content = v;
			w.Show();
			v.CanContentScroll = true;
			StackPanel p = new StackPanel();
			w.Width = 100;
			w.Height = 100;
			for (int i = 0; i < 10; i++) {
				global::System.Windows.Controls.Button b = new global::System.Windows.Controls.Button();
				b.Content = "Test";
				p.Children.Add(b);
			}
			v.Content = p;
			Assert.AreEqual(p.HorizontalOffset, 0, "1");
			Assert.AreEqual(p.VerticalOffset, 0, "2");
			p.SetHorizontalOffset(1);
			Assert.AreEqual(p.HorizontalOffset, 0, "3");
			p.SetVerticalOffset(1);
			Assert.AreEqual(p.VerticalOffset, 0, "4");
			p.SetHorizontalOffset(-1);
			Assert.AreEqual(p.HorizontalOffset, 0, "5");
			p.SetVerticalOffset(-1);
			Assert.AreEqual(p.VerticalOffset, 0, "6");
		}

		[Test]
		public void ViewportInitialValue() {
			StackPanel p = new StackPanel();
			Assert.AreEqual(p.ViewportWidth, 0, "1");
			Assert.AreEqual(p.ViewportHeight, 0, "2");
		}
		
		[Test]
		public void Viewport1() {
			Window w = new Window();
			ScrollViewer v = new ScrollViewer();
			w.Content = v;
			w.Show();
			v.CanContentScroll = true;
			StackPanel p = new StackPanel();
			v.Content = p;
			p.Children.Add(new global::System.Windows.Controls.Button());
			Assert.AreEqual(p.ViewportWidth, 0, "1");
			Assert.AreEqual(p.ViewportHeight, 0, "1");
		}
		
		[Test]
		public void Viewport2() {
			Window w = new Window();
			ScrollViewer v = new ScrollViewer();
			w.Content = v;
			v.CanContentScroll = true;
			StackPanel p = new StackPanel();
			v.Content = p;
			p.Children.Add(new global::System.Windows.Controls.Button());
			w.Show();
			Assert.AreEqual(p.ViewportWidth, v.ActualWidth - SystemParameters.VerticalScrollBarWidth, "1");
			Assert.AreEqual(p.ViewportHeight, 1, "2");
			w.Width /= 2;
			w.Height /= 2;
			Assert.AreEqual(p.ViewportWidth, v.ActualWidth - SystemParameters.VerticalScrollBarWidth, "3");
			Assert.AreEqual(p.ViewportHeight, 1, "4");
			p.Orientation = Orientation.Horizontal;
			Assert.AreEqual(p.ViewportWidth, 0, "5");
			Assert.AreEqual(p.ViewportHeight, 0, "6");
		}

		[Test]
		public void RenderTransform() {
			Window w = new Window();
			ScrollViewer v = new ScrollViewer();
			w.Content = v;
			v.CanContentScroll = true;
			StackPanel p = new StackPanel();
			v.Content = p;
			p.Children.Add(new global::System.Windows.Controls.Button());
			w.Show();
			Assert.AreEqual(p.RenderTransform, TranslateTransform.Identity);
		}

		[Test]
		public void Stretch() {
			Window window = new Window();
			window.Show();
			Grid grid = new Grid();
			window.Content = grid;
			StackPanel stack_panel = new StackPanel();
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.Children.Add(stack_panel);
			Assert.AreEqual(stack_panel.ActualWidth, grid.ActualWidth / 2);
		}

		[Test]
		public void Stretch2() {
			Window window = new Window();
			window.Show();
			Grid grid = new Grid();
			window.Content = grid;
			StackPanel stack_panel = new StackPanel();
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.RowDefinitions.Add(new RowDefinition());
			grid.RowDefinitions.Add(new RowDefinition());
			grid.Children.Add(new global::System.Windows.Controls.Button());
			grid.Children.Add(stack_panel);
			Grid.SetRow(stack_panel, 1);
			Grid.SetRow(stack_panel, 1);
			Assert.AreEqual(stack_panel.ActualWidth, grid.ActualWidth / 2);
		}

		[Test]
		public void MakeVisible() {
			StackPanel p = new StackPanel();
			Rect result = p.MakeVisible(null, Rect.Empty);
			Assert.AreEqual(p.VerticalOffset, 0, "1 1");
			Assert.IsTrue(double.IsNegativeInfinity(result.Width), "1");
			Assert.IsTrue(double.IsNegativeInfinity(result.Height), "2");
			result = p.MakeVisible(null, new Rect(100, 100, 100, 100));
			Assert.AreEqual(p.VerticalOffset, 0, "3 1");
			Assert.IsTrue(double.IsNegativeInfinity(result.Width), "3");
			Assert.IsTrue(double.IsNegativeInfinity(result.Height), "4");
			global::System.Windows.Controls.Button b = new global::System.Windows.Controls.Button();
			p.Children.Add(b);
			result = p.MakeVisible(b, Rect.Empty);
			Assert.AreEqual(p.VerticalOffset, 0, "5 1");
			Assert.IsTrue(double.IsNegativeInfinity(result.Width), "5");
			Assert.IsTrue(double.IsNegativeInfinity(result.Height), "6");
			ScrollViewer v = new ScrollViewer();
			v.Content = p;
			result = p.MakeVisible(null, Rect.Empty);
			Assert.AreEqual(p.VerticalOffset, 0, "7 1");
			Assert.IsTrue(double.IsNegativeInfinity(result.Width), "7");
			Assert.IsTrue(double.IsNegativeInfinity(result.Height), "8");
			result = p.MakeVisible(b, Rect.Empty);
			Assert.AreEqual(p.VerticalOffset, 0, "9 1");
			Assert.IsTrue(double.IsNegativeInfinity(result.Width), "9");
			Assert.IsTrue(double.IsNegativeInfinity(result.Height), "10");
			v.CanContentScroll = true;
			result = p.MakeVisible(null, Rect.Empty);
			Assert.AreEqual(p.VerticalOffset, 0, "11 1");
			Assert.IsTrue(double.IsNegativeInfinity(result.Width), "11");
			Assert.IsTrue(double.IsNegativeInfinity(result.Height), "12");
			result = p.MakeVisible(b, Rect.Empty);
			Assert.AreEqual(p.VerticalOffset, 0, "13 1");
			Assert.IsTrue(double.IsNegativeInfinity(result.Width), "13");
			Assert.IsTrue(double.IsNegativeInfinity(result.Height), "14");
			Window w = new Window();
			w.Content = v;
			w.Show();
			result = p.MakeVisible(null, Rect.Empty);
			Assert.AreEqual(p.VerticalOffset, 0, "15 1");
			Assert.IsTrue(double.IsNegativeInfinity(result.Width), "15");
			Assert.IsTrue(double.IsNegativeInfinity(result.Height), "16");
			result = p.MakeVisible(b, Rect.Empty);
			Assert.AreEqual(p.VerticalOffset, 0, "17 1");
			Assert.IsTrue(double.IsNegativeInfinity(result.Width), "17");
			Assert.IsTrue(double.IsNegativeInfinity(result.Height), "18");
		}
	}
}