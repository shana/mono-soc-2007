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
	// see: http://www.sqlite.org/faq.html
	// http://www.sqlite.org/google-talk-slides/page-021.html
	public class SqliteSchemaProvider : AbstractSchemaProvider
	{
		public SqliteSchemaProvider (IConnectionProvider connectionProvider)
			: base (connectionProvider)
		{
		}
		
		public override bool SupportsSchemaType (Type type)
		{
			if (type == typeof(TableSchema))
				return true;
			if (type == typeof(ColumnSchema))
				return true;
			else if (type == typeof(UserSchema))
				return false;
			else
				return false;
		}

		public override ICollection<TableSchema> GetTables ()
		{
			CheckConnectionState ();
			List<TableSchema> tables = new List<TableSchema> ();
			
			IDbCommand command = connectionProvider.CreateCommand (
				"SELECT name, sql FROM sqlite_master WHERE type = 'table'"
			);
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
						TableSchema table = new TableSchema (this);
	
						table.Name = r.GetString (0);
						table.IsSystemTable = table.Name.StartsWith ("sqlite_");
						table.Definition = r.GetString (1);
						
						tables.Add (table);
					}
					r.Close ();
				}
			}

			return tables;
		}
		
		public override ICollection<ColumnSchema> GetTableColumns (TableSchema table)
		{
			CheckConnectionState ();
			List<ColumnSchema> columns = new List<ColumnSchema> ();
			
			IDbCommand command = connectionProvider.CreateCommand (
				"PRAGMA table_info('" +  table.Name + "')"
			);
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
						ColumnSchema column = new ColumnSchema (this);
		
						column.ColumnID = r.GetInt32 (0);
						column.Name = r.GetString (1);
						column.DataTypeName = r.GetString (2);
						column.NotNull = r.IsDBNull (3);
						column.Default = r.GetString (4);
		
						columns.Add (column);
					}
					r.Close ();
				};
			}

			return columns;
		}
		
		public override ICollection<ViewSchema> GetViews ()
		{
			CheckConnectionState ();
			List<ViewSchema> views = new List<ViewSchema> ();
			
			IDbCommand command = connectionProvider.CreateCommand (
				"SELECT name, sql FROM sqlite_master WHERE type = 'views'"
			);
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
						ViewSchema view = new ViewSchema (this);
	
						view.Name = r.GetString (0);
						view.Definition = r.GetString (1);
						
						views.Add (view);
					}
					r.Close ();
				}
			}

			return views;
		}
		
		public override ICollection<ConstraintSchema> GetTableConstraints (TableSchema table)
		{
			CheckConnectionState ();
			List<ConstraintSchema> constraints = new List<ConstraintSchema> ();
			
			IDbCommand command = connectionProvider.CreateCommand ("SELECT name, tbl_name FROM sqlite_master WHERE sql IS NULL AND type = 'index'");
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
						ConstraintSchema constraint = null;
						
						if (r.IsDBNull (1) || r.GetString (1) == null) {
							constraint = new UniqueConstraintSchema (this);
						} else {
							ForeignKeyConstraintSchema fkc = new ForeignKeyConstraintSchema (this);
							fkc.ReferenceTableName = r.GetString (1);
							
							constraint = fkc;
						}
						constraint.Name = r.GetString (0);

						constraints.Add (constraint);
					}
					r.Close ();
				}
			}

			return constraints;
		}

		public override DataTypeSchema GetDataType (string name)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			name = name.ToUpper ();

			DataTypeSchema dts = new DataTypeSchema (this);
			dts.Name = name;
			switch (name) {
					//TODO: IMPLEMENT
				case "":
					break;
				default:
					dts = null;
					break;
			}
			
			return dts;
		}
	}
}
