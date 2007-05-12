using Cairo;
using Gtk;
using System;
using System.Threading;

namespace Ribbons
{
	public class Clock : Widget
	{
		protected Timer timer;
		
		public Clock()
		{
			this.SetFlag(WidgetFlags.NoWindow);
			
			this.AddEvents((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.ExposeEvent += OnExpose; 
			
			timer = new Timer(FaceUpdate, null, new TimeSpan(0, 0, 1), new TimeSpan(0, 0, 1));
		}
		
		protected void Draw(Context cr)
		{
			double x, y;
			double radius;
			int i;
			int hours, minutes, seconds;
			
			x = Allocation.Width / 2;
			y = Allocation.Height / 2;
			radius = Math.Min(Allocation.Width / 2,
				      Allocation.Height / 2) - 5;

			/* clock back */
			cr.Arc(x, y, radius, 0, 2 * Math.PI);
			cr.SetSourceRGB(1, 1, 1);
			cr.FillPreserve();
			cr.SetSourceRGB(0, 0, 0);
			cr.Stroke();

			/* clock ticks */
			for (i = 0; i < 12; i++)
			{
				double inset;
				
				cr.Save(); /* stack-pen-size */
				
				if (i % 3 == 0)
				{
					inset = 0.2 * radius;
				}
				else
				{
					inset = 0.1 * radius;
					cr.LineWidth *= 0.5;
				}
				
				cr.MoveTo(
						x + (radius - inset) * Math.Cos (i * Math.PI / 6),
						y + (radius - inset) * Math.Sin (i * Math.PI / 6));
				cr.LineTo(
						x + radius * Math.Cos (i * Math.PI / 6),
						y + radius * Math.Sin (i * Math.PI / 6));
				cr.Stroke();
				cr.Restore(); /* stack-pen-size */
			}

			/* clock hands */
			hours = DateTime.Now.Hour;
			minutes = DateTime.Now.Minute;
			seconds = DateTime.Now.Second;
			/* hour hand:
			 * the hour hand is rotated 30 degrees (pi/6 r) per hour +
			 * 1/2 a degree (pi/360 r) per minute
			 */
			cr.Save();
			cr.LineWidth *= 2.5;
			cr.MoveTo(x, y);
			cr.LineTo(x + radius / 2 * Math.Sin (Math.PI / 6 * hours +
								 Math.PI / 360 * minutes),
					   y + radius / 2 * -Math.Cos (Math.PI / 6 * hours +
						   		 Math.PI / 360 * minutes));
			cr.Stroke();
			cr.Restore();
			/* minute hand:
			 * the minute hand is rotated 6 degrees (pi/30 r) per minute
			 */
			cr.MoveTo(x, y);
			cr.LineTo(x + radius * 0.75 * Math.Sin (Math.PI / 30 * minutes),
					   y + radius * 0.75 * -Math.Cos (Math.PI / 30 * minutes));
			cr.Stroke();
			/* seconds hand:
			 * operates identically to the minute hand
			 */
			cr.Save();
			cr.SetSourceRGB(1, 0, 0); /* red */
			cr.MoveTo(x, y);
			cr.LineTo(x + radius * 0.7 * Math.Sin (Math.PI / 30 * seconds),
					   y + radius * 0.7 * -Math.Cos (Math.PI / 30 * seconds));
			cr.Stroke();
			cr.Restore();
		}
		
		[GLib.ConnectBefore]
		protected void OnExpose(object sender, ExposeEventArgs args)
		{
			Gdk.EventExpose evnt = args.Event;
			Context cr = Gdk.CairoHelper.Create(this.GdkWindow);
			
			cr.Rectangle(evnt.Area.X, evnt.Area.Y, evnt.Area.Width, evnt.Area.Height);
			cr.Clip();
			Draw(cr);
			
			args.RetVal = false;
		}
		
		protected void FaceUpdate(object state)
		{
			RedrawCanvas();
		}
		
		protected void RedrawCanvas()
		{
			if(this.GdkWindow == null) return;
			
			using(Gdk.Region region = this.GdkWindow.ClipRegion)
			{
				this.GdkWindow.InvalidateRegion(region, true);
				this.GdkWindow.ProcessUpdates(true);
			}
		}
	}
}