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
using System.Text;
using System.Data;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using MonoDevelop.Core;

namespace MonoDevelop.Database.Sql
{
	// see: http://www.sqlite.org/faq.html
	// http://www.sqlite.org/google-talk-slides/page-021.html
	[TableMetaData (TableMetaData.Name | TableMetaData.Schema | TableMetaData.Columns | TableMetaData.Definition | TableMetaData.Triggers | TableMetaData.IsSystem | TableMetaData.PrimaryKeyConstraints | TableMetaData.CheckConstraints | TableMetaData.UniqueConstraints)]
	[ViewMetaData (ViewMetaData.Name | ViewMetaData.Schema | ViewMetaData.Definition)]
	[TableColumnMetaData (ColumnMetaData.Name | ColumnMetaData.Definition | ColumnMetaData.Schema | ColumnMetaData.DataType | ColumnMetaData.DefaultValue | ColumnMetaData.Nullable | ColumnMetaData.Position | ColumnMetaData.PrimaryKeyConstraints | ColumnMetaData.CheckConstraints | ColumnMetaData.UniqueConstraint)]
	public class SqliteSchemaProvider : AbstractSchemaProvider
	{
		public SqliteSchemaProvider (IConnectionPool connectionPool)
			: base (connectionPool)
		{
		}
		
