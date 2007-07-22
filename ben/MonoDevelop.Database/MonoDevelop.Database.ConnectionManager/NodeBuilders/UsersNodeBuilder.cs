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
	public class UsersNodeBuilder : TypeNodeBuilder
	{
		private EventHandler RefreshHandler;
		
		public UsersNodeBuilder ()
			: base ()
		{
			RefreshHandler = new EventHandler (OnRefreshEvent);
		}
		
		public override Type NodeDataType {
			get { return typeof (UsersNode); }
		}
		
		public override string ContextMenuAddinPath {
			get { return "/SharpDevelop/Views/ConnectionManagerPad/ContextMenu/UsersNode"; }
		}
		
		public override Type CommandHandlerType {
			get { return typeof (UsersNodeCommandHandler); }
		}
		
		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return GettextCatalog.GetString ("Users");
		}
		
		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			label = GettextCatalog.GetString ("Users");
			icon = Context.GetIcon ("md-db-users");
			
			BaseNode node = (BaseNode) dataObject;
			node.RefreshEvent += RefreshHandler;
		}
		
		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			BaseNode node = dataObject as BaseNode;
			NodeState nodeState = new NodeState (builder, node.ConnectionContext, dataObject);
			
			ThreadPool.QueueUserWorkItem (new WaitCallback (BuildChildNodesThreaded), nodeState);
		}
		
		private void BuildChildNodesThreaded (object state)
		{
			NodeState nodeState = state as NodeState;
			UserSchemaCollection users = nodeState.ConnectionContext.SchemaProvider.GetUsers ();
			foreach (UserSchema user in users) {
				Services.DispatchService.GuiDispatch (delegate {
					nodeState.TreeBuilder.AddChild (new UserNode (nodeState.ConnectionContext, user));
					nodeState.TreeBuilder.Expanded = true;
				});
			}
		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}
		
		private void OnRefreshEvent (object sender, EventArgs args)
		{
			ITreeBuilder builder = Context.GetTreeBuilder ();
			
			if (builder != null)
				builder.UpdateChildren ();
			
			builder.ExpandToNode ();
		}
	}
	
	public class UsersNodeCommandHandler : NodeCommandHandler
	{
		public override DragOperation CanDragNode ()
		{
			return DragOperation.None;
		}
		
		[CommandHandler (ConnectionManagerCommands.Refresh)]
		protected void OnRefresh ()
		{
			BaseNode node = CurrentNode.DataItem as BaseNode;
			node.Refresh ();
		}
		
		[CommandHandler (ConnectionManagerCommands.CreateUser)]
		protected void OnCreateUser ()
		{
			
		}
		
		[CommandUpdateHandler (ConnectionManagerCommands.CreateUser)]
		protected void OnUpdateCreateUser (CommandInfo info)
		{
			BaseNode node = (BaseNode)CurrentNode.DataItem;
			info.Enabled = node.ConnectionContext.SchemaProvider.SupportsSchemaOperation (SqlStatementType.Create, SqlSchemaType.User);
		}
	}
}