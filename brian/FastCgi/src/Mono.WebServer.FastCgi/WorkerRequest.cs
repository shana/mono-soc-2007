//
// WorkerRequest.cs: Extends MonoWorkerRequest by getting information from and
// writing information to a Responder object.
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
using Mono.WebServer;
using System.Text;

namespace Mono.WebServer.FastCgi
{
	public class WorkerRequest : MonoWorkerRequest
	{
		private Responder responder;
		private byte [] input_data;
		public WorkerRequest (Responder responder, ApplicationHost appHost) : base (appHost)
		{
			this.responder = responder;
			input_data = responder.InputData;
		}
		
		public override int RequestId {
			get {return responder.RequestID;}
		}

		public override string GetPathInfo ()
		{
			return responder.GetParameter ("PATH_INFO");
		}
		
		string raw_url = null;
		public override string GetRawUrl ()
		{
			if (raw_url != null)
				return raw_url;
			
			StringBuilder b = new StringBuilder (GetUriPath ());
			string query = GetQueryString ();
			if (query != null && query.Length > 0) {
				b.Append ('?');
				b.Append (query);
			}
			
			raw_url = b.ToString ();
			return raw_url;
		}

		protected override bool GetRequestData ()
		{
			return true;
		}
		
		public override bool HeadersSent ()
		{
			return response_sent;
		}
		
		public override void FlushResponse (bool finalFlush)
		{
			if (finalFlush)
				CloseConnection ();
		}

		public override bool IsSecure ()
		{
			return false;
		}
		
		private bool closed = false;
		public override void CloseConnection ()
		{
			if (closed)
				return;
			
			if (!response_sent)
				responder.SendOutput ("\r\n", HeaderEncoding);
			
			responder.CompleteRequest (0);
			
			closed = true;
		}

		public override string GetHttpVerbName ()
		{
			return responder.GetParameter ("REQUEST_METHOD");
		}

		public override string GetHttpVersion ()
		{
			return responder.GetParameter ("SERVER_PROTOCOL");
		}

		public override string GetLocalAddress ()
		{
			string address = responder.GetParameter ("HTTP_HOST");
			if (address != null && address.Length > 0)
				return address.Split (new char [] {':'}, 2) [0];
			
			return base.GetLocalAddress ();
		}

		public override int GetLocalPort ()
		{
			try {
				return responder.PortNumber;
			} catch {
				return 0;
			}
		}

		public override string GetQueryString ()
		{
			return responder.GetParameter ("QUERY_STRING");
		}

		public override byte [] GetQueryStringRawBytes ()
		{
			string query_string = GetQueryString ();
			if (query_string == null)
				return null;
			return Encoding.GetBytes (query_string);
		}

		public override string GetRemoteAddress ()
		{
			return responder.GetParameter ("REMOTE_ADDR");
		}
		
		/*
		FIXME: HOW CAN WE FIGURE THIS OUT?
		
		public override string GetRemoteName ()
		{
		}

		public override int GetRemotePort ()
		{
			return remotePort;
		}
		*/
		
		public override string GetServerVariable (string name)
		{
			string value = responder.GetParameter (name);
			
			if (value == null)
				value = Environment.GetEnvironmentVariable (name);
			
			return value != null ? value : base.GetServerVariable (name);
		}
		
		private bool response_sent = false;
		public override void SendResponseFromMemory (byte [] data, int length)
		{
			if (!response_sent)
				responder.SendOutput ("\r\n", HeaderEncoding);
			
			responder.SendOutput (data, length);
			response_sent = true;
		}

		public override void SendStatus (int statusCode, string statusDescription)
		{
			if (statusCode == 200)
				return;
			
			string line = string.Format ("{0} {1} {2}\r\n", GetHttpVersion (), statusCode, statusDescription);
			responder.SendOutput (line, HeaderEncoding);
		}

		public override void SendUnknownResponseHeader (string name, string value)
		{
			string line = string.Format ("{0}: {1}\r\n", name, value);
			responder.SendOutput (line, HeaderEncoding);
		}

		public override bool IsClientConnected ()
		{
			return responder.IsConnected;
		}

		string uri_path = null;
		public override string GetUriPath ()
		{
			if (uri_path != null)
				return uri_path;
			
			uri_path = GetFilePath () + GetPathInfo ();
			return uri_path;
		}

		public override string GetFilePath ()
		{
			return responder.Path;
		}
		
		public override string GetUnknownRequestHeader (string name)
		{
			foreach (string [] pair in GetUnknownRequestHeaders ())
			{
				if (pair [0] == name)
					return pair [1];
			}
			
			
			return base.GetUnknownRequestHeader (name);
		}
		
		private string [][] unknownHeaders = null;
		private string [] knownHeaders = null;
		public override string [][] GetUnknownRequestHeaders ()
		{
			if (unknownHeaders != null)
				return unknownHeaders;
			
			IDictionary pairs = responder.GetParameters ();
			knownHeaders = new string [RequestHeaderMaximum];
			string [][] headers = new string [pairs.Count][];
			int count = 0;
			
			foreach (string key in pairs.Keys) {
				if (!key.StartsWith ("HTTP_"))
					continue;
				
				string name  = ReformatHttpHeader (key);
				string value = (string) pairs [key];
				int id = GetKnownRequestHeaderIndex (name);
				
				if (id >= 0) {
					knownHeaders [id] = value;
					continue;
				}
				
				headers [count++] = new string [] {name, value};
			}
			
			unknownHeaders = new string [count][];
			System.Array.Copy (headers, 0, unknownHeaders, 0, count);
			
			return unknownHeaders;
		}
		
		string ReformatHttpHeader (string header)
		{
			string [] parts = header.Substring (5).Split ('_');
			for (int i = 0; i < parts.Length; i ++)
				parts [i] = parts [i].Substring (0, 1).ToUpper ()
					+ parts [i].Substring (1).ToLower ();
			
			return string.Join ("-", parts);
		}
		
		
		public override string GetKnownRequestHeader (int index)
		{
			string value = null;
			switch (index)
			{
			case System.Web.HttpWorkerRequest.HeaderContentType:
				value = responder.GetParameter ("CONTENT_TYPE");
				break;
				
			case System.Web.HttpWorkerRequest.HeaderContentLength:
				value = responder.GetParameter ("CONTENT_LENGTH");
				break;
			default:
				GetUnknownRequestHeaders ();
				value = knownHeaders [index];
				break;
			}
			
			return (value != null) ? value : base.GetKnownRequestHeader (index);
		}
		
		public override string GetServerName ()
		{
			return responder.GetParameter ("SERVER_NAME");
		}

		public override byte [] GetPreloadedEntityBody ()
		{
			return input_data;
		}
		
		public override bool IsEntireEntityBodyIsPreloaded ()
		{
			return true;
		}
	}
}
