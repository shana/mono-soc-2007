using System;
using System.Collections.Generic;
using Cairo;
using Gtk;

namespace Ribbons
{
	public class Ribbon : Container
	{
		private List<RibbonPage> pages;
		private RibbonPage curPage;
		private Gdk.Rectangle bodyAllocation, pageAllocation;
		
		private Gdk.Color selectedTabFgColor = new Gdk.Color(0, 0, 0);
		private Gdk.Color unselectedTabFgColor = new Gdk.Color(1, 1, 1);
		
		public int Page
		{
			set
			{
				if(curPage != null)
				{
					curPage.Label.ModifyFg (StateType.Normal, unselectedTabFgColor);
				}
				curPage = pages[value];
				if(curPage != null)
				{
					curPage.Label.ModifyFg (StateType.Normal, selectedTabFgColor);
				}
			}
			get
			{
				RibbonPage p = curPage;
				return p == null ? -1 : PageNum (p.Page);
			}
		}
		
		public int NPages
		{
			get { return pages.Count; }
		}
		
		public Ribbon()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.pages = new List<RibbonPage>();
			
			this.BorderWidth = 4;
		}
		
		public void AppendPage (Widget Child, Widget Label)
		{
			InsertPage (Child, Label, -1);
		}
		
		public void PrependPage (Widget Child, Widget Label)
		{
			InsertPage (Child, Label, 0);
		}
		
		public void InsertPage (Widget Child, Widget Label, int Position)
		{
			RibbonPage p = new RibbonPage (this, Child, Label);
			
			if(Position == -1)
				pages.Add (p);
			else
				pages.Insert (Position, p);
			
			if(pages.Count == 1) Page = 0;
			else p.Label.ModifyFg (StateType.Normal, unselectedTabFgColor);
		}
		
		public void RemovePage (int PageNumber)
		{
			pages.RemoveAt (PageNumber);
		}
		
		public int PageNum (Widget Child)
		{
			// Since it is unlikely that the widget will containe more than
			// a dozen pages, it is just fine to do a linear search.
			for(int i = 0, i_up = pages.Count ; i < i_up ; ++i)
				if(pages[i].Page == Child)
					return i;
			return -1;
		}
		
		public void SetPageLabel (Widget Child, Widget Label)
		{
			pages[PageNum (Child)].Label = Label;
		}
		
		public Widget GetPageLabel (Widget Child)
		{
			return pages[PageNum (Child)].Label;
		}
		
		public Widget GetNthPage (int Position)
		{
			return pages[Position].Page;
		}
		
		public void PrevPage ()
		{
			int i = Page;
			if(i > 0) Page = i - 1;
		}
		
		public void NextPage ()
		{
			int i = Page;
			if(i < NPages - 1) Page = i + 1;
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			foreach(RibbonPage p in pages) callback (p.Label);
			if(curPage != null) callback (curPage.Page);
		}
		
		private double ribbon_borderWidth = 2.0;
		private double ribbon_space = 2.0;
		private double lineWidth = 1.0;
		private double roundSize = 4.0;
		private ColorScheme colorScheme = new ColorScheme();
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			int vertPadding = (int)(2*ribbon_space + ribbon_borderWidth); 
			
			int headerWidth = 0, headerHeight = 0;
			foreach(RibbonPage p in pages)
			{
				Gtk.Requisition req = p.Label.SizeRequest ();
				headerWidth += req.Width;
				headerHeight = Math.Max (headerHeight, req.Height);
			}
			headerHeight += vertPadding + (int)ribbon_space;
			
			int width = 0, height = 0;
			if(curPage != null)
			{
				Gtk.Requisition req = curPage.Page.SizeRequest ();
				width = req.Width;
				height = req.Height + (int)(2*BorderWidth + ribbon_space);
			}
			width = Math.Max (width, headerWidth);
			width += (int)(2 * (ribbon_borderWidth + ribbon_space));
			height += headerHeight;
			
			requisition.Width = width;
			requisition.Height = height;
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			int x = allocation.X + (int)(ribbon_space + roundSize), y = allocation.Y + (int)ribbon_space, maxH = 0;
			
			int horizPadding = (int)(4*ribbon_space+2*ribbon_borderWidth);
			int vertPadding = (int)(2*ribbon_space + ribbon_borderWidth); 
			
