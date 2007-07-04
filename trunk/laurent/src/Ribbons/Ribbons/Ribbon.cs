using System;
using System.Collections.Generic;
using Cairo;
using Gtk;

namespace Ribbons
{
	public class Ribbon : Container
	{
		private static double borderWidth = 2.0;
		private static double space = 2.0;
		private static double pagePadding = 3.0;
		private static double tabPadding = 4.0;
		private static double minimalTabsHorizontalPosition = 8.0;
		private static double lineWidth = 1.0;
		private static double roundSize = 4.0;
		
		protected ColorScheme colorScheme = new ColorScheme ();
		protected Theme theme = new Theme ();
		
		protected List<RibbonPage> pages;
		protected int curPageIndex;
		protected Widget shortcuts;
		private Gdk.Rectangle bodyAllocation, pageAllocation;
		
		private Gtk.Requisition shortcutsRequisition;
		private Gtk.Requisition pageRequisition;
		private double headerHeight;
		
		public event PageSelectedHandler PageSelected;
		public event PageAddedHandler PageAdded;
		public event PageMovedHandler PageMoved;
		public event PageRemovedHandler PageRemoved;
		
		public int CurrentPageIndex
		{
			set
			{
				if(curPageIndex != -1)
				{
					CurrentPage.Label.ModifyFg (StateType.Normal, theme.GetForecolorForRibbonTabs (false));
					CurrentPage.Page.Unparent ();
				}
				curPageIndex = value;
				if(curPageIndex != -1)
				{
					CurrentPage.Label.ModifyFg (StateType.Normal, theme.GetForecolorForRibbonTabs (true));
					CurrentPage.Page.Parent = this;
				}
				
				QueueDraw ();
			}
			get
			{
				return curPageIndex;
			}
		}
		
		public RibbonPage CurrentPage
		{
			get
			{
				int idx = curPageIndex;
				return idx == -1 ? null : pages[idx];
			}
		}
		
		public int NPages
		{
			get { return pages.Count; }
		}
		
		public Widget Shortcuts
		{
			set
			{
				if(shortcuts != null) shortcuts.Unparent ();
				shortcuts = value;
				if(shortcuts != null) shortcuts.Parent = this;
				QueueDraw ();
			}
			get { return shortcuts; }
		}
		
		public Ribbon()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.pages = new List<RibbonPage> ();
			this.curPageIndex = -1;
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
			{
				pages.Add (p);
			}
			else
			{
				pages.Insert (Position, p);
				
				if(curPageIndex != -1)
				{
					if(Position <= curPageIndex)
						++curPageIndex;
				}
			}
			
			if(pages.Count == 1)
			{
				CurrentPageIndex = 0;
			}
			else
			{
				Label.ModifyFg (StateType.Normal, theme.GetForecolorForRibbonTabs (false));
			}
			
			Label.ButtonPressEvent += delegate (object sender, ButtonPressEventArgs evnt)
			{
				this.SelectRibbonPage (p);
			};
			
			Label.EnterNotifyEvent += delegate (object sender, EnterNotifyEventArgs evnt)
			{
				
			};
			
			Label.LeaveNotifyEvent += delegate (object sender, LeaveNotifyEventArgs evnt)
			{
				
			};
			
			OnPageAdded (new PageEventArgs (p));
			for(int idx = Position + 1 ; idx < pages.Count ; ++idx)
			{
				OnPageSelected (new PageEventArgs (pages[idx]));
			}
		}
		
