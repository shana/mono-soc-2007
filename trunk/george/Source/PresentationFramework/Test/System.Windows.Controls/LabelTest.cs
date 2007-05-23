using NUnit.Framework;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class LabelTest {
		[Test]
		public void StaticProperties() {
			Assert.AreEqual(Label.TargetProperty.Name, "Target", "TargetProperty.Name");
			Assert.AreEqual(Label.TargetProperty.OwnerType, typeof(Label), "TargetProperty.OwnerType");
			Assert.AreEqual(Label.TargetProperty.PropertyType, typeof(UIElement), "TargetProperty.PropertyType");
		}

		[Test]
		public void Creation() {
			Label label = new Label();
			Assert.IsFalse(label.IsTabStop, "IsTabStop");
		}
	}
}