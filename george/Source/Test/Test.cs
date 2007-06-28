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
#if !Implementation
			Utility.LoadLunaTheme();
#endif
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

				Button l = new Button();
				l.Content = "_Test";
				Content = l;
			}
		}
	}
}