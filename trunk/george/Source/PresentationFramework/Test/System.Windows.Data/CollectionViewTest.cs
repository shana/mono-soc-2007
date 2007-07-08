using NUnit.Framework;
using System;
using System.Collections;
#if Implementation
namespace Mono.System.Windows.Data {
#else
namespace System.Windows.Data {
#endif
	[TestFixture]
	public class CollectionViewTest {
		#region Constructor
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorNull() {
			new CollectionView(null);
		}

		[Test]
		public void ConstructorNonNull() {
			new CollectionView(new object[] { });
		}
		#endregion

		#region CanFilter
		[Test]
		public void CanFilterDefaultValue() {
			Assert.IsTrue(new CollectionView(new object[] { }).CanFilter);
		}

		#region IfCanFilterReturnsFalseSettingFilterCausesAnException
		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void IfCanFilterReturnsFalseSettingFilterCausesAnException() {
			new IfCanFilterReturnsFalseSettingFilterCausesAnExceptionCollectionView();
		}

		class IfCanFilterReturnsFalseSettingFilterCausesAnExceptionCollectionView : CollectionView {
			public IfCanFilterReturnsFalseSettingFilterCausesAnExceptionCollectionView()
				: base(new object[] { }) {
				Filter = null;
			}

			public override bool CanFilter {
				get { return false; }
			}
		}
		#endregion
		#endregion

		#region CanGroup
		[Test]
		public void CanGroupDefaultValue() {
			Assert.IsFalse(new CollectionView(new object[] { }).CanGroup);
		}
		#endregion

		#region CanSort
		[Test]
		public void CanSortDefaultValue() {
			Assert.IsFalse(new CollectionView(new object[] { }).CanSort);
		}
		#endregion

		#region Comparer
		[Test]
		public void ComparerDefaultValue() {
			Assert.IsNull(new CollectionView(new object[] { }).Comparer);
		}
		#endregion

		#region Count
		[Test]
		public void CountCachesValue() {
			CollectionView c = new CollectionView(new CountCachesValueEnumerable());
			Assert.AreEqual(CountCachesValueEnumerable.get_enumerator_calls, 1, "1");
			Assert.AreEqual(CountCachesValueEnumerable.current_calls, 1, "2");
			Assert.AreEqual(CountCachesValueEnumerable.move_next_calls, 1, "3");
			Assert.AreEqual(CountCachesValueEnumerable.reset_calls, 0, "4");
			CountCachesValueEnumerable.get_enumerator_calls = 0;
			CountCachesValueEnumerable.current_calls = 0;
			CountCachesValueEnumerable.move_next_calls = 0;
			object dummy = c.Count;
			Assert.AreEqual(CountCachesValueEnumerable.get_enumerator_calls, 1, "5");
			Assert.AreEqual(CountCachesValueEnumerable.current_calls, 4, "6");
			Assert.AreEqual(CountCachesValueEnumerable.move_next_calls, 5, "7");
			Assert.AreEqual(CountCachesValueEnumerable.reset_calls, 0, "8");
			CountCachesValueEnumerable.get_enumerator_calls = 0;
			CountCachesValueEnumerable.current_calls = 0;
			CountCachesValueEnumerable.move_next_calls = 0;
			dummy = c.Count;
			Assert.AreEqual(CountCachesValueEnumerable.get_enumerator_calls, 0, "9");
			Assert.AreEqual(CountCachesValueEnumerable.current_calls, 0, "10");
			Assert.AreEqual(CountCachesValueEnumerable.move_next_calls, 0, "11");
			Assert.AreEqual(CountCachesValueEnumerable.reset_calls, 0, "12");
		}

		class CountCachesValueEnumerable : IEnumerable {
			public static int get_enumerator_calls;
			public static int current_calls;
			public static int move_next_calls;
			public static int reset_calls;

			public IEnumerator GetEnumerator() {
				get_enumerator_calls++;
				return new TestEnumerator();
			}

			class TestEnumerator : IEnumerator {
				int current;
				public object Current {
					get {
						current_calls++;
						return current;
					}
				}

				public bool MoveNext() {
					move_next_calls++;
					current++;
					return current < 5;
				}

				public void Reset() {
					reset_calls++;
					current = 0;
				}
			}
		}
		#endregion

		#region Culture
		[Test]
		public void CultureDefaultValue() {
			Assert.IsNull(new CollectionView(new object[] { }).Culture);
		}
		#endregion

		#region CurrentItem
		[Test]
		public void CurrentItemDefaultValue() {
			Assert.IsNull(new CollectionView(new object[] { }).CurrentItem);
		}

		[Test]
		public void TheFirstItemStartsAsTheCurrentItem() {
			Assert.AreEqual(new CollectionView(new object[] { 1, 2 }).CurrentItem, 1);
		}

		#region SortingPreservesCurrentItem
		[Test]
		public void SortingPreservesCurrentItem() {
			CollectionView c = new CollectionView(new object[] { 1, 2 });
			Assert.AreEqual(c.CurrentItem, 1);
		}

		class SortingPreservesCurrentItemCollectionView : CollectionView {
			public SortingPreservesCurrentItemCollectionView(IEnumerable collection)
				: base(collection) {
			}

			public override IComparer Comparer {
				get {
					return new SortingPreservesCurrentItemComparer();
				}
			}
		}

		class SortingPreservesCurrentItemComparer : IComparer {
			public int Compare(object x, object y) {
				return (int)x - (int)y;
			}
		}
		#endregion

		[Test]
		public void FilteringPreservesCurrentItemIfItPassesTheFilterOtherwiseCurrentItemIsSetToTheFirstInTheFilteredView() {
			CollectionView c = new CollectionView(new int[] { 1, 2, 3 });
			c.Filter = FilteringPreservesCurrentItemIfItPassesTheFilterOtherwiseCurrentItemIsSetToTheFirstInTheFilteredViewFilter1;
			Assert.AreEqual(c.CurrentItem, 1, "1");
			c.Filter = FilteringPreservesCurrentItemIfItPassesTheFilterOtherwiseCurrentItemIsSetToTheFirstInTheFilteredViewFilter2;
			Assert.AreEqual(c.CurrentItem, 1, "2");
		}

		bool FilteringPreservesCurrentItemIfItPassesTheFilterOtherwiseCurrentItemIsSetToTheFirstInTheFilteredViewFilter1(object item) {
			return true;
		}

		bool FilteringPreservesCurrentItemIfItPassesTheFilterOtherwiseCurrentItemIsSetToTheFirstInTheFilteredViewFilter2(object item) {
			return (int)item > 1;
		}
		#endregion
	}
}