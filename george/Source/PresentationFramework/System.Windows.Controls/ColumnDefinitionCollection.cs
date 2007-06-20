using System.Collections;
using System.Collections.Generic;
#if Implementation
using System;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	public sealed class ColumnDefinitionCollection : IList<ColumnDefinition>, ICollection<ColumnDefinition>, IEnumerable<ColumnDefinition>, IList, ICollection, IEnumerable {
		#region Private Fields
		ArrayList data = new ArrayList();
		#endregion

		#region Internal Constructors
		internal ColumnDefinitionCollection() {
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
		public ColumnDefinition this[int index] {
			get { return (ColumnDefinition)data[index]; }
			set { data[index] = value; }
		}
		#endregion

		#region Public Methods
		public void Add(ColumnDefinition value) {
			data.Add(value);
		}

		public void Clear() {
			data.Clear();
		}

		public bool Contains(ColumnDefinition value) {
			return data.Contains(value);
		}

		public void CopyTo(ColumnDefinition[] array, int index) {
			data.CopyTo(array, index);
		}

		public int IndexOf(ColumnDefinition value) {
			return data.IndexOf(value);
		}

		public void Insert(int index, ColumnDefinition value) {
			data.Insert(index, value);
		}

		public bool Remove(ColumnDefinition value) {
			if (!data.Contains(value))
				return false;
			try {
				data.Remove(value);
				return true;
			} catch (NotSupportedException) {
				return false;
			}
		}

		public void RemoveAt(int index) {
			data.RemoveAt(index);
		}

		public void RemoveRange(int index, int count) {
			data.RemoveRange(index, count);
		}
		#endregion

		#region Explicit Interface Implementations
		#region IList
		int IList.Add(object value) {
			return data.Add(value);
		}

		void IList.Clear() {
			data.Clear();
		}

		bool IList.Contains(object value) {
			return data.Contains(value);
		}

		int IList.IndexOf(object value) {
			return data.IndexOf(value);
		}

		void IList.Insert(int index, object value) {
			data.Insert(index, value);
		}

		bool IList.IsFixedSize {
			get { return data.IsFixedSize; }
		}

		bool IList.IsReadOnly {
			get { return data.IsReadOnly; }
		}

		void IList.Remove(object value) {
			data.Remove(value);
		}

		void IList.RemoveAt(int index) {
			data.RemoveAt(index);
		}

		object IList.this[int index] {
			get { return data[index]; }
			set { data[index] = value; }
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

		#region IList<ColumnDefinition>
		int IList<ColumnDefinition>.IndexOf(ColumnDefinition item) {
			return data.IndexOf(item);
		}

		void IList<ColumnDefinition>.Insert(int index, ColumnDefinition item) {
			data.Insert(index, item);
		}

		void IList<ColumnDefinition>.RemoveAt(int index) {
			data.RemoveAt(index);
		}

		ColumnDefinition IList<ColumnDefinition>.this[int index] {
			get { return (ColumnDefinition)data[index]; }
			set { data[index] = value; }
		}
		#endregion

		#region ICollection<ColumnDefinition>
		void ICollection<ColumnDefinition>.Add(ColumnDefinition item) {
			data.Add(item);
		}

		void ICollection<ColumnDefinition>.Clear() {
			data.Clear();
		}

		bool ICollection<ColumnDefinition>.Contains(ColumnDefinition item) {
			return data.Contains(item);
		}

		void ICollection<ColumnDefinition>.CopyTo(ColumnDefinition[] array, int arrayIndex) {
			data.CopyTo(array, arrayIndex);
		}

		int ICollection<ColumnDefinition>.Count {
			get { return data.Count; }
		}

		bool ICollection<ColumnDefinition>.IsReadOnly {
			get { return data.IsReadOnly; }
		}

		bool ICollection<ColumnDefinition>.Remove(ColumnDefinition item) {
			return Remove(item);
		}
		#endregion

		#region IEnumerable<ColumnDefinition>
		IEnumerator<ColumnDefinition> IEnumerable<ColumnDefinition>.GetEnumerator() {
			return new ColumnDefinitionEnumerator(data.GetEnumerator());
		}
		#endregion
		#endregion

		#region Classes
		class ColumnDefinitionEnumerator : IEnumerator<ColumnDefinition> {
			#region Private Fields
			IEnumerator data;
			#endregion

			#region Public Constructors
			public ColumnDefinitionEnumerator(IEnumerator data) {
				this.data = data;
			}
			#endregion

			#region Explicit Interface Implementations
			#region IEnumerator<ColumnDefinition>
			ColumnDefinition IEnumerator<ColumnDefinition>.Current {
				get { return (ColumnDefinition)data.Current; }
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