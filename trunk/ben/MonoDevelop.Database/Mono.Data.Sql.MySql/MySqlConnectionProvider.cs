//
// Authors:
//	Christian Hergert  <chris@mosaix.net>
//	Daniel Morgan <danielmorgan@verizon.net>
//	Sureshkumar T <tsureshkumar@novell.com>
//	Ben Motmans  <ben.motmans@gmail.com>
//
// Copyright (C) 2005 Mosaix Communications, Inc.
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

using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Mono.Data.Sql
{
	public class MySqlConnectionProvider : AbstractConnectionProvider
	{
		public MySqlConnectionProvider (IDbFactory factory, ConnectionSettings settings)
			: base (factory, settings)
		{
		}
		
		public override DataSet ExecuteQueryAsDataSet (string sql)
		{
			if (String.IsNullOrEmpty ("sql"))
				throw new ArgumentException ("sql");

			DataSet set = new DataSet ();
			using (IDbCommand command = CreateCommand (sql)) {
				using (MySqlDataAdapter adapter = new MySqlDataAdapter (command as MySqlCommand)) {
					try {
						adapter.Fill (set);
					} catch {
					} finally {
						command.Connection.Close ();
					}
				}
			}
			return set;
		}

		public override DataTable ExecuteQueryAsDataTable (string sql)
		{
			if (String.IsNullOrEmpty ("sql"))
				throw new ArgumentException ("sql");

			DataTable table = new DataTable ();
			using (IDbCommand command = CreateCommand (sql)) {
				using (MySqlDataAdapter adapter = new MySqlDataAdapter (command as MySqlCommand)) {
					try {
						adapter.Fill (table);
					} catch {
					} finally {
						command.Connection.Close ();
					}
				}
			}
			return table;
		}

		public override bool Open (out string errorMessage)
		{
			string connStr = null;
			try {	
				if (settings.UseConnectionString) {
					connStr = settings.ConnectionString;
				} else {
					//"Server=Server;Port=1234;Database=Test;Uid=UserName;Pwd=asdasd;Pooling=true;"
					//Default port is 3306. Enter value -1 to use a named pipe connection. 
					connStr = String.Format ("Server={0};Port={1};Database={2};Uid={3};Pwd={4};Pooling=true;",
						settings.Server, settings.Port, settings.Database, settings.Username, settings.Password);
				}
				SetConnectionStringParameter (connStr, String.Empty, "pooling", settings.EnablePooling.ToString ());
				if (settings.EnablePooling) {
					SetConnectionStringParameter (connStr, String.Empty, "MinPoolSize", settings.MinPoolSize.ToString ());
					SetConnectionStringParameter (connStr, String.Empty, "MaxPoolSize", settings.MaxPoolSize.ToString ());
				}
				connection = new MySqlConnection (connStr);
				connection.Open ();
				
				errorMessage = String.Empty;
				isConnectionError = false;
				return true;
			} catch {
				isConnectionError = true;
				errorMessage = String.Format ("Unable to connect. (CS={0})", connStr == null ? "NULL" : connStr);
				return false;
			}
		}
	}
}
