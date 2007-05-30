using System;
using System.Xml;
using System.Collections.Generic;
using Cairo;
using Gtk;
using Gdk;

namespace Tablet
{
    public class Handwriting : DrawingArea
    {
        public static readonly Cairo.Color Yellow = new Cairo.Color(1.0, 1.0, 0.75, 1.0);
        public static readonly Cairo.Color White = new Cairo.Color(1.0, 1.0, 1.0, 1.0);
        public static readonly double WideRule = 40.0;
        public static readonly double CollegeRule = 30.0;

        public event EventHandler Changed;

        private List<CairoStroke> strokes;
        private CairoStroke activeStroke;
        private Cairo.Color paperColor;
        private double ruleDistance;
        private bool horizontalRule;
        private bool verticalRule;
        private bool leftMargin;
        private bool holes;
        private int undo;
        private int countPoints;

        public Cairo.Color PaperColor {
            get {
                return paperColor;
            }
            set {
                paperColor = value;
                QueueDraw();
            }
        }

        public double RuleDistance {
            get {
                return ruleDistance;
            }
            set {
                ruleDistance = value;
                QueueDraw();
            }
        }

        public bool HorizontalRule {
            get {
                return horizontalRule;
            }
            set {
                horizontalRule = value;
                QueueDraw();
            }
        }

        public bool VerticalRule {
            get {
                return verticalRule;
            }
            set {
                verticalRule = value;
                QueueDraw();
            }
        }

        public bool LeftMargin {
            get {
                return leftMargin;
            }
            set {
                leftMargin = value;
                QueueDraw();
            }
        }

        public bool Holes {
            get {
                return holes;
            }
            set {
                holes = value;
                QueueDraw();
            }
        }

        public bool CanUndo {
            get {
                return undo < strokes.Count;
            }
        }

        public bool CanRedo {
            get {
                return undo > 0;
            }
        }

        public int CountPoints {
            get {
                return CountPoints;
            }
        }

        public Gdk.Rectangle Bounds {
            get {
                Gdk.Rectangle bounds = new Gdk.Rectangle();
                foreach(CairoStroke stroke in strokes) {
                    if(stroke.Bounds.X + stroke.Bounds.Width > bounds.Width)
                        bounds.Width = stroke.Bounds.X + stroke.Bounds.Width;
                    if(stroke.Bounds.Y + stroke.Bounds.Height > bounds.Height)
                        bounds.Height = stroke.Bounds.Y + stroke.Bounds.Height;
                }
                return bounds;
            }
        }

        public Handwriting() : base()
        {
            strokes = new List<CairoStroke>();
            undo = 0;
            AppPaintable = true;
            Events = EventMask.ExposureMask | EventMask.ButtonPressMask |
                     EventMask.ButtonReleaseMask | EventMask.PointerMotionMask;
            ExtensionEvents = ExtensionMode.Cursor;

//            GdkWindow.Cursor = Cursor.NewFromName(Display.Default, "Clock");
        }

        public void DrawToSurface(Cairo.Surface surface, Gdk.Rectangle size)
        {
            Cairo.Context cr = new Cairo.Context(surface);

            drawBackground(cr, size);
            drawStrokes(cr, size);

            ((IDisposable)cr).Dispose();
        }

