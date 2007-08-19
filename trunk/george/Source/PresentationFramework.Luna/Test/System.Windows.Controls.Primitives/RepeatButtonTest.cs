using NUnit.Framework;
#if Implementation
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives
{
#else
namespace System.Windows.Controls.Primitives {
#endif
	[TestFixture]
	public class RepeatButtonTest
	{
		[Test]
		public void Creation ()
		{
			RepeatButton repeat_button = new RepeatButton ();
			Assert.AreEqual (repeat_button.ActualWidth, 0, "repeat_button.ActualWidth");
			Assert.AreEqual (repeat_button.ActualHeight, 0, "repeat_button.ActualHeight");
		}
	}
}