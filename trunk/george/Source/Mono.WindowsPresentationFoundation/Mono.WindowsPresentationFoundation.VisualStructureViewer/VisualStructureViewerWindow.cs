using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Text;
namespace Mono.WindowsPresentationFoundation.VisualStructureViewer {
	class VisualStructureViewerWindow : Window {
		public VisualStructureViewerWindow() {
			Title = "Visual structure viewer";

			TextBox xaml_text_box = new TextBox();
			xaml_text_box.Text = @"<Page xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""></Page>";
			xaml_text_box.TextWrapping = TextWrapping.Wrap;
			Label xaml_label = new Label("_XAML", xaml_text_box);
			DockPanel xaml_panel = new DockPanel();
			DockPanel.SetDock(xaml_label, Dock.Top);
			xaml_panel.Children.Add(xaml_label);
			xaml_panel.Children.Add(xaml_text_box);

			System.Windows.Forms.Integration.WindowsFormsHost.EnableWindowsFormsInterop();

			System.Windows.Forms.PropertyGrid structure_viewer_property_grid = new System.Windows.Forms.PropertyGrid();
			System.Windows.Forms.Integration.WindowsFormsHost structure_viewer_host = new System.Windows.Forms.Integration.WindowsFormsHost();
			structure_viewer_host.Child = structure_viewer_property_grid;
			Label structure_viewer_label = new Label("_Structure", structure_viewer_host);
			StackPanel structure_viewer_panel = new StackPanel();
			structure_viewer_panel.Children.Add(structure_viewer_label);
			structure_viewer_panel.Children.Add(structure_viewer_host);

			Button view_button = new Button();
			view_button.Content = "_View";
			view_button.Click += delegate(object sender, RoutedEventArgs e) {
				try {
					Page page = (Page)XamlReader.Load(new MemoryStream(Encoding.UTF8.GetBytes(xaml_text_box.Text)));
					structure_viewer_property_grid.SelectedObject = VisualTreeHelper.GetDrawing(page);
				} catch (Exception ex) {
					MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			};

			Grid contents = new Grid();
			contents.RowDefinitions.Add(new RowDefinition());
			RowDefinition row2 = new RowDefinition();
			row2.Height = GridLength.Auto;
			contents.RowDefinitions.Add(row2);
			contents.ColumnDefinitions.Add(new ColumnDefinition());
			contents.ColumnDefinitions.Add(new ColumnDefinition());
			contents.Children.Add(xaml_panel);
			Grid.SetColumn(structure_viewer_panel, 1);
			contents.Children.Add(structure_viewer_panel);
			Grid.SetRow(view_button, 1);
			contents.Children.Add(view_button);
			Content = contents;
		}
	}
}