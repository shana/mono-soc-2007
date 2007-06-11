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
	public class WrapPanelTest {
		[Test]
		public void VeryHighChild() {
			Window window = new Window();
			Canvas canvas = new Canvas();
			window.Content = canvas;
			WrapPanel wrap_panel = new WrapPanel();
			canvas.Children.Add(wrap_panel);
			wrap_panel.Width = 100;
			wrap_panel.Height = 100;
			Button button1 = new Button();
			button1.Height = 200;
			wrap_panel.Children.Add(button1);
			Button button2 = new Button();
			wrap_panel.Children.Add(button2);
			window.Show();
			Assert.AreEqual(button1.ActualHeight, 200, "1");
			Assert.AreEqual(button2.ActualHeight, 100, "2");
		}
	}
}