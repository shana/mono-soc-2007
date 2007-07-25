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
using MySql.Data.MySqlClient;
using System.Collections.Generic;
namespace MonoDevelop.Database.Sql
{
	public class MySqlSchemaProvider : AbstractSchemaProvider
	{
		public MySqlSchemaProvider (IConnectionPool connectionPool)
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
				case SchemaMetaData.Procedure:
				case SchemaMetaData.Database:
				case SchemaMetaData.Constraint:
				case SchemaMetaData.Parameter:
				case SchemaMetaData.User:
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
				case SchemaMetaData.View:
				case SchemaMetaData.Procedure:
				case SchemaMetaData.Constraint:
				case SchemaMetaData.Trigger:
				case SchemaMetaData.User:
					return true;
				default:
					return false;
				}
			case OperationMetaData.Alter:
				switch (operation.Schema) {
				case SchemaMetaData.Database:
				case SchemaMetaData.Table:
				case SchemaMetaData.View:
				case SchemaMetaData.Procedure:
					return true;
				default:
					return false;
				}
			default:
				return false;
			}
		}
		
		public override DatabaseSchemaCollection GetDatabases ()
		{
			DatabaseSchemaCollection databases = new DatabaseSchemaCollection ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("SHOW DATABASES;");
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
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

		// see: http://dev.mysql.com/doc/refman/5.1/en/tables-table.html
		// // see: http://dev.mysql.com/doc/refman/5.1/en/show-create-table.html
		public override TableSchemaCollection GetTables ()
		{
			TableSchemaCollection tables = new TableSchemaCollection ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("SHOW TABLES;");
			using (command) {
				if (GetMainVersion (command) >= 5) {
					//in mysql 5.x we can use an sql query to provide the comment
					command.CommandText = "SELECT TABLE_NAME, TABLE_SCHEMA, TABLE_TYPE, TABLE_COMMENT FROM `information_schema`.`TABLES` "
						+ "WHERE TABLE_TYPE='BASE TABLE' AND TABLE_SCHEMA='"
						+ command.Connection.Database
						+ "' ORDER BY TABLE_NAME;";
					using (IDataReader r = command.ExecuteReader()) {
						while (r.Read ()) {
							TableSchema table = new TableSchema (this);
		
							table.Name = r.GetString (0);
							//table.OwnerName = command.Connection.Database;
							table.Comment = r.GetString (3);
							
							IPooledDbConnection conn2 = connectionPool.Request ();
							IDbCommand command2 = conn2.CreateCommand ("SHOW CREATE TABLE `" + table.Name + "`;");
							using (command2)
								table.Definition = command2.ExecuteScalar () as string;
							conn2.Release ();
							
							tables.Add (table);
						}
						r.Close ();
					}
				} else {
					//use the default command for mysql 4.x and 3.23
					using (IDataReader r = command.ExecuteReader()) {
						while (r.Read ()) {
							TableSchema table = new TableSchema (this);
		
							table.Name = r.GetString (0);
							//table.OwnerName = command.Connection.Database;
							
							IPooledDbConnection conn2 = connectionPool.Request ();
							IDbCommand command2 = conn2.CreateCommand ("SHOW CREATE TABLE `" + table.Name + "`;");
							using (command2)
								table.Definition = command2.ExecuteScalar () as string;
							conn2.Release ();
							
							tables.Add (table);
						}
						r.Close ();
					}
				}
			}
			conn.Release ();

			return tables;
		}
		
		public override ColumnSchemaCollection GetTableColumns (TableSchema table)
		{
			ColumnSchemaCollection columns = new ColumnSchemaCollection ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (String.Format ("DESCRIBE {0}", table.Name));
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
						ColumnSchema column = new ColumnSchema (this);
		
						column.Name = r.GetString (0);
						column.DataTypeName = r.GetString (1);
						column.IsNullable = r.IsDBNull (2);
						column.DefaultValue = r.GetString (4);
						column.Comment = r.GetString (5);
						column.OwnerName = table.Name;
		
						columns.Add (column);
					}
					r.Close ();
				};
			}
			conn.Release ();

			return columns;
		}

		// see: http://dev.mysql.com/doc/refman/5.1/en/views-table.html
		public override ViewSchemaCollection GetViews ()
		{
			ViewSchemaCollection views = new ViewSchemaCollection ();

			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (
				"SELECT TABLE_NAME, TABLE_SCHEMA FROM information_schema.VIEWS where TABLE_SCHEMA = '"
				+ ConnectionPool.ConnectionContext.ConnectionSettings.Database +
				"' ORDER BY TABLE_NAME"
			);
			using (command) {
				if (GetMainVersion (command) >= 5) {
					using (IDataReader r = command.ExecuteReader()) {
						while (r.Read ()) {
							ViewSchema view = new ViewSchema (this);
		
							view.Name = r.GetString (0);
							view.OwnerName = r.GetString (1);
							
							IPooledDbConnection conn2 = connectionPool.Request ();
							IDbCommand command2 = conn2.CreateCommand ("SHOW CREATE TABLE `" + view.Name + "`;");
							using (command2)
								view.Definition = command2.ExecuteScalar () as string;
							conn2.Release ();
							
							views.Add (view);
						}
						r.Close ();
					}
				} //else: do nothing, since views are only supported since mysql 5.x
			}
			conn.Release ();

			return views;
		}

		public override ColumnSchemaCollection GetViewColumns (ViewSchema view)
		{
			ColumnSchemaCollection columns = new ColumnSchemaCollection ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (String.Format ("DESCRIBE {0}", view.Name));
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					while (r.Read ()) {
						ColumnSchema column = new ColumnSchema (this);
		
						column.Name = r.GetString (0);
						column.DataTypeName = r.GetString (1);
						column.IsNullable = r.IsDBNull (2);
						column.DefaultValue = r.GetString (4);
						column.Comment = r.GetString (5);
						column.OwnerName = view.Name;
		
						columns.Add (column);
					}
					r.Close ();
				};
			}
			conn.Release ();

			return columns;
		}

		// see: http://dev.mysql.com/doc/refman/5.1/en/routines-table.html
		public override ProcedureSchemaCollection GetProcedures ()
		{
			ProcedureSchemaCollection procedures = new ProcedureSchemaCollection ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand (
				"SELECT ROUTINE_NAME, ROUTINE_SCHEMA, ROUTINE_TYPE FROM information_schema.ROUTINES WHERE ROUTINE_SCHEMA ='"
				+ ConnectionPool.ConnectionContext.ConnectionSettings.Database +
				"' ORDER BY ROUTINE_NAME"
			);
			
			using (command) {
				if (GetMainVersion (command) >= 5) {
				    	using (IDataReader r = command.ExecuteReader()) {
				    		while (r.Read ()) {
				    			ProcedureSchema procedure = new ProcedureSchema (this);
				    			
				    			procedure.Name = r.GetString (0);
				    			procedure.OwnerName = r.GetString (1);
				    			procedure.IsSystemProcedure = r.GetString (2).ToLower ().Contains ("system");
							
							IPooledDbConnection conn2 = connectionPool.Request ();
							IDbCommand command2 = conn2.CreateCommand ("SHOW CREATE PROCEDURE `" + procedure.Name + "`;");
							using (command2)
								procedure.Definition = command2.ExecuteScalar () as string;
				    			conn2.Release ();
							
				    			procedures.Add (procedure);
				    		}
						r.Close ();
					}
				} //else: do nothing, since procedures are only supported since mysql 5.x
			}
			conn.Release ();
			
			return procedures;
		}
