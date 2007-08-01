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
			get { return "/SharpDevelop/Views/ConnectionManagerPad/ContextMenu/ProcedureNode"; }
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
			ISchemaProvider schemaProvider = nodeState.ConnectionContext.SchemaProvider;

			if (MetaDataService.IsApplied (schemaProvider, typeof (ParameterMetaDataAttribute))) {
				ProcedureSchema procedure = (nodeState.DataObject as ProcedureNode).Procedure;
				Services.DispatchService.GuiDispatch (delegate {
					nodeState.TreeBuilder.AddChild (new ParametersNode (nodeState.ConnectionContext, procedure));
					nodeState.TreeBuilder.Expanded = true;
				});
			}
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
		
		public override void RenameItem (string newName)
		{
			
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
		
		[CommandHandler (ConnectionManagerCommands.AlterProcedure)]
		protected void OnAlterProcedure ()
		{
			ProcedureNode node = CurrentNode.DataItem as ProcedureNode;
			IDbFactory fac = node.ConnectionContext.DbFactory;
			ISchemaProvider schemaProvider = node.ConnectionContext.SchemaProvider;
			
			if (fac.GuiProvider.ShowProcedureEditorDialog (schemaProvider, node.Procedure, false))
				ThreadPool.QueueUserWorkItem (new WaitCallback (OnAlterProcedureThreaded), CurrentNode.DataItem);
		}
		
		private void OnAlterProcedureThreaded (object state)
		{
			ProcedureNode node = (ProcedureNode)state;
			ISchemaProvider provider = node.ConnectionContext.SchemaProvider;
			
			provider.AlterProcedure (node.Procedure);
		}
		
		[CommandHandler (ConnectionManagerCommands.DropProcedure)]
		protected void OnDropProcedure ()
		{
			ProcedureNode node = (ProcedureNode)CurrentNode.DataItem;
			if (Services.MessageService.AskQuestion (
				GettextCatalog.GetString ("Are you sure you want to drop procedure '{0}'", node.Procedure.Name),
				GettextCatalog.GetString ("Drop Procedure")
			)) {
				ThreadPool.QueueUserWorkItem (new WaitCallback (OnDropProcedureThreaded), CurrentNode.DataItem);
			}
		}
			
		private void OnDropProcedureThreaded (object state)
		{
			ProcedureNode node = (ProcedureNode)state;
			ISchemaProvider provider = node.ConnectionContext.SchemaProvider;
			
			provider.DropProcedure (node.Procedure);
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
			info.Enabled = MetaDataService.IsProcedureMetaDataSupported (node.ConnectionContext.SchemaProvider, ProcedureMetaData.Drop);
		}
		
		[CommandUpdateHandler (ConnectionManagerCommands.Rename)]
		protected void OnUpdateRenameProcedure (CommandInfo info)
		{
			BaseNode node = (BaseNode)CurrentNode.DataItem;
			info.Enabled = MetaDataService.IsProcedureMetaDataSupported (node.ConnectionContext.SchemaProvider, ProcedureMetaData.Rename);
		}
		
		[CommandUpdateHandler (ConnectionManagerCommands.AlterProcedure)]
		protected void OnUpdateAlterProcedure (CommandInfo info)
		{
			BaseNode node = (BaseNode)CurrentNode.DataItem;
			info.Enabled = MetaDataService.IsProcedureMetaDataSupported (node.ConnectionContext.SchemaProvider, ProcedureMetaData.Alter);
		}
	}
}
