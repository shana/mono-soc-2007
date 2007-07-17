﻿//
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

namespace MonoDevelop.Database.Sql
{
	public class MySqlDbFactory : IDbFactory
	{
		private ISqlDialect dialect;
		private IConnectionProvider connectionProvider;
		
		public string Identifier {
			get { return "MySql.Data.MySqlClient"; }
		}
		
		public string Name {
			get { return "MySql database"; }
		}
		
		public ISqlDialect Dialect {
			get {
				if (dialect == null)
					dialect = new MySqlDialect ();
				return dialect;
			}
		}
		
		public IConnectionProvider ConnectionProvider {
			get {
				if (connectionProvider == null)
					connectionProvider = new MySqlConnectionProvider ();
				return connectionProvider;
			}
		}
		
		public IConnectionPool CreateConnectionPool (DatabaseConnectionContext context)
		{
			return new DefaultConnectionPool (this, ConnectionProvider, context);
		}
		
		public ISchemaProvider CreateSchemaProvider (IConnectionPool connectionPool)
		{
			return new MySqlSchemaProvider (connectionPool);
		}
		
		public object GetOption (string name)
		{
			switch (name) {
			case "settings.requires.server":
			case "settings.requires.port":
			case "settings.requires.username":
			case "settings.requires.password":
			case "settings.can_list_databases":
				return true;
			default:
				return null;
			}
		}
		
		public DatabaseConnectionSettings GetDefaultConnectionSettings ()
		{
			DatabaseConnectionSettings settings = new DatabaseConnectionSettings ();
			settings.ProviderIdentifier = Identifier;
			settings.Server = "localhost";
			settings.Port = 3306;
			settings.Username = "root";
			settings.MaxPoolSize = 5;
			
			return settings;
		}
		
		public bool ShowOpenDatabaseDialog (out string database)
		{
			database = null;
			return false;
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