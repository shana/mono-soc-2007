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
		Grid grid;
		#endregion

		#region Internal Constructors
		internal RowDefinitionCollection(Grid grid) {
			this.grid = grid;
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
				data[index] = value;
				OnChanged();
			}
		}
		#endregion

		#region Public Methods
		public void Add(RowDefinition value) {
			data.Add(value);
			OnChanged();
		}

		public void Clear() {
			data.Clear();
			OnChanged();
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
			data.Insert(index, value);
			OnChanged();
		}

		public bool Remove(RowDefinition value) {
			if (!data.Contains(value))
				return false;
			try {
				data.Remove(value);
				OnChanged();
				return true;
			} catch (NotSupportedException) {
				return false;
			}
		}

		public void RemoveAt(int index) {
			data.RemoveAt(index);
			OnChanged();
		}

		public void RemoveRange(int index, int count) {
			data.RemoveRange(index, count);
			OnChanged();
		}
		#endregion

		#region Explicit Interface Implementations
		#region IList
		int IList.Add(object value) {
			int result = data.Add(value);
			OnChanged();
			return result;
		}

		void IList.Clear() {
			data.Clear();
			OnChanged();
		}

		bool IList.Contains(object value) {
			return data.Contains(value);
		}

		int IList.IndexOf(object value) {
			return data.IndexOf(value);
		}

		void IList.Insert(int index, object value) {
			data.Insert(index, value);
			OnChanged();
		}

		bool IList.IsFixedSize {
			get { return data.IsFixedSize; }
		}

		bool IList.IsReadOnly {
			get { return data.IsReadOnly; }
		}

		void IList.Remove(object value) {
			data.Remove(value);
			OnChanged();
		}

		void IList.RemoveAt(int index) {
			data.RemoveAt(index);
			OnChanged();
		}

		object IList.this[int index] {
			get { return data[index]; }
			set {
				data[index] = value;
				OnChanged();
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
			data.Insert(index, item);
			OnChanged();
		}

		void IList<RowDefinition>.RemoveAt(int index) {
			data.RemoveAt(index);
			OnChanged();
		}

		RowDefinition IList<RowDefinition>.this[int index] {
			get { return (RowDefinition)data[index]; }
			set {
				data[index] = value;
				OnChanged();
			}
		}
		#endregion

		#region ICollection<RowDefinition>
		void ICollection<RowDefinition>.Add(RowDefinition item) {
			data.Add(item);
			OnChanged();
		}

		void ICollection<RowDefinition>.Clear() {
			data.Clear();
			OnChanged();
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
			bool result = Remove(item);
			OnChanged();
			return result;
		}
		#endregion

		#region IEnumerable<RowDefinition>
		IEnumerator<RowDefinition> IEnumerable<RowDefinition>.GetEnumerator() {
			return new RowDefinitionEnumerator(data.GetEnumerator());
		}
		#endregion
		#endregion

		#region Private Methods
		void OnChanged() {
			foreach (RowDefinition item in data)
				item.ActualHeight = 0;
			grid.InvalidateMeasure();
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