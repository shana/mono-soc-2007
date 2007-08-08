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
#if NET_2_0
using System.Collections.Generic;
#else
using System.Collections;
#endif
using Mono.FastCgi;
using Mono.WebServer;
using System.Text;
using System.Net;
using System.Globalization;
using System.IO;

namespace Mono.WebServer.FastCgi
{
	public class WorkerRequest : MonoWorkerRequest
	{
		private static string [] indexFiles = { "index.aspx",
							"Default.aspx",
							"default.aspx",
							"index.html",
							"index.htm" };
		
		static WorkerRequest ()
		{
			#if NET_2_0
			SetDefaultIndexFiles (System.Configuration.ConfigurationManager.AppSettings [
				"MonoServerDefaultIndexFiles"]);
			#else
			SetDefaultIndexFiles (System.Configuration.ConfigurationSettings.AppSettings [
				"MonoServerDefaultIndexFiles"]);
			#endif
		}
		
		private static void SetDefaultIndexFiles (string list)
		{
			if (list == null)
				return;
			
			#if NET_2_0
			List<string> files = new List<string> ();
			#else
			ArrayList files = new ArrayList ();
			#endif
			
			string [] fs = list.Split (',');
			foreach (string f in fs) {
				string trimmed = f.Trim ();
				if (trimmed == "") 
					continue;

				files.Add (trimmed);
			}

			#if NET_2_0
			indexFiles = files.ToArray ();
			#else
			indexFiles = (string []) files.ToArray (typeof (string));
			#endif
		}


		private StringBuilder headers = new StringBuilder ();
		private Responder responder;
		private byte [] input_data;
		private string extra_path;
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
			// This checks if the current path is a directory and
			// tries to determine the appropriate file.
			string path = responder.PhysicalPath;
			
			if (!Directory.Exists (path))
				return true;
			
			foreach (string file in indexFiles)
				if (File.Exists (Path.Combine (path, file))) {
					extra_path = (responder.Path.EndsWith ("/") ?
						"" : "/") + file;
					return true;
				}
			
			return true;
		}
		
		public override bool HeadersSent ()
		{
			return headers == null;
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
			
			if (headers != null) {
				headers.Append ("\r\n");
				responder.SendOutput (headers.ToString (),
					HeaderEncoding);
				headers = null;
			}
			
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
			string address = responder.GetParameter ("SERVER_ADDR");
			if (address != null && address.Length > 0)
				return address;
			
			address = AddressFromHostName (
				responder.GetParameter ("HTTP_HOST"));
			if (address != null && address.Length > 0)
				return address;
			
			address = AddressFromHostName (
				responder.GetParameter ("SERVER_NAME"));
			if (address != null && address.Length > 0)
				return address;
			
			return base.GetLocalAddress ();
		}

		public override int GetLocalPort ()
		{
			try {
				return responder.PortNumber;
			} catch {
				return base.GetLocalPort ();
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
			string addr = responder.GetParameter ("REMOTE_ADDR");
			return addr != null && addr.Length > 0 ?
				addr : base.GetRemoteAddress ();
		}
		
		public override string GetRemoteName ()
		{
			string ip = GetRemoteAddress ();
			string name = null;
			try {
				#if NET_2_0
				IPHostEntry entry = Dns.GetHostEntry (ip);
				#else
				IPHostEntry entry = Dns.GetHostByName (ip);
				#endif
				name = entry.HostName;
			} catch {
				name = ip;
			}

			return name;
		}

		public override int GetRemotePort ()
		{
			string port = responder.GetParameter ("REMOTE_PORT");
			if (port == null || port.Length == 0)
				return base.GetRemotePort ();
			
			try {
				return int.Parse (port);
			} catch {
				return base.GetRemotePort ();
			}
		}
		
		public override string GetServerVariable (string name)
		{
			string value = responder.GetParameter (name);
			
			if (value == null)
				value = Environment.GetEnvironmentVariable (name);
			
			return value != null ? value : base.GetServerVariable (name);
		}
		
		public override void SendResponseFromMemory (byte [] data, int length)
		{
			if (headers != null) {
				headers.Append ("\r\n");
				responder.SendOutput (headers.ToString (),
					HeaderEncoding);
				headers = null;
			}
			
			responder.SendOutput (data, length);
		}

		public override void SendStatus (int statusCode, string statusDescription)
		{
			if (headers != null)
				headers.AppendFormat (
					CultureInfo.InvariantCulture,
					"{0} {1} {2}\r\n", GetHttpVersion (),
					statusCode, statusDescription);
		}

		public override void SendUnknownResponseHeader (string name, string value)
		{
			if (headers != null)
				headers.AppendFormat (
					CultureInfo.InvariantCulture,
					"{0}: {1}\r\n", name, value);
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
			if (extra_path != null)
				return responder.Path + extra_path;
			
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
			
			#if NET_2_0
			IDictionary<string,string> pairs =
				responder.GetParameters ();
			#else
			IDictionary pairs = responder.GetParameters ();
			#endif
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
			
			return (value != null) ?
				value : base.GetKnownRequestHeader (index);
		}
		
		public override string GetServerName ()
		{
			string server_name = HostNameFromString (
				responder.GetParameter ("SERVER_NAME"));
			
			if (server_name == null)
				server_name = HostNameFromString (
					responder.GetParameter ("HTTP_HOST"));
			
			if (server_name == null)
				server_name = GetLocalAddress ();
			
			return server_name;
		}

		public override byte [] GetPreloadedEntityBody ()
		{
			return input_data;
		}
		
		public override bool IsEntireEntityBodyIsPreloaded ()
		{
			return true;
		}
		
		private static string HostNameFromString (string host)
		{
			if (host == null || host.Length == 0)
				return null;
			
			int colon_index = host.IndexOf (':');
			
			if (colon_index == -1)
				return host;
			
			if (colon_index == 0)
				return null;
			
			return host.Substring (0, colon_index);
		}
		
		private static string AddressFromHostName (string host)
		{
			host = HostNameFromString (host);
			
			if (host == null || host.Length > 126)
				return null;
			
			System.Net.IPAddress [] addresses = null;
			try {
				#if NET_2_0
				addresses = Dns.GetHostEntry (host).AddressList;
				#else
				addresses = Dns.GetHostByName (host).AddressList;
				#endif
			} catch (System.Net.Sockets.SocketException) {
				return null;
			} catch (ArgumentException) {
				return null;
			}
			
			if (addresses == null || addresses.Length == 0)
				return null;
			
			return addresses [0].ToString ();
		}
	}
}