		public override bool SupportsSchemaOperation (SchemaOperation operation)
		{
			switch (operation.Operation) {
			case OperationMetaData.Select:
				switch (operation.Schema) {
				case SchemaMetaData.Table:
				case SchemaMetaData.Column:
				case SchemaMetaData.View:
				case SchemaMetaData.Constraint:
				case SchemaMetaData.Trigger:
					return true;
				default:
					return false;
				}
			case OperationMetaData.Create:
			case OperationMetaData.Drop:
			case OperationMetaData.Rename:
				switch (operation.Schema) {
				case SchemaMetaData.Database:
				case SchemaMetaData.Table:
				case SchemaMetaData.Column:
				case SchemaMetaData.View:
				case SchemaMetaData.Constraint:
				case SchemaMetaData.Trigger:
					return true;
				default:
					return false;
				}
			case OperationMetaData.Alter:
				return operation.Schema == SchemaMetaData.Table;
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
	
						table.SchemaName = "main";
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
		
						column.Position = r.GetInt32 (0);
						column.Name = r.GetString (1);
						column.DataTypeName = r.GetString (2);
						column.IsNullable = r.GetInt32 (3) == 0;
						column.DefaultValue = r.GetString (4);
		
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
	
						view.SchemaName = "main";
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
			return GetConstraints (table, null);
		}
		
		public virtual ConstraintSchemaCollection GetColumnConstraints (TableSchema table, ColumnSchema column)
		{
			return GetConstraints (table, column);
		}

		//http://www.sqlite.org/pragma.html
		public virtual ConstraintSchemaCollection GetConstraints (TableSchema table, ColumnSchema column)
		{
			if (table == null)
				throw new ArgumentNullException ("table");
			string columnName = column == null ? null : column.Name;
			
			ConstraintSchemaCollection constraints = new ConstraintSchemaCollection ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			
			//fk and unique
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
			
			//pk, column
			if (columnName != null) {
				command = conn.CreateCommand (
					"PRAGMA table_info('" +  table.Name + "')"
				);
				using (command) {
					using (IDataReader r = command.ExecuteReader()) {
						while (r.Read ()) {
							if (r.GetInt32 (5) == 1 && r.GetString (1) == columnName) {
								PrimaryKeyConstraintSchema constraint = new PrimaryKeyConstraintSchema (this);
							
								ColumnSchema priColumn = new ColumnSchema (this);
								priColumn.Name = r.GetString (1);
								
								constraint.Columns.Add (priColumn);
								constraint.IsColumnConstraint = true;
								constraint.Name = "pk_" + table.Name + "_" + priColumn.Name;
								
								constraints.Add (constraint);
							}
						}
						r.Close ();
					}
				}
			}
			
			conn.Release ();

			return constraints;
		}
		
		public override DataTypeSchemaCollection GetDataTypes ()
		{
			//FIXME: this override should be removed, but needed for now cause of a sqlite bug
			DataTypeSchemaCollection col = new DataTypeSchemaCollection ();
			DataTypeSchema s1 = new DataTypeSchema (this);
			DataTypeSchema s2 = new DataTypeSchema (this);
			DataTypeSchema s3 = new DataTypeSchema (this);
			DataTypeSchema s4 = new DataTypeSchema (this);
			s1.Name = "INTEGER";
			s2.Name = "REAL";
			s3.Name = "TEXT";
			s4.Name = "BLOB";
			col.Add (s1);
			col.Add (s2);
			col.Add (s3);
			col.Add (s4);
			return col;
		}

		//http://www.sqlite.org/datatype3.html
		protected override void ProvideDataTypeInformation (DataTypeSchema schema)
		{
			string name = schema.Name.ToUpper ();
			
			if (name.Contains ("INT")) {
				schema.DataTypeCategory = DataTypeCategory.Integer;
				schema.LengthRange = new Range (1, 8, 4);
			} else if (name.Contains ("REAL") || name.Contains ("FLOA") || name.Contains ("DOUB")) {
				schema.DataTypeCategory = DataTypeCategory.Float;
			} else if (name.Contains ("CHAR") || name.Contains ("CLOB") || name.Contains ("TEXT")) {
				schema.DataTypeCategory = DataTypeCategory.NVarChar;
			} else if (name.Contains ("BLOB")) {
				schema.DataTypeCategory = DataTypeCategory.Binary;
			} else {
				schema.DataTypeCategory = DataTypeCategory.Float;
			}
		}
		
		public override void CreateDatabase (DatabaseSchema database)
		{
			//TODO: error if db exists
			Runtime.LoggingService.Error ("CREATE START");
			
			SqliteConnection conn = new SqliteConnection ("URI=file:" + database.Name + ";Version=3;");
			conn.Open ();
			conn.Close ();
			
			Runtime.LoggingService.Error ("CREATE STOP");
		}

		//http://www.sqlite.org/lang_createtable.html
		public override void CreateTable (TableSchema table)
		{
			StringBuilder sb = new StringBuilder ();
			
			sb.Append ("CREATE TABLE ");
			sb.Append (table.Name);
			sb.Append (" ( ");

			foreach (ColumnSchema column in table.Columns) {
				sb.Append (column.Name);
				sb.Append (' ');
				sb.Append (column.DataTypeName); //FIXME: scale, pres, ...
				
				if (!column.IsNullable)
					sb.Append (" NOT NULL");
				if (column.HasDefaultValue)
					sb.Append (" DEFAULT " + column.DefaultValue == null ? "NULL" : column.DefaultValue.ToString ()); //TODO: '' chars if string
				
				//list all column constrains for this type
				foreach (ConstraintSchema constraint in column.Constraints) {
					sb.Append (" ");
					sb.Append (GetConstraintString (constraint));
				}
			}
			
			//table constraints
			foreach (ConstraintSchema constraint in table.Constraints) {
				sb.Append (", ");
				sb.Append (GetConstraintString (constraint));
			}
			
			sb.Append (" );");
			
			string sql = sb.ToString ();
			Runtime.LoggingService.Debug (sql);
			
//			IPooledDbConnection conn = connectionPool.Request ();
//			IDbCommand command = conn.CreateCommand (sql);
//			using (command)
//				conn.ExecuteNonQuery (command);
//			conn.Release ();
		}
		
		protected virtual string GetConstraintString (ConstraintSchema constraint)
		{
			//PRIMARY KEY [sort-order] [ conflict-clause ] [AUTOINCREMENT]
			//UNIQUE [ conflict-clause ]
			//CHECK ( expr )
			//COLLATE collation-name
			
			switch (constraint.ConstraintType) {
			case ConstraintType.PrimaryKey:
				//PrimaryKeyConstraintSchema pk = constraint as PrimaryKeyConstraintSchema;
				return "PRIMARY KEY"; //TODO: auto inc + sort
			case ConstraintType.Unique:
				//UniqueConstraintSchema u = constraint as UniqueConstraintSchema;
				return "UNIQUE";
			case ConstraintType.Check:
				CheckConstraintSchema chk = constraint as CheckConstraintSchema;
				return String.Concat ("CHECK (", chk.Source, ")");
			default:
				throw new NotImplementedException ();
			}
			
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
		
		public override void DropDatabase (DatabaseSchema db)
		{
			connectionPool.Close ();
			System.IO.File.Delete (db.Name);
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
//			if (constraint is IndexConstraintSchema) {
//				IPooledDbConnection conn = connectionPool.Request ();
//				IDbCommand command = conn.CreateCommand ("DROP INDEX IF EXISTS " + constraint.Name);
//				using (command)
//					command.ExecuteNonQuery ();
//				conn.Release ();
//			}
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
