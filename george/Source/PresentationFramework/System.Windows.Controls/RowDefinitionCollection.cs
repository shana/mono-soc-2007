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
		internal RowDefinitionCollection(Grid owner) {
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
		public RowDefinition this[int index] {
			get {
				CheckIndex(index);
				return (RowDefinition)data[index];
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
		public void Add(RowDefinition value) {
			AddAndReturnInt(value);
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
			CheckNull(value);
			CheckIfItIsInAnotherGrid(value);
			value.Grid = grid;
			data.Insert(index, value);
			OnChanged();
		}

		public bool Remove(RowDefinition value) {
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
		int AddAndReturnInt(RowDefinition value) {
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

		void CheckIfItIsInAnotherGrid(RowDefinition value) {
			if (value.Grid != null)
				throw new ArgumentException("'value' already belongs to another 'RowDefinitionCollection'.");
		}

		RowDefinition CheckType(object value) {
			RowDefinition typed_value = value as RowDefinition;
			if (typed_value == null)
				throw new ArgumentException("'RowDefinitionCollection' must be type 'RowDefinition'.");
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

		void IList.Remove(object value) {
			CheckNull(value);
			Remove(CheckType(value));
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
		#endregion

		#region IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return data.GetEnumerator();
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