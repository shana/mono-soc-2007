using System;
using System.Windows;
namespace Mono.WindowsPresentationFoundation {
	static class WindowsPresentationFoundationApplication {
		[STAThread]
		static void Main() {
			new Application().Run(new ColorFinder.ColorFinderWindow());
		}
	}
}