using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Threading;
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Data {
#else
namespace System.Windows.Data {
#endif
	public class CollectionView : DispatcherObject, ICollectionView, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged {
		#region Private Fields
		const int CachedCountNotComputed = -1;
		int cached_count = CachedCountNotComputed;
		CultureInfo culture;
		object current_item;
		int current_position = -1;
		Predicate<Object> filter;
		bool? cached_is_empty;
		IEnumerable source_collection;
		#endregion

		#region Public Constructors
		public CollectionView(IEnumerable collection) {
			if (collection == null)
				throw new ArgumentNullException("collection");
			this.source_collection = collection;

			IEnumerator enumerator = collection.GetEnumerator();
			if (enumerator.MoveNext()) {
				current_item = enumerator.Current;
				current_position = 0;
			}
		}
		#endregion

		#region Public Properties
		public virtual bool CanFilter {
			get { return true; }
		}

		public virtual bool CanGroup {
			get { return false; }
		}

		public virtual bool CanSort {
			get { return false; }
		}
		
		public virtual IComparer Comparer {
			get { return null; }
		}

		public virtual int Count {
			get {
				if (cached_count == CachedCountNotComputed) {
					cached_count = 0;
					IEnumerator enumerator = source_collection.GetEnumerator();
					while (enumerator.MoveNext()) {
						object dummy = enumerator.Current;
						cached_count++;
					}
				}
				return cached_count;
			}
		}

		[TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
		public virtual CultureInfo Culture {
			get { return culture; }
			set { culture = value; }
		}

		public virtual Object CurrentItem {
			get { return current_item; }
		}

		public virtual int CurrentPosition {
			get { return current_position; }
		}

		public virtual Predicate<Object> Filter {
			get { return filter; }
			set {
				if (!CanFilter)
					throw new NotSupportedException();
				filter = value; 
			}
		}

		public virtual ObservableCollection<GroupDescription> GroupDescriptions {
			get { return null; }
		}

		public virtual ReadOnlyObservableCollection<Object> Groups {
			get { return null; }
		}

		public virtual bool IsCurrentAfterLast {
			get { 
				//WDTDH
				return true; 
			}
		}

		public virtual bool IsCurrentBeforeFirst {
			get {
				//WDTDH
				return true; 
			}
		}

		public virtual bool IsEmpty {
			get {
				if (!cached_is_empty.HasValue) {
					IEnumerator enumerator = source_collection.GetEnumerator();
					cached_is_empty = !enumerator.MoveNext();
				}
				return cached_is_empty.Value; 
			}
		}

		public virtual bool NeedsRefresh {
			get {
				//WDTDH
				return true;
			}
		}

		public virtual SortDescriptionCollection SortDescriptions {
			get { return SortDescriptionCollection.Empty; }
		}

		public virtual IEnumerable SourceCollection {
			get { return source_collection; }
		}
		#endregion

		#region Protected Properties
		protected bool IsCurrentInSync {
			get {
				//WDTDH
				return false;
			}
		}

		protected bool IsDynamic {
			get {
				//WDTDH
				return false;
			}
		}

		protected bool IsRefreshDeferred {
			get {
				//WDTDH
				return false;
			}
		}

		protected bool UpdatedOutsideDispatcher {
			get {
				//WDTDH
				return false;
			}
		}
		#endregion

		#region Public Methods
		public virtual bool Contains(object item) {
			if (item == null)
				return false;
			if (!PassesFilter(item))
				return false;
			foreach (object collection_item in source_collection)
				if (item.Equals(collection_item))
					return true;
			return false;
		}

		public virtual IDisposable DeferRefresh() {
			//WDTDH
			return null;
		}

		public virtual Object GetItemAt(int index) {
			if (index < 0)
				throw new ArgumentOutOfRangeException("index");
			int current_collection_item_index = 0;
			foreach (object collection_item in source_collection) {
				if (current_collection_item_index == index)
					return collection_item;
				current_collection_item_index++;
			}
			throw new IndexOutOfRangeException();
		}

		public virtual int IndexOf(object item) {
			int current_collection_item_index = 0;
			foreach (object collection_item in source_collection) {
				if (!PassesFilter(collection_item))
					continue;
				if (object.Equals(item, collection_item))
					return current_collection_item_index;
				current_collection_item_index++;
			}
			return -1;
		}

		public virtual bool MoveCurrentTo(object item) {
			int current_collection_item_index = 0;
			foreach (object collection_item in source_collection) {
				if (object.Equals(item, collection_item)) {
					current_item = item;
					current_position = current_collection_item_index;
					return true;
				}
				current_collection_item_index++;
			}
			current_item = null;
			current_position = -1;
			return false;
		}

		public virtual bool MoveCurrentToFirst() {
			current_position = 0;
			IEnumerator enumerator = source_collection.GetEnumerator();
			if (enumerator.MoveNext()) {
				current_item = enumerator.Current;
				return true;
			} else {
				current_item = null;
				return false;
			}
		}

		public virtual bool MoveCurrentToLast() {
			current_item = null;
			current_position = -1;
			foreach(object item in source_collection) {
				current_item = item;
				current_position++;
			}
			return current_position != -1;
		}

		public virtual bool MoveCurrentToNext() {
			bool passed_current_item = false;
			current_position = 0;
			foreach (object item in source_collection) {
				if (passed_current_item) {
					current_item = item;
					return true;
				}
				passed_current_item = object.Equals(current_item, item);
				current_position++;
			}
			current_item = null;
			return false;
		}

		public virtual bool MoveCurrentToPosition(int position) {
			current_position = 0;
			foreach (object item in source_collection) {
				if (current_position == position) {
					current_item = item;
					return true;
				}
				current_position++;
			}
			current_item = null;
			if (position > current_position)
				throw new ArgumentOutOfRangeException("position");
			return false;
		}

		public virtual bool MoveCurrentToPrevious() {
			object previous_item = null;
			int previous_position = -1;
			foreach (object item in source_collection) {
				if (object.Equals(item, current_item)) {
					current_item = previous_item;
					current_position = previous_position;
					return current_position != -1;
				}
				previous_item = item;
				previous_position++;
			}
			return false;
		}

		public virtual bool PassesFilter(object item) {
			return filter == null ? true : filter(item);
		}

		public virtual void Refresh() {
			//WDTDH
		}
		#endregion

		#region Protected Methods
		protected void ClearChangeLog() {
			//WDTDH
		}

		protected virtual IEnumerator GetEnumerator() {
			return SourceCollection.GetEnumerator();
		}

		protected bool OKToChangeCurrent() {
			//WDTDH
			return true;
		}

		protected virtual void OnBeginChangeLogging(NotifyCollectionChangedEventArgs args) {
			//WDTDH
		}

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args) {
			if (CollectionChanged != null)
				CollectionChanged(this, args);
		}

		protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
			//WDTDH
		}

		protected virtual void OnCurrentChanged() {
			if (CurrentChanged != null)
				CurrentChanged(this, EventArgs.Empty);
		}

		protected void OnCurrentChanging() {
			current_position = -1;
			OnCurrentChanging(new CurrentChangingEventArgs(false));
		}

		protected virtual void OnCurrentChanging(CurrentChangingEventArgs args) {
			if (CurrentChanging != null)
				CurrentChanging(this, args);
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}

		protected virtual void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args) {
			//WDTDH
		}

		protected void RefreshOrDefer() {
			//WDTDH
		}

		protected virtual void RefreshOverride() {
			//WDTDH
		}

		protected void SetCurrent (object newItem, int newPosition) {
			current_item = newItem;
			current_position = newPosition;
		}
		#endregion

		#region Public Events
		public virtual event EventHandler CurrentChanged;
		public virtual event CurrentChangingEventHandler CurrentChanging;
		#endregion

		#region Protected Events
		protected virtual event NotifyCollectionChangedEventHandler CollectionChanged;
		protected virtual event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Explicit Interface Implementations
		#region IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			//WDTDH
			return null;
		}
		#endregion

		#region INotifyCollectionChanged
		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged {
			add {
				//WDTDH
			}
			remove {
				//WDTDH
			}
		}
		#endregion

		#region INotifyPropertyChanged
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add {
				//WDTDH
			}
			remove {
				//WDTDH
			}
		}
		#endregion
		#endregion
	}
}