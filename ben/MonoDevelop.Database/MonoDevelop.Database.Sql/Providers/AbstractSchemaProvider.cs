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

namespace MonoDevelop.Database.Sql
{
	public abstract class AbstractSchemaProvider : ISchemaProvider
	{
		protected IConnectionPool connectionPool;
		
		protected AbstractSchemaProvider (IConnectionPool connectionPool)
		{
			if (connectionPool == null)
				throw new ArgumentNullException ("connectionPool");
			
			this.connectionPool = connectionPool;
		}
		
		public IConnectionPool ConnectionPool {
			get { return connectionPool; }
		}
		
		public virtual bool SupportsSchemaOperation (SqlStatementType statement, SqlSchemaType schema)
		{
			return SupportsSchemaOperation (new SchemaOperation (statement, schema));
		}
		
		public virtual bool SupportsSchemaOperation (SchemaOperation operation)
		{
			return false;
		}
		
		public virtual DatabaseSchemaCollection GetDatabases ()
		{
			throw new NotImplementedException ();
		}

		public virtual TableSchemaCollection GetTables ()
		{
			throw new NotImplementedException ();
		}
		
		public virtual ColumnSchemaCollection GetTableColumns (TableSchema table)
		{
			throw new NotImplementedException ();
		}

		public virtual ViewSchemaCollection GetViews ()
		{
			throw new NotImplementedException ();
		}

		public virtual ColumnSchemaCollection GetViewColumns (ViewSchema view)
		{
			throw new NotImplementedException ();
		}

		public virtual ProcedureSchemaCollection GetProcedures ()
		{
			throw new NotImplementedException ();
		}

		public virtual ColumnSchemaCollection GetProcedureColumns (ProcedureSchema procedure)
		{
			throw new NotImplementedException ();
		}
		
		public virtual ParameterSchemaCollection GetProcedureParameters (ProcedureSchema procedure)
		{
			throw new NotImplementedException ();
		}

		public virtual ConstraintSchemaCollection GetTableConstraints (TableSchema table)
		{
			throw new NotImplementedException ();
		}

		public virtual UserSchemaCollection GetUsers ()
		{
			throw new NotImplementedException ();
		}
		
		public virtual DataTypeSchema GetDataType (string name)
		{
			throw new NotImplementedException ();
		}
		
		public virtual TriggerSchemaCollection GetTableTriggers (TableSchema table)
		{
			throw new NotImplementedException ();
		}
		
		public virtual void CreateDatabase (DatabaseSchema database)
		{
			throw new NotImplementedException ();
		}

		public virtual void CreateTable (TableSchema table)
		{
			throw new NotImplementedException ();
		}

		public virtual void CreateView (ViewSchema view)
		{
			throw new NotImplementedException ();
		}

		public virtual void CreateProcedure (ProcedureSchema procedure)
		{
			throw new NotImplementedException ();
		}

		public virtual void CreateConstraint (ConstraintSchema constraint)
		{
			throw new NotImplementedException ();
		}
		
		public virtual void CreateTrigger (TriggerSchema trigger)
		{
			throw new NotImplementedException ();
		}

		public virtual void CreateUser (UserSchema user)
		{
			throw new NotImplementedException ();
		}
		
		public virtual void AlterDatabase (DatabaseSchema database)
		{
			throw new NotImplementedException ();
		}

		public virtual void AlterTable (TableSchema table)
		{
			throw new NotImplementedException ();
		}

		public virtual void AlterView (ViewSchema view)
		{
			throw new NotImplementedException ();
		}

		public virtual void AlterProcedure (ProcedureSchema procedure)
		{
			throw new NotImplementedException ();
		}

		public virtual void AlterConstraint (ConstraintSchema constraint)
		{
			throw new NotImplementedException ();
		}
		
		public virtual void AlterTrigger (TriggerSchema trigger)
		{
			throw new NotImplementedException ();
		}

		public virtual void AlterUser (UserSchema user)
		{
			throw new NotImplementedException ();
		}
		
		public virtual void DropDatabase (DatabaseSchema database)
		{
			throw new NotImplementedException ();
		}

		public virtual void DropTable (TableSchema table)
		{
			throw new NotImplementedException ();
		}

		public virtual void DropView (ViewSchema view)
		{
			throw new NotImplementedException ();
		}

		public virtual void DropProcedure (ProcedureSchema procedure)
		{
			throw new NotImplementedException ();
		}

		public virtual void DropConstraint (ConstraintSchema constraint)
		{
			throw new NotImplementedException ();
		}
		
		public virtual void DropTrigger (TriggerSchema trigger)
		{
			throw new NotImplementedException ();
		}

		public virtual void DropUser (UserSchema user)
		{
			throw new NotImplementedException ();
		}
		
		public virtual void RenameDatabase (DatabaseSchema database, string name)
		{
			throw new NotImplementedException ();
		}

		public virtual void RenameTable (TableSchema table, string name)
		{
			throw new NotImplementedException ();
		}

		public virtual void RenameView (ViewSchema view, string name)
		{
			throw new NotImplementedException ();
		}

		public virtual void RenameProcedure (ProcedureSchema procedure, string name)
		{
			throw new NotImplementedException ();
		}

		public virtual void RenameConstraint (ConstraintSchema constraint, string name)
		{
			throw new NotImplementedException ();
		}
		
		public virtual void RenameTrigger (TriggerSchema trigger, string name)
		{
			throw new NotImplementedException ();
		}

		public virtual void RenameUser (UserSchema user, string name)
		{
			throw new NotImplementedException ();
		}
		
		protected int GetCheckedInt32 (IDataReader reader, int field)
		{
			if (reader.IsDBNull (field))
				return 0;

			object o = reader.GetValue (field);
			int res = 0;
			if (int.TryParse (o.ToString (), out res))
				return res;
			return 0;
		}
			    
		protected string GetCheckedString (IDataReader reader, int field)
		{
			if (reader.IsDBNull (field))
				return null;

			return reader.GetValue (field).ToString ();
		}
	}
}
