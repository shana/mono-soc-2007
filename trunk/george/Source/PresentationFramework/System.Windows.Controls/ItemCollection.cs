using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[Localizability(LocalizationCategory.Ignore)]
	public sealed class ItemCollection : Mono.System.Windows.Data.CollectionView, IList, ICollection, IEnumerable, IWeakEventListener {
		#region Internal Fields
		internal IEnumerable source_collection;
		#endregion

		#region Private Fields
		ArrayList data;
		ObservableCollection<GroupDescription> group_descriptions = new ObservableCollection<GroupDescription>();
		Predicate<object> filter;
		ArrayList filtered_data;
		#endregion

		#region Internal Constructors
		internal ItemCollection(ItemsControl owner)
			: base(owner.items_data) {
			data = owner.items_data;
			source_collection = this;
		}
		#endregion

		#region Public Properties
		public override bool CanFilter {
			get { return true; }
		}

		public override bool CanGroup {
			get { return false; }
		}

		public override bool CanSort {
			get { return true; }
		}

		public override int Count {
			get {
				return filter == null ? data.Count : filtered_data.Count;
			}
		}

		public override object CurrentItem {
			get {
				//WDTDH
				return base.CurrentItem;
			}
		}

		public override int CurrentPosition {
			get {
				//WDTDH
				return base.CurrentPosition;
			}
		}

		public override Predicate<object> Filter {
			get { return filter; }
			set {
				if (filter == value)
					return;
				filter = value;
				if (filter == null)
					filtered_data = null;
				else {
					filtered_data = new ArrayList();
					foreach (object item in data)
						if (filter(item))
							filtered_data.Add(item);
				}
			}
		}

		public override ObservableCollection<GroupDescription> GroupDescriptions {
			get { return group_descriptions; }
		}

		public override ReadOnlyObservableCollection<object> Groups {
			get {
				//WDTDH
				return base.Groups;
			}
		}

		public override bool IsCurrentAfterLast {
			get {
				//WDTDH
				return base.IsCurrentAfterLast;
			}
		}

		public override bool IsCurrentBeforeFirst {
			get {
				//WDTDH
				return base.IsCurrentBeforeFirst;
			}
		}

		public override bool IsEmpty {
			get {
				//WDTDH
				return base.IsEmpty;
			}
		}

		public override bool NeedsRefresh {
			get { return false; }
		}

		public override SortDescriptionCollection SortDescriptions {
			get {
				//WDTDH
				return base.SortDescriptions;
			}
		}

		public override IEnumerable SourceCollection {
			get { return source_collection; }
		}
		#endregion

		#region Public Indexers
		public object this[int index] {
			get { return data[index]; }
			set { data[index] = value; }
		}
		#endregion

		#region Public Methods
		public int Add(object newItem) {
			if (filter != null && filter(newItem))
				filtered_data.Add(newItem);
			return data.Add(newItem);
		}

		public void Clear() {
			filtered_data.Clear();
			data.Clear();
		}

		public override bool Contains(object item) {
			return data.Contains(item);
		}

		public void CopyTo(Array array, int index) {
			data.CopyTo(array, index);
		}

		public override IDisposable DeferRefresh() {
			//WDTDH
			return base.DeferRefresh();
		}

		public override object GetItemAt(int index) {
			//WDTDH
			return base.GetItemAt(index);
		}

		public override int IndexOf(object item) {
			return data.IndexOf(item);
		}

		public void Insert (int insertIndex, object insertItem) {
			data.Insert(insertIndex, insertItem);
		}

		public override bool MoveCurrentTo(object item) {
			//WDTDH
			return base.MoveCurrentTo(item);
		}

		public override bool MoveCurrentToFirst() {
			//WDTDH
			return base.MoveCurrentToFirst();
		}

		public override bool MoveCurrentToLast() {
			//WDTDH
			return base.MoveCurrentToLast();
		}

		public override bool MoveCurrentToNext() {
			//WDTDH
			return base.MoveCurrentToNext();
		}

		public override bool MoveCurrentToPosition(int position) {
			//WDTDH
			return base.MoveCurrentToPosition(position);
		}

		public override bool MoveCurrentToPrevious() {
			//WDTDH
			return base.MoveCurrentToPrevious();
		}

		public override bool PassesFilter(object item) {
			//WDTDH
			return base.PassesFilter(item);
		}

		public void Remove(object removeItem) {
			filtered_data.Remove(removeItem);
			data.Remove(removeItem);
		}

		public void RemoveAt(int removeIndex) {
			data.RemoveAt(removeIndex);
		}
		#endregion

		#region Explicit Interface Implementations
		#region IWeakEventListener
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return false;
		}
		#endregion

		#region ICollection
		bool ICollection.IsSynchronized {
			get { return data.IsSynchronized; }
		}

		object ICollection.SyncRoot {
			get { return data.SyncRoot; }
		}
		#endregion

		#region IList
		bool IList.IsFixedSize {
			get { return data.IsFixedSize; }
		}

		bool IList.IsReadOnly {
			get { return data.IsReadOnly; }
		}
		#endregion
		#endregion

		#region Internal Methods
		internal void SetItemsSource(IEnumerable value) {
			if (value == null) {
				data = new ArrayList();
				source_collection = this;
			} else {
				ArrayList writeable_data = new ArrayList();
				source_collection = value;
				foreach (object item in source_collection)
					writeable_data.Add(item);
				data = ArrayList.FixedSize(ArrayList.ReadOnly(writeable_data));
			}
		}

		internal IEnumerator LogicalChildren {
			get {
				return data.GetEnumerator();
			}
		}
		#endregion
	}
}