		public void RemovePage (int PageNumber)
		{
			if(curPageIndex != -1)
			{
				if(PageNumber < curPageIndex)
				{
					--curPageIndex;
				}
				else if(PageNumber == curPageIndex)
				{
					curPageIndex = -1;
				}
			}
			
			RibbonPage p = pages[PageNumber];
			pages.RemoveAt (PageNumber);
			
			OnPageRemoved (new PageEventArgs (p));
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
		
		public int RibbonPageNum (RibbonPage Page)
		{
			// Since it is unlikely that the widget will containe more than
			// a dozen pages, it is just fine to do a linear search.
			for(int i = 0, i_up = pages.Count ; i < i_up ; ++i)
				if(pages[i] == Page)
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
		
		public void SelectRibbonPage (RibbonPage page)
		{
			int idx = RibbonPageNum (page);
			if(idx != -1) CurrentPageIndex = idx;
			OnPageSelected (new PageEventArgs (page));
		}
		
		public void PrevPage ()
		{
			int i = CurrentPageIndex;
			if(i > 0) CurrentPageIndex = i - 1;
		}
		
		public void NextPage ()
		{
			int i = CurrentPageIndex;
			if(i < NPages - 1) CurrentPageIndex = i + 1;
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			if(Shortcuts != null) callback (Shortcuts);
			foreach(RibbonPage p in pages) callback (p.Label);
			if(CurrentPage != null) callback (CurrentPage.Page);
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			RibbonPage page = CurrentPage;
			
			double tabsWidth = 0, tabsHeight = 0;
			foreach(RibbonPage p in pages)
			{
				Gtk.Requisition req = p.Label.SizeRequest ();
				tabsWidth += req.Width;
				tabsHeight = Math.Max (tabsHeight, req.Height);
				p.LabelRequisition = req;
			}
			tabsWidth += pages.Count * 2 * tabPadding;
			tabsHeight += 2 * tabPadding;
			
			double headerWidth = tabsWidth;
			
			if(shortcuts != null)
			{
				shortcutsRequisition = shortcuts.SizeRequest ();
				double x = shortcutsRequisition.Width + space;
				headerWidth += Math.Max (x, minimalTabsHorizontalPosition);
			}
			else
			{
				shortcutsRequisition = new Gtk.Requisition ();
				headerWidth += minimalTabsHorizontalPosition;
			}
			
			headerHeight = Math.Max (tabsHeight, shortcutsRequisition.Height);
			
			double pageWidth = 0, pageHeight = 0;
			if(page != null)
			{
				pageRequisition = page.Page.SizeRequest ();
				pageWidth = pageRequisition.Width + 2 * pagePadding;
				pageHeight = pageRequisition.Height + 2 * pagePadding;
			}
			else
			{
				pageRequisition = new Gtk.Requisition ();
			}
			
			double width = Math.Max (headerWidth, pageWidth);
			width = borderWidth + width + borderWidth;
			double height = borderWidth + headerHeight + pageHeight + borderWidth;
			
			requisition.Width = (int)Math.Ceiling (width - double.Epsilon);
			requisition.Height = (int)Math.Ceiling (height - double.Epsilon);
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			RibbonPage page = CurrentPage;
			
			if(allocation.Height < headerHeight + borderWidth) return;
			
			double headerBottom = allocation.X + borderWidth + headerHeight;
			double currentX = space;
			
			if(shortcuts != null)
			{
				Gdk.Rectangle alloc;
				alloc.X = (int)currentX;
				alloc.Y = (int)(headerBottom - shortcutsRequisition.Height);
				alloc.Width = shortcutsRequisition.Width;
				alloc.Height = shortcutsRequisition.Height;
				shortcuts.SizeAllocate (alloc);
				currentX += shortcutsRequisition.Width;
			}
			currentX += space;
			currentX = Math.Max (currentX, minimalTabsHorizontalPosition);
			
			foreach(RibbonPage p in pages)
			{
				Gdk.Rectangle alloc;
				alloc.X = (int)(currentX + tabPadding);
				alloc.Y = (int)(headerBottom - tabPadding - p.LabelRequisition.Height);
				alloc.Width = p.LabelRequisition.Width;
				alloc.Height = p.LabelRequisition.Height;
				p.Label.SizeAllocate (alloc);
				
				alloc.X = (int)currentX;
				alloc.Y = (int)(headerBottom - tabPadding - p.LabelRequisition.Height - tabPadding);
				alloc.Width = (int)(tabPadding + p.LabelRequisition.Width + tabPadding);
				alloc.Height = (int)(tabPadding + p.LabelRequisition.Height + tabPadding);
				p.SetLabelAllocation (alloc);
				
				currentX += p.LabelRequisition.Width + 2 * tabPadding;
			}
			
			bodyAllocation.X = allocation.X + (int)borderWidth;
			bodyAllocation.Y = (int)headerBottom;
			bodyAllocation.Width = allocation.Width - bodyAllocation.X  - (int)borderWidth;
			bodyAllocation.Height = allocation.Height - bodyAllocation.Y - (int)borderWidth;
			
			if(page != null)
			{
				pageAllocation = bodyAllocation;
				int pad = (int)pagePadding;
				pageAllocation.Inflate (-pad, -pad);
				page.Page.SizeAllocate (pageAllocation);
			}
			else
			{
				pageAllocation = Gdk.Rectangle.Zero;
			}
			
			
			/*int x = allocation.X + (int)(ribbon_space + roundSize), y = allocation.Y + (int)ribbon_space, maxH = 0;
			
			int tab_horizPadding = (int)(4*ribbon_space+2*borderWidth);
			int tab_vertPadding = (int)(2*ribbon_space + borderWidth); 
			
			foreach(RibbonPage p in pages)
			{
				Gtk.Requisition req = p.Label.SizeRequest ();
				Gdk.Rectangle alloc = p.LabelAllocation;
				alloc.X = x;
				alloc.Y = y;
				alloc.Width = req.Width + tab_horizPadding;
				p.SetLabelAllocation (alloc);
				
				maxH = Math.Max(maxH, req.Height);
				x += alloc.Width;
			}
			maxH += tab_vertPadding;
			
			foreach(RibbonPage p in pages)
			{
				Gdk.Rectangle alloc = p.LabelAllocation;
				alloc.Height = maxH;
				p.SetLabelAllocation (alloc);
				
				alloc.X += tab_horizPadding >> 1;
				alloc.Y += tab_vertPadding >> 1;
				alloc.Width -= tab_horizPadding;
				alloc.Height -= tab_vertPadding;
				p.Label.SizeAllocate (alloc);
			}
			
			y += maxH;
			bodyAllocation.X = allocation.X + (int)ribbon_space;
			bodyAllocation.Y = y;
			bodyAllocation.Width = allocation.Width - bodyAllocation.X  - (int)ribbon_space;
			bodyAllocation.Height = allocation.Height - bodyAllocation.Y - (int)ribbon_space;
			
			if(page != null)
			{
				pageAllocation = bodyAllocation;
				int pad = (int)pagePadding;
				pageAllocation.Inflate (-pad, -pad);
				page.Page.SizeAllocate (pageAllocation);
			}
			else
			{
				pageAllocation = Gdk.Rectangle.Zero;
			}*/
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
			theme.DrawRibbon (cr, bodyAllocation, roundSize, lineWidth, this);
		}
		
		protected virtual void OnPageSelected (PageEventArgs args)
		{
			if(PageSelected != null) PageSelected (this, args);
		}
		
		protected virtual void OnPageAdded (PageEventArgs args)
		{
			if(PageAdded != null) PageAdded (this, args);
		}
		
		protected virtual void OnPageMoved (PageEventArgs args)
		{
			if(PageMoved != null) PageMoved (this, args);
		}
		
		protected virtual void OnPageRemoved (PageEventArgs args)
		{
			if(PageRemoved != null) PageRemoved (this, args);
		}
		
		public class RibbonPage
		{
			private Ribbon parent;
			private Widget label, page;
			private Requisition labelReq;
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
				set { page = value; }
				get { return page; }
			}
			
			internal Requisition LabelRequisition
			{
				set { labelReq = value; }
				get { return labelReq; }
			}
			
			public Gdk.Rectangle LabelAllocation
			{
				get { return labelAlloc; }
			}
			
			public RibbonPage (Ribbon Parent, Widget Page, Widget Label)
			{
				parent = Parent;
				this.Label = Label;
				this.Page = Page;
			}
			
			public void SetLabelAllocation (Gdk.Rectangle r)
			{
				labelAlloc = r;
			}
		}
	}
}
