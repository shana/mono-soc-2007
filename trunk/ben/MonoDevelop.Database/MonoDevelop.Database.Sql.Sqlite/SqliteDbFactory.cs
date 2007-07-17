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
using System.Data;
using System.Collections.Generic;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Database.Components;
using MonoDevelop.Ide.Gui.Dialogs;
namespace MonoDevelop.Database.Sql
{
	public class SqliteDbFactory : IDbFactory
	{
		private ISqlDialect dialect;
		private IConnectionProvider connectionProvider;
		
		public string Identifier {
			get { return "MonoDevelop.Database.SqliteClient"; }
		}
		
		public string Name {
			get { return "SQLite database"; }
		}
		
		public ISqlDialect Dialect {
			get {
				if (dialect == null)
					dialect = new SqliteDialect ();
				return dialect;
			}
		}
		
		public IConnectionProvider ConnectionProvider {
			get {
				if (connectionProvider == null)
					connectionProvider = new SqliteConnectionProvider ();
				return connectionProvider;
			}
		}
		
		public IConnectionPool CreateConnectionPool (DatabaseConnectionContext context)
		{
			return new DefaultConnectionPool (this, ConnectionProvider, context);
		}
		
		public ISchemaProvider CreateSchemaProvider (IConnectionPool connectionPool)
		{
			return new SqliteSchemaProvider (connectionPool);
		}
		
		public object GetOption (string name)
		{
			switch (name) {
			case "settings.can_open_database":
				return true;
			default:
				return null;
			}
		}
		
		public DatabaseConnectionSettings GetDefaultConnectionSettings ()
		{
			DatabaseConnectionSettings settings = new DatabaseConnectionSettings ();
			settings.ProviderIdentifier = Identifier;
			settings.MaxPoolSize = 5;
			
			return settings;
		}
		
		public bool ShowOpenDatabaseDialog (out string database)
		{
			FileChooserDialog dlg = new FileChooserDialog (
				GettextCatalog.GetString ("Open Database"), null, FileChooserAction.Open,
				"gtk-cancel", ResponseType.Cancel,
				"gtk-open", ResponseType.Accept
			);
			dlg.SelectMultiple = false;
			dlg.LocalOnly = true;
			dlg.Modal = true;
			
			FileFilter filter = new FileFilter ();
			filter.AddMimeType ("application/x-sqlite2");
			filter.AddMimeType ("application/x-sqlite3");
			filter.AddPattern ("*.db");
			filter.Name = GettextCatalog.GetString ("SQLite databases");
			FileFilter filterAll = new FileFilter ();
			filterAll.AddPattern ("*");
			filterAll.Name = GettextCatalog.GetString ("All files");
			dlg.AddFilter (filter);
			dlg.AddFilter (filterAll);

			if (dlg.Run () == (int)ResponseType.Accept) {
				database = dlg.Filename;
				dlg.Destroy ();
				return true;
			} else {
				dlg.Destroy ();
				database = null;
				return false;
			}
		}
		
		public bool ShowEditDatabaseConnectionDialog (DatabaseConnectionSettings connectionSettings)
		{
			DatabaseConnectionSettingsDialog dlg = new DatabaseConnectionSettingsDialog (connectionSettings);
			int result = dlg.Run ();
			dlg.Destroy ();
			return result == (int)ResponseType.Ok;
		}
		
		public bool ShowRemoveDatabaseConnectionDialog (DatabaseConnectionSettings connectionSettings)
		{
			return Services.MessageService.AskQuestionFormatted (
				GettextCatalog.GetString ("Are you sure you want to remove connection '{0}'?"),
				connectionSettings.Name); 
		}
	}
}