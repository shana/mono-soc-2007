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
using System.Threading;
using System.Collections.Generic;

namespace Mono.Data.Sql
{
	public abstract class AbstractConnectionProvider : IConnectionProvider
	{
		protected IDbConnection connection;
		protected ConnectionSettings settings;
		protected IDbFactory factory;
		protected bool isConnectionError;
		
		protected AbstractConnectionProvider (IDbFactory factory, ConnectionSettings settings)
		{
			if (factory == null)
				throw new ArgumentNullException ("factory");
			if (settings == null)
				throw new ArgumentNullException ("settings");
			
			this.factory = factory;
			this.settings = settings;
		}
		
		public virtual IDbFactory DbFactory {
			get { return factory; }
		}
		
		public virtual IDbConnection Connection {
			get { return connection; }
			protected internal set { connection = value; }
		}
		
		public virtual ConnectionSettings Settings {
			get { return settings; }
		}

		public virtual bool IsOpen {
			get { return connection != null && connection.State == ConnectionState.Open; }
		}
		
		public virtual bool IsConnectionError {
			get { return isConnectionError; }
		}
		
		public virtual bool SupportsPooling {
			get { return true; }
		}
		
		public virtual bool SupportsAutomaticConnectionString {
			get { return true; }
		}

		public abstract bool Open (out string errorMessage);

		public virtual void Close ()
		{
			connection.Close ();
			connection.Dispose ();
			connection = null;
		}
		
		public virtual IDbCommand CreateCommand (string sql)
		{
			IDbCommand command = connection.CreateCommand ();
			command.CommandText = sql;
			command.CommandType = CommandType.Text;
			return command;
		}

		public virtual void ExecuteQuery (IStatement statement)
		{
			if (statement == null)
				throw new ArgumentNullException ("statement");
			string sql = DbFactory.Dialect.GetSql (statement);
			ExecuteQuery (sql);
		}

		public virtual void ExecuteQuery (string sql)
		{
			if (String.IsNullOrEmpty ("sql"))
				throw new ArgumentException ("sql");
			IDbCommand command = CreateCommand (sql);
			command.ExecuteNonQuery ();
			command.Connection.Close ();
		}
		
		public virtual void ExecuteQueryAsync (IStatement statement, SqlResultCallback callback, object state)
		{
			if (statement == null)
				throw new ArgumentNullException ("statement");
			string sql = DbFactory.Dialect.GetSql (statement);
			ExecuteQueryAsync (sql, callback, state);
		}

		public virtual void ExecuteQueryAsync (string sql, SqlResultCallback callback, object state)
		{
			if (String.IsNullOrEmpty ("sql"))
				throw new ArgumentException ("sql");
			if (callback == null)
				throw new ArgumentNullException ("callback");
			IDbCommand command = CreateCommand (sql);
			ThreadPool.QueueUserWorkItem (new WaitCallback (ExecuteQueryThreaded),
				new object[] {command, callback, state} as object
			);
		}

		public virtual DataSet ExecuteQueryAsDataSet (IStatement statement)
		{
			if (statement == null)
				throw new ArgumentNullException ("statement");
			string sql = DbFactory.Dialect.GetSql (statement);
			return ExecuteQueryAsDataSet (sql);
		}

		public abstract DataSet ExecuteQueryAsDataSet (string sql);
		
		public virtual DataTable ExecuteQueryAsDataTable (IStatement statement)
		{
			if (statement == null)
				throw new ArgumentNullException ("statement");
			string sql = DbFactory.Dialect.GetSql (statement);
			return ExecuteQueryAsDataTable (sql);
		}

		public abstract DataTable ExecuteQueryAsDataTable (string sql);
		
		public virtual void ExecuteQueryAsDataSetAsync (IStatement statement, SqlResultCallback<DataSet> callback, object state)
		{
			if (statement == null)
				throw new ArgumentNullException ("statement");
			string sql = DbFactory.Dialect.GetSql (statement);
			ExecuteQueryAsDataSetAsync (sql, callback, state);
		}

		public virtual void ExecuteQueryAsDataSetAsync (string sql, SqlResultCallback<DataSet> callback, object state)
		{
			if (String.IsNullOrEmpty ("sql"))
				throw new ArgumentException ("sql");
			if (callback == null)
				throw new ArgumentNullException ("callback");
			
			ThreadPool.QueueUserWorkItem (new WaitCallback (ExecuteQueryAsDataSetThreaded),
				new object[] {sql, callback, state} as object
			);
		}
		
		public virtual void ExecuteQueryAsDataTableAsync (IStatement statement, SqlResultCallback<DataTable> callback, object state)
		{
			if (statement == null)
				throw new ArgumentNullException ("statement");
			string sql = DbFactory.Dialect.GetSql (statement);
			ExecuteQueryAsDataTableAsync (sql, callback, state);
		}
		
		public virtual void ExecuteQueryAsDataTableAsync (string sql, SqlResultCallback<DataTable> callback, object state)
		{
			if (String.IsNullOrEmpty ("sql"))
				throw new ArgumentException ("sql");
			if (callback == null)
				throw new ArgumentNullException ("callback");
			
			ThreadPool.QueueUserWorkItem (new WaitCallback (ExecuteQueryAsDataTableThreaded),
				new object[] {sql, callback, state}
			);
		}
		
		protected virtual string SetConnectionStringParameter (string connectionString, string quoteChar, string parameter, string value)
		{
			Regex regex = new Regex (parameter + "[ \t]*=[ \t]*" + quoteChar + "([a-zA-Z0-9_.]+)" + quoteChar, RegexOptions.IgnoreCase);
			Match match = regex.Match (connectionString);
			if (match.Success) {
				return connectionString.Substring (0, match.Index) + value + connectionString.Substring (match.Index + match.Length);
			} else {
				connectionString.TrimEnd ();
				return String.Concat (connectionString, connectionString.EndsWith (";") ? "" : ";",
					parameter, "=", quoteChar, value, quoteChar, ";");
			}
		}
			
		private void ExecuteQueryThreaded (object state)
		{
			object[] data = state as object[];
			IDbCommand command = data[0] as IDbCommand;
			SqlResultCallback callback = data[1] as SqlResultCallback;
			command.ExecuteNonQuery ();
			command.Connection.Close ();
			callback (this, data[2]);
		}
				
		private void ExecuteQueryAsDataSetThreaded (object state)
		{
			object[] data = state as object[];
			string sql = data[0] as string;
			SqlResultCallback<DataSet> callback = data[1] as SqlResultCallback<DataSet>;
					
			DataSet set = ExecuteQueryAsDataSet (sql);
			callback (this, set, data[2]);
		}
				
		private void ExecuteQueryAsDataTableThreaded (object state)
		{
			object[] data = state as object[];
			string sql = data[0] as string;
			SqlResultCallback<DataTable> callback = data[1] as SqlResultCallback<DataTable>;
					
			DataTable table = ExecuteQueryAsDataTable (sql);
			callback (this, table, data[2]);
		}
	}
}
