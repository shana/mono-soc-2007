using System;
using System.Collections.Generic;
using Gtk;

namespace Ribbons
{
	public class ToolBox : Container
	{
		private List<Widget> widgets;
		private Gtk.Requisition[] requisitions;
		
		public ToolBox ()
		{
			this.widgets = new List<Widget> ();
			
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			foreach(Widget w in widgets)
			{
				if(w.Visible) callback (w);
			}
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			if(requisitions == null || requisitions.Length != widgets.Count)
			{
				requisitions = new Gtk.Requisition[widgets.Count];
			}
			
			int totalWidth = 0, rowHeight = 0;
			foreach(Widget w in widgets)
			{
				if(w.Visible)
				{
					rowHeight = Math.Max (rowHeight, w.SizeRequest ().Height);
				}
			}
			int i = 0;
			foreach(Widget w in widgets)
			{
				if(w.Visible)
				{
					w.HeightRequest = rowHeight;
					requisitions[i] = w.SizeRequest ();
					totalWidth += requisitions[i].Width;
				}
				++i;
			}
			
			if(WidthRequest != -1 && HeightRequest != -1)
			{
				requisition.Width = WidthRequest;
				requisition.Height = HeightRequest;
			}
			else if(WidthRequest != -1)
			{
				int totalHeight = 0, curWidth = 0;
				int availWidth = WidthRequest - 2*(int)BorderWidth;
				
				i = 0;
				foreach(Widget w in widgets)
				{
					if(w.Visible)
					{
						Gtk.Requisition r = requisitions[i];
						
						if(curWidth == 0 || curWidth + r.Width <= availWidth)
						{	// Continue current line
							curWidth += r.Width;
						}
						else
						{	// Start new line
							totalHeight += rowHeight;
							curWidth = 0;
						}
					}
					++i;
				}
				
				requisition.Width = WidthRequest;
				requisition.Height = totalHeight + 2*(int)BorderWidth;
			}
			else
			{
				int rowsLeft = (int)Math.Ceiling ((double)HeightRequest / (double)rowHeight);
				int widthLeft = totalWidth;
				int curWidth = 0, maxWidth = 0;
				int minWidth = widthLeft / rowsLeft;
				
				i = 0;
				foreach(Widget w in widgets)
				{
					if(w.Visible)
					{
						Gtk.Requisition r = requisitions[i];
						
						widthLeft -= r.Width;
						curWidth += r.Width;
						
						if(curWidth >= minWidth)
						{	// Start new line
							maxWidth = Math.Max (maxWidth, curWidth);
							curWidth = 0;
							--rowsLeft;
							minWidth = widthLeft / rowsLeft;
						}
					}
					++i;
				}
				
				requisition.Width = minWidth + 2*(int)BorderWidth;
				requisition.Height = HeightRequest;
			}
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			int right = allocation.X + allocation.Width - (int)BorderWidth;
			int left = allocation.X + (int)BorderWidth;
			int bottom = allocation.Y + allocation.Height - (int)BorderWidth;
			int x = left, rowY = allocation.Y + (int)BorderWidth;
			int maxHeight = 0;
			
			int i = 0;
			foreach(Widget w in widgets)
			{
				if(w.Visible)
				{
					Gdk.Rectangle r;
					r.Width = requisitions[i].Width;
					r.Height = requisitions[i].Height;
					
					if(x > left && x + r.Width > right)
					{
						rowY += maxHeight;
						maxHeight = 0;
						x = left;
					}
					
					r.X = x;
					r.Y = rowY;
					r.Width = r.X - Math.Min (right, r.X + r.Width);
					r.Height = r.Y - Math.Min (bottom, r.Y + r.Height);
					w.SizeAllocate (r);
					
					x += r.Width;
					maxHeight = Math.Max (maxHeight, r.Height);
				}
				++i;
			}
		}
	}
}
