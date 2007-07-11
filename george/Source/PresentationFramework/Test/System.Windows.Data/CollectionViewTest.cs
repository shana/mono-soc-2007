using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
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

		[Test]
		public void NullItemsCount() {
			Assert.AreEqual(new CollectionView(new object[] { null }).Count, 1);
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
			// Why?
			Assert.AreEqual(c.CurrentItem, 1, "2");
			c.Refresh();
			Assert.AreEqual(c.CurrentItem, 1, "3");
			Assert.IsFalse(c.PassesFilter(c.CurrentItem), "4");
			Assert.AreEqual(c.CurrentPosition, 0, "5");
		}

		bool FilteringPreservesCurrentItemIfItPassesTheFilterOtherwiseCurrentItemIsSetToTheFirstInTheFilteredViewFilter1(object item) {
			return true;
		}

		bool FilteringPreservesCurrentItemIfItPassesTheFilterOtherwiseCurrentItemIsSetToTheFirstInTheFilteredViewFilter2(object item) {
			return (int)item > 1;
		}
		#endregion

		#region CurrentPosition
		[Test]
		public void CurrentPositionDefaultValue() {
			Assert.AreEqual(new CollectionView(new object[] { }).CurrentPosition, -1, "1");
			Assert.AreEqual(new CollectionView(new object[] { 1 }).CurrentPosition, 0, "2");
			Assert.AreEqual(new CollectionView(new object[] { null }).CurrentPosition, 0, "3");
		}
		#endregion

		#region Filter
		[Test]
		public void FilterDefaultValue() {
			Assert.IsNull(new CollectionView(new object[] { }).Filter);
		}

		[Test]
		public void Filter() {
			CollectionView c = new CollectionView(new object[] { -1, 1, });
			c.Filter = FilterFilter;
			// I don't understand.
			Assert.AreEqual(c.Count, 2, "1");
			c.Refresh();
			Assert.AreEqual(c.Count, 2, "2");
		}

		bool FilterFilter(object item) {
			return (int)item > 0;
		}
		#endregion

		#region GroupDescriptions
		[Test]
		public void GroupDescriptionsDefaultValue() {
			Assert.IsNull(new CollectionView(new object[] { }).GroupDescriptions);
		}
		#endregion

		#region Groups
		[Test]
		public void GroupsDefaultValue() {
			Assert.IsNull(new CollectionView(new object[] { }).Groups);
		}
		#endregion

		#region IsCurrentAfterLast
		[Test]
		public void IsCurrentAfterLastDefaultValue() {
			Assert.IsTrue(new CollectionView(new object[] { }).IsCurrentAfterLast);
		}

		[Test]
		public void IsCurrentAfterLast() {
			CollectionView c = new CollectionView(new object[] { 1, 2 });
			Assert.IsFalse(c.IsCurrentAfterLast, "1");
			Assert.IsFalse(c.IsCurrentBeforeFirst, "2");
			c.MoveCurrentTo(1);
			Assert.IsFalse(c.IsCurrentAfterLast, "3");
			Assert.IsFalse(c.IsCurrentBeforeFirst, "4");
			c.MoveCurrentTo(3);
			Assert.IsFalse(c.IsCurrentAfterLast, "5");
			Assert.IsTrue(c.IsCurrentBeforeFirst, "6");
			c.MoveCurrentTo(1);
			Assert.IsFalse(c.IsCurrentAfterLast, "7");
			Assert.IsFalse(c.IsCurrentBeforeFirst, "8");
			c.MoveCurrentToPrevious();
			Assert.IsFalse(c.IsCurrentAfterLast, "9");
			Assert.IsTrue(c.IsCurrentBeforeFirst, "10");
			c.MoveCurrentToLast();
			c.MoveCurrentToNext();
			Assert.IsTrue(c.IsCurrentAfterLast, "11");
			Assert.IsFalse(c.IsCurrentBeforeFirst, "12");
			c.MoveCurrentToFirst();
			Assert.IsFalse(c.IsCurrentAfterLast, "13");
			Assert.IsFalse(c.IsCurrentBeforeFirst, "14");
			c.MoveCurrentToPrevious();
			c.MoveCurrentToNext();
			Assert.IsFalse(c.IsCurrentAfterLast, "15");
			Assert.IsFalse(c.IsCurrentBeforeFirst, "16");
			c.MoveCurrentToLast();
			c.MoveCurrentToNext();
			c.MoveCurrentToPrevious();
			Assert.IsFalse(c.IsCurrentAfterLast, "17");
			Assert.IsFalse(c.IsCurrentBeforeFirst, "18");
		}

		#region IsCurrentAfterLastSetCurrent
		[Test]
		public void IsCurrentAfterLastSetCurrent() {
			new IsCurrentAfterLastSetCurrentCollectionView();
		}

		class IsCurrentAfterLastSetCurrentCollectionView : CollectionView {
			public IsCurrentAfterLastSetCurrentCollectionView()
				: base(new object[] { 1 }) {
				SetCurrent(1, 0);
				Assert.IsFalse(IsCurrentAfterLast, "1");
				Assert.IsFalse(IsCurrentBeforeFirst, "2");
				SetCurrent(1, -1);
				Assert.IsFalse(IsCurrentAfterLast, "3");
				Assert.IsTrue(IsCurrentBeforeFirst, "4");
				SetCurrent(1, 0);
				Assert.IsFalse(IsCurrentAfterLast, "5");
				Assert.IsFalse(IsCurrentBeforeFirst, "6");
				SetCurrent(2, 0);
				Assert.IsFalse(IsCurrentAfterLast, "7");
				Assert.IsFalse(IsCurrentBeforeFirst, "8");
				SetCurrent(null, 1);
				Assert.IsTrue(IsCurrentAfterLast, "9");
				Assert.IsFalse(IsCurrentBeforeFirst, "10");
			}
		}
		#endregion
		#endregion

		#region IsCurrentBeforeFirst
		[Test]
		public void IsCurrentBeforeFirstDefaultValue() {
			Assert.IsTrue(new CollectionView(new object[] { }).IsCurrentBeforeFirst);
		}
		#endregion

		#region IsEmpty
		[Test]
		public void IsEmptyDefaultValue() {
			Assert.IsTrue(new CollectionView(new object[] { }).IsEmpty);
		}

		[Test]
		public void IsEmptyNullFilterNonEmptySourceCollection() {
			Assert.IsFalse(new CollectionView(new object[] { 1 }).IsEmpty);
		}

		#region IsEmptyFilterNonEmptySourceCollection
		[Test]
		public void IsEmptyFilterNonEmptySourceCollection() {
			CollectionView c = new CollectionView(new object[] { 1 });
			c.Filter = IsEmptyFilterNonEmptySourceCollectionFilter;
			Assert.IsFalse(c.IsEmpty);
		}

		bool IsEmptyFilterNonEmptySourceCollectionFilter(object item) {
			return false;
		}
		#endregion

		#region IsEmptyCallsCount
		[Test]
		public void IsEmptyCallsCount() {
			new IsEmptyCallsCountCollectionView();
		}

		class IsEmptyCallsCountCollectionView : CollectionView {
			int count_calls;

			public IsEmptyCallsCountCollectionView()
				: base(new object[] { 1 }) {
				Assert.AreEqual(count_calls, 0, "1");
				object dummmy = IsEmpty;
				Assert.AreEqual(count_calls, 0, "2");
			}

			public override int Count {
				get {
					count_calls++;
					return base.Count;
				}
			}
		}
		#endregion

		#region IsEmptyCallsEnumerator
		[Test]
		public void IsEmptyCallsEnumerator() {
			new IsEmptyCallsEnumeratorCollectionView();
		}

		class IsEmptyCallsEnumeratorCollectionView : CollectionView {
			int calls;

			public IsEmptyCallsEnumeratorCollectionView()
				: base(new object[] { 1 }) {
				Assert.AreEqual(calls, 0, "1");
				object dummmy = IsEmpty;
				Assert.AreEqual(calls, 0, "2");
			}

			protected override IEnumerator GetEnumerator() {
				calls++;
				return base.GetEnumerator();
			}
		}
		#endregion

		#region IsEmptyCallsEnumeratorOnSourceCollection
		[Test]
		public void IsEmptyCallsEnumeratorOnSourceCollection() {
			new IsEmptyCallsEnumeratorOnSourceCollectionCollectionView();
		}

		class IsEmptyCallsEnumeratorOnSourceCollectionCollectionView : CollectionView {
			static int calls;

			public IsEmptyCallsEnumeratorOnSourceCollectionCollectionView()
				: base(new TestEnumerable()) {
				Assert.AreEqual(calls, 1, "1");
				object dummmy = IsEmpty;
				Assert.AreEqual(calls, 2, "2");
				dummmy = IsEmpty;
				Assert.AreEqual(calls, 2, "3");
			}

			class TestEnumerable : IEnumerable {
				public IEnumerator GetEnumerator() {
					calls++;
					return new TestEnumerator();
				}

				class TestEnumerator : IEnumerator {
					int current;
					public object Current {
						get { return current; }
					}

					public bool MoveNext() {
						current++;
						return current < 2;
					}

					public void Reset() {
						current = 0;
					}
				}
			}
		}
		#endregion
		#endregion

		#region NeedsRefresh
		[Test]
		public void NeedsRefreshDefaultValue() {
			Assert.IsTrue(new CollectionView(new object[] { }).NeedsRefresh);
		}
		#endregion

		#region SortDescriptions
		[Test]
		public void SortDescriptionsDefaultValue() {
			SortDescriptionCollection d = new CollectionView(new object[] { }).SortDescriptions;
			Assert.IsNotNull(d, "1");
			Assert.AreEqual(d.Count, 0, "2");
		}

		[Test]
		public void SortDescriptionsReturnsSortDescriptionCollectionEmpty() {
			Assert.AreSame(new CollectionView(new object[] { }).SortDescriptions, SortDescriptionCollection.Empty);
		}
		#endregion

		#region SourceCollection
		[Test]
		public void SourceCollectionDefaultValue() {
			IEnumerable data = new object[] { };
			Assert.AreEqual(new CollectionView(data).SourceCollection, data);
		}
		#endregion

		#region Contains
		[Test]
		public void ContainsFilterNull() {
			CollectionView c = new CollectionView(new object[] { 1 });
			Assert.IsFalse(c.Contains(null), "1");
			Assert.IsTrue(c.Contains(1), "2");
			Assert.IsFalse(c.Contains(2), "3");
		}

		#region ContainsFilter
		[Test]
		public void ContainsFilter() {
			CollectionView c = new CollectionView(new object[] { -1, 1 });
			c.Filter = ContainsFilterFilter;
			Assert.IsFalse(c.Contains(null), "1");
			Assert.IsTrue(c.Contains(1), "2");
			Assert.IsFalse(c.Contains(2), "3");
			Assert.IsFalse(c.Contains(-1), "4");
		}

		bool ContainsFilterFilter(object item) {
			return (int)item > 0;
		}
		#endregion

		#region ContainsCallsPassesFilter
		[Test]
		public void ContainsCallsPassesFilter() {
			new ContainsCallsPassesFilterCollectionView();
		}

		class ContainsCallsPassesFilterCollectionView : CollectionView {
			int calls;

			public ContainsCallsPassesFilterCollectionView()
				: base(new object[] { 1 }) {
				Assert.AreEqual(calls, 0, "1");
				object dummy = Contains(1);
				Assert.AreEqual(calls, 1, "1");
			}

			public override bool PassesFilter(object item) {
				calls++;
				return base.PassesFilter(item);
			}
		}
		#endregion
		#endregion

		#region GetItemAt
		[Test]
		public void GetItemAtNullFilter() {
			CollectionView c = new CollectionView(new object[] { 1 });
			Assert.AreEqual(c.GetItemAt(0), 1, "1");
			try {
				c.GetItemAt(-1);
				Assert.Fail("2");
			} catch (ArgumentOutOfRangeException) {
			}
			try {
				c.GetItemAt(1);
				Assert.Fail("3");
			} catch (IndexOutOfRangeException) {
			}
		}

		[Test]
		public void GetItemAtFilter() {
			CollectionView c = new CollectionView(new object[] { -1, 1 });
			c.Filter = GetItemAtFilterFilter;
			Assert.AreEqual(c.GetItemAt(0), -1, "1");
			Assert.AreEqual(c.GetItemAt(1), 1, "2");
			try {
				c.GetItemAt(-1);
				Assert.Fail("3");
			} catch (ArgumentOutOfRangeException) {
			}
			try {
				c.GetItemAt(2);
				Assert.Fail("4");
			} catch (IndexOutOfRangeException) {
			}
		}

		bool GetItemAtFilterFilter(object item) {
			return (int)item > 0;
		}
		#endregion

		#region IndexOf
		[Test]
		public void IndexOf() {
			CollectionView c = new CollectionView(new object[] { 1, 2 });
			Assert.AreEqual(c.IndexOf(null), -1, "1");
			Assert.AreEqual(c.IndexOf(0), -1, "2");
			Assert.AreEqual(c.IndexOf(1), 0, "3");
			Assert.AreEqual(c.IndexOf(2), 1, "4");
			c = new CollectionView(new object[] { null });
			Assert.AreEqual(c.IndexOf(null), 0, "5");
		}

		#region IndexOfFilter
		[Test]
		public void IndexOfFilter() {
			CollectionView c = new CollectionView(new object[] { -1, 1 });
			c.Filter = IndexOfFilterFilter;
			Assert.AreEqual(c.IndexOf(-1), -1, "1");
			Assert.AreEqual(c.IndexOf(1), 0, "2");
		}

		bool IndexOfFilterFilter(object item) {
			return (int)item > 0;
		}
		#endregion
		#endregion

		#region MoveCurrentTo
		[Test]
		public void MoveCurrentTo() {
			CollectionView c = new CollectionView(new object[] { 1, 2 });
			Assert.IsTrue(c.MoveCurrentTo(2), "1");
			Assert.AreEqual(c.CurrentItem, 2, "2");
			Assert.AreEqual(c.CurrentPosition, 1, "3");
			Assert.IsFalse(c.MoveCurrentTo(null), "4");
			Assert.IsNull(c.CurrentItem, "5");
			Assert.AreEqual(c.CurrentPosition, -1, "6");
			Assert.IsFalse(c.MoveCurrentTo(3), "7");
			Assert.IsNull(c.CurrentItem, "8");
			Assert.AreEqual(c.CurrentPosition, -1, "9");
			c = new CollectionView(new object[] { null, 1 });
			Assert.IsNull(c.CurrentItem, "10");
			Assert.AreEqual(c.CurrentPosition, 0, "11");
			Assert.IsTrue(c.MoveCurrentTo(1), "12");
			Assert.AreEqual(c.CurrentItem, 1, "13");
			Assert.AreEqual(c.CurrentPosition, 1, "14");
			Assert.IsTrue(c.MoveCurrentTo(null), "15");
			Assert.IsNull(c.CurrentItem, "16");
			Assert.AreEqual(c.CurrentPosition, 0, "17");
			c = new CollectionView(new object[] { -1, 1 });
			c.Filter = MoveCurrentToFilter;
			Assert.AreEqual(c.CurrentItem, -1, "18");
			Assert.AreEqual(c.CurrentPosition, 0, "19");
			Assert.IsTrue(c.MoveCurrentTo(-1), "20");
			Assert.AreEqual(c.CurrentItem, -1, "21");
			Assert.AreEqual(c.CurrentPosition, 0, "22");
		}

		bool MoveCurrentToFilter(object item) {
			return (int)item > 0;
		}

		#region MoveCurrentToCallsMoveCurrentToPosition
		[Test]
		public void MoveCurrentToCallsMoveCurrentToPosition() {
			new MoveCurrentToCallsMoveCurrentToPositionCollectionView();
		}

		class MoveCurrentToCallsMoveCurrentToPositionCollectionView : CollectionView {
			int calls;
			int argument;

			public MoveCurrentToCallsMoveCurrentToPositionCollectionView()
				: base(new object[] { 1, 2 }) {
				Assert.AreEqual(calls, 0, "1");
				Assert.IsTrue(MoveCurrentTo(1), "1 1");
				Assert.AreEqual(calls, 0, "2");
				MoveCurrentTo(2);
				Assert.AreEqual(calls, 1, "4");
				Assert.AreEqual(argument, 1, "5");
				MoveCurrentTo(2);
				Assert.AreEqual(calls, 1, "6");
				Assert.AreEqual(argument, 1, "7");
			}

			public override bool MoveCurrentToPosition(int position) {
				calls++;
				argument = position;
				return base.MoveCurrentToPosition(position);
			}
		}
		#endregion
		#endregion

		#region MoveCurrentToFirst
		[Test]
		public void MoveCurrentToFirst() {
			CollectionView c = new CollectionView(new object[] { 1, 2 });
			c.MoveCurrentTo(2);
			Assert.IsTrue(c.MoveCurrentToFirst(), "1");
			Assert.AreEqual(c.CurrentItem, 1, "2");
			Assert.AreEqual(c.CurrentPosition, 0, "3");
			c = new CollectionView(new object[] { });
			Assert.AreEqual(c.CurrentPosition, -1, "4");
			Assert.IsFalse(c.MoveCurrentToFirst(), "5");
			Assert.IsNull(c.CurrentItem, "6");
			// Why?
			Assert.AreEqual(c.CurrentPosition, 0, "7");
		}

		[Test]
		public void MoveCurrentToFirstFilter() {
			CollectionView c = new CollectionView(new object[] { -1, 1 });
			c.Filter = MoveCurrentToFirstFilterFilter;
			Assert.IsTrue(c.MoveCurrentToFirst(), "1");
			Assert.AreEqual(c.CurrentItem, -1, "2");
			Assert.AreEqual(c.CurrentPosition, 0, "3");
		}

		bool MoveCurrentToFirstFilterFilter(object item) {
			return (int)item > 0;
		}
		#endregion

		#region MoveCurrentToLast
		[Test]
		public void MoveCurrentToLast() {
			CollectionView c = new CollectionView(new object[] { 1, 2 });
			Assert.IsTrue(c.MoveCurrentToLast(), "1");
			Assert.AreEqual(c.CurrentItem, 2, "2");
			Assert.AreEqual(c.CurrentPosition, 1, "3");
			c = new CollectionView(new object[] { });
			Assert.IsFalse(c.MoveCurrentToLast(), "4");
			Assert.IsNull(c.CurrentItem, "5");
			Assert.AreEqual(c.CurrentPosition, -1, "6");
		}
		#endregion

		#region MoveCurrentToNext
		[Test]
		public void MoveCurrentToNext() {
			CollectionView c = new CollectionView(new object[] { 1, 2 });
			Assert.IsTrue(c.MoveCurrentToNext(), "1");
			Assert.AreEqual(c.CurrentItem, 2, "2");
			Assert.AreEqual(c.CurrentPosition, 1, "3");
			Assert.IsFalse(c.MoveCurrentToNext(), "4");
			Assert.IsNull(c.CurrentItem, "5");
			// Why?
			Assert.AreEqual(c.CurrentPosition, 2, "6");
			c.MoveCurrentToFirst();
			Assert.IsTrue(c.MoveCurrentToNext(), "7");
			Assert.IsFalse(c.MoveCurrentToNext(), "8");
		}

		#region MoveCurrentToNextCallsMoveCurrentToFirst
		[Test]
		public void MoveCurrentToNextCallsMoveCurrentToFirst() {
			new MoveCurrentToNextCallsMoveCurrentToFirstCollectionView();
		}

		class MoveCurrentToNextCallsMoveCurrentToFirstCollectionView : CollectionView {
			int calls;

			public MoveCurrentToNextCallsMoveCurrentToFirstCollectionView()
				: base(new object[] { 1 }) {
				MoveCurrentToPrevious();
				Assert.AreEqual(calls, 0, "1");
				MoveCurrentToNext();
				Assert.AreEqual(calls, 0, "2");
			}

			public override bool MoveCurrentToFirst() {
				calls++;
				return base.MoveCurrentToFirst();
			}
		}
		#endregion

		#region MoveCurrentToNextCallsMoveCurrentToPosition
		[Test]
		public void MoveCurrentToNextCallsMoveCurrentToPosition() {
			new MoveCurrentToNextCallsMoveCurrentToPositionCollectionView();
		}

		class MoveCurrentToNextCallsMoveCurrentToPositionCollectionView : CollectionView {
			int calls;
			int argument;

			public MoveCurrentToNextCallsMoveCurrentToPositionCollectionView()
				: base(new object[] { 1, 2 }) {
				Assert.AreEqual(calls, 0, "1");
				MoveCurrentToNext();
				Assert.AreEqual(calls, 1, "2");
				Assert.AreEqual(argument, 1, "3");
				MoveCurrentToNext();
				Assert.AreEqual(calls, 2, "4");
				Assert.AreEqual(argument, 2, "5");
				MoveCurrentToNext();
				Assert.AreEqual(calls, 2, "6");
				Assert.AreEqual(argument, 2, "7");
			}

			public override bool MoveCurrentToPosition(int position) {
				calls++;
				argument = position;
				return base.MoveCurrentToPosition(position);
			}
		}
		#endregion
		#endregion

		#region MoveCurrentToPosition
		[Test]
		public void MoveCurrentToPosition() {
			CollectionView c = new CollectionView(new object[] { 1, 2 });
			Assert.IsTrue(c.MoveCurrentToPosition(1), "1");
			Assert.AreEqual(c.CurrentItem, 2, "2");
			Assert.AreEqual(c.CurrentPosition, 1, "3");
			Assert.IsFalse(c.MoveCurrentToPosition(2), "4");
			Assert.AreEqual(c.CurrentItem, null, "5");
			Assert.AreEqual(c.CurrentPosition, 2, "6");
			try {
				c.MoveCurrentToPosition(3);
				Assert.Fail("7");
			} catch (ArgumentOutOfRangeException ex) {
				Assert.AreEqual(ex.ParamName, "position", "8");
			}
			Assert.AreEqual(c.CurrentItem, null, "9");
			Assert.AreEqual(c.CurrentPosition, 2, "10");

			c = new CollectionView(new object[] { 1, 2 });
			try {
				c.MoveCurrentToPosition(3);
				Assert.Fail("11");
			} catch (ArgumentOutOfRangeException ex) {
				Assert.AreEqual(ex.ParamName, "position", "12");
			}
		}
		#endregion

		#region MoveCurrentToPrevious
		[Test]
		public void MoveCurrentToPrevious() {
			CollectionView c = new CollectionView(new object[] { 1, 2 });
			Assert.IsFalse(c.MoveCurrentToPrevious(), "1");
			Assert.IsNull(c.CurrentItem, "2");
			Assert.AreEqual(c.CurrentPosition, -1, "3");
			Assert.IsFalse(c.MoveCurrentToPrevious(), "4");
			Assert.IsNull(c.CurrentItem, "5");
			Assert.AreEqual(c.CurrentPosition, -1, "6");
			c = new CollectionView(new object[] { 1, 2 });
			c.MoveCurrentTo(2);
			Assert.AreEqual(c.CurrentItem, 2, "8");
			Assert.AreEqual(c.CurrentPosition, 1, "9");
			Assert.IsTrue(c.MoveCurrentToPrevious(), "10");
			Assert.AreEqual(c.CurrentItem, 1, "11");
			Assert.AreEqual(c.CurrentPosition, 0, "12");
			Assert.IsFalse(c.MoveCurrentToPrevious(), "13");
			Assert.IsNull(c.CurrentItem, "14");
			Assert.AreEqual(c.CurrentPosition, -1, "15");
		}

		#region MoveCurrentToPreviousCallsMoveCurrentToPosition
		[Test]
		public void MoveCurrentToPreviousCallsMoveCurrentToPosition() {
			new MoveCurrentToPreviousCallsMoveCurrentToPositionCollectionView();
		}

		class MoveCurrentToPreviousCallsMoveCurrentToPositionCollectionView : CollectionView {
			int calls;
			int argument;

			public MoveCurrentToPreviousCallsMoveCurrentToPositionCollectionView()
				: base(new object[] { 1, 2 }) {
				Assert.AreEqual(calls, 0, "1");
				MoveCurrentToPrevious();
				Assert.AreEqual(calls, 1, "2");
				Assert.AreEqual(argument, -1, "3");
			}

			public override bool MoveCurrentToPosition(int position) {
				calls++;
				argument = position;
				return base.MoveCurrentToPosition(position);
			}
		}
		#endregion
		#endregion

		#region PassesFilter
		[Test]
		public void PassesFilter() {
			CollectionView c = new CollectionView(new object[] { -1, 1 });
			Assert.IsTrue(c.PassesFilter(1), "1");
			Assert.IsTrue(c.PassesFilter(2), "2");
			c.Filter = PassesFilterFilter;
			Assert.IsTrue(c.PassesFilter(1), "3");
			Assert.IsTrue(c.PassesFilter(2), "4");
			Assert.IsFalse(c.PassesFilter(-1), "5");
			Assert.IsFalse(c.PassesFilter(-2), "6");
		}

		bool PassesFilterFilter(object item) {
			return (int)item > 0;
		}

		#region CallsFilterOnNull
		bool calls_filter_on_null;

		[Test]
		public void CallsFilterOnNull() {
			CollectionView c = new CollectionView(new object[] { });
			c.Filter = CallsFilterOnNullFilter;
			calls_filter_on_null = false;
			c.PassesFilter(null);
			Assert.IsTrue(calls_filter_on_null);
		}

		bool CallsFilterOnNullFilter(object item) {
			calls_filter_on_null = true;
			return false;
		}
		#endregion
		#endregion

		#region GetEnumerator
		[Test]
		public void GetEnumerator() {
			new GetEnumeratorCollectionView();
		}

		class GetEnumeratorCollectionView : CollectionView {
			static int calls;
			static int source_collection_calls;

			public GetEnumeratorCollectionView()
				: base(new TestEnumerable()) {
				Assert.AreEqual(calls, 1, "1");
				Assert.AreEqual(source_collection_calls, 0, "2");
				GetEnumerator();
				Assert.AreEqual(calls, 2, "4");
				Assert.AreEqual(source_collection_calls, 1, "4");
			}

			public override IEnumerable SourceCollection {
				get {
					source_collection_calls++;
					return base.SourceCollection;
				}
			}

			class TestEnumerable : IEnumerable {
				public IEnumerator GetEnumerator() {
					calls++;
					return new object[] { }.GetEnumerator();
				}
			}
		}
		#endregion

		#region OKToChangeCurrent
		[Test]
		public void OKToChangeCurrent() {
			new OKToChangeCurrentCollectionView();
		}

		class OKToChangeCurrentCollectionView : CollectionView {
			public OKToChangeCurrentCollectionView()
				: base(new object[] { 1, 2 }) {
				Assert.IsTrue(OKToChangeCurrent(), "1");
			}
		}
		#endregion

		#region OnBeginChangeLogging
		[Test]
		public void OnBeginChangeLogging() {
			new OnBeginChangeLoggingCollectionView();
		}

		class OnBeginChangeLoggingCollectionView : CollectionView {
			public OnBeginChangeLoggingCollectionView()
				: base(new object[] { 1, 2 }) {
				OnBeginChangeLogging(null);
			}
		}
		#endregion

		#region OnCollectionChanged
		[Test]
		public void OnCollectionChanged() {
			new OnCollectionChagedCollectionView();
		}

		class OnCollectionChagedCollectionView : CollectionView {
			public OnCollectionChagedCollectionView()
				: base(new object[] { 1, 2 }) {
				Assert.AreEqual(GetHandlerCount(), 0);
			}

			public int GetHandlerCount() {
				EventHandler handler = (EventHandler)typeof(CollectionView).GetField("CollectionChanged", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
				if (handler == null)
					return 0;
				return handler.GetInvocationList().GetLength(0);
			}
		}
		#endregion

		#region OnCollectionChanged(object, NotifyCollectionChangedEventArgs)
		[Test]
		public void OnCollectionChangedObjectNotifyCollectionChangedEventArgs() {
			new OnCollectionChangedObjectNotifyCollectionChangedEventArgsCollectionView();
		}

		class OnCollectionChangedObjectNotifyCollectionChangedEventArgsCollectionView : CollectionView {
			int calls;

			public OnCollectionChangedObjectNotifyCollectionChangedEventArgsCollectionView()
				: base(new object[] { 1, 2 }) {
				Assert.AreEqual(calls, 0, "1");
				OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				Assert.AreEqual(calls, 1, "2");
				new Thread(delegate() {
					try {
						OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
						Assert.Fail("3");
					} catch (NotSupportedException ex) {
						Assert.AreEqual(ex.Message, "This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread.", "4");
					}
					Assert.AreEqual(calls, 1, "5");
				}).Start();
			}

			protected override void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args) {
				calls++;
				base.ProcessCollectionChanged(args);
			}
		}
		#endregion

		#region OnCurrentChanged
		class OnCurrentChangedCollectionView : CollectionView {
			int calls;

			public OnCurrentChangedCollectionView()
				: base(new object[] { 1, 2 }) {
				CurrentChanged += OnCurrentChanged;
				OnCurrentChanged();
				Assert.AreEqual(calls, 1);
			}

			void OnCurrentChanged(object sender, EventArgs e) {
				calls++;
			}
		}
		#endregion

		#region OnCurrentChanging
		[Test]
		public void OnCurrentChanging() {
			new OnCurrentChangingCollectionView();
		}

		class OnCurrentChangingCollectionView : CollectionView {
			int calls;
			int on_calls;

			public OnCurrentChangingCollectionView()
				: base(new object[] { 1, 2 }) {
				CurrentChanging += OnCurrentChanging;
				OnCurrentChanging();
				Assert.AreEqual(CurrentPosition, -1, "1");
				Assert.AreEqual(calls, 1, "2");
				Assert.AreEqual(on_calls, 1, "3");
			}

			void OnCurrentChanging(object sender, CurrentChangingEventArgs e) {
				Assert.AreEqual(CurrentPosition, -1, "3");
				Assert.IsFalse(e.IsCancelable, "4");
				calls++;
			}

			protected override void OnCurrentChanging(CurrentChangingEventArgs args) {
				base.OnCurrentChanging(args);
				on_calls++;
			}

		}
		#endregion

		#region SetCurrent
		[Test]
		public void SetCurrent() {
			new SetCurrentCollectionView();
		}

		class SetCurrentCollectionView : CollectionView {
			public SetCurrentCollectionView()
				: base(new object[] { 1, 2 }) {
				SetCurrent(1, 0);
				Assert.AreEqual(CurrentItem, 1, "1");
				Assert.AreEqual(CurrentPosition, 0, "2");
				SetCurrent(2, 1);
				Assert.AreEqual(CurrentItem, 2, "3");
				Assert.AreEqual(CurrentPosition, 1, "4");
				SetCurrent(null, -1);
				Assert.AreEqual(CurrentItem, null, "5");
				Assert.AreEqual(CurrentPosition, -1, "6");
				SetCurrent(3, 2);
				Assert.AreEqual(CurrentItem, 3, "6");
				Assert.AreEqual(CurrentPosition, 2, "7");
			}
		}
		#endregion

		#region CurrentChanged
		[Test]
		public void CurrentChanged() {
			new CurrentChangedCollectionView();
		}

		class CurrentChangedCollectionView : CollectionView {
			int calls;

			public CurrentChangedCollectionView()
				: base(new object[] { 1, 2 }) {
				CurrentChanged += OnCurrentChanged;
				MoveCurrentToNext();
				Assert.AreEqual(calls, 1, "1");
				MoveCurrentToNext();
				Assert.AreEqual(calls, 2, "2");
				MoveCurrentToNext();
				Assert.AreEqual(calls, 2, "3");
				MoveCurrentTo(1);
				Assert.AreEqual(calls, 3, "4");
				MoveCurrentTo(1);
				Assert.AreEqual(calls, 3, "4");
			}

			void OnCurrentChanged(object sender, EventArgs e) {
				calls++;
			}
		}
		#endregion

		#region IEnumerable.GetEnumerator
		[Test]
		public void IEnumerableGetEnumerator() {
			new IEnumerableGetEnumeratorCollectionView();
		}

		class IEnumerableGetEnumeratorCollectionView : CollectionView {
			int calls;
			public IEnumerableGetEnumeratorCollectionView()
				: base(new object[] { }) {
				Assert.AreEqual(calls, 0, "1");
				((IEnumerable)this).GetEnumerator();
				Assert.AreEqual(calls, 1, "2");
			}
			protected override IEnumerator GetEnumerator() {
				calls++;
				return base.GetEnumerator();
			}
		}
		#endregion
	}
}