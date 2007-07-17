//
// Authors:
//	Christian Hergert  <chris@mosaix.net>
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
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Npgsql;
namespace MonoDevelop.Database.Sql
{
	public class NpgsqlSchemaProvider : AbstractSchemaProvider
	{
		public NpgsqlSchemaProvider (IConnectionPool connectionPool)
			: base (connectionPool)
		{
		}
		
		public override bool SupportsSchemaType (Type type)
		{
			if (type == typeof(TableSchema))
				return true;
			else if (type == typeof(ViewSchema))
				return true;
			else if (type == typeof(ProcedureSchema))
				return true;
			else if (type == typeof(AggregateSchema))
				return true;
			else if (type == typeof(GroupSchema))
				return true;
			else if (type == typeof(UserSchema))
				return true;
			else if (type == typeof(LanguageSchema))
				return true;
			else if (type == typeof(OperatorSchema))
				return true;
			else if (type == typeof(RoleSchema))
				return true;
			else if (type == typeof(SequenceSchema))
				return true;
			else if (type == typeof(DataTypeSchema))
				return true;
			else if (type == typeof(TriggerSchema))
				return true;
			else if (type == typeof(ColumnSchema))
				return true;
			else if (type == typeof(ConstraintSchema))
				return true;
			else if (type == typeof(RuleSchema))
				return true;
			else if (type == typeof(ParameterSchema))
				return true;
			else
				return false;
		}

