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
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THEC
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using MonoDevelop.Core;
namespace MonoDevelop.Database.Sql
{
	// see:
	// http://www.alberton.info/sql_server_meta_info.html
	public class SqlServerSchemaProvider : AbstractSchemaProvider
	{
		public SqlServerSchemaProvider (IConnectionPool connectionPool)
			: base (connectionPool)
		{
		}
		
		public override DatabaseSchemaCollection GetDatabases ()
		{
			DatabaseSchemaCollection databases = new DatabaseSchemaCollection ();
			
			//TODO: check if the current database is "master", the drawback is that this only works if you have sysadmin privileges
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("select name from sysdatabases");
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read()) {
						DatabaseSchema db = new DatabaseSchema (this);
						db.Name = r.GetString (0);
						databases.Add (db);
					}
					r.Close ();
				}
			}
			conn.Release ();

			return databases;
		}

		public override TableSchemaCollection GetTables ()
		{
			TableSchemaCollection tables = new TableSchemaCollection ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (
				"SELECT su.name AS owner, so.name as table_name, so.id as table_id, " +
				" so.crdate as created_date, so.xtype as table_type " +
				" FROM dbo.sysobjects so, dbo.sysusers su " +
				"WHERE xtype IN ('S','U') " +
				"AND su.uid = so.uid " +
				"ORDER BY 1, 2"
			);
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read()) {
						TableSchema table = new TableSchema (this);
						table.Name = r.GetString(1);

						table.IsSystemTable = r.GetString(4) == "S" ? true : false;
						
						table.SchemaName = r.GetString(0);
						table.OwnerName = r.GetString(0);
						table.Definition = GetTableDefinition (table);
						
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
				"SELECT su.name as owner, so.name as table_name, " +
				"   sc.name as column_name, " +
				"   st.name as date_type, sc.length as column_length,  " +
				"   sc.xprec as data_preceision, sc.xscale as data_scale, " +
				"   sc.isnullable, sc.colid as column_id " +
				"FROM dbo.syscolumns sc, dbo.sysobjects so, " +
				"   dbo.systypes st, dbo.sysusers su " +
				"WHERE sc.id = so.id " +
				"AND so.xtype in ('U','S') " +
				"AND so.name = '" + table.Name + "' " +
				"AND su.name = '" + table.OwnerName + "' " +
				"AND sc.xusertype = st.xusertype " +
				"AND su.uid = so.uid " +
				"ORDER BY sc.colid"
			);
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read()) {
						ColumnSchema column = new ColumnSchema (this, table);
						
						column.Name = r.GetString (2);
						column.DataTypeName = r.GetString (3);
						column.DefaultValue = String.Empty;
						column.Comment = String.Empty;
						column.OwnerName = table.OwnerName;
						column.SchemaName = table.SchemaName;
						column.IsNullable = r.GetValue (7).ToString () == "0" ? true : false;
						column.DataType.LengthRange.Default = r.GetInt32 (4);
						column.DataType.PrecisionRange.Default = r.IsDBNull (5) ? 0 : r.GetInt32 (5);
						column.DataType.ScaleRange.Default = r.IsDBNull (6) ? 0 : r.GetInt32 (6);
						column.Definition = String.Concat (column.Name, " ", column.DataTypeName, " ",
							column.DataType.LengthRange.Default > 0 ? "(" + column.DataType.LengthRange.Default + ")" : "",
							column.IsNullable ? " NULL" : " NOT NULL");
						//TODO: append " DEFAULT ..." if column.Default.Length > 0

						columns.Add (column);
					}
					r.Close ();
				}
			}
			conn.Release ();
			
			return columns;
		}

		public override ViewSchemaCollection GetViews ()
		{
			ViewSchemaCollection views = new ViewSchemaCollection ();

			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (
				"SELECT su.name AS owner, so.name as table_name, so.id as table_id, " +
				" so.crdate as created_date, so.xtype as table_type " +
				"FROM dbo.sysobjects so, dbo.sysusers su " +
				"WHERE xtype = 'V' " +
				"AND su.uid = so.uid " +
				"ORDER BY 1, 2"
			);
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read()) {
						ViewSchema view = new ViewSchema (this);
						
						view.Name = r.GetString (1);
						view.SchemaName = r.GetString (0);
						view.OwnerName = r.GetString (0);
						
						StringBuilder sb = new StringBuilder();
						sb.AppendFormat ("-- View: {0}\n", view.Name);
						sb.AppendFormat ("-- DROP VIEW {0};\n\n", view.Name);
						sb.AppendFormat ("  {0}\n);", GetSource (view.Owner + "." + view.Name));
						view.Definition = sb.ToString ();
						
						views.Add (view);
					}
					r.Close ();
				}
			}
			conn.Release ();
			
			return views;
		}

		public override ColumnSchemaCollection GetViewColumns (ViewSchema view)
		{
			ColumnSchemaCollection columns = new ColumnSchemaCollection ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (
				"SELECT * FROM \"" + view.Name +
				"\" WHERE 1 = 0"
			);
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					for (int i = 0; i < r.FieldCount; i++) {
						ColumnSchema column = new ColumnSchema (this, view);
						
						column.Name = r.GetName(i);
						column.DataTypeName = r.GetDataTypeName(i);
						column.DefaultValue = "";
						column.Definition = "";
						column.OwnerName = view.OwnerName;
						column.SchemaName = view.OwnerName;
						
						columns.Add (column);
					}
					r.Close ();
				}
			}
			conn.Release ();
			
			return columns;
		}

		public override ProcedureSchemaCollection GetProcedures ()
		{
			ProcedureSchemaCollection procedures = new ProcedureSchemaCollection ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (
				"SELECT su.name AS owner, so.name as proc_name, so.id as proc_id, " +
				" so.crdate as created_date, so.xtype as proc_type " +
				"FROM dbo.sysobjects so, dbo.sysusers su " +
				"WHERE xtype = 'P' " +
				"AND su.uid = so.uid " +
				"ORDER BY 1, 2"
			);
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
						ProcedureSchema procedure = new ProcedureSchema (this);
						procedure.Name = r.GetString (1);
						procedure.OwnerName = r.GetString (0);
						procedure.LanguageName = "TSQL";
						
						StringBuilder sb = new StringBuilder();
						sb.AppendFormat ("-- Procedure: {0}\n", procedure.Name);
						sb.AppendFormat ("  {0}\n);", GetSource (procedure.Owner + "." + procedure.Name));
						procedure.Definition = sb.ToString ();

						// FIXME : get sysproc or not
						procedures.Add (procedure);
					}
					r.Close ();
				}
			}
			conn.Release ();
			
			return procedures; 
		}

		public override ConstraintSchemaCollection GetTableConstraints (TableSchema table)
		{
			ConstraintSchemaCollection constraints = new ConstraintSchemaCollection ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("select name, xtype from sysobjects where xtype = 'F'");
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
						string name = r.GetString (0);
						string[] tmp = r.GetString (1).Split ('_');
	
						ForeignKeyConstraintSchema constraint = new ForeignKeyConstraintSchema (this);
						constraint.ReferenceTableName = tmp[1];
						constraint.Name = name;
						
						constraints.Add (constraint);
					}
					r.Close ();
				}
			}
			conn.Release ();

			return constraints;
		}
		
		// see:
		// http://www.firebirdsql.org/manual/migration-mssql-data-types.html
		// http://webcoder.info/reference/MSSQLDataTypes.html
		// http://www.tar.hu/sqlbible/sqlbible0022.html
		// http://msdn2.microsoft.com/en-us/library/aa258876(SQL.80).aspx
		public override DataTypeSchema GetDataType (string name)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			name = name.ToLower ();

			DataTypeSchema dts = new DataTypeSchema (this);
			dts.Name = name;
			switch (name) {
				case "bigint":
					dts.LengthRange = new Range (8);
					dts.PrecisionRange = new Range (1, 19);
					dts.DataTypeCategory = DataTypeCategory.Integer;
					break;
				case "binary":
					dts.LengthRange = new Range (1, 8004);
					dts.PrecisionRange = new Range (1, 8000);
					dts.DataTypeCategory = DataTypeCategory.Binary;
					break;
				case "bit":
					dts.LengthRange = new Range (1);
					dts.DataTypeCategory = DataTypeCategory.Bit;
					break;
				case "char":
					dts.LengthRange = new Range (1, 8000);
					dts.PrecisionRange = new Range (1, 8000);
					dts.DataTypeCategory = DataTypeCategory.Char;
					break;
				case "datetime":
					dts.LengthRange = new Range (8);
					dts.DataTypeCategory = DataTypeCategory.DateTime;
					break;
				case "decimal":
					dts.LengthRange = new Range (5, 17);
					dts.PrecisionRange = new Range (1, 38);
					dts.ScaleRange = new Range (0, 37);
					dts.DataTypeCategory = DataTypeCategory.Float;
					break;
				case "float":
					dts.LengthRange = new Range (8);
					dts.ScaleRange = new Range (1, 15);
					dts.DataTypeCategory = DataTypeCategory.Float;
					break;
				case "image":
					dts.LengthRange = new Range (0, int.MaxValue);
					dts.PrecisionRange = new Range (0, int.MaxValue);
					dts.DataTypeCategory = DataTypeCategory.VarBinary;
					break;
				case "int":
					dts.LengthRange = new Range (4);
					dts.PrecisionRange = new Range (1, 10);
					dts.DataTypeCategory = DataTypeCategory.Integer;
					break;
				case "money":
					dts.LengthRange = new Range (8);
					dts.PrecisionRange = new Range (1, 19);
					dts.ScaleRange = new Range (4);
					dts.DataTypeCategory = DataTypeCategory.Integer;
					break;
				case "nchar":
					dts.LengthRange = new Range (2, 8000);
					dts.PrecisionRange = new Range (1, 4000);
					dts.DataTypeCategory = DataTypeCategory.NChar;
					break;
				case "ntext":
					dts.LengthRange = new Range (0, int.MaxValue);
					dts.PrecisionRange = new Range (0, 1073741823);
					dts.DataTypeCategory = DataTypeCategory.NVarChar;
					break;
				case "numeric":
					dts.LengthRange = new Range (5, 17);
					dts.PrecisionRange = new Range (1, 38);
					dts.ScaleRange = new Range (0, 37);
					dts.DataTypeCategory = DataTypeCategory.Float;
					break;
				case "nvarchar":
					dts.LengthRange = new Range (0, 8000);
					dts.PrecisionRange = new Range (0, 4000);
					dts.DataTypeCategory = DataTypeCategory.NVarChar;
					break;
				case "real":
					dts.LengthRange = new Range (4);
					dts.ScaleRange = new Range (7);
					dts.DataTypeCategory = DataTypeCategory.Float;
					break;
				case "smalldatetime":
					dts.LengthRange = new Range (4);
					dts.DataTypeCategory = DataTypeCategory.DateTime;
					break;
				case "smallint":
					dts.LengthRange = new Range (2);
					dts.PrecisionRange = new Range (5);
					dts.DataTypeCategory = DataTypeCategory.Integer;
					break;
				case "smallmoney":
					dts.LengthRange = new Range (4);
					dts.PrecisionRange = new Range (10);
					dts.ScaleRange = new Range (4);
					dts.DataTypeCategory = DataTypeCategory.Integer;
					break;
				case "text":
					dts.LengthRange = new Range (0, int.MaxValue);
					dts.PrecisionRange = new Range (0, int.MaxValue);
					dts.DataTypeCategory = DataTypeCategory.VarChar;
					break;
				case "timestamp":
					dts.LengthRange = new Range (1, 8);
					dts.DataTypeCategory = DataTypeCategory.TimeStamp;
					break;
				case "tinyint":
					dts.LengthRange = new Range (1);
					dts.PrecisionRange = new Range (1, 3);
					dts.DataTypeCategory = DataTypeCategory.Integer;
					break;
				case "varbinary":
					dts.LengthRange = new Range (1, 8004);
					dts.PrecisionRange = new Range (0, 8000);
					dts.DataTypeCategory = DataTypeCategory.VarBinary;
					break;
				case "varchar":
					dts.LengthRange = new Range (1, 8000);
					dts.PrecisionRange = new Range (0, 8000);
					dts.DataTypeCategory = DataTypeCategory.VarChar;
					break;
				case "uniqueidentifier":
					dts.LengthRange = new Range (16);
					dts.DataTypeCategory = DataTypeCategory.Uid;
					break;
				case "xml":
					dts.LengthRange = new Range (0, int.MaxValue);
					dts.PrecisionRange = new Range (0, int.MaxValue);
					dts.DataTypeCategory = DataTypeCategory.VarChar;
					break;
				case "cursor":
				case "table":
				case "sql_variant":
					dts.DataTypeCategory = DataTypeCategory.Other;
					break;
				default:
					dts = null;
					break;
			}
			
			return dts;
		}
		
		//http://msdn2.microsoft.com/en-us/library/aa258257(SQL.80).aspx
		public override void CreateDatabase (DatabaseSchema database)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("CREATE DATABASE " + database.Name);
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
		}

		//http://msdn2.microsoft.com/en-us/library/aa258259(SQL.80).aspx
		public override void CreateTable (TableSchema table)
		{
			throw new NotImplementedException ();
		}

		//http://msdn2.microsoft.com/en-us/library/aa258254(SQL.80).aspx
		public override void CreateView (ViewSchema view)
		{
			throw new NotImplementedException ();
		}

		//http://msdn2.microsoft.com/en-us/library/aa258259(SQL.80).aspx
		public override void CreateProcedure (ProcedureSchema procedure)
		{
			throw new NotImplementedException ();
		}

		//http://msdn2.microsoft.com/en-us/library/aa258259(SQL.80).aspx
		public override void CreateIndex (IndexSchema index)
		{
			throw new NotImplementedException ();
		}
		
		//http://msdn2.microsoft.com/en-us/library/aa258254(SQL.80).aspx
		public override void CreateTrigger (TriggerSchema trigger)
		{
			throw new NotImplementedException ();
		}
		
		//http://msdn2.microsoft.com/en-us/library/aa275464(SQL.80).aspx
		public override void AlterDatabase (DatabaseSchema database)
		{
			throw new NotImplementedException ();
		}

		//http://msdn2.microsoft.com/en-us/library/aa225939(SQL.80).aspx
		public override void AlterTable (TableSchema table)
		{
			throw new NotImplementedException ();
		}

		//http://msdn2.microsoft.com/en-us/library/aa225939(SQL.80).aspx
		public override void AlterView (ViewSchema view)
		{
			throw new NotImplementedException ();
		}

		//http://msdn2.microsoft.com/en-us/library/aa225939(SQL.80).aspx
		public override void AlterProcedure (ProcedureSchema procedure)
		{
			throw new NotImplementedException ();
		}

		public override void AlterIndex (IndexSchema index)
		{
			throw new NotImplementedException ();
		}
		
		//http://msdn2.microsoft.com/en-us/library/aa225939(SQL.80).aspx
		public override void AlterTrigger (TriggerSchema trigger)
		{
			throw new NotImplementedException ();
		}

		public override void AlterUser (UserSchema user)
		{
			throw new NotImplementedException ();
		}
		
		//http://msdn2.microsoft.com/en-us/library/aa258843(SQL.80).aspx
		public override void DropDatabase (DatabaseSchema database)
		{
			ExecuteNonQuery ("DROP DATABASE " + database.Name);
		}

		//http://msdn2.microsoft.com/en-us/library/aa258841(SQL.80).aspx
		public override void DropTable (TableSchema table)
		{
			ExecuteNonQuery ("DROP TABLE " + table.Name);
		}

		//http://msdn2.microsoft.com/en-us/library/aa258835(SQL.80).aspx
		public override void DropView (ViewSchema view)
		{
			ExecuteNonQuery ("DROP VIEW " + view.Name);
		}

		//http://msdn2.microsoft.com/en-us/library/aa258830(SQL.80).aspx
		public override void DropProcedure (ProcedureSchema procedure)
		{
			ExecuteNonQuery ("DROP PROCEDURE " + procedure.Name);
		}

		//http://msdn2.microsoft.com/en-us/library/aa225939(SQL.80).aspx
		public override void DropIndex (IndexSchema index)
		{
			ExecuteNonQuery ("DROP INDEX '" + index.TableName + "." + index.Name + "'");
		}
		
		//http://msdn2.microsoft.com/en-us/library/aa258846(SQL.80).aspx
		public override void DropTrigger (TriggerSchema trigger)
		{
			ExecuteNonQuery ("DROP TRIGGER " + trigger.Name);
		}
		
		//http://msdn2.microsoft.com/en-US/library/aa238878(SQL.80).aspx
		public override void RenameDatabase (DatabaseSchema database, string name)
		{
			Rename (database.Name, name, "DATABASE");
			
			database.Name = name;
			connectionPool.ConnectionContext.ConnectionSettings.Database = name;
		}

		//http://msdn2.microsoft.com/en-US/library/aa238878(SQL.80).aspx
		public override void RenameTable (TableSchema table, string name)
		{
			Rename (table.Name, name, "OBJECT");
			table.Name = name;
		}

		//http://msdn2.microsoft.com/en-US/library/aa238878(SQL.80).aspx
		public override void RenameView (ViewSchema view, string name)
		{
			Rename (view.Name, name, "OBJECT");
			view.Name = name;
		}

		//http://msdn2.microsoft.com/en-US/library/aa238878(SQL.80).aspx
		public override void RenameProcedure (ProcedureSchema procedure, string name)
		{
			Rename (procedure.Name, name, "OBJECT");
			procedure.Name = name;
		}

		//http://msdn2.microsoft.com/en-US/library/aa238878(SQL.80).aspx
		public override void RenameIndex (IndexSchema index, string name)
		{
			Rename (index.Name, name, "INDEX");
			index.Name = name;
		}
		
		//http://msdn2.microsoft.com/en-US/library/aa238878(SQL.80).aspx
		public override void RenameTrigger (TriggerSchema trigger, string name)
		{
			Rename (trigger.Name, name, "OBJECT");
			trigger.Name = name;
		}
		
		public override string GetViewAlterStatement (ViewSchema view)
		{
			return String.Concat ("DROP VIEW ", view.Name, "; ", Environment.NewLine, view.Definition); 
		}
		
		public override string GetProcedureAlterStatement (ProcedureSchema procedure)
		{
			return String.Concat ("DROP PROCEDURE ", procedure.Name, "; ", Environment.NewLine, procedure.Definition);
		}
		
		protected string GetTableDefinition (TableSchema table)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat ("-- Table: {0}\n", table.Name);
			sb.AppendFormat ("-- DROP TABLE {0};\n\n", table.Name);
			sb.AppendFormat ("CREATE TABLE {0} (\n", table.Name);

			ColumnSchemaCollection columns = table.Columns;
			string[] parts = new string[columns.Count];
			int i = 0;
			foreach (ColumnSchema col in columns)
				parts[i++] = col.Definition;
			sb.Append (String.Join (",\n", parts));
				
			ConstraintSchemaCollection constraints = table.Constraints;
			parts = new string[constraints.Count];
			if (constraints.Count > 0)
				sb.Append (",\n");
			i = 0;
			foreach (ConstraintSchema constr in constraints)
				parts[i++] = "\t" + constr.Definition;
			sb.Append (String.Join (",\n", parts));
				
			sb.Append ("\n);\n");
			//sb.AppendFormat ("COMMENT ON TABLE {0} IS '{1}';", table.Name, table.Comment);
			return sb.ToString ();
		}

		private string GetSource (string objectName) 
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateStoredProcedure (
				String.Format ("EXEC [master].[dbo].[sp_helptext] '{0}', null", objectName)
			);
			StringBuilder sb = new StringBuilder ();
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read ())
						sb.Append (r.GetString (0));
					r.Close ();
				}
			}
			conn.Release ();

			return sb.ToString ();
		}
		
		//http://msdn2.microsoft.com/en-US/library/aa238878(SQL.80).aspx
		private void Rename (string oldName, string newName, string type) 
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateStoredProcedure (
				String.Format ("EXEC sp_rename '{0}', '{1}', '{2}'", oldName, newName, type)
			);
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
		}
	}
}
