using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System;
namespace Mono.WindowsPresentationFoundation.ColorFinder {
	class ColorFinderWindow : Window {
		const double ElementSize = 60;
		const double ElementMargin = 2;

		public ColorFinderWindow() {
			Title = "Color finder";

			StackPanel argb_panel = new StackPanel();
			argb_panel.Orientation = Orientation.Horizontal;
			ArgbInputBox argb_input_box = new ArgbInputBox();
			Button find_button = new Button();
			find_button.Content = "_Find";
			find_button.ToolTip = "Searches the Colors and SystemColors classes for members that have the specified ARGB values. The first item found is copied to clipboard.";
			find_button.Margin = new Thickness(ElementMargin);
			find_button.Width = ElementSize;
			argb_panel.Children.Add(argb_input_box);
			argb_panel.Children.Add(find_button);

			ListBox results_list_box = new ListBox();

			StackPanel content = new StackPanel();
			content.Children.Add(argb_panel);
			content.Children.Add(new Label("R_esults (double clicking copies the selected item to clipboard)", results_list_box));
			content.Children.Add(results_list_box);

			ScrollViewer scroll_viewer = new ScrollViewer();
			scroll_viewer.Content = content;

			Content = scroll_viewer;

			find_button.Click += delegate(object sender, RoutedEventArgs e) {
				results_list_box.Items.Clear();
				byte a, r, g, b;
				try {
					a = argb_input_box.A;
					r = argb_input_box.R;
					g = argb_input_box.G;
					b = argb_input_box.B;
				} catch (FormatException) {
					MessageBox.Show(this, "One of the values is in the wrong format.", Title, MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				foreach (PropertyInfo property in typeof(Colors).GetProperties()) {
					Color color = (Color)property.GetValue(null, null);
					if (color.A == a && color.R == r && color.G == g && color.B == b)
						results_list_box.Items.Add("Colors." + property.Name);
				}
				foreach (PropertyInfo property in typeof(SystemColors).GetProperties()) {
					if (property.PropertyType == typeof(Color)) {
						Color color = (Color)property.GetValue(null, null);
						if (color.A == a && color.R == r && color.G == g && color.B == b)
							results_list_box.Items.Add("SystemColors." + property.Name);
					}
				}
				if (results_list_box.Items.Count != 0)
					Clipboard.SetText((string)results_list_box.Items[0]);
			};

			results_list_box.MouseDoubleClick += delegate(object sender, MouseButtonEventArgs e) {
				if (results_list_box.SelectedItem != null)
					Clipboard.SetText((string)results_list_box.SelectedItem);
			};
		}

		class ArgbInputBox : StackPanel {
			ByteInputBox alpha_box = new ByteInputBox("_Alpha");
			ByteInputBox red_box = new ByteInputBox("_Red");
			ByteInputBox green_box = new ByteInputBox("_Green");
			ByteInputBox blue_box = new ByteInputBox("_Blue");

			public ArgbInputBox() {
				Orientation = Orientation.Horizontal;
				Children.Add(alpha_box);
				Children.Add(red_box);
				Children.Add(green_box);
				Children.Add(blue_box);
			}

			public byte A {
				get { return alpha_box.Value; }
			}

			public byte R {
				get { return red_box.Value; }
			}
			
			public byte G {
				get { return green_box.Value; }
			}
			
			public byte B {
				get { return blue_box.Value; }
			}
			
			class ByteInputBox : StackPanel {
				TextBox text_box = new TextBox();

				public ByteInputBox(string labelText) {
					Margin = new Thickness(ElementMargin);
					Width = ElementSize;

					text_box.HorizontalContentAlignment = HorizontalAlignment.Right;
					text_box.Text = "FF";

					Children.Add(new Label(labelText, text_box));

					Children.Add(text_box);
				}

				public byte Value {
					get {
						return byte.Parse(text_box.Text, NumberStyles.HexNumber);
					}
				}
			}
		}

		class Label : System.Windows.Controls.Label {
			public Label(string labelText, UIElement target) {
				Content = labelText;
				Target = target;
				Padding = new Thickness(1);
			}
		}
	}
}