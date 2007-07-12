using NUnit.Framework;
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class ItemsPanelTemplateTest {
		#region ValidateTemplatedParent
		[Test]
		public void ValidateTemplatedParent() {
			new ValidateTemplatedParentItemsPanelTemplate();
		}

		class ValidateTemplatedParentItemsPanelTemplate : ItemsPanelTemplate {
			public ValidateTemplatedParentItemsPanelTemplate() {
				ValidateTemplatedParent(new ItemsPresenter());
				try {
					ValidateTemplatedParent(null);
					Assert.Fail("1");
				} catch (ArgumentNullException ex) {
					Assert.AreEqual(ex.ParamName, "templatedParent", "2");
				}
				try {
					ValidateTemplatedParent(new FrameworkElement());
					Assert.Fail("3");
				} catch (ArgumentException ex) {
				    Assert.AreEqual(ex.Message, "'ItemsPresenter' ControlTemplate TargetType does not match templated type 'FrameworkElement'.", "4");
				}
			}
		}
		#endregion
	}
}