using System;
using Cairo;
using Ribbons;

namespace Sample
{
	public class SampleTile : Tile
	{
		private Color a, b;
		private Pango.Layout textLayout;
		
		public SampleTile (string Text)
		{
			textLayout = CreatePangoLayout (Text);
		}
		
		public override void DrawContent (Cairo.Context Context, Cairo.Rectangle Area)
		{
			Context.Color = new Color (0, 0, 0);
			Pango.CairoHelper.UpdateLayout (Context, textLayout);
			Context.MoveTo (Area.X, Area.Y);
			Pango.CairoHelper.ShowLayout (Context, textLayout);
		}
	}
}
