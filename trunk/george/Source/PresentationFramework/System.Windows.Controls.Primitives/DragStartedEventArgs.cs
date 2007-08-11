/*
#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	public class DragStartedEventArgs : RoutedEventArgs {
		#region Private Fields
		double horizontalOffset;
		double verticalOffset;
		#endregion

		#region Public Constructors
		public DragStartedEventArgs(double horizontalOffset, double verticalOffset) {
			this.horizontalOffset = horizontalOffset;
			this.verticalOffset = verticalOffset;
		}
		#endregion

		#region Public Properties
		public double HorizontalOffset {
			get { return horizontalOffset; }
		}

		public double VerticalOffset {
			get { return verticalOffset; }
		}
		#endregion

		#region Protected Methods
		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) {
			((DragStartedEventHandler)genericHandler).Invoke(genericTarget, this);
		}
		#endregion
	}
}
*/