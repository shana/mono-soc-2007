using Mono.WindowsPresentationFoundation;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#if Implementation
using Mono.Microsoft.Windows.Themes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives {
#else
using Microsoft.Windows.Themes;
namespace System.Windows.Controls.Primitives {
#endif
	class Test {
		[STAThread]
		static void Main() {
			new Application();
			Utility.LoadLunaTheme();
			Application.Current.Run(new TestWindow());
		}
		class TestWindow : Window {
			public TestWindow() {
#if Implementation
				Title = "Mono";
#else
				Title = "Microsoft";
#endif
				Width = 200;
				Height = 100;
				Canvas c = new Canvas();
				DockPanel d = new DockPanel();
				c.Children.Add(d);
				Button b = new Button();
				b.Content = "Test";
				d.Children.Add(b);
				Content = c;
			}
		}
	}
}