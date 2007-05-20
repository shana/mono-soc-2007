using Cairo;
using System;

namespace Ribbons
{
	public class Theme
	{
		protected ColorScheme colorScheme = new ColorScheme ();
		
		public void DrawGroup (Context cr, Rectangle r, Pango.Layout l, RibbonGroup w)
		{
			double roundSize = 4.0;
			double lineWidth = 1.0;
			double space = 2.0;
			double lineWidth05 = lineWidth/2, lineWidth15 = 3*lineWidth05;
			LinearGradient linGrad;
			
			double x0 = r.X + roundSize, x1 = r.X + r.Width - roundSize;
			double y0 = r.Y + roundSize, y1 = r.Y + r.Height - roundSize;
			cr.Arc (x1, y1, roundSize - lineWidth05, 0, Math.PI/2);
			cr.Arc (x0 + lineWidth, y1, roundSize - lineWidth05, Math.PI/2, Math.PI);
			cr.Arc (x0, y0, roundSize - lineWidth15, Math.PI, 3*Math.PI/2);
			cr.Arc (x1, y0 + lineWidth, roundSize - lineWidth05, 3*Math.PI/2, 0);
			cr.LineTo (x1 + roundSize - lineWidth05, y1);
			cr.LineWidth = lineWidth;
			cr.Color = colorScheme.Bright;
			cr.Stroke ();
			
			if(l != null)
			{
				int lblWidth, lblHeight;
				Pango.CairoHelper.UpdateLayout (cr, l);
				l.GetPixelSize(out lblWidth, out lblHeight);
				
				double bandHeight = lblHeight + 2*space;
				cr.Arc (x1, y1, roundSize - lineWidth15, 0, Math.PI/2);
				cr.Arc (x0, y1 - lineWidth, roundSize - lineWidth05, Math.PI/2, Math.PI);
				double bandY = y1 + roundSize - 2*lineWidth - bandHeight;
				cr.LineTo (x0 - roundSize + lineWidth05, bandY);
				cr.LineTo (x1 + roundSize - lineWidth15, bandY);
				linGrad = new LinearGradient (0, bandY, 0, bandY + bandHeight);
				linGrad.AddColorStop (0.0, colorScheme.LightDark);
				linGrad.AddColorStop (1.0, ColorScheme.GetColor(colorScheme.LightDark, 0.92));
				cr.Pattern = linGrad;
				cr.Fill ();
				
				cr.Save ();
				cr.Rectangle (r.X + 2*lineWidth, bandY, r.Width - 4*lineWidth, bandHeight);
				cr.Clip ();
				
				cr.Color = new Color(0, 0, 0);
				Pango.CairoHelper.UpdateLayout (cr, l);
				cr.MoveTo (r.X + Math.Max(2*lineWidth, (r.Width - lblWidth) / 2), bandY + space);
				Pango.CairoHelper.ShowLayout (cr, l);
				
				cr.Restore();
			}
			
			cr.MoveTo (x1 + roundSize - lineWidth15, y1);
			cr.Arc (x1, y1, roundSize - lineWidth15, 0, Math.PI/2);
			cr.Arc (x0, y1 - lineWidth, roundSize - lineWidth05, Math.PI/2, Math.PI);
			cr.Arc (x0, y0, roundSize - lineWidth05, Math.PI, 3*Math.PI/2);
			cr.Arc (x1 - lineWidth, y0, roundSize - lineWidth05, 3*Math.PI/2, 0);
			cr.LineTo (x1 + roundSize - lineWidth15, y1);
			cr.LineWidth = lineWidth;
			linGrad = new LinearGradient (0, r.Y, 0, r.Y + r.Height - lineWidth);
			linGrad.AddColorStop (0.0, ColorScheme.GetColor(colorScheme.Dark, 1.1));
			linGrad.AddColorStop (1.0, colorScheme.Dark);
			cr.Pattern = linGrad;
			cr.Stroke ();
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
