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
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Mono.Data.Sql
{
	public class SqlSchemaProvider : SchemaProviderBase
	{
		public SqlSchemaProvider (SqlConnectionProvider connectionProvider)
			: base (connectionProvider)
		{
		}

		public override bool SupportsSchemaType (Type type)
		{
			if (type == typeof(TableSchema))
				return true;
			else if (type == typeof(ColumnSchema))
				return true;
			else if (type == typeof(ViewSchema))
				return true;
			else if (type == typeof(ProcedureSchema))
				return true;
			else if (type == typeof(UserSchema))
				return true;
			else if (type == typeof(SequenceSchema))
				return true;
			else if (type == typeof(TriggerSchema))
				return true;
			else if (type == typeof(DatabaseSchema))
				return true;
			else if (type == typeof(UserSchema))
				return true;
			else if (type == typeof(ConstraintsSchema))
				return true;
			else
				return false;
		}
		
		public override ICollection<DatabaseSchema> GetDatabases ()
		{
			CheckConnectionState ();
			List<DatabaseSchema> databases = new List<DatabaseSchema> ();
			
			//TODO: check if the current database is "master", the drawback is that this only works if you have sysadmin privileges
			IDbCommand command = connectionProvider.CreateCommand ("select name from sysdatabases");
			SqlDataReader r = command.ExecuteReader();
			
			while (r.Read ()) {
				DatabaseSchema db = new DatabaseSchema (this);
				db.Name = r.GetString (0);
				
			}
			r.Close ();
			r = null;
			command.Dispose ();
			command = null;

			return databases;
		}

		public override ICollection<TableSchema> GetTables ()
		{
			CheckConnectionState ();
			List<TableSchema> tables = new List<TableSchema> ();
			
			IDbCommand command = connectionProvider.CreateCommand (
				"SELECT su.name AS owner, so.name as table_name, so.id as table_id, " +
				" so.crdate as created_date, so.xtype as table_type " +
				" FROM dbo.sysobjects so, dbo.sysusers su " +
				"WHERE xtype IN ('S','U') " +
				"AND su.uid = so.uid " +
				"ORDER BY 1, 2"
			);
			SqlDataReader r = command.ExecuteReader();
			
			while (r.Read()) {
				TableSchema table = new TableSchema(this);
				table.Provider = this;
				table.Name = r.GetString(1);

				table.IsSystemTable = r.GetString(4) == "S" ? true : false;
				
				table.SchemaName = r.GetString(0);
				table.OwnerName = r.GetString(0);
				table.Comment = "";
				
				tables.Add (table);
			}
			r.Close ();
			r = null;
			command.Dispose ();
			command = null;
			
			return tables;
		}
		
		public override ICollection<ColumnSchema> GetTableColumns (TableSchema table)
		{
			CheckConnectionState ();
			List<ColumnSchema> columns = new List<ColumnSchema> ();
			
			IDbCommand command = connectionProvider.CreateCommand (
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
			SqlDataReader r = command.ExecuteReader();
			
			while (r.Read()) {
				ColumnSchema column = new ColumnSchema (this);
				
				column.Name = r.GetString (2);
				column.DataTypeName = r.GetString (3);
				column.Default = String.Empty;
				column.Comment = String.Empty;
				column.OwnerName = table.OwnerName;
				column.SchemaName = table.SchemaName;
				column.NotNull = r.GetValue (7).ToString () == "0" ? true : false;
				column.Length = r.GetInt32 (4);
				column.Precision = Convert.IsDBNull (r.GetValue (5)) ? 0 : r.GetInt32 (5);
				column.Scale = Convert.IsDBNull (r.GetValue (6)) ? 0 : r.GetInt32 (6);
				column.Definition = String.Concat (column.Name, " ", column.DataTypeName, " ",
					column.Length > 0 ? "(" + column.Length + ")" : "",
					column.NotNull ? " NOT NULL" : " NULL");
				//TODO: append " DEFAULT ..." if column.Default.Length > 0

				columns.Add (column);
			}
			r.Close ();
			r = null;
			command.Dispose ();
			command = null;
			
			return columns;
		}

		public override ICollection<ViewSchema> GetViews ()
		{
			CheckConnectionState ();
			List<ViewSchema> views = new List<ViewSchema> ();

			IDbCommand command = connectionProvider.CreateCommand (
				"SELECT su.name AS owner, so.name as table_name, so.id as table_id, " +
				" so.crdate as created_date, so.xtype as table_type " +
				"FROM dbo.sysobjects so, dbo.sysusers su " +
				"WHERE xtype = 'V' " +
				"AND su.uid = so.uid " +
				"ORDER BY 1, 2"
			);
			SqlDataReader r = command.ExecuteReader();
			
			while (r.Read()) {
				ViewSchema view = new ViewSchema (this);
				view.Provider = this;
				
				view.Name = r.GetString (1);
				view.SchemaName = r.GetString (0);
				view.OwnerName = r.GetString (0);
				
				views.Add (view);
			}
			r.Close ();
			r = null;
			command.Dispose();
			command = null;
			
			return views;
		}

		public override ICollection<ColumnSchema> GetViewColumns (ViewSchema view)
		{
			CheckConnectionState ();
			List<ColumnSchema> columns = new List<ColumnSchema> ();
			
			IDbCommand command = connectionProvider.CreateCommand (
				"SELECT * FROM \"" + view.Name +
				"\" WHERE 1 = 0"
			);
			SqlDataReader r = command.ExecuteReader();

			for (int i = 0; i < r.FieldCount; i++) {
				ColumnSchema column = new ColumnSchema();
				
				column.Name = r.GetName(i);
				column.DataTypeName = r.GetDataTypeName(i);
				column.Default = "";
				column.Definition = "";
				column.OwnerName = view.OwnerName;
				column.SchemaName = view.OwnerName;
				
				columns.Add (column);
			}
			r.Close ();
			r = null;
			command.Dispose ();
			command = null;

			return columns;
		}

		public override ICollection<ProcedureSchema> GetProcedures ()
		{
			CheckConnectionState ();
			List<ProcedureSchema> procedures = new List<ProcedureSchema> ();
			
			IDbCommand command = connectionProvider.CreateCommand (
				"SELECT su.name AS owner, so.name as proc_name, so.id as proc_id, " +
				" so.crdate as created_date, so.xtype as proc_type " +
				"FROM dbo.sysobjects so, dbo.sysusers su " +
				"WHERE xtype = 'P' " +
				"AND su.uid = so.uid " +
				"ORDER BY 1, 2"
			);
			SqlDataReader r = command.ExecuteReader();
			
			while (r.Read ()) {
				ProcedureSchema proc = new ProcedureSchema (this);
				proc.Provider = this;
				proc.Name = r.GetString (1);
				proc.OwnerName = r.GetString (0);
				proc.LanguageName = "TSQL";

				// FIXME : get sysproc or not
				procedures.Add (proc);
			}
			r.Close ();
			r = null;
			command.Dispose ();
			command = null;
			
			return procedures; 
		}

		public override ICollection<ColumnSchema> GetProcedureColumns (ProcedureSchema procedure)
		{
			CheckConnectionState ();
			List<ColumnSchema> columns = new List<ColumnSchema> ();
			
			SqlCommand command = connectionProvider.CreateCommand ("sp_sproc_columns") as SqlCommand;
			command.CommandType = CommandType.StoredProcedure;
			SqlParameter owner = command.Parameters.Add ("@procedure_owner", SqlDbType.VarChar);
			SqlParameter name = command.Parameters.Add ("@procedure_name", SqlDbType.VarChar);
			owner.Value = procedure.OwnerName;
			name.Value = procedure.Name;
				
			SqlDataReader r = command.ExecuteReader();

			while (r.Read ()) {
				ColumnSchema column = new ColumnSchema (this);
				column.Name = (string) r ["COLUMN_NAME"];
				column.DataTypeName = (string) r ["TYPE_NAME"];

				columns.Add (column);
			}
			r.Close ();
			r = null;
			command.Dispose ();
			command = null;
		      
			return columns;
		}
		
		public override ICollection<ParameterSchema> GetProcedureParameters (ProcedureSchema procedure)
		{
			CheckConnectionState ();
			throw new NotImplementedException ();
		}

		public override ICollection<ConstraintSchema> GetTableConstraints (TableSchema table)
		{
			CheckConnectionState ();
			List<ConstraintSchema> constraints = new List<ConstraintSchema> ();
			
			SqlCommand command = connectionProvider.CreateCommand ("select name, xtype from sysobjects where xtype = 'F'");
			SqlDataReader r = command.ExecuteReader();
			
			while (r.Read ()) {
				ConstraintSchema constraint = new ConstraintSchema (this);
			
				ColumnSchema pkColumn = new ColumnSchema (this);
				ColumnSchema fkColumn = new ColumnSchema (this);
				
				string name = r.GetString (0);
				string[] tmp = r.GetString (1).Split (new char[] ('_'));
				
				fkColumn.OwnerName = tmp[1];
				pkcolumn.OwnerName = tmp[2];
				
				constraint.ForeignKey = fkColumn;
				constraint.PrimaryKey = pkColumn;
				
				constraints.Add (constraint);
			}
			r.Close ();
			r = null;
			command.Dispose ();
			command = null;

			return constraints;
		}

		public override ICollection<UserSchema> GetUsers ()
		{
			CheckConnectionState ();
			throw new NotImplementedException ();
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
					dts.DataTypeCategory = DataTypeCategory.Boolean;
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
					dts.DataTypeCategory = DataTypeCategory.Time;
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
			}
			
			return dts;
		}
		
		public override string GetTableDefinition (TableSchema table)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat ("-- Table: {0}\n", table.Name);
			sb.AppendFormat ("-- DROP TABLE {0};\n\n", table.Name);
			sb.AppendFormat ("CREATE TABLE {0} (\n", table.Name);
				
			ICollection<ColumnSchema> columns = table.Columns;
			string[] parts = new string[columns.Count];
			for (int i = 0; i < parts.Length; i++)
				parts[i] = "\t" + columns[i].Definition;
			sb.Append (String.Join (",\n", parts));
				
			ICollection<ConstraintSchema> cons = table.Constraints;
			parts = new string[cons.Count];
			if (cons.Length > 0)
				sb.Append (",\n");
			for (int i = 0; i < parts.Length; i++)
				parts[i] = "\t" + cons[i].Definition;
			sb.Append (String.Join (",\n", parts));
				
			sb.Append ("\n);\n");
			//sb.AppendFormat ("COMMENT ON TABLE {0} IS '{1}';", table.Name, table.Comment);
			return sb.ToString ();
		}
		
		public override string GetViewDefinition (ViewSchema view)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat ("-- View: {0}\n", view.Name);
			sb.AppendFormat ("-- DROP VIEW {0};\n\n", view.Name);
			sb.AppendFormat ("  {0}\n);", GetSource (view.Owner + "." + view.Name));
			return sb.ToString ();
		}
		
		public override string GetProcedureDefinition (ProcedureSchema procedure)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat ("-- Procedure: {0}\n", procedure.Name);
			sb.AppendFormat ("  {0}\n);", GetSource (procedure.Owner + "." + procedure.Name));
			return sb.ToString ();
		}
		
		private string GetSource (string objectName) 
		{
			string sql = ;
			CheckConnectionState ();
			IDbCommand command = connectionProvider.CreateCommand (
				String.Format ("EXEC [master].[dbo].[sp_helptext] '{0}', null", objectName)
			);
			IDataReader reader = command.ExecuteReader ();

			StringBuilder sb = new StringBuilder ();

			while (reader.Read ()) {
				string text = reader.GetString (0);
				sb.Append (text);
			}

			reader.Close ();
			reader = null;
			command.Dispose ();
			command = null;

			return sb.ToString ();
		}
	}
}
