using System;
using Cairo;
using Gdk;
using Gtk;

namespace Ribbons
{
	public class RibbonButton : Widget
	{
		protected override bool OnExposeEvent (EventExpose evnt)
		{
			using(Drawable drw = this.GdkWindow)
			{
				Context ctxt = Gdk.CairoHelper.Create (drw);
				Gdk.Rectangle area = evnt.Area;
				ctxt.Rectangle (area.X, area.Y, area.Width, area.Height);
				ctxt.Clip ();
				
				int w, h;
				drw.GetSize(out w, out h);
				Console.WriteLine (w + " " + h);
				Draw (ctxt, w, h);
				
				return false;
			}
		}

		protected virtual void Draw (Context Ctxt, int Width, int Height)
		{
			Ctxt.Antialias = Antialias.Subpixel;
			//Ctxt.Scale (Width, Height);
			
			/*Ctxt.LineWidth = 1.0;
			Ctxt.Color = new Cairo.Color (0, 0, 0);
			Ctxt.MoveTo (0, 0);
			Ctxt.LineTo (Width, Height);
			Ctxt.Stroke ();*/
			
			Ctxt.Arc (0, 0, Math.Min (Width, Height), 0, 2 * Math.PI);
			Ctxt.Color = new Cairo.Color (0, 0, 0);
			Ctxt.FillPreserve ();
			Ctxt.Color = new Cairo.Color (1, 1, 1);
			Ctxt.Stroke ();
		}
	}
}
