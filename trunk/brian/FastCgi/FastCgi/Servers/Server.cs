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

namespace FastCgi
{
	public abstract class Server
	{
		private ArrayList connections = new ArrayList ();
		private Socket listen_socket;
	   
		protected Server ()
		{
		}
		
		protected abstract Socket CreateSocket ();
		
		AsyncCallback accept_cb;
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
		
		void OnAccept (IAsyncResult ares)
		{
			Logger.Write (LogLevel.Notice, "Server.OnAccept () [ENTER]");
			Socket accepted = null;
			try {
				accepted = listen_socket.EndAccept (ares);
			} catch {
			} finally {
				listen_socket.BeginAccept (accept_cb, null);
			}

			if (accepted == null)
				return;
			accepted.Blocking = true;
			Connection c = new Connection (accepted, this);
			connections.Add (c);
			c.Start ();
			Logger.Write (LogLevel.Notice, "Server.OnAccept () [EXIT]");
		}
	}
}
