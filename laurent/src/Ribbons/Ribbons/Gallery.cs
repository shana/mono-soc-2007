using System;
using System.Collections.Generic;
using Gtk;

namespace Ribbons
{
	public class Gallery : Container
	{
		private int tileWidth, tileHeight;
		private List<Tile> tiles;
		private ScrolledWindow scrollWin;
		
		public int TileWidth
		{
			set
			{
				tileWidth = value;
				QueueDraw ();
			}
			get { return tileWidth; }	
		}
		
		public int TileHeight
		{
			set
			{
				tileHeight = value;
				QueueDraw ();
			}
			get { return tileHeight; }
		}
		
		public Gallery()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.scrollWin = new ScrolledWindow ();
		}
		
		/// <summary>Adds a tile before all existing tiles.</summary>
		/// <param name="t">The tile to add.</param>
		public void PrependTile (Tile t)
		{
			InsertTile (t, 0);
		}
		
		/// <summary>Adds a tile after all existing tiles.</summary>
		/// <param name="t">The tile to add.</param>
		public void AppendTile (Tile t)
		{
			InsertTile (t, -1);
		}
		
		/// <summary>Inserts a tile at the specified location.</summary>
		/// <param name="t">The tile to add.</param>
		/// <param name="TileIndex">The index (starting at 0) at which the tile must be inserted, or -1 to insert the tile after all existing tiles.</param>
		public void InsertTile (Tile t, int TileIndex)
		{
			t.Parent = this;
			t.Visible = true;
			
			if(TileIndex == -1 || TileIndex == tiles.Count)
			{
				tiles.Add (t);
			}
			else
			{
				tiles.Insert (TileIndex, t);
			}
		}
		
		/// <summary>Removes the tile at the specified index.</summary>
		/// <param name="TileIndex">Index of the tile to remove.</param>
		public void RemoveTile (int TileIndex)
		{
			tiles[TileIndex].Parent = null;
			
			tiles.RemoveAt (TileIndex);
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			if(include_internals) callback (scrollWin);
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			Requisition scrollReq = scrollWin.SizeRequest ();
			
			requisition.Width = scrollReq.Width + 2 * (int)BorderWidth;
			requisition.Height = scrollReq.Height + 2 * (int)BorderWidth;
			
			if(WidthRequest != -1) requisition.Width = WidthRequest;
			if(HeightRequest != -1) requisition.Height = HeightRequest;
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			allocation.X += (int)BorderWidth;
			allocation.Y += (int)BorderWidth;
			allocation.Width -= 2 * (int)BorderWidth;
			allocation.Height -= 2 * (int)BorderWidth;
			
			scrollWin.SizeAllocate (allocation);
			
			foreach(Tile t in tiles)
			{
				t.HeightRequest = tileHeight;
				t.WidthRequest = tileWidth;
				
			}
		}
	}
}