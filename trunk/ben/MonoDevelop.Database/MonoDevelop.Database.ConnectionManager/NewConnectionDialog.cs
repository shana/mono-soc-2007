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
using Mono.Data.Sql;
using Mono.Addins;
using MonoDevelop.Core;

namespace MonoDevelop.Database.ConnectionManager
{
	public partial class NewConnectionDialog : Gtk.Dialog
	{
		private ConnectionSettings settings;
		private DbFactoryService service;

		private ListStore providerStore;
		
		public NewConnectionDialog ()
			: this (true)
		{
		}
		
		public NewConnectionDialog (bool showDefault)
		{
			this.Build();
			service = ServiceManager.GetService (typeof (DbFactoryService)) as DbFactoryService;
			
			providerStore = new ListStore (typeof (string), typeof (IDbFactory));
			CellRendererText text = new CellRendererText ();
			comboProvider.PackStart (text, true);
			comboProvider.AddAttribute (text, "text", 0);
			
			foreach (IDbFactory fac in service.DbFactories)
				providerStore.AppendValues (fac.Name, fac);
			
			if (showDefault) {
				TreeIter iter;
				if (providerStore.GetIterFirst (out iter))
					comboProvider.SetActiveIter (iter);
			}
		}
		
		public NewConnectionDialog (ConnectionSettings settings)
			: this (false)
		{
			this.settings = settings;
			ShowConnectionSettings (settings, true);
		}
		
		public ConnectionSettings ConnectionSettings {
			get { return settings; }
		}

		protected virtual void RadioGeneratedChange (object sender, System.EventArgs e)
		{
			alignmentGenerated.Sensitive = radioGenerated.Active;
			alignmentCustom.Sensitive = !radioGenerated.Active;
		}

		protected virtual void CheckPoolingActivated (object sender, System.EventArgs e)
		{
			spinMinPool.Editable = checkPooling.Active;
			spinMinPool.Editable = checkPooling.Active;
		}

		protected virtual void ComboProviderChanged (object sender, System.EventArgs e)
		{
			if (settings != null)
				return;
			
			TreeIter iter;
			if (comboProvider.GetActiveIter (out iter)) {
				IDbFactory fac = providerStore.GetValue (iter, 1) as IDbFactory;
				settings = fac.GetDefaultConnectionSettings ();
				ShowConnectionSettings (settings, false);
			}
		}
		
		private void ShowConnectionSettings (ConnectionSettings settings, bool updateCombo)
		{
			radioGenerated.Active = !settings.UseConnectionString;
			entryName.Text = String.IsNullOrEmpty (settings.Name) ? String.Empty : settings.Name;
			entryPassword.Text = String.IsNullOrEmpty (settings.Password) ? String.Empty : settings.Password;
			entryPort.Text = settings.Port.ToString ();
			entryServer.Text = String.IsNullOrEmpty (settings.Server) ? String.Empty : settings.Server;
			entryUsername.Text = String.IsNullOrEmpty (settings.Username) ? String.Empty : settings.Username;
			textConnectionString.Buffer.Text = String.IsNullOrEmpty (settings.ConnectionString) ? String.Empty : settings.ConnectionString;
			comboDatabase.Entry.Text = String.IsNullOrEmpty (settings.Database) ? String.Empty : settings.Database;
			checkPooling.Active = settings.EnablePooling;
			spinMinPool.Value = settings.MinPoolSize;
			spinMaxPool.Value = settings.MaxPoolSize;
			
			if (updateCombo) {
				TreeIter iter;
				if (providerStore.GetIterFirst (out iter)) {
					do {
						IDbFactory fac = providerStore.GetValue (iter, 1) as IDbFactory;
						if (settings.ProviderIdentifier == fac.Identifier) {
							comboProvider.SetActiveIter (iter);
							return;
						}
					} while (providerStore.IterNext (ref iter));
				}
			}
		}
	}
}
