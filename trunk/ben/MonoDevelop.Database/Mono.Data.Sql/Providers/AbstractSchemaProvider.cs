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
	public abstract class AbstractSchemaProvider : ISchemaProvider
	{
		protected IConnectionProvider connectionProvider;
		
		protected AbstractSchemaProvider (IConnectionProvider connectionProvider)
		{
			if (connectionProvider == null)
				throw new ArgumentNullException ("connectionProvider");
			
			this.connectionProvider = connectionProvider;
		}
		
		public IConnectionProvider ConnectionProvider {
			get { return connectionProvider; }
		}
		
		public virtual bool SupportsSchemaType (Type type)
		{
			return false;
		}
		
		public virtual ICollection<DatabaseSchema> GetDatabases ()
		{
			throw new NotImplementedException ();
		}

		public virtual ICollection<TableSchema> GetTables ()
		{
			throw new NotImplementedException ();
		}
		
		public virtual ICollection<ColumnSchema> GetTableColumns (TableSchema table)
		{
			throw new NotImplementedException ();
		}

		public virtual ICollection<ViewSchema> GetViews ()
		{
			throw new NotImplementedException ();
		}

		public virtual ICollection<ColumnSchema> GetViewColumns (ViewSchema view)
		{
			throw new NotImplementedException ();
		}

		public virtual ICollection<ProcedureSchema> GetProcedures ()
		{
			throw new NotImplementedException ();
		}

		public virtual ICollection<ColumnSchema> GetProcedureColumns (ProcedureSchema procedure)
		{
			throw new NotImplementedException ();
		}
		
		public virtual ICollection<ParameterSchema> GetProcedureParameters (ProcedureSchema procedure)
		{
			throw new NotImplementedException ();
		}

		public virtual ICollection<ConstraintSchema> GetTableConstraints (TableSchema table)
		{
			throw new NotImplementedException ();
		}

		public virtual ICollection<UserSchema> GetUsers ()
		{
			throw new NotImplementedException ();
		}
		
		public virtual DataTypeSchema GetDataType (string name)
		{
			throw new NotImplementedException ();
		}
		
		public virtual string GetTableDefinition (TableSchema table)
		{
			throw new NotImplementedException ();
		}
		
		public virtual string GetViewDefinition (ViewSchema view)
		{
			throw new NotImplementedException ();
		}
		
		public virtual string GetProcedureDefinition (ProcedureSchema procedure)
		{
			throw new NotImplementedException ();
		}

		protected virtual void CheckConnectionState ()
		{
			string error = null;
			if (!connectionProvider.IsOpen && !connectionProvider.Open (out error))
				throw new InvalidOperationException (String.Format ("Invalid Connection{0}", error == null ? "" : ": " + error));
			
			//TODO: if the connection is pooled, check if there is a connection available
		}
	}
}
