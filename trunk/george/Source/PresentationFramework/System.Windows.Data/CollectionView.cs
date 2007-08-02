using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
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
		bool is_current_after_last;
		bool is_current_before_first;
		IEnumerable source_collection;
		bool is_refresh_deferred;
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
				is_current_after_last = false;
				is_current_before_first = false;
			} else {
				is_current_after_last = true;
				is_current_before_first = true;
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
			get {
				VerifyRefreshNotDeferred();
				return current_item; 
			}
		}

		public virtual int CurrentPosition {
			get {
				VerifyRefreshNotDeferred();
				return current_position; 
			}
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
			get { return is_current_after_last; }
		}

		public virtual bool IsCurrentBeforeFirst {
			get { return is_current_before_first; }
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
			get { return is_refresh_deferred; }
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
			return new DeferHelper(this);
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
			VerifyRefreshNotDeferred();
			if (object.Equals(current_item, item))
				return true;
			int current_collection_item_index = 0;
			foreach (object collection_item in source_collection) {
				if (object.Equals(item, collection_item))
					return MoveCurrentToPosition(current_collection_item_index);
				current_collection_item_index++;
			}
			return MoveCurrentToPosition(-1);
		}

		public virtual bool MoveCurrentToFirst() {
			VerifyRefreshNotDeferred();
			current_position = 0;
			IEnumerator enumerator = source_collection.GetEnumerator();
			if (enumerator.MoveNext()) {
				current_item = enumerator.Current;
				is_current_after_last = false;
				is_current_before_first = false;
				return true;
			} else {
				current_item = null;
				is_current_after_last = true;
				is_current_before_first = true;
				return false;
			}
		}

		public virtual bool MoveCurrentToLast() {
			VerifyRefreshNotDeferred();
			current_item = null;
			current_position = -1;
			foreach (object item in source_collection) {
				current_item = item;
				current_position++;
			}
			is_current_after_last = current_position == -1;
			is_current_before_first = is_current_after_last;
			return !is_current_after_last;
		}

		public virtual bool MoveCurrentToNext() {
			VerifyRefreshNotDeferred();
			return is_current_after_last ? false : MoveCurrentToPosition(current_position + 1);
		}

		public virtual bool MoveCurrentToPosition(int position) {
			VerifyRefreshNotDeferred();
			CurrentChangingEventArgs e;
			is_current_after_last = false;
			is_current_before_first = false;
			if (position < 0) {
				e = new CurrentChangingEventArgs();
				OnCurrentChanging(e);
				if (e.Cancel)
					return true;
				current_position = -1;
				current_item = null;
				is_current_before_first = true;
				OnCurrentChanged();
				return false;
			}
			current_position = 0;
			foreach (object item in source_collection) {
				if (current_position == position) {
					e = new CurrentChangingEventArgs();
					OnCurrentChanging(e);
					if (e.Cancel)
						return true;
					current_item = item;
					OnCurrentChanged();
					return true;
				}
				current_position++;
			}
			if (position <= current_position) {
				e = new CurrentChangingEventArgs();
				OnCurrentChanging(e);
				if (e.Cancel)
					return true;
			}
			current_item = null;
			is_current_after_last = true;
			if (position > current_position)
				throw new ArgumentOutOfRangeException("position");
			OnCurrentChanged();
			return false;
		}

		public virtual bool MoveCurrentToPrevious() {
			VerifyRefreshNotDeferred();
			return is_current_before_first ? false : MoveCurrentToPosition(current_position - 1);
		}

		public virtual bool PassesFilter(object item) {
			return filter == null ? true : filter(item);
		}

		public virtual void Refresh() {
			RefreshOverride();
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
			//LAMESPEC?: Documentation says it should throw ArgumentNullException if args is null. It seems it does not.
		}

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args) {
			if (CollectionChanged != null)
				CollectionChanged(this, args);
		}

		protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
			//FIXME
			//LAMESPEC?
			if (Dispatcher.Thread != Thread.CurrentThread)
				throw new NotSupportedException("This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread.");
			ProcessCollectionChanged(args);
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
			is_current_before_first = current_position < 0;
			is_current_after_last = current_position >= Count;
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
			return GetEnumerator();
		}
		#endregion

		#region INotifyCollectionChanged
		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged {
			add { CollectionChanged += value; }
			remove { CollectionChanged -= value; }
		}
		#endregion

		#region INotifyPropertyChanged
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { PropertyChanged += value; }
			remove { PropertyChanged -= value; }
		}
		#endregion
		#endregion

		#region Private Methods
		void VerifyRefreshNotDeferred() {
			if (is_refresh_deferred)
				throw new InvalidOperationException("Cannot change or check the contents or Current position of CollectionView while Refresh is being deferred.");
		}
		#endregion

		#region Private Classes
		class DeferHelper : IDisposable {
			CollectionView owner;

			public DeferHelper(CollectionView owner) {
				this.owner = owner;
				owner.is_refresh_deferred = true;
			}

			public void Dispose() {
				owner.is_refresh_deferred = false;
				owner.Refresh();
			}
		}
		#endregion
	}
}