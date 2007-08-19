using NUnit.Framework;
using System.Threading;
using System.Windows.Media;
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
	public class TrackTest
	{
		[Test]
		public void PartBounds ()
		{
			Window w = new Window ();
			Track s = new Track ();
			s.Thumb = new Thumb ();
			w.Content = s;
			w.Show ();
			s.Value = s.Maximum;
			w.Width = 100;
			w.Height = 100;
			Assert.AreEqual (s.Thumb.ActualWidth, 4, "Thumb.ActualWidth");
			Assert.AreEqual (s.Thumb.ActualHeight, s.ActualHeight, "Thumb.ActualHeight");
		}
	}
}