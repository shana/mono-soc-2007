using System;
using System.Xml;
using Cairo;

namespace VirtualPaper
{
    public class NotebookPaper : Paper
    {
        private static Color blue  = new Color(0.75, 0.75, 1.0 , 1.0);
        private static Color red   = new Color(1.0 , 0.75, 0.75, 1.0);
        private static Color black = new Color(0.0 , 0.0 , 0.0 , 0.0);

        private const double marginSize   = 75.0;
        private const double ruleDistance = 35.0;

        private bool horizontalRule;
        private bool verticalRule;
        private bool leftMargin;
        private bool holes;

        public NotebookPaper(Color c) : base(c) { }

        public NotebookPaper(XmlTextReader xml) : base(xml)
        {
            while(xml.Read()) {
                if(xml.NodeType == XmlNodeType.Element) {
                    if(xml.Name == "background-color") {
                        BackgroundColor.R = Convert.ToDouble(xml.GetAttribute("r"));
                        BackgroundColor.G = Convert.ToDouble(xml.GetAttribute("g"));
                        BackgroundColor.B = Convert.ToDouble(xml.GetAttribute("b"));
                        BackgroundColor.A = Convert.ToDouble(xml.GetAttribute("a"));
                    } else if(xml.Name == "horizontal-rule") {
                        horizontalRule = true;
                    } else if(xml.Name == "vertical-rule") {
                        verticalRule = true;
                    } else if(xml.Name == "left-margin") {
                        leftMargin = true;
                    } else if(xml.Name == "holes") {
                        holes = true;
                    } else {
                        Console.WriteLine("Ignoring Unknown XML Element: {0}",
                            xml.Name);
                    }
                }
            }
        }

        public override void Draw(Context cr, Gdk.Rectangle clip)
        {
            base.Draw(cr, clip);

            double X      = clip.X;
            double Y      = clip.Y;
            double Width  = clip.Width;
            double Height = clip.Height;

            // Blue Rulings
            if(horizontalRule) {
                cr.Color = blue;
                for(double i = Y - (Y % ruleDistance) + ruleDistance;
                        i <= Y + Height; i += ruleDistance) {
                    cr.MoveTo(X, i);
                    cr.LineTo(X + Width, i);
                    cr.Stroke();
                }
            }
            if(verticalRule) {
                cr.Color = blue;
                for(double i = X - (X % ruleDistance) + ruleDistance;
                        i <= X + Width; i += ruleDistance) {
                    cr.MoveTo(i, Y);
                    cr.LineTo(i, Y + Height);
                    cr.Stroke();
                }
            }

            // Red Margin Line
            if(leftMargin) {
                cr.Color = red;
                cr.MoveTo(marginSize, Y);
                cr.LineTo(marginSize, Y + Height);
                cr.Stroke();
            }

            // Holes
            if(holes) {
                cr.Color = black;
                cr.Arc(ruleDistance/2, 150, 17, 0, 2 * Math.PI);
                cr.Arc(ruleDistance/2, 650, 17, 0, 2 * Math.PI);
                cr.Arc(ruleDistance/2, 1150,17, 0, 2 * Math.PI);
                cr.Fill();
            }
        }

        public override void Serialize(XmlTextWriter xml)
        {
            base.Serialize(xml);

            // TODO: Write some sort of unique identifier that
            // tells Mono which subclass of Paper to use.  Or do I
            // have to do that manually?  I may need to do that
            // manually.

            if(horizontalRule) {
                xml.WriteStartElement(null, "horizontal-rule", null);
                xml.WriteEndElement();
            }

            if(verticalRule) {
                xml.WriteStartElement(null, "vertical-rule", null);
                xml.WriteEndElement();
            }

            if(leftMargin) {
                xml.WriteStartElement(null, "left-margin", null);
                xml.WriteEndElement();
            }

            if(holes) {
                xml.WriteStartElement(null, "holes", null);
                xml.WriteEndElement();
            }
        }
    }
}
