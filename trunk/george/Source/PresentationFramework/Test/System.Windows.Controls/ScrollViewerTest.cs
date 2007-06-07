using NUnit.Framework;
namespace System.Windows.Controls {
	[TestFixture]
	public class ScrollViewerTest {
		[Test]
		public void CanContentScrollDefaultValue() {
			//LAMESPEC: Windows SDK Feb 2007.
			Assert.IsFalse(new ScrollViewer().CanContentScroll);
		}
	}
}