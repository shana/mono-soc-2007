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
	public class ViewNodeBuilder : TypeNodeBuilder
	{
		public ViewNodeBuilder ()
			: base ()
		{
		}
		
		public override Type NodeDataType {
			get { return typeof (ViewNode); }
		}
		
		public override string ContextMenuAddinPath {
			get { return "/SharpDevelop/Views/ConnectionManagerPad/ContextMenu/ViewNode"; }
		}
		
		public override Type CommandHandlerType {
			get { return typeof (ViewNodeCommandHandler); }
		}
		
		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return GettextCatalog.GetString ("View");
		}
		
		public override void BuildNode (ITreeBuilder builder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			ViewNode node = dataObject as ViewNode;

			label = node.View.Name;
			icon = Context.GetIcon ("md-db-view");
		}
		
		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			ViewNode node = dataObject as ViewNode;
			NodeState nodeState = new NodeState (builder, node.ConnectionContext, dataObject);
			
			ThreadPool.QueueUserWorkItem (new WaitCallback (BuildChildNodesThreaded), nodeState);
		}
		
		private void BuildChildNodesThreaded (object state)
		{
			NodeState nodeState = state as NodeState;
			ISchemaProvider provider = nodeState.ConnectionContext.SchemaProvider;
			
			if (MetaDataService.IsApplied (provider, typeof (ColumnMetaDataAttribute))) {
				ViewSchema view = (nodeState.DataObject as ViewNode).View;
				Services.DispatchService.GuiDispatch (delegate {
					nodeState.TreeBuilder.AddChild (new ColumnsNode (nodeState.ConnectionContext, view));
					nodeState.TreeBuilder.Expanded = true;
				});
			}
		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}
	}
	
	public class ViewNodeCommandHandler : NodeCommandHandler
	{
		public override DragOperation CanDragNode ()
		{
			return DragOperation.None;
		}
		
		public override void RenameItem (string newName)
		{
			//TODO: check if a view with the same name already exists
			ViewNode node = CurrentNode.DataItem as ViewNode;
			
			node.View.Name = newName;
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
		
		[CommandHandler (ConnectionManagerCommands.AlterView)]
		protected void OnAlterView ()
		{
			ViewNode node = CurrentNode.DataItem as ViewNode;
			IDbFactory fac = node.ConnectionContext.DbFactory;
			ISchemaProvider schemaProvider = node.ConnectionContext.SchemaProvider;
			
			if (fac.GuiProvider.ShowViewEditorDialog (schemaProvider, node.View, false))
				ThreadPool.QueueUserWorkItem (new WaitCallback (OnAlterViewThreaded), CurrentNode.DataItem);
		}
		
		private void OnAlterViewThreaded (object state)
		{
			ViewNode node = (ViewNode)state;
			ISchemaProvider provider = node.ConnectionContext.SchemaProvider;
			
			provider.AlterView (node.View);
		}
		
		[CommandHandler (ConnectionManagerCommands.DropView)]
		protected void OnDropView ()
		{
			ViewNode node = (ViewNode)CurrentNode.DataItem;
			if (Services.MessageService.AskQuestion (
				GettextCatalog.GetString ("Are you sure you want to drop view '{0}'", node.View.Name),
				GettextCatalog.GetString ("Drop View")
			)) {
				ThreadPool.QueueUserWorkItem (new WaitCallback (OnDropViewThreaded), CurrentNode.DataItem);
			}
		}
		
		private void OnDropViewThreaded (object state)
		{
			ViewNode node = (ViewNode)state;
			ISchemaProvider provider = node.ConnectionContext.SchemaProvider;
			
			provider.DropView (node.View);
		}
		
		[CommandHandler (ConnectionManagerCommands.Rename)]
		protected void OnRenameView ()
		{
			Tree.StartLabelEdit ();
		}
		
		[CommandUpdateHandler (ConnectionManagerCommands.DropView)]
		protected void OnUpdateDropView (CommandInfo info)
		{
			BaseNode node = (BaseNode)CurrentNode.DataItem;
			info.Enabled = MetaDataService.IsViewMetaDataSupported (node.ConnectionContext.SchemaProvider, ViewMetaData.Drop);
		}
		
		[CommandUpdateHandler (ConnectionManagerCommands.Rename)]
		protected void OnUpdateRenameView (CommandInfo info)
		{
			BaseNode node = (BaseNode)CurrentNode.DataItem;
			info.Enabled = MetaDataService.IsViewMetaDataSupported (node.ConnectionContext.SchemaProvider, ViewMetaData.Rename);
		}
		
		[CommandUpdateHandler (ConnectionManagerCommands.AlterView)]
		protected void OnUpdateAlterView (CommandInfo info)
		{
			BaseNode node = (BaseNode)CurrentNode.DataItem;
			info.Enabled = MetaDataService.IsViewMetaDataSupported (node.ConnectionContext.SchemaProvider, ViewMetaData.Alter);
		}
	}
}
