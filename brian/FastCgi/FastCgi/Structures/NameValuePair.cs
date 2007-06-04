//
// NameValuePair.cs: Handles the parsing of FastCGI name/value pairs.
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
using System.Text;

namespace FastCgi {
	public struct NameValuePair {
		
		#region Private Properties
		string name;
		string value;
		#endregion
		
		
		#region Public Fields
		public static readonly NameValuePair Empty = new NameValuePair
			(null, null);
		#endregion
		
		
		#region Constructors
		public NameValuePair (string name, string value)
		{
			this.name  = name;
			this.value = value;
		}
		
		public NameValuePair (byte [] data, ref int index)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			int name_length  = ReadLength (data, ref index);
			int value_length = ReadLength (data, ref index);
			
			if (index + name_length + value_length > data.Length)
				throw new ArgumentOutOfRangeException ("index");
			
			this.name = Encoding.Default.GetString (data, index,
				name_length);
			index += name_length;
			
			this.value = Encoding.Default.GetString (data, index,
				value_length);
			index += value_length;
		}
		#endregion
		
		
		#region Public Properties
		public string Name {
			get {return name;}
		}
		
		public string Value {
			get {return value;}
		}
		#endregion
		
		
		#region Private Static Methods
		private static int ReadLength (byte [] data, ref int index)
		{
			if (index >= data.Length)
				throw new ArgumentOutOfRangeException ("index");
			
			if (data [index] < 0x80)
				return data [index++];
			
			if (index + 4 >= data.Length)
				throw new ArgumentOutOfRangeException ("index");
			
			return (0x7F & (int) data [index++]) << 24
				+ ((int) data [index++]) << 16
				+ ((int) data [index++]) <<  8
				+ ((int) data [index++]);
		}
		#endregion
	}
}
