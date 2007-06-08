//
// Requests/ResponderRequest.cs: Handles FastCGI requests for a responder.
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
using System.Globalization;
using System.Threading;

namespace FastCgi
{
	public class ResponderRequest : Request
	{
		private byte [] input_data;
		private int write_index;
		private IResponder responder;
		
		public ResponderRequest (ushort requestID, Connection connection)
			: base (requestID, connection)
		{
			if (!Server.SupportsResponder)
				throw new Exception ();
			
			responder = Server.CreateResponder (this);
			
			InputDataReceived  += OnInputDataReceived;
			InputDataCompleted += OnInputDataCompleted;
		}
		
		public byte [] InputData {
			get {return input_data != null ? input_data : new byte [0];}
		}
		
		private void OnInputDataReceived (Request sender, byte [] data)
		{
			if (input_data == null)
			{
				string length_text = this.GetParameter
					("CONTENT_LENGTH");
				if (length_text == null)
				{
					AbortRequest ("Content length parameter missing.");
					return;
				}
				
				int length;
				try
				{
					length = int.Parse (length_text,
						CultureInfo.InvariantCulture);
				}
				catch
				{
					AbortRequest ("Content length parameter not an integer.");
					return;
				}
				
				input_data = new byte [length];
			}
			
			if (write_index + data.Length > input_data.Length)
			{
				AbortRequest ("Input data exceeds content length.");
				return;
			}
			
			data.CopyTo (input_data, write_index);
			write_index += data.Length;
		}
		
		private void OnInputDataCompleted (Request sender)
		{
			DataNeeded = false;
			if (input_data != null && (write_index < input_data.Length)) {
				AbortRequest ("Insufficient input data received.");
				return;
			}
			
			if (Server.MultiplexConnections)
				ThreadPool.QueueUserWorkItem (Worker);
			else
				Worker (null);
		}
		
		private void Worker (object state)
		{
			int appStatus = responder.Process ();
			CompleteRequest (appStatus, ProtocolStatus.RequestComplete);
		}
	}
	
	public interface IResponder
	{
		int Process ();
	}
}
