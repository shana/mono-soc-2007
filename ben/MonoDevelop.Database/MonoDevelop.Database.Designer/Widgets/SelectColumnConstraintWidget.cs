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
using MonoDevelop.Database.Components;

namespace MonoDevelop.Database.Designer
{
	public class SelectColumnConstraintWidget : SelectColumnWidget
	{
		protected int columnIsColumnConstraint = 3;
		
		public SelectColumnConstraintWidget ()
			: this (true)
		{
		}
		
		public SelectColumnConstraintWidget (bool showCheckBoxes)
			: base (showCheckBoxes)
		{
		}
		
		public bool IsColumnConstraint (ColumnSchema column)
		{
			TreeIter iter;
			if (store.GetIterFirst (out iter)) {
				do {
					object obj = store.GetValue (iter, columnObj);
					if (obj == column)
						return (bool)store.GetValue (iter, columnIsColumnConstraint);
				} while (store.IterNext (ref iter));
			}
			return false;
		}
		
		public IEnumerable<ColumnSchema> CheckedColumnConstraintColumns {
			get {
				return GetCheckedConstraintColumns (true); 
			}
		}
		
		public IEnumerable<ColumnSchema> CheckedTableConstraintColumns {
			get {
				return GetCheckedConstraintColumns (false); 
			}
		}
		
		protected virtual IEnumerable<ColumnSchema> GetCheckedConstraintColumns (bool isColumnConstraint)
		{
			TreeIter iter;
			if (store.GetIterFirst (out iter)) {
				do {
					bool chk = (bool)store.GetValue (iter, columnSelected);
					bool constraint = (bool)store.GetValue (iter, columnIsColumnConstraint);
					if (chk && constraint == isColumnConstraint)
						yield return store.GetValue (iter, columnObj) as ColumnSchema;
				} while (store.IterNext (ref iter));
			}
		}
		
		protected override void InitializeStore ()
		{
			store = new ListStore (typeof (bool), typeof (string), typeof (object), typeof (bool));
		}
		
		protected override void InitializeColumns (bool showCheckBoxes)
		{
			base.InitializeColumns (showCheckBoxes);
				
			TreeViewColumn col = new TreeViewColumn ();
			col.Title = GettextCatalog.GetString ("Column Constraint");

			CellRendererToggle toggleRenderer = new CellRendererToggle ();
			toggleRenderer.Activatable = true;
			toggleRenderer.Toggled += new ToggledHandler (IsColumnConstraintToggled);
			col.PackStart (toggleRenderer, false);

			list.AppendColumn (col);
		}

		protected override void SetColumnValues (TreeIter iter, ColumnSchema column)
		{
			base.SetColumnValues (iter, column);
			store.SetValue (iter, columnIsColumnConstraint, false);
		}
		
		private void IsColumnConstraintToggled (object sender, ToggledArgs args)
		{
	 		TreeIter iter;
			if (store.GetIterFromString (out iter, args.Path)) {
	 			bool val = (bool) store.GetValue (iter, columnIsColumnConstraint);
	 			store.SetValue (iter, columnIsColumnConstraint, !val);
	 		}
		}
	}
}