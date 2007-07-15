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
using Mono.Addins;

namespace MonoDevelop.Database.Sql
{
	public static class ConnectionSettingsService
	{
		public static event ConnectionSettingsEventHandler ConnectionAdded;
		public static event ConnectionSettingsEventHandler ConnectionRemoved;
		public static event ConnectionSettingsEventHandler ConnectionEdited;
		public static event ConnectionSettingsEventHandler ConnectionRefreshed;
		
		private static ConnectionSettingsCollection connections;
		private static string configFile = null;
		
		private static bool isInitialized;

		public static bool IsInitialized {
			get { return isInitialized; }
		}
		
		public static string ConfigFile {
			get { return configFile; }
			internal set {
				if (value == null)
					throw new ArgumentNullException ("ConfigFile");
				configFile = value;
			}
		}
		
		public static IEnumerable<ConnectionSettings> Connections {
			get { return connections; }
		}
		
		public static void AddConnection (ConnectionSettings settings)
		{
			if (!connections.Contains (settings)) {
				connections.Add (settings);
				Save ();
				if (ConnectionAdded != null)
					ConnectionAdded (null, new ConnectionSettingsEventArgs (settings));
			}
		}
		
		public static void RemoveConnection (ConnectionSettings settings)
		{
			connections.Remove (settings);
			Save ();
			if (ConnectionRemoved != null)
				ConnectionRemoved (null, new ConnectionSettingsEventArgs (settings));
		}
		
		public static void EditConnection (ConnectionSettings settings)
		{
			Save ();
			if (ConnectionEdited != null)
				ConnectionEdited (null, new ConnectionSettingsEventArgs (settings));
		}
		
		public static void Initialize (string configFile)
		{
			if (!isInitialized) {
				ConfigFile = configFile;

				if (File.Exists (configFile)) {
					try {
						using (FileStream fs = File.OpenRead (configFile)) {
							XmlSerializer serializer = new XmlSerializer (typeof (ConnectionSettingsCollection));
							connections = (ConnectionSettingsCollection) serializer.Deserialize (fs);
						}
						return;
					} catch {
						//TODO: log
						//Runtime.LoggingService.Error (GettextCatalog.GetString ("Unable to load stored SQL connection information."));
						File.Delete (configFile);
					}
				}
				
				connections = new ConnectionSettingsCollection ();
				isInitialized = true;
			}
		}

		public static void Save ()
		{
			//temporarily empty all passwords that don't need to be saved
			Dictionary<ConnectionSettings, string> tmp = new Dictionary<ConnectionSettings,string> ();
			foreach (ConnectionSettings settings in connections) {
				if (!settings.SavePassword) {
					tmp.Add (settings, settings.Password);
					settings.Password = null;
				}
			}
			
			using (FileStream fs = new FileStream (configFile, FileMode.Create)) {
				XmlSerializer serializer = new XmlSerializer (typeof (ConnectionSettingsCollection));
				serializer.Serialize (fs, connections);
			}
			
			foreach (KeyValuePair<ConnectionSettings, string> pair in tmp)
				pair.Key.Password = pair.Value;
		}
	}
}