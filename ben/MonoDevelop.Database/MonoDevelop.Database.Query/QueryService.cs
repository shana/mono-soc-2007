//
// Authors:
//   Christian Hergert <chris@mosaix.net>
//   Ben Motmans  <ben.motmans@gmail.com>
//
// Copyright (C) 2005 Christian Hergert
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
using Mono.Data.Sql;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Components;
using MonoDevelop.Database.Components;

namespace MonoDevelop.Database.Query
{
	public delegate void ConnectionSettingsCallback (ConnectionSettings settings, bool connected);

	public static class QueryService
	{
		//TODO: show errors
		public static void EnsureConnection (ConnectionSettings settings, ConnectionSettingsCallback callback)
		{
			if (settings == null)
				throw new ArgumentNullException ("settings");
			if (callback == null)
				throw new ArgumentNullException ("callback");
			
			IConnectionPool pool = settings.ConnectionPool;
			if (pool.IsInitialized) {
				callback (settings, true);
				return;
			}
			
			if (!settings.SavePassword && String.IsNullOrEmpty (settings.Password)) {
				PasswordDialog dlg = new PasswordDialog ();
				dlg.Username = settings.Username;
				
				if (dlg.Run () == (int)ResponseType.Ok) {
					settings.Password = dlg.Password;
				} else {
					callback (settings, false);
					return;
				}
			}
			
			EnsureConnectionState state = new EnsureConnectionState (settings, callback, 1);
			ThreadPool.QueueUserWorkItem (new WaitCallback (EnsureConnectionThreaded), state);
		}
				                   
		private static void EnsureConnectionThreaded (object obj)
		{
			EnsureConnectionState state = obj as EnsureConnectionState;
			IConnectionPool pool = state.ConnectionSettings.ConnectionPool;

			try {
				pool.Initialize ();
				Services.DispatchService.GuiDispatch (delegate () {
					state.Callback (state.ConnectionSettings, true);
				});
			} catch (Exception e) {
				Runtime.LoggingService.Debug (e);
				
				state.Attempt = state.Attempt + 1;
				Services.DispatchService.GuiDispatch (delegate () {
					EditConnectionGui (state);
				});
			}
		}
		
		private static void EditConnectionGui (object obj)
		{
			EnsureConnectionState state = obj as EnsureConnectionState;
			if (state.Attempt < 4) {
				ConnectionDialog dlg = new ConnectionDialog (state.ConnectionSettings);
				try {
					if (dlg.Run () == (int)ResponseType.Ok) {
						ConnectionSettingsService.EditConnection (state.ConnectionSettings);
						ThreadPool.QueueUserWorkItem (new WaitCallback (EnsureConnectionThreaded), state);
						return;
					}
				} finally {
					dlg.Destroy ();
				}
			}
			state.Callback (state.ConnectionSettings, false);
		}
	}
					
	internal class EnsureConnectionState
	{
		public int Attempt;
		public ConnectionSettings ConnectionSettings;
		public ConnectionSettingsCallback Callback;
		
		public EnsureConnectionState (ConnectionSettings settings, ConnectionSettingsCallback callback, int attempt)
		{
			ConnectionSettings = settings;
			Callback = callback;
			Attempt = attempt;
		}
	}
}