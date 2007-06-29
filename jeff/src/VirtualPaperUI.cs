using System;
using System.Xml;
using Gtk;
using Gnome;
using VirtualPaper;

public class VirtualPaperUI : App
{
    protected Handwriting handwriting;

    private bool hasChanged = false;

    public static Program program;

    private ActionGroup actions = new ActionGroup("Global");
    private UIManager ui        = new UIManager();
    private Menu stylePopup     = new Menu();

    private Toolbar TB;

    public VirtualPaperUI(Program p) : 
            base("Virtual Paper", "Virtual Paper")
    {
        program = p;

        DeleteEvent += new DeleteEventHandler(HandleDelete);

        handwriting = new Handwriting();

        ui.AddUiFromResource("UILayout.xml");
        PopulateActionGroups();
        
        Menus        = ui.GetWidget("/MainMenu")   as MenuBar;
        Toolbar = TB = ui.GetWidget("/ControlBar") as Toolbar;

        AddColorToolButton();

        Contents = handwriting;

        handwriting.PaperColor = Handwriting.White;
        handwriting.Changed += delegate {
            updateUndo();
        };
        
        updateUndo();

        ShowAll();
        program.Run();
    }

    private void AddColorToolButton()
    {
        ColorToolButton ctb;

        ctb = new ColorToolButton(new Gdk.Color(0x00, 0x00, 0x00), "Black");
        ctb.AddColor(new Gdk.Color(0xFF, 0xFF, 0xFF), "White");
        ctb.AddColor(new Gdk.Color(0xFF, 0x00, 0x00), "Red");
        ctb.AddColor(new Gdk.Color(0xFF, 0xFF, 0x00), "Yellow");
        ctb.AddColor(new Gdk.Color(0x00, 0xFF, 0x00), "Green");
        ctb.AddColor(new Gdk.Color(0x00, 0xFF, 0xFF), "Cyan");
        ctb.AddColor(new Gdk.Color(0x00, 0x00, 0xFF), "Blue");
        ctb.AddColor(new Gdk.Color(0xFF, 0x00, 0xFF), "Purple");
        ctb.ColorSelected += ColorChanged;
        ctb.Show();

        TB.Add(ctb);
    }

    private void PopulateActionGroups()
    {
        // Generic Actions
        actions.Add(new ActionEntry[] {
            new ActionEntry("Clear", Gtk.Stock.Clear, "_Clear",
                "<control>u", "Clear the current drawing",
                new EventHandler(HandleClear)),
            new ActionEntry("Export", Gtk.Stock.SaveAs, "_Export",
                "<control>e", "Export as a PNG or other format",
                new EventHandler(HandleExport)),
            new ActionEntry("Save", Gtk.Stock.Save, "_Save",
                "<control>s", "Save to a file that can be re-opened later",
                new EventHandler(HandleSave)),
            new ActionEntry("Open", Gtk.Stock.Open, "_Open",
                "<control>o", "Open a previously saved handwriting document",
                new EventHandler(HandleOpen)),
            new ActionEntry("Undo", Gtk.Stock.Undo, "_Undo",
                "<control>z", "Undo previous action",
                new EventHandler(HandleUndo)),
            new ActionEntry("Redo", Gtk.Stock.Redo, "_Redo",
                "<control><shift>z", "Redo previous action",
                new EventHandler(HandleRedo)),
            new ActionEntry("Pen Style", null, "Pen S_tyle",
                "<control>t", "Change Pen Style",
                delegate { stylePopup.Popup(); } ),
            new ActionEntry("Quit", Gtk.Stock.Quit, "_Quit",
                null, "Quit VirtualPaper",
                delegate { Destroy(); } ),
            new ActionEntry("About", Gtk.Stock.About, "_About",
                "<control>a", "About",
                new EventHandler(HandleAbout)),
            new ActionEntry("Print", Gtk.Stock.Print, "_Print",
                "<control>p", "Print",
                new EventHandler(HandlePrint))
        } );

        // Menus
        actions.Add(new ActionEntry[] {
            new ActionEntry("File", null, "_File", null, "File", null),
            new ActionEntry("Edit", null, "_Edit", null, "Edit", null),
            new ActionEntry("Color", null, "_Color", null, "Color", null),
            new ActionEntry("Style", null, "_Style", null, "Style", null),
            new ActionEntry("Paper", null, "P_aper", null, "Paper", null),
            new ActionEntry("Help", null, "_Help", null, "Help", null)
        } );

/*        actions.Add(new ToggleActionEntry[] {
            new ToggleActionEntry("HandwritingYellowToggle", null,
                "Yellow", null, "Set the background color",
                new EventHandler(HandleYellow),
                false),
            new ToggleActionEntry("HandwritingWideRuleToggle", null,
                "Wide", null, "Set the ruling width",
                new EventHandler(HandleWideRule),
                handwriting.RuleDistance == Handwriting.WideRule),
            new ToggleActionEntry("HandwritingHorizontalRuleToggle", null,
                "Horizontal", null, "Turn horizontal ruling on or off",
                new EventHandler(HandleHorizontalRule),
                handwriting.HorizontalRule),
            new ToggleActionEntry("HandwritingVerticalRuleToggle", null,
                "Vertical", null, "Turn vertical ruling on or off",
                new EventHandler(HandleVerticalRule),
                handwriting.VerticalRule),
            new ToggleActionEntry("HandwritingLeftMarginToggle", null,
                "Left", null, "Turn left margin marker on or off",
                new EventHandler(HandleLeftMargin),
                handwriting.LeftMargin),
            new ToggleActionEntry("HandwritingBindingHolesToggle", null,
                "3-Ring", null, "Turn binding holes on or off",
                new EventHandler(HandleBindingHoles),
                handwriting.Holes)
        } );*/

        ui.InsertActionGroup(actions, 0);
    }

/*    private void PopulateColorPopup()
    {
        MenuItem item;

        item = new MenuItem("Black");
    }*/

