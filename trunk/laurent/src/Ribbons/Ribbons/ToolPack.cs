using System;
using System.Collections.Generic;
using Cairo;
using Gtk;

namespace Ribbons
{
	public class ToolPack : Container
	{
		private List<Button> buttons;
		private int[] widths;
		
		public ToolPack ()
		{
			this.buttons = new List<Ribbons.Button> ();
			
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
		}
		
		public void PrependButton (Button Widget)
		{
			InsertButton (Widget, 0);
		}
		
		public void AppendButton (Button Widget)
		{
			InsertButton (Widget, -1);
		}
		
		public void InsertButton (Button Widget, int ButtonIndex)
		{
			Widget.Parent = this;
			
			Widget.DrawBackground = true;
			
			if(ButtonIndex == -1 || ButtonIndex == buttons.Count)
			{
				if(buttons.Count == 0)
				{
					Widget.GroupStyle = GroupStyle.Alone;
				}
				else
				{
					Widget.GroupStyle = GroupStyle.Right;
					
					if(buttons.Count == 1)
					{
						buttons[buttons.Count - 1].GroupStyle = GroupStyle.Left;
					}
					else if(buttons.Count > 1)
					{
						buttons[buttons.Count - 1].GroupStyle = GroupStyle.Center;
					}
				}
				buttons.Add (Widget);
			}
			else
			{
				if(ButtonIndex == 0)
				{
					buttons[buttons.Count - 1].GroupStyle = GroupStyle.Left;
					if(buttons.Count == 1)
					{
						buttons[0].GroupStyle = GroupStyle.Right;
					}
					else
					{
						buttons[0].GroupStyle = GroupStyle.Center;
					}
				}
				buttons.Insert (ButtonIndex, Widget);
			}
		}
		
		public void RemoveButton (int ButtonIndex)
		{
			buttons[ButtonIndex].Parent = null;
			
			if(ButtonIndex == 0)
			{
				if(buttons.Count > 1)
				{
					if(buttons.Count > 2)
					{
						buttons[0].GroupStyle = GroupStyle.Left;
					}
					else
					{
						buttons[0].GroupStyle = GroupStyle.Alone;
					}
				}
			}
			else if(ButtonIndex == buttons.Count - 1)
			{
				if(buttons.Count > 1)
				{
					if(buttons.Count > 2)
					{
						buttons[0].GroupStyle = GroupStyle.Right;
					}
					else
					{
						buttons[0].GroupStyle = GroupStyle.Alone;
					}
				}
			}
			buttons.RemoveAt (ButtonIndex);
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			foreach(Button b in buttons) callback (b);
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			if(widths == null || widths.Length != buttons.Count)
			{
				widths = new int[buttons.Count];
			}
			
			requisition.Height = requisition.Width = 0;
			int i = 0;
			foreach(Button b in buttons)
			{
				Gtk.Requisition req = b.SizeRequest ();
				if(requisition.Height < req.Height) requisition.Height = req.Height;
				requisition.Width += req.Width;
				widths[i++] = req.Width;
			}
			if(HeightRequest != -1) requisition.Height = HeightRequest;
			if(WidthRequest != -1) requisition.Width = WidthRequest;
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			int i = 0, x = allocation.X;
			foreach(Button b in buttons)
			{
				Gdk.Rectangle r;
				r.X = x;
				r.Y = allocation.Y;
				r.Width = widths[i++];
				r.Height = allocation.Height;
				b.SizeAllocate (r);
			}
		}
	}
}
