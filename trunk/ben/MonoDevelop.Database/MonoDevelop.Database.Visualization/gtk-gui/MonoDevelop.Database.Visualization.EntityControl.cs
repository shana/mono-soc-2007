// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.42
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace MonoDevelop.Database.Visualization {
    
    
    public partial class EntityControl {
        
        private Gtk.Frame frame1;
        
        private Gtk.Alignment GtkAlignment;
        
        private Gtk.ScrolledWindow scrolledwindow;
        
        private Gtk.TreeView list;
        
        private Gtk.Label labelCaption;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize();
            // Widget MonoDevelop.Database.Visualization.EntityControl
            Stetic.BinContainer.Attach(this);
            this.Name = "MonoDevelop.Database.Visualization.EntityControl";
            // Container child MonoDevelop.Database.Visualization.EntityControl.Gtk.Container+ContainerChild
            this.frame1 = new Gtk.Frame();
            this.frame1.Name = "frame1";
            this.frame1.LabelXalign = 0F;
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment = new Gtk.Alignment(0F, 0F, 1F, 1F);
            this.GtkAlignment.Name = "GtkAlignment";
            this.GtkAlignment.LeftPadding = ((uint)(12));
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            this.scrolledwindow = new Gtk.ScrolledWindow();
            this.scrolledwindow.CanFocus = true;
            this.scrolledwindow.Name = "scrolledwindow";
            this.scrolledwindow.VscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow.HscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow.Gtk.Container+ContainerChild
            this.list = new Gtk.TreeView();
            this.list.CanFocus = true;
            this.list.Name = "list";
            this.scrolledwindow.Add(this.list);
            this.GtkAlignment.Add(this.scrolledwindow);
            this.frame1.Add(this.GtkAlignment);
            this.labelCaption = new Gtk.Label();
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.LabelProp = Mono.Unix.Catalog.GetString("<b>Entity</b>");
            this.labelCaption.UseMarkup = true;
            this.frame1.LabelWidget = this.labelCaption;
            this.Add(this.frame1);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Show();
        }
    }
}
