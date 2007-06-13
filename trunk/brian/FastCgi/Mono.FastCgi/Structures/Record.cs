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
	public enum RecordType : byte {
		None            =  0,
		BeginRequest    =  1,
		AbortRequest    =  2,
		EndRequest      =  3,
		Params          =  4,
		StandardInput   =  5,
		StandardOutput  =  6,
		StandardError   =  7,
		Data            =  8,
		GetValues       =  9,
		GetValuesResult = 10,
		UnknownType     = 11
	}
	
	public struct Record
	{
		#region Private Properties
		private byte       version;
		private RecordType type;
		private ushort     request_id;
		private byte[]     body_data;
		#endregion
		
		
		#region Public Fields
		public const int HeaderSize = 8;
		#endregion
		
		
		#region Constructors
		public Record (ISocketAbstraction socket)
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
				throw new Exception ();
			
			// Check that a valid type is given.
			if (buffer [1] == 0 || buffer [1] > 11)
				throw new Exception ();
			
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
			if (body_length > 0) {
				if (socket.Receive (body_data) < body_length)
					throw new Exception ();
			}
			
			// Read the padding data, and throw an exception if the
			// complete padding cannot be read.
			if (padding_length > 0) {
				if (socket.Receive (padding_buffer) < padding_length)
					throw new Exception ();
			}
		}
		
		public Record (byte version, RecordType type, ushort requestID,
		               byte [] bodyData)
		{
			this.version    = version;
			this.type       = type;
			this.request_id = requestID;
			this.body_data  = bodyData;
		}
		#endregion
		
		
		#region Public Properties
		public byte Version {
			get {return version;}
		}
		
		public RecordType Type {
			get {return type;}
		}
		
		public ushort RequestID {
			get {return request_id;}
		}
		
		public byte[] Body {
			get {return body_data;}
		}
		#endregion
		
		
		#region Public Methods
		public override string ToString ()
		{
			return "FastCGI Record:" +
			       "\n   Version:        " + Version +
			       "\n   Type:           " + Type +
			       "\n   Request ID:     " + RequestID +
			       "\n   Content Length: " + Body.Length;
		}
		
		public void Send (ISocketAbstraction socket)
		{
			if (body_data.Length > 0xFFFF)
				throw new ArgumentException
				("Data is too large.", "data");
			
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
		internal static ushort ReadUInt16 (byte [] array, int arrayIndex)
		{
			ushort value = array [arrayIndex];
			value = (ushort) (value << 8);
			value += array [arrayIndex + 1];
			return value;
		}
		#endregion
	}
}
