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
	public class ThumbTest {
		[Test]
#if Implementation
		[Ignore]
#endif
		public void Creation() {
			Thumb t = new Thumb();
			Assert.AreEqual(t.Background, null, "Background");
			Assert.AreEqual(((SolidColorBrush)t.Foreground).Color.A, 0xFF, "Foreground");
		}

		[Test]
#if Implementation
		[Ignore]
#endif
		public void Style() {
			Thumb t = new Thumb();
			Assert.IsNull(t.Style, "Style");
		}
    }
}