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
	public class RangeBaseTest
	{
		#region InvalidMinimmum
		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void InvalidMinimmum ()
		{
			new InvalidMinimmumRangeBase ();
		}
		class InvalidMinimmumRangeBase : RangeBase
		{
			public InvalidMinimmumRangeBase ()
			{
				Minimum = double.NegativeInfinity;
			}
		}
		#endregion

		#region ValueOutOfRange
		[Test]
		public void ValueOutOfRange ()
		{
			new ValueOutOfRangeRangeBase ();
		}
		class ValueOutOfRangeRangeBase : RangeBase
		{
			public ValueOutOfRangeRangeBase ()
			{
				Value = Maximum + 1;
				Assert.AreEqual (Value, Maximum, "Value");
				Assert.AreEqual (call_count, 1, "OnValueChanged calls");
			}
			int call_count;
			protected override void OnValueChanged (double oldValue, double newValue)
			{
				base.OnValueChanged (oldValue, newValue);
				call_count++;
			}
		}
		#endregion

		#region MaximumAdjustsValue
		[Test]
		public void MaximumAdjustsValue ()
		{
			new MaximumAdjustsValueRangeBase ();
		}
		class MaximumAdjustsValueRangeBase : RangeBase
		{
			public MaximumAdjustsValueRangeBase ()
			{
				Maximum = Value - 1;
				Assert.AreEqual (Value, Maximum, "Value");
				Assert.AreEqual (call_count, 0, "OnValueChanged calls");
			}
			int call_count;
			protected override void OnValueChanged (double oldValue, double newValue)
			{
				base.OnValueChanged (oldValue, newValue);
				call_count++;
			}
		}
		#endregion
	}
}