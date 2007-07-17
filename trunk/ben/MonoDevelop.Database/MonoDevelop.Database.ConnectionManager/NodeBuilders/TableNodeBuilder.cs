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
using System.Data;
using System.Threading;
using System.Collections.Generic;
using MonoDevelop.Database.Sql;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Pads;
using MonoDevelop.Database.Query;
using MonoDevelop.Database.Components;
using MonoDevelop.Components.Commands;

namespace MonoDevelop.Database.ConnectionManager
{
	public class TableNodeBuilder : TypeNodeBuilder
	{
		public TableNodeBuilder ()
			: base ()
		{
		}
		
		public override Type NodeDataType {
			get { return typeof (TableNode); }
		}
		
		public override string ContextMenuAddinPath {
			get { return "/SharpDevelop/Views/ConnectionManagerPad/ContextMenu/TableNode"; }
		}
		
		public override Type CommandHandlerType {
			get { return typeof (TableNodeCommandHandler); }
		}
		
		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return GettextCatalog.GetString ("Table");
		}
		
		public override void BuildNode (ITreeBuilder builder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			TableNode node = dataObject as TableNode;

			label = node.Table.Name;
			icon = Context.GetIcon ("md-db-table");
		}
		
		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			TableNode node = dataObject as TableNode;
			NodeState nodeState = new NodeState (builder, node.ConnectionContext, dataObject);
			
