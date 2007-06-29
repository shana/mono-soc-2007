using System;
using System.Xml;
using Cairo;

namespace VirtualPaper
{
    public class Paper
    {
        protected Color BackgroundColor;

        public Paper(Color background)
        {
            BackgroundColor = background;
        }

        public Paper(XmlTextReader xml)
        {
            while(xml.Read()) {
                if(xml.NodeType == XmlNodeType.Element) {
                    if(xml.Name == "background-color") {
                        BackgroundColor.R = Convert.ToDouble(xml.GetAttribute("r"));
                        BackgroundColor.G = Convert.ToDouble(xml.GetAttribute("g"));
                        BackgroundColor.B = Convert.ToDouble(xml.GetAttribute("b"));
                        BackgroundColor.A = Convert.ToDouble(xml.GetAttribute("a"));
                    } else {
                        Console.WriteLine("Ignoring Unknown XML Element: {0}",
                            xml.Name);
                    }
                }
            }
        }

        public virtual void Draw(Context cr, Gdk.Rectangle clip)
        {
            cr.Rectangle(clip.X, clip.Y, clip.Width, clip.Height);
            cr.Color = BackgroundColor;
            cr.Fill();
            cr.Stroke();
        }

        public virtual void Serialize(XmlTextWriter xml)
        {
            xml.WriteStartElement(null, "background-color", null);
            xml.WriteAttributeString("r", BackgroundColor.R.ToString());
            xml.WriteAttributeString("g", BackgroundColor.G.ToString());
            xml.WriteAttributeString("b", BackgroundColor.B.ToString());
            xml.WriteAttributeString("a", BackgroundColor.A.ToString());
            xml.WriteEndElement();
        }

        public static Paper Deserialize(XmlTextReader xml)
        {
            // TODO: The following is a poor example of how we do object-
            // oriented programming.  I should be shot for this.  However,
            // much like the Undo code, I just want to see it work and clean
            // it up later.  Enjoy and laugh :-D
            string type = xml.GetAttribute("type");
            if(type == "NotebookPaper") {
                return new NotebookPaper(xml);
            } else if(type == "Paper") {
                return new Paper(xml);
            } else {
                Console.WriteLine("Unknown Paper Type: {0}", type);
                Console.WriteLine("Defaulting to Plain Paper");
                return new Paper(xml);
            }
        }
    }
}
