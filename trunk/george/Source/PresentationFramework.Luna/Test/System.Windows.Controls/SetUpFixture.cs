using NUnit.Framework;
#if Implementation
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[SetUpFixture]
	public class SetUpFixture {
		[SetUp]
		public void SetUp() {
			if (Application.Current == null)
				new Application();
			Mono.WindowsPresentationFoundation.Utility.LoadLunaTheme();
		}
	}
}