			ThreadPool.QueueUserWorkItem (new WaitCallback (BuildChildNodesThreaded), nodeState);
		}
		
		private void BuildChildNodesThreaded (object state)
		{
			NodeState nodeState = state as NodeState;
			ISchemaProvider provider = nodeState.ConnectionContext.SchemaProvider;
			
			if (provider.SupportsSchemaType (typeof (ColumnSchema)))
				Services.DispatchService.GuiDispatch (delegate {
					TableSchema table = (nodeState.DataObject as TableNode).Table;
					nodeState.TreeBuilder.AddChild (new ColumnsNode (nodeState.ConnectionContext, table));
				});
			
			if (provider.SupportsSchemaType (typeof (RuleSchema)))
				Services.DispatchService.GuiDispatch (delegate {
					nodeState.TreeBuilder.AddChild (new RulesNode (nodeState.ConnectionContext));
				});
			
			if (provider.SupportsSchemaType (typeof (ConstraintSchema)))
				Services.DispatchService.GuiDispatch (delegate {
					TableSchema table = (nodeState.DataObject as TableNode).Table;
					nodeState.TreeBuilder.AddChild (new ConstraintsNode (nodeState.ConnectionContext, table));
				});
			
			if (provider.SupportsSchemaType (typeof (TriggerSchema)))
				Services.DispatchService.GuiDispatch (delegate {
					nodeState.TreeBuilder.AddChild (new TriggersNode (nodeState.ConnectionContext));
				});
			
			Services.DispatchService.GuiDispatch (delegate {
				nodeState.TreeBuilder.Expanded = true;
			});
		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}
	}
	
	public class TableNodeCommandHandler : NodeCommandHandler
	{
		public override DragOperation CanDragNode ()
		{
			return DragOperation.None;
		}
		
		public override void ActivateItem ()
		{
			OnQueryCommand ();
		}
		
		[CommandHandler (ConnectionManagerCommands.Query)]
		protected void OnQueryCommand ()
		{
			TableNode node = (TableNode)CurrentNode.DataItem;
			
			IdentifierExpression tableId = new IdentifierExpression (node.Table.Name);
			SelectStatement sel = new SelectStatement (new FromTableClause (tableId));
			
			SqlQueryView view = new SqlQueryView ();
			view.SelectedConnectionContext = node.ConnectionContext;
			
			IDbFactory fac = DbFactoryService.GetDbFactory (node.ConnectionContext.ConnectionSettings);
			view.Text = fac.Dialect.GetSql (sel);

			IdeApp.Workbench.OpenDocument (view, true);
		}
		
		[CommandHandler (ConnectionManagerCommands.SelectColumns)]
		protected void OnSelectColumnsCommand ()
		{
			SelectColumnDialog dlg = new SelectColumnDialog (true);
			if (dlg.Run () == (int)Gtk.ResponseType.Ok) {
				TableNode node = (TableNode)CurrentNode.DataItem;
				IdentifierExpression tableId = new IdentifierExpression (node.Table.Name);
				List<IdentifierExpression> cols = new List<IdentifierExpression> ();
				foreach (ColumnSchema schema in dlg.CheckedColumns)
					cols.Add (new IdentifierExpression (schema.Name));
				
				SelectStatement sel = new SelectStatement (new FromTableClause (tableId), cols);

				IPooledDbConnection conn = node.ConnectionContext.ConnectionPool.Request ();
				IDbCommand command = conn.CreateCommand (sel);
				conn.ExecuteTableAsync (command, new ExecuteCallback<DataTable> (OnSelectCommandThreaded), null);
			}
		}
		
		[CommandHandler (ConnectionManagerCommands.SelectAll)]
		protected void OnSelectAllCommand ()
		{
			TableNode node = (TableNode)CurrentNode.DataItem;
			
			IdentifierExpression tableId = new IdentifierExpression (node.Table.Name);
			SelectStatement sel = new SelectStatement (new FromTableClause (tableId));

			IPooledDbConnection conn = node.ConnectionContext.ConnectionPool.Request ();
			IDbCommand command = conn.CreateCommand (sel);
			conn.ExecuteTableAsync (command, new ExecuteCallback<DataTable> (OnSelectCommandThreaded), null);
		}
		
		private void OnSelectCommandThreaded (IPooledDbConnection connection, DataTable table, object state)
		{
			connection.Release ();
				
			Services.DispatchService.GuiDispatch (delegate () {
				QueryResultView view = new QueryResultView (table);
				IdeApp.Workbench.OpenDocument (view, true);
			});
		}
		
		[CommandHandler (ConnectionManagerCommands.EmptyTable)]
		protected void OnEmptyTable ()
		{
			TableNode node = (TableNode)CurrentNode.DataItem;
			
			IdentifierExpression tableId = new IdentifierExpression (node.Table.Name);
			DeleteStatement del = new DeleteStatement (new FromTableClause (tableId));
			
			IPooledDbConnection conn = node.ConnectionContext.ConnectionPool.Request ();
			IDbCommand command = conn.CreateCommand (del);
			conn.ExecuteNonQueryAsync (command, new ExecuteCallback<int> (OnEmptyTableCallback), null);
		}
		
		private void OnEmptyTableCallback (IPooledDbConnection connection, int result, object state)
		{
			connection.Release ();

			Services.DispatchService.GuiDispatch (delegate () {
				IdeApp.Workbench.StatusBar.SetMessage (GettextCatalog.GetString ("Table emptied"));
			});
		}
		
		[CommandHandler (ConnectionManagerCommands.DropTable)]
		protected void OnDropTable ()
		{
			TableNode node = (TableNode)CurrentNode.DataItem;
			
			IdentifierExpression tableId = new IdentifierExpression (node.Table.Name);
			DropStatement drop = new DropStatement (tableId, DropStatementType.Table);
			
			IPooledDbConnection conn = node.ConnectionContext.ConnectionPool.Request ();
			IDbCommand command = conn.CreateCommand (drop);
			conn.ExecuteNonQueryAsync (command, new ExecuteCallback<int> (OnDropTableCallback), null);
		}
		
		private void OnDropTableCallback (IPooledDbConnection connection, int result, object state)
		{
			Services.DispatchService.GuiDispatch (delegate () {
				IdeApp.Workbench.StatusBar.SetMessage (GettextCatalog.GetString ("Table dropped"));
				OnRefresh ();
			});
		}
		
		[CommandHandler (ConnectionManagerCommands.Refresh)]
		protected void OnRefresh ()
		{
//			CurrentNode.MoveToParent ();
//			if (CurrentNode.DataItem as TablesNode != null)
//				(CurrentNode.DataItem as TablesNode).Refresh ();
//			
//			CurrentNode.ExpandToNode ();
		}
		
		[CommandHandler (ConnectionManagerCommands.AlterTable)]
		protected void OnAlterTable ()
		{
			
		}
		
		[CommandHandler (ConnectionManagerCommands.RenameTable)]
		protected void OnRenameTable ()
		{
			
		}
	}
}
