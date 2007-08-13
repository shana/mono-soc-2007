using System;
using Gtk;

namespace Ribbons
{
	public class GalleryPopupWindow : Window
	{
		private Gallery underlyingGallery;

		private Table tileTable;
		
		public Gallery UnderlyingGallery
		{
			get { return underlyingGallery; }
		}
		
		public GalleryPopupWindow (Gallery UnderlyingGallery) : base (WindowType.Popup)
		{
			this.underlyingGallery = UnderlyingGallery;
			this.tileTable = new Table ();
			
			HBox hbox = new HBox ();
			Scrollbar sbar = new Scrollbar ();
			
			this.Child = tileTable;
		}
		
		
	}
}
