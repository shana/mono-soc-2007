//
// Authors:
//    Ben Motmans  <ben.motmans@gmail.com>
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
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Components;
using MonoDevelop.Database.Sql;
using MonoDevelop.Database.Components;

namespace MonoDevelop.Database.Designer
{
	public partial class CheckConstraintEditorWidget : Gtk.Bin
	{
		public event EventHandler ContentChanged;
		
		private ISchemaProvider schemaProvider;
		private ColumnSchemaCollection columns;
		private ConstraintSchemaCollection constraints;
		
		private ListStore store;
		private ListStore storeColumns;
		
		private const int colNameIndex = 0;
		private const int colColumnNameIndex = 1;
		private const int colIsColumnConstraintIndex = 2;
		private const int colSourceIndex = 3;
		private const int colObjIndex = 4;
		
		private bool columnConstraintsSupported;
		private bool tableConstraintsSupported;
		
		public CheckConstraintEditorWidget (ISchemaProvider schemaProvider, ColumnSchemaCollection columns, ConstraintSchemaCollection constraints)
		{
			if (columns == null)
				throw new ArgumentNullException ("columns");
			if (constraints == null)
				throw new ArgumentNullException ("constraints");
			if (schemaProvider == null)
				throw new ArgumentNullException ("schemaProvider");
			
			this.schemaProvider = schemaProvider;
			this.columns = columns;
			this.constraints = constraints;
			
			this.Build();
			
			store = new ListStore (typeof (string), typeof (string), typeof (bool), typeof (string), typeof (object));
			storeColumns = new ListStore (typeof (string));
			
			listCheck.Model = store;
			
			//TODO: add/remove columns on events
			
			TreeViewColumn colName = new TreeViewColumn ();
			TreeViewColumn colColumn = new TreeViewColumn ();
			TreeViewColumn colIsColumnConstraint = new TreeViewColumn ();
			TreeViewColumn colSource = new TreeViewColumn ();
			
			colName.Title = GettextCatalog.GetString ("Name");
			colColumn.Title = GettextCatalog.GetString ("Column");
			colIsColumnConstraint.Title = GettextCatalog.GetString ("Column Constraint");
			colSource.Title = GettextCatalog.GetString ("Condition");
			
			colColumn.MinWidth = 120; //request a bigger width
			
			CellRendererText nameRenderer = new CellRendererText ();
			CellRendererCombo columnRenderer = new CellRendererCombo ();
			CellRendererToggle isColumnConstraintRenderer = new CellRendererToggle ();
			CellRendererText sourceRenderer = new CellRendererText ();

			nameRenderer.Editable = true;
			nameRenderer.Edited += new EditedHandler (NameEdited);
			
			columnRenderer.Model = storeColumns;
			columnRenderer.TextColumn = 0;
			columnRenderer.Editable = true;
			columnRenderer.Edited += new EditedHandler (ColumnEdited);

			isColumnConstraintRenderer.Activatable = true;
			isColumnConstraintRenderer.Toggled += new ToggledHandler (IsColumnConstraintToggled);
			
			colName.PackStart (nameRenderer, true);
			colColumn.PackStart (columnRenderer, true);
			colIsColumnConstraint.PackStart (isColumnConstraintRenderer, true);
			colSource.PackStart (sourceRenderer, true);

			colName.AddAttribute (nameRenderer, "text", colNameIndex);
			colColumn.AddAttribute (columnRenderer, "text", colColumnNameIndex);
			colIsColumnConstraint.AddAttribute (isColumnConstraintRenderer, "active", colIsColumnConstraintIndex);
			colSource.AddAttribute (sourceRenderer, "text", colSourceIndex);

			columnConstraintsSupported = MetaDataService.IsTableColumnMetaDataSupported (schemaProvider, ColumnMetaData.CheckConstraint);
			tableConstraintsSupported = MetaDataService.IsTableMetaDataSupported (schemaProvider, TableMetaData.CheckConstraint);
			
			listCheck.AppendColumn (colName);
			if (columnConstraintsSupported)
				listCheck.AppendColumn (colColumn);
			if (columnConstraintsSupported && tableConstraintsSupported)
				listCheck.AppendColumn (colIsColumnConstraint);
			listCheck.AppendColumn (colSource);
			
			listCheck.Selection.Changed += new EventHandler (OnSelectionChanged);
			sqlEditor.TextChanged += new EventHandler (SourceChanged);
			
			ShowAll ();
		}

