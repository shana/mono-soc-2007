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
using System.Threading;
using System.Collections.Generic;
using Mono.Addins;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Database.Sql;

namespace MonoDevelop.Database.Components
{
	public partial class DatabaseConnectionSettingsDialog : Gtk.Dialog
	{
		protected bool isEditMode;
		protected DatabaseConnectionSettings settings;
		
		protected bool isDefaultSettings;
		
		protected ListStore storeProviders;
		protected ListStore storeDatabases;
		protected bool isDatabaseListEmpty;
		
		protected bool enableServerEntry;
		protected bool enablePortEntry;
		protected bool enableUsernameEntry;
		protected bool enablePasswordEntry;
		protected bool enableOpenButton;
		protected bool enableRefreshButton;
		
		//TODO: add warning when no providers are available
		protected DatabaseConnectionSettingsDialog (bool isEditMode)
		{
			this.Build ();
			
			if (isEditMode)
				Title = GettextCatalog.GetString ("Edit Database Connection");
			else
				Title = GettextCatalog.GetString ("Add Database Connection");
			
			textConnectionString.Buffer.Changed += new EventHandler (ConnectionStringChanged);
			checkCustom.Toggled += new EventHandler (CustomConnectionStringActivated);
			
			storeProviders = new ListStore (typeof (string), typeof (object));
			comboProvider.Model = storeProviders;
			
			storeDatabases = new ListStore (typeof (string));
			comboDatabase.Model = storeDatabases;
			comboDatabase.TextColumn = 0;
			comboDatabase.Entry.Changed += new EventHandler (DatabaseChanged);

			CellRendererText providerRenderer = new CellRendererText ();
			comboProvider.PackStart (providerRenderer, true);
			comboProvider.AddAttribute (providerRenderer, "text", 0);
			
//			CellRendererText databaseRenderer = new CellRendererText ();
//			comboDatabase.PackStart (databaseRenderer, true);
//			comboDatabase.AddAttribute (databaseRenderer, "markup", 0);

			foreach (IDbFactory fac in DbFactoryService.DbFactories)
				storeProviders.AppendValues (fac.Name, fac);
		}

		public DatabaseConnectionSettingsDialog ()
			: this (false)
		{
			settings = new DatabaseConnectionSettings ();
			isDefaultSettings = true;
			
			TreeIter iter;
			if (storeProviders.GetIterFirst (out iter))
				comboProvider.SetActiveIter (iter);
			
			storeDatabases.AppendValues (GettextCatalog.GetString ("No databases found!"));
			isDatabaseListEmpty = true;
		}
		
		public DatabaseConnectionSettingsDialog (DatabaseConnectionSettings settings)
			: this (true)
		{
			if (settings == null)
				throw new ArgumentNullException ("settings");
			
			this.settings = settings;
			ShowSettings (settings, true);
			
			storeDatabases.AppendValues (settings.Database);
			isDatabaseListEmpty = false;
		}
		
		public DatabaseConnectionSettings ConnectionSettings {
			get { return settings; }
		}
		
		public bool EnableServerEntry {
			get { return enableServerEntry; }
			set { enableServerEntry = value; }
		}
		
		public bool EnablePortEntry {
			get { return enablePortEntry; }
			set { enablePortEntry = value; }
		}
		
		public bool EnableUsernameEntry {
			get { return enableUsernameEntry; }
			set { enableUsernameEntry = value; }
		}
		
		public bool EnablePasswordEntry {
			get { return enablePasswordEntry; }
			set { enablePasswordEntry = value; }
		}
		
		public bool EnableOpenButton {
			get { return enableOpenButton; }
			set { enableOpenButton = value; }
		}
		
		public bool EnableRefreshButton {
			get { return enableRefreshButton; }
			set { enableRefreshButton = value; }
		}

		protected virtual void NameChanged (object sender, System.EventArgs e)
		{
			CheckSettings ();
		}

		protected virtual void ServerChanged (object sender, System.EventArgs e)
		{
			CheckSettings ();
		}

		protected virtual void PortChanged (object sender, System.EventArgs e)
		{
			CheckSettings ();
		}

		protected virtual void UsernameChanged (object sender, System.EventArgs e)
		{
			CheckSettings ();
		}

		protected virtual void OnOkClicked (object sender, System.EventArgs e)
		{
			FillDatabaseConnectionSettings (settings);

			Respond (ResponseType.Ok);
			Hide ();
		}

		protected virtual void OnCancelClicked (object sender, System.EventArgs e)
		{
			Respond (ResponseType.Cancel);
			Hide ();
		}

