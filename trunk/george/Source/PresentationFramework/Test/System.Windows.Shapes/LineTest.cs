using NUnit.Framework;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#if Implementation
namespace Mono.System.Windows.Shapes {
#else
namespace System.Windows.Shapes {
#endif
	[TestFixture]
	public class LineTest {
		[Test]
		public void DependencyProperties() {
			Assert.AreEqual(Utility.GetOptions((FrameworkPropertyMetadata)Line.X1Property.GetMetadata(typeof(Line))),FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, "X1");
			Assert.AreEqual(Utility.GetOptions((FrameworkPropertyMetadata)Line.X2Property.GetMetadata(typeof(Line))), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, "X2");
			Assert.AreEqual(Utility.GetOptions((FrameworkPropertyMetadata)Line.Y1Property.GetMetadata(typeof(Line))), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, "Y1");
			Assert.AreEqual(Utility.GetOptions((FrameworkPropertyMetadata)Line.Y2Property.GetMetadata(typeof(Line))), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, "Y2");
		}
	}
}