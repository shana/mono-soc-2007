using NUnit.Framework;
using System.Collections;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class HeaderedContentControlTest {
		#region LogicalChildren
		[Test]
		public void LogicalChildren() {
			new LogicalChildrenHeaderedContentControl();
		}

		class LogicalChildrenHeaderedContentControl : HeaderedContentControl {
			public LogicalChildrenHeaderedContentControl() {
				IEnumerator c = LogicalChildren;
				//LAMESPEC
				Assert.IsNotNull(c, "1");
				Assert.IsFalse(c.MoveNext(), "2");
				Header = "1";
				c = LogicalChildren;
				Assert.IsTrue(c.MoveNext(), "3");
				Assert.AreEqual(c.Current, "1", "4");
				Assert.IsFalse(c.MoveNext(), "5");
				Content = "2";
				c = LogicalChildren;
				Assert.IsTrue(c.MoveNext(), "6");
				Assert.AreEqual(c.Current, "1", "7");
				Assert.IsTrue(c.MoveNext(), "8");
				Assert.AreEqual(c.Current, "2", "9");
				Assert.IsFalse(c.MoveNext(), "10");
				Header = null;
				c = LogicalChildren;
				Assert.IsTrue(c.MoveNext(), "11");
				Assert.AreEqual(c.Current, "2", "12");
				Assert.IsFalse(c.MoveNext(), "13");
				Content = null;
				c = LogicalChildren;
				Assert.IsFalse(c.MoveNext(), "14");
			}
		}
		#endregion

		[Test]
		public void ToString() {
			HeaderedContentControl c = new HeaderedContentControl();
			c.Header = "1";
			c.Content = "2";
			Assert.AreEqual(c.ToString(), "System.Windows.Controls.HeaderedContentControl Header:1 Content:2");
		}
	}
}