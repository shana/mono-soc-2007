using NUnit.Framework;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class ColumnDefinitionTest {
		[Test]
		public void StaticProperties() {
			FrameworkPropertyMetadata width_metadata = (FrameworkPropertyMetadata)ColumnDefinition.WidthProperty.GetMetadata(typeof(ColumnDefinition));
			Assert.IsNotNull(width_metadata.PropertyChangedCallback, "1");
			Assert.IsFalse(width_metadata.AffectsArrange, "2");
			Assert.IsFalse(width_metadata.AffectsMeasure, "3");
			Assert.IsFalse(width_metadata.AffectsParentArrange, "4");
			Assert.IsFalse(width_metadata.AffectsParentMeasure, "5");
			Assert.IsFalse(width_metadata.AffectsRender, "6");
		}

		[Test]
		public void Lenght() {
			GridLength l = new ColumnDefinition().Width;
			Assert.AreEqual(l.Value, 1, "1");
			Assert.AreEqual(l.GridUnitType, GridUnitType.Star, "2");
		}
	}
}