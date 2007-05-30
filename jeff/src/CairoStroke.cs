using System;
using System.Xml;
using System.Collections.Generic;
using Cairo;

namespace Tablet
{
    public class CairoStroke : Stroke
    {
        protected List<Color> color;
        protected bool eraser;

        public bool Eraser {
            get {
                return eraser;
            }
        }

        public Gdk.Rectangle Bounds {
            get {
                Gdk.Rectangle rect = new Gdk.Rectangle();
                rect.X = (int)minX;
                rect.Y = (int)minY;
                rect.Width = (int)(maxX - minX);
                rect.Height = (int)(maxY - minY);
/*                rect.X = (int)((minX < maxX) ? minX : maxX);
                rect.Y = (int)((minY < maxY) ? minY : maxY);
                rect.Width = (int)((minX < maxX) ? maxX-minX : minX-maxX);
                rect.Height = (int)((minY < maxY) ? maxY-minY : minY-maxY);*/
                return rect;
            }
        }

        public CairoStroke(bool erase) : base()
        {
            color = new List<Color>();
            eraser = erase;
        }

        public CairoStroke(XmlTextReader xml) : this(false)
        {
            minX = Convert.ToDouble(xml.GetAttribute("left"));
            minY = Convert.ToDouble(xml.GetAttribute("top"));
            maxX = Convert.ToDouble(xml.GetAttribute("right"));
            maxY = Convert.ToDouble(xml.GetAttribute("bottom"));

            int depth = xml.Depth;
            while(xml.Read() && xml.Depth > depth) {
                if(xml.NodeType == XmlNodeType.Element) {
                    if(xml.Name == "point") {
                        x.Add(Convert.ToDouble(xml.GetAttribute("x")));
                        y.Add(Convert.ToDouble(xml.GetAttribute("y")));
                        color.Add(new Cairo.Color(
                            Convert.ToDouble(xml.GetAttribute("r")),
                            Convert.ToDouble(xml.GetAttribute("g")),
                            Convert.ToDouble(xml.GetAttribute("b")),
                            Convert.ToDouble(xml.GetAttribute("a"))));
                        count++;
                     } else {
                         Console.WriteLine("Ignoring Unknown XML Element: {0}",
                             xml.Name);
                     }
                }
            }
        }

        public override void AddPoint(double x, double y)
        {
            AddPoint(x, y, new Color(0.0,0.0,0.0,0.0));
        }


        public Gdk.Rectangle AddPoint(double x, double y, Color color)
        {
            base.AddPoint(x, y);
            this.color.Add(color);
            if(this.x.Count > 1 && this.y.Count > 1) {
                double oldX = this.x[this.x.Count - 2];
                double oldY = this.y[this.y.Count - 2];
                if(x < oldX) {
                    double temp = oldX;
                    oldX = x;
                    x = temp;
                }
                if(y < oldY) {
                    double temp = oldY;
                    oldY = y;
                    y = temp;
                }
                
                return new Gdk.Rectangle((int)oldX, (int)oldY,
                    (int)(x - oldX), (int)(y - oldY));
            }
            return new Gdk.Rectangle((int)x, (int)y, 0, 0);
        }

        public void Draw(Context cr, Gdk.Rectangle clip)
        {
            for(int i = 1; i < count; i++) {
                Gdk.Rectangle rect = new Gdk.Rectangle();
                rect.X = (int)((x[i] < x[i-1]) ? x[i] : x[i-1]);
                rect.Y = (int)((y[i] < y[i-1]) ? y[i] : y[i-1]);
                rect.Width = (int)((x[i] < x[i-1]) ? x[i-1]-x[i] : x[i]-x[i-1]);
                rect.Height = (int)((y[i] < y[i-1]) ? y[i-1]-y[i] : y[i]-y[i-1]);

                if(clip.IntersectsWith(rect)) {
                    cr.MoveTo(x[i-1], y[i-1]);
                    cr.LineTo(x[i], y[i]);

                    LinearGradient g = new LinearGradient(x[i-1], y[i-1], x[i], y[i]);
                    g.AddColorStop(0.0, color[i-1]);
                    g.AddColorStop(1.0, color[i]);

                    cr.Pattern = g;
                    cr.Stroke();
                }
            }
        }

        protected override void WriteXmlPoints(XmlTextWriter xml)
        {
            for(int i = 0; i < count; i++) {
                xml.WriteStartElement(null, "point", null);
                xml.WriteAttributeString("x", x[i].ToString());
                xml.WriteAttributeString("y", y[i].ToString());
                xml.WriteAttributeString("r", color[i].R.ToString());
                xml.WriteAttributeString("g", color[i].G.ToString());
                xml.WriteAttributeString("b", color[i].B.ToString());
                xml.WriteAttributeString("a", color[i].A.ToString());
                xml.WriteEndElement();
            }
        }
    }
}
