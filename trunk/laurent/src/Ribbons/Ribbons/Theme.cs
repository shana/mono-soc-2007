using Cairo;
using System;

namespace Ribbons
{
	public class Theme
	{
		protected ColorScheme colorScheme = new ColorScheme ();
		
		protected double group_lineWidth = 1.0;
		protected double group_space = 2.0;
		
		public Gdk.Size SizeRequestedByGroup (RibbonGroup w, Pango.Layout l)
		{
			int lw, lh;
			l.GetPixelSize (out lw, out lh);
			return new Gdk.Size (lw + (int)(2 * (2*(group_lineWidth+group_space))), w.HeightRequest);
		}
		
		public Gdk.Size SizeRequestedByGroup (RibbonGroup w, Pango.Layout l, Gtk.Requisition childRequisition)
		{
			return new Gdk.Size (childRequisition.Width + (int)(2 * (2*group_lineWidth + w.BorderWidth)), w.HeightRequest);
		}
		
		public Gdk.Rectangle SizeAllocatedToChildByGroup (RibbonGroup w, Pango.Layout l, Gdk.Rectangle allocation)
		{
			int lw, lh;
			l.GetPixelSize (out lw, out lh);
			int frame_size = (int)(2*group_lineWidth) + (int)w.BorderWidth;
			int wi = allocation.Width - 2 * frame_size; 
			int he = allocation.Height - 2 * frame_size - (lh + (int)(2*group_space)); 
			return new Gdk.Rectangle (allocation.X + frame_size, allocation.Y + frame_size, wi, he);
		}
		
		public void DrawGroup (Context cr, Rectangle r, Pango.Layout l, RibbonGroup w)
		{
			double roundSize = 4.0;
			double lineWidth05 = group_lineWidth/2, lineWidth15 = 3*lineWidth05;
			LinearGradient linGrad;
			
			double x0 = r.X + roundSize, x1 = r.X + r.Width - roundSize;
			double y0 = r.Y + roundSize, y1 = r.Y + r.Height - roundSize;
			cr.Arc (x1, y1, roundSize - lineWidth05, 0, Math.PI/2);
			cr.Arc (x0 + group_lineWidth, y1, roundSize - lineWidth05, Math.PI/2, Math.PI);
			cr.Arc (x0, y0, roundSize - lineWidth15, Math.PI, 3*Math.PI/2);
			cr.Arc (x1, y0 + group_lineWidth, roundSize - lineWidth05, 3*Math.PI/2, 0);
			cr.LineTo (x1 + roundSize - lineWidth05, y1);
			cr.LineWidth = group_lineWidth;
			cr.Color = colorScheme.Bright;
			cr.Stroke ();
			
			if(l != null)
			{
				int lblWidth, lblHeight;
				Pango.CairoHelper.UpdateLayout (cr, l);
				l.GetPixelSize(out lblWidth, out lblHeight);
				
				double bandHeight = lblHeight + 2*group_space;
				cr.Arc (x1, y1, roundSize - lineWidth15, 0, Math.PI/2);
				cr.Arc (x0, y1 - group_lineWidth, roundSize - lineWidth05, Math.PI/2, Math.PI);
				double bandY = y1 + roundSize - 2*group_lineWidth - bandHeight;
				cr.LineTo (x0 - roundSize + lineWidth05, bandY);
				cr.LineTo (x1 + roundSize - lineWidth15, bandY);
				linGrad = new LinearGradient (0, bandY, 0, bandY + bandHeight);
				linGrad.AddColorStop (0.0, colorScheme.LightDark);
				linGrad.AddColorStop (1.0, colorScheme.Dark);
				cr.Pattern = linGrad;
				cr.Fill ();
				
				cr.Save ();
				cr.Rectangle (r.X + 2*group_lineWidth, bandY, r.Width - 4*group_lineWidth, bandHeight);
				cr.Clip ();
				
				cr.Color = new Color(0, 0, 0);
				Pango.CairoHelper.UpdateLayout (cr, l);
				cr.MoveTo (r.X + Math.Max(2*group_lineWidth, (r.Width - lblWidth) / 2), bandY + group_space);
				Pango.CairoHelper.ShowLayout (cr, l);
				
				cr.Restore();
			}
			
			cr.MoveTo (x1 + roundSize - lineWidth15, y1);
			cr.Arc (x1, y1, roundSize - lineWidth15, 0, Math.PI/2);
			cr.Arc (x0, y1 - group_lineWidth, roundSize - lineWidth05, Math.PI/2, Math.PI);
			cr.Arc (x0, y0, roundSize - lineWidth05, Math.PI, 3*Math.PI/2);
			cr.Arc (x1 - group_lineWidth, y0, roundSize - lineWidth05, 3*Math.PI/2, 0);
			cr.LineTo (x1 + roundSize - lineWidth15, y1);
			cr.LineWidth = group_lineWidth;
			linGrad = new LinearGradient (0, r.Y, 0, r.Y + r.Height - group_lineWidth);
			linGrad.AddColorStop (0.0, ColorScheme.GetColorRelative (colorScheme.Dark, -0.1));
			linGrad.AddColorStop (1.0, colorScheme.Dark);
			cr.Pattern = linGrad;
			cr.Stroke ();
		}
		
