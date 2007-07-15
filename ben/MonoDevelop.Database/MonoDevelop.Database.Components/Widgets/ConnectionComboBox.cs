//
// Authors:
//   Ben Motmans  <ben.motmans@gmail.com>
//
// Copyright (c) 2007 Ben Motmans
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using Gtk;
using System;
using System.Collections.Generic;
usingMonoDevelop.Database.Sql;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;

namespace MonoDevelop.Database.Components
{
	public class ConnectionComboBox : ComboBox
	{
		private ListStore store;
		
		//TODO: listen for .Add, .Remove, .Edit
		public ConnectionComboBox ()
		{
			store = new ListStore (/*typeof (Gdk.Pixbuf), */typeof (string), typeof (object));
			this.Model = store;

			CellRendererText nameRenderer = new CellRendererText ();
			//CellRendererPixbuf pixbufRenderer = new CellRendererPixbuf ();
			
			//AddAttribute (pixbufRenderer, "pixbuf", 0);
			AddAttribute (nameRenderer, "text", 1);
			
			//PackStart (pixbufRenderer, false);
			PackEnd (nameRenderer, true);

			foreach (ConnectionSettings settings in ConnectionSettingsService.Connections) {
				string iconString = "md-db-database";
				if (settings.HasConnectionPool && settings.ConnectionPool.IsInitialized) {
					iconString = "md-db-database-ok";
				}
				store.AppendValues (/*Services.Resources.GetIcon (iconString),*/ settings.Name, settings);
			}
			if (this.Active < 0)
				this.Active = 0;
		}
		
		public ConnectionSettings ConnectionSettings {
			get {
				TreeIter iter;
				if (this.GetActiveIter (out iter))
					return store.GetValue (iter, 1) as ConnectionSettings;
				return null;
			}
			set {
				TreeIter iter;
				if (store.GetIterFirst (out iter)) {
					do {
						ConnectionSettings settings = store.GetValue (iter, 1) as ConnectionSettings;
						if (settings == value) {
							SetActiveIter (iter);
							return;
						}
					} while (store.IterNext (ref iter));
				}
			}
		}
	}
}