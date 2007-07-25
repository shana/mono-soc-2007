using System;
using Gtk;

namespace Ribbons
{
	public class Tile : Widget
	{
		public Tile()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
		}
	}
}
