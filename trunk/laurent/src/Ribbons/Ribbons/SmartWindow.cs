using System;
using System.Collections.Generic;
using Gtk;

namespace Ribbons
{
	public class SmartWindow : Window
	{
		private List<Container> lastHoveredContainers;
		
		public SmartWindow(WindowType type) : base(type)
		{
			lastHoveredContainers = new List<Gtk.Container> ();
		}
		
		protected override bool OnWidgetEvent (Gdk.Event evnt)
		{
			switch(evnt.Type)
			{
				case Gdk.EventType.ButtonPress:
				case Gdk.EventType.ButtonRelease:
				case Gdk.EventType.ThreeButtonPress:
				case Gdk.EventType.TwoButtonPress:
					Gdk.EventButton eb = new Gdk.EventButton (evnt.Handle);
					return PropagateEventGivenCoordinate (evnt, (int)eb.X, (int)eb.Y);
				case Gdk.EventType.MotionNotify:
					Gdk.EventMotion em = new Gdk.EventMotion (evnt.Handle);
					return PropagateEventGivenCoordinate (evnt, (int)em.X, (int)em.Y);
				/*case Gdk.EventType.EnterNotify:
					break;
				case Gdk.EventType.LeaveNotify:
					break;*/
				default:
					return base.OnWidgetEvent (evnt);
			}
		}
		
		private bool PropagateEventGivenCoordinate (Gdk.Event evnt, int x, int y)
		{
			Container current = this;
			Widget match = this;
			do
			{
				Container next = null;
				foreach(Widget child in current.Children)
				{
					Gdk.Rectangle alloc = child.Allocation;
					if(alloc.Contains(x, y))
					{
						match = child;
						next = child as Container;
						break;
					}
				}
				current = next;
			} while (current != null);
			
			if(match == this)
			{
				return base.OnWidgetEvent (evnt);
			}
			else
			{
				match.ProcessEvent (evnt);
				return true;
			}
		}
	}
}
