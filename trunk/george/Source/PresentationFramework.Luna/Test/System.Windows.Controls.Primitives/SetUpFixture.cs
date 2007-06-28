#if !Implementation
using NUnit.Framework;
namespace System.Windows.Controls.Primitives {
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
#endif