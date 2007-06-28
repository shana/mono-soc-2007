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
	public abstract class AbstractConnectionProvider : IConnectionProvider
	{
		protected IDbConnection connection;
		protected ConnectionSettings settings;
		
		protected AbstractConnectionProvider (ConnectionSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException ("settings");
			this.settings = settings;
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
		
		public virtual bool IsPooled {
			get { return false; }
		}

		public abstract bool Open (out string errorMessage);

		public abstract void Close ();
		
		public virtual IDbCommand CreateCommand (string sql)
		{
			IDbCommand command = connection.CreateCommand ();
			command.CommandText = sql;
			command.CommandType = CommandType.Text;
			return command;
		}
	}
}
