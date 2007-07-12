using System;
using Cairo;
using Gtk;

namespace Ribbons
{
	/// <summary>Button to be used in Ribbons.</summary>
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
		
		/// <summary>Spacing between the content and the widget.</summary>
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
		
		/// <summary>Shape of the widget.</summary>
		public GroupStyle GroupStyle
		{
			set { groupStyle = value; }
			get { return groupStyle; }
		}
		
		/// <summary><b>true</b> if the widget should paint a background, <b>false</B> otherwise.</summary>
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
		
		/// <summary>Image to display.</summary>
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
		
		/// <summary>Position of the image relative to the label.</summary>
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
		
		/// <summary>Label to display.</summary>
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
		
		/// <summary>Default constructor.</summary>
		public Button ()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.Padding = 2;
			this.ImagePosition = PositionType.Top;
		}
		
		/// <summary>Constructor given a label to display.</summary>
		/// <param name="Label">Label to display.</param>
		public Button (string Label) : this ()
		{
			this.Label = Label;
		}
		
		/// <summary>Constructor given an image to display.</summary>
		/// <param name="Image">Image to display</param>
		public Button (Image Image) : this ()
		{
			this.Image = Image;
		}
		
		/// <summary>Constructor given a label and an image to display.</summary>
		/// <param name="Image">Image to display.</param>
		/// <param name="Label">Label to display.</param>
		public Button (Image Image, string Label) : this ()
		{
			this.Image = Image;
			this.Label = Label;
		}
		
		/// <summary>Constructs a Button from a stock.</summary>
		/// <param name="Name">Name of the stock.</param>
		/// <param name="Large"><b>true</b> if the image should be large, <b>false</b> otherwise.</param>
		public static Button FromStockIcon (string Name, bool Large)
		{
			Image img = new Image (Name, Large ? IconSize.LargeToolbar : IconSize.SmallToolbar);
			return new Button (img);
		}
		
		/// <summary>Constructs a Button from a stock.</summary>
		/// <param name="Name">Name of the stock.</param>
		/// <param name="Label">Label to display.</param>
		/// <param name="Large"><b>true</b> if the image should be large, <b>false</b> otherwise.</param>
		public static Button FromStockIcon (string Name, string Label, bool Large)
		{
			Image img = new Image (Name, Large ? IconSize.LargeToolbar : IconSize.SmallToolbar);
			return new Button (img, Label);
		}
		
		/// <summary>Fires the Click event.</summary>
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
			if(Child != null) Remove (Child);
			
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
			allocation.Height -= (int)(lineWidth * 4 + padding * 2);
			allocation.Width -= (int)(lineWidth * 4 + padding * 2);
			
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
