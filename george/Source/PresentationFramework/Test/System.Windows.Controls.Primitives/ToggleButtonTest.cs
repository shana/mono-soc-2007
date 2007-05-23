using NUnit.Framework;
#if Implementation
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	[TestFixture]
	public class ToggleButtonTest {
		[Test]
		public void StaticProperties() {
			Assert.AreEqual(ToggleButton.IsCheckedProperty.Name, "IsChecked", "IsCheckedProperty.Name");
			Assert.AreEqual(ToggleButton.IsCheckedProperty.OwnerType, typeof(ToggleButton), "IsCheckedProperty.OwnerType");
			Assert.AreEqual(ToggleButton.IsCheckedProperty.PropertyType, typeof(bool?), "IsCheckedProperty.PropertyType");
			Assert.AreEqual(ToggleButton.IsCheckedProperty.DefaultMetadata.DefaultValue, false, "IsCheckedProperty.DefaultMetadata.DefaultValue");

			Assert.AreEqual(ToggleButton.IsThreeStateProperty.Name, "IsThreeState", "IsThreeStateProperty.Name");
			Assert.AreEqual(ToggleButton.IsThreeStateProperty.OwnerType, typeof(ToggleButton), "IsThreeStateProperty.OwnerType");
			Assert.AreEqual(ToggleButton.IsThreeStateProperty.PropertyType, typeof(bool), "IsThreeStateProperty.PropertyType");
			Assert.AreEqual(ToggleButton.IsThreeStateProperty.DefaultMetadata.DefaultValue, false, "IsThreeStateProperty.DefaultMetadata.DefaultValue");
        }

        [Test]
        public void Creation() {
            ToggleButton toggle_button = new ToggleButton();
            Assert.AreEqual(toggle_button.IsChecked, false, "IsChecked");
            Assert.AreEqual(toggle_button.IsThreeState, false, "IsThreeState");
        }

        #region OnToggle
        [Test]
        public void OnToggle() {
            new OnToggleToggleButton();
        }

        class OnToggleToggleButton : ToggleButton {
            public OnToggleToggleButton() {
                OnToggle();
                Assert.AreEqual(IsChecked, true, "1");
                OnToggle();
                Assert.AreEqual(IsChecked, false, "2");
                IsThreeState = true;
                OnToggle();
                Assert.AreEqual(IsChecked, true, "3");
                OnToggle();
                Assert.AreEqual(IsChecked, null, "4");
            }
        }
        #endregion

        #region CheckedEvents
        [Test]
        public void CheckedEvents() {
            new CheckedEventsToggleButton();
        }

        class CheckedEventsToggleButton : ToggleButton {
            bool raised;

            public CheckedEventsToggleButton() {
                IsChecked = true;
                Assert.IsTrue(raised, "1");

                raised = false;
                IsChecked = true;
                Assert.IsFalse(raised, "2");

                raised = false;
                IsChecked = false;
                Assert.IsTrue(raised, "3");

                raised = false;
                IsChecked = null;
                Assert.IsTrue(raised, "4");
            }
            
            protected override void OnIndeterminate(RoutedEventArgs e) {
                base.OnIndeterminate(e);
                raised = true;
            }
            
            protected override void OnChecked(RoutedEventArgs e) {
                base.OnChecked(e);
                raised = true;
            }

            protected override void OnUnchecked(RoutedEventArgs e) {
                base.OnUnchecked(e);
                raised = true;
            }
        }
        #endregion
    }
}