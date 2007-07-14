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

namespace MonoDevelop.Database.Components
{
	public partial class ConnectionDialog : Gtk.Dialog
	{
		private ConnectionSettings settings;
		private bool isDefaultSettings;

		private ListStore store;
		
		public ConnectionDialog ()
			: this (true)
		{
		}
		
		public ConnectionDialog (bool showDefault)
		{
			this.Build ();
			textConnectionString.Buffer.Changed += new EventHandler (ConnectionStringChanged);
			this.settings = new ConnectionSettings ();

			store = new ListStore (typeof (object), typeof (string));
			comboProvider.Model = store;
			CellRendererText nameRenderer = new CellRendererText ();
			comboProvider.PackStart (nameRenderer, true);
			comboProvider.AddAttribute (nameRenderer, "text", 1);

			foreach (IDbFactory fac in DbFactoryService.DbFactories)
				store.AppendValues (fac, fac.Name);

			isDefaultSettings = true;
			if (showDefault) {
				TreeIter iter;
				if (store.GetIterFirst (out iter)) {
					comboProvider.SetActiveIter (iter);
					SetRequiredFieldsFromProvider ();
				}
			}
			alignmentCustom.Sensitive = false;
		}
		
		public ConnectionDialog (ConnectionSettings settings)
			: this (false)
		{
			if (settings == null)
				throw new ArgumentNullException ("settings");
			
			this.settings = settings;
			isDefaultSettings = false;
			SetRequiredFields (settings);
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
			SetRequiredFieldsFromProvider ();
		}
		
		private void SetRequiredFieldsFromProvider ()
		{
			Runtime.LoggingService.Debug ("SetRequiredFieldsFromProvider-- 1");
			
			TreeIter iter;
			ConnectionSettings defaultSettings = null;
			if (comboProvider.GetActiveIter (out iter)) {
				IDbFactory fac = store.GetValue (iter, 0) as IDbFactory;
				defaultSettings = fac.GetDefaultConnectionSettings ();
				SetRequiredFields (defaultSettings);
			}
			
			Runtime.LoggingService.Debug ("isDefaultSettings = " + isDefaultSettings);
			
			if (isDefaultSettings)
				ShowConnectionSettings (defaultSettings, false);
			else
				ShowConnectionSettings (settings, false);
			Runtime.LoggingService.Debug ("SetRequiredFieldsFromProvider-- 2");
		}
		
		private void SetRequiredFields (ConnectionSettings settings)
		{
			if (settings == null) {
				entryPassword.Sensitive = entryServer.Sensitive = entryUsername.Sensitive
					= entryDatabase.Sensitive = spinPort.Sensitive = true;
			} else {
				entryPassword.Sensitive = settings.Password != null;
				entryServer.Sensitive = settings.Server != null;
				entryUsername.Sensitive = settings.Username != null;
				entryDatabase.Sensitive = settings.Database != null;
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
			entryDatabase.Text = String.IsNullOrEmpty (settings.Database) ? String.Empty : settings.Database;
			spinMinPool.Value = settings.MinPoolSize;
			spinMaxPool.Value = settings.MaxPoolSize;
			
			if (updateCombo) {
				TreeIter iter;
				if (store.GetIterFirst (out iter)) {
					do {
						IDbFactory fac = store.GetValue (iter, 0) as IDbFactory;
						if (settings.ProviderIdentifier == fac.Identifier) {
							comboProvider.SetActiveIter (iter);
							return;
						}
					} while (store.IterNext (ref iter));
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
			TreeIter iter;
			if (comboProvider.GetActiveIter (out iter))
				settings.ProviderIdentifier = (store.GetValue (iter, 0) as IDbFactory).Identifier;
			settings.Name = entryName.Text;
			settings.Server = entryServer.Text;
			settings.Port = (int)spinPort.Value;
			settings.Database = entryDatabase.Text;
			settings.Username = entryUsername.Text;
			settings.Password = entryPassword.Text;
			settings.SavePassword = checkSavePassword.Active;
			settings.MaxPoolSize = (int)spinMaxPool.Value;
			settings.MinPoolSize = (int)spinMinPool.Value;
			settings.UseConnectionString = radioUseCustom.Active;
			settings.ConnectionString = textConnectionString.Buffer.Text;
			
			this.Respond (ResponseType.Ok);
			this.Destroy ();
		}

		protected virtual void NameChanged (object sender, EventArgs e)
		{
			isDefaultSettings = false;
			CheckConnectionSettings ();
		}

		protected virtual void ServerChanged (object sender, EventArgs e)
		{
			isDefaultSettings = false;
			CheckConnectionSettings ();
		}

		protected virtual void PortChanged (object sender, EventArgs e)
		{
			isDefaultSettings = false;
			CheckConnectionSettings ();
		}

		protected virtual void DatabaseChanged (object sender, EventArgs e)
		{
			isDefaultSettings = false;
			CheckConnectionSettings ();
		}

		protected virtual void UsernameChanged (object sender, EventArgs e)
		{
			isDefaultSettings = false;
			CheckConnectionSettings ();
		}

		protected virtual void PasswordChanged (object sender, EventArgs e)
		{
			isDefaultSettings = false;
			CheckConnectionSettings ();
		}
		
		protected virtual void ConnectionStringChanged (object sender, EventArgs e)
		{
			isDefaultSettings = false;
			CheckConnectionSettings ();
		}

		private void CheckConnectionSettings ()
		{
			Runtime.LoggingService.Debug ("CheckConnectionSettings 1");
			TreeIter iter;
			if (comboProvider.GetActiveIter (out iter)) {
				if (radioGenerated.Active) {
					buttonOk.Sensitive = (!entryName.Sensitive || entryName.Text.Length > 0)
						&& (!entryPassword.Sensitive || entryPassword.Text.Length > 0)
						&& (!entryServer.Sensitive || entryServer.Text.Length > 0)
						&& (!entryUsername.Sensitive || entryUsername.Text.Length > 0)
						&& (!entryDatabase.Sensitive || entryDatabase.Text.Length > 0);
				} else {
					buttonOk.Sensitive = textConnectionString.Buffer.Text.Length > 0;
				}
			} else {
				buttonOk.Sensitive = false;
			}
			Runtime.LoggingService.Debug ("CheckConnectionSettings 2");
		}
	}
}