		protected virtual void PasswordChanged (object sender, System.EventArgs e)
		{
			CheckSettings ();
		}

		protected virtual void RefreshClicked (object sender, System.EventArgs e)
		{
			DatabaseConnectionSettings settingsCopy = CreateDatabaseConnectionSettings ();
			storeDatabases.Clear ();
			
			ThreadPool.QueueUserWorkItem (new WaitCallback (RefreshClickedThreaded), settingsCopy);
		}
		
		protected virtual void RefreshClickedThreaded (object state)
		{
			DatabaseConnectionSettings settings = state as DatabaseConnectionSettings;
			DatabaseConnectionContext context = new DatabaseConnectionContext (settings);
			IDbFactory fac = DbFactoryService.GetDbFactory (settings.ProviderIdentifier);
			try {
				FakeConnectionPool pool = new FakeConnectionPool (fac, fac.ConnectionProvider, context);
				pool.Initialize ();
				
				ISchemaProvider prov = fac.CreateSchemaProvider (pool);
				DatabaseSchemaCollection databases = prov.GetDatabases ();
				
				foreach (DatabaseSchema db in databases) {
					Services.DispatchService.GuiDispatch (delegate () {
						storeDatabases.AppendValues (db.Name);
					});
				}
				isDatabaseListEmpty = databases.Count == 0;
			} catch {}

			if (isDatabaseListEmpty) {
				Services.DispatchService.GuiDispatch (delegate () {
					storeDatabases.AppendValues (GettextCatalog.GetString ("No databases found!"));
				});
			} else {
				Services.DispatchService.GuiDispatch (delegate () {
					TreeIter iter;
					if (storeDatabases.GetIterFirst (out iter))
						comboDatabase.SetActiveIter (iter);
				});
			}
		}

		protected virtual void OpenClicked (object sender, System.EventArgs e)
		{
			DatabaseConnectionSettings settingsCopy = CreateDatabaseConnectionSettings ();
			IDbFactory fac = DbFactoryService.GetDbFactory (settingsCopy.ProviderIdentifier);
			
			string database = null;
			if (fac.ShowOpenDatabaseDialog (out database)) {
				if (isDatabaseListEmpty)
					storeDatabases.Clear (); //clear the fake node
				isDatabaseListEmpty = false;
				
				TreeIter iter = storeDatabases.AppendValues (database);
				comboDatabase.SetActiveIter (iter);	
			}
		}

		protected virtual void MinPoolSizeChanged (object sender, System.EventArgs e)
		{
			if (spinMinPoolSize.Value > spinMaxPoolSize.Value)
				spinMaxPoolSize.Value = spinMinPoolSize.Value;
		}

		protected virtual void MaxPoolSizeChanged (object sender, System.EventArgs e)
		{
			if (spinMaxPoolSize.Value < spinMinPoolSize.Value)
				spinMinPoolSize.Value = spinMaxPoolSize.Value;
		}

		protected virtual void CustomConnectionStringActivated (object sender, System.EventArgs e)
		{
			bool sens = !checkCustom.Active;
			
			entryPassword.Sensitive = sens && enablePasswordEntry;
			entryUsername.Sensitive = sens && enableUsernameEntry;
			entryServer.Sensitive = sens && enableServerEntry;
			spinPort.Sensitive = sens && enablePortEntry;
			comboDatabase.Sensitive = sens;
			buttonOpen.Sensitive = sens && enableOpenButton;
			buttonRefresh.Sensitive = sens && enableRefreshButton;
			scrolledwindow.Sensitive = !sens;
		}
		
		protected virtual void ConnectionStringChanged (object sender, EventArgs e)
		{
			CheckSettings ();
		}
		
		protected virtual void DatabaseChanged (object sender, EventArgs e)
		{
			if (isDatabaseListEmpty && comboDatabase.Entry.Text.Length > 0)
				comboDatabase.Entry.Text = String.Empty;
			
			CheckSettings ();
		}

		protected virtual void CheckSettings ()
		{
			isDefaultSettings = false;
			buttonOk.Sensitive = ValidateFields (); 
		}
		
		protected virtual bool ValidateFields ()
		{
			bool ok = false;
			if (checkCustom.Active) {
				ok = textConnectionString.Buffer.Text.Length > 0;
			} else {
				TreeIter iter;
				ok = entryName.Text.Length > 0
					&& (entryServer.Text.Length > 0 || !enableServerEntry)
					&& (entryUsername.Text.Length > 0 || !enableUsernameEntry)
					&& (entryPassword.Text.Length > 0 || !enablePasswordEntry)
					&& (comboDatabase.Entry.Text.Length > 0)
					&& comboProvider.GetActiveIter (out iter);
			}
			return ok;
		}
		
