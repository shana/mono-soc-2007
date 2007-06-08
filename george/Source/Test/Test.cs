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

				ScrollViewer v = new ScrollViewer();
				StackPanel s = new StackPanel();
				for (int i = 1; i <= 10; i++)
					s.Children.Add(new TestButton(i));
				v.Content = s;
				//v.CanContentScroll = true;
				Content = v;
			}

			class TestButton : global::System.Windows.Controls.Button {
				public TestButton(int i) {
					Content = "Test" + i;
				}
			}
		}
	}
}