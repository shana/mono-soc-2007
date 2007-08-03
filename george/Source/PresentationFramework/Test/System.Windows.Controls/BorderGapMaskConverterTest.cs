using NUnit.Framework;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
#if Implementation
using System.Windows;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class BorderGapMaskConverterTest {
		[Test]
		public void Test() {
			Assert.AreEqual(new BorderGapMaskConverter().Convert(new object[] { 100, 500, 1000 }, null, 7, null), DependencyProperty.UnsetValue, "1");
			Assert.AreEqual(new BorderGapMaskConverter().Convert(new object[] { 100D, 500D, 1000D }, null, 7, null), DependencyProperty.UnsetValue, "2");
			VisualBrush result = (VisualBrush)new BorderGapMaskConverter().Convert(new object[] { 100D, 500D, 1000D }, null, 7D, null);
			Grid grid = (Grid)result.Visual;
			Assert.AreEqual(grid.Children.Count, 3, "3");
			Assert.AreEqual(((SolidColorBrush)((Rectangle)grid.Children[0]).Fill).Color, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), "4");
			Assert.AreEqual(Grid.GetColumn(grid.Children[0]), 0, "5");
			Assert.AreEqual(Grid.GetRow(grid.Children[0]), 0, "6");
			Assert.AreEqual(Grid.GetRowSpan(grid.Children[0]), 2, "7");
			Assert.AreEqual(((SolidColorBrush)((Rectangle)grid.Children[1]).Fill).Color, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), "8");
			Assert.AreEqual(Grid.GetColumn(grid.Children[1]), 1, "9");
			Assert.AreEqual(Grid.GetRow(grid.Children[1]), 1, "10");
			Assert.AreEqual(((SolidColorBrush)((Rectangle)grid.Children[2]).Fill).Color, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), "11");
			Assert.AreEqual(Grid.GetColumn(grid.Children[2]), 2, "12");
			Assert.AreEqual(Grid.GetRow(grid.Children[0]), 0, "13");
			Assert.AreEqual(Grid.GetRowSpan(grid.Children[2]), 2, "14");
			Assert.AreEqual(grid.Height, 1000, "15");
			Assert.AreEqual(grid.Width, 500, "16");
			Assert.AreEqual(grid.ColumnDefinitions.Count, 3, "17");
			Assert.AreEqual(grid.ColumnDefinitions[0].Width.Value, 7, "18");
			Assert.AreEqual(grid.ColumnDefinitions[1].Width.Value, 100, "19");
			Assert.IsTrue(grid.ColumnDefinitions[2].Width.IsStar, "20");
			Assert.AreEqual(grid.RowDefinitions.Count, 2, "21");
			Assert.AreEqual(grid.RowDefinitions[0].Height.Value, 500, "22");
			Assert.IsTrue(grid.RowDefinitions[1].Height.IsStar, "23");
		}

		[Test]
		public void Test2() {
			Assert.AreEqual(new BorderGapMaskConverter().Convert(new object[] { 100, 500, 1000 }, null, "7", null), DependencyProperty.UnsetValue, "1");
			VisualBrush result = (VisualBrush)new BorderGapMaskConverter().Convert(new object[] { 100D, 500D, 1000D }, null, "7", null);
			Grid grid = (Grid)result.Visual;
			Assert.AreEqual(grid.Children.Count, 3, "3");
			Assert.AreEqual(((SolidColorBrush)((Rectangle)grid.Children[0]).Fill).Color, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), "4");
			Assert.AreEqual(Grid.GetColumn(grid.Children[0]), 0, "5");
			Assert.AreEqual(Grid.GetRow(grid.Children[0]), 0, "6");
			Assert.AreEqual(Grid.GetRowSpan(grid.Children[0]), 2, "7");
			Assert.AreEqual(((SolidColorBrush)((Rectangle)grid.Children[1]).Fill).Color, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), "8");
			Assert.AreEqual(Grid.GetColumn(grid.Children[1]), 1, "9");
			Assert.AreEqual(Grid.GetRow(grid.Children[1]), 1, "10");
			Assert.AreEqual(((SolidColorBrush)((Rectangle)grid.Children[2]).Fill).Color, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), "11");
			Assert.AreEqual(Grid.GetColumn(grid.Children[2]), 2, "12");
			Assert.AreEqual(Grid.GetRow(grid.Children[0]), 0, "13");
			Assert.AreEqual(Grid.GetRowSpan(grid.Children[2]), 2, "14");
			Assert.AreEqual(grid.Height, 1000, "15");
			Assert.AreEqual(grid.Width, 500, "16");
			Assert.AreEqual(grid.ColumnDefinitions.Count, 3, "17");
			Assert.AreEqual(grid.ColumnDefinitions[0].Width.Value, 7, "18");
			Assert.AreEqual(grid.ColumnDefinitions[1].Width.Value, 100, "19");
			Assert.IsTrue(grid.ColumnDefinitions[2].Width.IsStar, "20");
			Assert.AreEqual(grid.RowDefinitions.Count, 2, "21");
			Assert.AreEqual(grid.RowDefinitions[0].Height.Value, 500, "22");
			Assert.IsTrue(grid.RowDefinitions[1].Height.IsStar, "23");
		}
	}
}