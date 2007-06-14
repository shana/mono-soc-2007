using System;

namespace Mono.FastCgi
{
	public static class SocketFactory
	{
		public static Socket CreateTcpSocket (System.Net.IPEndPoint
		                               localEndPoint)
		{
			return new TcpSocket (localEndPoint);
		}
		
		public static Socket CreateTcpSocket (System.Net.IPAddress
		                                      address, int port)
		{
			return new TcpSocket (address, port);
		}
	}
}
