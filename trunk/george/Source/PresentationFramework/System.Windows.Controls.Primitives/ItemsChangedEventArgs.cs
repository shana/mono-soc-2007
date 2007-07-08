using System.Collections.Specialized;
#if Implementation
using System;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	public class ItemsChangedEventArgs : EventArgs {
		#region Private Fields
		NotifyCollectionChangedAction action;
		int item_count;
		int item_ui_count;
		GeneratorPosition old_position;
		GeneratorPosition position;
		#endregion

		#region Internal Constructors
		internal ItemsChangedEventArgs(NotifyCollectionChangedAction action, int itemCount, int itemUICount, GeneratorPosition oldPosition, GeneratorPosition position) {
			this.action = action;
			item_count = itemCount;
			item_ui_count = itemUICount;
			old_position = oldPosition;
			this.position = position;
		}
		#endregion

		#region Public Properties
		public NotifyCollectionChangedAction Action {
			get { return action; }
		}

		public int ItemCount {
			get { return item_count; }
		}

		public int ItemUICount {
			get { return item_ui_count; }
		}

		public GeneratorPosition OldPosition {
			get { return old_position; }
		}

		public GeneratorPosition Position {
			get { return position; }
		}
		#endregion
	}
}
