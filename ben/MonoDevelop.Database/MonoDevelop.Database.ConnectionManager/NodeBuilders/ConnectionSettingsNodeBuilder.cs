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
using Mono.Data.Sql;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Ide.Gui.Pads;
using MonoDevelop.Database.Query;
using MonoDevelop.Database.Components;
using MonoDevelop.Components.Commands;

namespace MonoDevelop.Database.ConnectionManager
{
	public class ConnectionSettingsNodeBuilder : TypeNodeBuilder
	{
		private object sync = new object ();
		private ITreeBuilder builder;
		private ConnectionSettings settings;
		
		public ConnectionSettingsNodeBuilder ()
			: base ()
		{
		}
		
		public override Type NodeDataType {
			get { return typeof (ConnectionSettings); }
		}
		
		public override string ContextMenuAddinPath {
			get { return "/SharpDevelop/Views/ConnectionManagerPad/ContextMenu/ConnectionNode"; }
		}
		
		public override Type CommandHandlerType {
			get { return typeof (ConnectionSettingsCommandHandler); }
		}
		
		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return GettextCatalog.GetString ("Database Connections");
		}
		
		public override void BuildNode (ITreeBuilder builder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			ConnectionSettings settings = dataObject as ConnectionSettings;
			
			label = settings.Name;
			if (settings.HasConnectionProvider) {
				IConnectionProvider provider = settings.ConnectionProvider;
				if (provider.IsConnectionError)
					icon = Context.GetIcon ("md-db-database-error");
				else if (provider.IsOpen)
					icon = Context.GetIcon ("md-db-database-ok");
				else
					icon = Context.GetIcon ("md-db-database");
			} else {
				icon = Context.GetIcon ("md-db-database");
			}
		}
		
		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			ConnectionSettings settings = dataObject as ConnectionSettings;
			lock (sync) {
				this.builder = builder;
				this.settings = settings;
			}
			
			ThreadPool.QueueUserWorkItem (new WaitCallback (BuildChildNodesThreaded));
		}
		
		private void BuildChildNodesThreaded (object state)
		{
			ITreeBuilder builder = null;
			ConnectionSettings settings = null;
			
			lock (sync) {
				builder = this.builder;
				settings = this.settings;
			}
			
			QueryService.EnsureConnection (settings, new ConnectionSettingsCallback (BuildChildNodesGui));
		}
		
		private void BuildChildNodesGui (ConnectionSettings settings, bool connected)
		{
			ITreeBuilder builder = null;
			
			lock (sync) {
				builder = this.builder;
			}
			
			builder.Update ();
			if (connected) {
				ISchemaProvider provider = settings.SchemaProvider;
				if (provider.SupportsSchemaType (typeof (TableSchema)))
					builder.AddChild (new TablesNode (settings));

				if (provider.SupportsSchemaType (typeof (ViewSchema)))
					builder.AddChild (new ViewsNode (settings));
				
				if (provider.SupportsSchemaType (typeof (ProcedureSchema)))
					builder.AddChild (new ProceduresNode (settings));
				
				if (provider.SupportsSchemaType (typeof (AggregateSchema)))
					builder.AddChild (new AggregatesNode (settings));
				
				if (provider.SupportsSchemaType (typeof (GroupSchema)))
					builder.AddChild (new GroupsNode (settings));
				
				if (provider.SupportsSchemaType (typeof (LanguageSchema)))
					builder.AddChild (new LanguagesNode (settings));
				
				if (provider.SupportsSchemaType (typeof (OperatorSchema)))
					builder.AddChild (new OperatorsNode (settings));
				
				if (provider.SupportsSchemaType (typeof (RoleSchema)))
					builder.AddChild (new RolesNode (settings));
				
				if (provider.SupportsSchemaType (typeof (SequenceSchema)))
					builder.AddChild (new SequencesNode (settings));
				
				if (provider.SupportsSchemaType (typeof (UserSchema)))
					builder.AddChild (new UsersNode (settings));
				
				if (provider.SupportsSchemaType (typeof (DataTypeSchema)))
					builder.AddChild (new TypesNode (settings));
				
				builder.Expanded = true;
			}
		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}
	}
	
	public class ConnectionSettingsCommandHandler : NodeCommandHandler
	{
		[CommandHandler (ConnectionManagerCommands.RemoveConnection)]
		protected void OnRemoveConnection ()
		{
			//TODO: dialog to confirm
			ConnectionSettings settings = (ConnectionSettings) CurrentNode.DataItem;
			ConnectionSettingsService.RemoveConnection (settings);
		}
		
		[CommandHandler (ConnectionManagerCommands.EditConnection)]
		protected void OnEditConnection ()
		{
			ConnectionSettings settings = (ConnectionSettings) CurrentNode.DataItem;
			ConnectionDialog dlg = new ConnectionDialog (settings);
			try {
				if (dlg.Run () == (int)ResponseType.Ok) {
					ConnectionSettingsService.EditConnection (settings);
					OnRefreshConnection ();
				}
			} finally {
				dlg.Destroy ();
			}
		}
		
		[CommandHandler (ConnectionManagerCommands.RefreshConnection)]
		protected void OnRefreshConnection ()
		{
			ConnectionSettings settings = (ConnectionSettings) CurrentNode.DataItem;
			//TODO: refresh
		}
		
		[CommandHandler (ConnectionManagerCommands.DisconnectConnection)]
		protected void OnDisconnectConnection ()
		{
			ConnectionSettings settings = (ConnectionSettings) CurrentNode.DataItem;
			if (settings.HasConnectionProvider)
				settings.ConnectionProvider.Close ();
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
	}
}
