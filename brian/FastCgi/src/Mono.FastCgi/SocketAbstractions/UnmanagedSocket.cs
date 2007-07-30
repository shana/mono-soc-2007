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
using sock=System.Net.Sockets;
using Mono.Unix.Native;
using System.Runtime.InteropServices;
using System.IO;

namespace Mono.FastCgi {
	internal class UnmanagedSocket : Socket
	{
		private const int O_NONBLOCK = 2048;
		
		private IntPtr socket;
		private bool blocking = true;
		private bool connected = false;
		
		unsafe public UnmanagedSocket (IntPtr socket)
		{
			if (!supports_libc)
				throw new NotSupportedException ("Unmanaged sockets not supported.");
			
			if ((int) socket < 0)
				throw new ArgumentException ("Invalid socket.", "socket");
			
			byte [] address = new byte [1024];
			int size = 1024;
			fixed (byte* ptr = address)
				if (getsockname (socket, ptr, ref size) != 0)
					throw new sock.SocketException ();
		
		
			this.socket = socket;
		}
		
		public override void Close ()
		{
			connected = false;
			if (shutdown (socket, (int) sock.SocketShutdown.Both) != 0)
				throw new sock.SocketException ();
		}
		
		unsafe public override int Receive (byte [] buffer, int size, sock.SocketFlags flags)
		{
			if (!connected)
				return 0;
			
			int value;
			fixed (byte* ptr = buffer)
				value = recv (socket, ptr, size, (int) flags);
			
			if (value != size)
				connected = false;
			
			if (value >= 0)
				return value;
			
			throw new sock.SocketException ();
		}
		
		unsafe public override int Send (byte [] data, int size, sock.SocketFlags flags)
		{
			if (!connected)
				return 0;
			
			int value;
			fixed (byte* ptr = data)
				value = send (socket, ptr, size, (int) flags);
			
			if (value != size)
				connected = false;
			
			if (value >= 0)
				return value;
			
			throw new sock.SocketException ();
		}
		
		public override bool Blocking {
			get {return blocking;}
			set {
				long ret = Syscall.fcntl((int) socket, FcntlCommand.F_GETFL, 0L);
				if (ret == -1)
					throw new sock.SocketException ();
				
				
				if (value)
					ret |= O_NONBLOCK;
				else
					ret &= ~O_NONBLOCK;
				
				ret = Syscall.fcntl((int) socket, FcntlCommand.F_SETFL, ret);
				
				blocking = value;
			}
		}
		
		public override void Listen (int backlog)
		{
			listen (socket, backlog);
		}
		
		public override IAsyncResult BeginAccept (AsyncCallback callback,
		                                 object state)
		{
			SockAccept s = new SockAccept (socket, callback, state);
			System.Threading.ThreadPool.QueueUserWorkItem (s.Run);
			return s;
		}
		
		public override Socket EndAccept (IAsyncResult asyncResult)
		{
			SockAccept s = asyncResult as SockAccept;
			if (s == null || s.socket != socket)
				throw new ArgumentException (
					"Result was not produced by current instance.",
					"asyncResult");
			
			UnmanagedSocket u = new UnmanagedSocket (s.accepted);
			u.connected = true;
			return u;
		}
		
		public override bool Connected {
			get {return connected;}
		}
		
		[DllImport ("libc", SetLastError=true, EntryPoint="shutdown")]
		unsafe extern static int shutdown (IntPtr s, int how);
		
		[DllImport ("libc", SetLastError=true, EntryPoint="send")]
		unsafe extern static int send (IntPtr s, byte *buffer, int len, int flags);
		
		[DllImport ("libc", SetLastError=true, EntryPoint="recv")]
		unsafe extern static int recv (IntPtr s, byte *buffer, int len, int flags);
		
		[DllImport ("libc", SetLastError=true, EntryPoint="accept")]
		unsafe extern static IntPtr accept (IntPtr s, byte *addr, ref int addrlen);
		
		[DllImport("libc", SetLastError=true, EntryPoint="getsockname")]
		unsafe static extern int getsockname(IntPtr s, byte *addr, ref int namelen);
		
		[DllImport ("libc", SetLastError=true, EntryPoint="listen")]
		unsafe extern static int listen (IntPtr s, int count);
		
		private class SockAccept : IAsyncResult
		{
			private bool completed = false;
			public  IntPtr socket;
			public  IntPtr accepted;
			private AsyncCallback callback;
			private object state;
			
			public SockAccept (IntPtr socket, AsyncCallback callback,
			                   object state)
			{
				this.socket = socket;
				this.callback = callback;
				this.state = state;
			}
			
			unsafe public void Run (object state)
			{
				byte[] address = new byte [1024];
				int size = 1024;
				fixed (byte* ptr = address)
					accepted = accept (socket, ptr, ref size);
				
				completed = true;
				callback (this);
			}
			
			public bool IsCompleted {
				get {return completed;}
			}
			
			public bool CompletedSynchronously {
				get {return false;}
			}
			
			public System.Threading.WaitHandle AsyncWaitHandle {
				get {return null;}
			}
			
			public object AsyncState {
				get {return state;}
			}
		}
		
		private static bool supports_libc;
		
		static UnmanagedSocket ()
		{
			try {
				string os = "";
				using (Stream st = File.OpenRead ("/proc/sys/kernel/ostype")) {
					StreamReader sr = new StreamReader (st);
					os = sr.ReadToEnd ();
				}
				supports_libc = os.StartsWith ("Linux");
			} catch {
			}
		}
	}
}