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
		protected Gtk.Button expand;
		
		private double barHeight;
		
		protected double childPadding = 1.0;
		protected double lineWidth = 1.0;
		protected double space = 2.0;
		
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
			HeightRequest = 90;
			
			expand = new Gtk.Button ("+");
			expand.Parent = this;
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			base.ForAll (include_internals, callback);
			callback (expand);
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			int lw, lh;
			lbl_layout.GetPixelSize (out lw, out lh);
			
			Requisition childRequisition = new Requisition ();
			if(Child != null && Child.Visible)
			{
				childRequisition = Child.SizeRequest ();
			}
			
			barHeight = lh + 2 * space;
			if(expand != null && expand.Visible)
			{
				expand.SetSizeRequest (lh, lh);
				expand.SizeRequest ();
			}
			
			if(WidthRequest == -1)
			{
				if(Child != null && Child.Visible)
				{
					requisition.Width = childRequisition.Width + (int)(2 * (2*lineWidth + childPadding));
				}
				else
				{
					requisition.Width = lw + (int)(2 * (lineWidth+space));
					if(expand != null && expand.Visible)
					{
						requisition.Width += expand.WidthRequest + (int)space;
					}
				}
			}
			
			if(HeightRequest == -1)
			{
				requisition.Height = (int)(4 * lineWidth + barHeight);
				if(Child != null && Child.Visible)
				{
					requisition.Height += childRequisition.Height + (int)(2 * childPadding);
				}
			}
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			if(expand != null && expand.Visible)
			{
				double frameSize = 2*lineWidth + space;
				Gdk.Rectangle r;
				r.Height = expand.HeightRequest;
				r.Width = expand.WidthRequest;
				r.X = allocation.X + allocation.Width - r.Width - (int)frameSize;
				r.Y = allocation.Y + allocation.Height - r.Height - (int)frameSize;
				expand.SizeAllocate (r);
			}
			
			if(Child != null && Child.Visible)
			{
				double frameSize = 2*lineWidth + childPadding;
				int wi = allocation.Width - (int)(2 * frameSize);
				int he = allocation.Height - (int)(2 * frameSize + barHeight); 
				Gdk.Rectangle r = new Gdk.Rectangle (allocation.X + (int)frameSize, allocation.Y + (int)frameSize, wi, he);
				Child.SizeAllocate (r);
			}
		}
		
		protected void Draw (Context cr)
		{
			Rectangle rect = new Rectangle (Allocation.X, Allocation.Y, Allocation.Width, Allocation.Height);
			theme.DrawGroup (cr, rect, 4.0, lineWidth, space, lbl_layout, this);
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