/***********************************************************************
 *                                                                     *
 *  Copyright (c) 2007 Brian Nickel <brian.nickel@gmail.com            *
 *                                                                     *
 *  Permission is hereby granted, free of charge, to any person        *
 *  obtaining a copy of this software and associated documentation     *
 *  files (the "Software"), to deal in the Software without            *
 *  restriction, including without limitation the rights to use,       *
 *  copy, modify, merge, publish, distribute, sublicense, and/or sell  *
 *  copies of the Software, and to permit persons to whom the          *
 *  Software is furnished to do so, subject to the following           *
 *  conditions:                                                        *
 *                                                                     *
 *  The above copyright notice and this permission notice shall be     *
 *  included in all copies or substantial portions of the Software.    *
 *                                                                     *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,    *
 *  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES    *
 *  OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND           *
 *  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT        *
 *  HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,       *
 *  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING       *
 *  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR      *
 *  OTHER DEALINGS IN THE SOFTWARE.                                    *
 *                                                                     *
 ***********************************************************************/

using System;
using System.Runtime.InteropServices;

namespace FastCgi
{
	public class UnixSocketEndPoint : Mono.Unix.UnixEndPoint
	{
		public UnixSocketEndPoint (IntPtr socket) : base (GetSocketName (socket))
		{
		}
		
		public static string GetSocketName (IntPtr socket)
		{
			sockaddr addr	= new sockaddr ();
			int addr_length = 1024;
			
			if (getsockname(socket, ref addr, ref addr_length) != 0)
			{
				throw new ArgumentException ("Socket not found.", "socket");
			}
			
			return System.Text.Encoding.ASCII.GetString (addr.sa_data, 0, addr_length - 3);
		}
		
		[StructLayout(LayoutKind.Sequential)]
		private struct sockaddr
		{
			public short  sa_family;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=1022)]
			public byte[] sa_data;
		}
		
		[DllImport("libc.so.6")]
		private static extern int getsockname(IntPtr s, ref sockaddr name, ref int namelen);
	}
}
