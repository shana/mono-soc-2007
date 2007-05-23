using NUnit.Framework;
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class BooleanToVisibilityConverterTest {
		[Test]
		public void Test() {
			BooleanToVisibilityConverter c = new BooleanToVisibilityConverter();
			Assert.AreEqual(c.Convert(true, null, null, null), Visibility.Visible, "true");
			Assert.AreEqual(c.Convert(false, null, null, null), Visibility.Collapsed, "false");
			Assert.AreEqual(c.Convert(null, null, null, null), Visibility.Collapsed, "null");
			Assert.AreEqual(c.Convert((bool?)true, null, null, null), Visibility.Visible, "(bool?)true");
			Assert.AreEqual(c.Convert((bool?)false, null, null, null), Visibility.Collapsed, "(bool?)false");
			Assert.AreEqual(c.Convert((bool?)null, null, null, null), Visibility.Collapsed, "(bool?)null");
			Assert.AreEqual(c.Convert(string.Empty, null, null, null), Visibility.Collapsed, "string.Empty");

			Assert.IsTrue((bool)c.ConvertBack(Visibility.Visible, null, null, null), "Visibility.Visible");
			Assert.IsFalse((bool)c.ConvertBack(Visibility.Hidden, null, null, null), "Visibility.Hidden");
			Assert.IsFalse((bool)c.ConvertBack(Visibility.Collapsed, null, null, null), "Visibility.Collapsed");
			Assert.IsFalse((bool)c.ConvertBack(null, null, null, null), "null");
		}
	}
}