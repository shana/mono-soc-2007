//
// Authors:
//   Christian Hergert <chris@mosaix.net>
//   Ben Motmans  <ben.motmans@gmail.com>
//
// Copyright (c) 2005 Christian Hergert
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

using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using Mono.Data.Sql;
using Mono.Addins;
using MonoDevelop.Core;

namespace MonoDevelop.Database.ConnectionManager
{
	public class ConnectionManagerService : AbstractService
	{
		public event ConnectionSettingsEventHandler ConnectionAdded;
		public event ConnectionSettingsEventHandler ConnectionRemoved;
		public event ConnectionSettingsEventHandler ConnectionRefreshed;
		
		private ConnectionSettingsCollection connections;
		private static string configFile = Path.Combine (Path.Combine (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), ".config"), "MonoDevelop"), "MonoDevelop.Database.ConnectionManager.xml");
		
		public IEnumerable<ConnectionSettings> Connections {
			get { return connections; }
		}
		
		public void AddConnection (ConnectionSettings settings)
		{
			if (!connections.Contains (settings)) {
				connections.Add (settings);
				OnConnectionAdded (new ConnectionSettingsEventArgs (settings));
			}
		}
		
		public void RemoveConnection (ConnectionSettings settings)
		{
			connections.Remove (settings);
			OnConnectionRemoved (new ConnectionSettingsEventArgs (settings));
		}

		public override void InitializeService ()
		{
			base.InitializeService ();
			if (File.Exists (configFile)) {
				try {
					using (FileStream fs = File.OpenRead (configFile)) {
						XmlSerializer serializer = new XmlSerializer (typeof (ConnectionSettingsCollection));
						connections = (ConnectionSettingsCollection) serializer.Deserialize (fs);
					}
					return;
				} catch {
					Runtime.LoggingService.Error (GettextCatalog.GetString ("Unable to load stored SQL connection information."));
					Runtime.FileService.DeleteFile (configFile);
				}
			}
			
			connections = new ConnectionSettingsCollection ();
		}

		public override void UnloadService ()
		{
			using (FileStream fs = new FileStream(configFile, FileMode.Create)) {
				XmlSerializer serializer = new XmlSerializer (typeof (ConnectionSettingsCollection));
				serializer.Serialize (fs, connections);
			}
			
			base.UnloadService ();
		}
		
		protected virtual void OnConnectionAdded (ConnectionSettingsEventArgs args)
		{
			if (ConnectionAdded != null)
				ConnectionAdded (this, args);
		}
		
		protected virtual void OnConnectionRemoved (ConnectionSettingsEventArgs args)
		{
			if (ConnectionAdded != null)
				ConnectionAdded (this, args);
		}
		
		protected virtual void OnConnectionRefreshed (ConnectionSettingsEventArgs args)
		{
			if (ConnectionAdded != null)
				ConnectionAdded (this, args);
		}
	}
}