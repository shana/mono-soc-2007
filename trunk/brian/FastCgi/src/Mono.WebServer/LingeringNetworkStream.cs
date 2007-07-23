//
// Mono.WebServer.LingeringNetworkStream
//
// Authors:
//	Gonzalo Paniagua Javier (gonzalo@ximian.com)
//
// Documentation:
//	Brian Nickel
//
// (C) Copyright 2004 Novell, Inc. (http://www.novell.com)
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

namespace Mono.WebServer
{
	/// <summary>
	///    This class extends <see cref="NetworkStream" />, adding support
	///    for a lingering, rather than abrupt, shutdown of the socket on
	///    close.
	/// </summary>
	public class LingeringNetworkStream : NetworkStream 
	{
		/// <summary>
		///    Contains the number of microseconds to wait between
		///    checks for the shutdown socket.
		/// </summary>
		const int useconds_to_linger = 2000000;
		
		/// <summary>
		///    Contains the number of microseconds to wait for the
		///    socket to shutdown before forcively closing it.
		/// </summary>
		const int max_useconds_to_linger = 30000000;
		
		/// <summary>
		///    Indicates whether or not to enable a lingering close.
		/// </summary>
		bool enableLingering = true;
		
		/// <summary>
		///    Contains the buffer used for reading the socket during a
		///    lingering shutdown.
		/// </summary>
		/// <remarks>
		///    As the buffer data is not actually used, it is safe to
		///    cache it for the performance gains.
		/// </remarks>
		static byte [] buffer;
		
		/// <summary>
		///    Indicates whether or not the current instance owns the
		///    socket.
		/// </summary>
		bool owns;
		
		
		/// <summary>
		///    Constructs and initalizes a new instance of <see
		///    cref="LingeringNetworkStream" /> with a specified socket,
		///    indicating whether or not the new instance will own the
		///    socket.
		/// </summary>
		/// <param name="sock">
		///    The underlying <see cref="Socket" /> object to read from
		///    and write to.
		/// </param>
		/// <param name="owns">
		///    A <see cref="bool" /> indicating whether or not the new
		///    instance will own the underlying socket and be required
		///    to close it.
		/// </param>
		/// <remarks>
		///    See <see cref="NetworkStream(Socket,bool)" /> for more
		///    details.
		/// </remarks>
		public LingeringNetworkStream (Socket sock, bool owns) : base (sock, owns)
		{
			this.owns = owns;
		}

		/// <summary>
		///    Gets whether or not the current instance owns the
		///    underlying socket.
		/// </summary>
		/// <value>
		///    A <see cref="bool" /> indicating whether or not the
		///    current instance owns the underlying socket.
		/// </value>
		public bool OwnsSocket {
			get { return owns; }
		}
		
		/// <summary>
		///    Gets and sets whether or not a lingering close of the
		///    current instance is enabled.
		/// </summary>
		/// <value>
		///    A <see cref="bool" /> indicating whether or not a
		///    lingering close is enabled.
		/// </value>
		public bool EnableLingering
		{
			get { return enableLingering; }
			set { enableLingering = value; }
		}

		/// <summary>
		///    Shuts down the socket and waits up to 30 seconds for the
		///    socket to finish.
		/// </summary>
		void LingeringClose ()
		{
			int waited = 0;

			if (!Connected)
				return;

			Socket.Shutdown (SocketShutdown.Send);
			DateTime start = DateTime.UtcNow;
			while (waited < max_useconds_to_linger) {
				int nread = 0;
				try {
					if (!Socket.Poll (useconds_to_linger, SelectMode.SelectRead))
						break;

					if (buffer == null)
						buffer = new byte [512];

					nread = Socket.Receive (buffer, 0, buffer.Length, 0);
				} catch { }

				if (nread == 0)
					break;

				waited += (int) (DateTime.UtcNow - start).TotalMilliseconds * 1000;
			}
		}

		/// <summary>
		///    Closes the connection, optionally lingering up to 30
		///    seconds for the socket to shut down.
		/// </summary>
		/// <remarks>
		///    If <see cref="EnableLingering" /> is <see langword="true"
		///    />, the socket will be shut down and the method will wait
		///    up to 30 seconds before forcibly closing the stream.
		/// </remarks>
		public override void Close ()
		{
			if (enableLingering) {
				try {
					LingeringClose ();
				} finally {
					base.Close ();
				}
			}
			else
				base.Close ();
		}

		/// <summary>
		///    Gets whether or not the underlying socket is connected.
		/// </summary>
		/// <value>
		///    A <see cref="bool" /> indicating whether or not the
		///    underlying socket is connected.
		/// </value>
		public bool Connected {
			get { return Socket.Connected; }
		}
	}
}