//
//		public override ColumnSchemaCollection GetProcedureColumns (ProcedureSchema procedure)
//		{
//			ColumnSchemaCollection columns = new ColumnSchemaCollection ();
//			
//			IPooledDbConnection conn = connectionPool.Request ();
//			IDbCommand command = conn.CreateCommand (
//				"SELECT param_list FROM mysql.proc where name = '" + procedure.Name + "'"
//			);
//			
//			using (command) {
//				if (GetMainVersion (command) >= 5) {
//				    	using (IDataReader r = command.ExecuteReader()) {
//				    		while (r.Read ()) {
//				    			if (r.IsDBNull (0))
//				    				continue;
//				
//				    			string[] field = Encoding.ASCII.GetString ((byte[])r.GetValue (0)).Split (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
//				    			foreach (string chunk in field) {
//				    				ColumnSchema column = new ColumnSchema (this);
//				    				
//				    				string[] tmp = chunk.TrimStart (new char[] { ' ' }).Split (new char[] { ' ' });
//				    				column.Name = tmp[0];
//				    				column.OwnerName = procedure.Name;
//								column.DataTypeName = tmp[1];
//				    				
//				    				columns.Add (column);
//				    			}
//				    		}
//						r.Close ();
//					}
//				} //else: do nothing, since procedures are only supported since mysql 5.x
//			}
//			conn.Release ();
//			
//			return columns;
//		}
		
		public override ParameterSchemaCollection GetProcedureParameters (ProcedureSchema procedure)
		{
			throw new NotImplementedException ();
		}

		private static Regex constraintRegex = new Regex (@"`([\w ]+)`", RegexOptions.Compiled);
		public override ConstraintSchemaCollection GetTableConstraints (TableSchema table)
		{
			ConstraintSchemaCollection constraints = new ConstraintSchemaCollection ();
			
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("SHOW TABLE STATUS FROM `" + table.OwnerName + "`;");
			using (command) {
				using (IDataReader r = command.ExecuteReader()) {
					string[] chunks = ((string)r["Comment"]).Split (';');

					//the values we are looking for are in the format (`table`) REFER `database\table2` (`table2`)
					foreach (string chunk in chunks) {
						if (constraintRegex.IsMatch (chunk)) {
							MatchCollection matches = constraintRegex.Matches (chunk);
	
							ForeignKeyConstraintSchema constraint = new ForeignKeyConstraintSchema (this);
							constraint.ReferenceTableName = matches[1].Groups[1].ToString ();
							constraint.Name = matches[0].Groups[1].ToString ();

							constraints.Add (constraint);
						}
					}
					r.Close ();
				}
			}
			conn.Release ();

			return constraints;
		}

		public override UserSchemaCollection GetUsers ()
		{
			UserSchemaCollection users = new UserSchemaCollection ();

			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("SELECT DISTINCT user from mysql.user where user != '';");
			using (command) {
				using (IDataReader r = command.ExecuteReader ()) {
					while (r.Read ()) {
						UserSchema user = new UserSchema (this);
						user.Name = r.GetString (0);
	
						users.Add (user);
					}
	
					r.Close ();
				}
			}
			conn.Release ();

			return users;
		}
		
		// see:
		// http://www.htmlite.com/mysql003.php
		// http://kimbriggs.com/computers/computer-notes/mysql-notes/mysql-data-types.file
		// http://dev.mysql.com/doc/refman/5.1/en/data-type-overview.html
		public override DataTypeSchema GetDataType (string name)
		{
			if (name == null)
				throw new ArgumentNullException ("name");

			string type = null;
			int length = 0;
			int scale = 0;
			ParseType (name, out type, out length, out scale);

			DataTypeSchema dts = new DataTypeSchema (this);
			dts.Name = type;
			switch (type.ToLower ()) {
				case "tinyint":
				case "smallint":
				case "mediumint":
				case "int":
				case "integer":
				case "bigint":
					dts.LengthRange = new Range (length);
					dts.DataTypeCategory = DataTypeCategory.Integer;
					break;
				case "bit":
					dts.LengthRange = new Range (length); //in bits
					dts.DataTypeCategory = DataTypeCategory.Bit;
					break;
				case "bool":
				case "boolean":
					dts.LengthRange = new Range (1); //in bits
					dts.DataTypeCategory = DataTypeCategory.Boolean;
					break;
				case "float":
				case "double":
				case "double precision":
				case "decimal":
				case "dec":
					dts.LengthRange = new Range (length);
					dts.ScaleRange = new Range (scale);
					dts.DataTypeCategory = DataTypeCategory.Boolean;
					break;
				case "date":
					dts.DataTypeCategory = DataTypeCategory.Date;
					break;
				case "datetime":
					dts.DataTypeCategory = DataTypeCategory.DateTime;
					break;
				case "timestamp":
					dts.DataTypeCategory = DataTypeCategory.TimeStamp;
					break;
				case "time":
					dts.DataTypeCategory = DataTypeCategory.Time;
					break;
				case "year":
					dts.LengthRange = new Range (length);
					dts.DataTypeCategory = DataTypeCategory.Integer;
					break;
				case "binary":
				case "char byte":
					dts.LengthRange = new Range (length);
					dts.DataTypeCategory = DataTypeCategory.Binary;
					break;
				case "varbinary":
					dts.LengthRange = new Range (length);
					dts.DataTypeCategory = DataTypeCategory.VarBinary;
					break;
				case "tinyblob":
				case "mediumblob":
				case "longblob":
				case "blob":
					dts.LengthRange = new Range (length);
					dts.DataTypeCategory = DataTypeCategory.Binary;
					break;
				case "tinytext":
				case "mediumtext":
				case "longtext":
				case "text":
					dts.LengthRange = new Range (length);
					dts.DataTypeCategory = DataTypeCategory.NChar;
					break;
				case "national char":
				case "nchar":
					dts.LengthRange = new Range (length);
					dts.DataTypeCategory = DataTypeCategory.NChar;
					break;
				case "national varchar":
				case "nvarchar":
					dts.LengthRange = new Range (length);
					dts.DataTypeCategory = DataTypeCategory.NVarChar;
					break;
				case "varchar":
					dts.LengthRange = new Range (length);
					dts.DataTypeCategory = DataTypeCategory.VarChar;
					break;
				case "char":
					dts.LengthRange = new Range (length);
					dts.DataTypeCategory = DataTypeCategory.Char;
					break;
				case "set":
				case "enum":
					dts.DataTypeCategory = DataTypeCategory.Integer;
					break;
				default:
					dts = null;
					break;
			}
			
			return dts;
		}
		
		//http://dev.mysql.com/doc/refman/5.1/en/create-database.html
		public override void CreateDatabase (DatabaseSchema database)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("CREATE DATABASE " + database.Name);
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
		}

		//http://dev.mysql.com/doc/refman/5.1/en/create-table.html
		public override void CreateTable (TableSchema table)
		{
			throw new NotImplementedException ();
		}

		//http://dev.mysql.com/doc/refman/5.1/en/create-view.html
		public override void CreateView (ViewSchema view)
		{
			throw new NotImplementedException ();
		}

		//http://dev.mysql.com/doc/refman/5.1/en/create-procedure.html
		public override void CreateProcedure (ProcedureSchema procedure)
		{
			throw new NotImplementedException ();
		}

		//http://dev.mysql.com/doc/refman/5.1/en/create-index.html
		public override void CreateConstraint (ConstraintSchema constraint)
		{
			throw new NotImplementedException ();
		}
		
		//http://dev.mysql.com/doc/refman/5.1/en/create-trigger.html
		public override void CreateTrigger (TriggerSchema trigger)
		{
			throw new NotImplementedException ();
		}

		//http://dev.mysql.com/doc/refman/5.1/en/create-user.html
		public override void CreateUser (UserSchema user)
		{
			throw new NotImplementedException ();
		}
		
		//http://dev.mysql.com/doc/refman/5.1/en/alter-database.html
		public override void AlterDatabase (DatabaseSchema database)
		{
			throw new NotImplementedException ();
		}

		//http://dev.mysql.com/doc/refman/5.1/en/alter-table.html
		public override void AlterTable (TableSchema table)
		{
			throw new NotImplementedException ();
		}

		//http://dev.mysql.com/doc/refman/5.1/en/alter-view.html
		public override void AlterView (ViewSchema view)
		{
			throw new NotImplementedException ();
		}

		//http://dev.mysql.com/doc/refman/5.1/en/alter-procedure.html
		public override void AlterProcedure (ProcedureSchema procedure)
		{
			throw new NotImplementedException ();
		}
		
		//http://dev.mysql.com/doc/refman/5.1/en/drop-database.html
		public override void DropDatabase (DatabaseSchema database)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("DROP DATABASE IF EXISTS " + database.Name + ";");
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
		}

		//http://dev.mysql.com/doc/refman/5.1/en/drop-table.html
		public override void DropTable (TableSchema table)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("DROP TABLE IF EXISTS " + table.Name + ";");
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
		}

		//http://dev.mysql.com/doc/refman/5.1/en/drop-view.html
		public override void DropView (ViewSchema view)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("DROP VIEW IF EXISTS " + view.Name + ";");
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
		}

		//http://dev.mysql.com/doc/refman/5.1/en/drop-procedure.html
		public override void DropProcedure (ProcedureSchema procedure)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("DROP PROCEDURE IF EXISTS " + procedure.Name + ";");
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
		}

		//http://dev.mysql.com/doc/refman/5.1/en/drop-index.html
		public override void DropConstraint (ConstraintSchema constraint)
		{
//			if (constraint is IndexConstraintSchema) {
//				IndexConstraintSchema indexConstraint = constraint as IndexConstraintSchema;
//				
//				IPooledDbConnection conn = connectionPool.Request ();
//				IDbCommand command = conn.CreateCommand ("DROP INDEX " + constraint.Name + " ON " + indexConstraint.TableName + ";");
//				using (command)
//					command.ExecuteNonQuery ();
//				conn.Release ();
//			}
		}
		
		//http://dev.mysql.com/doc/refman/5.1/en/drop-trigger.html
		public override void DropTrigger (TriggerSchema trigger)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("DROP TRIGGER IF EXISTS " + trigger.Name + ";");
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
		}

		//http://dev.mysql.com/doc/refman/5.1/en/drop-user.html
		public override void DropUser (UserSchema user)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("DROP USER " + user.Name + ";");
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
		}
		
		//http://dev.mysql.com/doc/refman/5.1/en/rename-database.html
		public override void RenameDatabase (DatabaseSchema database, string name)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("RENAME DATABASE " + database.Name + " TO " + name + ";");
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
			
			database.Name = name;
		}

		//http://dev.mysql.com/doc/refman/5.1/en/rename-table.html
		public override void RenameTable (TableSchema table, string name)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("RENAME TABLE " + table.Name + " TO " + name + ";");
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
			
			table.Name = name;
		}

		//http://dev.mysql.com/doc/refman/5.1/en/rename-table.html
		public override void RenameView (ViewSchema view, string name)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			//this is no copy paste error, it really is "RENAME TABLE"
			IDbCommand command = conn.CreateCommand ("RENAME TABLE " + view.Name + " TO " + name + ";");
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
			
			view.Name = name;
		}

		public override void RenameProcedure (ProcedureSchema procedure, string name)
		{
			DropProcedure (procedure);
			procedure.Name = name;
			CreateProcedure (procedure);
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

		//http://dev.mysql.com/doc/refman/5.1/en/rename-user.html
		public override void RenameUser (UserSchema user, string name)
		{
			IPooledDbConnection conn = connectionPool.Request ();
			IDbCommand command = conn.CreateCommand ("RENAME USER " + user.Name + " TO " + name + ";");
			using (command)
				command.ExecuteNonQuery ();
			conn.Release ();
			
			user.Name = name;
		}
		
		private int GetMainVersion (IDbCommand command)
		{
			string str = (command.Connection as MySqlConnection).ServerVersion;
			int version = -1;
			if (int.TryParse (str.Substring (0, str.IndexOf (".")), out version))
				return version;
			return -1;
		}
		
		private void ParseType (string str, out string type, out int length, out int scale)
		{
			int parenOpen = str.IndexOf ('(');
			int parenClose = str.IndexOf (')');
			int commaPos = str.IndexOf (',', parenOpen);
			
			if (parenOpen > 0) {
				type = str.Substring (0, parenOpen).Trim ();
				
				string lengthString = null;
				if (commaPos > 0) {
					lengthString = str.Substring (parenOpen + 1, commaPos - parenOpen);
					string scaleString = str.Substring (commaPos + 1, parenClose - commaPos).Trim ();
					int.TryParse (scaleString, out scale);
				} else {
					lengthString = str.Substring (parenOpen + 1, parenClose - parenOpen);
					scale = 0;
				}
				int.TryParse (lengthString, out length);
			} else {
				type = str;
				length = 1;
				scale = 0;
			}
		}
	}
}
