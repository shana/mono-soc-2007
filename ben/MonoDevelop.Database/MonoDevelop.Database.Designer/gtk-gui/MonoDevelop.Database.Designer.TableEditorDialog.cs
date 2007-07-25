// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.42
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace MonoDevelop.Database.Designer {
    
    
    public partial class TableEditorDialog {
        
        private Gtk.VBox vboxContent;
        
        private Gtk.HBox hboxName;
        
        private Gtk.Label label7;
        
        private Gtk.Entry entryName;
        
        private Gtk.CheckButton checkPreview;
        
        private Gtk.Button buttonCancel;
        
        private Gtk.Button buttonOk;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize();
            // Widget MonoDevelop.Database.Designer.TableEditorDialog
            this.Name = "MonoDevelop.Database.Designer.TableEditorDialog";
            this.Title = Mono.Unix.Catalog.GetString("Edit Table");
            this.TypeHint = ((Gdk.WindowTypeHint)(1));
            this.WindowPosition = ((Gtk.WindowPosition)(1));
            this.SkipTaskbarHint = true;
            this.HasSeparator = false;
            // Internal child MonoDevelop.Database.Designer.TableEditorDialog.VBox
            Gtk.VBox w1 = this.VBox;
            w1.Name = "dialog1_VBox";
            w1.BorderWidth = ((uint)(2));
            // Container child dialog1_VBox.Gtk.Box+BoxChild
            this.vboxContent = new Gtk.VBox();
            this.vboxContent.Name = "vboxContent";
            this.vboxContent.Spacing = 6;
            this.vboxContent.BorderWidth = ((uint)(6));
            // Container child vboxContent.Gtk.Box+BoxChild
            this.hboxName = new Gtk.HBox();
            this.hboxName.Name = "hboxName";
            this.hboxName.Spacing = 6;
            // Container child hboxName.Gtk.Box+BoxChild
            this.label7 = new Gtk.Label();
            this.label7.Name = "label7";
            this.label7.Xalign = 0F;
            this.label7.LabelProp = Mono.Unix.Catalog.GetString("Name");
            this.hboxName.Add(this.label7);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.hboxName[this.label7]));
            w2.Position = 0;
            w2.Expand = false;
            w2.Fill = false;
            // Container child hboxName.Gtk.Box+BoxChild
            this.entryName = new Gtk.Entry();
            this.entryName.CanFocus = true;
            this.entryName.Name = "entryName";
            this.entryName.IsEditable = true;
            this.entryName.InvisibleChar = '●';
            this.hboxName.Add(this.entryName);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.hboxName[this.entryName]));
            w3.Position = 1;
            this.vboxContent.Add(this.hboxName);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.vboxContent[this.hboxName]));
            w4.Position = 0;
            w4.Expand = false;
            w4.Fill = false;
            // Container child vboxContent.Gtk.Box+BoxChild
            this.checkPreview = new Gtk.CheckButton();
            this.checkPreview.CanFocus = true;
            this.checkPreview.Name = "checkPreview";
            this.checkPreview.Label = Mono.Unix.Catalog.GetString("Preview SQL");
            this.checkPreview.DrawIndicator = true;
            this.checkPreview.UseUnderline = true;
            this.vboxContent.Add(this.checkPreview);
            Gtk.Box.BoxChild w5 = ((Gtk.Box.BoxChild)(this.vboxContent[this.checkPreview]));
            w5.PackType = ((Gtk.PackType)(1));
            w5.Position = 2;
            w5.Expand = false;
            w5.Fill = false;
            w1.Add(this.vboxContent);
            Gtk.Box.BoxChild w6 = ((Gtk.Box.BoxChild)(w1[this.vboxContent]));
            w6.Position = 0;
            // Internal child MonoDevelop.Database.Designer.TableEditorDialog.ActionArea
            Gtk.HButtonBox w7 = this.ActionArea;
            w7.Name = "dialog1_ActionArea";
            w7.Spacing = 6;
            w7.BorderWidth = ((uint)(5));
            w7.LayoutStyle = ((Gtk.ButtonBoxStyle)(4));
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonCancel = new Gtk.Button();
            this.buttonCancel.CanDefault = true;
            this.buttonCancel.CanFocus = true;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseStock = true;
            this.buttonCancel.UseUnderline = true;
            this.buttonCancel.Label = "gtk-cancel";
            this.AddActionWidget(this.buttonCancel, -6);
            Gtk.ButtonBox.ButtonBoxChild w8 = ((Gtk.ButtonBox.ButtonBoxChild)(w7[this.buttonCancel]));
            w8.Expand = false;
            w8.Fill = false;
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonOk = new Gtk.Button();
            this.buttonOk.CanDefault = true;
            this.buttonOk.CanFocus = true;
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseStock = true;
            this.buttonOk.UseUnderline = true;
            this.buttonOk.Label = "gtk-ok";
            this.AddActionWidget(this.buttonOk, -5);
            Gtk.ButtonBox.ButtonBoxChild w9 = ((Gtk.ButtonBox.ButtonBoxChild)(w7[this.buttonOk]));
            w9.Position = 1;
            w9.Expand = false;
            w9.Fill = false;
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 538;
            this.DefaultHeight = 355;
            this.Show();
            this.entryName.Changed += new System.EventHandler(this.NameChanged);
            this.buttonCancel.Clicked += new System.EventHandler(this.CancelClicked);
            this.buttonOk.Clicked += new System.EventHandler(this.OkClicked);
        }
    }
}
