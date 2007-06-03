using NUnit.Framework;
using System.Windows;
#if Implementation
using Microsoft.Windows.Themes;
namespace Mono.Microsoft.Windows.Themes {
#else
namespace Microsoft.Windows.Themes {
#endif
	[TestFixture]
	public class ButtonChromeTest {
		[Test]
		public void StaticProperties() {
			TestProperty(ButtonChrome.BorderBrushProperty);
			TestProperty(ButtonChrome.FillProperty);
			TestProperty(ButtonChrome.RenderDefaultedProperty);
			TestProperty(ButtonChrome.RenderMouseOverProperty);
			TestProperty(ButtonChrome.RenderPressedProperty);
		}
		void TestProperty(DependencyProperty property) {
			Assert.IsNull(property.ValidateValueCallback, property.Name + " ValidateValueCallback");
			PropertyMetadata metadata = property.GetMetadata(typeof(ButtonChrome));
			Assert.IsInstanceOfType(typeof(FrameworkPropertyMetadata), metadata, property.Name + " metadata type");
			Assert.IsTrue(((FrameworkPropertyMetadata)metadata).AffectsRender, property.Name + " AffectsRender");
		}
		[Test]
		public void Creation() {
			ButtonChrome button_chrome = new ButtonChrome();
			Assert.IsNull(button_chrome.BorderBrush, "button_chrome.BorderBrush");
			Assert.IsNull(button_chrome.Fill, "button_chrome.Fill");
			Assert.IsFalse(button_chrome.RenderDefaulted, "button_chrome.RenderDefaulted");
			Assert.IsFalse(button_chrome.RenderPressed, "button_chrome.RenderPressed");
			Assert.IsFalse(button_chrome.RenderMouseOver, "button_chrome.RenderMouseOver");
			Assert.AreEqual(button_chrome.ThemeColor, ThemeColor.NormalColor, "button_chrome.ThemeColor");
			Assert.IsNull(button_chrome.Clip, "button_chrome.Clip");
		}

		[Test]
		public void Alignment() {
			ButtonChrome b = new ButtonChrome();
			Assert.AreEqual(b.HorizontalAlignment, HorizontalAlignment.Stretch, "HorizontalAlignment");
			Assert.AreEqual(b.VerticalAlignment, VerticalAlignment.Stretch, "VerticalAlignment");
		}

		[Test]
		public void Measure() {
			ButtonChrome b = new ButtonChrome();
			b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(b.DesiredSize.Width, 8, "Width");
			Assert.AreEqual(b.DesiredSize.Height, 8, "Height");
			b.Measure(new Size(double.PositiveInfinity, 10));
			Assert.AreEqual(b.DesiredSize.Width, 8, "Width 2");
			Assert.AreEqual(b.DesiredSize.Height, 8, "Height 2");
			b.Measure(new Size(double.PositiveInfinity, 7));
			Assert.AreEqual(b.DesiredSize.Width, 8, "Width 3");
			Assert.AreEqual(b.DesiredSize.Height, 7, "Height 3");
			b.Measure(new Size(7, 7));
			Assert.AreEqual(b.DesiredSize.Width, 7, "Width 4");
			Assert.AreEqual(b.DesiredSize.Height, 7, "Height 4");

			FrameworkElement x = new FrameworkElement();
			x.Width = 10;
			x.Height = 10;
			b.Child = x;
			b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(b.DesiredSize.Width, 18, "Width 5");
			Assert.AreEqual(b.DesiredSize.Height, 18, "Height 5");
			b.Measure(new Size(double.PositiveInfinity, 0));
			Assert.AreEqual(b.DesiredSize.Width, 18, "Width 6");
			Assert.AreEqual(b.DesiredSize.Height, 0, "Height 6");
			b.Measure(new Size(0, 0));
			Assert.AreEqual(b.DesiredSize.Width, 0, "Width 7");
			Assert.AreEqual(b.DesiredSize.Height, 0, "Height 7");
			b.Measure(new Size(double.PositiveInfinity, 17));
			Assert.AreEqual(b.DesiredSize.Width, 18, "Width 8");
			Assert.AreEqual(b.DesiredSize.Height, 17, "Height 8");

			b = new ButtonChrome();
			b.Measure(new Size(100, 100));
			Assert.AreEqual(b.DesiredSize.Width, 8, "1");
		}
	}
}