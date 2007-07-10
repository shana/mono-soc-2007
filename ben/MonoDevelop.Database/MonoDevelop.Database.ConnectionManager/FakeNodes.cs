//
// Authors:
//   Christian Hergert	<chris@mosaix.net>
//   Ben Motmans  <ben.motmans@gmail.com>
//
// Copyright (C) 2005 Mosaix Communications, Inc.
// Copyright (c) 2007 Ben Motmans
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using Mono.Data.Sql;

namespace MonoDevelop.Database.ConnectionManager
{
	public abstract class BaseNode
	{
		public event EventHandler RefreshEvent;
		protected ConnectionSettings settings;
		
		public BaseNode (ConnectionSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException ("settings");
			
			this.settings = settings;
		}
		
		public ConnectionSettings Settings {
			get { return settings; }
		}
		
		public void Refresh ()
		{
			if (RefreshEvent != null)
				RefreshEvent (this, EventArgs.Empty);
		}
	}
	
	public class TableNode : BaseNode
	{
		protected TableSchema table;
		
		public TableNode (ConnectionSettings settings, TableSchema table)
			: base (settings)
		{
			if (table == null)
				throw new ArgumentNullException ("table");
			
			this.table = table;
		}
		
		public TableSchema Table {
			get { return table; }
		}
	}
	
	public class TablesNode : BaseNode
	{
		public TablesNode (ConnectionSettings settings)
			: base (settings)
		{
		}
	}
	
	public class ViewNode : BaseNode
	{
		protected ViewSchema view;
		
		public ViewNode (ConnectionSettings settings, ViewSchema view)
			: base (settings)
		{
			if (view == null)
				throw new ArgumentNullException ("view");
			
			this.view = view;
		}
		
		public ViewSchema View {
			get { return view; }
		}
	}
	
	public class ViewsNode : BaseNode
	{
		public ViewsNode (ConnectionSettings settings)
			: base (settings)
		{
		}
	}
	
	public class ProcedureNode : BaseNode
	{
		protected ProcedureSchema procedure;
		
		public ProcedureNode (ConnectionSettings settings, ProcedureSchema procedure)
			: base (settings)
		{
			if (procedure == null)
				throw new ArgumentNullException ("procedure");
			
			this.procedure = procedure;
		}
		
		public ProcedureSchema Procedure {
			get { return procedure; }
		}
	}
	
	public class ProceduresNode : BaseNode
	{
		public ProceduresNode (ConnectionSettings settings)
			: base (settings)
		{
		}
	}
	
	public class AggregatesNode : BaseNode
	{
		public AggregatesNode (ConnectionSettings settings)
			: base (settings)
		{
		}
	}
	
	public class GroupsNode : BaseNode
	{
		public GroupsNode (ConnectionSettings settings)
			: base (settings)
		{
		}
	}
	
	public class LanguagesNode : BaseNode
	{
		public LanguagesNode (ConnectionSettings settings)
			: base (settings)
		{
		}
	}
	
	public class OperatorsNode : BaseNode
	{
		public OperatorsNode (ConnectionSettings settings)
			: base (settings)
		{
		}
	}
	
	public class RulesNode : BaseNode
	{
		public RulesNode (ConnectionSettings settings)
			: base (settings)
		{
		}
	}

	public class RolesNode : BaseNode
	{
		public RolesNode (ConnectionSettings settings)
			: base (settings)
		{
		}
	}
	
	public class SequencesNode : BaseNode
	{
		public SequencesNode (ConnectionSettings settings)
			: base (settings)
		{
		}
	}
	
	public class UserNode : BaseNode
	{
		protected UserSchema user;
		
		public UserNode (ConnectionSettings settings, UserSchema user)
			: base (settings)
		{
			if (user == null)
				throw new ArgumentNullException ("user");
			
			this.user = user;
		}
		
		public UserSchema User {
			get { return user; }
		}
	}
	
	public class UsersNode : BaseNode
	{
		public UsersNode (ConnectionSettings settings)
			: base (settings)
		{
		}
	}

	public class TypesNode : BaseNode
	{
		public TypesNode (ConnectionSettings settings)
			: base (settings)
		{
		}
	}
	
	public class ColumnNode : BaseNode
	{
		protected ColumnSchema column;
		
		public ColumnNode (ConnectionSettings settings, ColumnSchema column)
			: base (settings)
		{
			if (column == null)
				throw new ArgumentNullException ("column");
			
			this.column = column;
		}
		
		public ColumnSchema Column {
			get { return column; }
		}
	}

	public class ColumnsNode : BaseNode
	{
		protected TableSchema table;
		
		public ColumnsNode (ConnectionSettings settings, TableSchema table)
			: base (settings)
		{
			if (table == null)
				throw new ArgumentNullException ("table");
			
			this.table = table;
		}
		
		public TableSchema Table {
			get { return table; }
		}
	}
	
	public class ConstraintsNode : BaseNode
	{
		protected TableSchema table;
		
		public ConstraintsNode (ConnectionSettings settings, TableSchema table)
			: base (settings)
		{
			if (table == null)
				throw new ArgumentNullException ("table");
			
			this.table = table;
		}
		
		public TableSchema Table {
			get { return table; }
		}
	}
	
	public class TriggersNode : BaseNode
	{
		public TriggersNode (ConnectionSettings settings)
			: base (settings)
		{
		}
	}
	
	public class ParametersNode : BaseNode
	{
		public ParametersNode (ConnectionSettings settings)
			: base (settings)
		{
		}
	}
}