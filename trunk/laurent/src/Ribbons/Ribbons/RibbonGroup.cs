using Cairo;
using Gtk;
using System;

namespace Ribbons
{
	public class RibbonGroup : Bin
	{
		protected Theme theme = new Theme ();
		protected string lbl;
		protected Pango.Layout lbl_layout;
		
		public string Label
		{
			set
			{
				lbl = value;
				
				if(lbl == null)
					lbl_layout = null;
				else if(lbl_layout == null)
					lbl_layout = CreatePangoLayout (this.lbl);
				else
					lbl_layout.SetText (lbl);
			}
			get { return lbl; }
		}
		
		public RibbonGroup ()
		{
			// This is a No Window widget => it does not have its own Gdk Window => it can be transparent
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			Label = null;
			HeightRequest = 87;
			BorderWidth = 0;
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			if(WidthRequest == -1)
			{
				if(Child != null && Child.Visible)
				{
					Requisition childRequisition = Child.SizeRequest ();
					requisition.Width = theme.SizeRequestedByGroup (this, lbl_layout, childRequisition).Width; 
				}
				else
				{
					requisition.Width = theme.SizeRequestedByGroup (this, lbl_layout).Width;
				}
			}
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			if(Child != null && Child.Visible)
			{
				Child.SizeAllocate (theme.SizeAllocatedToChildByGroup (this, lbl_layout, allocation));
			}
		}
		
		protected void Draw (Context cr)
		{
			Rectangle rect = new Rectangle (Allocation.X, Allocation.Y, Allocation.Width, Allocation.Height);
			theme.DrawGroup (cr, rect, lbl_layout, this);
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			Context cr = Gdk.CairoHelper.Create (this.GdkWindow);
			
			cr.Rectangle (evnt.Area.X, evnt.Area.Y, evnt.Area.Width, evnt.Area.Height);
			cr.Clip ();
			Draw (cr);
			
			return base.OnExposeEvent (evnt);
		}
	}
}