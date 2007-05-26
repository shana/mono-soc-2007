using Cairo;
using System;

namespace Ribbons
{
	public class ColorScheme
	{
		private Color dark, lightDark, normal, lightBright, bright, prettyBright;

		public Color Dark	// 0.75
		{
			set { dark = value; }
			get { return dark; }
		}

		public Color LightDark	// 0.975
		{
			set { lightDark = value; }
			get { return lightDark; }
		}

		public Color Normal	// 1.000
		{
			set { normal = value; }
			get { return normal; }
		}

		public Color LightBright	// 1.025
		{
			set { lightBright = value; }
			get { return lightBright; }
		}

		public Color Bright	// 1.075
		{
			set { bright = value; }
			get { return bright; }
		}

		public Color PrettyBright	// 1.085
		{
			set { prettyBright = value; }
			get { return prettyBright; }
		}
		
		public ColorScheme() : this (new Color(0.957, 0.957, 0.957))
		{
			
		}
		
		public ColorScheme (Color Normal)
		{
			dark = GetColor (Normal, 0.75);
			lightDark = GetColor (Normal, 0.975);
			normal = Normal;
			lightBright = GetColor (Normal, 1.025);
			bright = GetColor (Normal, 1.075);
			prettyBright = GetColor (Normal, 1.085);
		}
		
		public static Color SetAlphaChannel(Color C, double Alpha)
		{
			return new Color (C.R, C.G, C.B, Alpha);
		}
		
		public static Color GetColor(Color C, double luminance)
		{
			double h, s, v;
			RGB2HSV(C, out h, out s, out v);
			v = v + (1.7 * (luminance - 1));
			if(v < 0) v = 0; else if(v > 1) v = 1;
			return HSV2RGB(h, s, v);
		}

		private static void RGB2HSV(Color C, out double H, out double S, out double V)
		{	// http://www.daniweb.com/techtalkforums/thread38302.html
			double r = C.R, g = C.G, b = C.B;
			double max = Math.Max(r, Math.Max(g, b));
			double min = Math.Min(r, Math.Min(g, b));
			double delta = max - min;
			V = max;
			if(Math.Abs(delta) < 0.0000001)
			{
				H = S = 0;
			}
			else
			{
				S = delta / max;
				if(r == max)
					H = 60.0 * (g - b) / delta;
				else if(g == max)
					H = 60.0 * (2 + (b - r) / delta);
				else
					H = 60.0 * (4 + (r - g) / delta);
				if(H < 0) H += 360; else if(H > 360) H -= 360;
			}
		}

		private static Color HSV2RGB(double H, double S, double V)
		{	// http://en.wikipedia.org/wiki/HSV_color_space
			int H_i = (int)(H / 60) % 6;
			double f = H / 60 - H_i;
			if(H_i == 0) return new Color(V, V * (1 - (1 - f) * S), V * (1 - S));
			if(H_i == 1) return new Color(V * (1 - f * S), V, V * (1 - S));
			if(H_i == 2) return new Color(V * (1 - S), V, V * (1 - (1 - f) * S));
			if(H_i == 3) return new Color(V * (1 - S), V * (1 - f * S), V);
			if(H_i == 4) return new Color(V * (1 - (1 - f) * S), V * (1 - S), V);
			return new Color(V, V * (1 - S), V * (1 - f * S));
		}
	}
}
