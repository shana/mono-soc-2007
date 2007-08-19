using NUnit.Framework;
using System.Windows.Shapes;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class ProgressBarTest
	{
		[Test]
		public void Creation ()
		{
			ProgressBar p = new ProgressBar ();
			Assert.IsFalse (p.IsIndeterminate, "IsIndeterminate");
			Assert.AreEqual (p.Orientation, Orientation.Horizontal, "Orientation");
			Assert.AreEqual (p.Value, 0, "Value");
			Assert.AreEqual (p.Minimum, 0, "Minimum");
			Assert.AreEqual (p.Maximum, 100, "Maximum");
			Assert.AreEqual (p.SmallChange, 0.1, "SmallChange");
			Assert.AreEqual (p.LargeChange, 1, "LargeChange");
		}

		#region OnApplyTemplate
		[Test]
		public void OnApplyTemplate ()
		{
			new OnApplyTemplateProgressBar ();
		}

		class OnApplyTemplateProgressBar : ProgressBar
		{
			public OnApplyTemplateProgressBar ()
			{
				Value = 30;
				Assert.IsNull (GetTemplateChild ("PART_Track"), "Initially");
				OnApplyTemplate ();
				Assert.IsNull (GetTemplateChild ("PART_Track"), "OnApplyTemplate");
				ApplyTemplate ();
				Assert.IsNull (GetTemplateChild ("PART_Track"), "ApplyTemplate");
			}
		}
		#endregion

		#region IndicatorLenght
		[Test]
		[Ignore ("This does not work under Windows XP. I will try later on Windows Vista.")]
		public void IndicatorLenght ()
		{
			new IndicatorLenghtProgressBar ();
		}

		class IndicatorLenghtProgressBar : ProgressBar
		{
			public IndicatorLenghtProgressBar ()
			{
				Window w = new Window ();
				w.Content = this;
				w.Show ();
				Value = Maximum;
				Assert.AreEqual (Indicator.Width, Track.Width, "1");
				Value = Maximum / 2;
				Assert.AreEqual (Indicator.Width, Track.Width / 2, "2");
			}

			FrameworkElement Track {
				get {
					return (FrameworkElement)GetTemplateChild ("PART_Track");
				}
			}

			FrameworkElement Indicator {
				get {
					return (FrameworkElement)GetTemplateChild ("Indicator");
				}
			}
		}
		#endregion
	}
}