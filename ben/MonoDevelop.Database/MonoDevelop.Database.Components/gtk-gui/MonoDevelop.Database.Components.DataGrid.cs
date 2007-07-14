// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.42
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace MonoDevelop.Database.Components {
    
    
    public partial class DataGrid {
        
        private Gtk.VBox vbox;
        
        private Gtk.ScrolledWindow scrolledwindow;
        
        private Gtk.TreeView grid;
        
        private Gtk.HBox hbox;
        
        private Gtk.Button buttonFirst;
        
        private Gtk.Button buttonPrevious;
        
        private Gtk.Entry entryCurrent;
        
        private Gtk.Label label1;
        
        private Gtk.Entry entryTotal;
        
        private Gtk.Button buttonNext;
        
        private Gtk.Button buttonLast;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize();
            // Widget MonoDevelop.Database.Components.DataGrid
            Stetic.BinContainer.Attach(this);
            this.Name = "MonoDevelop.Database.Components.DataGrid";
            // Container child MonoDevelop.Database.Components.DataGrid.Gtk.Container+ContainerChild
            this.vbox = new Gtk.VBox();
            this.vbox.Name = "vbox";
            this.vbox.Spacing = 5;
            this.vbox.BorderWidth = ((uint)(5));
            // Container child vbox.Gtk.Box+BoxChild
            this.scrolledwindow = new Gtk.ScrolledWindow();
            this.scrolledwindow.CanFocus = true;
            this.scrolledwindow.Name = "scrolledwindow";
            this.scrolledwindow.VscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow.HscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow.Gtk.Container+ContainerChild
            this.grid = new Gtk.TreeView();
            this.grid.HasDefault = true;
            this.grid.CanFocus = true;
            this.grid.Name = "grid";
            this.grid.EnableSearch = false;
            this.scrolledwindow.Add(this.grid);
            this.vbox.Add(this.scrolledwindow);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.vbox[this.scrolledwindow]));
            w2.Position = 0;
            // Container child vbox.Gtk.Box+BoxChild
            this.hbox = new Gtk.HBox();
            this.hbox.Name = "hbox";
            this.hbox.Spacing = 5;
            // Container child hbox.Gtk.Box+BoxChild
            this.buttonFirst = new Gtk.Button();
            this.buttonFirst.CanFocus = true;
            this.buttonFirst.Name = "buttonFirst";
            this.buttonFirst.UseStock = true;
            this.buttonFirst.UseUnderline = true;
            this.buttonFirst.Label = "gtk-goto-first";
            this.hbox.Add(this.buttonFirst);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.hbox[this.buttonFirst]));
            w3.Position = 0;
            w3.Expand = false;
            w3.Fill = false;
            // Container child hbox.Gtk.Box+BoxChild
            this.buttonPrevious = new Gtk.Button();
            this.buttonPrevious.CanFocus = true;
            this.buttonPrevious.Name = "buttonPrevious";
            this.buttonPrevious.UseStock = true;
            this.buttonPrevious.UseUnderline = true;
            this.buttonPrevious.Label = "gtk-go-back";
            this.hbox.Add(this.buttonPrevious);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.hbox[this.buttonPrevious]));
            w4.Position = 1;
            w4.Expand = false;
            w4.Fill = false;
            // Container child hbox.Gtk.Box+BoxChild
            this.entryCurrent = new Gtk.Entry();
            this.entryCurrent.CanFocus = true;
            this.entryCurrent.Name = "entryCurrent";
            this.entryCurrent.IsEditable = true;
            this.entryCurrent.InvisibleChar = '●';
            this.entryCurrent.Xalign = 0.5F;
            this.hbox.Add(this.entryCurrent);
            Gtk.Box.BoxChild w5 = ((Gtk.Box.BoxChild)(this.hbox[this.entryCurrent]));
            w5.Position = 2;
            w5.Expand = false;
            // Container child hbox.Gtk.Box+BoxChild
            this.label1 = new Gtk.Label();
            this.label1.Name = "label1";
            this.label1.LabelProp = Mono.Unix.Catalog.GetString("of");
            this.hbox.Add(this.label1);
            Gtk.Box.BoxChild w6 = ((Gtk.Box.BoxChild)(this.hbox[this.label1]));
            w6.Position = 3;
            w6.Expand = false;
            w6.Fill = false;
            // Container child hbox.Gtk.Box+BoxChild
            this.entryTotal = new Gtk.Entry();
            this.entryTotal.CanFocus = true;
            this.entryTotal.Name = "entryTotal";
            this.entryTotal.IsEditable = false;
            this.entryTotal.InvisibleChar = '●';
            this.entryTotal.Xalign = 0.5F;
            this.hbox.Add(this.entryTotal);
            Gtk.Box.BoxChild w7 = ((Gtk.Box.BoxChild)(this.hbox[this.entryTotal]));
            w7.Position = 4;
            w7.Expand = false;
            // Container child hbox.Gtk.Box+BoxChild
            this.buttonNext = new Gtk.Button();
            this.buttonNext.CanFocus = true;
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.UseStock = true;
            this.buttonNext.UseUnderline = true;
            this.buttonNext.Label = "gtk-go-forward";
            this.hbox.Add(this.buttonNext);
            Gtk.Box.BoxChild w8 = ((Gtk.Box.BoxChild)(this.hbox[this.buttonNext]));
            w8.Position = 5;
            w8.Expand = false;
            w8.Fill = false;
            // Container child hbox.Gtk.Box+BoxChild
            this.buttonLast = new Gtk.Button();
            this.buttonLast.CanFocus = true;
            this.buttonLast.Name = "buttonLast";
            this.buttonLast.UseStock = true;
            this.buttonLast.UseUnderline = true;
            this.buttonLast.Label = "gtk-goto-last";
            this.hbox.Add(this.buttonLast);
            Gtk.Box.BoxChild w9 = ((Gtk.Box.BoxChild)(this.hbox[this.buttonLast]));
            w9.Position = 6;
            w9.Expand = false;
            w9.Fill = false;
            this.vbox.Add(this.hbox);
            Gtk.Box.BoxChild w10 = ((Gtk.Box.BoxChild)(this.vbox[this.hbox]));
            w10.Position = 1;
            w10.Expand = false;
            w10.Fill = false;
            this.Add(this.vbox);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Show();
            this.buttonFirst.Clicked += new System.EventHandler(this.ButtonFirstClicked);
            this.buttonPrevious.Clicked += new System.EventHandler(this.ButtonPreviousClicked);
            this.entryCurrent.Activated += new System.EventHandler(this.EntryCurrentActivated);
            this.buttonNext.Clicked += new System.EventHandler(this.ButtonNextClicked);
            this.buttonLast.Clicked += new System.EventHandler(this.ButtonLastClicked);
        }
    }
}
