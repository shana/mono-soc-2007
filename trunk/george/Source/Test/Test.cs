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
			/////////////////////////////////////
			
			//return;

			/////////////////////////////////////
			new Application().Run(new TestWindow());
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
				Canvas container = new Canvas();
				Canvas canvas = new Canvas();
				container.Children.Add(canvas);
				Button b1 = new Button();
				b1.Content = "111111";
				Canvas.SetLeft(b1, 11);
				canvas.Children.Add(b1);
				Canvas.SetZIndex(b1, 1);

				Button b2 = new Button();
				b2.Content = "222222";
				Canvas.SetLeft(b2, 22);
				canvas.Children.Add(b2);
				
				
				container.Background = Brushes.Green;
				canvas.ClipToBounds = true;
				canvas.Background = Brushes.Red;
				Content = container;
			}
		}
	}
}