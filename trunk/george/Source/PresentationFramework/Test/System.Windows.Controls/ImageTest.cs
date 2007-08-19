using NUnit.Framework;
using System.IO;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	[Ignore ("This class cannot be implemented now.")]
	public class ImageTest
	{
		#region BaseUri
		[Test]
		public void BaseUri ()
		{
			new BaseUriImage ();
		}

		class BaseUriImage : Image
		{
			int get_calls;
			int set_calls;

			public BaseUriImage ()
			{
				((IUriContext)this).BaseUri = ((IUriContext)this).BaseUri;
				Assert.AreEqual (get_calls, 1, "1");
				Assert.AreEqual (set_calls, 1, "2");
				object dummy = ((IUriContext)this).BaseUri;
				Assert.AreEqual (get_calls, 2, "3");
				Assert.AreEqual (set_calls, 1, "4");
			}

			protected override Uri BaseUri {
				get {
					get_calls++;
					return base.BaseUri;
				}
				set {
					set_calls++;
					base.BaseUri = value;
				}
			}
		}
		#endregion

		#region BaseUriDefaultValue
		[Test]
		public void BaseUriDefaultValue ()
		{
			new BaseUriDefaultValueImage ();
		}

		class BaseUriDefaultValueImage : Image
		{
			public BaseUriDefaultValueImage ()
			{
				Assert.IsNull (BaseUri);
			}
		}
		#endregion

		#region ArrangeOverride
		[Test]
		public void ArrangeOverride ()
		{
			new ArrangeOverrideImage ();
		}

		class ArrangeOverrideImage : Image
		{
			Size arrange_size;
			Size arrange_result;

			public ArrangeOverrideImage ()
			{
				Width = 100;
				Height = 100;
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				Assert.AreEqual (arrange_size, new Size (100, 100), "1");
				Assert.AreEqual (arrange_result, new Size (0, 0), "2");
			}

			protected override Size ArrangeOverride (Size arrangeSize)
			{
				return arrange_result = base.ArrangeOverride (arrange_size = arrangeSize);
			}
		}
		#endregion

		#region ArrangeOverride2
		[Test]
		public void ArrangeOverride2 ()
		{
			new ArrangeOverride2Image ();
		}

		class ArrangeOverride2Image : Image
		{
			Size arrange_size;
			Size arrange_result;

			public ArrangeOverride2Image ()
			{
				Source = new DrawingImage (new GeometryDrawing (Brushes.Red, null, new RectangleGeometry (new Rect (0, 0, 10, 10))));
				Width = 100;
				Height = 100;
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				Assert.AreEqual (arrange_size, new Size (100, 100), "1");
				Assert.AreEqual (arrange_result, new Size (100, 100), "2");
			}

			protected override Size ArrangeOverride (Size arrangeSize)
			{
				return arrange_result = base.ArrangeOverride (arrange_size = arrangeSize);
			}
		}
		#endregion

		#region MeasureOverride
		[Test]
		public void MeasureOverride ()
		{
			new MeasureOverrideImage ();
		}

		class MeasureOverrideImage : Image
		{
			Size measure_size;
			Size measure_result;

			public MeasureOverrideImage ()
			{
				Width = 100;
				Height = 100;
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				Assert.AreEqual (measure_size, new Size (100, 100), "1");
				Assert.AreEqual (measure_result, new Size (0, 0), "2");
			}

			protected override Size MeasureOverride (Size availableSize)
			{
				return measure_result = base.MeasureOverride (measure_size = availableSize);
			}
		}
		#endregion

		#region MeasureOverride2
		[Test]
		public void MeasureOverride2 ()
		{
			new MeasureOverride2Image ();
		}

		class MeasureOverride2Image : Image
		{
			Size measure_size;
			Size measure_result;

			public MeasureOverride2Image ()
			{
				Source = new DrawingImage (new GeometryDrawing (Brushes.Red, null, new RectangleGeometry (new Rect (0, 0, 10, 10))));
				Width = 100;
				Height = 100;
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				Assert.AreEqual (measure_size, new Size (100, 100), "1");
				Assert.AreEqual (measure_result, new Size (100, 100), "2");
			}

			protected override Size MeasureOverride (Size availableSize)
			{
				return measure_result = base.MeasureOverride (measure_size = availableSize);
			}
		}
		#endregion

		#region MeasureOverrideStretchNone
		[Test]
		public void MeasureOverrideStretchNone ()
		{
			new MeasureOverrideStretchNoneImage ();
		}

		class MeasureOverrideStretchNoneImage : Image
		{
			Size measure_size;
			Size measure_result;

			public MeasureOverrideStretchNoneImage ()
			{
				Stretch = Stretch.None;
				Source = new DrawingImage (new GeometryDrawing (Brushes.Red, null, new RectangleGeometry (new Rect (0, 0, 10, 10))));
				Width = 100;
				Height = 100;
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				Assert.AreEqual (measure_size, new Size (100, 100), "1");
				Assert.AreEqual (measure_result, new Size (10, 10), "2");
			}

			protected override Size MeasureOverride (Size availableSize)
			{
				return measure_result = base.MeasureOverride (measure_size = availableSize);
			}
		}
		#endregion

		#region MeasureOverrideStretchNoneDifferentWidthHeight
		[Test]
		public void MeasureOverrideStretchNoneDifferentWidthHeight ()
		{
			new MeasureOverrideStretchNoneDifferentWidthHeightImage ();
		}

		class MeasureOverrideStretchNoneDifferentWidthHeightImage : Image
		{
			Size measure_size;
			Size measure_result;

			public MeasureOverrideStretchNoneDifferentWidthHeightImage ()
			{
				Stretch = Stretch.None;
				Source = new DrawingImage (new GeometryDrawing (Brushes.Red, null, new RectangleGeometry (new Rect (0, 0, 10, 20))));
				Width = 100;
				Height = 100;
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				Assert.AreEqual (measure_size, new Size (100, 100), "1");
				Assert.AreEqual (measure_result, new Size (10, 20), "2");
			}

			protected override Size MeasureOverride (Size availableSize)
			{
				return measure_result = base.MeasureOverride (measure_size = availableSize);
			}
		}
		#endregion

		#region MeasureOverrideStretchFillDifferentWidthHeight
		[Test]
		public void MeasureOverrideStretchFillDifferentWidthHeight ()
		{
			new MeasureOverrideStretchFillDifferentWidthHeightImage ();
		}

		class MeasureOverrideStretchFillDifferentWidthHeightImage : Image
		{
			Size measure_size;
			Size measure_result;

			public MeasureOverrideStretchFillDifferentWidthHeightImage ()
			{
				Stretch = Stretch.Fill;
				Source = new DrawingImage (new GeometryDrawing (Brushes.Red, null, new RectangleGeometry (new Rect (0, 0, 10, 20))));
				Width = 100;
				Height = 100;
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				Assert.AreEqual (measure_size, new Size (100, 100), "1");
				Assert.AreEqual (measure_result, new Size (100, 100), "2");
			}

			protected override Size MeasureOverride (Size availableSize)
			{
				return measure_result = base.MeasureOverride (measure_size = availableSize);
			}
		}
		#endregion

		#region MeasureOverrideStretchUniformDifferentWidthHeight
		[Test]
		public void MeasureOverrideStretchUniformDifferentWidthHeight ()
		{
			new MeasureOverrideStretchUniformDifferentWidthHeightImage ();
		}

		class MeasureOverrideStretchUniformDifferentWidthHeightImage : Image
		{
			Size measure_size;
			Size measure_result;

			public MeasureOverrideStretchUniformDifferentWidthHeightImage ()
			{
				Stretch = Stretch.Uniform;
				Source = new DrawingImage (new GeometryDrawing (Brushes.Red, null, new RectangleGeometry (new Rect (0, 0, 10, 20))));
				Width = 100;
				Height = 100;
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				Assert.AreEqual (measure_size, new Size (100, 100), "1");
				Assert.AreEqual (measure_result, new Size (50, 100), "2");
			}

			protected override Size MeasureOverride (Size availableSize)
			{
				return measure_result = base.MeasureOverride (measure_size = availableSize);
			}
		}
		#endregion

		#region MeasureOverrideStretchUniformToFillDifferentWidthHeight
		[Test]
		public void MeasureOverrideStretchUniformToFillDifferentWidthHeight ()
		{
			new MeasureOverrideStretchUniformToFillDifferentWidthHeightImage ();
		}

		class MeasureOverrideStretchUniformToFillDifferentWidthHeightImage : Image
		{
			Size measure_size;
			Size measure_result;

			public MeasureOverrideStretchUniformToFillDifferentWidthHeightImage ()
			{
				Stretch = Stretch.UniformToFill;
				Source = new DrawingImage (new GeometryDrawing (Brushes.Red, null, new RectangleGeometry (new Rect (0, 0, 10, 20))));
				Width = 100;
				Height = 100;
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				Assert.AreEqual (measure_size, new Size (100, 100), "1");
				Assert.AreEqual (measure_result, new Size (100, 200), "2");
			}

			protected override Size MeasureOverride (Size availableSize)
			{
				return measure_result = base.MeasureOverride (measure_size = availableSize);
			}
		}
		#endregion

		#region MeasureOverrideInfiniteAvailableSize
		[Test]
		public void MeasureOverrideInfiniteAvailableSize ()
		{
			new MeasureOverrideInfiniteAvailableSizeImage ();
		}

		class MeasureOverrideInfiniteAvailableSizeImage : Image
		{
			Size measure_result;

			public MeasureOverrideInfiniteAvailableSizeImage ()
			{
				Source = new DrawingImage (new GeometryDrawing (Brushes.Red, null, new RectangleGeometry (new Rect (0, 0, 10, 20))));
				Window w = new Window ();
				Canvas c = new Canvas ();
				w.Content = c;
				c.Children.Add (this);
				w.Show ();
				Assert.AreEqual (measure_result, new Size (10, 20));
			}

			protected override Size MeasureOverride (Size availableSize)
			{
				return measure_result = base.MeasureOverride (availableSize);
			}
		}
		#endregion

		#region MeasureOverrideMixedAvailableSize
		[Test]
		public void MeasureOverrideMixedAvailableSize ()
		{
			new MeasureOverrideMixedAvailableSizeImage ();
		}

		class MeasureOverrideMixedAvailableSizeImage : Image
		{
			Size measure_result;

			public MeasureOverrideMixedAvailableSizeImage ()
			{
				Source = new DrawingImage (new GeometryDrawing (Brushes.Red, null, new RectangleGeometry (new Rect (0, 0, 10, 20))));
				Height = 100;
				Window w = new Window ();
				Canvas c = new Canvas ();
				w.Content = c;
				c.Children.Add (this);
				w.Show ();
				Assert.AreEqual (measure_result, new Size (50, 100));
			}

			protected override Size MeasureOverride (Size availableSize)
			{
				return measure_result = base.MeasureOverride (availableSize);
			}
		}
		#endregion

		#region MeasureOverrideMixedAvailableSizeStretchNone
		[Test]
		public void MeasureOverrideMixedAvailableSizeStretchNone ()
		{
			new MeasureOverrideMixedAvailableSizeStretchNoneImage ();
		}

		class MeasureOverrideMixedAvailableSizeStretchNoneImage : Image
		{
			Size measure_result;

			public MeasureOverrideMixedAvailableSizeStretchNoneImage ()
			{
				Stretch = Stretch.None;
				Source = new DrawingImage (new GeometryDrawing (Brushes.Red, null, new RectangleGeometry (new Rect (0, 0, 10, 20))));
				Height = 100;
				Window w = new Window ();
				Canvas c = new Canvas ();
				w.Content = c;
				c.Children.Add (this);
				w.Show ();
				Assert.AreEqual (measure_result, new Size (10, 20));
			}

			protected override Size MeasureOverride (Size availableSize)
			{
				return measure_result = base.MeasureOverride (availableSize);
			}
		}
		#endregion

		#region MeasureOverrideMixedAvailableSizeStretchFill
		[Test]
		public void MeasureOverrideMixedAvailableSizeStretchFill ()
		{
			new MeasureOverrideMixedAvailableSizeStretchFillImage ();
		}

		class MeasureOverrideMixedAvailableSizeStretchFillImage : Image
		{
			Size measure_result;

			public MeasureOverrideMixedAvailableSizeStretchFillImage ()
			{
				Stretch = Stretch.Fill;
				Source = new DrawingImage (new GeometryDrawing (Brushes.Red, null, new RectangleGeometry (new Rect (0, 0, 10, 20))));
				Height = 100;
				Window w = new Window ();
				Canvas c = new Canvas ();
				w.Content = c;
				c.Children.Add (this);
				w.Show ();
				Assert.AreEqual (measure_result, new Size (50, 100));
			}

			protected override Size MeasureOverride (Size availableSize)
			{
				return measure_result = base.MeasureOverride (availableSize);
			}
		}
		#endregion

		#region MeasureOverrideMixedAvailableSizeStretchUniformToFill
		[Test]
		public void MeasureOverrideMixedAvailableSizeStretchUniformToFill ()
		{
			new MeasureOverrideMixedAvailableSizeStretchUniformToFillImage ();
		}

		class MeasureOverrideMixedAvailableSizeStretchUniformToFillImage : Image
		{
			Size measure_result;

			public MeasureOverrideMixedAvailableSizeStretchUniformToFillImage ()
			{
				Stretch = Stretch.UniformToFill;
				Source = new DrawingImage (new GeometryDrawing (Brushes.Red, null, new RectangleGeometry (new Rect (0, 0, 10, 20))));
				Height = 100;
				Window w = new Window ();
				Canvas c = new Canvas ();
				w.Content = c;
				c.Children.Add (this);
				w.Show ();
				Assert.AreEqual (measure_result, new Size (50, 100));
			}

			protected override Size MeasureOverride (Size availableSize)
			{
				return measure_result = base.MeasureOverride (availableSize);
			}
		}
		#endregion

		[Test]
		public void BitmapImageUriKindRelativeOrAbsolute ()
		{
			BitmapImage bitmap_image = new BitmapImage ();
			bitmap_image.BeginInit ();
			Assert.IsTrue (File.Exists ("Test.png"), "1");
			bitmap_image.UriSource = new Uri ("/Test.png", UriKind.RelativeOrAbsolute);
			bitmap_image.EndInit ();
			Image image = new Image ();
			image.Source = bitmap_image;
			image.Stretch = Stretch.None;
			Window w = new Window ();
			w.Content = image;
			w.Show ();
			Assert.AreEqual (image.ActualWidth, 0, "2");
		}

		[Test]
		public void BitmapImageUriKindRelative ()
		{
			BitmapImage bitmap_image = new BitmapImage ();
			bitmap_image.BeginInit ();
			Assert.IsTrue (File.Exists ("Test.png"), "1");
			bitmap_image.UriSource = new Uri ("/Test.png", UriKind.Relative);
			bitmap_image.EndInit ();
			Image image = new Image ();
			image.Source = bitmap_image;
			image.Stretch = Stretch.None;
			Window w = new Window ();
			w.Content = image;
			w.Show ();
			Assert.AreEqual (image.ActualWidth, 0, "2");
		}

		[Test]
		public void BitmapImageUriKindAbsolute ()
		{
			BitmapImage bitmap_image = new BitmapImage ();
			bitmap_image.BeginInit ();
			Assert.IsTrue (File.Exists ("Test.png"), "1");
			bitmap_image.UriSource = new Uri (Path.Combine (Environment.CurrentDirectory, "Test.png"), UriKind.Absolute);
			bitmap_image.EndInit ();
			Image image = new Image ();
			image.Source = bitmap_image;
			image.Stretch = Stretch.None;
			Window w = new Window ();
			w.Content = image;
			w.Show ();
			Assert.AreEqual (image.ActualWidth, 34, "2");
		}

		[Test]
		public void ImageFailed ()
		{
			BitmapImage bitmap_image = new BitmapImage ();
			bitmap_image.BeginInit ();
			Assert.IsFalse (File.Exists ("Nonexistent"), "1");
			bitmap_image.UriSource = new Uri ("Nonexistent", UriKind.Relative);
			bitmap_image.EndInit ();
			Image image = new Image ();
			int calls = 0;
			image.ImageFailed += delegate (object sender, ExceptionRoutedEventArgs e)
			{
				calls++;
			};
			image.Source = bitmap_image;
			image.Stretch = Stretch.None;
			Window w = new Window ();
			w.Content = image;
			Assert.AreEqual (calls, 0, "1");
			w.Show ();
			Assert.AreEqual (calls, 1, "2");
			Assert.AreEqual (image.ActualWidth, 0, "3");
		}


	}
}