using NUnit.Framework;
using System.Windows.Input;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class GridSplitterTest {
		[Test]
		public void DependencyProperties() {
			Assert.AreEqual(Utility.GetOptions((FrameworkPropertyMetadata)GridSplitter.DragIncrementProperty.GetMetadata(typeof(GridSplitter))), FrameworkPropertyMetadataOptions.None, "DragIncrement");
			Assert.AreEqual(Utility.GetOptions((FrameworkPropertyMetadata)GridSplitter.KeyboardIncrementProperty.GetMetadata(typeof(GridSplitter))), FrameworkPropertyMetadataOptions.None, "KeyboardIncrement");
			Assert.AreEqual(Utility.GetOptions((FrameworkPropertyMetadata)GridSplitter.PreviewStyleProperty.GetMetadata(typeof(GridSplitter))), FrameworkPropertyMetadataOptions.None, "PreviewStyle");
			Assert.AreEqual(Utility.GetOptions((FrameworkPropertyMetadata)GridSplitter.ResizeBehaviorProperty.GetMetadata(typeof(GridSplitter))), FrameworkPropertyMetadataOptions.None, "ResizeBehavior");
			Assert.AreEqual(Utility.GetOptions((FrameworkPropertyMetadata)GridSplitter.ResizeDirectionProperty.GetMetadata(typeof(GridSplitter))), FrameworkPropertyMetadataOptions.None, "ResizeDirection");
			Assert.AreEqual(Utility.GetOptions((FrameworkPropertyMetadata)GridSplitter.ShowsPreviewProperty.GetMetadata(typeof(GridSplitter))), FrameworkPropertyMetadataOptions.None, "ShowsPreview");
			Assert.AreEqual(GridSplitter.HorizontalAlignmentProperty.GetMetadata(typeof(GridSplitter)).DefaultValue, HorizontalAlignment.Right, "HorizontalAlignment default value");
			Assert.AreEqual(GridSplitter.VerticalAlignmentProperty.GetMetadata(typeof(GridSplitter)).DefaultValue, VerticalAlignment.Stretch, "VerticalAlignment default value");
		}

		[Test]
		public void SizeAffectsResizeBehaviorResizeDirection() {
			GridSplitter grid_splitter = new GridSplitter();
			grid_splitter.Width = 100;
			grid_splitter.Height = 100;
			grid_splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
			Window w = new Window();
			w.Content = grid_splitter;
			w.Show();
			Assert.AreEqual(grid_splitter.ResizeBehavior, GridResizeBehavior.BasedOnAlignment, "1");
			Assert.AreEqual(grid_splitter.ResizeDirection, GridResizeDirection.Auto, "2");
			grid_splitter.Height = 200;
			Assert.AreEqual(grid_splitter.ResizeBehavior, GridResizeBehavior.BasedOnAlignment, "3");
			Assert.AreEqual(grid_splitter.ResizeDirection, GridResizeDirection.Auto, "4");
		}
	}
}