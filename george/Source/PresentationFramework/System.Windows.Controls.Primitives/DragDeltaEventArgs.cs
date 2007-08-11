//FIXME: This causes test to fail.
/*
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	public class DragDeltaEventArgs : RoutedEventArgs {
		#region Private Fields
		double horizontalChange;
		double verticalChange;
		#endregion

		#region Public Constructors
		public DragDeltaEventArgs(double horizontalChange, double verticalChange) {
			this.horizontalChange = horizontalChange;
			this.verticalChange = verticalChange;
		}
		#endregion

		#region Public Properties
		public double HorizontalChange {
			get { return horizontalChange; }
		}

		public double VerticalChange {
			get { return verticalChange; }
		}
		#endregion

		#region Protected Methods
		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) {
			((DragDeltaEventHandler)genericHandler).Invoke(genericTarget, this);
		}
		#endregion
	}
}
*/