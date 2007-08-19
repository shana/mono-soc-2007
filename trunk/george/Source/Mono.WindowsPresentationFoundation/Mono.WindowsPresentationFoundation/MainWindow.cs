using System.Windows;
using System.Windows.Controls;
namespace Mono.WindowsPresentationFoundation
{
	class MainWindow : Window
	{
		public MainWindow ()
		{
			Title = "Mono Windows Presentation Foundation utility";

			MenuItem color_finder_menu = new MenuItem ();
			color_finder_menu.Header = "_Color finder";
			color_finder_menu.Click += delegate (object sender, RoutedEventArgs e)
			{
				new ColorFinder.ColorFinderWindow ().Show ();
			};

			MenuItem visual_structure_viewer_menu = new MenuItem ();
			visual_structure_viewer_menu.Header = "_Visual structure viewer";
			visual_structure_viewer_menu.Click += delegate (object sender, RoutedEventArgs e)
			{
				new VisualStructureViewer.VisualStructureViewerWindow ().Show ();
			};

			MenuItem utilities_menu = new MenuItem ();
			utilities_menu.Header = "_Utilities";
			utilities_menu.Items.Add (color_finder_menu);
			utilities_menu.Items.Add (visual_structure_viewer_menu);

			Menu menu = new Menu ();
			menu.Items.Add (utilities_menu);

			DockPanel contents = new DockPanel ();
			contents.LastChildFill = false;
			DockPanel.SetDock (menu, Dock.Top);
			contents.Children.Add (menu);

			Content = contents;
		}


	}
}