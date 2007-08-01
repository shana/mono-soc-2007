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
using System.Collections.Generic;
using MonoDevelop.Core;
using MonoDevelop.Database.Sql;

namespace MonoDevelop.Database.Components
{
	public class SelectColumnWidget : ScrolledWindow
	{
		public event EventHandler ColumnToggled;
		
		protected TreeView list;
		protected ListStore store;
		
		private ColumnSchemaCollection columns;

		protected const int columnSelected = 0;
		protected const int columnName = 1;
		protected const int columnObj = 2;
		
		private bool singleSelect;
		
		public SelectColumnWidget ()
			: this (true)
		{
		}
		
		public SelectColumnWidget (bool showCheckBoxes)
		{
			InitializeStore ();
			
			list = new TreeView (store);
			list.HeadersVisible = true;
			
			InitializeColumns (showCheckBoxes);
			
			this.Add (list);
		}
		
		public bool SingleSelect {
			get { return singleSelect; }
			set {
				singleSelect = value;
				if (value)
					DeselectAll ();
			}
		}
		
		protected virtual void InitializeStore ()
		{
			store = new ListStore (typeof (bool), typeof (string), typeof (ColumnSchema));
		}
		
		protected virtual void InitializeColumns (bool showCheckBoxes)
		{
			TreeViewColumn col = new TreeViewColumn ();
			col.Title = GettextCatalog.GetString ("Column");

			if (showCheckBoxes) {
				CellRendererToggle toggleRenderer = new CellRendererToggle ();
				toggleRenderer.Activatable = true;
				toggleRenderer.Toggled += new ToggledHandler (ItemToggled);
				col.PackStart (toggleRenderer, false);
				col.AddAttribute (toggleRenderer, "active", columnSelected);
			}

			CellRendererText textRenderer = new CellRendererText ();
			col.PackStart (textRenderer, true);
			col.AddAttribute (textRenderer, "text", columnName);

			list.AppendColumn (col);
		}
		
		public void Initialize (ColumnSchemaCollection columns)
		{
			this.columns = columns;
			
			foreach (ColumnSchema column in columns)
				store.AppendValues (false, column.Name, column);
			
			columns.ItemAdded += new SortedCollectionItemEventHandler<ColumnSchema> (ColumnAdded);
			columns.ItemRemoved += new SortedCollectionItemEventHandler<ColumnSchema> (ColumnRemoved);
		}
		
		protected virtual void ColumnAdded (object sender, SortedCollectionItemEventArgs<ColumnSchema> args)
		{
			ColumnSchemaCollection columns = sender as ColumnSchemaCollection;
			int index = columns.IndexOf (args.Item);
			TreeIter iter = store.Insert (index);
			SetColumnValues (iter, args.Item);
		}
		
		protected virtual void SetColumnValues (TreeIter iter, ColumnSchema column)
		{
			store.SetValue (iter, columnSelected, false);
			store.SetValue (iter, columnName, column.Name);
			store.SetValue (iter, columnObj, column);
		}
		
		protected virtual void ColumnRemoved (object sender, SortedCollectionItemEventArgs<ColumnSchema> args)
		{
			TreeIter iter;
			if (store.GetIterFirst (out iter)) {
				do {
					object obj = store.GetValue (iter, columnObj);
					if (obj == args.Item) {
						store.Remove (ref iter);
						return;
					}
				} while (store.IterNext (ref iter));
			}
		}
		
		public ColumnSchema SelectedColumn {
			get {
				TreeIter iter;
				if (list.Selection.GetSelected (out iter))
					return store.GetValue (iter, columnObj) as ColumnSchema;
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
		
		public bool IsColumnChecked {
			get {
				TreeIter iter;
				if (store.GetIterFirst (out iter)) {
					do {
						bool chk = (bool)store.GetValue (iter, columnSelected);
						if (chk)
							return true;
					} while (store.IterNext (ref iter));
				}
				return false;
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
		
		public void Select (string column)
		{
			if (column == null)
				throw new ArgumentNullException ("column");
			
			ColumnSchema col = columns.Search (column);
			if (col != null)
				Select (col);
		}
		
		public void Select (ColumnSchema column)
		{
			if (column == null)
				throw new ArgumentNullException ("column");
			
			TreeIter iter;
			if (store.GetIterFirst (out iter)) {
				do {
					ColumnSchema col = store.GetValue (iter, columnObj) as ColumnSchema;
					if (column == col) {
						store.SetValue (iter, columnSelected, true);
						OnColumnToggled ();
						return;
					} else {
						if (singleSelect)
							store.SetValue (iter, columnSelected, false);
					}
				} while (store.IterNext (ref iter));
			}	
		}

		private void SetSelectState (bool state)
		{
			TreeIter iter;
			if (store.GetIterFirst (out iter)) {
				do {
					store.SetValue (iter, columnSelected, state);
				} while (store.IterNext (ref iter));
			}
			OnColumnToggled ();
		}
		
		private void ItemToggled (object sender, ToggledArgs args)
		{
	 		TreeIter iter;
			if (store.GetIterFromString (out iter, args.Path)) {
	 			bool val = (bool) store.GetValue (iter, columnSelected);
	 			store.SetValue (iter, columnSelected, !val);
				CheckSingleSelect (store.GetValue (iter, columnObj) as ColumnSchema);
				OnColumnToggled ();
	 		}
		}
		
		private void CheckSingleSelect (ColumnSchema column)
		{
			if (!singleSelect)
				return;
			
			TreeIter iter;
			if (store.GetIterFirst (out iter)) {
				do {
					object obj = store.GetValue (iter, columnObj);
					if (obj != column) {
						store.SetValue (iter, columnSelected, false);
						return;
					}
				} while (store.IterNext (ref iter));
			}
		}
		
		protected virtual void OnColumnToggled ()
		{
			if (ColumnToggled != null)
				ColumnToggled (this, EventArgs.Empty);
		}
	}
}