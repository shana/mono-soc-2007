using System;
using System.Xml;
using Gtk;
using Tablet;

public class HandwritingWindow : Gtk.Window
{
    protected Handwriting handwriting;

    private bool hasChanged = false;

    private ActionGroup actions = new ActionGroup("Global");
    private UIManager ui = new UIManager();

    public HandwritingWindow() : base("Virtual Paper!")
    {
        DeleteEvent += new DeleteEventHandler(HandleDelete);
        VBox box = new VBox();

        handwriting = new Handwriting();
        handwriting.SetSizeRequest(640,480);

        handwriting.RuleDistance = Handwriting.CollegeRule;
        handwriting.HorizontalRule = true;
        handwriting.VerticalRule = false;
        handwriting.LeftMargin = true;
        handwriting.Holes = true;

        ui.AddUiFromResource("UILayout.xml");
        PopulateActionGroups();

        box.PackStart(ui.GetWidget("/ControlBar"), false, false, 0);
        box.PackStart(handwriting, true, true, 0);
        Add(box);

        handwriting.PaperColor = Handwriting.White;
        handwriting.Changed += delegate {
            updateUndo();
        };
        
        updateUndo();
    }

    private void PopulateActionGroups()
    {
        actions.Add(new ActionEntry[] {
            new ActionEntry("HandwritingClearAction", Stock.Clear, "_Clear",
                "<control>u", "Clear the current drawing",
                new EventHandler(HandleClear)),
            new ActionEntry("HandwritingExportAction", Stock.SaveAs, "_Export",
                "<control>e", "Export as a PNG or other format",
                new EventHandler(HandleExport)),
            new ActionEntry("HandwritingSaveAction", Stock.Save, "_Save",
                "<control>s", "Save to a file that can be re-opened later",
                new EventHandler(HandleSave)),
            new ActionEntry("HandwritingOpenAction", Stock.Open, "_Open",
                "<control>o", "Open a previously saved handwriting document",
                new EventHandler(HandleOpen)),
            new ActionEntry("HandwritingUndoAction", Stock.Undo, "_Undo",
                "<control>z", "Undo previous action",
                new EventHandler(HandleUndo)),
            new ActionEntry("HandwritingRedoAction", Stock.Redo, "_Redo",
                "<control><shift>z", "Redo previous action",
                new EventHandler(HandleRedo))
        } );

        actions.Add(new ToggleActionEntry[] {
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
        } );

        ui.InsertActionGroup(actions, 0);
    }

    public static void Main(string[] args)
    {
        Application.Init();
        HandwritingWindow hwin = new HandwritingWindow();

        hwin.ShowAll();
        Application.Run();
    }

    public bool DoSaveDialog()
    {
        bool retval = false;

        FileChooserDialog chooser = new FileChooserDialog(
            "Save Handwriting", null, FileChooserAction.Save);
        chooser.SetCurrentFolder(Environment.GetFolderPath(
            Environment.SpecialFolder.Personal));
        chooser.AddButton(Stock.Cancel, ResponseType.Cancel);
        chooser.AddButton(Stock.Ok, ResponseType.Ok);
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
        actions.GetAction("HandwritingUndoAction").Sensitive =
            hasChanged = handwriting.CanUndo;
        actions.GetAction("HandwritingRedoAction").Sensitive =
            handwriting.CanRedo;
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
        dialog.AddButton(Stock.Cancel, ResponseType.Cancel);
        dialog.AddButton(Stock.SaveAs, ResponseType.Yes);
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

    public void HandleExport(object o, EventArgs ev)
    {
        FileChooserDialog chooser = new FileChooserDialog(
            "Export Handwriting to PNG", null, FileChooserAction.Save);
        chooser.SetCurrentFolder(Environment.GetFolderPath(
            Environment.SpecialFolder.Personal));
        chooser.AddButton(Stock.Cancel, ResponseType.Cancel);
        chooser.AddButton(Stock.Ok, ResponseType.Ok);
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
        chooser.AddButton(Stock.Cancel, ResponseType.Cancel);
        chooser.AddButton(Stock.Ok, ResponseType.Ok);
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

    public void HandleYellow(object o, EventArgs ev)
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
    }

    public void HandleDelete(object o, DeleteEventArgs ev)
    {
        ev.RetVal = true;

        if(!hasChanged) {
            ev.RetVal = false;
            Application.Quit();
            return;
        }

        HigMessageDialog dialog = new HigMessageDialog(this, DialogFlags.Modal |
            DialogFlags.NoSeparator, MessageType.Warning, ButtonsType.None,
            "Would you like to save the current page before closing?",
            "If you close the page without saving, it will be permanently lost.");
        dialog.AddButton("Close without Saving", ResponseType.Close);
        dialog.AddButton(Stock.Cancel, ResponseType.Cancel);
        dialog.AddButton(Stock.SaveAs, ResponseType.Yes);
        ResponseType response = (ResponseType)dialog.Run();
        dialog.Destroy();

        switch(response) {
            case ResponseType.Close:
                ev.RetVal = false;
                Application.Quit();
                break;
            case ResponseType.Yes:
                if(DoSaveDialog()) {
                    ev.RetVal = false;
                    Application.Quit();
                }
                    break;
        }
    }
}
