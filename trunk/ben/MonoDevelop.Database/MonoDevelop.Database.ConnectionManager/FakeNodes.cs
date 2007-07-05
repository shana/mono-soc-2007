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
		protected ConnectionContext context;
		
		public BaseNode (ConnectionContext context)
		{
			if (context == null)
				throw new ArgumentNullException ("context");
			
			this.context = context;
		}
		
		public ConnectionContext Context {
			get { return context; }
		}
	}
	
	public class TablesNode : BaseNode
	{
		public TablesNode (ConnectionContext context)
			: base (context)
		{
		}
	}
	
	public class ViewsNode : BaseNode
	{
		public ViewsNode (ConnectionContext context)
			: base (context)
		{
		}
	}
	
	public class ProceduresNode : BaseNode
	{
		public ProceduresNode (ConnectionContext context)
			: base (context)
		{
		}
	}
	
	public class AggregatesNode : BaseNode
	{
		public AggregatesNode (ConnectionContext context)
			: base (context)
		{
		}
	}
	
	public class GroupsNode : BaseNode
	{
		public GroupsNode (ConnectionContext context)
			: base (context)
		{
		}
	}
	
	public class LanguagesNode : BaseNode
	{
		public LanguagesNode (ConnectionContext context)
			: base (context)
		{
		}
	}
	
	public class OperatorsNode : BaseNode
	{
		public OperatorsNode (ConnectionContext context)
			: base (context)
		{
		}
	}
	
	public class RulesNode : BaseNode
	{
		public RulesNode (ConnectionContext context)
			: base (context)
		{
		}
	}

	public class RolesNode : BaseNode
	{
		public RolesNode (ConnectionContext context)
			: base (context)
		{
		}
	}
	
	public class SequencesNode : BaseNode
	{
		public SequencesNode (ConnectionContext context)
			: base (context)
		{
		}
	}
	
	public class UsersNode : BaseNode
	{
		public UsersNode (ConnectionContext context)
			: base (context)
		{
		}
	}

	public class TypesNode : BaseNode
	{
		public TypesNode (ConnectionContext context)
			: base (context)
		{
		}
	}

	public class ColumnsNode : BaseNode
	{
		public ColumnsNode (ConnectionContext context)
			: base (context)
		{
		}
	}
	
	public class ConstraintsNode : BaseNode
	{
		public ConstraintsNode (ConnectionContext context)
			: base (context)
		{
		}
	}
	
	public class TriggersNode : BaseNode
	{
		public TriggersNode (ConnectionContext context)
			: base (context)
		{
		}
	}
	
	public class ParametersNode : BaseNode
	{
		public ParametersNode (ConnectionContext context)
			: base (context)
		{
		}
	}
}