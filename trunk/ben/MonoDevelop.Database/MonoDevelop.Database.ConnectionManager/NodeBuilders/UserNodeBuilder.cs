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
using MonoDevelop.Database.Designer;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Ide.Gui.Pads;
using MonoDevelop.Components.Commands;

namespace MonoDevelop.Database.ConnectionManager
{
	public class UserNodeBuilder : TypeNodeBuilder
	{
		public UserNodeBuilder ()
			: base ()
		{
		}
		
		public override Type NodeDataType {
			get { return typeof (UserNode); }
		}
		
		public override string ContextMenuAddinPath {
			get { return "/SharpDevelop/Users/ConnectionManagerPad/ContextMenu/UserNode"; }
		}
		
		public override Type CommandHandlerType {
			get { return typeof (UserNodeCommandHandler); }
		}
		
		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return GettextCatalog.GetString ("User");
		}
		
		public override void BuildNode (ITreeBuilder builder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			UserNode node = dataObject as UserNode;

			label = node.User.Name;
			icon = Context.GetIcon ("md-db-user");
		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return false;
		}
	}
	
	public class UserNodeCommandHandler : NodeCommandHandler
	{
		public override DragOperation CanDragNode ()
		{
			return DragOperation.None;
		}
		
		[CommandHandler (ConnectionManagerCommands.Refresh)]
		protected void OnRefresh ()
		{
			CurrentNode.MoveToParent ();
			BaseNode node = CurrentNode.DataItem as BaseNode;
			if (node != null)
				node.Refresh ();
			CurrentNode.ExpandToNode ();
		}
		
		[CommandHandler (ConnectionManagerCommands.AlterUser)]
		protected void OnAlterUser ()
		{
			
		}
		
		[CommandHandler (ConnectionManagerCommands.DropUser)]
		protected void OnDropUser ()
		{
			UserNode node = (UserNode)CurrentNode.DataItem;
			if (Services.MessageService.AskQuestion (
				GettextCatalog.GetString ("Are you sure you want to drop user '{0}'", node.User.Name),
				GettextCatalog.GetString ("Drop User")
			)) {
				ThreadPool.QueueUserWorkItem (new WaitCallback (OnDropUserThreaded), CurrentNode.DataItem);
			}
		}
			
		private void OnDropUserThreaded (object state)
		{
			UserNode node = (UserNode)state;
			ISchemaProvider provider = node.ConnectionContext.SchemaProvider;
			
			provider.DropUser (node.User);
		}
		
		[CommandHandler (ConnectionManagerCommands.Rename)]
		protected void OnRenameUser ()
		{
			Tree.StartLabelEdit ();
		}
		
		[CommandUpdateHandler (ConnectionManagerCommands.DropUser)]
		protected void OnUpdateDropUser (CommandInfo info)
		{
			BaseNode node = (BaseNode)CurrentNode.DataItem;
			info.Enabled = node.ConnectionContext.SchemaProvider.SupportsSchemaOperation (OperationMetaData.Drop, SchemaMetaData.User);
		}
		
		[CommandUpdateHandler (ConnectionManagerCommands.Rename)]
		protected void OnUpdateRenameUser (CommandInfo info)
		{
			BaseNode node = (BaseNode)CurrentNode.DataItem;
			info.Enabled = node.ConnectionContext.SchemaProvider.SupportsSchemaOperation (OperationMetaData.Rename, SchemaMetaData.User);
		}
		
		[CommandUpdateHandler (ConnectionManagerCommands.AlterUser)]
		protected void OnUpdateAlterUser (CommandInfo info)
		{
			BaseNode node = (BaseNode)CurrentNode.DataItem;
			info.Enabled = node.ConnectionContext.SchemaProvider.SupportsSchemaOperation (OperationMetaData.Alter, SchemaMetaData.User);
		}
	}
}
