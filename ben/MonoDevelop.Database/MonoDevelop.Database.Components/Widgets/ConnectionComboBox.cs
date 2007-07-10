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
using Mono.Data.Sql;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using MonoDevelop.Projects.Gui;

namespace MonoDevelop.Database.Components
{
	public class ConnectionComboBox : ComboBox
	{
		private ListStore store;
		
		//TODO: listen for .Add, .Remove, .Edit
		public ConnectionComboBox ()
		{
			store = new ListStore (typeof (ConnectionSettings));

			CellRendererText nameRenderer = new CellRendererText ();
			this.PackStart (nameRenderer, true);
			SetCellDataFunc (nameRenderer, new CellLayoutDataFunc (NameDataFunc));
			
			this.Model = store;
			
			foreach (ConnectionSettings settings in ConnectionSettingsService.Connections)
				store.AppendValues (settings);
			if (this.Active < 0)
				this.Active = 0;
		}
		
		public ConnectionSettings ConnectionSettings {
			get {
				TreeIter iter;
				if (this.GetActiveIter (out iter))
					return store.GetValue (iter, 0) as ConnectionSettings;
				return null;
			}
			set {
				TreeIter iter;
				if (store.GetIterFirst (out iter)) {
					do {
						ConnectionSettings settings = store.GetValue (iter, 0) as ConnectionSettings;
						if (settings == value) {
							SetActiveIter (iter);
							return;
						}
					} while (store.IterNext (ref iter));
				}
			}
		}
		
		private void NameDataFunc (CellLayout layout, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			CellRendererText textRenderer = cell as CellRendererText;
			ConnectionSettings settings = model.GetValue (iter, 0) as ConnectionSettings;
			textRenderer.Text = settings.Name;
		}
	}
}