using System;
using System.Collections.Generic;
using Gtk;

namespace Ribbons
{
	public class ToolBox : Container
	{
		private List<Widget> widgets;
		private Gtk.Requisition[] requisitions;
		
		public ToolBox()
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
			foreach(Widget w in widgets) rowHeight = Math.Max (rowHeight, w.SizeRequest ().Height);
			int i = 0;
			foreach(Widget w in widgets)
			{
				w.HeightRequest = rowHeight;
				requisitions[i] = w.SizeRequest ();
				totalWidth += requisitions[i].Width;
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
				
				foreach(Gtk.Requisition r in requisitions)
				{
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
				
				requisition.Width = WidthRequest;
				requisition.Height = totalHeight + 2*(int)BorderWidth;
			}
			else
			{
				int rowsLeft = (int)Math.Ceiling ((double)HeightRequest / (double)rowHeight);
				int widthLeft = totalWidth;
				int curWidth = 0, maxWidth = 0;
				int minWidth = widthLeft / rowsLeft;
				
				foreach(Gtk.Requisition r in requisitions)
				{
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
				
				requisition.Width = minWidth + 2*(int)BorderWidth;
				requisition.Height = HeightRequest;
			}
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			
		}
	}
}
