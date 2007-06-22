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
		Grid grid;
		#endregion

		#region Internal Constructors
		internal ColumnDefinitionCollection(Grid owner) {
			grid = owner;
		}
		#endregion

		#region Public Properties
		public int Count {
			get { return data.Count; }
		}

		public bool IsReadOnly {
			get { return data.IsReadOnly; }
		}

		public bool IsSynchronized {
			get { return data.IsSynchronized; }
		}

		public object SyncRoot {
			get { return data.SyncRoot; }
		}
		#endregion

		#region Public Indexers
		public ColumnDefinition this[int index] {
			get {
				CheckIndex(index);
				return (ColumnDefinition)data[index]; 
			}
			set {
				CheckNull(value);
				CheckIfItIsInAnotherGrid(value);
				CheckIndex(index);
				data[index] = value;
				OnChanged();
			}
		}
		#endregion

		#region Public Methods
		public void Add(ColumnDefinition value) {
			AddAndReturnInt(value);
		}

		public void Clear() {
			data.Clear();
			OnChanged();
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
			CheckNull(value);
			CheckIfItIsInAnotherGrid(value);
			value.Grid = grid;
			data.Insert(index, value);
			OnChanged();
		}

		public bool Remove(ColumnDefinition value) {
			if (!data.Contains(value))
				return false;
			try {
				value.Grid = null;
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

		#region Private Methods
		int AddAndReturnInt(ColumnDefinition value) {
			CheckIfItIsInAnotherGrid(value);
			value.Grid = grid;
			int result = data.Add(value);
			OnChanged();
			return result;
		}

		void CheckIndex(int index) {
			if (index < 0 || index >= data.Count)
				throw new ArgumentOutOfRangeException("Index is out of collection's boundary.");
		}

		void CheckIfItIsInAnotherGrid(ColumnDefinition value) {
			if (value.Grid != null)
				throw new ArgumentException("'value' already belongs to another 'ColumnDefinitionCollection'.");
		}

		ColumnDefinition CheckType(object value) {
			ColumnDefinition typed_value = value as ColumnDefinition;
			if (typed_value == null)
				throw new ArgumentException("'ColumnDefinitionCollection' must be type 'ColumnDefinition'.");
			return typed_value;
		}

		void CheckNull(object value) {
			if (value == null)
				throw new ArgumentNullException();
		}
		#endregion

		#region Explicit Interface Implementations
		#region IList
		int IList.Add(object value) {
			CheckNull(value);
			return AddAndReturnInt(CheckType(value));
		}

		void IList.Clear() {
			Clear();
		}

		bool IList.Contains(object value) {
			return Contains(CheckType(value));
		}

		int IList.IndexOf(object value) {
			return IndexOf(CheckType(value));
		}

		void IList.Insert(int index, object value) {
			CheckNull(value);
			Insert(index, CheckType(value));
		}

		bool IList.IsFixedSize {
			get { return data.IsFixedSize; }
		}

		bool IList.IsReadOnly {
			get { return IsReadOnly; }
		}

		void IList.Remove(object value) {
			CheckNull(value);
			Remove(CheckType(value));
		}

		void IList.RemoveAt(int index) {
			RemoveAt(index);
		}

		object IList.this[int index] {
			get { return this[index]; }
			set {
				CheckNull(value);
				this[index] = CheckType(value); 
			}
		}
		#endregion

		#region ICollection
		void ICollection.CopyTo(Array array, int index) {
			data.CopyTo(array, index);
		}

		int ICollection.Count {
			get { return Count; }
		}

		bool ICollection.IsSynchronized {
			get { return IsSynchronized; }
		}

		object ICollection.SyncRoot {
			get { return SyncRoot; }
		}
		#endregion

		#region IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return data.GetEnumerator();
		}
		#endregion

		#region IList<ColumnDefinition>
		int IList<ColumnDefinition>.IndexOf(ColumnDefinition item) {
			return IndexOf(item);
		}

		void IList<ColumnDefinition>.Insert(int index, ColumnDefinition item) {
			Insert(index, item);
		}

		void IList<ColumnDefinition>.RemoveAt(int index) {
			RemoveAt(index);
		}

		ColumnDefinition IList<ColumnDefinition>.this[int index] {
			get { return this[index]; }
			set { this[index] = value; }
		}
		#endregion

		#region ICollection<ColumnDefinition>
		void ICollection<ColumnDefinition>.Add(ColumnDefinition item) {
			Add(item);
		}

		void ICollection<ColumnDefinition>.Clear() {
			Clear();
		}

		bool ICollection<ColumnDefinition>.Contains(ColumnDefinition item) {
			return Contains(item);
		}

		void ICollection<ColumnDefinition>.CopyTo(ColumnDefinition[] array, int arrayIndex) {
			CopyTo(array, arrayIndex);
		}

		int ICollection<ColumnDefinition>.Count {
			get { return Count; }
		}

		bool ICollection<ColumnDefinition>.IsReadOnly {
			get { return IsReadOnly; }
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

		#region Private Methods
		void OnChanged() {
			foreach (ColumnDefinition item in data)
				item.ActualWidth = 0;
			grid.InvalidateMeasure();
		}
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