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
		private ListStore storeColumns;
		private ListStore storeTypes;
		
		private const int colPKIndex = 0;
		private const int colFKIndex = 1;
		private const int colNameIndex = 2;
		private const int colTypeIndex = 3;
		private const int colLengthIndex = 4;
		private const int colNullableIndex = 5;
		private const int colCommentIndex = 6;
		private const int colObjectIndex = 7;
		private const int colKeyObjectIndex = 8;
		
		private ISchemaProvider schemaProvider;
		private TableSchema table;
		private object sync = new object ();
		
		private ColumnSchemaCollection columns = null;
		private ConstraintSchemaCollection constraints = null;
		private TriggerSchemaCollection triggers = null;
		
		public ColumnsEditorWidget()
		{
			this.Build();
			
			storeTypes = new ListStore (typeof (string));
			storeColumns = new ListStore (typeof (Gdk.Pixbuf), typeof (Gdk.Pixbuf), typeof (string), typeof (string), typeof (int), typeof (bool), typeof (string), typeof (object), typeof (object));
			treeColumns.Model = storeColumns;
			
			TreeViewColumn colName = new TreeViewColumn (GettextCatalog.GetString ("Name"));
			TreeViewColumn colType = new TreeViewColumn (GettextCatalog.GetString ("Type"));
			TreeViewColumn colLength = new TreeViewColumn (GettextCatalog.GetString ("Length"));
			TreeViewColumn colPK = new TreeViewColumn (GettextCatalog.GetString ("PK"));
			TreeViewColumn colNullable = new TreeViewColumn (GettextCatalog.GetString ("N"));
			TreeViewColumn colComment = new TreeViewColumn (GettextCatalog.GetString ("Comment"));
			
			CellRendererText nameRenderer = new CellRendererText ();
			CellRendererCombo typeRenderer = new CellRendererCombo ();
			CellRendererSpin lengthRenderer = new CellRendererSpin ();
			CellRendererToggle pkRenderer = new CellRendererToggle ();
			CellRendererToggle nullableRenderer = new CellRendererToggle ();
			CellRendererText commentRenderer = new CellRendererText ();
			
			nameRenderer.Editable = true;
			nameRenderer.Edited += new EditedHandler (NameEdited);
			
			typeRenderer.Model = storeTypes;
			
			pkRenderer.Activatable = true;
			pkRenderer.Toggled += new ToggledHandler (PkToggled);
			
			nullableRenderer.Activatable = true;
			nullableRenderer.Toggled += new ToggledHandler (NullableToggled);
			
			commentRenderer.Editable = true;
			commentRenderer.Edited += new EditedHandler (CommentEdited);
			
			colName.PackStart (nameRenderer, true);
			colType.PackStart (typeRenderer, true);
			colLength.PackStart (lengthRenderer, true);
			colPK.PackStart (pkRenderer, true);
			colNullable.PackStart (nullableRenderer, true);
			colComment.PackStart (commentRenderer, true);
			
			treeColumns.AppendColumn (colName);
			treeColumns.AppendColumn (colType);
			treeColumns.AppendColumn (colLength);
			treeColumns.AppendColumn (colPK);
			treeColumns.AppendColumn (colNullable);
			treeColumns.AppendColumn (colComment);
			
			treeColumns.Reorderable = false;
			treeColumns.HeadersClickable = false;
			treeColumns.HeadersVisible = true;
			treeColumns.EnableGridLines = TreeViewGridLines.Vertical;
			treeColumns.EnableSearch = false;
			
			treeColumns.ShowAll ();
		}
		
		public void Initialize (ISchemaProvider schemaProvider, TableSchema table)
		{
			if (schemaProvider == null)
				throw new ArgumentNullException ("schemaProvider");
			if (table == null)
				throw new ArgumentNullException ("table");
			
			lock (sync) {
				this.schemaProvider = schemaProvider;
				this.table = table;
			}
			ThreadPool.QueueUserWorkItem (new WaitCallback (InitializeThreaded));
		}
		
		private void InitializeThreaded (object state)
		{
			lock (sync) {
				columns = schemaProvider.GetTableColumns (table);
				constraints = schemaProvider.GetTableConstraints (table);
				triggers = schemaProvider.GetTriggers (table);
			}
			
			foreach (ColumnSchema column in columns) {
				Services.DispatchService.GuiDispatch (delegate () {
					AppendColumnSchema (column);
				});
			}
		}
		
		private void AppendColumnSchema (ColumnSchema column)
		{
			//TODO: PK+FK
			//storeColumns.AppendValues (column.Name, column.DataTypeName, column.Length, false, column.NotNull, column.Comment, column);
		}

		protected virtual void AddClicked (object sender, EventArgs e)
		{
			ColumnSchema column = new ColumnSchema (schemaProvider);
			AppendColumnSchema (column);
		}

		protected virtual void RemoveClicked (object sender, EventArgs e)
		{
			TreeIter iter;
			if (treeColumns.Selection.GetSelected (out iter)) {
				ColumnSchema column = storeColumns.GetValue (iter, colObjectIndex) as ColumnSchema;
				
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
	 			bool val = (bool) storeColumns.GetValue (iter, colPkIndex);
	 			storeColumns.SetValue (iter, colPkIndex, !val);
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
		
		private void CommentEdited (object sender, EditedArgs args)
		{
			TreeIter iter;
			if (storeColumns.GetIterFromString (out iter, args.Path))
				storeColumns.SetValue (iter, colNameIndex, args.NewText);
		}
	}
}
