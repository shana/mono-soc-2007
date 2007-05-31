using NUnit.Framework;
using System.Threading;
using System.Windows.Media;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	[TestFixture]
	public class TrackTest {
		[Test]
		public void StaticProperties() {
			Assert.AreEqual(Track.OrientationProperty.GetMetadata(typeof(Track)).DefaultValue, Orientation.Horizontal, "Orientation default value");
			Assert.IsTrue(((FrameworkPropertyMetadata)Track.ValueProperty.GetMetadata(typeof(Track))).BindsTwoWayByDefault, "BindsTwoWayByDefault");
		}

		[Test]
		public void Creation() {
			Track t = new Track();
			Assert.AreEqual(t.Value, 0, "Value");
			Assert.AreEqual(t.Maximum, 1, "Maximum");
			Assert.AreEqual(t.Minimum, 0, "Minimum");
			Assert.AreEqual(t.Orientation, Orientation.Horizontal, "Orientation");
			Assert.IsNull(t.Thumb, "Thumb");
			Assert.IsNull(t.IncreaseRepeatButton, "Thumb");
			Assert.IsNull(t.DecreaseRepeatButton, "Thumb");
			Assert.IsNaN(t.ViewportSize, "ViewportSize");
			Assert.IsFalse(t.IsDirectionReversed, "IsDirectionReversed");
			t.Thumb = new Thumb();
			Assert.AreEqual(t.Orientation, Orientation.Horizontal, "Orientation");
			Assert.IsFalse(t.SnapsToDevicePixels, "SnapsToDevicePixels");
		}

		[Test]
		public void IsEnabled() {
			Assert.IsTrue(new Track().IsEnabled);
		}

		#region VisualChildrenCount
		[Test]
		public void VisualChildrenCount() {
			new VisualChildrenCountTrack();
		}

		class VisualChildrenCountTrack : Track {
			public VisualChildrenCountTrack() {
				Assert.AreEqual(VisualChildrenCount, 0, "VisualChildrenCount 1");
				Thumb = new Thumb();
				Assert.AreEqual(VisualChildrenCount, 1, "VisualChildrenCount 2");
			}
		}
		#endregion

		#region GetVisualChild
		[Test]
		public void GetVisualChild() {
			new GetVisualChildTrack();
		}

		class GetVisualChildTrack : Track {
			public GetVisualChildTrack() {
				Thumb = new Thumb();
				Assert.AreEqual(GetVisualChild(0), Thumb, "GetVisualChild(0) 1");
				DecreaseRepeatButton = new RepeatButton();
				Assert.AreEqual(GetVisualChild(0), Thumb, "GetVisualChild(0) 2");
				Thumb = Thumb;
				Assert.AreEqual(GetVisualChild(0), Thumb, "GetVisualChild(0) 2");
				Thumb = new Thumb();
				Assert.AreEqual(GetVisualChild(0), DecreaseRepeatButton, "GetVisualChild(0) 3");
			}
		}
		#endregion

		#region ValueFrom
		[Test]
		public void ValueFrom() {
			new ValueFromTrack();
		}

		class ValueFromTrack : Track {
			public ValueFromTrack() {
				Assert.AreEqual(ValueFromDistance(0, 0), double.NaN, "1");
				Assert.AreEqual(ValueFromDistance(0, 1), double.NaN, "2");
				Assert.AreEqual(ValueFromDistance(0, double.PositiveInfinity), double.NaN, "3");
				Assert.AreEqual(ValueFromDistance(3, 0), double.NaN, "4");
				Assert.AreEqual(ValueFromDistance(3, 1), double.NaN, "5");
				Assert.AreEqual(ValueFromDistance(3, double.PositiveInfinity), double.NaN, "6");
				Window w = new Window();
				Canvas c = new Canvas();
				c.Children.Add(this);
				w.Content = c;
				w.Show();
				Width = 100;
				Height = 200;
				Assert.AreEqual(ValueFromDistance(0, 0), double.NaN, "7");
				Assert.AreEqual(ValueFromDistance(0, 1), double.NaN, "8");
				Assert.AreEqual(ValueFromDistance(0, double.PositiveInfinity), double.NaN, "9");
				Assert.AreEqual(ValueFromDistance(3, 0), double.PositiveInfinity, "10");
				Assert.AreEqual(ValueFromDistance(3, 1), double.PositiveInfinity, "11");
				Assert.AreEqual(ValueFromDistance(3, double.PositiveInfinity), double.PositiveInfinity, "12");
				Maximum = 10;
				Assert.AreEqual(ValueFromDistance(0, 0), double.NaN, "13");
				Assert.AreEqual(ValueFromDistance(0, 1), double.NaN, "14");
				Assert.AreEqual(ValueFromDistance(0, double.PositiveInfinity), double.NaN, "15");
				Assert.AreEqual(ValueFromDistance(3, 0), double.PositiveInfinity, "16");
				Assert.AreEqual(ValueFromDistance(3, 1), double.PositiveInfinity, "17");
				Assert.AreEqual(ValueFromDistance(3, double.PositiveInfinity), double.PositiveInfinity, "18");
				Width = double.NaN;
				Assert.AreEqual(ValueFromDistance(0, 0), double.NaN, "19");
				Assert.AreEqual(ValueFromDistance(0, 1), double.NaN, "20");
				Assert.AreEqual(ValueFromDistance(0, double.PositiveInfinity), double.NaN, "21");
				Assert.AreEqual(ValueFromDistance(3, 0), double.PositiveInfinity, "22");
				Assert.AreEqual(ValueFromDistance(3, 1), double.PositiveInfinity, "23");
				Assert.AreEqual(ValueFromDistance(3, double.PositiveInfinity), double.PositiveInfinity, "24");
			}
		}
		#endregion

		#region ValueFromVertical
		[Test]
		public void ValueFromVertical() {
			new ValueFromVerticalTrack();
		}

		class ValueFromVerticalTrack : Track {
			public ValueFromVerticalTrack() {
				Orientation = Orientation.Vertical;
				Assert.AreEqual(ValueFromDistance(0, 0), double.NaN, "1");
				Assert.AreEqual(ValueFromDistance(0, 1), double.NaN, "2");
				Assert.AreEqual(ValueFromDistance(0, double.PositiveInfinity), double.NaN, "3");
				Assert.AreEqual(ValueFromDistance(3, 0), double.NaN, "4");
				Assert.AreEqual(ValueFromDistance(3, 1), double.NaN, "5");
				Assert.AreEqual(ValueFromDistance(3, double.PositiveInfinity), double.NaN, "6");
				Window w = new Window();
				Canvas c = new Canvas();
				c.Children.Add(this);
				w.Content = c;
				w.Show();
				Width = 100;
				Height = 200;
				Assert.AreEqual(ValueFromDistance(0, 0), double.NaN, "7");
				Assert.AreEqual(ValueFromDistance(0, 1), double.NegativeInfinity, "8");
				Assert.AreEqual(ValueFromDistance(0, double.PositiveInfinity), double.NegativeInfinity, "9");
				Assert.AreEqual(ValueFromDistance(3, 0), double.NaN, "10");
				Assert.AreEqual(ValueFromDistance(3, 1), double.NegativeInfinity, "11");
				Assert.AreEqual(ValueFromDistance(3, double.PositiveInfinity), double.NegativeInfinity, "12");
				Maximum = 10;
				Assert.AreEqual(ValueFromDistance(0, 0), double.NaN, "13");
				Assert.AreEqual(ValueFromDistance(0, 1), double.NegativeInfinity, "14");
				Assert.AreEqual(ValueFromDistance(0, double.PositiveInfinity), double.NegativeInfinity, "15");
				Assert.AreEqual(ValueFromDistance(3, 0), double.NaN, "16");
				Assert.AreEqual(ValueFromDistance(3, 1), double.NegativeInfinity, "17");
				Assert.AreEqual(ValueFromDistance(3, double.PositiveInfinity), double.NegativeInfinity, "18");
				Width = double.NaN;
				Assert.AreEqual(ValueFromDistance(0, 0), double.NaN, "19");
				Assert.AreEqual(ValueFromDistance(0, 1), double.NegativeInfinity, "20");
				Assert.AreEqual(ValueFromDistance(0, double.PositiveInfinity), double.NegativeInfinity, "21");
				Assert.AreEqual(ValueFromDistance(3, 0), double.NaN, "22");
				Assert.AreEqual(ValueFromDistance(3, 1), double.NegativeInfinity, "23");
				Assert.AreEqual(ValueFromDistance(3, double.PositiveInfinity), double.NegativeInfinity, "24");
			}
		}
		#endregion

		#region MeasureOnThumb
		[Test]
		public void MeasureOnThumb() {
			Track t = new Track();
			t.Thumb = new MeasureOnThumbThumb();
			MeasureOnThumbThumb.ShouldLog = true;
			t.Measure(new Size(100, 100));
			Assert.AreEqual(MeasureOnThumbThumb.Constraint.Width, 100, "Width");
			Assert.AreEqual(MeasureOnThumbThumb.Constraint.Height, 100, "Height");
		}

		class MeasureOnThumbThumb : Thumb {
			static public bool ShouldLog;
			static public Size Constraint;
			protected override Size MeasureOverride(Size constraint) {
				if (ShouldLog)
					Constraint = constraint;
				return base.MeasureOverride(constraint);
			}
		}
		#endregion

		#region ArrangeOnThumb
		[Test]
#if Implementation
		[Ignore]
#endif
		public void ArrangeOnThumb() {
			Track t = new Track();
			t.Thumb = new ArrangeOnThumbThumb();
			ArrangeOnThumbThumb.ShouldLog = true;
			t.Arrange(new Rect(0, 0, 100, 100));
			Assert.AreEqual(ArrangeOnThumbThumb.ArrangeBounds.Width, 4, "Width");
			Assert.AreEqual(ArrangeOnThumbThumb.ArrangeBounds.Height, 100, "Height");
		}

		class ArrangeOnThumbThumb : Thumb {
			static public bool ShouldLog;
			static public Size ArrangeBounds;
			protected override Size ArrangeOverride(Size arrangeBounds) {
				if (ShouldLog)
					ArrangeBounds = arrangeBounds;
				return base.ArrangeOverride(arrangeBounds);
			}
		}
		#endregion

		#region MeasureOnThumbMaximumEqualMinimum
		[Test]
		public void MeasureOnThumbMaximumEqualMinimum() {
			Track t = new Track();
			t.Maximum = 2;
			t.Minimum = 2;
			t.Thumb = new MeasureOnThumbMaximumEqualMinimumThumb();
			MeasureOnThumbMaximumEqualMinimumThumb.ShouldLog = true;
			t.Measure(new Size(100, 100));
			Assert.AreEqual(MeasureOnThumbMaximumEqualMinimumThumb.Constraint.Width, 100, "Width");
			Assert.AreEqual(MeasureOnThumbMaximumEqualMinimumThumb.Constraint.Height, 100, "Height");
		}

		class MeasureOnThumbMaximumEqualMinimumThumb : Thumb {
			static public bool ShouldLog;
			static public Size Constraint;
			protected override Size MeasureOverride(Size constraint) {
				if (ShouldLog)
					Constraint = constraint;
				return base.MeasureOverride(constraint);
			}
		}
		#endregion

		#region ArrangeOnThumbMaximumEqualMinimum
		[Test]
#if Implementation
		[Ignore]
#endif
		public void ArrangeOnThumbMaximumEqualMinimum() {
			Track t = new Track();
			t.Maximum = 2;
			t.Minimum = 2;
			t.Thumb = new ArrangeOnThumbMaximumEqualMinimumThumb();
			ArrangeOnThumbMaximumEqualMinimumThumb.ShouldLog = true;
			t.Arrange(new Rect(0, 0, 100, 100));
			Assert.AreEqual(ArrangeOnThumbMaximumEqualMinimumThumb.ArrangeBounds.Width, 4, "Width");
			Assert.AreEqual(ArrangeOnThumbMaximumEqualMinimumThumb.ArrangeBounds.Height, 100, "Height");
			Rect b = VisualTreeHelper.GetContentBounds(t.Thumb);
			Assert.IsTrue(double.IsPositiveInfinity(b.Left), "Left");
			Assert.IsTrue(double.IsPositiveInfinity(b.Top), "Top");
		}

		class ArrangeOnThumbMaximumEqualMinimumThumb : Thumb {
			static public bool ShouldLog;
			static public Size ArrangeBounds;
			protected override Size ArrangeOverride(Size arrangeBounds) {
				if (ShouldLog)
					ArrangeBounds = arrangeBounds;
				return base.ArrangeOverride(arrangeBounds);
			}
		}
		#endregion

		//FIXME: This one seems to cause others fail.
#if !Implementation
		[Test]
		public void ValueAdjustments() {
			Track t = new Track();
			t.Value = -1;
			Assert.AreEqual(t.Value, -1, "1");
			t.Value = 2;
			Assert.AreEqual(t.Value, 2, "2");
			ScrollBar s = new ScrollBar();
			Window w = new Window();
			w.Content = s;
			w.Show();
			s.Track.Value = 2;
			Assert.AreEqual(s.Track.Value, 2, "3");
			s.Track.Value = -1;
			Assert.AreEqual(s.Track.Value, -1, "4");
		}
#endif

		[Test]
		public void Simple() {
			Track t = new Track();
			t.ValueFromDistance(0, 0);
			Canvas c = new Canvas();
			c.Children.Add(t);
			Window w = new Window();
			w.Content = c;
			w.Show();
		}

		#region MeasureOverride
		[Test]
		public void MeasureOverride() {
			new MeasureOverrideTrack();
		}

		class MeasureOverrideTrack : Track {
			public MeasureOverrideTrack() {
				Size result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 0, "Width 1");
				Assert.AreEqual(result.Height, 0, "Height 1");
				Thumb = new Thumb();
				result = MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Assert.AreEqual(result.Width, 4, "Width 2");
				Assert.AreEqual(result.Height, 4, "Height 2");
				Assert.AreEqual(Thumb.DesiredSize.Width, 4, "Width 3");
				Assert.AreEqual(Thumb.DesiredSize.Height, 4, "Height 3");
			}
		}
		#endregion

		[Test]
		public void ValueBindingScrollBar() {
			ScrollBar s = new ScrollBar();
			s.Value = 0.5;
			Window w = new Window();
			w.Content = s;
			w.Show();
			Assert.AreEqual(s.Track.Value, 0.5);
		}


		#region ValueBindingSlider
		[Test]
		public void ValueBindingSlider() {
			ValueBindingSliderSlider s = new ValueBindingSliderSlider();
			s.Value = 0.5;
			Window w = new Window();
			w.Content = s;
			w.Show();
			Assert.AreEqual(s.TrackValue(), 0.5);
		}

		class ValueBindingSliderSlider : Slider {
			public double TrackValue() {
				return ((Track)GetTemplateChild("PART_Track")).Value;
			}
		}
		#endregion

		[Test]
		public void ThumbLargerThanTrack() {
			Track track = new Track();
			track.Width = 100;
			track.Height = 100;
			Thumb thumb = new Thumb();
			thumb.Width = 200;
			thumb.Height = 200;
			track.Thumb = thumb;
			track.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual(track.DesiredSize.Width, 100, "track.DesiredSize.Width");
			Assert.AreEqual(thumb.DesiredSize.Width, 100, "thumb.DesiredSize.Width");
			Assert.AreEqual(VisualTreeHelper.GetContentBounds(thumb), Rect.Empty, "VisualTreeHelper.GetContentBounds(thumb)");

		}

		[Test]
		public void SettingIncreaseRepeatButtonTwice() {
			Thread runner = new Thread(delegate() {
				Track s = new Track();
				Window w = new Window();
				w.Content = s;
				w.Show();
				s.IncreaseRepeatButton = new RepeatButton();
				Thread current = Thread.CurrentThread;
				Thread killer = new Thread(delegate() {
					Thread.Sleep(5000);
					current.Abort();
				});
				killer.Start();
				s.IncreaseRepeatButton = new RepeatButton();
				killer.Abort();
				Assert.Fail();
			});
			runner.SetApartmentState(ApartmentState.STA);
			runner.Start();
		}

		[Test]
		public void SettingDecreaseRepeatButtonTwice() {
			Thread runner = new Thread(delegate() {
				Track s = new Track();
				Window w = new Window();
				w.Content = s;
				w.Show();
				s.DecreaseRepeatButton = new RepeatButton();
				Thread current = Thread.CurrentThread;
				Thread killer = new Thread(delegate() {
					Thread.Sleep(5000);
					current.Abort();
				});
				killer.Start();
				s.DecreaseRepeatButton = new RepeatButton();
				killer.Abort();
				Assert.Fail();
			});
			runner.SetApartmentState(ApartmentState.STA);
			runner.Start();
		}

		[Test]
		public void SettingThumbTwice() {
			Thread runner = new Thread(delegate() {
				Track s = new Track();
				Window w = new Window();
				w.Content = s;
				w.Show();
				s.Thumb = new Thumb();
				Thread current = Thread.CurrentThread;
				Thread killer = new Thread(delegate() {
					Thread.Sleep(5000);
					current.Abort();
				});
				killer.Start();
				s.Thumb = new Thumb();
				killer.Abort();
				Assert.Fail();
			});
			runner.SetApartmentState(ApartmentState.STA);
			runner.Start();
		}

		[Test]
		public void PartBounds() {
			Window w = new Window();
			Track s = new Track();
			s.Thumb = new Thumb();
			w.Content = s;
			w.Show();
			s.Value = s.Maximum;
			w.Width = 100;
			w.Height = 100;
			Assert.AreEqual(s.Thumb.ActualWidth, 4, "Thumb.ActualWidth");
			Assert.AreEqual(s.Thumb.ActualHeight, 66, "Thumb.ActualHeight");
		}
	}
}