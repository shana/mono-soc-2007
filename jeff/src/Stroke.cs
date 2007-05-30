using System;
using System.Xml;
using System.Collections.Generic;

namespace Tablet
{
    public class Stroke
    {
        protected List<double> x, y;
        protected int count;
        protected double minX;
        protected double minY;
        protected double maxX;
        protected double maxY;
        protected bool firstStroke;

        public virtual double X {
            get {
                return minX;
            }
        }

        public virtual double Y {
            get {
                return minY;
            }
        }

        public virtual double Height {
            get {
                return maxY - minY;
            }
        }

        public virtual double Width {
            get {
                return maxX - minX;
            }
        }
        
        public virtual int Count {
            get {
                return count;
            }
        }

        public Stroke()
        {
            x = new List<double>();
            y = new List<double>();
            count = 0;
            minX = 0;
            minY = 0;
            maxX = 0;
            maxY = 0;
            firstStroke = false;
        }

        public virtual void AddPoint(double x, double y)
        {
            this.x.Add(x);
            this.y.Add(y);
            count++;

            if(firstStroke) {
                if(x < minX) minX = x;
                else if(x > maxX) maxX = x;
                if(y < minY) minY = y;
                else if(y > maxY) maxY = y;
            } else {
                minX = x;
                maxX = x;
                minY = y;
                maxY = y;
                firstStroke = true;
            }
        }

        public virtual void WriteXml(XmlTextWriter xml)
        {
            xml.WriteStartElement(null, "stroke", null);

            xml.WriteAttributeString("left", minX.ToString());
            xml.WriteAttributeString("top", minY.ToString());
            xml.WriteAttributeString("right", maxX.ToString());
            xml.WriteAttributeString("bottom", maxY.ToString());

            WriteXmlPoints(xml);

            xml.WriteEndElement();
        }

        protected virtual void WriteXmlPoints(XmlTextWriter xml)
        {
            for(int i = 0; i < count; i++) {
                xml.WriteStartElement(null, "point", null);
                xml.WriteAttributeString("x", x[i].ToString());
                xml.WriteAttributeString("y", y[i].ToString());
                xml.WriteEndElement();
            }
        }
    }
}
