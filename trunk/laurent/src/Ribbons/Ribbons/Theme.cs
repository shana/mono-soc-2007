using Cairo;
using System;

namespace Ribbons
{
	public class Theme
	{
		protected ColorScheme colorScheme = new ColorScheme ();
		
		public void DrawGroup (Context cr, Rectangle r, RibbonGroup w)
		{
			double roundSize = 3.0;
			double space = 2.0;
			double bottom = r.Y + r.Height;
			
			//TextExtents te = cr.TextExtents (w.Label);
			double bandHeight = 20; //te.Height + 2 * space;
			Rectangle bandRect = new Rectangle (r.X, bottom - bandHeight, r.Width, bandHeight);
			BottomRoundRectanglePath (cr, bandRect, roundSize);
			LinearGradient ribbonGroupBandGradient = new LinearGradient (bandRect.X, bandRect.Y, bandRect.X, bandRect.Y + bandRect.Height);
			ribbonGroupBandGradient.AddColorStop(0.0, colorScheme.LightDark);
			ribbonGroupBandGradient.AddColorStop(1.0, ColorScheme.GetColor(colorScheme.LightDark, 0.85));
			cr.Pattern = ribbonGroupBandGradient;
			cr.Fill ();
			
			/*cr.MoveTo (
			cr.ShowText (w.Label);*/
		}
		
		private void BottomRoundRectanglePath (Context cr, Rectangle r, double radius)
		{
			double size = radius * 2.0;
			cr.Arc (r.X + r.Width - size, r.Y + r.Height - size, size, 0, Math.PI/2);
			cr.Arc (r.X + size, r.Y + r.Height - size, size, Math.PI/2, Math.PI);
			cr.LineTo (r.X, r.Y);
			cr.LineTo (r.X + r.Width, r.Y);
		}
	}
}
