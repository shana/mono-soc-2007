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

using Gtk;
using System;
using System.Threading;
using MonoDevelop.Database.Sql;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Ide.Gui.Pads;
using MonoDevelop.Database.Query;
using MonoDevelop.Database.Components;
using MonoDevelop.Components.Commands;

namespace MonoDevelop.Database.ConnectionManager
{
	public class ConnectionContextNodeBuilder : TypeNodeBuilder
	{
		public ConnectionContextNodeBuilder ()
			: base ()
		{
		}
		
		public override Type NodeDataType {
			get { return typeof (DatabaseConnectionContext); }
		}
		
		public override string ContextMenuAddinPath {
			get { return "/SharpDevelop/Views/ConnectionManagerPad/ContextMenu/ConnectionNode"; }
		}
		
		public override Type CommandHandlerType {
			get { return typeof (ConnectionContextCommandHandler); }
		}
		
		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return GettextCatalog.GetString ("Database Connections");
		}
		
		public override void BuildNode (ITreeBuilder builder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			DatabaseConnectionContext context = dataObject as DatabaseConnectionContext;
			
			label = context.ConnectionSettings.Name;
			if (context.HasConnectionPool) {
				IConnectionPool pool = context.ConnectionPool;
				if (pool.IsInitialized)
					icon = Context.GetIcon ("md-db-database-ok");
//				else if (provider.IsError)
//					icon = Context.GetIcon ("md-db-database-error");
				else
					icon = Context.GetIcon ("md-db-database");
			} else {
				icon = Context.GetIcon ("md-db-database");
			}
		}
		
		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			DatabaseConnectionContext context = dataObject as DatabaseConnectionContext;
			NodeState nodeState = new NodeState (builder, context, dataObject);
			
			ThreadPool.QueueUserWorkItem (new WaitCallback (BuildChildNodesThreaded), nodeState);
		}
		
		private void BuildChildNodesThreaded (object state)
		{
			NodeState nodeState = state as NodeState;
			
			QueryService.EnsureConnection (nodeState.ConnectionContext, new DatabaseConnectionContextCallback (BuildChildNodesGui), state);
		}
		
		private void BuildChildNodesGui (DatabaseConnectionContext context, bool connected, object state)
		{
			NodeState nodeState = state as NodeState;
			
			nodeState.TreeBuilder.Update ();
			if (connected) {
				ISchemaProvider provider = nodeState.ConnectionContext.SchemaProvider;

				if (MetaDataService.IsApplied (provider, typeof (TableMetaDataAttribute)))
					nodeState.TreeBuilder.AddChild (new TablesNode (nodeState.ConnectionContext));

				if (MetaDataService.IsApplied (provider, typeof (ViewMetaDataAttribute)))
					nodeState.TreeBuilder.AddChild (new ViewsNode (nodeState.ConnectionContext));
				
				if (MetaDataService.IsApplied (provider, typeof (ProcedureMetaDataAttribute)))
					nodeState.TreeBuilder.AddChild (new ProceduresNode (nodeState.ConnectionContext));

				if (MetaDataService.IsApplied (provider, typeof (UserMetaDataAttribute)))
					nodeState.TreeBuilder.AddChild (new UsersNode (nodeState.ConnectionContext));
				
				//TODO: custom datatypes, sequences, roles, operators, languages, groups and aggregates
				
				nodeState.TreeBuilder.Expanded = true;
			}
		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}
	}
	
	public class ConnectionContextCommandHandler : NodeCommandHandler
	{
		[CommandHandler (ConnectionManagerCommands.RemoveConnection)]
		protected void OnRemoveConnection ()
		{
			DatabaseConnectionContext context = (DatabaseConnectionContext) CurrentNode.DataItem;
			if (Services.MessageService.AskQuestion (
				GettextCatalog.GetString ("Are you sure you want to remove connection '{0}'?", context.ConnectionSettings.Name),
				GettextCatalog.GetString ("Remove Connection"))) {
				ConnectionContextService.RemoveDatabaseConnectionContext (context);
			}
		}
		
		[CommandHandler (ConnectionManagerCommands.EditConnection)]
		protected void OnEditConnection ()
		{
			DatabaseConnectionContext context = (DatabaseConnectionContext) CurrentNode.DataItem;
			DatabaseConnectionSettingsDialog dlg = new DatabaseConnectionSettingsDialog (context.ConnectionSettings);

			if (dlg.Run () == (int)ResponseType.Ok) {
				ConnectionContextService.EditDatabaseConnectionContext (context);
				OnRefreshConnection ();
			}
			dlg.Destroy ();
		}
		
		[CommandHandler (ConnectionManagerCommands.Refresh)]
		protected void OnRefreshConnection ()
		{
			DatabaseConnectionContext context = (DatabaseConnectionContext) CurrentNode.DataItem;
			//TODO: refresh
		}
		
		[CommandHandler (ConnectionManagerCommands.DisconnectConnection)]
		protected void OnDisconnectConnection ()
		{
			DatabaseConnectionContext context = (DatabaseConnectionContext) CurrentNode.DataItem;
			if (context.HasConnectionPool)
				context.ConnectionPool.Close ();
		}
		
		public override void ActivateItem ()
		{
			OnQueryCommand ();
		}
		
		[CommandHandler (ConnectionManagerCommands.Query)]
		protected void OnQueryCommand ()
		{
//			SqlQueryView sql = new SqlQueryView ();
//			sql.Connection = (DbProviderBase) CurrentNode.DataItem;
//			IdeApp.Workbench.OpenDocument (sql, true);
//			
//			CurrentNode.MoveToParent ();
		}
		
		[CommandUpdateHandler (ConnectionManagerCommands.DropDatabase)]
		protected void OnUpdateDropDatabase (CommandInfo info)
		{
			DatabaseConnectionContext context = (DatabaseConnectionContext) CurrentNode.DataItem;
			info.Enabled = MetaDataService.IsDatabaseMetaDataSupported (context.SchemaProvider, DatabaseMetaData.Drop);
		}

		[CommandUpdateHandler (ConnectionManagerCommands.AlterDatabase)]
		protected void OnUpdateAlterDatabase (CommandInfo info)
		{
			DatabaseConnectionContext context = (DatabaseConnectionContext) CurrentNode.DataItem;
			info.Enabled = MetaDataService.IsDatabaseMetaDataSupported (context.SchemaProvider, DatabaseMetaData.Alter);
		}
		
		[CommandHandler (ConnectionManagerCommands.RenameDatabase)]
		protected void OnRenameDatabase ()
		{
			//TODO: show a dialog, since inline tree renaming for this node renames the custom name
		}
		
		[CommandUpdateHandler (ConnectionManagerCommands.RenameDatabase)]
		protected void OnUpdateRenameDatabase (CommandInfo info)
		{
			DatabaseConnectionContext context = (DatabaseConnectionContext) CurrentNode.DataItem;
			info.Enabled = MetaDataService.IsDatabaseMetaDataSupported (context.SchemaProvider, DatabaseMetaData.Rename);
		}
	}
}
