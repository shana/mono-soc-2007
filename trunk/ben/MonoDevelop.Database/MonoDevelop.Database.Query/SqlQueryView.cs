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
using System;
using System.Data;
using System.Threading;
using System.Collections.Generic;
using Mono.Addins;
using MonoDevelop.Database.Sql;
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
		private SqlEditorWidget sqlEditor;
		private ToolButton buttonExecute;
		private ToolButton buttonStop;
		private ToolButton buttonClear;
		private Menu menuConnections;
		private Notebook notebook;
		private VPaned pane;
		private GLib.SList group;
		private EventHandler connectionMenuActivatedHandler;
		private TextView status;
		
		private object currentQueryState;
		private List<object> stoppedQueries;
		
		private DatabaseConnectionContext selectedConnection;
		
		public SqlQueryView ()
		{
			stoppedQueries = new List<object> ();
			
			vbox = new VBox (false, 6);
			vbox.BorderWidth = 6;

			sqlEditor = new SqlEditorWidget ();
			sqlEditor.TextChanged += new EventHandler (SqlChanged);
			
			Toolbar toolbar = new Toolbar ();
			toolbar.ToolbarStyle = ToolbarStyle.BothHoriz;
			
			buttonExecute = new ToolButton (
				Services.Resources.GetImage ("md-db-execute", IconSize.SmallToolbar),
				GettextCatalog.GetString ("Execute")
			);
			buttonStop = new ToolButton ("gtk-stop");
			buttonClear = new ToolButton (Services.Resources.GetImage ("gtk-clear", IconSize.Button), GettextCatalog.GetString ("Clear Results"));
			buttonStop.Sensitive = false;
			buttonExecute.Sensitive = false;
			
			buttonExecute.Clicked += new EventHandler (ExecuteClicked);
			buttonStop.Clicked += new EventHandler (StopClicked);
			buttonClear.Clicked += new EventHandler (ClearClicked);
			
			MenuToolButton menuConnectionsButton = new MenuToolButton (
				Services.Resources.GetImage ("md-db-database", IconSize.SmallToolbar),
				GettextCatalog.GetString ("Select Connection")
			);
			menuConnections = new Menu ();
			menuConnectionsButton.Menu = menuConnections;

			connectionMenuActivatedHandler = new EventHandler (ConnectionMenuActivated);
			foreach (DatabaseConnectionContext context in ConnectionContextService.DatabaseConnections)
				AddConnectionSettingsMenu (context);
			SelectFirstConnectionSettings ();

			buttonExecute.IsImportant = true;
			menuConnectionsButton.IsImportant = true;
			
			toolbar.Add (buttonExecute);
			toolbar.Add (buttonStop);
			toolbar.Add (buttonClear);
			toolbar.Add (new SeparatorToolItem ());
			toolbar.Add (menuConnectionsButton);
			
			pane = new VPaned ();

			ScrolledWindow windowStatus = new ScrolledWindow ();
			status = new TextView ();
			windowStatus.Add (status);
			
			notebook = new Notebook ();
			notebook.AppendPage (windowStatus, new Label (GettextCatalog.GetString ("Status")));
			
			pane.Pack1 (sqlEditor, true, true);
			pane.Pack2 (notebook, true, true);
			
			vbox.PackStart (toolbar, false, true, 0);
			vbox.PackStart (pane, true, true, 0);
			
			ConnectionContextService.ConnectionContextAdded += (DatabaseConnectionContextEventHandler)Services.DispatchService.GuiDispatch (new DatabaseConnectionContextEventHandler (OnConnectionAdded));
			ConnectionContextService.ConnectionContextRemoved += (DatabaseConnectionContextEventHandler)Services.DispatchService.GuiDispatch (new DatabaseConnectionContextEventHandler (OnConnectionRemoved));
			ConnectionContextService.ConnectionContextEdited += (DatabaseConnectionContextEventHandler)Services.DispatchService.GuiDispatch (new DatabaseConnectionContextEventHandler (OnConnectionEdited));

			vbox.ShowAll ();
			notebook.Hide ();
		}
		
		public string Text {
			get { return sqlEditor.Text; }
			set { sqlEditor.Text = value; }
		}
		
		public DatabaseConnectionContext SelectedConnectionContext {
			get { return selectedConnection; }
			set {
				selectedConnection = value;
				buttonExecute.Sensitive = value != null;
				foreach (ConnectionContextMenuItem item in menuConnections.Children) {
					if (item.ConnectionContext == value) {
						if (!item.Active)
							item.Active = true;
						return;
					}
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
			if (selectedConnection == null || Text.Length < 0) {
				SetQueryState (false, "No connection selected.");
				return;
			}
			
			QueryService.EnsureConnection (selectedConnection, new DatabaseConnectionContextCallback (ExecuteQueryCallback), null);
		}
		
		private void ExecuteQueryCallback (DatabaseConnectionContext context, bool connected, object state)
		{
			if (!connected) {
				Services.MessageService.ShowErrorFormatted (
					GettextCatalog.GetString ("Unable to connect to database '{0}'"), context.ConnectionSettings.Name);
				return;
			}
			
			currentQueryState = new object ();
			IPooledDbConnection conn = context.ConnectionPool.Request ();
			IDbCommand command = conn.CreateCommand (Text);
			conn.ExecuteSetAsync (command, new ExecuteCallback<DataSet> (ExecuteQueryThreaded), currentQueryState);
		}
		
		private void ExecuteQueryThreaded (IPooledDbConnection connection, DataSet result, object state)
		{
			connection.Release ();

			Services.DispatchService.GuiDispatch (delegate () {
				notebook.ShowAll ();
				string msg = GettextCatalog.GetPluralString ("Query executed ({0} result table)",
					"Query executed ({0} result tables)", result.Tables.Count);
				SetQueryState (false, String.Format (msg, result.Tables.Count));
			});
			
			if (stoppedQueries.Contains (state)) {
				stoppedQueries.Remove (state);
				return;
			}

			if (result != null) {
				foreach (DataTable table in result.Tables) {
					Services.DispatchService.GuiDispatch (delegate () {
						MonoDevelop.Database.Components.DataGrid grid = new MonoDevelop.Database.Components.DataGrid ();
						grid.DataSource = table;
						grid.DataBind ();
	
						string msg = String.Concat (Environment.NewLine, GettextCatalog.GetString ("Table"), ": ",table.TableName,
							Environment.NewLine, "\t", GettextCatalog.GetString ("Affected Rows"), ": ", table.Rows.Count);
						status.Buffer.Text += msg;
							
						TabLabel label = new TabLabel (new Label (table.TableName), Services.Resources.GetImage ("md-db-table", IconSize.Menu));
						label.CloseClicked += new EventHandler (OnResultTabClose);
						notebook.AppendPage (grid, label);
						notebook.ShowAll ();
					});
				}
			}
			
			if (result == null || result.Tables.Count == 0) {
				Services.DispatchService.GuiDispatch (delegate () {
					status.Buffer.Text += GettextCatalog.GetString ("No Results");
				});
			}
		}
			
		private void OnResultTabClose (object sender, EventArgs args)
		{
			Widget tabLabel = (Widget)sender;
			foreach (Widget child in notebook.Children) {
				if (notebook.GetTabLabel (child) == tabLabel) {
					notebook.Remove (child);
					break;
				}
			}
		}
		
		private void ExecuteClicked (object sender, EventArgs e)
		{
			SetQueryState (true, GettextCatalog.GetString ("Executing query"));
			ExecuteQuery ();
		}
		
		private void ClearClicked (object sender, EventArgs e)
		{
			for (int i=1; i<notebook.NPages; i++)
				notebook.RemovePage (i);
			status.Buffer.Text = String.Empty;
			notebook.Hide ();
		}
		
		private void StopClicked (object sender, EventArgs e)
		{
			SetQueryState (false, GettextCatalog.GetString ("Query execute cancelled"));
			
			//since we can't abort a threadpool task, each task is assigned a unique state
			//when stop is pressed, the state is added to the list of results that need
			//to be discarded when they get in
			if (!stoppedQueries.Contains (currentQueryState))
				stoppedQueries.Add (currentQueryState);
		}
		
		private void OnConnectionAdded (object sender, DatabaseConnectionContextEventArgs args)
		{
			AddConnectionSettingsMenu (args.ConnectionContext);
		}
		
		private void OnConnectionRemoved (object sender, DatabaseConnectionContextEventArgs args)
		{
			ConnectionContextMenuItem removeItem = null;
			foreach (ConnectionContextMenuItem item in menuConnections.Children) {
				if (item.ConnectionContext == args.ConnectionContext) {
					removeItem = item;
					break;
				}
			}
			if (removeItem != null) {
				removeItem.Activated -= connectionMenuActivatedHandler;
				menuConnections.Remove (removeItem);
			}
			
			SelectFirstConnectionSettings ();
		}
		
		private void OnConnectionEdited (object sender, DatabaseConnectionContextEventArgs args)
		{
			foreach (ConnectionContextMenuItem item in menuConnections.Children) {
				if (item.ConnectionContext == args.ConnectionContext) {
					item.Update ();
					return;
				}
			}
		}
		
		private void AddConnectionSettingsMenu (DatabaseConnectionContext context)
		{
			ConnectionContextMenuItem item = new ConnectionContextMenuItem (context);
			if (group == null)
				group = item.Group;
			else
				item.Group = group;
			item.Active = false;
			item.Activated += connectionMenuActivatedHandler;
			item.ShowAll ();
			menuConnections.Append (item);
		}
		
		private void ConnectionMenuActivated (object sender, EventArgs args)
		{
			ConnectionContextMenuItem item = sender as ConnectionContextMenuItem;
			selectedConnection = item.ConnectionContext;
			buttonExecute.Sensitive = true;
		}
		
		private void SqlChanged (object sender, EventArgs args)
		{
			buttonExecute.Sensitive = sqlEditor.Text.Length > 0;
		}
		
		private void SetQueryState (bool exec, string msg)
		{
			buttonExecute.Sensitive = !exec && sqlEditor.Text.Length > 0;
			buttonStop.Sensitive = exec;
			buttonClear.Sensitive = !exec;
			sqlEditor.Editable = !exec;
			
			status.Buffer.Text = msg + Environment.NewLine;
		}
		
		private void SelectFirstConnectionSettings ()
		{
			foreach (ConnectionContextMenuItem item in menuConnections.Children) {
				item.Active = true;
				selectedConnection = item.ConnectionContext;
				buttonExecute.Sensitive = true;
				return;
			}
		}
	}
	
	internal class ConnectionContextMenuItem : RadioMenuItem
	{
		private DatabaseConnectionContext context;
		
		public ConnectionContextMenuItem (DatabaseConnectionContext context)
			: base (context.ConnectionSettings.Name)
		{
			this.context = context;
		}
		
		public DatabaseConnectionContext ConnectionContext {
			get { return context; }
		}
		
		public void Update ()
		{
			(Child as Label).Text = context.ConnectionSettings.Name;
		}
	}
}