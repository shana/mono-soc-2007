//#define PropertyGrid
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Text;
namespace Mono.WindowsPresentationFoundation.VisualStructureViewer {
	class VisualStructureViewerWindow : Window {
		string xaml_file_name = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, "XAML.xaml");
		TextBox xaml_text_box = new TextBox();

		public VisualStructureViewerWindow() {
			Title = "Visual structure viewer";

			if (File.Exists(xaml_file_name))
				xaml_text_box.Text = new StreamReader(xaml_file_name).ReadToEnd();
			else
				xaml_text_box.Text = @"<theme:ScrollChrome xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:theme=""clr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Luna"" theme:ScrollGlyph=""DownArrow"" Width=""100"" Height=""100""/>";
			xaml_text_box.TextWrapping = TextWrapping.Wrap;
			Label xaml_label = new Label("_XAML", xaml_text_box);
			DockPanel xaml_panel = new DockPanel();
			DockPanel.SetDock(xaml_label, Dock.Top);
			xaml_panel.Children.Add(xaml_label);
			xaml_panel.Children.Add(xaml_text_box);

#if PropertyGrid
			System.Windows.Forms.PropertyGrid structure_viewer_property_grid = new System.Windows.Forms.PropertyGrid();
			structure_viewer_property_grid.Dock = System.Windows.Forms.DockStyle.Fill;
			System.Windows.Forms.Integration.WindowsFormsHost structure_viewer_host = new System.Windows.Forms.Integration.WindowsFormsHost();
			structure_viewer_host.Child = structure_viewer_property_grid;
			Label structure_viewer_label = new Label("_Structure", structure_viewer_host);
			StackPanel structure_viewer_panel = new StackPanel();
			structure_viewer_panel.Children.Add(structure_viewer_label);
			structure_viewer_panel.Children.Add(structure_viewer_host);
#else
			ObjectViewer structure_viewer = new ObjectViewer();
			Label structure_viewer_label = new Label("_Structure", structure_viewer);
			DockPanel structure_viewer_panel = new DockPanel();
			DockPanel.SetDock(structure_viewer_label, Dock.Top);
			structure_viewer_panel.Children.Add(structure_viewer_label);
			structure_viewer_panel.Children.Add(structure_viewer);
#endif

			Button view_button = new Button();
			view_button.Content = "_View";
			view_button.Click += delegate(object sender, RoutedEventArgs e) {
				try {
					Visual content = (Visual)XamlReader.Load(new MemoryStream(Encoding.UTF8.GetBytes(xaml_text_box.Text)));
					Window window = new Window();
					window.Content = content;
					window.Title = "Content";
					window.Show();
#if PropertyGrid
					structure_viewer_property_grid.SelectedObject = VisualTreeHelper.GetDrawing(content);
#else
					structure_viewer.Object = VisualTreeHelper.GetDrawing(content);
#endif
				} catch (Exception ex) {
					MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			};

			Grid contents = new Grid();
			contents.RowDefinitions.Add(new RowDefinition());
			RowDefinition view_row = new RowDefinition();
			view_row.Height = GridLength.Auto;
			contents.RowDefinitions.Add(view_row);
			contents.ColumnDefinitions.Add(new ColumnDefinition());
			contents.ColumnDefinitions.Add(new ColumnDefinition());
			contents.Children.Add(xaml_panel);
			Grid.SetColumn(structure_viewer_panel, 1);
			Grid.SetRowSpan(structure_viewer_panel, 2);
			contents.Children.Add(structure_viewer_panel);
			Grid.SetRow(view_button, 1);
			contents.Children.Add(view_button);
			GridSplitter splitter = new GridSplitter();
			splitter.HorizontalAlignment = HorizontalAlignment.Right;
			splitter.VerticalAlignment = VerticalAlignment.Stretch;
			splitter.Width = 10;
			contents.Children.Add(splitter);
			Content = contents;
		}

		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			StreamWriter stream_writer = new StreamWriter(xaml_file_name);
			stream_writer.Write(xaml_text_box.Text);
			stream_writer.Close();
		}
	}
}