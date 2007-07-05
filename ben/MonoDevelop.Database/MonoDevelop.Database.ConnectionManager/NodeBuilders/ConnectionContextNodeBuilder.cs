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
using Mono.Data.Sql;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Ide.Gui.Pads;

namespace MonoDevelop.Database.ConnectionManager
{
	public class ConnectionContextNodeBuilder : TypeNodeBuilder
	{
		private object threadSync = new object ();
		private ITreeBuilder treeBuilder;
		private ConnectionContext context;
		
		public ConnectionContextNodeBuilder ()
			: base ()
		{
		}
		
		public override Type NodeDataType {
			get { return typeof (ConnectionContext); }
		}
		
		public override string ContextMenuAddinPath {
			get { return "/SharpDevelop/Views/ConnectionManagerPad/ContextMenu/ConnectionNode"; }
		}
		
		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return GettextCatalog.GetString ("Database Connections");
		}
		
		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			ConnectionContext context = (ConnectionContext) dataObject;
			label = context.ConnectionSettings.Name;
			icon = Context.GetIcon ("md-db-connection");
		}
		
		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			lock (threadSync) {
				treeBuilder = builder;
				context = (ConnectionContext) dataObject;
				
				ThreadPool.QueueUserWorkItem (new WaitCallback (BuildChildNodesThreaded));
			}
		}
		
		private void BuildChildNodesThreaded (object state)
		{
			ITreeBuilder treeBuilder = null;
			ConnectionContext context = null;
			
			lock (threadSync) {
				treeBuilder = this.treeBuilder;
				context = this.context;
			}

			string error = null;
			if (!context.ConnectionProvider.IsOpen && !context.ConnectionProvider.Open (out error)) {
				//TODO: show error message
				return;
			}
			
			ISchemaProvider provider = context.SchemaProvider;
			
			if (provider.SupportsSchemaType (typeof (TableSchema)))
				Services.DispatchService.GuiDispatch (delegate {
					treeBuilder.AddChild (new TablesNode (context));
				});

			if (provider.SupportsSchemaType (typeof (ViewSchema)))
				Services.DispatchService.GuiDispatch (delegate {
					treeBuilder.AddChild (new ViewsNode (context));
				});
			
			if (provider.SupportsSchemaType (typeof (ProcedureSchema)))
				Services.DispatchService.GuiDispatch (delegate {
					treeBuilder.AddChild (new ProceduresNode (context));
				});
			
			if (provider.SupportsSchemaType (typeof (AggregateSchema)))
				Services.DispatchService.GuiDispatch (delegate {
					treeBuilder.AddChild (new AggregatesNode (context));
				});
			
			if (provider.SupportsSchemaType (typeof (GroupSchema)))
				Services.DispatchService.GuiDispatch (delegate {
					treeBuilder.AddChild (new GroupsNode (context));
				});
			
			if (provider.SupportsSchemaType (typeof (LanguageSchema)))
				Services.DispatchService.GuiDispatch (delegate {
					treeBuilder.AddChild (new LanguagesNode (context));
				});
			
			if (provider.SupportsSchemaType (typeof (OperatorSchema)))
				Services.DispatchService.GuiDispatch (delegate {
					treeBuilder.AddChild (new OperatorsNode (context));
				});
			
			if (provider.SupportsSchemaType (typeof (RoleSchema)))
				Services.DispatchService.GuiDispatch (delegate {
					treeBuilder.AddChild (new RolesNode (context));
				});
			
			if (provider.SupportsSchemaType (typeof (SequenceSchema)))
				Services.DispatchService.GuiDispatch (delegate {
					treeBuilder.AddChild (new SequencesNode (context));
				});
			
			if (provider.SupportsSchemaType (typeof (UserSchema)))
				Services.DispatchService.GuiDispatch (delegate {
					treeBuilder.AddChild (new UsersNode (context));
				});
			
			if (provider.SupportsSchemaType (typeof (DataTypeSchema)))
				Services.DispatchService.GuiDispatch (delegate {
					treeBuilder.AddChild (new TypesNode (context));
				});
			
			Services.DispatchService.GuiDispatch (delegate {
				treeBuilder.Expanded = true;
			});
		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}
	}
}
