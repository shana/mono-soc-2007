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

namespace FastCgi
{
	public enum Role : ushort
	{
		Responder  = 1,
		Authorizer = 2,
		Filter     = 3
	}
	
	[Flags]
	public enum BeginRequestFlags : byte
	{
		None      = 0,
		KeepAlive = 1
	}
	
	public struct BeginRequestBody
	{
		ushort _role;
		byte   _flags;
		public BeginRequestBody(Record record)
		{
			if (record.Content.Length != 8)
				throw new ArgumentException ("8 bytes expected.", "record");
			
			_role  = Record.ReadUInt16 (record.Content, 0);
			_flags = record.Content [2];
		}
		
		public Role Role {get {return (Role) _role;}}
		public BeginRequestFlags Flags {get {return (BeginRequestFlags) _flags;}}
	}
}
