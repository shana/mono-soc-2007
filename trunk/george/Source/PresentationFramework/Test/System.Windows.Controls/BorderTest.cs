using NUnit.Framework;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class BorderTest {
		[Test]
		public void Creation() {
			Border b = new Border();
			Assert.AreEqual(b.BorderThickness, new Thickness(0), "BorderThickness");
			Assert.AreEqual(b.Padding, new Thickness(0), "Padding");
			Assert.IsFalse(b.SnapsToDevicePixels, "SnapsToDevicePixels");
		}

		#region MeasureOverride
		[Test]
		public void MeasureOverride() {
			new MeasureOverrideBorder();
		}

		class MeasureOverrideBorder : Border {
			public MeasureOverrideBorder() {
				Size result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 0, "Width");
				Assert.AreEqual(result.Height, 0, "Height");
				
				Control child = new Control();
				child.Width = 100;
				child.Height = 100;
				Child = child;
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 100, "Width 1");
				Assert.AreEqual(result.Height, 100, "Height 1");

				Padding = new Thickness(1, 2, 3, 4);
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 104, "Width 2");
				Assert.AreEqual(result.Height, 106, "Height 2");

				BorderThickness = new Thickness(1, 2, 3, 4);
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 108, "Width 3");
				Assert.AreEqual(result.Height, 112, "Height 3");

				Padding = new Thickness(0);
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 104, "Width 4");
				Assert.AreEqual(result.Height, 106, "Height 4");

				Child = null;
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 4, "Width 5");
				Assert.AreEqual(result.Height, 6, "Height 5");

				Padding = new Thickness(1, 2, 3, 4);
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 8, "Width 6");
				Assert.AreEqual(result.Height, 12, "Height 6");

				Padding = new Thickness(0);
				BorderThickness = new Thickness(0);
				child = new Control();
				Child = child;
				MeasureOverride(new Size(100, 100));
				Assert.AreEqual(child.ActualWidth, 0, "Width 7");
				Assert.AreEqual(child.ActualHeight, 0, "Height 7");
				Assert.AreEqual(child.DesiredSize.Width, 0, "Width 8");
				Assert.AreEqual(child.DesiredSize.Height, 0, "Height 8");
				Window window = new Window();
				window.Show();
				Canvas canvas = new Canvas();
				window.Content = canvas;
				canvas.Children.Add(this);
				result = MeasureOverride(new Size(100, 100));
				Assert.AreEqual(child.ActualWidth, 0, "Width 9");
				Assert.AreEqual(child.ActualHeight, 0, "Height 9");
				Assert.AreEqual(child.DesiredSize.Width, 0, "Width 10");
				Assert.AreEqual(child.DesiredSize.Height, 0, "Height 10");
				Assert.AreEqual(result.Width, 0, "Width 11");
				Assert.AreEqual(result.Height, 0, "Height 11");
				Measure(new Size(100, 100));
				Assert.AreEqual(child.ActualWidth, 0, "Width 12");
				Assert.AreEqual(child.ActualHeight, 0, "Height 12");
				Assert.AreEqual(child.DesiredSize.Width, 0, "Width 13");
				Assert.AreEqual(child.DesiredSize.Height, 0, "Height 13");
				ArrangeOverride(new Size(100, 100));
				Assert.AreEqual(child.ActualWidth, 100, "Width 14");
				Assert.AreEqual(child.ActualHeight, 100, "Height 14");
				Assert.AreEqual(child.DesiredSize.Width, 0, "Width 15");
				Assert.AreEqual(child.DesiredSize.Height, 0, "Height 15");
			}
		}
		#endregion

		#region ChildMeasureCalled
		[Test]
		public void ChildMeasureCalled() {
			new ChildMeasureCalledBorder();
		}

		class ChildMeasureCalledBorder : Border {
			public ChildMeasureCalledBorder() {
				Child = new ChildBorder();
				ChildBorder.SetMeasureOverrideCalled = true;
				MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				ChildBorder.SetMeasureOverrideCalled = false;
				Assert.IsTrue(ChildBorder.MeasureOverrideCalled);
			}

			class ChildBorder : Border {
				static public bool SetMeasureOverrideCalled;
				static public bool MeasureOverrideCalled;
				
				protected override Size MeasureOverride(Size constraint) {
					if (SetMeasureOverrideCalled)
						MeasureOverrideCalled = true;
					return new Size(0, 0);
				}
			}
		}
		#endregion
	}
}