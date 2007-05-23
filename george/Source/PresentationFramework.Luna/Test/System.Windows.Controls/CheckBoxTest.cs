using NUnit.Framework;
using System.Windows.Automation.Peers;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#if Implementation
using Mono.Microsoft.Windows.Themes;
using System;
using System.Windows;
using System.Windows.Controls;
using Mono.System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls {
#else
using Microsoft.Windows.Themes;
using System.Windows.Controls.Primitives;
namespace System.Windows.Controls {
#endif
    [TestFixture]
	public class CheckBoxTest {
		[Test]
		public void Layout() {
			TextBlock tall_text = new TextBlock();
			tall_text.Text = "1" + Environment.NewLine + "2";

			CheckBox check_box = new CheckBox();
			check_box.Content = tall_text;
			
			check_box.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

			UIElement bullet = (UIElement)VisualTreeHelper.GetChild(check_box, 0);

			Point location = bullet.TranslatePoint(new Point(0, 0), check_box);

			Assert.AreEqual(location.X, 0, "Bullet X");
			Assert.AreEqual(location.Y, 0, "Bullet Y");
		}
	}
}