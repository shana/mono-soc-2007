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
using System.Collections;
using System.Net.Sockets;

namespace FastCgi
{
	public class Connection
	{
		private Hashtable requests = new Hashtable ();
		private bool      keepAlive;
		private Socket    socket;
		private Server    server;
		private bool      stop = false;
		
		private object    requestLock = "REQUEST LOCK";
		
		public Connection (Socket socket, Server server)
		{
			this.socket = socket;
			this.server = server;
		}
		
		public void Start ()
		{
			Logger.Write (LogLevel.Notice, "Receving records...");
			
			do
			{
				Record record = new Record (socket);
				Request request = (Request) (requests.ContainsKey (record.RequestID) ? requests [record.RequestID] : null);
				
				Logger.Write (LogLevel.Notice, " Record received (" + record.Type + ", " + record.RequestID + ") " + (request == null ? "[NEW]" : "[EXISTING]"));
				
				switch (record.Type)
				{
					case RecordType.BeginRequest:
						{
							if (request != null)
							{
								stop = true;
								Logger.Write (LogLevel.Error, "Request with given ID already exists. Terminating connection.");
								break;
							}
							
							BeginRequestBody body = new BeginRequestBody (record);
							if (body.Role == Role.Responder && ResponderRequest.IsSupported)
								request = new ResponderRequest (record.RequestID, this);
							
							if (request == null)
							{
								Logger.Write (LogLevel.Warning, "Unknown role (" + body.Role + ").");
								EndRequest (record.RequestID, 0, ProtocolStatus.UnknownRole);
								break;
							}
							
							lock (requestLock)
							{
								requests.Add (record.RequestID, request);
							}
							
							keepAlive = keepAlive || (body.Flags & BeginRequestFlags.KeepAlive) != 0;
						}
						break;
					
					case RecordType.Params:
						{
							if (request == null)
							{
								stop = true;
								Logger.Write (LogLevel.Error, "Request with given ID does not exist. Terminating connection.");
								break;
							}
							
							request.AddParameterData (record.Content);
						}
						break;
					
					case RecordType.StandardInput:
						{
							if (request == null)
							{
								stop = true;
								Logger.Write (LogLevel.Error, "Request with given ID does not exist. Terminating connection.");
								break;
							}
							
							request.AddInputData (record.Content);
						}
						break;
					
					case RecordType.Data:
						{
							if (request == null)
							{
								stop = true;
								Logger.Write (LogLevel.Error, "Request with given ID does not exist. Terminating connection.");
								break;
							}
							
							request.AddFileData (record.Content);
						}
						break;
				}
			}
			while (!stop && (UnfinishedRequests || keepAlive));
		}
		
		private bool UnfinishedRequests
		{
			get
			{
				foreach (Request request in requests.Values)
					if (request != null && request.DataNeeded == true)
						return true;
				
				return false;
			}
		}
		
		public void SendResponse (RecordType type, ushort requestID, byte [] data)
		{
			Record.Send (socket, 1, type, requestID, data);
		}
		
		public void EndRequest (ushort requestID, int appStatus, ProtocolStatus protocolStatus)
		{
			Record.Send (socket, 1, RecordType.EndRequest, requestID, new EndRequestBody (appStatus, protocolStatus).Data);
			if (requests.ContainsKey (requestID))
			{
				lock (requestLock)
				{
					requests.Remove (requestID);
				}
			}
			
			if (requests.Count == 0 && !keepAlive)
			{
				socket.Close ();
//				server.EndConnection ();
			}
		}
	}
}
