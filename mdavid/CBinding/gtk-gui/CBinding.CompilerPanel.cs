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
    
    
    public partial class CompilerPanel {
        
        private Gtk.Table table2;
        
        private Gtk.ComboBox compilerComboBox;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize();
            // Widget CBinding.CompilerPanel
            Stetic.BinContainer.Attach(this);
            this.Name = "CBinding.CompilerPanel";
            // Container child CBinding.CompilerPanel.Gtk.Container+ContainerChild
            this.table2 = new Gtk.Table(((uint)(3)), ((uint)(3)), false);
            this.table2.Name = "table2";
            this.table2.RowSpacing = ((uint)(6));
            this.table2.ColumnSpacing = ((uint)(6));
            // Container child table2.Gtk.Table+TableChild
            this.compilerComboBox = Gtk.ComboBox.NewText();
            this.compilerComboBox.Name = "compilerComboBox";
            this.table2.Add(this.compilerComboBox);
            Gtk.Table.TableChild w1 = ((Gtk.Table.TableChild)(this.table2[this.compilerComboBox]));
            w1.TopAttach = ((uint)(1));
            w1.BottomAttach = ((uint)(2));
            w1.LeftAttach = ((uint)(1));
            w1.RightAttach = ((uint)(2));
            w1.XOptions = ((Gtk.AttachOptions)(4));
            w1.YOptions = ((Gtk.AttachOptions)(4));
            this.Add(this.table2);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Show();
        }
    }
}