    public static void Main(string[] args)
    {
        new VirtualPaperUI(
            new Program("VirtualPaperUI", "0.3", Gnome.Modules.UI, args));
    }

    public bool DoSaveDialog()
    {
        bool retval = false;

        FileChooserDialog chooser = new FileChooserDialog(
            "Save Handwriting", null, FileChooserAction.Save);
        chooser.SetCurrentFolder(Environment.GetFolderPath(
            Environment.SpecialFolder.Personal));
        chooser.AddButton(Gtk.Stock.Cancel, ResponseType.Cancel);
        chooser.AddButton(Gtk.Stock.Ok, ResponseType.Ok);
        chooser.DefaultResponse = ResponseType.Ok;

        if(chooser.Run() == (int)ResponseType.Ok) {
            string path = (new Uri(chooser.Uri)).AbsolutePath;
            XmlTextWriter xml = new XmlTextWriter(path,
                System.Text.Encoding.UTF8);
            xml.Formatting = Formatting.Indented;
            handwriting.Serialize(xml);
            xml.Close();
            hasChanged = false;
            retval = true;
        }

        chooser.Destroy();
        return retval;
    }

    private void updateUndo()
    {
        actions.GetAction("Undo").Sensitive =
            hasChanged = handwriting.CanUndo;
        actions.GetAction("Redo").Sensitive =
            handwriting.CanRedo;
    }

    public void HandlePrint(object o, EventArgs ev)
    {
        // TODO: Printer Stuff
    }

    public void HandleAbout(object o, EventArgs ev)
    {
        String[] authors = { "Jeff Tickle" };
        String[] documenters = { };

        About ab = new About("Virtual Paper",
                            "0.3",
                            "Copyright (C) 2007 Jeff Tickle",
                            "Comments",
                            authors,
                            documenters,
                            "translator",
                            null);
        ab.Run();
    }

    public void HandleClear(object o, EventArgs ev)
    {
        if(!hasChanged) {
            handwriting.Clear();
            return;
        }

        HigMessageDialog dialog = new HigMessageDialog(this, DialogFlags.Modal |
            DialogFlags.NoSeparator, MessageType.Warning, ButtonsType.None,
            "Would you like to save the current page before clearing?",
            "If you clear the page without saving, it will be permanently lost.");
        dialog.AddButton("Clear without Saving", ResponseType.Close);
        dialog.AddButton(Gtk.Stock.Cancel, ResponseType.Cancel);
        dialog.AddButton(Gtk.Stock.SaveAs, ResponseType.Yes);
        ResponseType response = (ResponseType)dialog.Run();
        dialog.Destroy();

        switch(response) {
            case ResponseType.Close:
                handwriting.Clear();
                break;
            case ResponseType.Yes:
                if(DoSaveDialog())
                    handwriting.Clear();
                break;
        }

        updateUndo();
    }

    public void EasterEgg(object o, EventArgs ev)
    {
        HigMessageDialog dialog = new HigMessageDialog(this, DialogFlags.Modal |
            DialogFlags.NoSeparator, MessageType.Warning, ButtonsType.None,
            "Error 1337", "HIGcat is not amused.");
        dialog.AddButton("Lettuce", ResponseType.Close);
        dialog.AddButton("Tomato", ResponseType.Close);
        dialog.AddButton("Cheezburger", ResponseType.Close);
        dialog.Run();
        dialog.Destroy();
    }

    public void HandleExport(object o, EventArgs ev)
    {
        FileChooserDialog chooser = new FileChooserDialog(
            "Export Handwriting to PNG", null, FileChooserAction.Save);
        chooser.SetCurrentFolder(Environment.GetFolderPath(
            Environment.SpecialFolder.Personal));
        chooser.AddButton(Gtk.Stock.Cancel, ResponseType.Cancel);
        chooser.AddButton(Gtk.Stock.Ok, ResponseType.Ok);
        chooser.DefaultResponse = ResponseType.Ok;

        if(chooser.Run() == (int)ResponseType.Ok) {
            string path = (new Uri(chooser.Uri)).AbsolutePath;
            Cairo.ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.ARGB32,
                 handwriting.Allocation.Width, handwriting.Allocation.Height);
            handwriting.DrawToSurface(surface, handwriting.Allocation);
            Console.WriteLine("PNG saved to {0}", path);
            surface.WriteToPng(path);
        }

