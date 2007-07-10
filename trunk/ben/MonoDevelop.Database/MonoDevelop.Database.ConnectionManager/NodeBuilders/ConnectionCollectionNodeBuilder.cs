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

using Mono.Data.Sql;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Ide.Gui.Pads;

namespace MonoDevelop.Database.ConnectionManager
{
	public class ConnectionCollectionNodeBuilder : TypeNodeBuilder
	{
		private ITreeBuilder builder;
		
		public ConnectionCollectionNodeBuilder ()
			: base ()
		{
			ConnectionSettingsService.ConnectionAdded += (ConnectionSettingsEventHandler)Services.DispatchService.GuiDispatch (new ConnectionSettingsEventHandler (OnConnectionAdded));
			ConnectionSettingsService.ConnectionRemoved += (ConnectionSettingsEventHandler)Services.DispatchService.GuiDispatch (new ConnectionSettingsEventHandler (OnConnectionRemoved));
		}
		
		public override Type NodeDataType {
			get { return typeof (ConnectionSettingsCollection); }
		}
		
		public override string ContextMenuAddinPath {
			get { return "/SharpDevelop/Views/ConnectionManagerPad/ContextMenu/ConnectionsNode"; }
		}
		
		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return GettextCatalog.GetString ("Database Connections");
		}
		
		public override void BuildNode (ITreeBuilder builder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			label = GettextCatalog.GetString ("Database Connections");
			icon = Context.GetIcon ("md-db-connection");
			this.builder = builder;
		}
		
		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			ConnectionSettingsCollection collection = (ConnectionSettingsCollection) dataObject;
				
			foreach (ConnectionSettings settings in collection)
				builder.AddChild (settings);
			builder.Expanded = true;
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			ConnectionSettingsCollection collection = (ConnectionSettingsCollection) dataObject;
			return collection.Count > 0;
		}
		
		private void OnConnectionAdded (object sender, ConnectionSettingsEventArgs args)
		{
			builder.AddChild (args.ConnectionSettings);
		}
		
		private void OnConnectionRemoved (object sender, ConnectionSettingsEventArgs args)
		{
			if (builder.MoveToObject (args.ConnectionSettings)) {
				builder.Remove ();
				builder.MoveToParent ();
			}
		}
	}
}
