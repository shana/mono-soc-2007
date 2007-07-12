using Cairo;
using Gtk;
using System;

namespace Ribbons
{
	/// <summary>Ribbon group.</summary>
	public class RibbonGroup : Bin
	{
		protected Theme theme = new Theme ();
		protected string lbl;
		protected Pango.Layout lbl_layout;
		protected Button expandButton;
		protected EventHandler expandHandler;
		
		private double barHeight;
		
		protected double childPadding = 1.0;
		protected double lineWidth = 1.0;
		protected double space = 2.0;
		
		/// <summary>Displayed label.</summary>
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
		
		/// <summary>Expand event.</summary>
		/// <remarks>Fired whenever the expand button is clicked.</remarks>
		public event EventHandler Expand
		{
			add
			{
				expandHandler += value;
				if(expandHandler != null)
					expandButton.Show ();
				else
					expandButton.Hide ();
			}
			remove
			{
				expandHandler -= value;
				if(expandHandler != null)
					expandButton.Show ();
				else
					expandButton.Hide ();
			}
		}
		
		/// <summary>Default constructor.</summary>
		public RibbonGroup ()
		{
			// This is a No Window widget => it does not have its own Gdk Window => it can be transparent
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			Label = null;
			HeightRequest = 90;
			
			expandButton = new Button ("+");
			expandButton.Hide ();
			expandButton.Parent = this;
		}
		
		protected virtual void OnExpand (EventArgs e)
		{
			if(expandHandler != null) expandHandler (this, e);
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			base.ForAll (include_internals, callback);
			
			if(include_internals)
			{
				callback (expandButton);
			}
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
			if(expandButton != null && expandButton.Visible)
			{
				expandButton.SetSizeRequest (lh, lh);
				expandButton.SizeRequest ();
			}
			
			if(WidthRequest == -1)
			{
				if(Child != null && Child.Visible)
				{
					requisition.Width = childRequisition.Width + (int)(2 * (2*lineWidth + childPadding));
				}
				else
				{
					requisition.Width = lw + (int)(2 * (2*lineWidth + space));
					if(expandButton != null && expandButton.Visible)
					{
						requisition.Width += expandButton.WidthRequest + (int)space;
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
			
			if(expandButton != null && expandButton.Visible)
			{
				double frameSize = 2*lineWidth + space;
				Gdk.Rectangle r;
				r.Height = expandButton.HeightRequest;
				r.Width = expandButton.WidthRequest;
				r.X = allocation.X + allocation.Width - r.Width - (int)frameSize;
				r.Y = allocation.Y + allocation.Height - r.Height - (int)frameSize;
				expandButton.SizeAllocate (r);
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
			theme.DrawGroup (cr, rect, 4.0, lineWidth, space, lbl_layout, expandButton, this);
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