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
	public class Request
	{
		#region Delegates
		public delegate void DataReceivedHandler  (Request sender, byte [] data);
		public delegate void DataCompletedHandler (Request sender);
		#endregion
		
		
		
		#region Private Variables
		ushort     requestID;
		Connection connection;
		#endregion
		
		
		
		#region Constructors
		public Request (ushort requestID, Connection connection)
		{
			this.requestID  = requestID;
			this.connection = connection;
		}
		#endregion
		
		
		
		#region Request Completion Handling
		
		// When a request is completed, the output and error streams
		// must be completed and the final farewell must be send so
		// the HTTP server can finish its response.
		
		public void CompleteRequest (int appStatus, ProtocolStatus protocolStatus)
		{
			// Close the standard output if it was opened.
			if (stdoutSent)
				SendStreamData (RecordType.StandardOutput, new byte [0]);
			
			// Close the standard error if it was opened.
			if (stderrSent)
				SendStreamData (RecordType.StandardError,  new byte [0]);
			
			connection.EndRequest (requestID, appStatus, protocolStatus);
		}
		
		private bool dataNeeded = true;
		public bool DataNeeded {get {return dataNeeded;} protected set {dataNeeded = value;}}
		#endregion
		
		
		
		#region Parameter Handling
		
		// Parameters are sent in all requests and are analogous to
		// environment variables. With the exception of some FastCGI
		// specific ones, they are identical to those that would be
		// provided to a CGI script.
		
		protected event DataCompletedHandler ParameterDataCompleted;
		
		ArrayList  parameterData  = new ArrayList ();
		Hashtable  parameterTable = new Hashtable ();
		
		public void AddParameterData (byte [] data)
		{
			// Validate arguments in public methods.
			if (data == null)
				throw new ArgumentNullException ("data");
			
			// When all the parameter data is received, it is acted
			// on and the _parameter_data object is nullified.
			// Further data suggests a problem with the HTTP server.
			if (parameterData == null)
				throw new Exception ("Data already completed.");
			
			// If data was provided, append it to that already
			// received, and exit.
			if (data.Length > 0)
			{
				parameterData.AddRange (data);
				return;
			}
			
			// A zero length record indicates the end of that form
			// of data. When it is received, the data can then be
			// examined and worked on.
			
			data = (byte []) parameterData.ToArray (typeof (byte));
			
			// Read name/value pairs from the data.
			int index = 0;
			NameValuePair pair;
			while (NameValuePair.TryParse (data, ref index, out pair))
			{
				if (parameterTable.ContainsKey (pair.Name))
					Logger.Write (LogLevel.Warning, "A duplicate name/value pair was detected.");
				else
					parameterTable.Add (pair.Name, pair.Value);
			}
			
			// The parameter data is no longer needed and can be
			// sent to the garbage collector.
			parameterData = null;
			
			// Inform listeners of the completion.
			if (ParameterDataCompleted != null)
				ParameterDataCompleted (this);
		}
		
		// The GetParameter and GetParameters methods are analogous to
		// Environment.GetEnvironmentVariable and
		// Environment.GetEnvironmentVariables.
		
		public string GetParameter (string parameter)
		{
			if (parameterTable.ContainsKey (parameter))
				return (string) parameterTable [parameter];
			
			return null;
		}
		
		public IDictionary GetParameters ()
		{
			return parameterTable;
		}
		#endregion
		
		
		
		#region Standard Input Handling
		
		// Input is read by responders and filters. It represents post
		// data accompanying a request.
		
		protected event DataReceivedHandler  InputDataReceived;
		protected event DataCompletedHandler InputDataCompleted;
		
		private bool inputDataCompleted = false;
		
		public void AddInputData (byte [] data)
		{
			// Validate arguments in public methods.
			if (data == null)
				throw new ArgumentNullException ("data");
			
			// There should be no data following a zero byte record.
			if (inputDataCompleted)
				throw new Exception ("Data already completed.");
			
			if (data.Length > 0)
			{
				// Inform listeners of the data.
				if (InputDataReceived != null)
					InputDataReceived (this, data);
			}
			else
			{
				// Inform listeners of the completion.
				inputDataCompleted = true;
				if (InputDataCompleted != null)
					InputDataCompleted (this);
			}
		}
		#endregion
		
		
		
		#region File Data Handling
		
		// File data is read by filters. It represents the contents of a
		// requested file that are to be filtered.
		
		protected event DataReceivedHandler  FileDataReceived;
		protected event DataCompletedHandler FileDataCompleted;
		
		private bool fileDataCompleted = false;
		
		public void AddFileData (byte [] data)
		{
			// Validate arguments in public methods.
			if (data == null)
				throw new ArgumentNullException ("data");
			
			// There should be no data following a zero byte record.
			if (fileDataCompleted)
				throw new Exception ("Data already completed.");
			
			if (data.Length > 0)
			{
				// Inform listeners of the data.
				if (FileDataReceived != null)
					FileDataReceived (this, data);
			}
			else
			{
				// Inform listeners of the completion.
				fileDataCompleted = true;
				if (FileDataCompleted != null)
					FileDataCompleted (this);
			}
		}
		#endregion
		
		
		
		#region Standard Output Handling
		
		// Output provides the contents of the web page or file,
		// including HTTP headers.
		
		bool stdoutSent = false;
		
		public void SendOutputData (byte [] data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			if (data.Length == 0)
				return;
			
			stdoutSent = true;
			
			SendStreamData (RecordType.StandardOutput, data);
		}
		
		public void SendOutputText (string text)
		{
			SendOutputText (text, System.Text.Encoding.UTF8);
		}
		
		public void SendOutputText (string text, System.Text.Encoding encoding)
		{
			SendOutputData (encoding.GetBytes (text));
		}
		#endregion
		
		
		
		#region Standard Error Handling
		
		// Error provides error text to the HTTP server.
		
		bool stderrSent = false;
		
		public void SendErrorData (byte [] data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			if (data.Length == 0)
				return;
			
			stderrSent = true;
			
			SendStreamData (RecordType.StandardError, data);
		}
		
		public void SendErrorText (string text)
		{
			SendErrorText (text, System.Text.Encoding.UTF8);
		}
		
		public void SendErrorText (string text, System.Text.Encoding encoding)
		{
			SendErrorData (encoding.GetBytes (text));
		}
		#endregion
		
		
		
		#region Private Methods
		private void SendStreamData (RecordType type, byte [] data)
		{
			// Records are only able to hold 65535 bytes of data. If
			// larger data is to be sent, it must be broken into
			// smaller components.
			
			if (data.Length <= 0xFFFF)
				connection.SendResponse (type, requestID, data);
			else
			{
				int index = 0;
				byte [] data_part = new byte [0xFFFF];
				while (index < data.Length)
				{
					int length = (index + 0xFFFF < data.Length) ? 0xFFFF : (data.Length - index);
					if (length != data_part.Length)
						data_part = new byte [length];
					
					Array.Copy (data, index, data_part, 0, length);
					connection.SendResponse (type, requestID, data_part);
				}
			}
		}
		#endregion
	}
}
