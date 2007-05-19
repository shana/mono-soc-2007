using Cairo;
using System;

namespace Ribbons
{
	public class Theme
	{
		protected ColorScheme colorScheme = new ColorScheme ();
		
		public void DrawGroup (Context cr, Rectangle r, RibbonGroup w)
		{
			double roundSize = 4.0;
			double lineWidth = 1.0;
			double space = 2.0;
			double lineWidth05 = lineWidth/2, lineWidth15 = 3*lineWidth05;
			
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
			
			Pango.Layout layout = new Pango.Layout(w.PangoContext);
			layout.SetText(w.Label);
			int lblWidth, lblHeight;
			layout.GetPixelSize(out lblWidth, out lblHeight);
			
			double bandHeight = lblHeight + 2*space;
			cr.Arc (x1, y1, roundSize - lineWidth15, 0, Math.PI/2);
			cr.Arc (x0, y1 - lineWidth, roundSize - lineWidth05, Math.PI/2, Math.PI);
			double bandY = y1 + lineWidth - lineWidth15 - bandHeight;
			cr.LineTo (x0 - roundSize + lineWidth05, bandY);
			cr.LineTo (x1 + roundSize - lineWidth15, bandY);
			LinearGradient ribbonGroupBandGradient = new LinearGradient (0, bandY, 0, bandY + bandHeight);
			ribbonGroupBandGradient.AddColorStop (0.0, colorScheme.LightDark);
			ribbonGroupBandGradient.AddColorStop (1.0, ColorScheme.GetColor(colorScheme.LightDark, 0.92));
			cr.Pattern = ribbonGroupBandGradient;
			cr.Fill ();
			
			/*cr.Color = new Color(0, 0, 0);
			cr.MoveTo (10, bandY);
			Pango.CairoHelper.ShowLayout(cr, layout);*/
			
			cr.Arc (x1, y1, roundSize - lineWidth15, 0, Math.PI/2);
			cr.Arc (x0, y1 - lineWidth, roundSize - lineWidth05, Math.PI/2, Math.PI);
			cr.Arc (x0, y0, roundSize - lineWidth05, Math.PI, 3*Math.PI/2);
			cr.Arc (x1 - lineWidth, y0, roundSize - lineWidth05, 3*Math.PI/2, 0);
			cr.LineTo (x1 + roundSize - lineWidth15, y1);
			cr.LineWidth = lineWidth;
			ribbonGroupBandGradient = new LinearGradient (0, r.Y, 0, r.Y + r.Height - lineWidth);
			ribbonGroupBandGradient.AddColorStop (0.0, ColorScheme.GetColor(colorScheme.Dark, 1.1));
			ribbonGroupBandGradient.AddColorStop (1.0, colorScheme.Dark);
			cr.Pattern = ribbonGroupBandGradient;
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
