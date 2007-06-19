//
// Record.cs: Handles sending and receiving FastCGI records via sockets.
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
	///    Specifies the type of information contained in the record.
	/// </summary>
	public enum RecordType : byte {
		/// <summary>
		///    No record type specified.
		/// </summary>
		None            =  0,
		
		/// <summary>
		///    The record contains the beginning of a request. Sent by
		///    the client.)
		/// </summary>
		BeginRequest    =  1,
		
		/// <summary>
		///    The record is informing that a request has been aborted.
		///    (Sent by the client.)
		/// </summary>
		AbortRequest    =  2,
		
		/// <summary>
		///    The record contains the end of a request. (Sent by the
		///    server.)
		/// </summary>
		EndRequest      =  3,
		
		/// <summary>
		///    The record contains the parameters for a request. (Sent 
		///    by the client.)
		/// </summary>
		Params          =  4,
		
		/// <summary>
		///    The record contains standard input for a request. (Sent 
		///    by the client.)
		/// </summary>
		StandardInput   =  5,
		
		/// <summary>
		///    The record contains standard output for a request. (Sent 
		///    by the server.)
		/// </summary>
		StandardOutput  =  6,
		
		/// <summary>
		///    The record contains standard error for a request. (Sent 
		///    by the server.)
		/// </summary>
		StandardError   =  7,
		
		/// <summary>
		///    The record contains file contents for a request. (Sent 
		///    by the client.)
		/// </summary>
		Data            =  8,
		
		/// <summary>
		///    The record contains a request for server values. (Sent 
		///    by the client.)
		/// </summary>
		GetValues       =  9,
		
		/// <summary>
		///    The record contains a server values. (Sent by the
		///    server.)
		/// </summary>
		GetValuesResult = 10,
		
		/// <summary>
		///    The record contains a notice of failure to recognize a
		///    record type. (Sent by the server.)
		/// </summary>
		UnknownType     = 11
	}
	
	/// <summary>
	///    This struct sends and receives FastCGI records.
	/// </summary>
	public struct Record
	{
		#region Private Fields
		
		/// <summary>
		///    Contains the FastCGI version.
		/// </summary>
		private byte version;
		
		/// <summary>
		///    Contains the record type.
		/// </summary>
		private RecordType type;
		
		/// <summary>
		///    Contains the request ID.
		/// </summary>
		private ushort request_id;
		
		/// <summary>
		///    Contains the body data.
		/// </summary>
		private byte [] body_data;
		
		#endregion
		
		
		
		#region Public Fields
		
		/// <summary>
		///    The size of a FastCGI record header.
		/// </summary>
		public const int HeaderSize = 8;
		
		#endregion
		
		
		
		#region Constructors
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="Record" /> by reading the contents from a specified
		///    socket.
		/// </summary>
		/// <param name="socket">
		///    A <see cref="Socket" /> object to receive the record data
		///    from.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="socket" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="ArgumentException">
		///    <paramref name="socket" /> does not contain a complete
		///    record.
		/// </exception>
		public Record (Socket socket)
		{
			if (socket == null)
				throw new ArgumentNullException ("socket");
			
			byte[] buffer = new byte [HeaderSize];
			ushort body_length;
			byte   padding_length;
			byte[] padding_buffer;
			
			// Read the 8 byte record header. If 8 bytes aren't
			// read, the stream is corrupted and an exception is
			// thrown.
			if (socket.Receive (buffer) < HeaderSize)
				throw new ArgumentException (
					"Socket does not contain sufficient data.",
					"socket");
			
			// Read the values from the data.
			version        = buffer [0];
			type           = (RecordType) buffer [1];
			request_id     = ReadUInt16 (buffer, 2);
			body_length    = ReadUInt16 (buffer, 4);
			padding_length = buffer [6];
			
			body_data  = new byte [body_length];
			padding_buffer = new byte [padding_length];
			
			// Read the record data, and throw an exception if the
			// complete data cannot be read.
			if (body_length > 0 && socket.Receive (body_data) < 
				body_length)
				throw new ArgumentException (
					"Socket does not contain sufficient data.",
					"socket");
			
			// Read the padding data, and throw an exception if the
			// complete padding cannot be read.
			if (padding_length > 0 && socket.Receive (
				padding_buffer) < padding_length)
				throw new ArgumentException (
					"Socket does not contain sufficient data.",
					"socket");
		}
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="Record" /> populating it with a specified version,
		///    type, ID, and body.
		/// </summary>
		/// <param name="version">
		///    A <see cref="byte" /> containing the FastCGI version the
		///    record is structured for.
		/// </param>
		/// <param name="type">
		///    A <see cref="RecordType" /> containing the type of
		///    record to create.
		/// </param>
		/// <param name="requestID">
		///    A <see cref="ushort" /> containing the ID of the request
		///    associated with the new record.
		/// </param>
		/// <param name="bodyData">
		///    A <see cref="byte[]" /> containing the contents to use
		///    in the new record.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="bodyData" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="ArgumentException">
		///    <paramref name="bodyData" /> contains more than 65535
		///    bytes and cannot be sent.
		/// </exception>
		public Record (byte version, RecordType type, ushort requestID,
		               byte [] bodyData)
		{
			if (bodyData == null)
				throw new ArgumentNullException ("data");
			if (bodyData.Length > 0xFFFF)
				throw new ArgumentException
				("Data is too large.", "data");
			
			this.version    = version;
			this.type       = type;
			this.request_id = requestID;
			this.body_data  = bodyData;
		}
		
		#endregion
		
		
		
		#region Public Properties
		
		/// <summary>
		///    Gets the FastCGI version of the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="byte" /> containing the FastCGI version of
		///    the current instance.
		/// </value>
		public byte Version {
			get {return version;}
		}
		
		/// <summary>
		///    Gets the FastCGI record type of the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="byte" /> containing the FastCGI record type
		///    of the current instance.
		/// </value>
		public RecordType Type {
			get {return type;}
		}
		
		/// <summary>
		///    Gets the ID of the request associated with the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="byte" /> containing the ID of the request
		///    associated with the current instance.
		/// </value>
		public ushort RequestID {
			get {return request_id;}
		}
		
		/// <summary>
		///    Gets the body data of with the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="byte" /> containing the body data of the
		///    current instance.
		/// </value>
		public byte[] Body {
			get {return body_data;}
		}
		
		#endregion
		
		
		
		#region Public Methods
		
		/// <summary>
		///    Creates and returns a <see cref="string" />
		///    representation of the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> representation of the current
		///    instance.
		/// </value>
		public override string ToString ()
		{
			return "FastCGI Record:" +
			       "\n   Version:        " + Version +
			       "\n   Type:           " + Type +
			       "\n   Request ID:     " + RequestID +
			       "\n   Content Length: " + Body.Length;
		}
		
		/// <summary>
		///    Sends a FastCGI record with the data from the current
		///    instance over a given socket.
		/// </summary>
		/// <param name="socket">
		///    A <see cref="Socket" /> object to send the data over.
		/// </param>
		public void Send (Socket socket)
		{
			ushort body_length  = (ushort) body_data.Length;
			byte   padding_size = (byte) ((8 - (body_length % 8)) % 8);
			
			byte [] data = new byte [8 + body_length + padding_size];
			data [0] = version;
			data [1] = (byte) type;
			data [2] = (byte) (request_id >> 8);
			data [3] = (byte) (request_id & 0xFF);
			data [4] = (byte) (body_length >> 8);
			data [5] = (byte) (body_length & 0xFF);
			data [6] = padding_size;
			
			body_data.CopyTo (data, 8);
			socket.Send (data);
		}
		
		#endregion
		
		
		
		#region Internal Static Methods
		
		/// <summary>
		///    Reads two bytes of data from an array and returns the
		///    appropriate value.
		/// </summary>
		/// <param name="array">
		///    A <see cref="byte[]" /> containing an array of data to
		///    read from.
		/// </param>
		/// <param name="arrayIndex">
		///    A <see cref="int" /> specifying the index in the array at
		///    which to start reading.
		/// </param>
		internal static ushort ReadUInt16 (byte [] array,
		                                   int arrayIndex)
		{
			ushort value = array [arrayIndex];
			value = (ushort) (value << 8);
			value += array [arrayIndex + 1];
			return value;
		}
		
		#endregion
	}
}