using NUnit.Framework;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#if Implementation
namespace Mono.System.Windows.Shapes
{
#else
namespace System.Windows.Shapes {
#endif
	[TestFixture]
	public class PathTest
	{
		[Test]
		public void DependencyProperties ()
		{
			Assert.AreEqual (Utility.GetOptions ((FrameworkPropertyMetadata)Path.DataProperty.GetMetadata (typeof (Path))), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender);
		}
	}
}