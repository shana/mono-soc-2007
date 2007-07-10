//
// Authors:
//   Christian Hergert <chris@mosaix.net>
//   Daniel Morgan <danielmorgan@verizon.net>
//   Ben Motmans  <ben.motmans@gmail.com>
//
// Copyright (C) 2005 Christian Hergert
// Copyright (C) 2005 Daniel Morgan
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
using GtkSourceView;
using System;
using System.Data;
using System.Threading;
using System.Collections.Generic;
using Mono.Addins;
using Mono.Data.Sql;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Database.Components;

namespace MonoDevelop.Database.Query
{
	public class SqlQueryView : AbstractViewContent
	{
		private VBox vbox;
		private SourceView sourceView;
		private ToolButton buttonExecute;
		private ToolButton buttonStop;
		private Menu menuConnections;
		
		private object currentQueryState;
		private List<object> stoppedQueries;
		
		private ConnectionSettings selectedConnection;
		
		public SqlQueryView ()
		{
			stoppedQueries = new List<object> ();
			
			vbox = new VBox (false, 6);
			vbox.BorderWidth = 6;
			
			SourceLanguagesManager lm = new SourceLanguagesManager ();
			SourceLanguage lang = lm.GetLanguageFromMimeType ("text/x-sql");
			SourceBuffer buf = new SourceBuffer (lang);
			buf.Highlight = true;
			sourceView = new SourceView (buf);
			sourceView.ShowLineNumbers = true;
			
			Toolbar toolbar = new Toolbar ();
			toolbar.ToolbarStyle = ToolbarStyle.BothHoriz;
			
			buttonExecute = new ToolButton (
				Services.Resources.GetImage ("md-db-execute", IconSize.SmallToolbar), GettextCatalog.GetString ("Execute")
			);
			buttonStop = new ToolButton ("gtk-stop");
			buttonStop.Sensitive = false;
			
			buttonExecute.Clicked += new EventHandler (ExecuteClicked);
			buttonStop.Clicked += new EventHandler (StopClicked);
			
			MenuToolButton menuConnectionsButton = new MenuToolButton (Services.Resources.GetImage ("md-db-database", IconSize.SmallToolbar), GettextCatalog.GetString ("Select Connection"));
			menuConnections = new Menu ();
			menuConnectionsButton.Menu = menuConnections;
			GLib.SList group = null;
			foreach (ConnectionSettings settings in ConnectionSettingsService.Connections) {
				ConnectionSettingsMenuItem item = new ConnectionSettingsMenuItem (settings);
				if (group == null)
					group = item.Group;
				else
					item.Group = group;
				menuConnections.Append (item);
			}
			ConnectionSettings = null;

			buttonExecute.IsImportant = true;
			menuConnectionsButton.IsImportant = true;
			
			toolbar.Add (buttonExecute);
			toolbar.Add (buttonStop);
			toolbar.Add (new SeparatorToolItem ());
			toolbar.Add (menuConnectionsButton);
			
			vbox.PackStart (toolbar, false, true, 0);
			vbox.PackEnd (sourceView, true, true, 0);
			
			sourceView.GrabFocus ();
			
			ConnectionSettingsService.ConnectionAdded += (ConnectionSettingsEventHandler)Services.DispatchService.GuiDispatch (new ConnectionSettingsEventHandler (OnConnectionAdded));
			ConnectionSettingsService.ConnectionRemoved += (ConnectionSettingsEventHandler)Services.DispatchService.GuiDispatch (new ConnectionSettingsEventHandler (OnConnectionRemoved));
			ConnectionSettingsService.ConnectionEdited += (ConnectionSettingsEventHandler)Services.DispatchService.GuiDispatch (new ConnectionSettingsEventHandler (OnConnectionEdited));
			
			vbox.ShowAll ();
		}
		
		public string Text {
			get { return sourceView.Buffer.Text; }
			set { sourceView.Buffer.Text = value; }
		}
		
