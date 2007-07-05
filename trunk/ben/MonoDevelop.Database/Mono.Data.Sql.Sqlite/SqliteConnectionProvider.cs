//
// Authors:
//   Christian Hergert	<chris@mosaix.net>
//   Ankit Jain  <radical@corewars.org>
//   Ben Motmans  <ben.motmans@gmail.com>
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
using System.Collections.Generic;
using Mono.Data.SqliteClient;

namespace Mono.Data.Sql
{
	public class SqliteConnectionProvider : AbstractConnectionProvider
	{
		public SqliteConnectionProvider (IDbFactory factory, ConnectionSettings settings)
			: base (factory, settings)
		{
		}

		public override bool SupportsPooling {
			get { return false; }
		}
		
		public override DataSet ExecuteQueryAsDataSet (string sql)
		{
			if (String.IsNullOrEmpty ("sql"))
				throw new ArgumentException ("sql");

			DataSet set = new DataSet ();
			using (IDbCommand command = CreateCommand (sql)) {
				using (SqliteDataAdapter adapter = new SqliteDataAdapter (command as SqliteCommand)) {
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
				using (SqliteDataAdapter adapter = new SqliteDataAdapter (command as SqliteCommand)) {
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
			SqliteConnectionStringBuilder builder = null;
			try {	
				if (settings.UseConnectionString) {
					builder = new SqliteConnectionStringBuilder (settings.ConnectionString);
				} else {
					builder = new SqliteConnectionStringBuilder ();
					builder.DataSource = settings.Database;
				}
				connection = new SqliteConnection (builder.ToString ());
				connection.Open ();
				
				errorMessage = String.Empty;
				return true;
			} catch {
				errorMessage = String.Format ("Unable to connect. (CS={0})", builder == null ? "NULL" : builder.ToString ());
				return false;
			}
		}
	}
}
