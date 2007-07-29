using System;
using System.Collections.Generic;
using Gtk;

namespace Ribbons
{
	public class Gallery : Container
	{
		private int tileWidth, tileHeight;
		private List<Tile> tiles;
		private Button up, down, expand;
		private int defaultTilesPerRow;
		private int tileSpacing;
		
		private int firstDisplayedTileIndex, lastDisplayedTileIndex;
		private int btnWidth;
		private Requisition upReq, downReq, expandReq;
		
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
		
		public int TileSpacing
		{
			set
			{
				tileSpacing = value;
				QueueDraw ();
			}
			get { return tileSpacing; }
		}
		
		public int DefaultTilesPerRow
		{
			set
			{
				defaultTilesPerRow = value;
				QueueDraw ();
			}
			get { return defaultTilesPerRow; }
		}
		
		public Gallery()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.tiles = new List<Ribbons.Tile> ();
			
			this.defaultTilesPerRow = 5;
			
			this.tileHeight = 48;
			this.tileWidth = 64;
			this.tileSpacing = 8;
			
			this.up = new Button ("\u25B2");
			this.up.Parent = this;
			this.up.Padding = 0;
			this.up.Clicked += up_Clicked;
			
			this.down = new Button ("\u25BC");
			this.down.Parent = this;
			this.down.Padding = 0;
			this.down.Clicked += down_Clicked;
			
			this.expand = new Button ("\u2193");
			this.expand.Parent = this;
			this.expand.Padding = 0;
			this.expand.Clicked += expand_Clicked;
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
			if(TileIndex == -1 || TileIndex == tiles.Count)
			{
				tiles.Add (t);
			}
			else
			{
				tiles.Insert (TileIndex, t);
			}
			
			t.Parent = this;
			t.Visible = true;
		}
		
		/// <summary>Removes the tile at the specified index.</summary>
		/// <param name="TileIndex">Index of the tile to remove.</param>
		public void RemoveTile (int TileIndex)
		{
			tiles[TileIndex].Parent = null;
			
			tiles.RemoveAt (TileIndex);
		}
		
		private void up_Clicked(object Sender, EventArgs e)
		{
			MoveUp ();
		}
		
		private void down_Clicked(object Sender, EventArgs e)
		{
			MoveDown ();
		}
		
		private void expand_Clicked(object Sender, EventArgs e)
		{
			
		}
		
		private void MoveUp ()
		{
			if(firstDisplayedTileIndex > 0)
			{
				firstDisplayedTileIndex = -1;
				lastDisplayedTileIndex = firstDisplayedTileIndex - 1;
			}
			QueueDraw ();
		}
		
		private void MoveDown ()
		{
			if(lastDisplayedTileIndex < tiles.Count - 1)
			{
				firstDisplayedTileIndex = lastDisplayedTileIndex + 1;
				lastDisplayedTileIndex = -1;
			}
			QueueDraw ();
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			//if(include_internals)
			{
				callback (up);
				callback (down);
				callback (expand);
			}
			
			for(int i = firstDisplayedTileIndex ; i < lastDisplayedTileIndex ; ++i)
			{
				callback (tiles[i]);
			}
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			upReq = up.SizeRequest ();
			downReq = down.SizeRequest ();
			expandReq = expand.SizeRequest ();
			
			btnWidth = Math.Max (upReq.Width, Math.Max (downReq.Width, expandReq.Width));
			int btnHeight = upReq.Height + downReq.Height + expandReq.Height;
			
			requisition.Width = btnWidth + 2 * (int)BorderWidth;
			requisition.Height = Math.Max (tileHeight + 2*tileSpacing, btnHeight) + 2 * (int)BorderWidth;
			
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
			
			Gdk.Rectangle btnAlloc;
			btnAlloc.Width = btnWidth;
			btnAlloc.X = allocation.X + allocation.Width - (int)BorderWidth - btnAlloc.Width;
			
			btnAlloc.Y = allocation.Y + (int)BorderWidth;
			btnAlloc.Height = upReq.Height;
			up.SizeAllocate (btnAlloc);
			
			btnAlloc.Y += btnAlloc.Height;
			btnAlloc.Height = downReq.Height;
			down.SizeAllocate (btnAlloc);
			
			btnAlloc.Y += btnAlloc.Height;
			btnAlloc.Height = expandReq.Height;
			expand.SizeAllocate (btnAlloc);
			
			Gdk.Rectangle tilesAlloc;
			tilesAlloc.Y = allocation.Y + (int)BorderWidth + tileSpacing;
			tilesAlloc.X = allocation.X + (int)BorderWidth + tileSpacing;
			tilesAlloc.Width = btnAlloc.X - tilesAlloc.X - tileSpacing;
			tilesAlloc.Height = allocation.Height - 2 * ((int)BorderWidth + tileSpacing); 
			
			Gdk.Rectangle tileAlloc;
			tileAlloc.X = tilesAlloc.X;
			tileAlloc.Y = tilesAlloc.Y;
			tileAlloc.Height = tilesAlloc.Height;
			tileAlloc.Width = tileWidth;
			
			int maxTiles = (tilesAlloc.Width + tileSpacing) / (tileWidth + tileSpacing);
			
			if(firstDisplayedTileIndex == -1)
			{
				firstDisplayedTileIndex = lastDisplayedTileIndex - maxTiles + 1;
				if(firstDisplayedTileIndex < 0)
				{
					lastDisplayedTileIndex -= firstDisplayedTileIndex;
					firstDisplayedTileIndex = 0;
				}
			}
			else if(lastDisplayedTileIndex == -1)
			{
				lastDisplayedTileIndex = firstDisplayedTileIndex + maxTiles - 1;
			}
			
			if(lastDisplayedTileIndex >= tiles.Count)
			{
				lastDisplayedTileIndex = tiles.Count - 1;
			}
			
			for(int tileIndex = firstDisplayedTileIndex ; tileIndex <= lastDisplayedTileIndex ; ++tileIndex)
			{
				Tile t = tiles[tileIndex];
				
				t.HeightRequest = tileHeight;
				t.WidthRequest = tileWidth;
				t.SizeRequest ();
				
				t.SizeAllocate (tileAlloc);
				tileAlloc.X += tileAlloc.Width + tileSpacing;
			}
		}
	}
}
