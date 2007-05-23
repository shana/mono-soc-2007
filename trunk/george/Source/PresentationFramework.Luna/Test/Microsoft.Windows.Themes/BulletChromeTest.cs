using NUnit.Framework;
using System;
using System.Windows;
#if Implementation
using Microsoft.Windows.Themes;
namespace Mono.Microsoft.Windows.Themes {
#else
namespace Microsoft.Windows.Themes {
#endif
	[TestFixture]
	public class BulletChromeTest {
		[Test]
		public void StaticPropertiesDefaultValues() {
			Assert.IsNull(BulletChrome.BackgroundProperty.DefaultMetadata.DefaultValue, "Background");
			Assert.IsNull(BulletChrome.BorderBrushProperty.DefaultMetadata.DefaultValue, "BorderBrush");
			Assert.AreEqual(BulletChrome.BorderThicknessProperty.DefaultMetadata.DefaultValue, new Thickness(0), "BorderThickness");
			Assert.IsFalse((bool)BulletChrome.IsCheckedProperty.DefaultMetadata.DefaultValue, "IsChecked");
			Assert.IsFalse((bool)BulletChrome.IsRoundProperty.DefaultMetadata.DefaultValue, "IsRound");
			Assert.IsFalse((bool)BulletChrome.RenderMouseOverProperty.DefaultMetadata.DefaultValue, "RenderMouseOver");
			Assert.IsFalse((bool)BulletChrome.RenderPressedProperty.DefaultMetadata.DefaultValue, "RenderPressed");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void BorderThickness() {
			BulletChrome b = new BulletChrome();
			Thickness t = new Thickness(-1);
			b.BorderThickness = t;
		}

		[Test]
		public void Creation() {
			BulletChrome b = new BulletChrome();
			Assert.IsFalse(b.SnapsToDevicePixels, "SnapsToDevicePixels");
		}

		[Test]
		public void Size() {
			BulletChrome b = new BulletChrome();
			b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(b.DesiredSize.Width, 11, "Width");
			Assert.AreEqual(b.DesiredSize.Height, 11, "Height");
			b.Measure(new Size(1, 1));
			Assert.AreEqual(b.DesiredSize.Width, 1, "Width with constraint");
			Assert.AreEqual(b.DesiredSize.Height, 1, "Height with constraint");
			b.Measure(new Size(1, 50));
			Assert.AreEqual(b.DesiredSize.Width, 1, "Width with constraint 2");
			Assert.AreEqual(b.DesiredSize.Height, 11, "Height with constraint 2");
			b.BorderThickness = new Thickness(1, 2, 3, 4);
			b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(b.DesiredSize.Width, 15, "Width, border");
			Assert.AreEqual(b.DesiredSize.Height, 17, "Height, border");
		}

		[Test]
		public void RoundSize() {
			BulletChrome b = new BulletChrome();
			b.IsRound = true;
			b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(b.DesiredSize.Width, 13, "Width");
			Assert.AreEqual(b.DesiredSize.Height, 13, "Height");
			b.Measure(new Size(1, 1));
			Assert.AreEqual(b.DesiredSize.Width, 1, "Width with constraint");
			Assert.AreEqual(b.DesiredSize.Height, 1, "Height with constraint");
			b.Measure(new Size(1, 50));
			Assert.AreEqual(b.DesiredSize.Width, 1, "Width with constraint 2");
			Assert.AreEqual(b.DesiredSize.Height, 13, "Height with constraint 2");
			b.BorderThickness = new Thickness(1, 2, 3, 4);
			b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(b.DesiredSize.Width, 13, "Width, border");
			Assert.AreEqual(b.DesiredSize.Height, 13, "Height, border");
			b.BorderThickness = new Thickness(10);
			b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(b.DesiredSize.Width, 13, "Width, border 2");
			Assert.AreEqual(b.DesiredSize.Height, 13, "Height, border 2");
		}

		[Test]
		public void MeasureOverride() {
			BulletChrome b = new BulletChrome();
			b.Measure(new Size(double.PositiveInfinity, 10));
			Assert.AreEqual(b.DesiredSize.Width, 11, "Width");
			Assert.AreEqual(b.DesiredSize.Height, 10, "Height");
		}
	}
}