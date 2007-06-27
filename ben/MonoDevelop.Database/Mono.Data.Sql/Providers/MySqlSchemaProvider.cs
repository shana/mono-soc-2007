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
using System.Text.RegularExpressions;
using System.Data;
using ByteFX.Data.MySqlClient;
using System.Collections.Generic;

namespace Mono.Data.Sql
{
	public class SqlSchemaProvider : AbstractSchemaProvider
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
			else if (type == typeof(DatabaseSchema))
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
			
			MySqlCommand command = connectionProvider.CreateCommand ("SHOW DATABASES;");
			using (command) {
				using (MySqlDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
						DatabaseSchema db = new DatabaseSchema (this);
						db.Name = r.GetString (0);
						databases.Add (db);
						
					}
				}
			}
			
			return databases;
		}

		// see: http://dev.mysql.com/doc/refman/5.1/en/tables-table.html
		public override ICollection<TableSchema> GetTables ()
		{
			CheckConnectionState ();
			List<TableSchema> tables = new List<TableSchema> ();
			
			MySqlCommand command = connectionProvider.CreateCommand ("SHOW TABLES;");
			using (command) {
				if (GetMainVersion (pooledCommand) >= 5) {
					//in mysql 5.x we can use an sql query to provide the comment
					command.CommandText = "SELECT TABLE_NAME, TABLE_SCHEMA, TABLE_TYPE, TABLE_COMMENT FROM `information_schema`.`TABLES`"
						+ "WHERE TABLE_TYPE='BASE TABLE' AND TABLE_SCHEMA='" +
						+ command.Connection.Database +
						"' ORDER BY TABLE_NAME;";
					using (MySqlDataReader r = command.ExecuteReader()) {
						while (r.Read ()) {
							TableSchema table = new TableSchema (this);
		
							table.Name = r.GetString (0);
							table.OwnerName = command.Connection.Database;
							table.Comment = r.GetString (3);
							
							tables.Add (table);
						}
					}
				} else {
					//use the default command for mysql 4.x and 3.23
					using (MySqlDataReader r = command.ExecuteReader()) {
						while (r.Read ()) {
							TableSchema table = new TableSchema (this);
		
							table.Name = r.GetString (0);
							table.OwnerName = command.Connection.Database;
		
							tables.Add (table);
						}
					}
				}
			}

			return tables;
		}
		
		public override ICollection<ColumnSchema> GetTableColumns (TableSchema table)
		{
			CheckConnectionState ();
			List<ColumnSchema> columns = new List<ColumnSchema> ();
			
			MySqlCommand command = connectionProvider.CreateCommand (String.Format ("DESCRIBE {0}", table.Name));
			using (command) {
				using (MySqlDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
						ColumnSchema column = new ColumnSchema (this);
		
						column.Name = r.GetString (0);
						column.DataType.DataTypeName = r.GetString (1);
						column.NotNull = r.IsDBNull (2);
						column.Default = r.GetString (4);
						column.Comment = r.GetString (5);
						column.OwnerName = table.Name;
		
						columns.Add (column);
					}
				};
			}

			return columns;
		}

		// see: http://dev.mysql.com/doc/refman/5.1/en/views-table.html
		public override ICollection<ViewSchema> GetViews ()
		{
			CheckConnectionState ();
			List<ViewSchema> views = new List<ViewSchema> ();

			MySqlCommand command = connectionProvider.CreateCommand (
				"SELECT TABLE_NAME, TABLE_SCHEMA FROM information_schema.VIEWS where TABLE_SCHEMA = '"
				+ command.Connection.Database +
				"' ORDER BY TABLE_NAME"
			);
			using (command) {
				if (GetMainVersion (command) >= 5) {
					using (MySqlDataReader r = command.ExecuteReader()) {
						while (r.Read ()) {
							ViewSchema view = new ViewSchema (this);
		
							view.Name = r.GetString (0);
							view.OwnerName = r.GetString (1);
							views.Add (view);
						}
					}
				} //else: do nothing, since views are only supported since mysql 5.x
			}
			return views;
		}

		public override ICollection<ColumnSchema> GetViewColumns (ViewSchema view)
		{
			CheckConnectionState ();
			List<ColumnSchema> columns = new List<ColumnSchema> ();
			
			MySqlCommand command = connectionProvider.CreateCommand (String.Format ("DESCRIBE {0}", view.Name));
			using (command) {
				using (MySqlDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
						ColumnSchema column = new ColumnSchema (this);
		
						column.Name = r.GetString (0);
						column.DataType.DataTypeName = r.GetString (1);
						column.NotNull = r.IsDBNull (2);
						column.Default = r.GetString (4);
						column.Comment = r.GetString (5);
						column.OwnerName = view.Name;
		
						columns.Add (column);
					}
				};
			}

			return columns;
		}

		// see: http://dev.mysql.com/doc/refman/5.1/en/routines-table.html
		public override ICollection<ProcedureSchema> GetProcedures ()
		{
			CheckConnectionState ();
			List<ProcedureSchema> procedures = new List<ProcedureSchema> ();
			
			MySqlCommand command = connectionProvider.CreateCommand (
				"SELECT ROUTINE_NAME, ROUTINE_SCHEMA, ROUTINE_TYPE FROM information_schema.ROUTINES WHERE ROUTINE_SCHEMA ='"
				+ command.Connection.Database +
				"' ORDER BY ROUTINE_NAME"
			);
			
			using (command) {
				if (GetMainVersion (command >= 5) {
				    	using (MySqlDataReader r = command.ExecuteReader()) {
				    		while (r.Read ()) {
				    			ProcedureSchema procedure = new ProcedureSchema (this);
				    			
				    			procedure.Name = reader.GetString (0);
				    			procedure.OwnerName = reader.GetString (1);
				    			procedure.IsSystemProcedure = reader.GetString (2).ToLower ().Contains ("system");
				    			
				    			procedures.Add (procedure);
				    		}
					}
				} //else: do nothing, since procedures are only supported since mysql 5.x
			}
			
			return procedures;
		}

		public override ICollection<ColumnSchema> GetProcedureColumns (ProcedureSchema procedure)
		{
			CheckConnectionState ();
			List<ColumnSchema> columns = new List<ColumnSchema> ();
			
			MySqlCommand command = connectionProvider.CreateCommand (
				"SELECT param_list FROM mysql.proc where name = '" + procedure.Name + "'"
			);
			
			using (command) {
				if (GetMainVersion (command >= 5) {
				    	using (MySqlDataReader r = command.ExecuteReader()) {
				    		while (r.Read ()) {
				    			if (reader.IsDbNull (0))
				    				continue;
				
				    			string[] field = Encoding.ASCII.GetString ((byte[])reader.GetValue (0)).Split (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				    			foreach (string chunk in field) {
				    				ColumnSchema column = new ColumnSchema (this);
				    				
				    				string[] tmp = chunk.TrimStart (new char[] { ' ' }).Split (new char[] { ' ' });
				    				column.Name = tmp[0];
				    				column.OwnerName = procedure.Name;
								column.DataType.DataTypeName = tmp[1];
				    				
				    				columns.Add (column);
				    			}
				    		}
					}
				} //else: do nothing, since procedures are only supported since mysql 5.x
			}
			
			return columns;
		}
		
		public override ICollection<ParameterSchema> GetProcedureParameters (ProcedureSchema procedure)
		{
			CheckConnectionState ();
			throw new NotImplementedException ();
		}

		private static Regex constraintRegex = new Regex (@"`([\w ]+)`", RegexOptions.Compiled);
		public override ICollection<ConstraintSchema> GetTableConstraints (TableSchema table)
		{
			CheckConnectionState ();
			List<ConstraintSchema> constraints = new List<ConstraintSchema> ();
			
			MySqlCommand command = connectionProvider.CreateCommand ("SHOW TABLE STATUS FROM `" + table.OwnerName + "`;");
			using (command) {
				using (MySqlDataReader r = command.ExecuteReader()) {
					string[] chunks = ((string)r["Comment"]).Split (';');

					//the values we are looking for are in the format (`table`) REFER `database\table2` (`table2`)
					foreach (string chunk in chunks) {
						if (constraintRegex.IsMatch (chunk)) {
							MatchCollection matches = _constraintRegex.Matches (chunk);
							
							ColumnSchema pkColumn = new ColumnSchema (this);
							ColumnSchema fkColumn = new ColumnSchema (this);
							
							pkColumn.Name = matches[0].Groups[1].ToString ();
							fkColumn.Name = matches[1].Groups[1].ToString ();

							ConstraintSchema constraint = new ConstraintSchema (this);
							constraint.PrimaryKey = pkColumn;
							constraint.ForeignKey = fkColumn;
							
							constraints.Add (constraint);
						}
					}
				}
			}

			return constraints;
		}

		public override ICollection<UserSchema> GetUsers ()
		{
			CheckConnectionState ();
			List<UserSchema> users = new List<UserSchema> ();

			MySqlCommand command = connectionProvider.CreateCommand ("SELECT DISTINCT user from mysql.user where user != '';");
			using (command) {
				using (MySqlDataReader r = command.ExecuteReader ()) {
					while (r.Read ()) {
						UserSchema user = new UserSchema (this);
						user.Name = r.GetString (0);
	
						users.Add (user);
					}
	
					r.Close ();
				}
			}

			return users;
		}
		
		// see:
		// http://www.htmlite.com/mysql003.php
		// http://kimbriggs.com/computers/computer-notes/mysql-notes/mysql-data-types.file
		// http://dev.mysql.com/doc/refman/5.1/en/numeric-type-overview.html
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
			}
			
			return dts;
		}
		
		// see: http://dev.mysql.com/doc/refman/5.1/en/show-create-table.html
		public override string GetTableDefinition (TableSchema table)
		{
			CheckConnectionState ();

			MySqlCommand command = connectionProvider.CreateCommand ("SHOW CREATE TABLE `" + table.Name + "`;");
			using (command)
				return command.ExecuteScalar () as string;
		}
		
		public override string GetViewDefinition (ViewSchema view)
		{
			CheckConnectionState ();

			MySqlCommand command = connectionProvider.CreateCommand ("SHOW CREATE TABLE `" + view.Name + "`;");
			using (command)
				return command.ExecuteScalar () as string;
		}
		
		public override string GetProcedureDefinition (ProcedureSchema procedure)
		{
			CheckConnectionState ();

			MySqlCommand command = connectionProvider.CreateCommand ("SHOW CREATE PROCEDURE `" + procedure.Name + "`;");
			using (command)
				return command.ExecuteScalar () as string;
		}
		
		private int GetMainVersion (MySqlCommand command)
		{
			string str = (command.Connection as MySqlConnection).ServerVersion;
			int version = -1;
			if (int.TryParse (str.Substring (0, str.IndexOf (".")), out version)
			    return version;
			return -1;
		}
	}
}
