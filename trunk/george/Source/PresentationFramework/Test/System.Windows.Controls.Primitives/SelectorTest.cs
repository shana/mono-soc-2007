using NUnit.Framework;
using System.Reflection;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls.Primitives
{
#else
namespace System.Windows.Controls.Primitives {
#endif
	[TestFixture]
#if Implementation
	[Ignore ("Not implementable now")]
#endif
	public class SelectorTest
	{
		[Test]
		public void SelectedItemSetsSelectedIndex ()
		{
			TestSelector s = new TestSelector ();
			object o = new object ();
			s.Items.Add (o);
			Assert.AreEqual (s.SelectedIndex, -1, "1");
			s.SelectedItem = o;
			Assert.AreEqual (s.SelectedIndex, 0, "2");
		}

		[Test]
		public void SelectedIndexSetsSelectedItem ()
		{
			TestSelector s = new TestSelector ();
			object o = new object ();
			s.Items.Add (o);
			Assert.IsNull (s.SelectedItem, "1");
			s.SelectedIndex = 0;
			Assert.AreEqual (s.SelectedItem, o, "2");
		}

		[Test]
		public void InVisibleWindowAddingItemsSetsSelection ()
		{
			TestSelector s = new TestSelector ();
			Window w = new Window ();
			w.Content = s;
			w.Show ();
			s.Items.Add (new object ());
			Assert.AreEqual (s.SelectedIndex, -1, "1");
			Assert.IsNull (s.SelectedItem, "2");
		}

		[Test]
		public void AddingToAVisibleWindowSetsSelection ()
		{
			TestSelector s = new TestSelector ();
			s.Items.Add (new object ());
			Window w = new Window ();
			w.Content = s;
			w.Show ();
			Assert.AreEqual (s.SelectedIndex, -1, "1");
			Assert.IsNull (s.SelectedItem, "2");
		}

		#region OnSelectionChanged
		[Test]
		public void OnSelectionChanged ()
		{
			new OnSelectionChangedSelector ();
		}

		class OnSelectionChangedSelector : Selector
		{
			int calls;
			object o = new object ();

			public OnSelectionChangedSelector ()
			{
				Items.Add (o);
				Assert.AreEqual (calls, 0, "1");
				SelectedIndex = 0;
				Assert.AreEqual (calls, 1, "2");
			}

			protected override void OnSelectionChanged (SelectionChangedEventArgs e)
			{
				calls++;
				base.OnSelectionChanged (e);
				Assert.AreEqual (e.AddedItems.Count, 1, "3");
				Assert.AreEqual (e.AddedItems [0], o, "4");
				Assert.AreEqual (e.RemovedItems.Count, 0, "5");
			}
		}
		#endregion

		#region ItemContainerGeneratorStatusChanged
		[Test]
		public void ItemContainerGeneratorStatusChanged ()
		{
			ItemContainerGeneratorStatusChangedSelector t = new ItemContainerGeneratorStatusChangedSelector ();
			Assert.AreEqual (t.GetHandlerCount (), 1, "1");
			Window w = new Window ();
			w.Content = t;
			w.Show ();
			Assert.AreEqual (t.GetHandlerCount (), 1, "2");
		}

		class ItemContainerGeneratorStatusChangedSelector : Selector
		{
			public int GetHandlerCount ()
			{
				EventHandler handler = (EventHandler)typeof (ItemContainerGenerator).GetField ("StatusChanged", BindingFlags.Instance | BindingFlags.NonPublic).GetValue (ItemContainerGenerator);
				if (handler == null)
					return 0;
				return handler.GetInvocationList ().GetLength (0);
			}
		}
		#endregion

		class TestSelector : Selector
		{
		}
	}
}