		public ConnectionSettings SelectedConnectionSettings {
			get { return selectedConnection; }
			set {
				selectedConnection = value;
				ConnectionSettingsMenuItem first = null;
				foreach (ConnectionSettingsMenuItem item in menuConnections.Children) {
					if (first == null) first = item;
					if (item.ConnectionSettings == value) {
						item.Active = true;
						return;
					}
				}
				if (first != null) {
					selectedConnection = first.ConnectionSettings;
					first.Active = true;
				}
			}
		}
		
		public override string UntitledName {
			get { return "SQL Query"; }
		}
		
		public override void Dispose ()
		{
			Control.Dispose ();
		}
		
		public override void Load (string filename)
		{
			throw new NotImplementedException ();
		}
		
		public override Widget Control {
			get { return vbox; }
		}
		
		public void ExecuteQuery ()
		{
			string sql = Text;
			if (sql.Length > 0) {
				
				IConnectionProvider provider = ConnectionSettings.ConnectionProvider;
				string error = null;
				if (!provider.IsOpen && provider.Open (out error)) {
					Services.MessageService.ShowError (error);
					IdeApp.Workbench.StatusBar.ShowErrorMessage (error);
					return;
				}
				
				currentQueryState = new object ();
				provider.ExecuteQueryAsDataSetAsync (sql, new SqlResultCallback<DataSet> (ExecuteQueryThreaded), currentQueryState);
			}
		}
		
		private void ExecuteQueryThreaded (object sender, DataSet result, object state)
		{
			if (stoppedQueries.Contains (state)) {
				stoppedQueries.Remove (state);
				return;
			}
			
			if (result != null) {
				foreach (DataTable table in result.Tables) {
					Services.DispatchService.GuiDispatch (delegate () {
						QueryResultView view = new QueryResultView (table);
						IdeApp.Workbench.OpenDocument (view, true);
					});
				}
			}
			
			Services.DispatchService.GuiDispatch (delegate () {
				buttonExecute.Sensitive = true;
				buttonStop.Sensitive = false;
			});
		}
		
		public void StopExecuteQuery ()
		{
			//since we can't abort a threadpool task, each task is assigned a unique state
			//when stop is pressed, the state is added to the list of results that need
			//to be discarded when they get in
			if (!stoppedQueries.Contains (currentQueryState))
				stoppedQueries.Add (currentQueryState);
						
			buttonExecute.Sensitive = true;
			buttonStop.Sensitive = false;
		}
		
		private void ExecuteClicked (object sender, EventArgs e)
		{
			buttonExecute.Sensitive = false;
			ExecuteQuery ();
		}
		
		private void StopClicked (object sender, EventArgs e)
		{
			buttonStop.Sensitive = false;
			StopExecuteQuery ();
		}
		
		private void OnConnectionAdded (object sender, ConnectionSettingsEventArgs args)
		{
			ConnectionSettingsMenuItem item = new ConnectionSettingsMenuItem (settings);
			menuConnections.Append (item);
		}
		
		private void OnConnectionRemoved (object sender, ConnectionSettingsEventArgs args)
		{
			if (selectedConnection == args.ConnectionSettings)
				SelectedConnectionSettings = null;
			
			ConnectionSettingsMenuItem removeItem = null;
			foreach (ConnectionSettingsMenuItem item in menuConnections.Children) {
				if (item.ConnectionSettings == args.ConnectionSettings) {
					removeItem = item;
					break;
				}
			}
			if (removeItem != null)
				menuConnections.Remove (removeItem);
		}
		
		private void OnConnectionEdited (object sender, ConnectionSettingsEventArgs args)
		{
			foreach (ConnectionSettingsMenuItem item in menuConnections.Children) {
				if (item.ConnectionSettings == args.ConnectionSettings) {
					item.Update ();
					return;
				}
			}
		}
	}
	
	internal class ConnectionSettingsMenuItem : RadioMenuItem
	{
		private ConnectionSettings settings;
		
		public ConnectionSettingsMenuItem (ConnectionSettings settings)
			: base (settings.Name)
		{
		}
		
		public ConnectionSettings ConnectionSettings {
			get { return settings; }
		}
		
		public void Update ()
		{
			(Child as Label).Text = settings.Name;
		}
	}
}