using NUnit.Framework;
using System.Collections;
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
	public class ScrollBarTest {
		[Test]
		public void Creation() {
			ScrollBar p = new ScrollBar();
			Assert.AreEqual(p.Value, 0, "Value");
			Assert.AreEqual(p.Minimum, 0, "Minimum");
			Assert.AreEqual(p.Maximum, 1, "Maximum");
			Assert.AreEqual(p.SmallChange, 0.1, "SmallChange");
			Assert.AreEqual(p.LargeChange, 1, "LargeChange");
			Assert.AreEqual(p.Orientation, Orientation.Vertical, "Orientation");
			Assert.IsNull(p.Track, "Track");
			Assert.IsTrue(p.IsEnabled, "IsEnabled");
			Assert.AreEqual(p.CommandBindings.Count, 0, "CommandBindings.Count");
			Assert.IsNull(p.ContextMenu, "ContextMenu");
			Assert.IsFalse(p.Focusable, "Focusable");
		}

		[Test]
		public void OnApplyTemplate() {
			ScrollBar p = new ScrollBar();
			p.OnApplyTemplate();
			Assert.IsNull(p.Track, "Track");
			Assert.IsTrue(p.IsEnabled, "IsEnabled");
			Assert.AreEqual(p.CommandBindings.Count, 0, "CommandBindings.Count");
			Window w = new Window();
			w.Content = p;
			p.OnApplyTemplate();
			Assert.IsNull(p.Track, "Track 2");
			Assert.IsTrue(p.IsEnabled, "IsEnabled 2");
			Assert.AreEqual(p.CommandBindings.Count, 0, "CommandBindings.Count");
			w.Show();
			Assert.IsTrue(p.Track.IsEnabled, "Track.IsEnabled");
			Assert.IsTrue(p.Track.IncreaseRepeatButton.IsEnabled, "Track.IncreaseRepeatButton.IsEnabled");
		}

		#region IsEnabledCore
		[Test]
		public void IsEnabledCore() {
			new IsEnabledCoreScrollBar();
		}
		
		class IsEnabledCoreScrollBar : ScrollBar {
			public IsEnabledCoreScrollBar() {
				Assert.IsTrue(IsEnabledCore, "IsEnabledCore 1");
				Assert.IsTrue(IsEnabled, "IsEnabled 1");
				IsEnabled = false;
				Assert.IsTrue(IsEnabledCore, "IsEnabledCore 2");
				Assert.IsFalse(IsEnabled, "IsEnabled 2");
				Minimum = Maximum;
				Assert.IsFalse(IsEnabledCore, "IsEnabledCore 3");
				Assert.IsFalse(IsEnabled, "IsEnabled 3");
				IsEnabled = true;
				Assert.IsFalse(IsEnabledCore, "IsEnabledCore 4");
				Assert.IsFalse(IsEnabled, "IsEnabled 4");
			}
		}
		#endregion

		[Test]
		public void Command() {
			ScrollBar s = new ScrollBar();
			ScrollBar.LineDownCommand.Execute(null, s);
			Assert.AreEqual(s.Value, s.SmallChange, "Value after LineDown");
			s.Value = s.Maximum;
			Assert.IsTrue(ScrollBar.LineDownCommand.CanExecute(null, s), "Can execute LineDown");
			ScrollBar.LineDownCommand.Execute(null, s);
			Assert.AreEqual(s.Value, s.Maximum, "Value after LineDown 2");
		}

		[Test]
		public void RepeatButtonCommand() {
			ScrollBar s = new ScrollBar();
			Window w = new Window();
			w.Content = s;
			w.Show();
			Assert.AreEqual(s.Track.DecreaseRepeatButton.Command, ScrollBar.PageUpCommand, "Command");
			Assert.IsNull(s.Track.DecreaseRepeatButton.CommandTarget, "CommandTarget");
			Assert.IsNull(s.Track.DecreaseRepeatButton.CommandParameter, "CommandParameter");
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void SettingIncreaseRepeatButtonTwice() {
			ScrollBar s = new ScrollBar();
			Window w = new Window();
			w.Content = s;
			w.Show();
			s.Track.IncreaseRepeatButton = new RepeatButton();
		}
		
		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void SettingDecreaseRepeatButtonTwice() {
			ScrollBar s = new ScrollBar();
			Window w = new Window();
			w.Content = s;
			w.Show();
			s.Track.DecreaseRepeatButton = new RepeatButton();
		}

		[Test]
		public void SettingThumbTwice() {
			Thread runner = new Thread(delegate() {
				ScrollBar s = new ScrollBar();
				Window w = new Window();
				w.Content = s;
				w.Show();
				Thread current = Thread.CurrentThread;
				Thread killer = new Thread(delegate() {
					Thread.Sleep(1000);
					current.Abort();
				});
				killer.Start();
				s.Track.Thumb = new Thumb();
				killer.Abort();
				Assert.Fail();
			});
			runner.SetApartmentState(ApartmentState.STA);
			runner.Start();
		}

		[Test]
		public void SnapsToDevicePixels() {
			ScrollBar s = new ScrollBar();
			Window w = new Window();
			w.Content = s;
			w.Show();
			Assert.IsFalse(s.SnapsToDevicePixels, "ScrollBar");
			Assert.IsTrue(s.Track.SnapsToDevicePixels, "Track");
			Assert.IsTrue(s.Track.Thumb.SnapsToDevicePixels, "Track.Thumb");
			Assert.IsTrue(s.Track.IncreaseRepeatButton.SnapsToDevicePixels, "Track.IncreaseRepeatButton");
			Assert.IsTrue(s.Track.DecreaseRepeatButton.SnapsToDevicePixels, "Track.DecreaseRepeatButton");
		}
	}
}