using System;
using Gtk;
using Gdk;

public delegate void ColorSelectedHandler(object o,
    ColorSelectedEventArgs args);

public class ColorToolButton : MenuToolButton
{
    public event ColorSelectedHandler ColorSelected;

    private int numCustomItems;

    public ColorToolButton(Color color, string name) :
        base(new ColorBlockWidget(color), name)
    {
        Menu = new Menu();

        SeparatorMenuItem sep = new SeparatorMenuItem();
        (Menu as Menu).Append(sep);
        sep.Show();

        // TODO: Figure out how to replace "new AccelGroup()"
        ImageMenuItem colors = new ImageMenuItem(Stock.ColorPicker,
            new AccelGroup());
        if(colors.Child is Label) (colors.Child as Label).Text =
            "More Colors...";
        colors.Activated += MoreColorsActivated;
        (Menu as Menu).Append(colors);
        colors.Show();

        SetMainColor(AddColor(color, name));

        numCustomItems = 0;
    }

    public ImageMenuItem AddColor(Color color, string name)
    {
        ImageMenuItem item = CreateImageMenuItem(color, name);

        if(!(Menu is Menu)) {
            Menu = new Menu();
        }
        Menu m = Menu as Menu;

        m.Insert(item, m.Children.Length - 2);

        item.ShowAll();
        return item;
    }

    public ImageMenuItem AddCustomColor(Color color)
    {
        ImageMenuItem item = CreateImageMenuItem(color, "Custom");

        if(!(Menu is Menu)) {
            Menu = new Menu();
        }
        Menu m = Menu as Menu;

        if(numCustomItems == 0) {
            SeparatorMenuItem sep = new SeparatorMenuItem();
            m.Prepend(sep);
            sep.Show();
        }

        m.Prepend(item);
        numCustomItems++;

        if(numCustomItems > 5) {
            m.Remove(m.Children[5]);
            numCustomItems--;
        }

        item.ShowAll();
        return item;
    }

    private ImageMenuItem CreateImageMenuItem(Color color, string name)
    {
        ImageMenuItem item = new ImageMenuItem(name);
        item.Activated += ColorActivated;
        item.Image = new ColorBlockWidget(color);
        return item;
    }

    public void ColorActivated(object o, EventArgs ev)
    {
        if(!(o is ImageMenuItem)) {
            return;
        }

        ImageMenuItem item = o as ImageMenuItem;

        if(!(item.Image is ColorBlockWidget)) {
            return;
        }

        ColorBlockWidget block = item.Image as ColorBlockWidget;

        ColorSelectedEventArgs args = new ColorSelectedEventArgs(
            block.Color, item);

        OnColorSelected(args);
    }

    public void MoreColorsActivated(object o, EventArgs ev)
    {
        ColorSelectionDialog colorSel =
            new ColorSelectionDialog("Select Color");

        if(IconWidget is ColorBlockWidget) {
            colorSel.ColorSelection.CurrentColor =
                (IconWidget as ColorBlockWidget).Color;
        }

        // TODO: Add support for opacity as Handwriting supports this
        colorSel.ColorSelection.HasOpacityControl = false;
        colorSel.Response += CustomColorResponded;
        colorSel.Run();
    }

    public void CustomColorResponded(object o, ResponseArgs response)
    {
        if(o is ColorSelectionDialog) {
            ColorSelectionDialog d = o as ColorSelectionDialog;
            if(response.ResponseId == ResponseType.Ok) {
                ImageMenuItem item =
                    AddCustomColor(d.ColorSelection.CurrentColor);
                item.Activate();
            }
            d.Destroy();
        }
    }

    public void SetMainColor(ImageMenuItem item)
    {
        if(item.Child is Label && item.Image is ColorBlockWidget) {
            IconWidget = (item.Image as ColorBlockWidget).Clone();
            IconWidget.Show();
            Label = (item.Child as Label).Text;
        }
    }

    public void SetMainColor(Color color, string name)
    {
        IconWidget = new ColorBlockWidget(color);
        Label = name;
    }

    protected void OnColorSelected(ColorSelectedEventArgs args)
    {
        SetMainColor(args.Sender);

        if(ColorSelected != null) {
            ColorSelected((object)args.Sender, args);
        }

        return;
    }

    protected override void OnClicked()
    {
        if(Menu is Menu)
            (Menu as Menu).Popup();

        base.OnClicked();
    }
}

public class ColorSelectedEventArgs : EventArgs
{
    public readonly Color            NewColor;
    public readonly ImageMenuItem    Sender;

    public ColorSelectedEventArgs(Color color, ImageMenuItem imi)
    {
        NewColor = color;
        Sender   = imi;
    }
}

public class ColorBlockWidget : Bin
{
    public readonly Color  Color;

    public ColorBlockWidget(Color c) : base()
    {
        Color = c;
        SetSizeRequest(24, 24);
    }

    protected override bool OnExposeEvent(EventExpose ev)
    {
        if(!IsRealized) {
            return false;
        }

        Gdk.GC context = new Gdk.GC(GdkWindow);
        context.RgbFgColor = Color;
        GdkWindow.DrawRectangle(context, true, ev.Area);

        return true;
    }

    public ColorBlockWidget Clone()
    {
        return new ColorBlockWidget(Color);
    }
}
