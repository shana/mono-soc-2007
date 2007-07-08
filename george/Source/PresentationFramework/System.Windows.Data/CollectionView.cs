using System.Collections;
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
	public class CollectionView /*: DispatcherObject, ICollectionView, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged*/ {
		#region Private Fields
		IEnumerable collection;
		const int CachedCountNotComputed = -1;
		int cached_count = CachedCountNotComputed;
		CultureInfo culture;
		Predicate<Object> filter;
		object current_item;
		#endregion

		#region Public Constructors
		public CollectionView(IEnumerable collection) {
			if (collection == null)
				throw new ArgumentNullException("collection");
			this.collection = collection;

			IEnumerator enumerator = collection.GetEnumerator();
			if (enumerator.MoveNext())
				current_item = enumerator.Current;
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
					IEnumerator enumerator = collection.GetEnumerator();
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

		public virtual Predicate<Object> Filter {
			get { return filter; }
			set {
				if (!CanFilter)
					throw new NotSupportedException();
				filter = value; 
			}
		}
		#endregion
	}
}