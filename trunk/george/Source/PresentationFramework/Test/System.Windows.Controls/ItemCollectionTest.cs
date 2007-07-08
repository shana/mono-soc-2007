using NUnit.Framework;
using System.Collections;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class ItemCollectionTest {
		[Test]
		public void WorksAtAll() {
			ItemCollection c = new ItemsControl().Items;
			c.Add("Test");
			Assert.AreEqual(c.Count, 1);
		}

		[Test]
		public void Test() {
			ItemCollection c = new ItemsControl().Items;
			Assert.IsTrue(c.CanFilter, "1");
			Assert.IsFalse(c.CanGroup, "2");
			Assert.IsTrue(c.CanSort, "3");
			Assert.IsNull(c.Comparer, "4");
			Assert.AreEqual(c.Count, 0, "5");
			Assert.IsNull(c.CurrentItem, "6");
			Assert.AreEqual(c.CurrentPosition, -1, "7");
			Assert.IsNull(c.Filter, "8");
			Assert.IsNotNull(c.GroupDescriptions, "9");
			Assert.AreEqual(c.GroupDescriptions.Count, 0, "9 1");
			Assert.IsNull(c.Groups, "10");
			Assert.IsTrue(c.IsEmpty, "11");
			Assert.IsFalse(c.NeedsRefresh, "12");
			Assert.IsNotNull(c.SortDescriptions, "13");
			Assert.AreEqual(c.SortDescriptions.Count, 0, "13 1");
			Assert.IsNotNull(c.SourceCollection, "14");
			Assert.AreEqual(c.SourceCollection.GetType(), typeof(ItemCollection), "14 1");
			Assert.AreSame(c.SourceCollection, c, "14 2");
		}

		#region ItemSourceNullEnumerator
		[Test]
		[ExpectedException(typeof(NullReferenceException))]
		public void ItemSourceNullEnumerator() {
			new ItemsControl().ItemsSource = new ItemSourceNullEnumeratorEnumerable();
		}
		class ItemSourceNullEnumeratorEnumerable : IEnumerable {
			public IEnumerator GetEnumerator() {
				return null;
			}
		}
		#endregion

		#region CountEnumeratesSourceCollection
		[Test]
		public void CountEnumeratesSourceCollection() {
			ItemsControl c = new ItemsControl();
			c.ItemsSource = new CountEnumeratesSourceCollectionEnumerable();
			Assert.IsTrue(CountEnumeratesSourceCollectionEnumerable.Called, "1");
			ItemCollection i = c.Items;
			CountEnumeratesSourceCollectionEnumerable.Called = false;
			object dummy = i.Count;
			Assert.IsFalse(CountEnumeratesSourceCollectionEnumerable.Called, "2");
		}
		class CountEnumeratesSourceCollectionEnumerable : IEnumerable {
			public static bool Called;
			
			public IEnumerator GetEnumerator() {
				Called = true;
				return new TestEnumerator();
			}
			
			class TestEnumerator : IEnumerator {
				public object Current {
					get { return null; }
				}

				public bool MoveNext() {
					return false;
				}

				public void Reset() {
				}
			}
		}
		#endregion

		#region Filter
		[Test]
		public void Filter() {
			ItemCollection c = new ItemsControl().Items;
			c.Add(1);
			c.Add(-1);
			Assert.AreEqual(c.Count, 2, "1");
			c.Filter = FilterFilter;
			Assert.AreEqual(c.Count, 1, "2");
			c.Filter = null;
			Assert.AreEqual(c.Count, 2, "3");
			c.Filter = FilterFilter;
			c.Add(2);
			Assert.AreEqual(c.Count, 2, "4");
			c.Filter = null;
			Assert.AreEqual(c.Count, 3, "5");
			c.Filter = FilterFilter;
			c.Remove(-1);
			c.Filter = null;
			Assert.AreEqual(c.Count, 2, "6");
		}
		
		bool FilterFilter(object item) {
			return (int)item > 0;
		}
		#endregion

		#region CountCallsFilter
		[Test]
		public void CountCallsFilter() {
			ItemCollection c = new ItemsControl().Items;
			c.Add(1);
			c.Filter = CountCallsFilterFilter;
			Assert.AreEqual(count_calls_filter_filter_calls, 1, "1");
			count_calls_filter_filter_calls = 0;
			object dummy = c.Count;
			Assert.AreEqual(count_calls_filter_filter_calls, 0, "2");
		}
		
		int count_calls_filter_filter_calls;

		bool CountCallsFilterFilter(object item) {
			count_calls_filter_filter_calls++;
			return true;
		}
		#endregion
	}
}