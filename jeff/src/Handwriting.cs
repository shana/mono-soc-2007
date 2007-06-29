using System;
using System.Xml;
using System.Collections.Generic;
using Cairo;
using Gtk;
using Gdk;

namespace VirtualPaper
{
    public class Handwriting : DrawingArea
    {
        public static readonly Cairo.Color Yellow = new Cairo.Color(1.0, 1.0, 0.75, 1.0);
        public static readonly Cairo.Color White = new Cairo.Color(1.0, 1.0, 1.0, 1.0);

        public event EventHandler Changed;

        private List<CairoStroke> strokes;
        private CairoStroke activeStroke;
        private PenStyle penStyle;
        private PenStyle eraserStyle;
        private Paper paper;
        private Cairo.Color paperColor;
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

        public PenStyle Pen {
            get {
                return penStyle;
            }
            set {
                penStyle = value;
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

            paper = new NotebookPaper(White);

            penStyle = new PenStyle();
            penStyle.PenColor = new Cairo.Color(0.0,0.0,1.0,1.0);
            penStyle.PenSize  = 3.0;

            eraserStyle = new PenStyle();
            eraserStyle.PenColor = paperColor;
            eraserStyle.PenSize  = 10.0;

//            GdkWindow.Cursor = Cursor.NewFromName(Display.Default, "Clock");
        }

        public void DrawToSurface(Cairo.Surface surface, Gdk.Rectangle size)
        {
            Cairo.Context cr = new Cairo.Context(surface);

            paper.Draw(cr, size);
            drawStrokes(cr, size);

            ((IDisposable)cr).Dispose();
        }

        public void Serialize(XmlTextWriter xml)
        {
            xml.WriteStartDocument();
            xml.WriteStartElement(null, "handwriting-data", null);
            xml.WriteAttributeString("version", "0.1");

            // Paper Configuration
            xml.WriteStartElement(null, "paper", null);

            paper.Serialize(xml);

            xml.WriteEndElement();

            // Strokes
            xml.WriteStartElement(null, "strokes", null);

            foreach(Stroke s in strokes) {
                s.WriteXml(xml);
            }

            xml.WriteEndElement();
        }

        public void Deserialize(XmlTextReader xml)
        {
            Clear();

            while(xml.Read()) {
                switch(xml.NodeType) {
                case XmlNodeType.Element:
                    if(xml.Name == "handwriting-data") {
                        break;
                    } else if(xml.Name == "paper") {
                        paper = Paper.Deserialize(xml);
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
                Stroke undoneStroke = strokes[strokes.Count - undo];
                QueuePaddedDrawArea(undoneStroke);
            }
        }

        public void Redo()
        {
            if(CanRedo) {
                Stroke redoneStroke = strokes[strokes.Count - undo];
                undo--;
                QueuePaddedDrawArea(redoneStroke);
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

            paper.Draw(cr, rect);
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
                    strokeContinue(axes[0], axes[1], axes[2]);
                } else {
                    strokeContinue(axes[0], axes[1], 1.0);
                }
            } else if(ev.Device.Source == InputSource.Eraser ||
                      mask == ModifierType.Button3Mask) {
                strokeContinue(axes[0], axes[1], 1.0);
            } else {
                // Unknown Button
            }

            return true;
        }

        private void strokeBegin(bool erase)
        {
            activeStroke = new CairoStroke(erase?eraserStyle:penStyle);

            // Fix undo/redo
            strokes.RemoveRange(strokes.Count - undo, undo);
            undo = 0;

            strokes.Add(activeStroke);
        }

        private void strokeContinue(double x, double y, double pressure)
        {
            if(activeStroke == null)
                return;

            Gdk.Rectangle changed =
                 activeStroke.AddPoint(x, y, pressure);
            if(changed.Width < 1) changed.Width = 1;
            if(changed.Height < 1) changed.Height = 1;
            QueuePaddedDrawArea(changed);
            countPoints++;

            Changed(this,new EventArgs());
        }

        private void strokeEnd()
        {
            activeStroke = null;
        }

        private void QueuePaddedDrawArea(Stroke s)
        {
            QueuePaddedDrawArea((int)s.X,(int)s.Y,(int)s.Width,(int)s.Height);
        }

        private void QueuePaddedDrawArea(Gdk.Rectangle r)
        {
            QueuePaddedDrawArea(r.X, r.Y, r.Width, r.Height);
        }

        private void QueuePaddedDrawArea(int X, int Y, int Width, int Height) {
            int lineWidth = Convert.ToInt32(penStyle.PenSize);
            QueueDrawArea(X     -   lineWidth, Y      -   lineWidth,
                          Width + 2*lineWidth, Height + 2*lineWidth);
        }

        private void drawStrokes(Cairo.Context cr, Gdk.Rectangle clip)
        {
            int strokeCount = strokes.Count;
            foreach(CairoStroke stroke in strokes) {

                strokeCount--;
                if(strokeCount < undo) break;

                if(stroke.Bounds.IntersectsWith(clip)) {
                    stroke.Draw(cr, clip);
                }
            }
        }
    }
}
