using NUnit.Framework;
using System.Windows.Input;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class CheckBoxTest {
        #region OnAccessKeyNull
        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void OnAccessKeyNull() {
            new OnAccessKeyNullCheckBox();
        }

        class OnAccessKeyNullCheckBox : CheckBox {
            public OnAccessKeyNullCheckBox() {
                OnAccessKey(null);
            }
        }
        #endregion
		
		[Test]
		public void VerticalAlignmentTest() {
			CheckBox c = new CheckBox();
			Assert.AreEqual(c.VerticalAlignment, VerticalAlignment.Stretch, "VerticalAlignment");
			Assert.AreEqual(c.VerticalContentAlignment, VerticalAlignment.Top, "VerticalContentAlignment");
		}
	}
}