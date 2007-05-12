using Cairo;
using Gtk;
using System;

namespace Ribbons
{
	public class RibbonGroup : Widget
	{
		protected Theme theme = new Theme ();
		protected string lbl;
		
		public string Label
		{
			set { lbl = value; }
			get { return lbl; }
		}
		
		public RibbonGroup ()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.ExposeEvent += OnExpose; 
		}
		
		protected void Draw (Context cr)
		{
			Rectangle rect = new Rectangle (Allocation.X, Allocation.Y, Allocation.Width, Allocation.Height);
			theme.DrawGroup (cr, rect, this);
		}
		
		[GLib.ConnectBefore]
		protected void OnExpose (object sender, ExposeEventArgs args)
		{
			Gdk.EventExpose evnt = args.Event;
			Context cr = Gdk.CairoHelper.Create(this.GdkWindow);
			
			cr.Rectangle(evnt.Area.X, evnt.Area.Y, evnt.Area.Width, evnt.Area.Height);
			cr.Clip();
			Draw(cr);
			
			args.RetVal = false;
		}
	}
}