using NUnit.Framework;
#if Implementation
using Mono.System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls
{
#else
using System.Windows.Controls.Primitives;
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class ItemContainerGeneratorTest
	{
		[Test]
		public void Status ()
		{
			ItemContainerGenerator g = new ItemsControl ().ItemContainerGenerator;
			Assert.AreEqual (g.Status, GeneratorStatus.NotStarted);
		}
	}
}