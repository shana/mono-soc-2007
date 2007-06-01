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
	public enum RecordType : byte
	{
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
	
	/*
	From the FastCGI 1.1 specification.
	typedef struct {
            unsigned char version;
            unsigned char type;
            unsigned char requestIdB1;
            unsigned char requestIdB0;
            unsigned char contentLengthB1;
            unsigned char contentLengthB0;
            unsigned char paddingLength;
            unsigned char reserved;
            unsigned char contentData[contentLength];
            unsigned char paddingData[paddingLength];
	} FCGI_Record;
	*/
	
	public struct Record
	{
		public const int HeaderSize = 8;
		
		private byte   _version;
		private byte   _type;
		private ushort _request_id;
		private byte[] _content_data;
		
		public Record (Socket socket)
		{
			if (socket == null)
			{
				throw new ArgumentNullException ("socket");
			}
			
			byte[] buffer = new byte [HeaderSize];
			ushort content_length;
			byte   padding_length;
			byte[] padding_buffer;
			
			// Read the 8 byte record header. If 8 bytes aren't
			// read, the stream is corrupted and an exception is
			// thrown.
			if (socket.Receive (buffer, HeaderSize, System.Net.Sockets.SocketFlags.None) < HeaderSize)
			{
				throw new Exception ();
			}
			
			foreach(byte b in buffer)
				Console.Write (" " + b.ToString ("X2"));
			Console.Write ("\n");
			// Read the values from the data.
			_version       = buffer [0];
			_type          = buffer [1];
			_request_id    = ReadUInt16 (buffer, 2);
			content_length = ReadUInt16 (buffer, 4);
			padding_length = buffer [6];
			
			// Check that a valid type is given.
			if (_type == 0 || _type > 11)
				throw new Exception ();
			
			_content_data  = new byte [content_length];
			padding_buffer = new byte [padding_length];
			
			// Read the record data, and throw an exception if the
			// complete data cannot be read.
			if (content_length > 0)
			{
				if (socket.Receive (_content_data, content_length, SocketFlags.None) < content_length)
				{
					throw new Exception ();
				}
			}
			
			// Read the padding data, and throw an exception if the
			// complete padding cannot be read.
			if (padding_length > 0)
			{
				if (socket.Receive (padding_buffer, padding_length, SocketFlags.None) < padding_length)
				{
					throw new Exception ();
				}
			}
		}
		
		public byte       Version   {get {return _version;}}
		public RecordType Type      {get {return (RecordType) _type;}}
		public ushort     RequestID {get {return _request_id;}}
		public byte[]     Content   {get {return _content_data;}}
		
		public override string ToString ()
		{
			return "FastCGI Record:" +
			       "\n   Version:        " + Version +
			       "\n   Type:           " + Type +
			       "\n   Request ID:     " + RequestID +
			       "\n   Content Length: " + Content.Length;
		}
		
		public static void Send (Socket socket, byte version, RecordType type, ushort requestID, byte [] data)
		{
				Logger.Write (LogLevel.Notice, "3.4.1");
			if (data.Length > 0xFFFF)
				throw new ArgumentException ("Data is too large.", "data");
			
				Logger.Write (LogLevel.Notice, "3.4.2");
			int padding_size = (8 - (data.Length % 8)) % 8;
			
				Logger.Write (LogLevel.Notice, "3.4.3");
			ArrayList l = new ArrayList ();
				Logger.Write (LogLevel.Notice, "3.4.4");
			l.Add (version);
			l.Add ((byte) type);
			l.AddRange (WriteUInt16 (requestID));
			l.AddRange (WriteUInt16 ((ushort) data.Length));
			l.Add ((byte) padding_size);
			l.Add ((byte) 0);
			l.AddRange (data);
			l.AddRange (new byte [padding_size]);
				Logger.Write (LogLevel.Notice, "3.4.5");
			byte [] byte_data = (byte []) l.ToArray (typeof (byte));
			
				Logger.Write (LogLevel.Notice, "3.4.6");
			Logger.Write (LogLevel.Notice, "" + byte_data.Length);
			
				Logger.Write (LogLevel.Notice, "3.4.7");
			foreach(byte b in byte_data)
				Console.Write (" " + b.ToString ("X2"));
			Console.Write ("\n");
			
				Logger.Write (LogLevel.Notice, "3.4.8");
			socket.Send (byte_data);
				Logger.Write (LogLevel.Notice, "3.4.9");
		}
		
		internal static ushort ReadUInt16 (byte [] array, int arrayIndex)
		{
			ushort value = array [arrayIndex];
			value = (ushort) (value << 8);
			value += array [arrayIndex + 1];
			return value;
		}
		
		internal static byte[] WriteUInt16 (ushort value)
		{
			byte [] array = new byte [2];
			array [0] = (byte) (value >> 8);
			array [1] = (byte) (value & 0xFF);
			return array;
		}
	}
}
