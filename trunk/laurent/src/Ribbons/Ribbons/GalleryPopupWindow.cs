using System;
using Gtk;

namespace Ribbons
{
	public class GalleryPopupWindow : Window
	{
		private Gallery underlyingGallery;

		private uint rows, columns;

		private ScrolledWindow internalWindow;
		private Table tileTable;
		
		public Gallery UnderlyingGallery
		{
			get { return underlyingGallery; }
		}
		
		public GalleryPopupWindow (Gallery UnderlyingGallery) : base (WindowType.Popup)
		{
			this.underlyingGallery = UnderlyingGallery;
			
			this.tileTable = new Table (rows, columns, true);
			this.internalWindow = new ScrolledWindow ();
			this.internalWindow.Child = this.tileTable;
			
			this.Child = internalWindow;
		}
		
		
	}
}
