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
using System.Collections.Generic;
using MonoDevelop.Database.Sql;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Database.Components;

namespace MonoDevelop.Database.GlueGenerator
{
	public partial class GenerateObjectsDialog : Gtk.Dialog
	{
		private ListStore storeColumns;
		private TreeStore storeDataSources;
		
		private TreeIter iterTables;
		private TreeIter iterViews;
		private TreeIter iterProcedures;
		private TreeIter iterCustom;
		
		private const int colDSPixbuf = 0;
		private const int colDSSelect = 1;
		private const int colDSName = 2;
		
		private const int colCSelect = 0;
		private const int colCName = 1;
		private const int colCType = 2;
		private const int colCPropName = 3;
		private const int colCPropType = 4;
		private const int colCNullable = 5;
		
		public GenerateObjectsDialog()
		{
			this.Build();
			
			storeColumns = new ListStore (typeof (bool), typeof (string), typeof (string), typeof (string), typeof (string), typeof (bool));
			storeDataSources = new TreeStore (typeof (Gdk.Pixbuf), typeof (bool), typeof (string));
			
			treeDataSource.Model = storeDataSources;
			treeDataSource.HeadersVisible = false;
			
			TreeViewColumn colDS = new TreeViewColumn ();
			treeDataSource.AppendColumn (colDS);
			
			CellRendererPixbuf dsPixbufRenderer = new CellRendererPixbuf ();
			CellRendererToggle dsSelectRenderer = new CellRendererToggle ();
			CellRendererText dsNameRenderer = new CellRendererText ();
			
			dsSelectRenderer.Toggled += new ToggledHandler (SelectDSToggled);
			
			colDS.AddAttribute (dsPixbufRenderer, "pixbuf", colDSPixbuf);
			colDS.AddAttribute (dsSelectRenderer, "active", colDSSelect);
			colDS.AddAttribute (dsNameRenderer, "text", colDSName);
			
			colDS.PackStart (dsPixbufRenderer, false);
			colDS.PackStart (dsSelectRenderer, false);
			colDS.PackEnd (dsNameRenderer, true);
			
			iterTables = storeDataSources.AppendValues (Services.Resources.GetIcon ("md-db-tables"), true, GettextCatalog.GetString ("Tables"));
			iterViews = storeDataSources.AppendValues (Services.Resources.GetIcon ("md-db-views"), true, GettextCatalog.GetString ("Views"));
			iterProcedures = storeDataSources.AppendValues (Services.Resources.GetIcon ("md-db-procedures"), true, GettextCatalog.GetString ("Stored Procedures"));
			iterCustom = storeDataSources.AppendValues (Services.Resources.GetIcon ("md-db-execute"), true, GettextCatalog.GetString ("Custom Queries"));
			
			listColumns.Model = storeColumns;
		}

		protected virtual void CancelClicked (object sender, System.EventArgs e)
		{
			Respond (ResponseType.Cancel);
			Destroy ();
		}

		protected virtual void BackClicked (object sender, System.EventArgs e)
		{
			if (notebook.Page == 1) {
				notebook.Page = 0;
				buttonBack.Sensitive = false;
				CheckStep1 ();
			}
		}
		
		protected virtual void ForwardClicked (object sender, System.EventArgs e)
		{
			if (notebook.Page == 0) {
				notebook.Page = 1;
				buttonForward.Sensitive = false;
				buttonBack.Sensitive = true;
			} else {
				Respond (ResponseType.Ok);
				Destroy ();
			}
		}

		protected virtual void NamespaceChanged (object sender, System.EventArgs e)
		{
			CheckStep1 ();
		}

		protected virtual void LocationChanged (object sender, System.EventArgs e)
		{
			CheckStep1 ();
		}

		protected virtual void ConnectionChanged (object sender, System.EventArgs e)
		{
			CheckStep1 ();
		}
		
		private void CheckStep1 ()
		{
			if (comboConnection.Active >= 0 && comboProject.Active >= 0
				&& entryNamespace.Text.Length > 0) {
				buttonForward.Sensitive = true;
			} else {
				buttonForward.Sensitive = false;
			}
		}
		
		private void SelectDSToggled (object sender, ToggledArgs args)
		{
	 		Gtk.TreeIter iter;
			if (storeDataSources.GetIterFromString (out iter, args.Path)) {
	 			bool val = (bool) storeDataSources.GetValue (iter, colDSSelect);
	 			storeDataSources.SetValue (iter, colDSSelect, !val);
	 		}
		}

		protected virtual void RemoveClicked (object sender, System.EventArgs e)
		{
		}

		protected virtual void AddClicked (object sender, System.EventArgs e)
		{
		}
	}
}
