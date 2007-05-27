using NUnit.Framework;
using System.Collections.Generic;
using System.Windows.Input;
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
	public class ThumbTest {
		[Test]
#if Implementation
		[Ignore]
#endif
		public void Creation() {
			Thumb t = new Thumb();
			Assert.AreEqual(t.Background, null, "Background");
			Assert.AreEqual(((SolidColorBrush)t.Foreground).Color.A, 0xFF, "Foreground");
		}

		[Test]
#if Implementation
		[Ignore]
#endif
		public void Style() {
			Thumb t = new Thumb();
			Assert.IsNull(t.Style, "Style");
		}

		#region OrderOfEvents
		[Test]
		public void OrderOfEvents() {
			new OrderOfEventsThumb();
		}

		class OrderOfEventsThumb : Thumb {
			List<string> events = new List<string>();

			public OrderOfEventsThumb() {
				DragStarted += new DragStartedEventHandler(OrderOfEventsThumb_DragStarted);
				DragDelta += new DragDeltaEventHandler(OrderOfEventsThumb_DragDelta);
				DragCompleted += new DragCompletedEventHandler(OrderOfEventsThumb_DragCompleted);
				MouseButtonEventArgs e = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left);
				e.RoutedEvent = MouseLeftButtonDownEvent;
				OnMouseLeftButtonDown(e);
				Assert.AreEqual(events.Count, 1);
			}

			void OrderOfEventsThumb_DragStarted(object sender, DragStartedEventArgs e) {
				events.Add("DragStarted");
			}

			void OrderOfEventsThumb_DragDelta(object sender, DragDeltaEventArgs e) {
				events.Add("DragDelta");
			}

			void OrderOfEventsThumb_DragCompleted(object sender, DragCompletedEventArgs e) {
				events.Add("DragCompleted");
			}
		}
		#endregion
    }
}