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

namespace MonoDevelop.Database.Sql
{
	// see: http://www.sqlite.org/faq.html
	// http://www.sqlite.org/google-talk-slides/page-021.html
	public class SqliteSchemaProvider : AbstractSchemaProvider
	{
		public SqliteSchemaProvider (IConnectionPool connectionPool)
			: base (connectionPool)
		{
		}
		
		public override bool SupportsSchemaOperation (SchemaOperation operation)
		{
			switch (operation.Statement) {
			case SqlStatementType.Select:
				switch (operation.Schema) {
				case SqlSchemaType.Table:
				case SqlSchemaType.Column:
				case SqlSchemaType.View:
				case SqlSchemaType.Constraint:
				case SqlSchemaType.Trigger:
					return true;
				default:
					return false;
				}
			case SqlStatementType.Create:
			case SqlStatementType.Drop:
			case SqlStatementType.Rename:
				switch (operation.Schema) {
				case SqlSchemaType.Table:
				case SqlSchemaType.Column:
				case SqlSchemaType.View:
				case SqlSchemaType.Constraint:
				case SqlSchemaType.Trigger:
					return true;
				default:
					return false;
				}
			case SqlStatementType.Alter:
				return operation.Schema == SqlSchemaType.Table;
			default:
				return false;
			}
		}

		public override TableSchemaCollection GetTables ()
		{
			TableSchemaCollection tables = new TableSchemaCollection ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (
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
			conn.Release ();

			return tables;
		}
		
		public override ColumnSchemaCollection GetTableColumns (TableSchema table)
		{
			ColumnSchemaCollection columns = new ColumnSchemaCollection ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (
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
			conn.Release ();

			return columns;
		}
		
		public override ViewSchemaCollection GetViews ()
		{
			ViewSchemaCollection views = new ViewSchemaCollection ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (
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
			conn.Release ();

			return views;
		}
		
		public override ConstraintSchemaCollection GetTableConstraints (TableSchema table)
		{
			ConstraintSchemaCollection constraints = new ConstraintSchemaCollection ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("SELECT name, tbl_name FROM sqlite_master WHERE sql IS NULL AND type = 'index'");
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
			conn.Release ();

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

		//http://www.sqlite.org/lang_createtable.html
		public override void CreateTable (TableSchema table)
		{
			throw new NotImplementedException ();
		}

		//http://www.sqlite.org/lang_createview.html
		public override void CreateView (ViewSchema view)
		{
			throw new NotImplementedException ();
		}

		//http://www.sqlite.org/lang_createindex.html
		public override void CreateConstraint (ConstraintSchema constraint)
		{
			throw new NotImplementedException ();
		}
		
		//http://www.sqlite.org/lang_createtrigger.html
		public override void CreateTrigger (TriggerSchema trigger)
		{
			throw new NotImplementedException ();
		}

		//http://www.sqlite.org/lang_altertable.html
		//http://www.sqlite.org/lang_vacuum.html
		public override void AlterTable (TableSchema table)
		{
			throw new NotImplementedException ();
		}

		//http://www.sqlite.org/lang_droptable.html
		public override void DropTable (TableSchema table)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("DROP TABLE IF EXISTS " + table.Name);
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
		}

		//http://www.sqlite.org/lang_dropview.html
		public override void DropView (ViewSchema view)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("DROP VIEW IF EXISTS " + view.Name);
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
		}

		//http://www.sqlite.org/lang_dropindex.html
		public override void DropConstraint (ConstraintSchema constraint)
		{
			if (constraint is IndexConstraintSchema) {
				IPooledDbConnection conn = connectionPool.Request ();
				IDbCommand command = conn.CreateCommand ("DROP INDEX IF EXISTS " + constraint.Name);
				using (command)
					command.ExecuteNonQuery ();
				conn.Release ();
			}
		}
		
		//http://www.sqlite.org/lang_droptrigger.html
		public override void DropTrigger (TriggerSchema trigger)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("DROP TRIGGER IF EXISTS " + trigger.Name);
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
		}

		//http://www.sqlite.org/lang_altertable.html
		public override void RenameTable (TableSchema table, string name)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("ALTER TABLE " + table.Name + " RENAME TO " + name);
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
			
			table.Name = name;
		}

		public override void RenameView (ViewSchema view, string name)
		{
			DropView (view);
			view.Name = name;
			CreateView (view);
		}

		public override void RenameConstraint (ConstraintSchema constraint, string name)
		{
			DropConstraint (constraint);
			constraint.Name = name;
			CreateConstraint (constraint);
		}
		
		public override void RenameTrigger (TriggerSchema trigger, string name)
		{
			DropTrigger (trigger);
			trigger.Name = name;
			CreateTrigger (trigger);
		}
	}
}
