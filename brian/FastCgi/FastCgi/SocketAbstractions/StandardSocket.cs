//
// SocketAbstractions/StandardSocket.cs: Provides a wrapper around a standard
// .NET socket.
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
using System.Net;
using System.Net.Sockets;

namespace Mono.FastCgi
{
	public class StandardSocket : ISocketAbstraction
	{
		private Socket socket;
		
		public StandardSocket (Socket socket)
		{
			if (socket == null)
				throw new ArgumentNullException ("socket");
			
			this.socket = socket;
		}
		
		public StandardSocket (AddressFamily addressFamily,
		                       SocketType socketType,
		                       ProtocolType protocolType,
		                       EndPoint localEndPoint)
		{
			if (localEndPoint == null)
				throw new ArgumentNullException ("localEndPoint");
			
			socket = new Socket (addressFamily, socketType,
				protocolType);
			
			socket.Bind (localEndPoint);
		}
		
		public void Close ()
		{
			socket.Close ();
		}
		
		public int Receive (byte [] buffer)
		{
			return socket.Receive (buffer);
		}
		
		public int Send (byte [] data)
		{
			return socket.Send (data);
		}
		
		public bool Blocking {
			get {return socket.Blocking;}
			set {socket.Blocking = value;}
		}
		
		public void Listen (int backlog)
		{
			socket.Listen (backlog);
		}
		
		public IAsyncResult BeginAccept (AsyncCallback callback,
		                                 object state)
		{
			return socket.BeginAccept (callback, state);
		}
		
		public ISocketAbstraction EndAccept (IAsyncResult asyncResult)
		{
			return new StandardSocket (socket.EndAccept (asyncResult));
		}
	}
}