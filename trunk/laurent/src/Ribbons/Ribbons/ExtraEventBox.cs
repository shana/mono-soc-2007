using Gtk;
using System;

namespace Ribbons
{
	public class ExtraEventBox : EventBox
	{
		protected override bool OnWidgetEvent (Gdk.Event evnt)
		{
			if(evnt.Window.Equals (this.GdkWindow))
			{
				if(evnt.Type != Gdk.EventType.Expose)
				{
					if(Child != null) Child.ProcessEvent (evnt);
				}
			}
			return base.OnWidgetEvent (evnt);
		}
	}
}
