using System;
using Cairo;
using Gtk;

namespace Ribbons
{
	public class Button : Bin
	{
		private GroupStyle groupStyle;
		private bool isHot = false;
		
		protected double lineWidth = 1.0;
		
		public GroupStyle GroupStyle
		{
			set { groupStyle = value; }
			get { return groupStyle; }
		}
		
		public bool IsHot
		{
			get { return isHot; }
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
		
		public Button ()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
		}
		
		public Button (string Label) : this()
		{
			this.Label = Label;
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			if(Child != null && Child.Visible)
			{
				Child.GetSizeRequest (out requisition.Width, out requisition.Height);
			}
			requisition.Height += (int)(lineWidth * 4);
			requisition.Width += (int)(lineWidth * 4);
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			allocation.X += (int)(lineWidth * 2);
			allocation.Y += (int)(lineWidth * 2);
			allocation.Height -= (int)(lineWidth * 4);
			allocation.Width -= (int)(lineWidth * 4);
			
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
		
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			return base.OnButtonPressEvent (evnt);
		}
		
		protected override bool OnEnterNotifyEvent (Gdk.EventCrossing evnt)
		{
			bool ret = base.OnEnterNotifyEvent (evnt);
			isHot = true;
			this.QueueDraw ();
			return ret;
		}
		
		protected override bool OnLeaveNotifyEvent (Gdk.EventCrossing evnt)
		{
			bool ret = base.OnLeaveNotifyEvent (evnt);
			isHot = false;
			this.QueueDraw ();
			return ret;
		}
	}
}
