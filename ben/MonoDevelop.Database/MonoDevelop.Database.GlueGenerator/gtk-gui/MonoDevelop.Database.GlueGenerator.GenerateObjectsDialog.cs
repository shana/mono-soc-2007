// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.42
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace MonoDevelop.Database.GlueGenerator {
    
    
    public partial class GenerateObjectsDialog {
        
        private Gtk.Notebook notebook;
        
        private Gtk.Table tableStep1;
        
        private MonoDevelop.Database.Components.ConnectionComboBox comboConnection;
        
        private MonoDevelop.Database.Components.ProjectDirectoryComboBox comboProject;
        
        private Gtk.Entry entryNamespace;
        
        private Gtk.Label label3;
        
        private Gtk.Label label4;
        
        private Gtk.Label label5;
        
        private Gtk.Label label1;
        
        private Gtk.Table tableStep2;
        
        private Gtk.HButtonBox hbuttonbox2;
        
        private Gtk.Button buttonAddDataSource;
        
        private Gtk.Button buttonRemoveDataSource;
        
        private Gtk.Label label6;
        
        private Gtk.Label label7;
        
        private Gtk.ScrolledWindow scrolledwindow1;
        
        private Gtk.TreeView treeDataSource;
        
        private Gtk.ScrolledWindow scrolledwindow2;
        
        private Gtk.TreeView listColumns;
        
        private Gtk.Label label2;
        
        private Gtk.Button buttonCancel;
        
        private Gtk.Button buttonBack;
        
        private Gtk.Button buttonForward;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize();
            // Widget MonoDevelop.Database.GlueGenerator.GenerateObjectsDialog
            this.Name = "MonoDevelop.Database.GlueGenerator.GenerateObjectsDialog";
            this.Title = Mono.Unix.Catalog.GetString("Generate Data Objects");
            this.TypeHint = ((Gdk.WindowTypeHint)(1));
            this.WindowPosition = ((Gtk.WindowPosition)(1));
            this.SkipTaskbarHint = true;
            this.HasSeparator = false;
            // Internal child MonoDevelop.Database.GlueGenerator.GenerateObjectsDialog.VBox
            Gtk.VBox w1 = this.VBox;
            w1.Name = "dialog1_VBox";
            w1.BorderWidth = ((uint)(2));
            // Container child dialog1_VBox.Gtk.Box+BoxChild
            this.notebook = new Gtk.Notebook();
            this.notebook.CanFocus = true;
            this.notebook.Name = "notebook";
            this.notebook.CurrentPage = 1;
            // Container child notebook.Gtk.Notebook+NotebookChild
            this.tableStep1 = new Gtk.Table(((uint)(3)), ((uint)(2)), false);
            this.tableStep1.Name = "tableStep1";
            this.tableStep1.RowSpacing = ((uint)(6));
            this.tableStep1.ColumnSpacing = ((uint)(6));
            this.tableStep1.BorderWidth = ((uint)(6));
            // Container child tableStep1.Gtk.Table+TableChild
            this.comboConnection = new MonoDevelop.Database.Components.ConnectionComboBox();
            this.comboConnection.Name = "comboConnection";
            this.tableStep1.Add(this.comboConnection);
            Gtk.Table.TableChild w2 = ((Gtk.Table.TableChild)(this.tableStep1[this.comboConnection]));
            w2.LeftAttach = ((uint)(1));
            w2.RightAttach = ((uint)(2));
            w2.XOptions = ((Gtk.AttachOptions)(4));
            w2.YOptions = ((Gtk.AttachOptions)(4));
            // Container child tableStep1.Gtk.Table+TableChild
            this.comboProject = new MonoDevelop.Database.Components.ProjectDirectoryComboBox();
            this.comboProject.Name = "comboProject";
            this.tableStep1.Add(this.comboProject);
            Gtk.Table.TableChild w3 = ((Gtk.Table.TableChild)(this.tableStep1[this.comboProject]));
            w3.TopAttach = ((uint)(1));
            w3.BottomAttach = ((uint)(2));
            w3.LeftAttach = ((uint)(1));
            w3.RightAttach = ((uint)(2));
            w3.YOptions = ((Gtk.AttachOptions)(4));
            // Container child tableStep1.Gtk.Table+TableChild
            this.entryNamespace = new Gtk.Entry();
            this.entryNamespace.CanFocus = true;
            this.entryNamespace.Name = "entryNamespace";
            this.entryNamespace.IsEditable = true;
            this.entryNamespace.InvisibleChar = '●';
            this.tableStep1.Add(this.entryNamespace);
            Gtk.Table.TableChild w4 = ((Gtk.Table.TableChild)(this.tableStep1[this.entryNamespace]));
            w4.TopAttach = ((uint)(2));
            w4.BottomAttach = ((uint)(3));
            w4.LeftAttach = ((uint)(1));
            w4.RightAttach = ((uint)(2));
            w4.YOptions = ((Gtk.AttachOptions)(4));
            // Container child tableStep1.Gtk.Table+TableChild
            this.label3 = new Gtk.Label();
            this.label3.Name = "label3";
            this.label3.Xalign = 0F;
            this.label3.LabelProp = Mono.Unix.Catalog.GetString("Database");
            this.tableStep1.Add(this.label3);
            Gtk.Table.TableChild w5 = ((Gtk.Table.TableChild)(this.tableStep1[this.label3]));
            w5.XOptions = ((Gtk.AttachOptions)(4));
            w5.YOptions = ((Gtk.AttachOptions)(4));
            // Container child tableStep1.Gtk.Table+TableChild
            this.label4 = new Gtk.Label();
            this.label4.Name = "label4";
            this.label4.Xalign = 0F;
            this.label4.LabelProp = Mono.Unix.Catalog.GetString("Location");
            this.tableStep1.Add(this.label4);
            Gtk.Table.TableChild w6 = ((Gtk.Table.TableChild)(this.tableStep1[this.label4]));
            w6.TopAttach = ((uint)(1));
            w6.BottomAttach = ((uint)(2));
            w6.XOptions = ((Gtk.AttachOptions)(4));
            w6.YOptions = ((Gtk.AttachOptions)(4));
            // Container child tableStep1.Gtk.Table+TableChild
            this.label5 = new Gtk.Label();
            this.label5.Name = "label5";
            this.label5.Xalign = 0F;
            this.label5.LabelProp = Mono.Unix.Catalog.GetString("Namespace");
            this.tableStep1.Add(this.label5);
            Gtk.Table.TableChild w7 = ((Gtk.Table.TableChild)(this.tableStep1[this.label5]));
            w7.TopAttach = ((uint)(2));
            w7.BottomAttach = ((uint)(3));
            w7.XOptions = ((Gtk.AttachOptions)(4));
            w7.YOptions = ((Gtk.AttachOptions)(4));
            this.notebook.Add(this.tableStep1);
            Gtk.Notebook.NotebookChild w8 = ((Gtk.Notebook.NotebookChild)(this.notebook[this.tableStep1]));
            w8.TabExpand = false;
            // Notebook tab
            this.label1 = new Gtk.Label();
            this.label1.Name = "label1";
            this.label1.LabelProp = Mono.Unix.Catalog.GetString("Step 1");
            this.notebook.SetTabLabel(this.tableStep1, this.label1);
            // Container child notebook.Gtk.Notebook+NotebookChild
            this.tableStep2 = new Gtk.Table(((uint)(3)), ((uint)(2)), false);
            this.tableStep2.Name = "tableStep2";
            this.tableStep2.RowSpacing = ((uint)(6));
            this.tableStep2.ColumnSpacing = ((uint)(6));
            this.tableStep2.BorderWidth = ((uint)(6));
            // Container child tableStep2.Gtk.Table+TableChild
            this.hbuttonbox2 = new Gtk.HButtonBox();
            this.hbuttonbox2.Name = "hbuttonbox2";
            this.hbuttonbox2.Spacing = 6;
            // Container child hbuttonbox2.Gtk.ButtonBox+ButtonBoxChild
            this.buttonAddDataSource = new Gtk.Button();
            this.buttonAddDataSource.CanFocus = true;
            this.buttonAddDataSource.Name = "buttonAddDataSource";
            this.buttonAddDataSource.UseStock = true;
            this.buttonAddDataSource.UseUnderline = true;
            this.buttonAddDataSource.Label = "gtk-add";
            this.hbuttonbox2.Add(this.buttonAddDataSource);
            Gtk.ButtonBox.ButtonBoxChild w9 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox2[this.buttonAddDataSource]));
            w9.Expand = false;
            w9.Fill = false;
            // Container child hbuttonbox2.Gtk.ButtonBox+ButtonBoxChild
            this.buttonRemoveDataSource = new Gtk.Button();
            this.buttonRemoveDataSource.CanFocus = true;
            this.buttonRemoveDataSource.Name = "buttonRemoveDataSource";
            this.buttonRemoveDataSource.UseStock = true;
            this.buttonRemoveDataSource.UseUnderline = true;
            this.buttonRemoveDataSource.Label = "gtk-remove";
            this.hbuttonbox2.Add(this.buttonRemoveDataSource);
            Gtk.ButtonBox.ButtonBoxChild w10 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox2[this.buttonRemoveDataSource]));
            w10.Position = 1;
            w10.Expand = false;
            w10.Fill = false;
            this.tableStep2.Add(this.hbuttonbox2);
            Gtk.Table.TableChild w11 = ((Gtk.Table.TableChild)(this.tableStep2[this.hbuttonbox2]));
            w11.TopAttach = ((uint)(2));
            w11.BottomAttach = ((uint)(3));
            w11.XOptions = ((Gtk.AttachOptions)(4));
            w11.YOptions = ((Gtk.AttachOptions)(4));
            // Container child tableStep2.Gtk.Table+TableChild
            this.label6 = new Gtk.Label();
            this.label6.Name = "label6";
            this.label6.Xalign = 0F;
            this.label6.LabelProp = Mono.Unix.Catalog.GetString("Data Source");
            this.tableStep2.Add(this.label6);
            Gtk.Table.TableChild w12 = ((Gtk.Table.TableChild)(this.tableStep2[this.label6]));
            w12.XOptions = ((Gtk.AttachOptions)(4));
            w12.YOptions = ((Gtk.AttachOptions)(4));
            // Container child tableStep2.Gtk.Table+TableChild
            this.label7 = new Gtk.Label();
            this.label7.Name = "label7";
            this.label7.Xalign = 0F;
            this.label7.LabelProp = Mono.Unix.Catalog.GetString("Columns");
            this.tableStep2.Add(this.label7);
            Gtk.Table.TableChild w13 = ((Gtk.Table.TableChild)(this.tableStep2[this.label7]));
            w13.LeftAttach = ((uint)(1));
            w13.RightAttach = ((uint)(2));
            w13.XOptions = ((Gtk.AttachOptions)(4));
            w13.YOptions = ((Gtk.AttachOptions)(4));
            // Container child tableStep2.Gtk.Table+TableChild
            this.scrolledwindow1 = new Gtk.ScrolledWindow();
            this.scrolledwindow1.CanFocus = true;
            this.scrolledwindow1.Name = "scrolledwindow1";
            this.scrolledwindow1.VscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow1.HscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow1.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow1.Gtk.Container+ContainerChild
            this.treeDataSource = new Gtk.TreeView();
            this.treeDataSource.CanFocus = true;
            this.treeDataSource.Name = "treeDataSource";
            this.scrolledwindow1.Add(this.treeDataSource);
            this.tableStep2.Add(this.scrolledwindow1);
            Gtk.Table.TableChild w15 = ((Gtk.Table.TableChild)(this.tableStep2[this.scrolledwindow1]));
            w15.TopAttach = ((uint)(1));
            w15.BottomAttach = ((uint)(2));
            w15.XOptions = ((Gtk.AttachOptions)(4));
            // Container child tableStep2.Gtk.Table+TableChild
            this.scrolledwindow2 = new Gtk.ScrolledWindow();
            this.scrolledwindow2.CanFocus = true;
            this.scrolledwindow2.Name = "scrolledwindow2";
            this.scrolledwindow2.VscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow2.HscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow2.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow2.Gtk.Container+ContainerChild
            this.listColumns = new Gtk.TreeView();
            this.listColumns.CanFocus = true;
            this.listColumns.Name = "listColumns";
            this.scrolledwindow2.Add(this.listColumns);
            this.tableStep2.Add(this.scrolledwindow2);
            Gtk.Table.TableChild w17 = ((Gtk.Table.TableChild)(this.tableStep2[this.scrolledwindow2]));
            w17.TopAttach = ((uint)(1));
            w17.BottomAttach = ((uint)(3));
            w17.LeftAttach = ((uint)(1));
            w17.RightAttach = ((uint)(2));
            this.notebook.Add(this.tableStep2);
            Gtk.Notebook.NotebookChild w18 = ((Gtk.Notebook.NotebookChild)(this.notebook[this.tableStep2]));
            w18.Position = 1;
            w18.TabExpand = false;
            // Notebook tab
            this.label2 = new Gtk.Label();
            this.label2.Name = "label2";
            this.label2.LabelProp = Mono.Unix.Catalog.GetString("Step 2");
            this.notebook.SetTabLabel(this.tableStep2, this.label2);
            w1.Add(this.notebook);
            Gtk.Box.BoxChild w19 = ((Gtk.Box.BoxChild)(w1[this.notebook]));
            w19.Position = 0;
            // Internal child MonoDevelop.Database.GlueGenerator.GenerateObjectsDialog.ActionArea
            Gtk.HButtonBox w20 = this.ActionArea;
            w20.Name = "dialog1_ActionArea";
            w20.Spacing = 6;
            w20.BorderWidth = ((uint)(5));
            w20.LayoutStyle = ((Gtk.ButtonBoxStyle)(4));
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonCancel = new Gtk.Button();
            this.buttonCancel.CanDefault = true;
            this.buttonCancel.CanFocus = true;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseStock = true;
            this.buttonCancel.UseUnderline = true;
            this.buttonCancel.Label = "gtk-cancel";
            this.AddActionWidget(this.buttonCancel, -6);
            Gtk.ButtonBox.ButtonBoxChild w21 = ((Gtk.ButtonBox.ButtonBoxChild)(w20[this.buttonCancel]));
            w21.Expand = false;
            w21.Fill = false;
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonBack = new Gtk.Button();
            this.buttonBack.Sensitive = false;
            this.buttonBack.CanDefault = true;
            this.buttonBack.CanFocus = true;
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.UseStock = true;
            this.buttonBack.UseUnderline = true;
            this.buttonBack.Label = "gtk-go-back";
            this.AddActionWidget(this.buttonBack, 0);
            Gtk.ButtonBox.ButtonBoxChild w22 = ((Gtk.ButtonBox.ButtonBoxChild)(w20[this.buttonBack]));
            w22.Position = 1;
            w22.Expand = false;
            w22.Fill = false;
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonForward = new Gtk.Button();
            this.buttonForward.Sensitive = false;
            this.buttonForward.CanDefault = true;
            this.buttonForward.CanFocus = true;
            this.buttonForward.Name = "buttonForward";
            this.buttonForward.UseStock = true;
            this.buttonForward.UseUnderline = true;
            this.buttonForward.Label = "gtk-go-forward";
            this.AddActionWidget(this.buttonForward, 0);
            Gtk.ButtonBox.ButtonBoxChild w23 = ((Gtk.ButtonBox.ButtonBoxChild)(w20[this.buttonForward]));
            w23.Position = 2;
            w23.Expand = false;
            w23.Fill = false;
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 580;
            this.DefaultHeight = 423;
            this.Show();
            this.entryNamespace.Changed += new System.EventHandler(this.NamespaceChanged);
            this.comboProject.Changed += new System.EventHandler(this.LocationChanged);
            this.comboConnection.Changed += new System.EventHandler(this.ConnectionChanged);
            this.buttonAddDataSource.Clicked += new System.EventHandler(this.AddClicked);
            this.buttonRemoveDataSource.Clicked += new System.EventHandler(this.RemoveClicked);
            this.buttonCancel.Clicked += new System.EventHandler(this.CancelClicked);
            this.buttonBack.Clicked += new System.EventHandler(this.BackClicked);
            this.buttonForward.Clicked += new System.EventHandler(this.ForwardClicked);
        }
    }
}
