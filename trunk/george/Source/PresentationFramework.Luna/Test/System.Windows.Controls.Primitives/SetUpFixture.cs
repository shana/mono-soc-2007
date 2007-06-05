using NUnit.Framework;
#if Implementation
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
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
