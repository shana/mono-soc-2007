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
using System.Collections.Generic;

namespace Mono.Data.Sql
{
	public delegate void SqlResultCallback<T> (object sender, T Results, object state);
	public delegate void SqlResultCallback (object sender, object state);
	
	public interface IConnectionProvider
	{
		IDbFactory DbFactory { get; }
		
		IDbConnection Connection { get; }
		
		ConnectionSettings Settings { get; }
		
		bool IsOpen { get; }
		
		bool IsConnectionError { get; }
		
		bool SupportsPooling { get; }
		
		bool SupportsAutomaticConnectionString { get; }

		bool Open (out string errorMessage);

		void Close ();
		
		IDbCommand CreateCommand (string sql);
		
		void ExecuteQuery (IStatement statement);
		void ExecuteQuery (string sql);

		DataSet ExecuteQueryAsDataSet (IStatement statement);
		DataSet ExecuteQueryAsDataSet (string sql);
		
		DataTable ExecuteQueryAsDataTable (IStatement statement);
		DataTable ExecuteQueryAsDataTable (string sql);
		
		void ExecuteQueryAsync (IStatement statement, SqlResultCallback callback, object state);
		void ExecuteQueryAsync (string sql, SqlResultCallback callback, object state);
		
		void ExecuteQueryAsDataSetAsync (IStatement statement, SqlResultCallback<DataSet> callback, object state);
		void ExecuteQueryAsDataSetAsync (string sql, SqlResultCallback<DataSet> callback, object state);
		
		void ExecuteQueryAsDataTableAsync (IStatement statement, SqlResultCallback<DataTable> callback, object state);
		void ExecuteQueryAsDataTableAsync (string sql, SqlResultCallback<DataTable> callback, object state);
	}
}
