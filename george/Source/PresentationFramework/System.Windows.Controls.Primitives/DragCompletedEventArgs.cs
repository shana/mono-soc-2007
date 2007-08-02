#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	public class DragCompletedEventArgs : RoutedEventArgs {
		#region Private Fields
		bool canceled;
		double horizontalChange;
		double verticalChange;
		#endregion

		#region Public Constructors
		public DragCompletedEventArgs(double horizontalChange, double verticalChange, bool canceled) {
			this.canceled = canceled;
			this.horizontalChange = horizontalChange;
			this.verticalChange = verticalChange;
		}
		#endregion

		#region Public Properties
		public bool Canceled {
			get { return canceled; }
		}

		public double HorizontalChange {
			get { return horizontalChange; }
		}

		public double VerticalChange {
			get { return verticalChange; }
		}
		#endregion

		#region Protected Methods
		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) {
			((DragCompletedEventHandler)genericHandler).Invoke(genericTarget, this);
		}
		#endregion
	}
}