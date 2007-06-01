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
using System.Text;

namespace FastCgi
{
	public struct NameValuePair
	{
		string _name;
		string _value;
		
		public static readonly NameValuePair Empty = new NameValuePair (null, null);
		
		public NameValuePair (string name, string value)
		{
			_name = name;
			_value = value;
		}
		
		public string Name  {get {return _name;}}
		public string Value {get {return _value;}}
		
		public static bool TryParse (byte [] data, ref int index, out NameValuePair pair)
		{
			pair = Empty;
			
			int i = index;
			int name_length;
			int value_length;
			
			if (!TryReadLength (data, ref i, out name_length))
				return false;
			
			if (!TryReadLength (data, ref i, out value_length))
				return false;
			
			if (i + name_length + value_length > data.Length)
				return false;
			
			string name  = Encoding.ASCII.GetString (data, i, name_length);
			string value = Encoding.ASCII.GetString (data, i + name_length, value_length);
			Logger.Write (LogLevel.Notice, "PARAMS READ: " + name + "=" + value);
			pair = new NameValuePair (name, value);
			index = i + name_length + value_length;
			return true;
		}
		
		private static bool TryReadLength (byte [] data, ref int index, out int value)
		{
			value = 0;
			
			if (index >= data.Length)
				return false;
			
			if (data [index] < 0x80)
			{
				value = data [index++];
				return true;
			}
			
			if (index + 4 >= data.Length)
				return false;
			
			value  = (0x7F & (int) data [index++]) << 24;
			value +=        ((int) data [index++]) << 16;
			value +=        ((int) data [index++]) <<  8;
			value +=        ((int) data [index++]);
			
			return true;
		}
	}
}
