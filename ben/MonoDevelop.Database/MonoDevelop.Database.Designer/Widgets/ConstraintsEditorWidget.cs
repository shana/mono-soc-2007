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
using MonoDevelop.Database.Components;

namespace MonoDevelop.Database.Designer
{
	public partial class ConstraintsEditorWidget : Gtk.Bin
	{
		public event EventHandler ContentChanged;
		
		private Notebook notebook;
		
		private ISchemaProvider schemaProvider;
		private ConstraintSchemaCollection constraints;
		private ColumnSchemaCollection columns;
		
		private PrimaryKeyConstraintEditorWidget pkEditor;
		private ForeignKeyConstraintEditorWidget fkEditor;
		private CheckConstraintEditorWidget checkEditor;
		private UniqueConstraintEditorWidget uniqueEditor;
		
		public ConstraintsEditorWidget (ISchemaProvider schemaProvider)
		{
			if (schemaProvider == null)
				throw new ArgumentNullException ("schemaProvider");
			
			this.schemaProvider = schemaProvider;
			
			//TODO: enable/disable features based on schema provider metadata
			
			this.Build();
			
			notebook = new Notebook ();
			Add (notebook);
		}
		
		public void Initialize (ColumnSchemaCollection columns, ConstraintSchemaCollection constraints, DataTypeSchemaCollection dataTypes)
		{
			Runtime.LoggingService.Error ("COEW: Initialize");
			if (columns == null)
				throw new ArgumentNullException ("columns");
			if (constraints == null)
				throw new ArgumentNullException ("constraints");

			this.columns = columns;
			this.constraints = constraints;

			if (MetaDataService.IsTableMetaDataSupported (schemaProvider, TableMetaData.PrimaryKeyConstraint)) {
				//not for column constraints, since they are already editable in the column editor
				pkEditor = new PrimaryKeyConstraintEditorWidget (schemaProvider, columns, constraints);
				pkEditor.ContentChanged += new EventHandler (OnContentChanged);
				notebook.AppendPage (checkEditor, new Label (GettextCatalog.GetString ("Primary Key")));
			}
			
			if (MetaDataService.IsTableMetaDataSupported (schemaProvider, TableMetaData.ForeignKeyConstraint)
				|| MetaDataService.IsTableColumnMetaDataSupported (schemaProvider, ColumnMetaData.ForeignKeyConstraint)) {
				fkEditor = new ForeignKeyConstraintEditorWidget ();
				fkEditor.ContentChanged += new EventHandler (OnContentChanged);
				notebook.AppendPage (fkEditor, new Label (GettextCatalog.GetString ("Foreign Key")));
			}
			
			if (MetaDataService.IsTableMetaDataSupported (schemaProvider, TableMetaData.CheckConstraint)
				|| MetaDataService.IsTableColumnMetaDataSupported (schemaProvider, ColumnMetaData.CheckConstraint)) {
				checkEditor = new CheckConstraintEditorWidget (schemaProvider, columns, constraints);
				checkEditor.ContentChanged += new EventHandler (OnContentChanged);
				notebook.AppendPage (checkEditor, new Label (GettextCatalog.GetString ("Check")));
			}
			
			if (MetaDataService.IsTableMetaDataSupported (schemaProvider, TableMetaData.UniqueConstraint)
				|| MetaDataService.IsTableColumnMetaDataSupported (schemaProvider, ColumnMetaData.UniqueConstraint)) {
				uniqueEditor = new UniqueConstraintEditorWidget (schemaProvider, columns, constraints);
				uniqueEditor.ContentChanged += new EventHandler (OnContentChanged);
				notebook.AppendPage (uniqueEditor, new Label (GettextCatalog.GetString ("Unique")));
			}

			ShowAll ();
			
			Runtime.LoggingService.Error ("COEW: Initialize 2");
		}
		
		private void OnContentChanged (object sender, EventArgs args)
		{
			if (ContentChanged != null)
				ContentChanged (this, args);
		}
		
		public virtual bool Validate ()
		{
			return (pkEditor != null && pkEditor.Validate ())
				&& (fkEditor != null && fkEditor.Validate ())
				&& (checkEditor != null && checkEditor.Validate ())
				&& (uniqueEditor != null && uniqueEditor.Validate ());
		}
	}
}
