using NUnit.Framework;
using System.Windows.Media;
#if Implementation
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class BorderGapMaskConverterTest {
		[Test]
		public void Test() {
			Assert.AreEqual(new BorderGapMaskConverter().Convert(new object[] { 100, 500, 1000 }, null, 7, null), DependencyProperty.UnsetValue, "1");
			Assert.AreEqual(new BorderGapMaskConverter().Convert(new object[] { 100D, 500D, 1000D }, null, 7, null), DependencyProperty.UnsetValue, "2");
			VisualBrush result = (VisualBrush) new BorderGapMaskConverter().Convert(new object[] { 100D, 500D, 1000D }, null, 7D, null);
			Grid grid = (Grid)result.Visual;
			Assert.AreEqual(grid.Children.Count, 3, "3");
		}
	}
}