			foreach(RibbonPage p in pages)
			{
				Gtk.Requisition req = p.Label.SizeRequest ();
				Gdk.Rectangle alloc = p.LabelAllocation;
				alloc.X = x;
				alloc.Y = y;
				alloc.Width = req.Width + horizPadding;
				p.LabelAllocation = alloc;
				
				maxH = Math.Max(maxH, req.Height);
				x += alloc.Width;
			}
			maxH += vertPadding;
			
			foreach(RibbonPage p in pages)
			{
				Gdk.Rectangle alloc = p.LabelAllocation;
				alloc.Height = maxH;
				p.LabelAllocation = alloc;
				
				alloc.X += horizPadding >> 1;
				alloc.Y += vertPadding >> 1;
				alloc.Width -= horizPadding;
				alloc.Height -= vertPadding;
				p.Label.SizeAllocate (alloc);
			}
			
			y += maxH;
			bodyAllocation.X = allocation.X + (int)ribbon_space;
			bodyAllocation.Y = y;
			bodyAllocation.Width = allocation.Width - bodyAllocation.X  - (int)ribbon_space;
			bodyAllocation.Height = allocation.Height - bodyAllocation.Y - (int)ribbon_space;
			
			if(curPage != null)
			{
				pageAllocation = bodyAllocation;
				int bw = (int)BorderWidth;
				pageAllocation.Inflate (-bw, -bw);
				curPage.Page.SizeAllocate (pageAllocation);
			}
			else
			{
				pageAllocation = Gdk.Rectangle.Zero;
			}
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			Context cr = Gdk.CairoHelper.Create (this.GdkWindow);
			
			cr.Rectangle (evnt.Area.X, evnt.Area.Y, evnt.Area.Width, evnt.Area.Height);
			cr.Clip ();
			Draw(cr);
			