        public void Serialize(XmlTextWriter xml)
        {
            xml.WriteStartDocument();
            xml.WriteStartElement(null, "handwriting-data", null);
            xml.WriteAttributeString("version", "0.1");

            // Widget Settings
            xml.WriteStartElement(null, "paper-color", null);
            xml.WriteAttributeString("r", paperColor.R.ToString());
            xml.WriteAttributeString("g", paperColor.G.ToString());
            xml.WriteAttributeString("b", paperColor.B.ToString());
            xml.WriteAttributeString("a", paperColor.A.ToString());
            xml.WriteEndElement();

            if(horizontalRule) {
                xml.WriteStartElement(null, "horizontal-rule", null);
                xml.WriteAttributeString("size", ruleDistance.ToString());
                xml.WriteEndElement();
            }

            if(verticalRule) {
                xml.WriteStartElement(null, "vertical-rule", null);
                xml.WriteAttributeString("size", ruleDistance.ToString());
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

            // Strokes
            xml.WriteStartElement(null, "strokes", null);

            foreach(Stroke s in strokes) {
                s.WriteXml(xml);
            }

            xml.WriteEndElement();
        }

        public void Deserialize(XmlTextReader xml)
        {
            horizontalRule = false;
            verticalRule = false;
            leftMargin = false;
            holes = false;
            Clear();

            while(xml.Read()) {
                switch(xml.NodeType) {
                case XmlNodeType.Element:
                    if(xml.Name == "handwriting-data")
                        break;
                    else if(xml.Name == "paper-color") {
                        paperColor.R = Convert.ToDouble(xml.GetAttribute("r"));
                        paperColor.G = Convert.ToDouble(xml.GetAttribute("g"));
                        paperColor.B = Convert.ToDouble(xml.GetAttribute("b"));
                        paperColor.A = Convert.ToDouble(xml.GetAttribute("a"));
                    } else if(xml.Name == "horizontal-rule") {
                        horizontalRule = true;
                        ruleDistance = Convert.ToInt32(xml.GetAttribute("size"));
                    } else if(xml.Name == "vertical-rule") {
                        verticalRule = true;
                        ruleDistance = Convert.ToInt32(xml.GetAttribute("size"));
                    } else if(xml.Name == "left-margin") {
                        leftMargin = true;
                    } else if(xml.Name == "holes") {
                        holes = true;
                    } else if(xml.Name == "strokes") {
                        while(xml.Read()) {
                            if(xml.NodeType == XmlNodeType.Element &&
                                xml.Name == "stroke") {
                                CairoStroke stroke = new CairoStroke(xml);
                                countPoints += stroke.Count;
                                strokes.Add(stroke);
                            } else if(xml.NodeType == XmlNodeType.EndElement) {
                                break;
                            }
                        }
                    } else {
                        Console.WriteLine("Ignoring Unknown XML Element: {0}",
                            xml.Name);
                    }
                    break;
                }
            }
        }

        public void Undo()
        {
            if(CanUndo) {
                undo++;
                QueueDraw();
            }
        }

        public void Redo()
        {
            if(CanRedo) {
                undo--;
                QueueDraw();
            }
        }

        public void Clear()
        {
            strokes.Clear();
            undo = 0;
            QueueDraw();
        }

        protected override bool OnConfigureEvent(EventConfigure ev)
        {
            foreach(Device d in GdkWindow.Display.ListDevices()) {
                if(d.Source == InputSource.Pen ||
                   d.Source == InputSource.Eraser ||
                   d.Source == InputSource.Mouse) {
                    d.SetMode(InputMode.Screen);
                }
            }

            return true;
        }

        protected override bool OnExposeEvent(EventExpose ev)
        {
            if(!IsRealized) {
                return false;
            }

            Cairo.Context cr = CairoHelper.Create(GdkWindow);

            Gdk.Rectangle rect = ev.Area;
            cr.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            cr.Clip();

            drawBackground(cr, rect);
            drawStrokes(cr, rect);

            // Uncomment this to see the clipping region
/*            cr.Color = new Cairo.Color(0.0,1.0,0.0,0.5);
            cr.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            cr.Stroke();*/

            ((IDisposable)cr).Dispose();

            return true;
        }

        protected override bool OnButtonPressEvent(EventButton ev)
        {
            if(ev.Button == 1 || ev.Device.Source == InputSource.Pen) {
                strokeBegin(false);
//                strokeContinue(ev.X, ev.Y, new Cairo.Color(0.0,0.0,0.0,1.0));
            } else if(ev.Button == 3 || ev.Device.Source == InputSource.Eraser) {
                strokeBegin(true);
//                strokeContinue(ev.X, ev.Y, paperColor);
            }

            return true;
        }

        protected override bool OnButtonReleaseEvent(EventButton ev)
        {
            strokeEnd();

            return true;
        }

        protected override bool OnMotionNotifyEvent(EventMotion ev)
        {
            if(activeStroke == null) {
                return true;
            }
            double[] axes;
            ModifierType mask;

            if(ev.IsHint) {
                ev.Device.GetState(GdkWindow, out axes, out mask);
            } else {
                axes = ev.Axes;
                mask = ev.State;
            }

            if(ev.Device.Source == InputSource.Pen) {
                if(ev.Device.NumAxes > 2) {
                    strokeContinue(axes[0], axes[1], new Cairo.Color(0.0,0.0,0.0,axes[2]));
                } else {
                    strokeContinue(axes[0], axes[1], new Cairo.Color(0.0,0.0,0.0,1.0));
                }
            } else if(ev.Device.Source == InputSource.Eraser ||
                      mask == ModifierType.Button3Mask) {
                strokeContinue(axes[0], axes[1], paperColor);
            } else {
                // Unknown Button
            }

/*            if(mask == ModifierType.Button1Mask ||
               mask == ModifierType.Button3Mask ||
               axes[2] > 0.0) {
                if(ev.Device.Source == InputSource.Pen) {
                    Console.WriteLine("Pen");
                    if(activeStroke == null) strokeBegin(false);
                    strokeContinue(axes[0], axes[1], new Cairo.Color(0.0,0.0,0.0,axes[2]));
                } else if(ev.Device.Source == InputSource.Eraser) {
                    Console.WriteLine("Eraser");
                    if(activeStroke == null) strokeBegin(true);
                    strokeContinue(axes[0], axes[1], paperColor);
                } else {
                    if(mask == ModifierType.Button1Mask) {
                        Console.WriteLine("Button1");
                        if(activeStroke == null) strokeBegin(false);
                        strokeContinue(axes[0], axes[1], new Cairo.Color(0.0,0.0,0.0,1.0));
                    } else if(mask == ModifierType.Button3Mask) {
                        Console.WriteLine("Button3");
                        if(activeStroke == null) strokeBegin(true);
                        strokeContinue(axes[0], axes[1], paperColor);
                    } else {    // Probably shouldn't get here
                        Console.WriteLine("None");
                        if(activeStroke != null) strokeEnd();
                    }
                }
            } else {
                if(activeStroke != null) strokeEnd();
            }*/

//            strokeContinue(x, y, activeStroke.Eraser?paperColor:new Cairo.Color(0.0,0.0,0.0,1.0));

            return true;
        }

        private void strokeBegin(bool erase)
        {
            activeStroke = new CairoStroke(erase);

            // Fix undo/redo
            strokes.RemoveRange(strokes.Count - undo, undo);
            undo = 0;

            strokes.Add(activeStroke);
        }

        private void strokeContinue(double x, double y, Cairo.Color stroke_color)
        {
            if(activeStroke == null)
                return;

            Gdk.Rectangle changed =
                 activeStroke.AddPoint(x, y, stroke_color);
            if(changed.Width < 1) changed.Width = 1;
            if(changed.Height < 1) changed.Height = 1;
            changed.Inflate(5, 5);
            QueueDrawArea(changed.X, changed.Y, changed.Width, changed.Height);
            countPoints++;

            Changed(this,new EventArgs());
        }

        private void strokeEnd()
        {
            activeStroke = null;
        }

        private void drawBackground(Cairo.Context cr, Gdk.Rectangle clip)
        {
            Cairo.Color blue   = new Cairo.Color(0.75, 0.75, 1.0,  1.0);
            Cairo.Color red    = new Cairo.Color( 1.0, 0.75, 0.75, 1.0);
            Cairo.Color black  = new Cairo.Color( 0.0, 0.0,  0.0,  1.0);

            double X      = clip.X;
            double Y      = clip.Y;
            double Width  = clip.Width;
            double Height = clip.Height;

            double EdgeDistance = 75.0;

            // Background Color
            cr.Rectangle(X, Y, Width, Height);
            cr.Color = paperColor;
            cr.Fill();

            // Blue Rulings
            if(horizontalRule) {
                cr.Color = blue;
                for(double i = Y - (Y % ruleDistance) + ruleDistance; i <= Y + Height;
                    i += ruleDistance) {
                    cr.MoveTo(X, i);
                    cr.LineTo(X + Width, i);
                    cr.Stroke();
                }
            }
            if(verticalRule) {
                cr.Color = blue;
                for(double i = X - (X % ruleDistance) + ruleDistance; i <= X + Width;
                    i += ruleDistance) {
                    cr.MoveTo(i, Y);
                    cr.LineTo(i, Y + Height);
                    cr.Stroke();
                }
            }

            // Red Line
            if(leftMargin) {
                cr.Color = red;
                cr.MoveTo(EdgeDistance, Y);
                cr.LineTo(EdgeDistance, Y + Height);
                cr.Stroke();
            }

            // Holes
            if(holes) {
                cr.Color = black;
                cr.Arc(EdgeDistance/2, 150, 17, 0, 2 * Math.PI);
                cr.Arc(EdgeDistance/2, 650, 17, 0, 2 * Math.PI);
                cr.Arc(EdgeDistance/2, 1150,17, 0, 2 * Math.PI);
                cr.Fill();
            }
        }

        private void drawStrokes(Cairo.Context cr, Gdk.Rectangle clip)
        {
            int strokeCount = strokes.Count;
            foreach(CairoStroke stroke in strokes) {

                strokeCount--;
                if(strokeCount < undo) break;

                if(stroke.Bounds.IntersectsWith(clip)) {
                    if(stroke.Eraser) {
                        double temp = cr.LineWidth;
                        cr.LineWidth = 10;
                        stroke.Draw(cr, clip);
                        cr.LineWidth = temp;
                    } else {
                        stroke.Draw(cr, clip);
                    }
                }
            }
        }
    }
}
