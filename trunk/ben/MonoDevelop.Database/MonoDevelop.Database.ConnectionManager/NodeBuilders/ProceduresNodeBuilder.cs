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
using Mono.Data.Sql;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Ide.Gui.Pads;

namespace MonoDevelop.Database.ConnectionManager
{
	public class ProceduresNodeBuilder : TypeNodeBuilder
	{
		private object threadSync = new object ();
		private ITreeBuilder treeBuilder;
		private ConnectionContext context;
		
		public ProceduresNodeBuilder ()
			: base ()
		{
		}
		
		public override Type NodeDataType {
			get { return typeof (ProceduresNode); }
		}
		
		public override string ContextMenuAddinPath {
			get { return "/SharpDevelop/Views/ConnectionManagerPad/ContextMenu/ProceduresNode"; }
		}
		
		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return GettextCatalog.GetString ("Procedures");
		}
		
		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			label = GettextCatalog.GetString ("Procedures");
			icon = Context.GetIcon ("md-db-procedures");
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
			
			bool showSystemObjects = (bool)treeBuilder.Options["ShowSystemObjects"];
			ICollection<ProcedureSchema> procedures = context.SchemaProvider.GetProcedures ();
			foreach (ProcedureSchema procedure in procedures) {
				if (procedure.IsSystemProcedure && !showSystemObjects)
					continue;
				
				Services.DispatchService.GuiDispatch (delegate {
					treeBuilder.AddChild (procedure);
					treeBuilder.Expanded = true;
				});
			}
		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}
	}
}
