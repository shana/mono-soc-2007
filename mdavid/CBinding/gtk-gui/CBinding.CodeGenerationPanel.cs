// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.42
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace CBinding {
    
    
    public partial class CodeGenerationPanel {
        
        private Gtk.Notebook notebook1;
        
        private Gtk.VPaned vpaned2;
        
        private Gtk.Table table1;
        
        private Gtk.Entry defineSymbolsTextEntry;
        
        private Gtk.Label label12;
        
        private Gtk.Label label4;
        
        private Gtk.Label label5;
        
        private Gtk.Label label6;
        
        private Gtk.SpinButton optimizationSpinButton;
        
        private Gtk.ComboBox targetComboBox;
        
        private Gtk.VBox vbox1;
        
        private Gtk.RadioButton noWarningRadio;
        
        private Gtk.RadioButton normalWarningRadio;
        
        private Gtk.RadioButton allWarningRadio;
        
        private Gtk.Frame frame2;
        
        private Gtk.Alignment GtkAlignment;
        
        private Gtk.Table table5;
        
        private Gtk.Label label11;
        
        private Gtk.Label label7;
        
        private Gtk.ScrolledWindow scrolledwindow4;
        
        private Gtk.TextView extraCompilerTextView;
        
        private Gtk.ScrolledWindow scrolledwindow5;
        
        private Gtk.TextView extraLinkerTextView;
        
        private Gtk.Label GtkLabel12;
        
        private Gtk.Label label1;
        
        private Gtk.Table table2;
        
        private Gtk.Button addLibButton;
        
        private Gtk.Label label8;
        
        private Gtk.Entry libAddEntry;
        
        private Gtk.ScrolledWindow scrolledwindow1;
        
        private Gtk.TreeView libTreeView;
        
        private Gtk.VBox vbox4;
        
        private Gtk.Button browseButton;
        
        private Gtk.Button removeLibButton;
        
        private Gtk.Label label2;
        
        private Gtk.VPaned vpaned1;
        
        private Gtk.Table table3;
        
        private Gtk.Button includePathAddButton;
        
        private Gtk.Entry includePathEntry;
        
        private Gtk.Label label9;
        
        private Gtk.ScrolledWindow scrolledwindow2;
        
        private Gtk.TreeView includePathTreeView;
        
        private Gtk.VBox vbox5;
        
        private Gtk.Button includePathBrowseButton;
        
        private Gtk.Button includePathRemoveButton;
        
        private Gtk.Table table4;
        
        private Gtk.Label label10;
        
        private Gtk.Button libPathAddButton;
        
        private Gtk.Entry libPathEntry;
        
        private Gtk.ScrolledWindow scrolledwindow3;
        
        private Gtk.TreeView libPathTreeView;
        
        private Gtk.VBox vbox3;
        
        private Gtk.Button libPathBrowseButton;
        
        private Gtk.Button libPathRemoveButton;
        
        private Gtk.Label label3;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize();
            // Widget CBinding.CodeGenerationPanel
            Stetic.BinContainer.Attach(this);
            this.Name = "CBinding.CodeGenerationPanel";
            // Container child CBinding.CodeGenerationPanel.Gtk.Container+ContainerChild
            this.notebook1 = new Gtk.Notebook();
            this.notebook1.CanFocus = true;
            this.notebook1.Name = "notebook1";
            this.notebook1.CurrentPage = 0;
            // Container child notebook1.Gtk.Notebook+NotebookChild
            this.vpaned2 = new Gtk.VPaned();
            this.vpaned2.CanFocus = true;
            this.vpaned2.Name = "vpaned2";
            this.vpaned2.Position = 195;
            // Container child vpaned2.Gtk.Paned+PanedChild
            this.table1 = new Gtk.Table(((uint)(4)), ((uint)(2)), false);
            this.table1.Name = "table1";
            this.table1.RowSpacing = ((uint)(10));
            this.table1.ColumnSpacing = ((uint)(10));
            this.table1.BorderWidth = ((uint)(3));
            // Container child table1.Gtk.Table+TableChild
            this.defineSymbolsTextEntry = new Gtk.Entry();
            Gtk.Tooltips w1 = new Gtk.Tooltips();
            w1.SetTip(this.defineSymbolsTextEntry, "A space seperated list of symbols to define.", "A space seperated list of symbols to define.");
            this.defineSymbolsTextEntry.CanFocus = true;
            this.defineSymbolsTextEntry.Name = "defineSymbolsTextEntry";
            this.defineSymbolsTextEntry.IsEditable = true;
            this.defineSymbolsTextEntry.InvisibleChar = '●';
            this.table1.Add(this.defineSymbolsTextEntry);
            Gtk.Table.TableChild w2 = ((Gtk.Table.TableChild)(this.table1[this.defineSymbolsTextEntry]));
            w2.TopAttach = ((uint)(3));
            w2.BottomAttach = ((uint)(4));
            w2.LeftAttach = ((uint)(1));
            w2.RightAttach = ((uint)(2));
            w2.XOptions = ((Gtk.AttachOptions)(4));
            w2.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label12 = new Gtk.Label();
            this.label12.Name = "label12";
            this.label12.Xpad = 10;
            this.label12.Xalign = 0F;
            this.label12.LabelProp = Mono.Unix.Catalog.GetString("Define Symbols:");
            this.table1.Add(this.label12);
            Gtk.Table.TableChild w3 = ((Gtk.Table.TableChild)(this.table1[this.label12]));
            w3.TopAttach = ((uint)(3));
            w3.BottomAttach = ((uint)(4));
            w3.XOptions = ((Gtk.AttachOptions)(4));
            w3.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label4 = new Gtk.Label();
            this.label4.Name = "label4";
            this.label4.Xpad = 10;
            this.label4.Xalign = 0F;
            this.label4.LabelProp = Mono.Unix.Catalog.GetString("Warning Level:");
            this.table1.Add(this.label4);
            Gtk.Table.TableChild w4 = ((Gtk.Table.TableChild)(this.table1[this.label4]));
            w4.XOptions = ((Gtk.AttachOptions)(4));
            w4.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label5 = new Gtk.Label();
            this.label5.Name = "label5";
            this.label5.Xpad = 10;
            this.label5.Xalign = 0F;
            this.label5.LabelProp = Mono.Unix.Catalog.GetString("Optimization Level:");
            this.table1.Add(this.label5);
            Gtk.Table.TableChild w5 = ((Gtk.Table.TableChild)(this.table1[this.label5]));
            w5.TopAttach = ((uint)(1));
            w5.BottomAttach = ((uint)(2));
            w5.XOptions = ((Gtk.AttachOptions)(4));
            w5.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label6 = new Gtk.Label();
            this.label6.Name = "label6";
            this.label6.Xpad = 10;
            this.label6.Xalign = 0F;
            this.label6.LabelProp = Mono.Unix.Catalog.GetString("Target:");
            this.table1.Add(this.label6);
            Gtk.Table.TableChild w6 = ((Gtk.Table.TableChild)(this.table1[this.label6]));
            w6.TopAttach = ((uint)(2));
            w6.BottomAttach = ((uint)(3));
            w6.XOptions = ((Gtk.AttachOptions)(4));
            w6.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.optimizationSpinButton = new Gtk.SpinButton(0, 3, 1);
            this.optimizationSpinButton.CanFocus = true;
            this.optimizationSpinButton.Name = "optimizationSpinButton";
            this.optimizationSpinButton.Adjustment.PageIncrement = 10;
            this.optimizationSpinButton.ClimbRate = 1;
            this.optimizationSpinButton.Numeric = true;
            this.table1.Add(this.optimizationSpinButton);
            Gtk.Table.TableChild w7 = ((Gtk.Table.TableChild)(this.table1[this.optimizationSpinButton]));
            w7.TopAttach = ((uint)(1));
            w7.BottomAttach = ((uint)(2));
            w7.LeftAttach = ((uint)(1));
            w7.RightAttach = ((uint)(2));
            w7.XOptions = ((Gtk.AttachOptions)(4));
            w7.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.targetComboBox = Gtk.ComboBox.NewText();
            this.targetComboBox.AppendText(Mono.Unix.Catalog.GetString("Executable"));
            this.targetComboBox.AppendText(Mono.Unix.Catalog.GetString("Static Library"));
            this.targetComboBox.AppendText(Mono.Unix.Catalog.GetString("Shared Object"));
            this.targetComboBox.Name = "targetComboBox";
            this.table1.Add(this.targetComboBox);
            Gtk.Table.TableChild w8 = ((Gtk.Table.TableChild)(this.table1[this.targetComboBox]));
            w8.TopAttach = ((uint)(2));
            w8.BottomAttach = ((uint)(3));
            w8.LeftAttach = ((uint)(1));
            w8.RightAttach = ((uint)(2));
            w8.XOptions = ((Gtk.AttachOptions)(4));
            w8.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.vbox1 = new Gtk.VBox();
            this.vbox1.Name = "vbox1";
            this.vbox1.Spacing = 1;
            // Container child vbox1.Gtk.Box+BoxChild
            this.noWarningRadio = new Gtk.RadioButton(Mono.Unix.Catalog.GetString("no warnings"));
            this.noWarningRadio.CanFocus = true;
            this.noWarningRadio.Name = "noWarningRadio";
            this.noWarningRadio.Active = true;
            this.noWarningRadio.DrawIndicator = true;
            this.noWarningRadio.UseUnderline = true;
            this.noWarningRadio.Group = new GLib.SList(System.IntPtr.Zero);
            this.vbox1.Add(this.noWarningRadio);
            Gtk.Box.BoxChild w9 = ((Gtk.Box.BoxChild)(this.vbox1[this.noWarningRadio]));
            w9.Position = 0;
            w9.Expand = false;
            w9.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.normalWarningRadio = new Gtk.RadioButton(Mono.Unix.Catalog.GetString("normal"));
            this.normalWarningRadio.CanFocus = true;
            this.normalWarningRadio.Name = "normalWarningRadio";
            this.normalWarningRadio.DrawIndicator = true;
            this.normalWarningRadio.UseUnderline = true;
            this.normalWarningRadio.Group = this.noWarningRadio.Group;
            this.vbox1.Add(this.normalWarningRadio);
            Gtk.Box.BoxChild w10 = ((Gtk.Box.BoxChild)(this.vbox1[this.normalWarningRadio]));
            w10.Position = 1;
            w10.Expand = false;
            w10.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.allWarningRadio = new Gtk.RadioButton(Mono.Unix.Catalog.GetString("all"));
            this.allWarningRadio.CanFocus = true;
            this.allWarningRadio.Name = "allWarningRadio";
            this.allWarningRadio.DrawIndicator = true;
            this.allWarningRadio.UseUnderline = true;
            this.allWarningRadio.Group = this.noWarningRadio.Group;
            this.vbox1.Add(this.allWarningRadio);
            Gtk.Box.BoxChild w11 = ((Gtk.Box.BoxChild)(this.vbox1[this.allWarningRadio]));
            w11.Position = 2;
            w11.Expand = false;
            w11.Fill = false;
            this.table1.Add(this.vbox1);
            Gtk.Table.TableChild w12 = ((Gtk.Table.TableChild)(this.table1[this.vbox1]));
            w12.LeftAttach = ((uint)(1));
            w12.RightAttach = ((uint)(2));
            w12.XOptions = ((Gtk.AttachOptions)(4));
            w12.YOptions = ((Gtk.AttachOptions)(4));
            this.vpaned2.Add(this.table1);
            Gtk.Paned.PanedChild w13 = ((Gtk.Paned.PanedChild)(this.vpaned2[this.table1]));
            w13.Resize = false;
            // Container child vpaned2.Gtk.Paned+PanedChild
            this.frame2 = new Gtk.Frame();
            this.frame2.Name = "frame2";
            this.frame2.ShadowType = ((Gtk.ShadowType)(0));
            this.frame2.LabelXalign = 0F;
            // Container child frame2.Gtk.Container+ContainerChild
            this.GtkAlignment = new Gtk.Alignment(0F, 0F, 1F, 1F);
            this.GtkAlignment.Name = "GtkAlignment";
            this.GtkAlignment.LeftPadding = ((uint)(12));
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            this.table5 = new Gtk.Table(((uint)(2)), ((uint)(2)), false);
            this.table5.Name = "table5";
            this.table5.RowSpacing = ((uint)(8));
            this.table5.ColumnSpacing = ((uint)(12));
            this.table5.BorderWidth = ((uint)(3));
            // Container child table5.Gtk.Table+TableChild
            this.label11 = new Gtk.Label();
            this.label11.Name = "label11";
            this.label11.Xalign = 0F;
            this.label11.LabelProp = Mono.Unix.Catalog.GetString("Extra Linker Options");
            this.table5.Add(this.label11);
            Gtk.Table.TableChild w14 = ((Gtk.Table.TableChild)(this.table5[this.label11]));
            w14.LeftAttach = ((uint)(1));
            w14.RightAttach = ((uint)(2));
            w14.XOptions = ((Gtk.AttachOptions)(4));
            w14.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table5.Gtk.Table+TableChild
            this.label7 = new Gtk.Label();
            this.label7.Name = "label7";
            this.label7.Xalign = 0F;
            this.label7.LabelProp = Mono.Unix.Catalog.GetString("Extra Compiler Options");
            this.table5.Add(this.label7);
            Gtk.Table.TableChild w15 = ((Gtk.Table.TableChild)(this.table5[this.label7]));
            w15.XOptions = ((Gtk.AttachOptions)(4));
            w15.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table5.Gtk.Table+TableChild
            this.scrolledwindow4 = new Gtk.ScrolledWindow();
            this.scrolledwindow4.CanFocus = true;
            this.scrolledwindow4.Name = "scrolledwindow4";
            this.scrolledwindow4.VscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow4.HscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow4.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow4.Gtk.Container+ContainerChild
            this.extraCompilerTextView = new Gtk.TextView();
            w1.SetTip(this.extraCompilerTextView, "A newline seperated list of extra options to send to the compiler.\nOne option can be in more than one line.\nExample:\n\t`pkg-config\n\t--cflags\n\tcairo`", "A newline seperated list of extra options to send to the compiler.\nOne option can be in more than one line.\nExample:\n\t`pkg-config\n\t--cflags\n\tcairo`");
            this.extraCompilerTextView.CanFocus = true;
            this.extraCompilerTextView.Name = "extraCompilerTextView";
            this.scrolledwindow4.Add(this.extraCompilerTextView);
            this.table5.Add(this.scrolledwindow4);
            Gtk.Table.TableChild w17 = ((Gtk.Table.TableChild)(this.table5[this.scrolledwindow4]));
            w17.TopAttach = ((uint)(1));
            w17.BottomAttach = ((uint)(2));
            // Container child table5.Gtk.Table+TableChild
            this.scrolledwindow5 = new Gtk.ScrolledWindow();
            this.scrolledwindow5.CanFocus = true;
            this.scrolledwindow5.Name = "scrolledwindow5";
            this.scrolledwindow5.VscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow5.HscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow5.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow5.Gtk.Container+ContainerChild
            this.extraLinkerTextView = new Gtk.TextView();
            w1.SetTip(this.extraLinkerTextView, "A newline seperated list of extra options to send to the linker.\nOne option can be in more than one line.\nExample:\n\t`pkg-config\n\t--libs\n\tcairo`", "A newline seperated list of extra options to send to the linker.\nOne option can be in more than one line.\nExample:\n\t`pkg-config\n\t--libs\n\tcairo`");
            this.extraLinkerTextView.CanFocus = true;
            this.extraLinkerTextView.Name = "extraLinkerTextView";
            this.scrolledwindow5.Add(this.extraLinkerTextView);
            this.table5.Add(this.scrolledwindow5);
            Gtk.Table.TableChild w19 = ((Gtk.Table.TableChild)(this.table5[this.scrolledwindow5]));
            w19.TopAttach = ((uint)(1));
            w19.BottomAttach = ((uint)(2));
            w19.LeftAttach = ((uint)(1));
            w19.RightAttach = ((uint)(2));
            w19.YOptions = ((Gtk.AttachOptions)(4));
            this.GtkAlignment.Add(this.table5);
            this.frame2.Add(this.GtkAlignment);
            this.GtkLabel12 = new Gtk.Label();
            this.GtkLabel12.Name = "GtkLabel12";
            this.GtkLabel12.LabelProp = Mono.Unix.Catalog.GetString("<b>Extra Options</b>");
            this.GtkLabel12.UseMarkup = true;
            this.frame2.LabelWidget = this.GtkLabel12;
            this.vpaned2.Add(this.frame2);
            this.notebook1.Add(this.vpaned2);
            Gtk.Notebook.NotebookChild w23 = ((Gtk.Notebook.NotebookChild)(this.notebook1[this.vpaned2]));
            w23.TabExpand = false;
            // Notebook tab
            this.label1 = new Gtk.Label();
            this.label1.Name = "label1";
            this.label1.LabelProp = Mono.Unix.Catalog.GetString("Code Generation");
            this.notebook1.SetTabLabel(this.vpaned2, this.label1);
            // Container child notebook1.Gtk.Notebook+NotebookChild
            this.table2 = new Gtk.Table(((uint)(2)), ((uint)(3)), false);
            this.table2.Name = "table2";
            this.table2.RowSpacing = ((uint)(10));
            this.table2.ColumnSpacing = ((uint)(10));
            this.table2.BorderWidth = ((uint)(3));
            // Container child table2.Gtk.Table+TableChild
            this.addLibButton = new Gtk.Button();
            this.addLibButton.CanFocus = true;
            this.addLibButton.Name = "addLibButton";
            this.addLibButton.UseUnderline = true;
            this.addLibButton.Label = Mono.Unix.Catalog.GetString("Add");
            this.table2.Add(this.addLibButton);
            Gtk.Table.TableChild w24 = ((Gtk.Table.TableChild)(this.table2[this.addLibButton]));
            w24.LeftAttach = ((uint)(2));
            w24.RightAttach = ((uint)(3));
            w24.XOptions = ((Gtk.AttachOptions)(4));
            w24.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.label8 = new Gtk.Label();
            this.label8.Name = "label8";
            this.label8.LabelProp = Mono.Unix.Catalog.GetString("Library:");
            this.table2.Add(this.label8);
            Gtk.Table.TableChild w25 = ((Gtk.Table.TableChild)(this.table2[this.label8]));
            w25.XOptions = ((Gtk.AttachOptions)(4));
            w25.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.libAddEntry = new Gtk.Entry();
            this.libAddEntry.CanFocus = true;
            this.libAddEntry.Name = "libAddEntry";
            this.libAddEntry.IsEditable = true;
            this.libAddEntry.InvisibleChar = '●';
            this.table2.Add(this.libAddEntry);
            Gtk.Table.TableChild w26 = ((Gtk.Table.TableChild)(this.table2[this.libAddEntry]));
            w26.LeftAttach = ((uint)(1));
            w26.RightAttach = ((uint)(2));
            w26.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.scrolledwindow1 = new Gtk.ScrolledWindow();
            this.scrolledwindow1.CanFocus = true;
            this.scrolledwindow1.Name = "scrolledwindow1";
            this.scrolledwindow1.VscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow1.HscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow1.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow1.Gtk.Container+ContainerChild
            this.libTreeView = new Gtk.TreeView();
            this.libTreeView.CanFocus = true;
            this.libTreeView.Name = "libTreeView";
            this.scrolledwindow1.Add(this.libTreeView);
            this.table2.Add(this.scrolledwindow1);
            Gtk.Table.TableChild w28 = ((Gtk.Table.TableChild)(this.table2[this.scrolledwindow1]));
            w28.TopAttach = ((uint)(1));
            w28.BottomAttach = ((uint)(2));
            w28.LeftAttach = ((uint)(1));
            w28.RightAttach = ((uint)(2));
            // Container child table2.Gtk.Table+TableChild
            this.vbox4 = new Gtk.VBox();
            this.vbox4.Name = "vbox4";
            this.vbox4.Spacing = 6;
            // Container child vbox4.Gtk.Box+BoxChild
            this.browseButton = new Gtk.Button();
            this.browseButton.CanFocus = true;
            this.browseButton.Name = "browseButton";
            this.browseButton.UseUnderline = true;
            this.browseButton.Label = Mono.Unix.Catalog.GetString("Browse...");
            this.vbox4.Add(this.browseButton);
            Gtk.Box.BoxChild w29 = ((Gtk.Box.BoxChild)(this.vbox4[this.browseButton]));
            w29.Position = 0;
            w29.Expand = false;
            w29.Fill = false;
            // Container child vbox4.Gtk.Box+BoxChild
            this.removeLibButton = new Gtk.Button();
            this.removeLibButton.CanFocus = true;
            this.removeLibButton.Name = "removeLibButton";
            this.removeLibButton.UseUnderline = true;
            this.removeLibButton.Label = Mono.Unix.Catalog.GetString("Remove");
            this.vbox4.Add(this.removeLibButton);
            Gtk.Box.BoxChild w30 = ((Gtk.Box.BoxChild)(this.vbox4[this.removeLibButton]));
            w30.Position = 1;
            w30.Expand = false;
            w30.Fill = false;
            this.table2.Add(this.vbox4);
            Gtk.Table.TableChild w31 = ((Gtk.Table.TableChild)(this.table2[this.vbox4]));
            w31.TopAttach = ((uint)(1));
            w31.BottomAttach = ((uint)(2));
            w31.LeftAttach = ((uint)(2));
            w31.RightAttach = ((uint)(3));
            w31.XOptions = ((Gtk.AttachOptions)(4));
            this.notebook1.Add(this.table2);
            Gtk.Notebook.NotebookChild w32 = ((Gtk.Notebook.NotebookChild)(this.notebook1[this.table2]));
            w32.Position = 1;
            w32.TabExpand = false;
            // Notebook tab
            this.label2 = new Gtk.Label();
            this.label2.Name = "label2";
            this.label2.LabelProp = Mono.Unix.Catalog.GetString("Libraries");
            this.notebook1.SetTabLabel(this.table2, this.label2);
            // Container child notebook1.Gtk.Notebook+NotebookChild
            this.vpaned1 = new Gtk.VPaned();
            this.vpaned1.CanFocus = true;
            this.vpaned1.Name = "vpaned1";
            this.vpaned1.Position = 172;
            this.vpaned1.BorderWidth = ((uint)(3));
            // Container child vpaned1.Gtk.Paned+PanedChild
            this.table3 = new Gtk.Table(((uint)(2)), ((uint)(3)), false);
            this.table3.Name = "table3";
            this.table3.RowSpacing = ((uint)(10));
            this.table3.ColumnSpacing = ((uint)(10));
            // Container child table3.Gtk.Table+TableChild
            this.includePathAddButton = new Gtk.Button();
            this.includePathAddButton.CanFocus = true;
            this.includePathAddButton.Name = "includePathAddButton";
            this.includePathAddButton.UseUnderline = true;
            this.includePathAddButton.Label = Mono.Unix.Catalog.GetString("Add");
            this.table3.Add(this.includePathAddButton);
            Gtk.Table.TableChild w33 = ((Gtk.Table.TableChild)(this.table3[this.includePathAddButton]));
            w33.LeftAttach = ((uint)(2));
            w33.RightAttach = ((uint)(3));
            w33.XOptions = ((Gtk.AttachOptions)(4));
            w33.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table3.Gtk.Table+TableChild
            this.includePathEntry = new Gtk.Entry();
            this.includePathEntry.CanFocus = true;
            this.includePathEntry.Name = "includePathEntry";
            this.includePathEntry.IsEditable = true;
            this.includePathEntry.InvisibleChar = '●';
            this.table3.Add(this.includePathEntry);
            Gtk.Table.TableChild w34 = ((Gtk.Table.TableChild)(this.table3[this.includePathEntry]));
            w34.LeftAttach = ((uint)(1));
            w34.RightAttach = ((uint)(2));
            w34.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table3.Gtk.Table+TableChild
            this.label9 = new Gtk.Label();
            this.label9.Name = "label9";
            this.label9.LabelProp = Mono.Unix.Catalog.GetString("Include:");
            this.table3.Add(this.label9);
            Gtk.Table.TableChild w35 = ((Gtk.Table.TableChild)(this.table3[this.label9]));
            w35.XOptions = ((Gtk.AttachOptions)(4));
            w35.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table3.Gtk.Table+TableChild
            this.scrolledwindow2 = new Gtk.ScrolledWindow();
            this.scrolledwindow2.CanFocus = true;
            this.scrolledwindow2.Name = "scrolledwindow2";
            this.scrolledwindow2.VscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow2.HscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow2.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow2.Gtk.Container+ContainerChild
            this.includePathTreeView = new Gtk.TreeView();
            this.includePathTreeView.CanFocus = true;
            this.includePathTreeView.Name = "includePathTreeView";
            this.scrolledwindow2.Add(this.includePathTreeView);
            this.table3.Add(this.scrolledwindow2);
            Gtk.Table.TableChild w37 = ((Gtk.Table.TableChild)(this.table3[this.scrolledwindow2]));
            w37.TopAttach = ((uint)(1));
            w37.BottomAttach = ((uint)(2));
            w37.LeftAttach = ((uint)(1));
            w37.RightAttach = ((uint)(2));
            // Container child table3.Gtk.Table+TableChild
            this.vbox5 = new Gtk.VBox();
            this.vbox5.Name = "vbox5";
            this.vbox5.Spacing = 6;
            // Container child vbox5.Gtk.Box+BoxChild
            this.includePathBrowseButton = new Gtk.Button();
            this.includePathBrowseButton.CanFocus = true;
            this.includePathBrowseButton.Name = "includePathBrowseButton";
            this.includePathBrowseButton.UseUnderline = true;
            this.includePathBrowseButton.Label = Mono.Unix.Catalog.GetString("Browse...");
            this.vbox5.Add(this.includePathBrowseButton);
            Gtk.Box.BoxChild w38 = ((Gtk.Box.BoxChild)(this.vbox5[this.includePathBrowseButton]));
            w38.Position = 0;
            w38.Expand = false;
            w38.Fill = false;
            // Container child vbox5.Gtk.Box+BoxChild
            this.includePathRemoveButton = new Gtk.Button();
            this.includePathRemoveButton.CanFocus = true;
            this.includePathRemoveButton.Name = "includePathRemoveButton";
            this.includePathRemoveButton.UseUnderline = true;
            this.includePathRemoveButton.Label = Mono.Unix.Catalog.GetString("Remove");
            this.vbox5.Add(this.includePathRemoveButton);
            Gtk.Box.BoxChild w39 = ((Gtk.Box.BoxChild)(this.vbox5[this.includePathRemoveButton]));
            w39.Position = 1;
            w39.Expand = false;
            w39.Fill = false;
            this.table3.Add(this.vbox5);
            Gtk.Table.TableChild w40 = ((Gtk.Table.TableChild)(this.table3[this.vbox5]));
            w40.TopAttach = ((uint)(1));
            w40.BottomAttach = ((uint)(2));
            w40.LeftAttach = ((uint)(2));
            w40.RightAttach = ((uint)(3));
            w40.XOptions = ((Gtk.AttachOptions)(4));
            this.vpaned1.Add(this.table3);
            Gtk.Paned.PanedChild w41 = ((Gtk.Paned.PanedChild)(this.vpaned1[this.table3]));
            w41.Resize = false;
            // Container child vpaned1.Gtk.Paned+PanedChild
            this.table4 = new Gtk.Table(((uint)(2)), ((uint)(3)), false);
            this.table4.Name = "table4";
            this.table4.RowSpacing = ((uint)(10));
            this.table4.ColumnSpacing = ((uint)(10));
            // Container child table4.Gtk.Table+TableChild
            this.label10 = new Gtk.Label();
            this.label10.Name = "label10";
            this.label10.LabelProp = Mono.Unix.Catalog.GetString("Library:");
            this.table4.Add(this.label10);
            Gtk.Table.TableChild w42 = ((Gtk.Table.TableChild)(this.table4[this.label10]));
            w42.XOptions = ((Gtk.AttachOptions)(4));
            w42.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table4.Gtk.Table+TableChild
            this.libPathAddButton = new Gtk.Button();
            this.libPathAddButton.CanFocus = true;
            this.libPathAddButton.Name = "libPathAddButton";
            this.libPathAddButton.UseUnderline = true;
            this.libPathAddButton.Label = Mono.Unix.Catalog.GetString("Add");
            this.table4.Add(this.libPathAddButton);
            Gtk.Table.TableChild w43 = ((Gtk.Table.TableChild)(this.table4[this.libPathAddButton]));
            w43.LeftAttach = ((uint)(2));
            w43.RightAttach = ((uint)(3));
            w43.XOptions = ((Gtk.AttachOptions)(4));
            w43.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table4.Gtk.Table+TableChild
            this.libPathEntry = new Gtk.Entry();
            this.libPathEntry.CanFocus = true;
            this.libPathEntry.Name = "libPathEntry";
            this.libPathEntry.IsEditable = true;
            this.libPathEntry.InvisibleChar = '●';
            this.table4.Add(this.libPathEntry);
            Gtk.Table.TableChild w44 = ((Gtk.Table.TableChild)(this.table4[this.libPathEntry]));
            w44.LeftAttach = ((uint)(1));
            w44.RightAttach = ((uint)(2));
            w44.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table4.Gtk.Table+TableChild
            this.scrolledwindow3 = new Gtk.ScrolledWindow();
            this.scrolledwindow3.CanFocus = true;
            this.scrolledwindow3.Name = "scrolledwindow3";
            this.scrolledwindow3.VscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow3.HscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow3.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow3.Gtk.Container+ContainerChild
            this.libPathTreeView = new Gtk.TreeView();
            this.libPathTreeView.CanFocus = true;
            this.libPathTreeView.Name = "libPathTreeView";
            this.scrolledwindow3.Add(this.libPathTreeView);
            this.table4.Add(this.scrolledwindow3);
            Gtk.Table.TableChild w46 = ((Gtk.Table.TableChild)(this.table4[this.scrolledwindow3]));
            w46.TopAttach = ((uint)(1));
            w46.BottomAttach = ((uint)(2));
            w46.LeftAttach = ((uint)(1));
            w46.RightAttach = ((uint)(2));
            // Container child table4.Gtk.Table+TableChild
            this.vbox3 = new Gtk.VBox();
            this.vbox3.Name = "vbox3";
            this.vbox3.Spacing = 6;
            // Container child vbox3.Gtk.Box+BoxChild
            this.libPathBrowseButton = new Gtk.Button();
            this.libPathBrowseButton.CanFocus = true;
            this.libPathBrowseButton.Name = "libPathBrowseButton";
            this.libPathBrowseButton.UseUnderline = true;
            this.libPathBrowseButton.Label = Mono.Unix.Catalog.GetString("Browse...");
            this.vbox3.Add(this.libPathBrowseButton);
            Gtk.Box.BoxChild w47 = ((Gtk.Box.BoxChild)(this.vbox3[this.libPathBrowseButton]));
            w47.Position = 0;
            w47.Expand = false;
            w47.Fill = false;
            // Container child vbox3.Gtk.Box+BoxChild
            this.libPathRemoveButton = new Gtk.Button();
            this.libPathRemoveButton.CanFocus = true;
            this.libPathRemoveButton.Name = "libPathRemoveButton";
            this.libPathRemoveButton.UseUnderline = true;
            this.libPathRemoveButton.Label = Mono.Unix.Catalog.GetString("Remove");
            this.vbox3.Add(this.libPathRemoveButton);
            Gtk.Box.BoxChild w48 = ((Gtk.Box.BoxChild)(this.vbox3[this.libPathRemoveButton]));
            w48.Position = 1;
            w48.Expand = false;
            w48.Fill = false;
            this.table4.Add(this.vbox3);
            Gtk.Table.TableChild w49 = ((Gtk.Table.TableChild)(this.table4[this.vbox3]));
            w49.TopAttach = ((uint)(1));
            w49.BottomAttach = ((uint)(2));
            w49.LeftAttach = ((uint)(2));
            w49.RightAttach = ((uint)(3));
            w49.XOptions = ((Gtk.AttachOptions)(4));
            this.vpaned1.Add(this.table4);
            this.notebook1.Add(this.vpaned1);
            Gtk.Notebook.NotebookChild w51 = ((Gtk.Notebook.NotebookChild)(this.notebook1[this.vpaned1]));
            w51.Position = 2;
            w51.TabExpand = false;
            // Notebook tab
            this.label3 = new Gtk.Label();
            this.label3.Name = "label3";
            this.label3.LabelProp = Mono.Unix.Catalog.GetString("Paths");
            this.notebook1.SetTabLabel(this.vpaned1, this.label3);
            this.Add(this.notebook1);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Show();
        }
    }
}