		public override ICollection<TableSchema> GetTables ()
		{
			List<TableSchema> tables = new List<TableSchema> ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (
				"SELECT c.relname, n.nspname, u.usename, d.description "
				+ "FROM pg_class c "
				+ " LEFT JOIN pg_description d ON c.oid = d.objoid, "
				+ "pg_namespace n, pg_user u "
				+ "WHERE c.relnamespace = n.oid "
				+ "AND c.relowner = u.usesysid "
				+ "AND c.relkind='r' AND NOT EXISTS "
				+ "   (SELECT 1 FROM pg_rewrite r "
				+ "      WHERE r.ev_class = c.oid AND r.ev_type = '1') "
				+ "ORDER BY relname;"
			);
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
						TableSchema table = new TableSchema (this);
	
						table.Name = r.GetString (0);
						table.IsSystemTable = table.Name.StartsWith ("pg_") || table.Name.StartsWith ("sql_");
						table.SchemaName = r.GetString (1);
						table.OwnerName = r.GetString (2);
						table.Comment = r.GetString (3);
						
						StringBuilder sb = new StringBuilder();
						sb.AppendFormat ("-- Table: {0}\n", table.Name);
						sb.AppendFormat ("-- DROP TABLE {0};\n\n", table.Name);
						sb.AppendFormat ("CREATE TABLE {0} (\n", table.Name);
						
						ICollection<ColumnSchema> columns = table.Columns;
						string[] parts = new string[columns.Count];
						int i = 0;
						foreach (ColumnSchema col in columns)
							parts[i++] = col.Definition;
						sb.Append (String.Join (",\n", parts));
						
						ICollection<ConstraintSchema> constraints = table.Constraints;
						parts = new string[constraints.Count];
						if (constraints.Count > 0)
							sb.Append (",\n");
						i = 0;
						foreach (ConstraintSchema constr in constraints)
							parts[i++] = "\t" + constr.Definition;
						sb.Append (String.Join (",\n", parts));
						
						sb.Append ("\n);\n");
						sb.AppendFormat ("COMMENT ON TABLE {0} IS '{1}';", table.Name, table.Comment);
						table.Definition = sb.ToString();
						
						tables.Add (table);
					}
					r.Close ();
				}
			}
			conn.Release ();

			return tables;
		}
		
		public override ICollection<ColumnSchema> GetTableColumns (TableSchema table)
		{
			List<ColumnSchema> columns = new List<ColumnSchema> ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (
				"SELECT a.attname, a.attnotnull, a.attlen, "
				+ "typ.typname, adef.adsrc "
				+ "FROM "
				+ "  pg_catalog.pg_attribute a LEFT JOIN "
				+ "  pg_catalog.pg_attrdef adef "
				+ "  ON a.attrelid=adef.adrelid "
				+ "  AND a.attnum=adef.adnum "
				+ "  LEFT JOIN pg_catalog.pg_type t ON a.atttypid=t.oid, "
				+ "  pg_catalog.pg_type typ "
				+ "WHERE "
				+ "  a.attrelid = (SELECT oid FROM pg_catalog.pg_class "
				+ "  WHERE relname='" + table.Name + "') "
				+ "AND a.attnum > 0 AND NOT a.attisdropped "
				+ "AND a.atttypid = typ.oid "
				+ "ORDER BY a.attnum;"
			);
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
						ColumnSchema column = new ColumnSchema (this);
		
						column.Name = r.GetString (0);
						column.DataTypeName = r.GetString (3);
						column.NotNull = r.GetBoolean (1);
						column.Default = r.GetString (4);
						column.Length = r.GetInt32 (2);
				
						StringBuilder sb = new StringBuilder();
						sb.AppendFormat("{0} {1}{2}",
							column.Name,
							column.DataTypeName,
							(column.Length > 0) ? ("(" + column.Length + ")") : "");
						sb.AppendFormat(" {0}", column.NotNull ? "NOT NULL" : "NULL");
						if (column.Default.Length > 0)
							sb.AppendFormat(" DEFAULT {0}", column.Default);
						column.Definition = sb.ToString();
		
						columns.Add (column);
					}
					r.Close ();
				};
			}
			conn.Release ();

			return columns;
		}

		public override ICollection<ViewSchema> GetViews ()
		{
			List<ViewSchema> views = new List<ViewSchema> ();

			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (
				"SELECT v.schemaname, v.viewname, v.viewowner, v.definition,"
				+ " (c.oid <= " + LastSystemOID + "), "
				+ "(SELECT description from pg_description pd, "
				+ " pg_class pc WHERE pc.oid=pd.objoid AND pc.relname="
				+ " v.viewname) "
				+ "FROM pg_views v, pg_class c "
				+ "WHERE v.viewname = c.relname "
				+ "ORDER BY viewname"
			);
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
						ViewSchema view = new ViewSchema (this);
	
						view.Name = r.GetString (1);
						view.OwnerName = r.GetString (2);
						view.SchemaName = r.GetString (0);
						view.IsSystemView = r.GetBoolean (4);
						view.Comment = r.GetString (5);
						
						StringBuilder sb = new StringBuilder();
						sb.AppendFormat ("-- View: {0}\n", view.Name);
						sb.AppendFormat ("-- DROP VIEW {0};\n\n", view.Name);
						sb.AppendFormat ("CREATE VIEW {0} AS (\n", view.Name);
						string core = r.GetString(3);
						sb.AppendFormat ("  {0}\n);", core.Substring (0, core.Length-1));
						view.Definition = sb.ToString ();
						
						views.Add (view);
					}
					r.Close ();
				}
			}
			conn.Release ();
			
			return views;
		}

		public override ICollection<ColumnSchema> GetViewColumns (ViewSchema view)
		{
			List<ColumnSchema> columns = new List<ColumnSchema> ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (
				"SELECT attname, typname, attlen, attnotnull "
				+ "FROM "
				+ "  pg_catalog.pg_attribute a LEFT JOIN pg_catalog.pg_attrdef adef "
				+ "  ON a.attrelid=adef.adrelid "
				+ "  AND a.attnum=adef.adnum "
				+ "  LEFT JOIN pg_catalog.pg_type t ON a.atttypid=t.oid "
				+ "WHERE "
				+ "  a.attrelid = (SELECT oid FROM pg_catalog.pg_class WHERE relname='"
				+ view.Name + "') "
				+ "  AND a.attnum > 0 AND NOT a.attisdropped "
				+ "     ORDER BY a.attnum;"
			);
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
						ColumnSchema column = new ColumnSchema (this);

						column.Name = r.GetString(0);
						column.DataTypeName = r.GetString (1);
						column.SchemaName = view.SchemaName;
						column.NotNull = r.GetBoolean (3);
						column.Length = r.GetInt32 (2);
		
						columns.Add (column);
					}
					r.Close ();
				};
			}
			conn.Release ();

			return columns;
		}

		public override ICollection<ProcedureSchema> GetProcedures ()
		{
			List<ProcedureSchema> procedures = new List<ProcedureSchema> ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (
				"SELECT pc.proname, pc.oid::integer, pl.lanname, pc.prosrc "
				+ "FROM "
				+ " pg_proc pc, "
				+ " pg_user pu, "
				+ " pg_type pt, "
				+ " pg_language pl "
				+ "WHERE pc.proowner = pu.usesysid "
				+ "AND pc.prorettype = pt.oid "
				+ "AND pc.prolang = pl.oid "
				+ "UNION "
				+ "SELECT pc.proname, pt.oid::integer, pl.lanname, pc.prosrc "
				+ "FROM "
				+ " pg_proc pc, "
				+ " pg_user pu, "
				+ " pg_type pt, "
				+ " pg_language pl "
				+ "WHERE pc.proowner = pu.usesysid "
				+ "AND pc.prorettype = 0 "
				+ "AND pc.prolang = pl.oid;"
			);
			
			using (command) {
			    	using (IDataReader r = command.ExecuteReader()) {
			    		while (r.Read ()) {
			    			ProcedureSchema procedure = new ProcedureSchema (this);
						
						procedure.Name = r.GetString (0);
						procedure.Definition = r.GetString (3);
						procedure.LanguageName = r.GetString (2);
						
						if (!r.IsDBNull (1) && r.GetInt32 (1) <= LastSystemOID)
							procedure.IsSystemProcedure = true;
			    			
			    			procedures.Add (procedure);
			    		}
					r.Close ();
				}
			}
			conn.Release ();
			
			return procedures;
		}

		public override ICollection<ColumnSchema> GetProcedureColumns (ProcedureSchema procedure)
		{
			List<ColumnSchema> columns = new List<ColumnSchema> ();
			
			// FIXME: Won't work properly with overload functions.
			// Maybe check the number of columns in the parameters for
			// proper match.
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (String.Format (
				"SELECT format_type (prorettype, NULL) "
				+ "FROM pg_proc pc, pg_language pl "
				+ "WHERE pc.prolang = pl.oid "
				+ "AND pc.proname = '{0}';", procedure.Name
			));
			
			using (command) {
			    	using (IDataReader r = command.ExecuteReader()) {
			    		while (r.Read ()) {	
						ColumnSchema column = new ColumnSchema (this);
						column.DataTypeName = r.GetString (0);
						column.Name = r.GetString (0);
						columns.Add (column);
			    		}
					r.Close ();
				}
			}
			conn.Release ();
			
			return columns;
		}
		
		public override ICollection<ParameterSchema> GetProcedureParameters (ProcedureSchema procedure)
		{
			throw new NotImplementedException ();
		}

		public override ICollection<ConstraintSchema> GetTableConstraints (TableSchema table)
		{
			List<ConstraintSchema> constraints = new List<ConstraintSchema> ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (String.Format (
				"SELECT "
				+ "pc.conname, "
				+ "pg_catalog.pg_get_constraintdef(pc.oid, true) AS consrc, "
				+ "pc.contype, "
				+ "CASE WHEN pc.contype='u' OR pc.contype='p' THEN ( "
				+ "	SELECT "
				+ "		indisclustered "
				+ "	FROM "
				+ "		pg_catalog.pg_depend pd, "
				+ "		pg_catalog.pg_class pl, "
				+ "		pg_catalog.pg_index pi "
				+ "	WHERE "
				+ "		pd.refclassid=pc.tableoid "
				+ "		AND pd.refobjid=pc.oid "
				+ "		AND pd.objid=pl.oid "
				+ "		AND pl.oid=pi.indexrelid "
				+ ") ELSE "
				+ "	NULL "
				+ "END AS indisclustered "
				+ "FROM "
				+ "pg_catalog.pg_constraint pc "
				+ "WHERE "
				+ "pc.conrelid = (SELECT oid FROM pg_catalog.pg_class WHERE relname='{0}' "
				+ "	AND relnamespace = (SELECT oid FROM pg_catalog.pg_namespace "
				+ "	WHERE nspname='{1}')) "
				+ "ORDER BY "
				+ "1;", table.Name, table.SchemaName
			));
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {	
						ConstraintSchema constraint = null;
										
						// XXX: Add support for Check constraints.
						switch (r.GetString (2)) {
							case "f":
								string match = @".*REFERENCES (.+)\(.*\).*";
								constraint = new ForeignKeyConstraintSchema (this);
								if (Regex.IsMatch (r.GetString (1), match))
									(constraint as ForeignKeyConstraintSchema).ReferenceTableName
										= Regex.Match (r.GetString (1), match).Groups[0].Captures[0].Value;
								break;
							case "u":
								constraint = new UniqueConstraintSchema (this);
								break;
							case "p":
							default:
								constraint = new PrimaryKeyConstraintSchema (this);
								break;
						}
					
						constraint.Name = r.GetString (0);
						constraint.Definition = r.GetString (1);
						
						constraints.Add (constraint);
					}
					r.Close ();
				}
			}
			conn.Release ();

			return constraints;
		}

		public override ICollection<UserSchema> GetUsers ()
		{
			List<UserSchema> users = new List<UserSchema> ();

			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("SELECT * FROM pg_user;");
			using (command) {
				using (IDataReader r = command.ExecuteReader ()) {
					while (r.Read ()) {
						UserSchema user = new UserSchema (this);
						
						user.Name = r.GetString (0);
						user.UserId = String.Format ("{0}", r.GetValue (1));
						user.Expires = r.IsDBNull (6) ? DateTime.MinValue : r.GetDateTime (6);
						user.Options["createdb"] = r.GetBoolean (2);
						user.Options["createuser"] = r.GetBoolean (3);
						user.Password = r.GetString (5);
						
						StringBuilder sb = new StringBuilder ();
						sb.AppendFormat ("-- User: \"{0}\"\n\n", user.Name);
						sb.AppendFormat ("-- DROP USER {0};\n\n", user.Name);
						sb.AppendFormat ("CREATE USER {0}", user.Name);
						sb.AppendFormat ("  WITH SYSID {0}", user.UserId);
						if (user.Password != "********")
							sb.AppendFormat (" ENCRYPTED PASSWORD {0}", user.Password);
						sb.AppendFormat (((bool) user.Options["createdb"]) ?
							" CREATEDB" : " NOCREATEDB");
						sb.AppendFormat (((bool) user.Options["createuser"]) ?
							" CREATEUSER" : " NOCREATEUSER");
						if (user.Expires != DateTime.MinValue)
							sb.AppendFormat (" VALID UNTIL {0}", user.Expires);
						sb.Append (";");
						user.Definition = sb.ToString ();
	
						users.Add (user);
					}
					r.Close ();
				}
			}
			conn.Release ();

			return users;
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
						
		/// <summary>
		/// Last system OID used in postgres to monitor system vs user
		/// objects. This varies based on the connections Server Version.
		/// </summary>
		protected int LastSystemOID {
			get {
				IPooledDbConnection conn = connectionPool.Request ();
				NpgsqlConnection internalConn = conn.DbConnection as NpgsqlConnection;
				int major = internalConn.ServerVersion.Major;
				int minor = internalConn.ServerVersion.Minor;
				conn.Release ();
				
				if (major == 8)
					return 17137;
				else if (major == 7 && minor == 1)
					return 18539;
				else if (major == 7 && minor == 2)
					return 16554;
				else if (major == 7 && minor == 3)
					return 16974;
				else if (major == 7 && minor == 4)
					return 17137;
				else
					return 17137;
			}
		}
	}
}
