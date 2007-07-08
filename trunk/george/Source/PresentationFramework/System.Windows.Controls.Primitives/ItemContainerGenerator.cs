#if Implementation
using System;
using System.Windows;
using Mono.System.Windows.Controls.Primitives;
namespace Mono.System.Windows.Controls {
#else
using System.Windows.Controls.Primitives;
namespace System.Windows.Controls {
#endif
	public sealed class ItemContainerGenerator : IItemContainerGenerator, IWeakEventListener {
		#region Public Properties
		public GeneratorStatus Status {
			get {
				//WDTDH
				return GeneratorStatus.Error;
			}
		}
		#endregion

		#region Public Methods
		public DependencyObject ContainerFromIndex(int index) {
			//WDTDH
			return null;
		}

		public DependencyObject ContainerFromItem(object item) {
			//WDTDH
			return null;
		}

		public int IndexFromContainer(DependencyObject container) {
			//WDTDH
			return 0;
		}

		public object ItemFromContainer(DependencyObject container) {
			//WDTDH
			return null;
		}
		#endregion

		#region Public Events
		public event ItemsChangedEventHandler ItemsChanged;
		public event EventHandler StatusChanged;
		#endregion

		#region Explicit Interface Implementations
		#region IItemContainerGenerator
		DependencyObject IItemContainerGenerator.GenerateNext() {
			throw new Exception("The method or operation is not implemented.");
		}

		DependencyObject IItemContainerGenerator.GenerateNext(out bool isNewlyRealized) {
			throw new Exception("The method or operation is not implemented.");
		}

		GeneratorPosition IItemContainerGenerator.GeneratorPositionFromIndex(int itemIndex) {
			throw new Exception("The method or operation is not implemented.");
		}

		ItemContainerGenerator IItemContainerGenerator.GetItemContainerGeneratorForPanel(Panel panel) {
			throw new Exception("The method or operation is not implemented.");
		}

		int IItemContainerGenerator.IndexFromGeneratorPosition(GeneratorPosition position) {
			throw new Exception("The method or operation is not implemented.");
		}

		void IItemContainerGenerator.PrepareItemContainer(DependencyObject container) {
			throw new Exception("The method or operation is not implemented.");
		}

		void IItemContainerGenerator.Remove(GeneratorPosition position, int count) {
			throw new Exception("The method or operation is not implemented.");
		}

		void IItemContainerGenerator.RemoveAll() {
			throw new Exception("The method or operation is not implemented.");
		}

		IDisposable IItemContainerGenerator.StartAt(GeneratorPosition position, GeneratorDirection direction) {
			throw new Exception("The method or operation is not implemented.");
		}

		IDisposable IItemContainerGenerator.StartAt(GeneratorPosition position, GeneratorDirection direction, bool allowStartAtRealizedItem) {
			throw new Exception("The method or operation is not implemented.");
		}
		#endregion

		#region IWeakEventListener
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			throw new Exception("The method or operation is not implemented.");
		}
		#endregion
		#endregion
	}
}