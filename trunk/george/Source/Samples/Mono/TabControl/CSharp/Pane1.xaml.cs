using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace TabControl
{
    /// <summary>
    /// Interaction logic for Pane1.xaml
    /// </summary>

    public partial class Pane1 : StackPanel
    {

        Mono.System.Windows.Controls.TabControl tabcon;
		Mono.System.Windows.Controls.TabItem ti1, ti2, ti3;

        void OnClick(object sender, RoutedEventArgs e)
        {
			tabcon = new Mono.System.Windows.Controls.TabControl();
			ti1 = new Mono.System.Windows.Controls.TabItem();
            ti1.Header = "Background";
            tabcon.Items.Add(ti1);
			ti2 = new Mono.System.Windows.Controls.TabItem();
            ti2.Header = "Foreground";
            tabcon.Items.Add(ti2);
			ti3 = new Mono.System.Windows.Controls.TabItem();
            ti3.Header = "FontFamily";
            tabcon.Items.Add(ti3);

            cv2.Children.Add(tabcon);
        }
    }
}