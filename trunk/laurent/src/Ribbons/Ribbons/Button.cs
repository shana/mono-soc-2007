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
		private PositionType imgPos;
		private bool drawBg;
		private Widget img;
		private Label lbl;
		private double padding;
		
		protected double lineWidth = 1.0;
		
		public event EventHandler Clicked;
		
		public double Padding
		{
			set
			{
				if(padding == value) return;
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
		
		public bool DrawBackground
		{
			set
			{
				if(drawBg == value) return;
				drawBg = value;
				QueueDraw ();
			}
			get { return drawBg; }
		}
		
		public Widget Image
		{
			set
			{
				if(img == value) return;
				if(img != null) UnbindWidget (img);
				img = value;
				if(img != null) BindWidget (img);
				UpdateImageLabel ();
			}
			get { return img; }
		}
		
		public PositionType ImagePosition
		{
			set
			{
				if(imgPos == value) return;
				imgPos = value;
				UpdateImageLabel ();
			}
			get { return imgPos; }
		}
		
		public string Label
		{
			set
			{
				if(lbl != null) UnbindWidget (lbl);
				lbl = new Gtk.Label (value);
				if(lbl != null) BindWidget (lbl);
				UpdateImageLabel ();
			}
			get
			{
				return lbl == null ? null : lbl.Text;
			}
		}
		
		public Button ()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.Padding = 0;
			this.ImagePosition = PositionType.Top;
		}
		
		public Button (string Label) : this ()
		{
			this.Label = Label;
		}
		
		public Button (Image Image) : this ()
		{
			this.Image = Image;
		}
		
		public Button (Image Image, string Label) : this ()
		{
			this.Image = Image;
			this.Label = Label;
		}
		
		public static Button FromStockIcon (string Name, bool Large)
		{
			Image img = new Image (Name, Large ? IconSize.LargeToolbar : IconSize.SmallToolbar);
			return new Button (img);
		}
		
		public static Button FromStockIcon (string Name, string Label, bool Large)
		{
			Image img = new Image (Name, Large ? IconSize.LargeToolbar : IconSize.SmallToolbar);
			return new Button (img, Label);
		}
		
		public void Click ()
		{
			if(Clicked != null) Clicked (this, EventArgs.Empty);
		}
		
		private void BindWidget (Widget w)
		{
			w.ButtonPressEvent += BindedWidget_ButtonPressEvent;
			w.ButtonReleaseEvent += BindedWidget_ButtonReleaseEvent;
		}
		
		private void UnbindWidget (Widget w)
		{
			w.ButtonPressEvent -= BindedWidget_ButtonPressEvent;
			w.ButtonReleaseEvent -= BindedWidget_ButtonReleaseEvent;
		}
		
		private void BindedWidget_ButtonPressEvent (object sender, ButtonPressEventArgs evnt)
		{
			ProcessEvent (evnt.Event);
		}
		
		private void BindedWidget_ButtonReleaseEvent (object sender, ButtonReleaseEventArgs evnt)
		{
			ProcessEvent (evnt.Event);
			Click ();
		}
		
		private void UpdateImageLabel ()
		{
			if(lbl != null && lbl.Parent != null) lbl.Unparent ();
			if(img != null && img.Parent != null) img.Unparent ();
			
			if(lbl != null && img != null)
			{
				switch(imgPos)
				{
					case PositionType.Top:
					{
						VBox box = new VBox (false, 0);
						box.Add (img);
						box.Add (lbl);
						Child = box;
						break;
					}
					case PositionType.Bottom:
					{
						VBox box = new VBox (false, 0);
						box.Add (lbl);
						box.Add (img);
						Child = box;
						break;
					}
					case PositionType.Left:
					{
						HBox box = new HBox (false, 0);
						box.Add (img);
						box.Add (lbl);
						Child = box;
						break;
					}
					case PositionType.Right:
					{
						HBox box = new HBox (false, 0);
						box.Add (lbl);
						box.Add (img);
						Child = box;
						break;
					}
				}
			}
			else if(lbl != null)
			{
				Child = lbl;
			}
			else if(img != null)
			{
				Child = img;
			}
			else
			{
				if(Child != null) Remove (Child);
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
			
			if(HeightRequest == -1)
			{
				requisition.Height = childRequisition.Height + (int)(lineWidth * 4 + padding * 2);
			}
			if(WidthRequest == -1)
			{
				requisition.Width = childRequisition.Width + (int)(lineWidth * 4 + padding * 2);
			}
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			allocation.X += (int)(lineWidth * 2 + padding);
			allocation.Y += (int)(lineWidth * 2 + padding);
			allocation.Height -= (int)(lineWidth * 4 - padding * 2);
			allocation.Width -= (int)(lineWidth * 4 - padding * 2);
			
			if(allocation.Height < 0) allocation.Height = 0;
			if(allocation.Width < 0) allocation.Width = 0;
			
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
