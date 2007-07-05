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
using MonoDevelop.Core.Gui;

namespace MonoDevelop.Database.ConnectionManager
{
	public partial class NewConnectionDialog : Gtk.Dialog
	{
		private ConnectionSettings settings;
		private bool isDefaultSettings = true;

		private ListStore providerStore;
		
		public NewConnectionDialog ()
			: this (true)
		{
		}
		
		public NewConnectionDialog (bool showDefault)
		{
			this.Build ();

			providerStore = new ListStore (typeof (string), typeof (IDbFactory));
			comboProvider.Model = providerStore;
			CellRendererText text = new CellRendererText ();
			comboProvider.PackStart (text, true);
			comboProvider.AddAttribute (text, "text", 0);
			
			foreach (IDbFactory fac in DbFactoryService.DbFactories)
				providerStore.AppendValues (fac.Name, fac);
			
			if (showDefault) {
				TreeIter iter;
				if (providerStore.GetIterFirst (out iter))
					comboProvider.SetActiveIter (iter);
			}
			alignmentCustom.Sensitive = false;
		}
		
		public NewConnectionDialog (ConnectionSettings settings)
			: this (false)
		{
			this.settings = settings;
			isDefaultSettings = false;
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
			spinMinPool.Sensitive = checkPooling.Active;
			spinMinPool.Sensitive = checkPooling.Active;
		}

		protected virtual void ComboProviderChanged (object sender, System.EventArgs e)
		{
			if (!isDefaultSettings) {
				ConnectionSettings defaultSettings = DbFactoryService.GetDbFactory (settings).GetDefaultConnectionSettings ();
				SetRequiredFields (defaultSettings);
				return;
			}
			
			TreeIter iter;
			if (comboProvider.GetActiveIter (out iter)) {
				IDbFactory fac = providerStore.GetValue (iter, 1) as IDbFactory;
				settings = fac.GetDefaultConnectionSettings ();
				SetRequiredFields (settings);
				ShowConnectionSettings (settings, false);
			}
		}
		
		private void SetRequiredFields (ConnectionSettings settings)
		{
			if (settings == null) {
				entryPassword.Sensitive = entryServer.Sensitive = entryUsername.Sensitive
					= comboDatabase.Sensitive = spinPort.Sensitive = true;
			} else {
				entryPassword.Sensitive = settings.Password != null;
				entryServer.Sensitive = settings.Server != null;
				entryUsername.Sensitive = settings.Username != null;
				comboDatabase.Sensitive = settings.Database != null;
				spinPort.Sensitive = settings.Port != -1;
			}
		}
		
		private void ShowConnectionSettings (ConnectionSettings settings, bool updateCombo)
		{
			radioGenerated.Active = !settings.UseConnectionString;
			entryName.Text = String.IsNullOrEmpty (settings.Name) ? String.Empty : settings.Name;
			entryPassword.Text = String.IsNullOrEmpty (settings.Password) ? String.Empty : settings.Password;
			spinPort.Value = settings.Port != -1 ? settings.Port : spinPort.Value;
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

		protected virtual void CancelClicked (object sender, System.EventArgs e)
		{
			this.Respond (ResponseType.Cancel);
			this.Destroy ();
		}

		protected virtual void OkClicked (object sender, System.EventArgs e)
		{
			//TODO: only set sensitive to OK when all data is correct
			
			this.Respond (ResponseType.Ok);
			this.Destroy ();
		}

		protected virtual void NameChanged (object sender, System.EventArgs e)
		{
			isDefaultSettings = false;
			CheckConnectionSettings ();
		}

		protected virtual void ServerChanged (object sender, System.EventArgs e)
		{
			isDefaultSettings = false;
			CheckConnectionSettings ();
		}

		protected virtual void PortChanged (object sender, System.EventArgs e)
		{
			isDefaultSettings = false;
			CheckConnectionSettings ();
		}

		protected virtual void DatabaseChanged (object sender, System.EventArgs e)
		{
			isDefaultSettings = false;
			CheckConnectionSettings ();
		}

		protected virtual void UsernameChanged (object sender, System.EventArgs e)
		{
			isDefaultSettings = false;
			CheckConnectionSettings ();
		}

		protected virtual void PasswordChanged (object sender, System.EventArgs e)
		{
			isDefaultSettings = false;
			CheckConnectionSettings ();
		}
		
		private void CheckConnectionSettings ()
		{
			TreeIter iter;
			if (comboProvider.GetActiveIter (out iter)) {
				if (radioGenerated.Active) {
					buttonOk.Sensitive = (!entryName.Sensitive || entryName.Text.Length > 0)
						&& (!entryPassword.Sensitive || entryPassword.Text.Length > 0)
						&& (!entryServer.Sensitive || entryServer.Text.Length > 0)
						&& (!entryUsername.Sensitive || entryUsername.Text.Length > 0);
				} else {
					buttonOk.Sensitive = textConnectionString.Buffer.Text.Length > 0;
				}
			}
			buttonOk.Sensitive = false;
		}
	}
}
