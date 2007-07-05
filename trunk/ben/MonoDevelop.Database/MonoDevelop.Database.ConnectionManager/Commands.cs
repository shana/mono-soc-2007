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
using Mono.Data.Sql;
using MonoDevelop.Core.Gui;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Pads;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;

namespace MonoDevelop.Database.ConnectionManager
{
	public enum ConnectionManagerCommands
	{
		AddConnection,
		RemoveConnection,
		DisconnectConnection,
		RefreshConnectionList,
		RefreshConnection,
		Query,
		EmptyTable,
		DropTable,
		Refresh
	}
	
	public class AddConnectionHandler : CommandHandler
	{
		protected override void Run ()
		{
			ConnectionManagerService service = ServiceManager.GetService (typeof (ConnectionManagerService)) as ConnectionManagerService;
			NewConnectionDialog dlg = new NewConnectionDialog ();
			try {
				if (dlg.Run () == (int)ResponseType.Ok) {
					ConnectionSettings settings = dlg.ConnectionSettings;
					service.AddConnection (settings);
				}
			} finally {
				dlg.Destroy ();
			}
		}
	}
}