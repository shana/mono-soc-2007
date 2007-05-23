using NUnit.Framework;
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
	public class RangeBaseTest {
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidMinimmum() {
			new InvalidMinimmumRangeBase();
		}
		class InvalidMinimmumRangeBase : RangeBase {
			public InvalidMinimmumRangeBase() {
				Minimum = double.NegativeInfinity;
			}
		}
    }
}