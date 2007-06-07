//
// Main.cs: Driver class for the application.
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

namespace FastCgi
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Logger.Open ("log.txt");
			Logger.Level = LogLevel.All;
			
			Logger.Write (LogLevel.Notice, "Starting server:");
			foreach (string arg in args)
			{
				Logger.Write (LogLevel.Notice, "   Argument: " + arg);
			}
			
			foreach (string s in System.Environment.GetEnvironmentVariables ().Keys)
			{
				Logger.Write (LogLevel.Notice, "   Environ:  " + s + ": " + System.Environment.GetEnvironmentVariable (s));
			}
			
			Server server = new TcpServer (IPAddress.Any, 1234);
			server.SetResponder (typeof (TestResponder));
			server.Start ();
			Console.WriteLine ("Hit Return to stop the server.");
			Console.ReadLine ();
			
			Logger.Write (LogLevel.Notice, "Server stopped.");
			Logger.Close ();
		}
		
		private class TestResponder : IResponder
		{
			private ResponderRequest request;
			
			public TestResponder (ResponderRequest request)
			{
				this.request = request;
			}
			
			public int Process ()
			{
				request.SendOutputText ("Content-type: text/html\r\n\r\n<html>\n<head><title>Summer of Code Status Report 1</title></head>\n<body>\n");
				request.SendOutputText ("<h2>Parameters</h2>\n<table align=\"center\">\n");
				foreach (string key in request.GetParameters ().Keys)
				{
					request.SendOutputText ("<tr><th>" + key + "</th><td>" + request.GetParameter (key) + "</td></tr>");
				}
				request.SendOutputText ("</table>\n");
				
				request.SendOutputText ("<h2>Request Data</h2>\n<pre>");
				request.SendOutputData (request.InputData);
				request.SendOutputText ("</pre>\n</body>\n</html>");
				
				return 0;
			}
		}
	}
}