        chooser.Destroy();
    }

    public void HandleSave(object o, EventArgs ev)
    {
        DoSaveDialog();
    }

    public void HandleOpen(object o, EventArgs ev)
    {
        FileChooserDialog chooser = new FileChooserDialog(
            "Open Handwriting", null, FileChooserAction.Open);
        chooser.SetCurrentFolder(Environment.GetFolderPath(
            Environment.SpecialFolder.Personal));
        chooser.AddButton(Gtk.Stock.Cancel, ResponseType.Cancel);
        chooser.AddButton(Gtk.Stock.Ok, ResponseType.Ok);
        chooser.DefaultResponse = ResponseType.Ok;

        if(chooser.Run() == (int)ResponseType.Ok) {
            string path = (new Uri(chooser.Uri)).AbsolutePath;
            XmlTextReader xml = new XmlTextReader(path);
            handwriting.Clear();
            handwriting.Deserialize(xml);
            xml.Close();
            updateUndo();
            hasChanged = false;
        }

        chooser.Destroy();
    }

    public void HandleUndo(object o, EventArgs ev)
    {
        handwriting.Undo();
        updateUndo();
    }

    public void HandleRedo(object o, EventArgs ev)
    {
        handwriting.Redo();
        updateUndo();
    }

/*    public void HandlePen(object o, EventArgs ev)
    {
//        colorPopup.Popup();
        ColorSelectionDialog colorSel = 
            new ColorSelectionDialog("Select Pen Color");

        Gdk.Color currentColor = new Gdk.Color(0,0,0);
        currentColor.Red   = (ushort)(handwriting.Pen.PenColor.R * 65535);
        currentColor.Green = (ushort)(handwriting.Pen.PenColor.G * 65535);
        currentColor.Blue  = (ushort)(handwriting.Pen.PenColor.B * 65535);

        colorSel.ColorSelection.CurrentColor = currentColor;
        colorSel.ColorSelection.CurrentAlpha = 
            Convert.ToUInt16(handwriting.Pen.PenColor.A * 65535);
        colorSel.ColorSelection.HasOpacityControl = true;

        colorSel.ColorSelection.ColorChanged += ColorChanged;

        colorSel.ShowAll();
    }*/

    public void ColorChanged(object o, ColorSelectedEventArgs ev)
    {
        handwriting.Pen.PenColor = new Cairo.Color(
            (double)ev.NewColor.Red   / 65535.0,
            (double)ev.NewColor.Green / 65535.0,
            (double)ev.NewColor.Blue  / 65535.0,
            1.0);
    }

/*    public void HandleYellow(object o, EventArgs ev)
    {
        if((o as ToggleAction).Active) {
            handwriting.PaperColor = Handwriting.Yellow;
        } else {
            handwriting.PaperColor = Handwriting.White;
        }
    }

    public void HandleWideRule(object o, EventArgs ev)
    {
        if((o as ToggleAction).Active) {
            handwriting.RuleDistance = Handwriting.WideRule;
        } else {
            handwriting.RuleDistance = Handwriting.CollegeRule;
        }
    }

    public void HandleHorizontalRule(object o, EventArgs ev)
    {
        handwriting.HorizontalRule = (o as ToggleAction).Active;
    }

    public void HandleVerticalRule(object o, EventArgs ev)
    {
        handwriting.VerticalRule = (o as ToggleAction).Active;
    }

    public void HandleLeftMargin(object o, EventArgs ev)
    {
        handwriting.LeftMargin = (o as ToggleAction).Active;
    }

    public void HandleBindingHoles(object o, EventArgs ev)
    {
        handwriting.Holes = (o as ToggleAction).Active;
    }*/

    public void HandleDelete(object o, DeleteEventArgs ev)
    {
        ev.RetVal = true;

        if(!hasChanged) {
            ev.RetVal = false;
            program.Quit();
            return;
        }

        HigMessageDialog dialog = new HigMessageDialog(this, DialogFlags.Modal |
            DialogFlags.NoSeparator, MessageType.Warning, ButtonsType.None,
            "Would you like to save the current page before closing?",
            "If you close the page without saving, it will be permanently lost.");
        dialog.AddButton("Close without Saving", ResponseType.Close);
        dialog.AddButton(Gtk.Stock.Cancel, ResponseType.Cancel);
        dialog.AddButton(Gtk.Stock.SaveAs, ResponseType.Yes);
        ResponseType response = (ResponseType)dialog.Run();
        dialog.Destroy();

        switch(response) {
            case ResponseType.Close:
                ev.RetVal = false;
                program.Quit();
                break;
            case ResponseType.Yes:
                if(DoSaveDialog()) {
                    ev.RetVal = false;
                    program.Quit();
                }
                    break;
        }
    }
}