		protected virtual void AddClicked (object sender, EventArgs e)
		{
			CheckConstraintSchema check = schemaProvider.GetNewCheckConstraintSchema ("check_new");
			int index = 1;
			while (constraints.Contains (check.Name))
				check.Name = "check_new" + (index++); 
			constraints.Add (check);
			AddConstraint (check);
		}

		protected virtual void RemoveClicked (object sender, EventArgs e)
		{
			TreeIter iter;
			if (listCheck.Selection.GetSelected (out iter)) {
				CheckConstraintSchema check = store.GetValue (iter, colObjIndex) as CheckConstraintSchema;
				
				if (Services.MessageService.AskQuestion (
					GettextCatalog.GetString ("Are you sure you want to remove constraint '{0}'?", check.Name),
					GettextCatalog.GetString ("Remove Constraint")
				)) {
					store.Remove (ref iter);
					constraints.Remove (check);
				}
			}
		}
		
		protected virtual void OnSelectionChanged (object sender, EventArgs e)
		{
			TreeIter iter;
			if (listCheck.Selection.GetSelected (out iter)) {
				buttonRemove.Sensitive = true;
				sqlEditor.Editable = true;
				
				CheckConstraintSchema check = store.GetValue (iter, colObjIndex) as CheckConstraintSchema;
				
				sqlEditor.Text = check.Source;
				
			} else {
				buttonRemove.Sensitive = false;
				sqlEditor.Editable = false;
				sqlEditor.Text = String.Empty;
			}
		}
		
		private void IsColumnConstraintToggled (object sender, ToggledArgs args)
		{
	 		TreeIter iter;
			if (storeColumns.GetIterFromString (out iter, args.Path)) {
	 			bool val = (bool) storeColumns.GetValue (iter, colIsColumnConstraintIndex);
	 			storeColumns.SetValue (iter, colIsColumnConstraintIndex, !val);
	 		}
		}
		
		private void NameEdited (object sender, EditedArgs args)
		{
			TreeIter iter;
			if (storeColumns.GetIterFromString (out iter, args.Path)) {
				if (!string.IsNullOrEmpty (args.NewText)) {
					storeColumns.SetValue (iter, colNameIndex, args.NewText);
				} else {
					string oldText = storeColumns.GetValue (iter, colNameIndex) as string;
					(sender as CellRendererText).Text = oldText;
				}
			}
		}
		
		private void ColumnEdited (object sender, EditedArgs args)
		{
			TreeIter iter;
			if (storeColumns.GetIterFromString (out iter, args.Path)) {
				if (columns.Contains (args.NewText)) { //only allow existing columns
					storeColumns.SetValue (iter, colColumnNameIndex, args.NewText);
				} else {
					string oldText = storeColumns.GetValue (iter, colColumnNameIndex) as string;
					(sender as CellRendererText).Text = oldText;
				}
			}
		}
		
		private void SourceChanged (object sender, EventArgs args)
		{
			TreeIter iter;
			if (listCheck.Selection.GetSelected (out iter)) {
				CheckConstraintSchema check = store.GetValue (iter, colObjIndex) as CheckConstraintSchema;
				check.Source = sqlEditor.Text;
				store.SetValue (iter, colSourceIndex, sqlEditor.Text);
			}
		}
		
		private void AddConstraint (CheckConstraintSchema check)
		{
			store.AppendValues (check.Name, String.Empty, false, String.Empty, check);
		}
		
		protected virtual void EmitContentChanged ()
		{
			if (ContentChanged != null)
				ContentChanged (this, EventArgs.Empty);
		}
		
		public virtual bool Validate ()
		{
			return false;
		}
	}
}
