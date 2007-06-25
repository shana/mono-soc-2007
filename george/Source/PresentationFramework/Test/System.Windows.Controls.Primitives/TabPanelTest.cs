using NUnit.Framework;
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
	[TestFixture]
	public class TabPanelTest {
		#region MeasureAutoEmpty
		[Test]
		public void MeasureAutoEmpty() {
			new MeasureAutoEmptyTabPanel();
		}

		class MeasureAutoEmptyTabPanel : TabPanel {
			public MeasureAutoEmptyTabPanel() {
				Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(DesiredSize.Width, 0, "1");
				Assert.AreEqual(DesiredSize.Height, 0, "2");
			}
		}
		#endregion

		#region MeasureEmpty
		[Test]
		public void MeasureEmpty() {
			new MeasureEmptyTabPanel();
		}

		class MeasureEmptyTabPanel : TabPanel {
			public MeasureEmptyTabPanel() {
				Measure(new Size(100, 100));
				Assert.AreEqual(DesiredSize.Width, 0, "1");
				Assert.AreEqual(DesiredSize.Height, 0, "2");
			}
		}
		#endregion

		#region GetLayoutClip
		[Test]
		public void GetLayoutClip() {
			new GetLayoutClipTabPanel();
		}

		class GetLayoutClipTabPanel : TabPanel {
			public GetLayoutClipTabPanel() {
				Assert.IsNull(GetLayoutClip(new Size(double.PositiveInfinity, double.PositiveInfinity)), "1");
				Assert.IsNull(GetLayoutClip(new Size(100, 100)), "2");
				Children.Add(new TabItem());
				Assert.IsNull(GetLayoutClip(new Size(double.PositiveInfinity, double.PositiveInfinity)), "3");
				Assert.IsNull(GetLayoutClip(new Size(100, 100)), "4");
			}
		}
		#endregion

		#region DockLeft
		[Test]
		public void DockLeft() {
			DockLeftTabControl t = new DockLeftTabControl();

			TabItem tab_item1 = new TabItem();
			tab_item1.Header = "H1";
			t.Items.Add(tab_item1);

			TabItem tab_item2 = new TabItem();
			tab_item2.Header = "H2...........";
			t.Items.Add(tab_item2);

			t.TabStripPlacement = Dock.Left;
			Window w = new Window();
			w.Content = t;
			w.Show();
			double width = tab_item2.ActualWidth;
			double header_panel_width = t.GetHeaderPanel().ActualWidth;
			tab_item2.IsSelected = true;
			Assert.AreEqual(tab_item2.ActualWidth, width, "1");
			Assert.AreEqual(t.GetHeaderPanel().ActualWidth, header_panel_width, "2");
			Assert.AreEqual(header_panel_width, width, "3");
		}

		class DockLeftTabControl : TabControl {
			public TabPanel GetHeaderPanel() {
				return (TabPanel)GetTemplateChild("HeaderPanel");
			}
		}
		#endregion
	}
}