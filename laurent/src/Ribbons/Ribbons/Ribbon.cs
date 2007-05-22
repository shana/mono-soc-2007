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
		
		public int Page
		{
			set
			{
				curPage.Unmap ();
				curPage = pages[value];
				curPage.Map ();
			}
			get { return PageNum (curPage.Page); }
		}
		
		public int NPages
		{
			get { return pages.Count; }
		}
		
		public Ribbon()
		{
			pages = new List<RibbonPage>();
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
			RibbonPage p = new RibbonPage (Child, Label);
			
			if(Position == -1)
				pages.Add (p);
			else
				pages.Insert (Position, p);
			
			if(curPage == null)
				curPage = p;
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
		
		protected override void OnRealized ()
		{
			SetFlag (WidgetFlags.Realized);
			
			Gdk.WindowAttr attr = new Gdk.WindowAttr();
			attr.X = Allocation.X;
			attr.Y = Allocation.Y;
			attr.Width = Allocation.Width;
			attr.Height = Allocation.Height;
			attr.Wclass = Gdk.WindowClass.InputOutput;
			attr.WindowType = Gdk.WindowType.Child;
			attr.Mask = Events | Gdk.EventMask.ExposureMask | Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask | Gdk.EventMask.PointerMotionHintMask;
			attr.Visual = Visual;
			attr.Colormap = Colormap;
			
			int attrMask = (int)(Gdk.WindowAttributesType.X | Gdk.WindowAttributesType.Y | Gdk.WindowAttributesType.Visual | Gdk.WindowAttributesType.Colormap);
			
			this.GdkWindow = new Gdk.Window (Parent.GdkWindow, attr, attrMask);
			
			this.Style = this.Style.Attach (GdkWindow);
			this.Style.Background (StateType.Active);
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			if(this.GdkWindow != null)
				this.GdkWindow.MoveResize (allocation);
			
			foreach(RibbonPage p in pages)
			{
				p.Label.SizeAllocate (p.LabelAllocation);
			}
			
			if(curPage != null)
			{
				pageAllocation = bodyAllocation;
				pageAllocation.Offset (BorderWidth, BorderWidth);
				pageAllocation.Inflate (-BorderWidth, -BorderWidth);
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
			cr.Color = new Color(0.5, 1, 0.5);
			cr.Paint();
			Draw(cr);
			
			return base.OnExposeEvent (evnt);
		}
		
		protected void Draw (Context cr)
		{
			
		}
		
		private class RibbonPage
		{
			private Widget label, page;
			private Gdk.Rectangle labelAlloc;
			
			public Widget Label
			{
				set { label = value; }
				get { return label; }
			}
			
			public Widget Page
			{
				set { page = value; }
				get { return page; }
			}
			
			public Gdk.Rectangle LabelAllocation
			{
				set { labelAlloc = value; }
				get { return labelAlloc; }
			}
			
			public RibbonPage (Widget Label, Widget Page)
			{
				label = Label;
				page = Page;
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
