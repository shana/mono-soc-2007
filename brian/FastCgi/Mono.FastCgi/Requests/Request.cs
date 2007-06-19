//
// Requests/Request.cs: Handles FastCGI requests.
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
	public class Request
	{
		#region Private Fields
		
		/// <summary>
		///    Contains the request ID to use when sending data.
		/// </summary>
		private ushort     requestID;
		
		/// <summary>
		///    Contains the <see cref="Connection" /> object object from
		///    which data is received and to which data will be sent.
		/// </summary>
		private Connection connection;
		
		#endregion
		
		
		
		#region Constructors
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="Request" /> with the specified request ID and
		///    connection.
		/// </summary>
		/// <param name="requestID">
		///    A <see cref="ushort" /> containing the request ID of the
		///    new instance.
		/// </param>
		/// <param name="connection">
		///    A <see cref="Connection" /> object from which data is
		///    received and to which data will be sent.
		/// </param>
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
		
		/// <summary>
		///    Completes the request by closing any opened response and
		///    error streams and sending the EndRequest record to the
		///    client.
		/// </summary>
		/// <param name="appStatus">
		///    <para>A <see cref="int" /> containing the application
		///    status the request ended with.</para>
		///    <para>This is the same value as would be returned by a
		///    program on termination. On successful termination, this
		///    would be zero.</para>
		/// </param>
		/// <param name="protocolStatus">
		///    A <see cref="ProtocolStatus" /> containing the FastCGI
		///    protocol status with which the request is being ended.
		/// </param>
		/// <remarks>
		///    To close the request, this method calls <see
		///    cref="Connection.EndRequest" />, which additionally
		///    releases the resources so they can be garbage collected.
		/// </remarks>
		public void CompleteRequest (int appStatus,
		                             ProtocolStatus protocolStatus)
		{
			// Data is no longer needed.
			DataNeeded = false;
			
			// Close the standard output if it was opened.
			if (stdout_sent)
				SendStreamData (RecordType.StandardOutput,
					new byte [0], 0);
			
			// Close the standard error if it was opened.
			if (stderr_sent)
				SendStreamData (RecordType.StandardError,
					new byte [0], 0);
			
			connection.EndRequest (requestID, appStatus,
				protocolStatus);
		}
		
		/// <summary>
		///    Aborts the request by sending a message on the error
		///    stream, logging it, and completing the request with an
		///    application status of -1.
		/// </summary>
		/// <param name="message">
		///    A <see cref="string" /> containing the error message.
		/// </param>
		public void AbortRequest (string message)
		{
			SendError (message);
			Logger.Write (LogLevel.Error,
				"Aborting request. Reason follows:");
			Logger.Write (LogLevel.Error, message);
			CompleteRequest (-1, ProtocolStatus.RequestComplete);
		}
		
		/// <summary>
		///    Indicates whether or not data is needed by the current
		///    instance.
		/// </summary>
		private bool data_needed = true;
		
		/// <summary>
		///    Gets and sets whether or not data is still needed by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="bool" /> indicating whether or not data is
		///    still needed by the current instance.
		/// </value>
		/// <remarks>
		///    This value is used by the connection to determine whether
		///    or not it still needs to receive data for the request
		///    from the socket. As soon as a request has received all
		///    the necessary data, it should set the value to <see
		///    langword="false" /> so the connection can continue on
		///    with its next task.
		/// </remarks>
		public bool DataNeeded {
			get {return data_needed;}
			protected set {data_needed = value;}
		}
		
		/// <summary>
		///    Gets the server that spawned the connection used by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="FastCgi.Server" /> containing the server
		///    that spawned the connection used by the current instance.
		/// </value>
		public Server Server {
			get {return connection.Server;}
		}
		
		/// <summary>
		///    Gets the request ID of the current instance as used by
		///    the connection.
		/// </summary>
		/// <value>
		///    A <see cref="ushort" /> containing the request ID of the
		///    current instance.
		/// </value>
		public ushort RequestID {
			get {return requestID;}
		}
		
		/// <summary>
		///    Gets whether or not the connection used by the current
		///    instance is connected.
		/// </summary>
		/// <value>
		///    A <see cref="bool" /> indicating whether or not the
		///    connection used by the current instance is connected.
		/// </value>
		/// <remarks>
		///    If the connection is not connected, any response data is
		///    disregarded. As such, before any intense operation, this
		///    value should be checked as to avoid any unneccessary
		///    work.
		/// </remarks>
		public bool IsConnected {
			get {return connection.IsConnected;}
		}
		#endregion
		
		
		
		#region Parameter Handling
		
		/// <summary>
		///    This event is called when the parameter data has been
		///    completely read and parsed by the current instance.
		/// </summary>
		protected event EventHandler ParameterDataCompleted;
		
		/// <summary>
		///    Contains the paramter data as it is received from the
		///    server. Upon completion, the parameter data is parsed and
		///    the value is set to <see langword="null" />.
		/// </summary>
		ArrayList parameter_data = new ArrayList ();
		
		/// <summary>
		///    Contains the name/value pairs as they have been parsed.
		/// </summary>
		IDictionary parameter_table = null;
		
		/// <summary>
		///    Adds a block of FastCGI parameter data to the current
		///    instance.
		/// </summary>
		/// <param name="data">
		///    A <see cref="byte[]" /> containing a chunk of parameter
		///    data.
		/// </param>
		/// <remarks>
		///    <para>In the standard FastCGI method, if the data
		///    received has a length of zero, the parameter data has
		///    been completed and the the data will be parsed. At that
		///    point <see cref="ParameterDataCompleted" /> will be
		///    called.</para>
		///    <para>If an exception is encountered while parsing the
		///    parameters, the request will be aborted.</para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="data" /> is <see langref="null" />.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		///    The parameter data has already been completed and parsed.
		/// </exception>
		public void AddParameterData (byte [] data)
		{
			// Validate arguments in public methods.
			if (data == null)
				throw new ArgumentNullException ("data");
			
			// When all the parameter data is received, it is acted
			// on and the _parameter_data object is nullified.
			// Further data suggests a problem with the HTTP server.
			if (parameter_data == null)
				throw new InvalidOperationException (
					"Data already completed.");
			
			// If data was provided, append it to that already
			// received, and exit.
			if (data.Length > 0) {
				parameter_data.AddRange (data);
				return;
			}
			
			// A zero length record indicates the end of that form
			// of data. When it is received, the data can then be
			// examined and worked on.
			
			data = (byte []) parameter_data.ToArray (typeof (byte));
			
			try {
				parameter_table = NameValuePair.FromData (data);
				// The parameter data is no longer needed and
				// can be sent to the garbage collector.
				parameter_data = null;
			
				// Inform listeners of the completion.
				if (ParameterDataCompleted != null)
					ParameterDataCompleted (this,
						EventArgs.Empty);
			} catch {
				AbortRequest ("Error parsing parameters.");
			}
			
		}
		
		/// <summary>
		///    Gets a parameter with a specified name.
		/// </summary>
		/// <param name="parameter">
		///    A <see cref="string" /> containing a parameter namte to
		///    find in current instance.
		/// </param>
		/// <returns>
		///    A <see cref="string" /> containing the parameter with the
		///    specified name, or <see langref="null" /> if it was not
		///    found.
		/// </returns>
		/// <remarks>
		///    This method is analogous to <see
		///    cref="System.Environment.GetEnvironmentVariable" /> as
		///    FastCGI parameters represent environment variables that
		///    would be passed to a CGI/1.1 program.
		/// </remarks>
		public string GetParameter (string parameter)
		{
			if (parameter_table != null &&
				parameter_table.Contains (parameter))
				return (string) parameter_table [parameter];
			
			return null;
		}
		
		/// <summary>
		///    Gets all parameter contained in the current instance.
		/// </summary>
		/// <returns>
		///    A <see cref="IDictionary" /> containing all the
		///    parameters contained in the current instance.
		/// </returns>
		/// <remarks>
		///    This method is analogous to <see
		///    cref="System.Environment.GetEnvironmentVariables" /> as
		///    FastCGI parameters represent environment variables that
		///    would be passed to a CGI/1.1 program.
		/// </remarks>
		public IDictionary GetParameters ()
		{
			return parameter_table;
		}
		#endregion
		
		
		
		#region Standard Input Handling
		
		/// <summary>
		///    This event is called when standard input data has been
		///    received by the current instance.
		/// </summary>
		/// <remarks>
		///    Input data is analogous to standard input in CGI/1.1
		///    programs and contains post data from the HTTP request.
		/// </remarks>
		protected event DataReceivedHandler  InputDataReceived;
		
		/// <summary>
		///    Indicates whether or not the standard input data has
		///    been completely read by the current instance.
		/// </summary>
		private bool input_data_completed = false;
		
		/// <summary>
		///    Adds a block of standard input data to the current
		///    instance.
		/// </summary>
		/// <param name="data">
		///    A <see cref="byte[]" /> containing a chunk of input
		///    data.
		/// </param>
		/// <remarks>
		///    <para>Input data is analogous to standard input in
		///    CGI/1.1 programs and contains post data from the HTTP
		///    request.</para>
		///    <para>When data is received, <see
		///    cref="InputDataReceived" /> is called.</para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="data" /> is <see langref="null" />.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		///    The input data has already been completed.
		/// </exception>
		public void AddInputData (byte [] data)
		{
			// Validate arguments in public methods.
			if (data == null)
				throw new ArgumentNullException ("data");
			
			// There should be no data following a zero byte record.
			if (input_data_completed)
				throw new InvalidOperationException (
					"Data already completed.");
			
			if (data.Length == 0)
				input_data_completed = true;
			
			// Inform listeners of the data.
			if (InputDataReceived != null)
				InputDataReceived (this, new DataReceivedArgs (data));
		}
		#endregion
		
		
		
		#region File Data Handling
		
		/// <summary>
		///    This event is called when file data has been received by
		///    the current instance.
		/// </summary>
		/// <remarks>
		///    File data send for the FastCGI filter role and contains
		///    the contents of the requested file to be filtered.
		/// </remarks>
		protected event DataReceivedHandler  FileDataReceived;
		
		/// <summary>
		///    Indicates whether or not the file data has been
		///    completely read by the current instance.
		/// </summary>
		private bool file_data_completed = false;
		
		/// <summary>
		///    Adds a block of file data to the current instance.
		/// </summary>
		/// <param name="data">
		///    A <see cref="byte[]" /> containing a chunk of file data.
		/// </param>
		/// <remarks>
		///    <para>File data send for the FastCGI filter role and
		///    contains the contents of the requested file to be
		///    filtered.</para>
		///    <para>When data is received, <see
		///    cref="FileDataReceived" /> is called.</para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="data" /> is <see langref="null" />.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		///    The file data has already been completed.
		/// </exception>
		public void AddFileData (byte [] data)
		{
			// Validate arguments in public methods.
			if (data == null)
				throw new ArgumentNullException ("data");
			
			// There should be no data following a zero byte record.
			if (file_data_completed)
				throw new Exception ("Data already completed.");
			
			if (data.Length == 0)
				file_data_completed = true;
			
			// Inform listeners of the data.
			if (FileDataReceived != null)
				FileDataReceived (this, new DataReceivedArgs (data));
		}
		#endregion
		
		
		
		#region Standard Output Handling
		
		/// <summary>
		///    Indicates whether or not output data has been sent.
		/// </summary>
		bool stdout_sent = false;
		
		/// <summary>
		///    Sends a specified number of bytes of standard output
		///    data.
		/// </summary>
		/// <param name="data">
		///    A <see cref="byte[]" /> containing output data to send.
		/// </param>
		/// <param name="length">
		///    A <see cref="int" /> containing the number of bytes of
		///    <paramref name="data" /> to send.
		/// </param>
		/// <remarks>
		///    <para>FastCGI output data is analogous to CGI/1.1
		///    standard output data.</para>
		/// </remarks>
		public void SendOutput (byte [] data, int length)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			if (data.Length == 0)
				return;
			
			stdout_sent = true;
			
			SendStreamData (RecordType.StandardOutput, data, length);
		}
		
		/// <summary>
		///    Sends standard output data.
		/// </summary>
		/// <param name="data">
		///    A <see cref="byte[]" /> containing output data to send.
		/// </param>
		/// <remarks>
		///    <para>FastCGI output data is analogous to CGI/1.1
		///    standard output data.</para>
		///    <para>To send text, use <see
		///    cref="SendOutput(string,System.Text.Encoding)" />.</para>
		///    <para>To send only the beginning of a <see
		///    cref="byte[]" /> (as in the case of buffers), use <see
		///    cref="SendOutput(byte[],int)" />.</para>
		/// </remarks>
		public void SendOutput (byte [] data)
		{
			SendOutput (data, data.Length);
		}
		
		/// <summary>
		///    Sends standard outpu text in UTF-8 encoding.
		/// </summary>
		/// <param name="text">
		///    A <see cref="string" /> containing text to send.
		/// </param>
		/// <remarks>
		///    <para>FastCGI output data is analogous to CGI/1.1
		///    standard output data.</para>
		///    <para>To specify the text encoding, use <see
		///    cref="SendOutput(string,System.Text.Encoding)" />.</para>
		/// </remarks>
		public void SendOutputText (string text)
		{
			SendOutput (text, System.Text.Encoding.UTF8);
		}
		
		/// <summary>
		///    Sends standard output text in a specified encoding.
		/// </summary>
		/// <param name="text">
		///    A <see cref="string" /> containing text to send.
		/// </param>
		/// <param name="encoding">
		///    A <see cref="System.Text.Encoding" /> containing a
		///    encoding to use when converting the text.
		/// </param>
		/// <remarks>
		///    <para>FastCGI output data is analogous to CGI/1.1
		///    standard output data.</para>
		/// </remarks>
		public void SendOutput (string text, System.Text.Encoding encoding)
		{
			SendOutput (encoding.GetBytes (text));
		}
		#endregion
		
		
		
		#region Standard Error Handling
		
		/// <summary>
		///    Indicates whether or not error data has been sent.
		/// </summary>
		bool stderr_sent = false;
		
		/// <summary>
		///    Sends a specified number of bytes of standard error data.
		/// </summary>
		/// <param name="data">
		///    A <see cref="byte[]" /> containing error data to send.
		/// </param>
		/// <param name="length">
		///    A <see cref="int" /> containing the number of bytes of
		///    <paramref name="data" /> to send.
		/// </param>
		/// <remarks>
		///    <para>FastCGI error data is analogous to CGI/1.1 standard
		///    error data.</para>
		/// </remarks>
		public void SendError (byte [] data, int length)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			if (data.Length == 0)
				return;
			
			stderr_sent = true;
			
			SendStreamData (RecordType.StandardError, data, length);
		}
		
		/// <summary>
		///    Sends standard error data.
		/// </summary>
		/// <param name="data">
		///    A <see cref="byte[]" /> containing error data to send.
		/// </param>
		/// <remarks>
		///    <para>FastCGI error data is analogous to CGI/1.1 standard
		///    error data.</para>
		///    <para>To send text, use <see
		///    cref="SendError(string,System.Text.Encoding)" />.</para>
		///    <para>To send only the beginning of a <see
		///    cref="byte[]" /> (as in the case of buffers), use <see
		///    cref="SendError(byte[],int)" />.</para>
		/// </remarks>
		public void SendError (byte [] data)
		{
			SendError (data, data.Length);
		}
		
		/// <summary>
		///    Sends standard error text in UTF-8 encoding.
		/// </summary>
		/// <param name="text">
		///    A <see cref="string" /> containing text to send.
		/// </param>
		/// <remarks>
		///    <para>FastCGI error data is analogous to CGI/1.1 standard
		///    error data.</para>
		///    <para>To specify the text encoding, use <see
		///    cref="SendError(string,System.Text.Encoding)" />.</para>
		/// </remarks>
		public void SendError (string text)
		{
			SendError (text, System.Text.Encoding.UTF8);
		}
		
		/// <summary>
		///    Sends standard error text in a specified encoding.
		/// </summary>
		/// <param name="text">
		///    A <see cref="string" /> containing text to send.
		/// </param>
		/// <param name="encoding">
		///    A <see cref="System.Text.Encoding" /> containing a
		///    encoding to use when converting the text.
		/// </param>
		/// <remarks>
		///    <para>FastCGI error data is analogous to CGI/1.1 standard
		///    error data.</para>
		/// </remarks>
		public void SendError (string text,
		                       System.Text.Encoding encoding)
		{
			SendError (encoding.GetBytes (text));
		}
		#endregion
		
		
		
		#region Private Methods
		
		/// <summary>
		///    Sends a block of data with a specified length to the
		///    client in a specified type of record, splitting it into
		///    smaller records if the data is too large.
		/// </summary>
		/// <param name="type">
		///    A <see cref="RecordType" /> containing the type of
		///    record to send the data in.
		/// </param>
		/// <param name="data">
		///    A <see cref="byte[]" /> containing the data to send.
		/// </param>
		/// <param name="length">
		///    A <see cref="int" /> containing the length of the data to
		///    send. If greater than the length of <paramref
		///    name="data" />, it will decreased to that size.
		/// </param>
		private void SendStreamData (RecordType type, byte [] data,
		                             int length)
		{
			// Records are only able to hold 65535 bytes of data. If
			// larger data is to be sent, it must be broken into
			// smaller components.
			
			if (length > data.Length)
				length = data.Length;
			
			if (length == data.Length && length < 0xFFFF)
				connection.SendRecord (type, requestID, data);
			else
			{
				int index = 0;
				byte [] data_part = new byte [System.Math.Min (
					0xFFFF, length)];
				while (index < length)
				{
					int chunk_length = (index + 0xFFFF <
						length) ? 0xFFFF : (length - index);
					if (chunk_length != data_part.Length)
						data_part = new byte [chunk_length];
					
					Array.Copy (data, index, data_part, 0,
						chunk_length);
					connection.SendRecord (type, requestID,
						data_part);
				}
			}
		}
		#endregion
	}
	
	/// <summary>
	///    This delegate is used for notification that data has been
	///    received, typically by <see cref="Request" />.
	/// </summary>
	/// <param name="sender">
	///    A <see cref="Request" /> object that sent the event.
	/// </param>
	/// <param name="args">
	///    A <see cref="DataReceivedArgs" /> object containing the arguments
	///    for the event.
	/// </param>
	public delegate void DataReceivedHandler  (Request sender,
	                                           DataReceivedArgs args);
	
	/// <summary>
	///    This class extends <see cref="EventArgs" /> and provides
	///    arguments for the event that data is received.
	/// </summary>
	public class DataReceivedArgs : EventArgs
	{
		/// <summary>
		///    Contains the data that was received.
		/// </summary>
		private byte [] data;
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="DataReceivedArgs" /> with the specified data.
		/// </summary>
		/// <param name="data">
		///    A <see cref="byte[]" /> containing the data that was
		///    received.
		/// </param>
		/// <exception name="ArgumentNullException">
		///    <paramref name="data" /> is <see langref="null" />.
		/// </exception>
		public DataReceivedArgs (byte [] data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			this.data = data;
		}
		
		/// <summary>
		///    Gets whether or not the data has been completed.
		/// </summary>
		/// <value>
		///    A <see cref="bool" /> indicating whether or not the data
		///    has been completed.
		/// </value>
		/// <remarks>
		///    Data completeness means that this is that last event
		///    of this type coming from the sender. It is the standard
		///    FastCGI test equivalent to <c><I>args</I>.Data.Length ==
		///    0</c>.
		/// </remarks>
		public bool DataCompleted {
			get {return data.Length == 0;}
		}
		
		/// <summary>
		///    Gets the data that was received.
		/// </summary>
		/// <value>
		///    A <see cref="byte[]" /> containing the data that was
		///    received.
		/// </value>
		public byte [] Data {
			get {return data;}
		}
		
	}
}
