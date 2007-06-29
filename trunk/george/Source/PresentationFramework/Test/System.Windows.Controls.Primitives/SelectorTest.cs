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
	public class SelectorTest {
		[Test]
		public void SelectedItemSetsSelectedIndex() {
			TestSelector s = new TestSelector();
			object o = new object();
			s.Items.Add(o);
			Assert.AreEqual(s.SelectedIndex, -1, "1");
			s.SelectedItem = o;
			Assert.AreEqual(s.SelectedIndex, 0, "2");
		}

		[Test]
		public void SelectedIndexSetsSelectedItem() {
			TestSelector s = new TestSelector();
			object o = new object();
			s.Items.Add(o);
			Assert.IsNull(s.SelectedItem, "1");
			s.SelectedIndex = 0;
			Assert.AreEqual(s.SelectedItem, o, "2");
		}

		[Test]
		public void InVisibleWindowAddingItemsSetsSelection() {
			TestSelector s = new TestSelector();
			Window w = new Window();
			w.Content = s;
			w.Show();
			s.Items.Add(new object());
			Assert.AreEqual(s.SelectedIndex, -1, "1");
			Assert.IsNull(s.SelectedItem, "2");
		}

		[Test]
		public void AddingToAVisibleWindowSetsSelection() {
			TestSelector s = new TestSelector();
			s.Items.Add(new object());
			Window w = new Window();
			w.Content = s;
			w.Show();
			Assert.AreEqual(s.SelectedIndex, -1, "1");
			Assert.IsNull(s.SelectedItem, "2");
		}

		#region OnSelectionChanged
		[Test]
		public void OnSelectionChanged() {
			new OnSelectionChangedSelector();
		}
		
		class OnSelectionChangedSelector : Selector {
			int calls;

			public OnSelectionChangedSelector() {
				Items.Add(new object());
				Assert.AreEqual(calls, 0, "1");
				SelectedIndex = 0;
				Assert.AreEqual(calls, 1, "2");
			}

			protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
				calls++;
				base.OnSelectionChanged(e);
			}
		}
		#endregion

		class TestSelector : Selector {
		}
	}
}