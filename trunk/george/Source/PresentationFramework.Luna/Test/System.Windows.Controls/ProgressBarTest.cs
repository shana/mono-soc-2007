using NUnit.Framework;
using System.Windows.Shapes;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class ProgressBarTest {
		#region Sizing
		[Test]
		public void Sizing() {
			new SizingProgressBar();
		}

		class SizingProgressBar : ProgressBar {
			public SizingProgressBar() {
				Window w = new Window();
				w.Content = this;
				w.Show();
				FrameworkElement track = (FrameworkElement)GetTemplateChild("PART_Track");
				FrameworkElement indicator = (FrameworkElement)GetTemplateChild("PART_Indicator");
				Value = 30;
				Assert.IsTrue(Utility.AreCloseEnough(indicator.Width, track.ActualWidth * Value / Maximum), "Value");
				Minimum = 10;
				Assert.IsTrue(Utility.AreCloseEnough(indicator.Width, track.ActualWidth * Value / Maximum), "Minimum");
			}
		}
		#endregion
	}
}