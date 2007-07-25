//
// Authors:
//   Ben Motmans  <ben.motmans@gmail.com>
//
// Copyright (c) 2007 Ben Motmans
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using Gtk;
using System;
using System.Threading;
using System.Collections.Generic;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Database.Sql;

namespace MonoDevelop.Database.Designer
{
	public partial class ColumnsEditorWidget : Gtk.Bin
	{
		public EventHandler ContentChanged;
		
		private ListStore storeColumns;
		private ListStore storeTypes;
		
		private const int colPKIndex = 0;
		private const int colNameIndex = 1;
		private const int colTypeIndex = 2;
		private const int colLengthIndex = 3;
		private const int colNullableIndex = 4;
		private const int colCommentIndex = 5;
		private const int colObjectIndex = 6;
		
		private ColumnSchemaCollection columns;
		private ConstraintSchemaCollection constraints;
		private DataTypeSchemaCollection dataTypes;
		private ISchemaProvider schemaProvider;
		
		public ColumnsEditorWidget (ISchemaProvider schemaProvider)
		{
			if (schemaProvider == null)
				throw new ArgumentNullException ("schemaProvider");
			
			this.schemaProvider = schemaProvider;

			this.Build();
			
			storeTypes = new ListStore (typeof (string), typeof (object));
			storeColumns = new ListStore (typeof (bool), typeof (string), typeof (string), typeof (string), typeof (bool), typeof (string), typeof (object));
			treeColumns.Model = storeColumns;
			treeColumns.Selection.Changed += new EventHandler (OnSelectionChanged);

			//TODO: cols for scale, precision, ... ?
			TreeViewColumn colPK = new TreeViewColumn ();
			TreeViewColumn colName = new TreeViewColumn ();
			TreeViewColumn colType = new TreeViewColumn ();
			TreeViewColumn colLength = new TreeViewColumn ();
			TreeViewColumn colNullable = new TreeViewColumn ();
			TreeViewColumn colComment = new TreeViewColumn ();
			
			colPK.Title = GettextCatalog.GetString ("PK");
			colName.Title = GettextCatalog.GetString ("Name");
			colType.Title = GettextCatalog.GetString ("Type");
			colLength.Title = GettextCatalog.GetString ("Length");
			colNullable.Title = GettextCatalog.GetString ("Nullable");
			colComment.Title = GettextCatalog.GetString ("Comment");
			
			colType.MinWidth = 120; //request a bigger width

			CellRendererToggle pkRenderer = new CellRendererToggle ();
			CellRendererText nameRenderer = new CellRendererText ();
			CellRendererCombo typeRenderer = new CellRendererCombo ();
			CellRendererText lengthRenderer = new CellRendererText ();
			CellRendererToggle nullableRenderer = new CellRendererToggle ();
			CellRendererText commentRenderer = new CellRendererText ();

			nameRenderer.Editable = true;
			nameRenderer.Edited += new EditedHandler (NameEdited);
			
			typeRenderer.Model = storeTypes;
			typeRenderer.TextColumn = 0;
			typeRenderer.Editable = true;
			typeRenderer.Edited += new EditedHandler (TypeEdited);
			
			lengthRenderer.Editable = true;
			lengthRenderer.Edited += new EditedHandler (LengthEdited);
			
			pkRenderer.Activatable = true;
			pkRenderer.Toggled += new ToggledHandler (PkToggled);
			
			nullableRenderer.Activatable = true;
			nullableRenderer.Toggled += new ToggledHandler (NullableToggled);
			
			commentRenderer.Editable = true;
			commentRenderer.Edited += new EditedHandler (CommentEdited);
			
			colPK.PackStart (pkRenderer, true);
			colName.PackStart (nameRenderer, true);
			colType.PackStart (typeRenderer, true);
			colLength.PackStart (lengthRenderer, true);
			colNullable.PackStart (nullableRenderer, true);
			colComment.PackStart (commentRenderer, true);

			colPK.AddAttribute (pkRenderer, "active", colPKIndex);
			colName.AddAttribute (nameRenderer, "text", colNameIndex);
			colType.AddAttribute (typeRenderer, "text", colTypeIndex);
			colLength.AddAttribute (lengthRenderer, "text", colLengthIndex);
			colNullable.AddAttribute (nullableRenderer, "active", colNullableIndex);
			colComment.AddAttribute (commentRenderer, "text", colCommentIndex);

			if (MetaDataService.IsTableColumnMetaDataSupported (schemaProvider, ColumnMetaData.PrimaryKeyConstraints))
				treeColumns.AppendColumn (colPK);
			treeColumns.AppendColumn (colName);
			treeColumns.AppendColumn (colType);
			if (MetaDataService.IsTableColumnMetaDataSupported (schemaProvider, ColumnMetaData.Length))
				treeColumns.AppendColumn (colLength);
			if (MetaDataService.IsTableColumnMetaDataSupported (schemaProvider, ColumnMetaData.Nullable))
				treeColumns.AppendColumn (colNullable);
			if (MetaDataService.IsTableColumnMetaDataSupported (schemaProvider, ColumnMetaData.Comment))
				treeColumns.AppendColumn (colComment);

			treeColumns.Reorderable = false;
			treeColumns.HeadersClickable = false;
			treeColumns.HeadersVisible = true;
			treeColumns.EnableGridLines = TreeViewGridLines.Both;
			treeColumns.EnableSearch = false;

			ShowAll ();
		}
		
