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
using System.Net.Sockets;

namespace FastCgi {
	public class Connection {
		private Hashtable requests = new Hashtable ();
		private bool      keepAlive;
		private Socket    socket;
		private Server    server;
		private bool      stop = false;
		
		private object    requestLock = new object ();
		
		public Connection (Socket socket, Server server)
		{
			this.socket = socket;
			this.server = server;
		}
		
		public int RequestCount {
			get {return requests.Count;}
		}
		
		public void Start ()
		{
			Logger.Write (LogLevel.Notice, "Receving records...");
			
			do {
				Record record = new Record (socket);
				Request request = (Request) (requests.ContainsKey
					(record.RequestID) ? requests [record.RequestID] : null);
				
				Logger.Write (LogLevel.Notice,
					" Record received (" + record.Type + ", " + record.RequestID + ") " + (request == null ? "[NEW]" : "[EXISTING]"));
				
				switch (record.Type) {
				case RecordType.BeginRequest:
				
					if (request != null) {
						stop = true;
						Logger.Write (LogLevel.Error, "Request with given ID already exists. Terminating connection.");
						break;
					}
					
					BeginRequestBody body = new BeginRequestBody
						(record);
						
					if (!server.CanRequest)
					{
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
							"Unknown role (" + body.Role + ").");
						EndRequest (record.RequestID, 0,
							ProtocolStatus.UnknownRole);
						break;
					}
					
					lock (requestLock) {
						requests.Add (record.RequestID,
							request);
					}
					
					keepAlive = keepAlive ||
						(body.Flags & BeginRequestFlags.KeepAlive) != 0;
					
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
			while (!stop && (UnfinishedRequests || keepAlive));
		}
		
		private bool UnfinishedRequests {
			get {
				foreach (Request request in requests.Values)
					if (request != null &&
						request.DataNeeded == true)
						return true;
				
				return false;
			}
		}
		
		public void SendResponse (RecordType type, ushort requestID,
		                          byte [] bodyData)
		{
			new Record (1, type, requestID, bodyData).Send (socket);
		}
		
		public void EndRequest (ushort requestID, int appStatus,
		                        ProtocolStatus protocolStatus)
		{
			EndRequestBody body = new EndRequestBody (appStatus,
				protocolStatus);
			
			new Record (1, RecordType.EndRequest, requestID,
				body.Data).Send (socket);
			
			if (requests.ContainsKey (requestID)) {
				lock (requestLock) {
					requests.Remove (requestID);
				}
			}
			
			if (requests.Count == 0 && !keepAlive) {
				socket.Close ();
				server.EndConnection (this);
			}
		}
		
		public Server Server {
			get {return server;}
		}
	}
}