		protected void DrawRibbon (Context cr, Rectangle alloc, Ribbon widget)
		{
			double lineWidth05 = lineWidth / 2;
			double lineWidth15 = 3 * lineWidth05;
			double x0, x1, y0, y1;
			LinearGradient linGrad;
			
			RibbonPage p = widget.GetNthPage (widget.Page);
			if(p != null)
			{
				//Color c = ColorScheme.GetColor(colorScheme.Bright, 0.92);
				Color c = colorScheme.Normal;
				
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
				cr.Color = ColorScheme.GetColorAbsolute (colorScheme.Bright, 0.3);
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
				
				/*** TAB :: BACKGROUND ***/
				
				cr.MoveTo (x0 + lineWidth05, y1);
				cr.LineTo (x0 + lineWidth05, y0 + roundSize);
				cr.Arc (x0 + roundSize, y0 + roundSize, roundSize - lineWidth05, Math.PI, 3*Math.PI/2);
				cr.Arc (x1 - roundSize, y0 + roundSize, roundSize - lineWidth05, 3*Math.PI/2, 0);
				cr.LineTo (x1 - lineWidth05, y1);
				
				linGrad = new LinearGradient (0, y0, 0, y1);
				linGrad.AddColorStop (0.0, colorScheme.PrettyBright);
				linGrad.AddColorStop (1.0, c);
				cr.Pattern = linGrad;
				cr.Fill ();
				
				/*** TAB :: DARK BORDER ***/
				
				cr.MoveTo (x0 + lineWidth05, y1);
				cr.LineTo (x0 + lineWidth05, y0 + roundSize);
				cr.Arc (x0 + roundSize, y0 + roundSize, roundSize - lineWidth05, Math.PI, 3*Math.PI/2);
				cr.Arc (x1 - roundSize, y0 + roundSize, roundSize - lineWidth05, 3*Math.PI/2, 0);
				cr.LineTo (x1 - lineWidth05, y1);
				
				cr.LineWidth = lineWidth;
				cr.Color = ColorScheme.GetColorRelative (colorScheme.Bright, -0.1);
				cr.Stroke ();
				
				y1 -= 1.0;
				
				/*** TAB :: HIGHLIGHT ***/
				
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
				
				/*** TAB :: SHADOW ***/
				
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
		
		
		private void BottomRoundRectanglePath (Context cr, Rectangle r, double radius)
		{
			cr.Arc (r.X + r.Width - radius, r.Y + r.Height - radius, radius, 0, Math.PI/2);
			cr.Arc (r.X + radius, r.Y + r.Height - radius, radius, Math.PI/2, Math.PI);
			cr.LineTo (r.X, r.Y);
			cr.LineTo (r.X + r.Width, r.Y);
		}
		
		private void RoundRectanglePath (Context cr, Rectangle r, double radius)
		{
			cr.Arc (r.X + r.Width - radius, r.Y + r.Height - radius, radius, 0, Math.PI/2);
			cr.Arc (r.X + radius, r.Y + r.Height - radius, radius, Math.PI/2, Math.PI);
			cr.Arc (r.X + radius, r.Y + radius, radius, Math.PI, 3*Math.PI/2);
			cr.Arc (r.X + r.Width - radius, r.Y + radius, radius, 3*Math.PI/2, 0);
			cr.LineTo (r.X + r.Width, r.Y + r.Height - radius);
		}
	}
}
