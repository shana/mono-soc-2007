using System.Windows;
namespace Mono.WindowsPresentationFoundation
{
	class Label : System.Windows.Controls.Label
	{
		public Label (string labelText, UIElement target)
		{
			Content = labelText;
			Target = target;
			Padding = new Thickness (1);
		}
	}
}