		protected virtual DatabaseConnectionSettings CreateDatabaseConnectionSettings ()
		{
			DatabaseConnectionSettings settings = new DatabaseConnectionSettings ();
			FillDatabaseConnectionSettings (settings);
			return settings;
		}
		
		protected virtual void FillDatabaseConnectionSettings (DatabaseConnectionSettings settings)
		{
			settings.ConnectionString = textConnectionString.Buffer.Text;
			settings.UseConnectionString = checkCustom.Active;
			settings.MinPoolSize = (int)spinMinPoolSize.Value;
			settings.MaxPoolSize = (int)spinMaxPoolSize.Value;
			settings.Name = entryName.Text;
			settings.Username = entryUsername.Text;
			settings.Password = entryPassword.Text;
			settings.Server = entryServer.Text;
			settings.Port = (int)spinPort.Value;
			settings.Database = comboDatabase.Entry.Text;
			settings.SavePassword = checkSavePassword.Active;
			
			TreeIter iter;
			if (comboProvider.GetActiveIter (out iter)) {
				IDbFactory fac = storeProviders.GetValue (iter, 1) as IDbFactory;
				settings.ProviderIdentifier = fac.Identifier;
			}
		}
		
		protected virtual void ShowSettings (DatabaseConnectionSettings settings, bool updateProviderCombo)
		{
			checkCustom.Active = settings.UseConnectionString;
			entryName.Text = String.IsNullOrEmpty (settings.Name) ? String.Empty : settings.Name;
			entryPassword.Text = String.IsNullOrEmpty (settings.Password) ? String.Empty : settings.Password;
			spinPort.Value = settings.Port > 0 ? settings.Port : spinPort.Value;
			entryServer.Text = String.IsNullOrEmpty (settings.Server) ? String.Empty : settings.Server;
			entryUsername.Text = String.IsNullOrEmpty (settings.Username) ? String.Empty : settings.Username;
			textConnectionString.Buffer.Text = String.IsNullOrEmpty (settings.ConnectionString) ? String.Empty : settings.ConnectionString;
			comboDatabase.Entry.Text = String.IsNullOrEmpty (settings.Database) ? String.Empty : settings.Database;
			spinMinPoolSize.Value = settings.MinPoolSize;
			spinMaxPoolSize.Value = settings.MaxPoolSize;
			
			if (updateProviderCombo) {
				TreeIter iter;
				if (storeProviders.GetIterFirst (out iter)) {
					do {
						IDbFactory fac = storeProviders.GetValue (iter, 1) as IDbFactory;
						if (settings.ProviderIdentifier == fac.Identifier) {
							comboProvider.SetActiveIter (iter);
							return;
						}
					} while (storeProviders.IterNext (ref iter));
				}
			}
		}
		
		protected virtual void ProviderChanged (object sender, System.EventArgs e)
		{
			TreeIter iter;
			if (comboProvider.GetActiveIter (out iter)) {
				IDbFactory fac = storeProviders.GetValue (iter, 1) as IDbFactory;
	
				bool prevDefaultSettings = isDefaultSettings;
				if (isDefaultSettings) {
					DatabaseConnectionSettings defaultSettings = fac.GetDefaultConnectionSettings ();
					ShowSettings (defaultSettings, false);
				}
				
				SetWidgetStates (fac);
				CheckSettings ();
				isDefaultSettings = prevDefaultSettings;
			}
		}
		
		protected virtual bool GetBoolOption (IDbFactory fac, string option, bool defaultValue)
		{
			object val = fac.GetOption (option);
			if (val == null)
				return defaultValue;
			else
				return (bool)val;
		}
		
		protected virtual void SetWidgetStates (IDbFactory fac)
		{
			enableServerEntry = GetBoolOption (fac, "settings.requires.server", false);
			enablePortEntry = GetBoolOption (fac, "settings.requires.port", false);
			enableUsernameEntry = GetBoolOption (fac, "settings.requires.username", false);
			enablePasswordEntry = GetBoolOption (fac, "settings.requires.password", false);
			enableOpenButton = GetBoolOption (fac, "settings.can_open_database", false);
			enableRefreshButton = GetBoolOption (fac, "settings.can_list_databases", false);
			
			entryServer.Sensitive = enableServerEntry;
			spinPort.Sensitive = enablePortEntry;
			entryUsername.Sensitive = enableUsernameEntry;
			entryPassword.Sensitive = enablePasswordEntry;
			buttonOpen.Sensitive = enableOpenButton;
			buttonRefresh.Sensitive = enableRefreshButton;
		}
	}
}
