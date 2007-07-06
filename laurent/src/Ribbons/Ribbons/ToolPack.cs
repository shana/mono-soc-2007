using System;
using Cairo;
using Gtk;

namespace Ribbons
{
	public class ToolPack : Container
	{
		public ToolPack ()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
		}
		
		public void AddButton ()
		{
			
		}
	}
}
