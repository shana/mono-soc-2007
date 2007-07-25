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
using System.Threading;
using System.Collections.Generic;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Components;
using MonoDevelop.Database.Sql;
using MonoDevelop.Database.Components;

namespace MonoDevelop.Database.Designer
{
	public partial class TableEditorDialog : Gtk.Dialog
	{
		private bool create;
		
		private ISchemaProvider schemaProvider;
		private TableSchema table;
		private ColumnSchemaCollection columns;
		private ConstraintSchemaCollection constraints;
		private IndexSchemaCollection indexes;
		private TriggerSchemaCollection triggers;
		private DataTypeSchemaCollection dataTypes;
		
		private Notebook notebook;

		private ColumnsEditorWidget columnEditor;
		private ConstraintsEditorWidget constraintEditor;
		private IndicesEditorWidget indexEditor;
		private TriggersEditorWidget triggerEditor;
		private CommentEditorWidget commentEditor;
		
		public TableEditorDialog (ISchemaProvider schemaProvider, TableSchema table, bool create)
		{
			if (schemaProvider == null)
				throw new ArgumentNullException ("schemaProvider");
			if (table == null)
				throw new ArgumentNullException ("table");
			
			this.schemaProvider = schemaProvider;
			this.table = table;
			this.create = create;
			
			this.Build();
			
			if (create)
				Title = GettextCatalog.GetString ("Create Table");
			else
				Title = GettextCatalog.GetString ("Alter Table");
			
			notebook = new Notebook ();
			vboxContent.PackStart (notebook, true, true, 0);

			columnEditor = new ColumnsEditorWidget (schemaProvider);
			constraintEditor = new ConstraintsEditorWidget (schemaProvider);
			indexEditor = new IndicesEditorWidget (schemaProvider);
			triggerEditor = new TriggersEditorWidget (schemaProvider);
			commentEditor = new CommentEditorWidget ();
			
			//TODO: only append if supported
			notebook.AppendPage (columnEditor, new Label (GettextCatalog.GetString ("Columns")));
			notebook.AppendPage (constraintEditor, new Label (GettextCatalog.GetString ("Constraints")));
			notebook.AppendPage (indexEditor, new Label (GettextCatalog.GetString ("Indexes")));
			notebook.AppendPage (triggerEditor, new Label (GettextCatalog.GetString ("Triggers")));
			notebook.AppendPage (commentEditor, new Label (GettextCatalog.GetString ("Comment")));
			notebook.ShowAll ();
			notebook.Page = 0;
			
			entryName.Text = table.Name;
			
			commentEditor.Sensitive = MetaDataService.IsTableMetaDataSupported (schemaProvider, TableMetaData.Comment);

			dataTypes = schemaProvider.GetDataTypes ();
			
			WaitDialog.ShowDialog ("Loading table data ...");
			notebook.Sensitive = false;
			ThreadPool.QueueUserWorkItem (new WaitCallback (InitializeThreaded));
			vboxContent.ShowAll ();
		}
		
		private void InitializeThreaded (object state)
		{
			columns = table.Columns;
			constraints = table.Constraints;
			triggers = table.Triggers;
			//TODO: indexex
			indexes = new IndexSchemaCollection ();
			
			Runtime.LoggingService.Debug ("COLUMNS.COUNT = " + columns.Count);
			
//			foreach (ColumnSchema col in columns) {
//				int dummy = col.Constraints.Count; //get column constraints
//			}

			Services.DispatchService.GuiDispatch (delegate () {
				InitializeGui ();
			});
		}
		
		private void InitializeGui ()
		{
			notebook.Sensitive = true;
			WaitDialog.HideDialog ();

			columnEditor.Initialize (columns, constraints, dataTypes);
			constraintEditor.Initialize (columns, constraints, dataTypes);
		}

		protected virtual void CancelClicked (object sender, System.EventArgs e)
		{
			Respond (ResponseType.Cancel);
			Hide ();
		}

		protected virtual void OkClicked (object sender, System.EventArgs e)
		{
			//TODO: validate
			
			Respond (ResponseType.Ok);
			Hide ();
		}

		protected virtual void NameChanged (object sender, System.EventArgs e)
		{
			if (entryName.Text.Length == 0)
				entryName.Text = table.Name;
			else
				table.Name = entryName.Text;
		}
	}
}
