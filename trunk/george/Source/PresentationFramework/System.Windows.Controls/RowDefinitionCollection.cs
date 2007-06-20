using System.Collections;
using System.Collections.Generic;
#if Implementation
using System;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public sealed class RowDefinitionCollection : IList<RowDefinition>, ICollection<RowDefinition>, IEnumerable<RowDefinition>, IList, ICollection, IEnumerable {
		#region Private Fields
		ArrayList data = new ArrayList();
		#endregion

		#region Internal Constructors
		internal RowDefinitionCollection() {
		}
		#endregion

		#region Public Properties
		public int Count {
			get { return data.Count; }
		}

		public bool IsReadOnly {
			get { return data.IsReadOnly; }
		}

		public bool IsSyncronized {
			get { return data.IsSynchronized; }
		}

		public object SyncRoot {
			get { return data.SyncRoot; }
		}
		#endregion

		#region Public Indexers
		public RowDefinition this[int index] {
			get { return (RowDefinition)data[index]; }
			set {
				ResetActualSize();
				data[index] = value; 
			}
		}
		#endregion

		#region Public Methods
		public void Add(RowDefinition value) {
			ResetActualSize();
			data.Add(value);
		}

		public void Clear() {
			data.Clear();
			ResetActualSize();
		}

		public bool Contains(RowDefinition value) {
			return data.Contains(value);
		}

		public void CopyTo(RowDefinition[] array, int index) {
			data.CopyTo(array, index);
		}

		public int IndexOf(RowDefinition value) {
			return data.IndexOf(value);
		}

		public void Insert(int index, RowDefinition value) {
			ResetActualSize();
			data.Insert(index, value);
		}

		public bool Remove(RowDefinition value) {
			if (!data.Contains(value))
				return false;
			ResetActualSize();
			try {
				data.Remove(value);
				return true;
			} catch (NotSupportedException) {
				return false;
			}
		}

		public void RemoveAt(int index) {
			ResetActualSize();
			data.RemoveAt(index);
		}

		public void RemoveRange(int index, int count) {
			ResetActualSize();
			data.RemoveRange(index, count);
		}
		#endregion

		#region Explicit Interface Implementations
		#region IList
		int IList.Add(object value) {
			ResetActualSize();
			return data.Add(value);
		}

		void IList.Clear() {
			ResetActualSize();
			data.Clear();
		}

		bool IList.Contains(object value) {
			return data.Contains(value);
		}

		int IList.IndexOf(object value) {
			return data.IndexOf(value);
		}

		void IList.Insert(int index, object value) {
			ResetActualSize();
			data.Insert(index, value);
		}

		bool IList.IsFixedSize {
			get { return data.IsFixedSize; }
		}

		bool IList.IsReadOnly {
			get { return data.IsReadOnly; }
		}

		void IList.Remove(object value) {
			ResetActualSize();
			data.Remove(value);
		}

		void IList.RemoveAt(int index) {
			ResetActualSize();
			data.RemoveAt(index);
		}

		object IList.this[int index] {
			get { return data[index]; }
			set {
				ResetActualSize();
				data[index] = value; 
			}
		}
		#endregion

		#region ICollection
		void ICollection.CopyTo(Array array, int index) {
			data.CopyTo(array, index);
		}

		int ICollection.Count {
			get { return data.Count; }
		}

		bool ICollection.IsSynchronized {
			get { return data.IsSynchronized; }
		}

		object ICollection.SyncRoot {
			get { return data.SyncRoot; }
		}
		#endregion

		#region IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return data.GetEnumerator();
		}
		#endregion

		#region IList<RowDefinition>
		int IList<RowDefinition>.IndexOf(RowDefinition item) {
			return data.IndexOf(item);
		}

		void IList<RowDefinition>.Insert(int index, RowDefinition item) {
			ResetActualSize();
			data.Insert(index, item);
		}

		void IList<RowDefinition>.RemoveAt(int index) {
			ResetActualSize();
			data.RemoveAt(index);
		}

		RowDefinition IList<RowDefinition>.this[int index] {
			get { return (RowDefinition)data[index]; }
			set {
				ResetActualSize();
				data[index] = value; 
			}
		}
		#endregion

		#region ICollection<RowDefinition>
		void ICollection<RowDefinition>.Add(RowDefinition item) {
			ResetActualSize();
			data.Add(item);
		}

		void ICollection<RowDefinition>.Clear() {
			ResetActualSize();
			data.Clear();
		}

		bool ICollection<RowDefinition>.Contains(RowDefinition item) {
			return data.Contains(item);
		}

		void ICollection<RowDefinition>.CopyTo(RowDefinition[] array, int arrayIndex) {
			data.CopyTo(array, arrayIndex);
		}

		int ICollection<RowDefinition>.Count {
			get { return data.Count; }
		}

		bool ICollection<RowDefinition>.IsReadOnly {
			get { return data.IsReadOnly; }
		}

		bool ICollection<RowDefinition>.Remove(RowDefinition item) {
			ResetActualSize();
			return Remove(item);
		}
		#endregion

		#region IEnumerable<RowDefinition>
		IEnumerator<RowDefinition> IEnumerable<RowDefinition>.GetEnumerator() {
			return new RowDefinitionEnumerator(data.GetEnumerator());
		}
		#endregion
		#endregion

		#region Private Methods
		void ResetActualSize() {
			foreach (RowDefinition item in data)
				item.ActualHeight = 0;
		}
		#endregion

		#region Classes
		class RowDefinitionEnumerator : IEnumerator<RowDefinition> {
			#region Private Fields
			IEnumerator data;
			#endregion

			#region Public Constructors
			public RowDefinitionEnumerator(IEnumerator data) {
				this.data = data;
			}
			#endregion

			#region Explicit Interface Implementations
			#region IEnumerator<RowDefinition>
			RowDefinition IEnumerator<RowDefinition>.Current {
				get { return (RowDefinition)data.Current; }
			}
			#endregion

			#region IDisposable
			void IDisposable.Dispose() {
			}
			#endregion

			#region IEnumerator
			object IEnumerator.Current {
				get { return data.Current; }
			}

			bool IEnumerator.MoveNext() {
				return data.MoveNext();
			}

			void IEnumerator.Reset() {
				data.Reset();
			}
			#endregion
			#endregion
		}
		#endregion
	}
}