		public void Initialize (ColumnSchemaCollection columns, ConstraintSchemaCollection constraints, DataTypeSchemaCollection dataTypes)
		{
			if (columns == null)
				throw new ArgumentNullException ("columns");
			if (constraints == null)
				throw new ArgumentNullException ("constraints");

			this.columns = columns;
			this.constraints = constraints;
			this.dataTypes = dataTypes;
			
			foreach (ColumnSchema column in columns)
				AppendColumnSchema (column);
			
			foreach (DataTypeSchema dataType in dataTypes)
				storeTypes.AppendValues (dataType.Name, storeTypes);
		}
		
		public bool ValidateContent ()
		{
			//TODO: validate col types
			return false;
		}
		
		private void AppendColumnSchema (ColumnSchema column)
		{
			bool pk = false;
			
//			pk = column.Constraints.GetConstraintWithColumn (column.Name, ConstraintType.PrimaryKey) != null;
//			fk = column.Constraints.GetConstraintWithColumn (column.Name, ConstraintType.ForeignKey) != null;
//			
//			pk |= constraints.GetConstraintWithColumn (column.Name, ConstraintType.PrimaryKey) != null;
//			fk |= constraints.GetConstraintWithColumn (column.Name, ConstraintType.ForeignKey) != null;
			
			storeColumns.AppendValues (pk, column.Name, column.DataTypeName, column.Length.ToString (), column.IsNullable, column.Comment, column);
		}

		protected virtual void AddClicked (object sender, EventArgs e)
		{
			ColumnSchema column = new ColumnSchema (schemaProvider);
			column.Name = CreateColumnName ();
			TreeIter iter;
			if (storeTypes.GetIterFirst (out iter))
				column.DataTypeName = storeTypes.GetValue (iter, 0) as string;
			
			columns.Add (column);
			AppendColumnSchema (column);
		}

		protected virtual void RemoveClicked (object sender, EventArgs e)
		{
			TreeIter iter;
			if (treeColumns.Selection.GetSelected (out iter)) {
				ColumnSchema column = storeColumns.GetValue (iter, colObjectIndex) as ColumnSchema;
				
				//TODO: also check for attached constraints
				
				bool result = Services.MessageService.AskQuestion (
					GettextCatalog.GetString ("Are you sure you want to remove column '{0}'", column.Name),
					GettextCatalog.GetString ("Remove Column")
				);
				
				if (result)
					storeColumns.Remove (ref iter);
			}
		}
		
		private void PkToggled (object sender, ToggledArgs args)
		{
	 		TreeIter iter;
			if (storeColumns.GetIterFromString (out iter, args.Path)) {
	 			bool val = (bool) storeColumns.GetValue (iter, colPKIndex);
	 			storeColumns.SetValue (iter, colPKIndex, !val);
	 		}
		}
		
		private void NullableToggled (object sender, ToggledArgs args)
		{
	 		TreeIter iter;
			if (storeColumns.GetIterFromString (out iter, args.Path)) {
	 			bool val = (bool) storeColumns.GetValue (iter, colNullableIndex);
	 			storeColumns.SetValue (iter, colNullableIndex, !val);
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
		
		private void TypeEdited (object sender, EditedArgs args)
		{
			TreeIter iter;
			if (storeColumns.GetIterFromString (out iter, args.Path)) {
				if (!string.IsNullOrEmpty (args.NewText)) {
					storeColumns.SetValue (iter, colTypeIndex, args.NewText);
				} else {
					string oldText = storeColumns.GetValue (iter, colTypeIndex) as string;
					(sender as CellRendererText).Text = oldText;
				}
			}
		}
		
		private void LengthEdited (object sender, EditedArgs args)
		{
			TreeIter iter;
			if (storeColumns.GetIterFromString (out iter, args.Path)) {
				int len;
				if (!string.IsNullOrEmpty (args.NewText) && int.TryParse (args.NewText, out len)) {
					storeColumns.SetValue (iter, colLengthIndex, args.NewText);
				} else {
					string oldText = storeColumns.GetValue (iter, colLengthIndex) as string;
					(sender as CellRendererText).Text = oldText;
				}
			}
		}
		
		private void CommentEdited (object sender, EditedArgs args)
		{
			TreeIter iter;
			if (storeColumns.GetIterFromString (out iter, args.Path))
				storeColumns.SetValue (iter, colCommentIndex, args.NewText);
		}

		protected virtual void DownClicked (object sender, EventArgs e)
		{
			TreeIter iter;
			if (treeColumns.Selection.GetSelected (out iter)) {
				TreePath path = storeColumns.GetPath (iter);
				int x = path.Indices[0];
				columns.Swap (x, x + 1);
			}
		}

		protected virtual void UpClicked (object sender, EventArgs e)
		{
			TreeIter iter;
			if (treeColumns.Selection.GetSelected (out iter)) {
				TreePath path = storeColumns.GetPath (iter);
				int x = path.Indices[0];
				columns.Swap (x, x - 1);
			}
		}
		
		private void OnSelectionChanged (object sender, EventArgs e)
		{
			TreeIter iter;
			bool sel = false;
			bool next = false;
			bool prev = false;
			
			if (treeColumns.Selection.GetSelected (out iter)) {
				TreePath path = storeColumns.GetPath (iter);
				int index = path.Indices[0];
				
				sel = true;
				prev = index > 0;
				next = storeColumns.IterNext (ref iter);
			}
			
			buttonUp.Sensitive = prev;
			buttonDown.Sensitive = next;
			buttonRemove.Sensitive = sel;
		}
		
		private string CreateColumnName ()
		{
			int counter = 1;
			do {
				string name = "column" + (counter++);
				if (!columns.Contains (name))
					return name;
			} while (true);
			return null;
		}
	}
}
