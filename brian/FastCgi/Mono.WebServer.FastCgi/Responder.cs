//
// Responder.cs: Resonds to FastCGI "Responder" requests with an ASP.NET
// application.
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
using System.Collections;
using Mono.FastCgi;

namespace Mono.WebServer.FastCgi
{
	public class Responder : MarshalByRefObject, IResponder
	{
		private ResponderRequest request;
		
		public Responder (ResponderRequest request)
		{
			this.request = request;
		}
		
		public int Process ()
		{
			// Uncommenting the following lines will cause the page
			// + headers to be rendered as plain text. (Pretty sweet
			// for debugging.)
			
			//SendOutputText ("Content-type: text/plain\r\n\r\n");
			//SendOutputText ("Output:\r\n");
			
			string vhost = request.GetParameter ("HTTP_HOST");
			int    port  = int.Parse (request.GetParameter ("SERVER_PORT"));
			string path  = request.GetParameter ("REQUEST_URI");
			
			ApplicationHost host = Server.GetApplicationForPath (vhost, port, path, true);
			
			try {
				host.ProcessRequest (this);
			} catch (Exception e) {
				Logger.Write (LogLevel.Error, "ERROR PROCESSING REQUEST: " + e);
				return 0;
			}
			
			// MIN_VALUE means don't close.
			return int.MinValue;
		}
		
		public ResponderRequest Request {
			get {return request;}
		}
		
		public void SendOutput(string text, System.Text.Encoding encoding)
		{
			request.SendOutput (text, encoding);
		}
		
		public void SendOutput (byte [] data, int length)
		{
			request.SendOutput (data, length);
		}
		
		public string GetParameter (string name)
		{
			return request.GetParameter (name);
		}
		
		public IDictionary GetParameters ()
		{
			return request.GetParameters ();
		}
		
		public int RequestID {
			get {return request.RequestID;}
		}
		
		public byte [] InputData {
			get {return request.InputData;}
		}
		
		public void CompleteRequest (int appStatus)
		{
			request.CompleteRequest (appStatus, ProtocolStatus.RequestComplete);
		}
		
		public bool IsConnected {
			get {return request.IsConnected;}
		}
	}
}
