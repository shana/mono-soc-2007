using NUnit.Framework;
namespace System.Windows.Controls {
	[TestFixture]
	public class UIElementTest {
		#region AutomationPeerUIElement
		[Test]
		public void AutomationPeer() {
			new AutomationPeerUIElement();
		}

		class AutomationPeerUIElement : UIElement {
			public AutomationPeerUIElement() {
				Assert.IsNull(OnCreateAutomationPeer());
			}
		}
		#endregion
	}
}