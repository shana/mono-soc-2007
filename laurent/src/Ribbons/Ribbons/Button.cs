using System;
using Cairo;
using Gtk;

namespace Ribbons
{
	public class Button : Bin
	{
		protected Theme theme = new Theme ();
		private GroupStyle groupStyle;
		private Theme.ButtonState state = Theme.ButtonState.Default;
		private Widget oldChild;
		private double padding;
		
		protected double lineWidth = 1.0;
		
		public double Padding
		{
			set
			{
				padding = value;
				QueueDraw ();
			}
			get { return padding; }
		}
		
		public GroupStyle GroupStyle
		{
			set { groupStyle = value; }
			get { return groupStyle; }
		}
		
		public string Label
		{
			set { Child = new Gtk.Label (value); }
			get
			{
				Label lbl = Child as Gtk.Label;
				return lbl == null ? null : lbl.Text;
			}
		}
		
		public Button ()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.Padding = 0;
		}
		
		public Button (string Label) : this()
		{
			this.Label = Label;
		}
		
		private void Child_ButtonPressEvent (object sender, ButtonPressEventArgs evnt)
		{
			ProcessEvent (evnt.Event);
		}
		
		private void Child_ButtonReleaseEvent (object sender, ButtonReleaseEventArgs evnt)
		{
			ProcessEvent (evnt.Event);
		}
		
		protected override void OnHierarchyChanged (Widget previous_toplevel)
		{
			if(oldChild != Child)
			{
				if(oldChild != null)
				{
					oldChild.ButtonPressEvent -= Child_ButtonPressEvent;
					oldChild.ButtonReleaseEvent -= Child_ButtonReleaseEvent;
				}
				if(Child != null)
				{
					Child.ButtonPressEvent += Child_ButtonPressEvent;
					Child.ButtonReleaseEvent += Child_ButtonReleaseEvent;
				}
				oldChild = Child;
			}
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			Requisition childRequisition = new Requisition ();
			if(Child != null && Child.Visible)
			{
				childRequisition = Child.SizeRequest ();
			}
			
			requisition.Height = childRequisition.Height + (int)(lineWidth * 4 + padding * 2);
			requisition.Width = childRequisition.Width + (int)(lineWidth * 4 + padding * 2);
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			allocation.X += (int)(lineWidth * 2 + padding);
			allocation.Y += (int)(lineWidth * 2 + padding);
			allocation.Height -= (int)(lineWidth * 4 - padding * 2);
			allocation.Width -= (int)(lineWidth * 4 - padding * 2);
			
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
			Rectangle rect = new Rectangle (Allocation.X, Allocation.Y, Allocation.Width, Allocation.Height);
			theme.DrawButton (cr, rect, state, 3.0, lineWidth, this);
		}
		
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			bool ret = base.OnButtonPressEvent (evnt);
			state = Theme.ButtonState.Pressed;
			this.QueueDraw ();
			return ret;
		}
		
		protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
		{
			bool ret = base.OnButtonReleaseEvent (evnt);
			state = Theme.ButtonState.Hover;
			this.QueueDraw ();
			return ret;
		}
		
		protected override bool OnEnterNotifyEvent (Gdk.EventCrossing evnt)
		{
			bool ret = base.OnEnterNotifyEvent (evnt);
			state = Theme.ButtonState.Hover;
			this.QueueDraw ();
			return ret;
		}
		
		protected override bool OnLeaveNotifyEvent (Gdk.EventCrossing evnt)
		{
			bool ret = base.OnLeaveNotifyEvent (evnt);
			state = Theme.ButtonState.Default;
			this.QueueDraw ();
			return ret;
		}
	}
}