			return base.OnExposeEvent (evnt);
		}
		
		protected void Draw (Context cr)
		{
			double lineWidth05 = lineWidth / 2;
			double lineWidth15 = 3 * lineWidth05;
			double x0, x1, y0, y1;
			LinearGradient linGrad;
			
			RibbonPage p = curPage;
			if(p != null)
			{
				Color c = ColorScheme.GetColor(colorScheme.Bright, 0.92);
				
				/*** PAGE ***/
				
				x0 = bodyAllocation.X; x1 = bodyAllocation.X + bodyAllocation.Width;
				y0 = bodyAllocation.Y; y1 = bodyAllocation.Y + bodyAllocation.Height;
				
				cr.Arc (x0 + roundSize, y1 - roundSize, roundSize - lineWidth05, Math.PI/2, Math.PI);
				cr.Arc (x0 + roundSize, y0 + roundSize, roundSize - lineWidth05, Math.PI, 3*Math.PI/2);
				cr.Arc (x1 - roundSize, y0 + roundSize, roundSize - lineWidth05, 3*Math.PI/2, 0);
				cr.Arc (x1 - roundSize, y1 - roundSize, roundSize - lineWidth05, 0, Math.PI/2);
				cr.LineTo (x0 + roundSize, y1 - lineWidth05);
				
				/*** BACKGOUND ***/
				cr.Color = c;
				cr.FillPreserve ();

				/*** DARK BORDER ***/
				cr.LineWidth = lineWidth;
				cr.Color = ColorScheme.GetColor(colorScheme.Bright, 0.90);
				cr.Stroke ();
				
				y0 = Math.Round(y0 + (y1 - y0) * 0.25);
				cr.Arc (x0 + roundSize, y1 - roundSize, roundSize - lineWidth, Math.PI/2, Math.PI);
				cr.LineTo (x0 + lineWidth, y0);
				cr.LineTo (x1 - lineWidth, y0);
				cr.Arc (x1 - roundSize, y1 - roundSize, roundSize - lineWidth, 0, Math.PI/2);
				cr.LineTo (x0 + roundSize, y1 - lineWidth);
				
				linGrad = new LinearGradient (0, y0, 0, y1);
				linGrad.AddColorStop (0.0, new Color (0, 0, 0, 0.05));
				linGrad.AddColorStop (0.66, new Color (0, 0, 0, 0.0));
				cr.Pattern = linGrad;
				cr.Fill ();
				
				/*** TAB ***/
				
				Gdk.Rectangle r = p.LabelAllocation;
				
				x0 = r.X; x1 = r.X + r.Width;
				y0 = r.Y; y1 = r.Y + r.Height + lineWidth;
				
				/*** BACKGOUND ***/
				
				cr.MoveTo (x0 + lineWidth05, y1);
				cr.LineTo (x0 + lineWidth05, y0 + roundSize);
				cr.Arc (x0 + roundSize, y0 + roundSize, roundSize - lineWidth05, Math.PI, 3*Math.PI/2);
				cr.Arc (x1 - roundSize, y0 + roundSize, roundSize - lineWidth05, 3*Math.PI/2, 0);
				cr.LineTo (x1 - lineWidth05, y1);
				
				linGrad = new LinearGradient (0, y0, 0, y1);
				linGrad.AddColorStop (0.0, colorScheme.Bright);
				linGrad.AddColorStop (1.0, c);
				cr.Pattern = linGrad;
				cr.Fill ();
				
				y1 -= 1.0;
				
				/*** DARK BORDER ***/
				
				cr.MoveTo (x0 + lineWidth05, y1);
				cr.LineTo (x0 + lineWidth05, y0 + roundSize);
				cr.Arc (x0 + roundSize, y0 + roundSize, roundSize - lineWidth05, Math.PI, 3*Math.PI/2);
				cr.Arc (x1 - roundSize, y0 + roundSize, roundSize - lineWidth05, 3*Math.PI/2, 0);
				cr.LineTo (x1 - lineWidth05, y1);
				
				cr.LineWidth = lineWidth;
				cr.Color = ColorScheme.GetColor(colorScheme.Bright, 0.90);
				cr.Stroke ();
				
				/*** HIGHLIGHT ***/
				
				cr.MoveTo (x0 + lineWidth15, y1);
				cr.LineTo (x0 + lineWidth15, y0 + roundSize);
				cr.Arc (x0 + roundSize, y0 + roundSize, roundSize - lineWidth15, Math.PI, 3*Math.PI/2);
				cr.Arc (x1 - roundSize, y0 + roundSize, roundSize - lineWidth15, 3*Math.PI/2, 0);
				cr.LineTo (x1 - lineWidth15, y1);
				
				cr.LineWidth = lineWidth;
				linGrad = new LinearGradient (0, y0+lineWidth, 0, y1);
				linGrad.AddColorStop (0.0, colorScheme.PrettyBright);
				linGrad.AddColorStop (1.0, ColorScheme.SetAlphaChannel (colorScheme.Bright, 0));
				cr.Pattern = linGrad;
				cr.Stroke ();
				
				/*** SHADOW ***/
				
				cr.MoveTo (x0 - lineWidth05, y1);
				cr.LineTo (x0 - lineWidth05, y0 + roundSize);
				cr.Arc (x0 + roundSize, y0 + roundSize, roundSize + lineWidth05, Math.PI, 3*Math.PI/2);
				cr.Arc (x1 - roundSize, y0 + roundSize, roundSize + lineWidth05, 3*Math.PI/2, 0);
				cr.LineTo (x1 + lineWidth05, y1);
				
				cr.LineWidth = lineWidth;
				cr.Color = new Color (0, 0, 0, 0.2);
				cr.Stroke ();
			}
		}
		
		private class RibbonPage
		{
			private Ribbon parent;
			private Widget label, page;
			private Gdk.Rectangle labelAlloc;
			
			public Widget Label
			{
				set
				{
					if(label != null) label.Unparent ();
					label = value;
					if(label != null) label.Parent = parent;
				}
				get { return label; }
			}
			
			public Widget Page
			{
				set
				{
					if(page != null) page.Unparent ();
					page = value;
					if(page != null) page.Parent = parent;
				}
				get { return page; }
			}
			
			public Gdk.Rectangle LabelAllocation
			{
				set { labelAlloc = value; }
				get { return labelAlloc; }
			}
			
			public RibbonPage (Ribbon Parent, Widget Page, Widget Label)
			{
				parent = Parent;
				this.Label = Label;
				this.Page = Page;
			}
			
			public void Map ()
			{
				page.Map ();
			}
			
			public void Unmap ()
			{
				page.Unmap ();
			}
		}
	}
}
