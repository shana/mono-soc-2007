using NUnit.Framework;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class RowDefinitionTest {
		[Test]
		public void StaticProperties() {
			FrameworkPropertyMetadata height_metadata = (FrameworkPropertyMetadata)RowDefinition.HeightProperty.GetMetadata(typeof(RowDefinition));
			Assert.IsNotNull(height_metadata.PropertyChangedCallback, "1");
			Assert.IsFalse(height_metadata.AffectsArrange, "2");
			Assert.IsFalse(height_metadata.AffectsMeasure, "3");
			Assert.IsFalse(height_metadata.AffectsParentArrange, "4");
			Assert.IsFalse(height_metadata.AffectsParentMeasure, "5");
			Assert.IsFalse(height_metadata.AffectsRender, "6");
		}

		[Test]
		public void Lenght() {
			GridLength l = new RowDefinition().Height;
			Assert.AreEqual(l.Value, 1, "1");
			Assert.AreEqual(l.GridUnitType, GridUnitType.Star, "2");
		}
	}
}