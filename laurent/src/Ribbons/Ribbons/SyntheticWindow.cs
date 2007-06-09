using System;
using System.Collections.Generic;
using Gtk;

namespace Ribbons
{
	public class SyntheticWindow : Window
	{
		private List<Widget> lastHoveredWidgets;
		
		public SyntheticWindow (WindowType type) : base (type)
		{
			lastHoveredWidgets = new List<Gtk.Widget> ();
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
			Container current = this;	// Current container containing the coordinate
			Widget match = this;	// Current match for the position
			int pos = 0;	// Current position in lastHoveredWidgets
			do
			{
				Container next = null;
				
				if(lastHoveredWidgets.Count > pos)	// Does it has a candidate ?
				{
					Widget candidate = lastHoveredWidgets[pos];
					if(candidate.Parent == current)	// Is it still a child of the current container ?
					{
						Gdk.Rectangle alloc = candidate.Allocation;
						if(alloc.Contains (x, y))	// Does it contain the coordinate ?
						{
							match = candidate;	// The candiate is the winner!
							next = candidate as Container;
						}
					}
				}
				
				foreach(Widget child in current.Children)
				{
					Gdk.Rectangle alloc = child.Allocation;
					if(alloc.Contains (x, y))
					{
						match = child;
						next = child as Container;
						break;
					}
				}
				
				if(match != current)	// Has a child been found ?
				{
					if(lastHoveredWidgets.Count > pos)	// Is there enough room ?
						lastHoveredWidgets[pos] = match;
					else
						lastHoveredWidgets.Add(match);
					++pos;
				}
				
				current = next;
			} while (current != null);
			
			// Remove excess widgets kept from last time
			lastHoveredWidgets.RemoveRange (pos, lastHoveredWidgets.Count - pos);
			
			if(match == this)	// A widget has been found, let's send it the event
			{
				return base.OnWidgetEvent (evnt);
			}
			else	// No widget found, the window keeps the event
			{
				match.ProcessEvent (evnt);
				return true;
			}
		}
	}
}
