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
using System.Threading;
using System.Collections.Generic;
using MonoDevelop.Database.Sql;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Ide.Gui.Pads;
using MonoDevelop.Components.Commands;

namespace MonoDevelop.Database.ConnectionManager
{
	public class ProcedureNodeBuilder : TypeNodeBuilder
	{
		public ProcedureNodeBuilder ()
			: base ()
		{
		}
		
		public override Type NodeDataType {
			get { return typeof (ProcedureNode); }
		}
		
		public override string ContextMenuAddinPath {
			get { return "/SharpDevelop/Procedures/ConnectionManagerPad/ContextMenu/ProcedureNode"; }
		}
		
		public override Type CommandHandlerType {
			get { return typeof (ProcedureNodeCommandHandler); }
		}
		
		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return GettextCatalog.GetString ("Procedure");
		}
		
		public override void BuildNode (ITreeBuilder builder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			ProcedureNode node = dataObject as ProcedureNode;

			label = node.Procedure.Name;
			icon = Context.GetIcon ("md-db-procedure");
		}
		
		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			ProcedureNode node = dataObject as ProcedureNode;
			NodeState nodeState = new NodeState (builder, node.ConnectionContext, dataObject);
			
			ThreadPool.QueueUserWorkItem (new WaitCallback (BuildChildNodesThreaded), nodeState);
		}
		
		private void BuildChildNodesThreaded (object state)
		{
			NodeState nodeState = state as NodeState;
			
			ISchemaProvider provider = nodeState.ConnectionContext.SchemaProvider;
			
			//TODO: build columns + params
		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}
	}
	
	public class ProcedureNodeCommandHandler : NodeCommandHandler
	{
		public override DragOperation CanDragNode ()
		{
			return DragOperation.None;
		}
		
		[CommandHandler (ConnectionManagerCommands.Refresh)]
		protected void OnRefresh ()
		{
			
		}
		
		[CommandHandler (ConnectionManagerCommands.AlterProcedure)]
		protected void OnAlterProcedure ()
		{
			
		}
		
		[CommandHandler (ConnectionManagerCommands.DropProcedure)]
		protected void OnDropProcedure ()
		{
			
		}
		
		[CommandHandler (ConnectionManagerCommands.Rename)]
		protected void OnRenameProcedure ()
		{
			Tree.StartLabelEdit ();
		}
		
		[CommandUpdateHandler (ConnectionManagerCommands.DropProcedure)]
		protected void OnUpdateDropProcedure (CommandInfo info)
		{
			BaseNode node = (BaseNode)CurrentNode.DataItem;
			info.Enabled = node.ConnectionContext.SchemaProvider.SupportsSchemaOperation (SqlStatementType.Drop, SqlSchemaType.Procedure);
		}
		
		[CommandUpdateHandler (ConnectionManagerCommands.Rename)]
		protected void OnUpdateRenameProcedure (CommandInfo info)
		{
			BaseNode node = (BaseNode)CurrentNode.DataItem;
			info.Enabled = node.ConnectionContext.SchemaProvider.SupportsSchemaOperation (SqlStatementType.Rename, SqlSchemaType.Procedure);
		}
		
		[CommandUpdateHandler (ConnectionManagerCommands.AlterProcedure)]
		protected void OnUpdateAlterProcedure (CommandInfo info)
		{
			BaseNode node = (BaseNode)CurrentNode.DataItem;
			info.Enabled = node.ConnectionContext.SchemaProvider.SupportsSchemaOperation (SqlStatementType.Alter, SqlSchemaType.Procedure);
		}
	}
}
