//
// Connection.cs: Handles a FastCGI connection.
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

namespace Mono.FastCgi {
	/// <summary>
	///    Handles a FastCGI connection by processing records and
	///    calling responders.
	/// </summary>
	public class Connection
	{
		#region Private Fields
		
		// Collection of requests.
		private ArrayList requests = new ArrayList ();
		
		// Socket to work with.
		private Socket socket;
		
		// Calling server.
		private Server server;
		
		// Keep connection alive.
		private bool keep_alive;
		
		// Listen
		private bool stop;
		
		// Lock for modifying requests.
		private object request_lock = new object ();
		#endregion
		
		public Connection (Socket socket, Server server)
		{
			this.socket = socket;
			this.server = server;
		}
		
		public int RequestCount {
			get {return requests.Count;}
		}
		
		public bool IsConnected {
			get {return socket.Connected;}
		}
		
		public void Run ()
		{
			Logger.Write (LogLevel.Notice, "Receving records...");
			
			do {
				Record record = new Record (socket);
				Request request = GetRequest (record.RequestID);
				
				Logger.Write (LogLevel.Notice,
					" Record received ({0}, {1}, {2}) [{3}]",
					record.Type, record.RequestID,
					record.Body.Length,
					request == null ? "NEW" : "EXISTING");
				
				switch (record.Type) {
				case RecordType.BeginRequest:
					if (request != null) {
						stop = true;
						Logger.Write (LogLevel.Error, "Request with given ID already exists. Terminating connection.");
						break;
					}
					
					BeginRequestBody body = new BeginRequestBody
						(record);
					
					if (!server.MultiplexConnections && UnfinishedRequests) {
						EndRequest (record.RequestID, 0,
							ProtocolStatus.CantMultiplexConnections);
						break;
					}
					
					if (!server.CanRequest) {
						EndRequest (record.RequestID, 0,
							ProtocolStatus.Overloaded);
						break;
					}
					
					if (body.Role == Role.Responder &&
						server.SupportsResponder)
						request = new ResponderRequest
							(record.RequestID, this);
					
					if (request == null) {
						Logger.Write (LogLevel.Warning,
							"Unknown role ({0}).",
							body.Role);
						EndRequest (record.RequestID, 0,
							ProtocolStatus.UnknownRole);
						break;
					}
					
					lock (request_lock) {
						requests.Add (request);
					}
					
					keep_alive = (body.Flags & BeginRequestFlags.KeepAlive) != 0;
					
				break;
				
				case RecordType.GetValues:
					byte [] response_data;
					
					try {
						IDictionary pairs_in  = NameValuePair.FromData (record.Body);
						IDictionary pairs_out = server.GetValues (pairs_in.Keys);
						response_data = NameValuePair.ToData (pairs_out);
					} catch {
						response_data = new byte [0];
					}
					
					SendResponse (RecordType.GetValuesResult, record.RequestID, response_data);
				break;
			
				case RecordType.Params:
					if (request == null) {
						stop = true;
						Logger.Write (LogLevel.Error, "Request with given ID does not exist. Terminating connection.");
						break;
					}
					
					request.AddParameterData (record.Body);
				
				break;
					
				case RecordType.StandardInput:
					if (request == null) {
						stop = true;
						Logger.Write (LogLevel.Error, "Request with given ID does not exist. Terminating connection.");
						break;
					}
					
					request.AddInputData (record.Body);
				
				break;
				
				case RecordType.Data:
					if (request == null) {
						stop = true;
						Logger.Write (LogLevel.Error, "Request with given ID does not exist. Terminating connection.");
						break;
					}
					
					request.AddFileData (record.Body);
				
				break;
				}
			}
			while (!stop && (UnfinishedRequests || keep_alive));
		}
		
		private bool UnfinishedRequests {
			get {
				foreach (Request request in requests)
					if (request.DataNeeded)
						return true;
				
				return false;
			}
		}
		
		public void SendResponse (RecordType type, ushort requestID,
		                          byte [] bodyData)
		{
			if (IsConnected)
				new Record (1, type, requestID, bodyData).Send (socket);
		}
		
		public void EndRequest (ushort requestID, int appStatus,
		                        ProtocolStatus protocolStatus)
		{
			EndRequestBody body = new EndRequestBody (appStatus,
				protocolStatus);
			
			if (IsConnected)
				new Record (1, RecordType.EndRequest, requestID,
					body.Data).Send (socket);
			
			int index = GetRequestIndex (requestID);
			
			if (index >= 0) {
				lock (request_lock) {
					requests.RemoveAt (index);
				}
			}
			
			if (requests.Count == 0 && (!keep_alive || stop)) {
				socket.Close ();
				server.EndConnection (this);
			}
		}
		
		public void Stop ()
		{
			stop = true;
			foreach (Request req in new ArrayList (requests))
				EndRequest (req.RequestID, -1,
					ProtocolStatus.RequestComplete);
		}
		
		public Server Server {
			get {return server;}
		}
		
		private Request GetRequest (ushort requestID)
		{
			foreach (Request request in requests)
				if (request.RequestID == requestID)
					return request;
			
			return null;
		}
		
		private int GetRequestIndex (ushort requestID)
		{
			int i = 0;
			int count = requests.Count;
			while (i < count && (requests [i] as Request).RequestID != requestID)
				i ++;
			
			return (i != count) ? i : -1;
		}
	}
}
