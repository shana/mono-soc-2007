using System;
using Cairo;
using Gtk;

namespace Ribbons
{
	public class Button : Bin
	{
		private GroupStyle groupStyle;
		
		public GroupStyle GroupStyle
		{
			set { groupStyle = value; }
			get { return groupStyle; }
		}
		
		public string Label
		{
			set { Child = new Label (value); }
			get
			{
				Label lbl = Child as Label;
				return lbl == null ? null : lbl.Text;
			}
		}
		
		public Button()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			if(Child != null && Child.Visible)
			{
				Child.GetSizeRequest (out requisition.Width, out requisition.Height);
			}
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			if(Child != null && Child.Visible)
			{
				Child.SizeAllocate (allocation);
			}
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			Context cr = Gdk.CairoHelper.Create (this.GdkWindow);
			
			cr.Rectangle (evnt.Area.X, evnt.Area.Y, evnt.Area.Width, evnt.Area.Height);
			cr.Clip ();
			Draw (cr);
			
			return base.OnExposeEvent (evnt);
		}
		
		protected void Draw (Context cr)
		{
		
		}
	}
}
