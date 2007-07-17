//
// Authors:
//   Ben Motmans  <ben.motmans@gmail.com>
//
// Copyright (c) 2007 Ben Motmans
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using Gtk;
using System;
using System.Collections.Generic;using MonoDevelop.Database.Sql;

namespace MonoDevelop.Database.Components
{
	public class SelectColumnWidget : ScrolledWindow
	{
		private TreeView list;
		private ListStore store;

		private const int columnSelected = 0;
		private const int columnObj = 1;
		
		public SelectColumnWidget (bool showCheckBoxes)
		{
			store = new ListStore (typeof (bool), typeof (ColumnSchema));
			list = new TreeView (store);
			
			TreeViewColumn col = new TreeViewColumn ();

			if (showCheckBoxes) {
				CellRendererToggle toggleRenderer = new CellRendererToggle ();
				toggleRenderer.Activatable = true;
				toggleRenderer.Toggled += new ToggledHandler (ItemToggled);
				col.PackStart (toggleRenderer, false);
			}
			
			CellRendererPixbuf pixbufRenderer = new CellRendererPixbuf ();
			col.PackStart (pixbufRenderer, false);

			CellRendererText textRenderer = new CellRendererText ();
			col.PackStart (textRenderer, true);

			col.SetCellDataFunc (textRenderer, new CellLayoutDataFunc (TextDataFunc));
			col.SetCellDataFunc (pixbufRenderer, new CellLayoutDataFunc (PixbufDataFunc));

			list.AppendColumn (col);
			list.HeadersVisible = false;
			
			this.Add (list);
		}
		
		public void Append (IEnumerable<ColumnSchema> columns)
		{
			foreach (ColumnSchema column in columns)
				store.AppendValues (true, column);
		}
		
		public ColumnSchema SelectedColumn {
			get {
				TreeIter iter;
				if (list.Selection.GetSelected (out iter))
					return store.GetValue (iter, 2) as ColumnSchema;
				return null;
			}
		}
		
		public IEnumerable<ColumnSchema> CheckedColumns {
			get {
				TreeIter iter;
				if (store.GetIterFirst (out iter)) {
					do {
						bool chk = (bool)store.GetValue (iter, columnSelected);
						if (chk)
							yield return store.GetValue (iter, columnObj) as ColumnSchema;
					} while (store.IterNext (ref iter));
				}
			}
		}
		
		public void SelectAll ()
		{
			SetSelectState (true);
		}
		
		public void DeselectAll ()
		{
			SetSelectState (false);
		}
		private void SetSelectState (bool state)
		{
			TreeIter iter;
			if (store.GetIterFirst (out iter)) {
				do {
					store.SetValue (iter, columnSelected, state);
				} while (store.IterNext (ref iter));
			}	
		}
		
		private void TextDataFunc (CellLayout layout, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			CellRendererText textRenderer = cell as CellRendererText;
			ColumnSchema schema = model.GetValue (iter, columnObj) as ColumnSchema;
			textRenderer.Text = schema.Name;
		}
		
		private void PixbufDataFunc (CellLayout layout, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			CellRendererPixbuf pixbufRenderer = cell as CellRendererPixbuf;
			pixbufRenderer.Pixbuf = MonoDevelop.Core.Gui.Services.Resources.GetIcon ("md-db-column");
		}
		
		private void ItemToggled (object sender, ToggledArgs args)
		{
	 		TreeIter iter;
			if (store.GetIterFromString (out iter, args.Path)) {
	 			bool val = (bool) store.GetValue (iter, columnSelected);
	 			store.SetValue (iter, columnSelected, !val);
	 		}
		}
	}
}