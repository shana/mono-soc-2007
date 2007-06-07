//
// Server/Server.cs: Handles connections.
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2007 Brian Nickel
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Globalization;

namespace FastCgi
{
	public abstract class Server
	{
		private ArrayList connections = new ArrayList ();
		private Socket listen_socket;
		private bool accepting = false;
		private object accept_lock = new object ();
		private AsyncCallback accept_cb;
		private int  max_connections       = int.MaxValue;
		private int  max_requests          = int.MaxValue;
		private bool multiplex_connections = false;
	   	
		protected Server ()
		{
		}
		
		protected abstract Socket CreateSocket ();
		
		public int MaxConnections {
			get {return max_connections;}
			set {
				if (value < 1)
					throw new ArgumentOutOfRangeException
						("value", "At least one connection must be permitted.");
				
				max_connections = value;
			}
		}
		
		public int MaxRequests {
			get {return max_requests;}
			set {
				if (value < 1)
					throw new ArgumentOutOfRangeException
						("value", "At least one connection must be permitted.");
				
				max_requests = value;
			}
		}
		
		public bool MultiplexConnections {
			get {return multiplex_connections;}
			set {multiplex_connections = value;}
		}
		
		public void Start ()
		{
			Logger.Write (LogLevel.Notice, "Server.Start () [ENTER]");
			listen_socket = CreateSocket ();
			listen_socket.Blocking = false;
			listen_socket.Listen (500);
			accept_cb = new AsyncCallback (OnAccept);
			listen_socket.BeginAccept (accept_cb, null);
			Logger.Write (LogLevel.Notice, "Server.Start () [EXIT]");
		}
		
		public void EndConnection (Connection connection)
		{
			if (connections.Contains (connection))
				connections.Remove (connection);
			
			if (!accepting && CanAccept)
				BeginAccept ();
		}
		
		public bool CanAccept {
			get {return ConnectionCount < max_connections;}
		}
		
		public bool CanRequest {
			get {return RequestCount < max_requests;}
		}
		
		public int ConnectionCount {
			get {return connections.Count;}
		}
		
		public int RequestCount {
			get {
				int requests = 0;
				foreach (Connection c in connections)
					requests += c.RequestCount;
				return requests;
			}
		}
		
		public IDictionary GetValues (IEnumerable names)
		{
			Hashtable pairs = new Hashtable ();
			foreach (object key in names) {
				
				if (pairs.ContainsKey (key)) {
					Logger.Write (LogLevel.Warning,
						"Server.GetValues: Duplicate name, '" + key + "', encountered.");
					continue;
				}
				
				string name = key as string;
				
				if (name == null)
					throw new ArgumentException (
						"Names must all be strings.",
						"names");
				
				
				string value = null;
				switch (name)
				{
				case "FCGI_MAX_CONNS":
					value = max_connections.ToString (CultureInfo.InvariantCulture);
				break;
					
				case "FCGI_MAX_REQS":
					value = max_requests.ToString (CultureInfo.InvariantCulture);
				break;
				
				case "FCGI_MPXS_CONNS":
					value = multiplex_connections.ToString (CultureInfo.InvariantCulture);
				break;
				}
				
				if (value == null) {
					Logger.Write (LogLevel.Warning,
						"Server.GetValues: Unknown name, '" + name + "', encountered.");
					continue;
				}
				
				pairs.Add (name, value);
			}
			
			return pairs;
		}
		
		private void OnAccept (IAsyncResult ares)
		{
			Logger.Write (LogLevel.Notice, "Server.OnAccept () [ENTER]");
			Connection connection = null;
			
			lock (accept_lock) {
				accepting = false;
			}
			
			try {
				Socket accepted = listen_socket.EndAccept (ares);
				accepted.Blocking = true;
				connection = new Connection (accepted, this);
				connections.Add (connections);
			} catch {
			}
			
			if (CanAccept)
				BeginAccept ();
			
			if (connection != null)
				connection.Start ();
			
			Logger.Write (LogLevel.Notice, "Server.OnAccept () [EXIT]");
		}
		
		private void BeginAccept ()
		{
			lock (accept_lock) {
				accepting = true;
				listen_socket.BeginAccept (accept_cb, null);
			}
		}
		
		private System.Type responder_type = null;
		public void SetResponder (System.Type responder)
		{
			if (!responder.IsSubclassOf (typeof (IResponder)))
				throw new ArgumentException
					("Responder must implement the FastCgi.IResponder interface.",
					"responder");
			
			if (responder.GetConstructor
				(new System.Type[] {typeof (ResponderRequest)}) == null)
				throw new ArgumentException
					("Responder must contain constructor 'ctor(ResponderRequest)'",
					"responder");
			
			responder_type = responder;
		}
		
		public IResponder CreateResponder (ResponderRequest request)
		{
			return (IResponder) Activator.CreateInstance
				(responder_type, new object [] {this});
		}
		
		public bool SupportsResponder {get {return responder_type != null;}}
	}
}
