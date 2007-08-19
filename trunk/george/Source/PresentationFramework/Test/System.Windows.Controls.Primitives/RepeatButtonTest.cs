using NUnit.Framework;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives
{
#else
namespace System.Windows.Controls.Primitives {
#endif
	[TestFixture]
	public class RepeatButtonTest
	{
		[Test]
		public void StaticProperties ()
		{
			Assert.AreEqual (RepeatButton.DelayProperty.Name, "Delay", "DelayProperty.Name");
			Assert.AreEqual (RepeatButton.DelayProperty.OwnerType, typeof (RepeatButton), "DelayProperty.OwnerType");
			Assert.AreEqual (RepeatButton.DelayProperty.PropertyType, typeof (int), "DelayProperty.PropertyType");
			Assert.LessOrEqual ((int)RepeatButton.DelayProperty.DefaultMetadata.DefaultValue - GetSystemDelay (), 10, "DelayProperty.DefaultMetadata.DefaultValue");

			Assert.AreEqual (RepeatButton.IntervalProperty.Name, "Interval", "IntervalProperty.Name");
			Assert.AreEqual (RepeatButton.IntervalProperty.OwnerType, typeof (RepeatButton), "IntervalProperty.OwnerType");
			Assert.AreEqual (RepeatButton.IntervalProperty.PropertyType, typeof (int), "IntervalProperty.PropertyType");
			Assert.LessOrEqual (Math.Abs (1 - (int)RepeatButton.IntervalProperty.DefaultMetadata.DefaultValue / GetSystemInterval ()), 0.2, "IntervalProperty.DefaultMetadata.DefaultValue");
		}

		static int GetSystemDelay ()
		{
			return 250 * (SystemParameters.KeyboardDelay + 1);
		}

		static int GetSystemInterval ()
		{
			const double minimum_rate = 2.5;
			const double maximum_rate = 30;
			return (int)(1000 / (minimum_rate + (maximum_rate - minimum_rate) / 31 * SystemParameters.KeyboardSpeed));
		}

		[Test]
		public void IsPressed ()
		{
			RepeatButton repeat_button = new RepeatButton ();
			Assert.IsNull (RepeatButton.IsPressedProperty.DefaultMetadata.PropertyChangedCallback, "RepeatButton.IsPressedProperty.DefaultMetadata.PropertyChangedCallback");
		}

		[Test]
		public void InvalidValues ()
		{
			RepeatButton repeat_button = new RepeatButton ();
			const int minimum_delay = 0;
			repeat_button.SetValue (RepeatButton.DelayProperty, minimum_delay);
			try {
				repeat_button.SetValue (RepeatButton.DelayProperty, minimum_delay - 1);
				Assert.Fail ("Delay");
			} catch (ArgumentException) {
			}
			const int minimum_interval = 1;
			repeat_button.SetValue (RepeatButton.IntervalProperty, minimum_interval);
			try {
				repeat_button.SetValue (RepeatButton.IntervalProperty, minimum_interval - 1);
				Assert.Fail ("Interval");
			} catch (ArgumentException) {
			}
		}

		[Test]
		public void ClickMode ()
		{
			Assert.AreEqual (new RepeatButton ().ClickMode, global::System.Windows.Controls.ClickMode.Press);
		}
	}
}