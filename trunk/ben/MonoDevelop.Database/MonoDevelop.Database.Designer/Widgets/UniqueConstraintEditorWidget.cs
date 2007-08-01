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
using System.Text;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Components;
using MonoDevelop.Database.Sql;
using MonoDevelop.Database.Components;

namespace MonoDevelop.Database.Designer
{
	public partial class UniqueConstraintEditorWidget : Gtk.Bin
	{
		public event EventHandler ContentChanged;
		
		private ISchemaProvider schemaProvider;
		private ColumnSchemaCollection columns;
		private ConstraintSchemaCollection constraints;
		
		private ListStore store;
		
		private const int colNameIndex = 0;
		private const int colIsColumnConstraintIndex = 1;
		private const int colObjIndex = 2;
		
		public UniqueConstraintEditorWidget (ISchemaProvider schemaProvider, ColumnSchemaCollection columns, ConstraintSchemaCollection constraints)
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
		}

		protected virtual void AddClicked (object sender, EventArgs e)
		{
			UniqueConstraintSchema uni = schemaProvider.GetNewUniqueConstraintSchema ("uni_new");
			int index = 1;
			while (constraints.Contains (uni.Name))
				uni.Name = "uni_new" + (index++); 
			constraints.Add (uni);
			//AddConstraint (uni);
		}

		protected virtual void RemoveClicked (object sender, EventArgs e)
		{
			TreeIter iter;
			if (listUnique.Selection.GetSelected (out iter)) {
				UniqueConstraintSchema uni = store.GetValue (iter, colObjIndex) as UniqueConstraintSchema;
				
				if (Services.MessageService.AskQuestion (
					GettextCatalog.GetString ("Are you sure you want to remove constraint '{0}'?", uni.Name),
					GettextCatalog.GetString ("Remove Constraint")
				)) {
					store.Remove (ref iter);
					constraints.Remove (uni);
				}
			}
		}
		
		public virtual bool Validate ()
		{
			return false;
		}
		
		protected virtual void EmitContentChanged ()
		{
			if (ContentChanged != null)
				ContentChanged (this, EventArgs.Empty);
		}
	}
}
