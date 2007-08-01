using System;
using Cairo;
using Ribbons;

namespace Sample
{
	public class SampleTile : Tile
	{
		private Color a, b;
		
		public SampleTile (Color A, Color B)
		{
			a = A;
			b = B;
		}
		
		public override void DrawContent (Cairo.Context Context, Cairo.Rectangle Area)
		{
			LinearGradient gr = new LinearGradient (Area.X, Area.Y, Area.X + Area.Width, Area.Y + Area.Height);
			gr.AddColorStop (0, a);
			gr.AddColorStop (1, b);
			
			Context.Rectangle (Area);
			Context.Pattern = gr;
			Context.Fill ();
		}
	}
}
