using System;
using System.Collections.Generic;
using Cairo;
using Gtk;

namespace Ribbons
{
	public class Ribbon : Container
	{
		protected Theme theme = new Theme ();
		private List<RibbonPage> pages;
		private RibbonPage curPage;
		private Gdk.Rectangle bodyAllocation, pageAllocation;
		
		public int Page
		{
			set
			{
				if(curPage != null)
				{
					curPage.Label.ModifyFg (StateType.Normal, theme.GetForecolorForRibbonTabs (false));
				}
				curPage = pages[value];
				if(curPage != null)
				{
					curPage.Label.ModifyFg (StateType.Normal, theme.GetForecolorForRibbonTabs (true));
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
			else p.Label.ModifyFg (StateType.Normal, theme.GetForecolorForRibbonTabs (false));
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
		
		public RibbonPage GetNthRibbonPage (int Position)
		{
			return pages[Position];
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
				p.SetLabelAllocation (alloc);
				
				maxH = Math.Max(maxH, req.Height);
				x += alloc.Width;
			}
			maxH += vertPadding;
			
			foreach(RibbonPage p in pages)
			{
				Gdk.Rectangle alloc = p.LabelAllocation;
				alloc.Height = maxH;
				p.SetLabelAllocation (alloc);
				
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
			theme.DrawRibbon (cr, bodyAllocation, roundSize, lineWidth, this);
		}
		
		public class RibbonPage
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
				//set { labelAlloc = value; }
				get { return labelAlloc; }
			}
			
			public RibbonPage (Ribbon Parent, Widget Page, Widget Label)
			{
				parent = Parent;
				this.Label = Label;
				this.Page = Page;
			}
			
			internal void SetLabelAllocation (Gdk.Rectangle r)
			{
				labelAlloc = r;
			}
		}
	}
}
