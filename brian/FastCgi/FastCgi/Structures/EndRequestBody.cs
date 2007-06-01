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
	public enum ProtocolStatus : byte
	{
		RequestComplete          = 0,
		CantMultiplexConnections = 1,
		Overloaded               = 2,
		UnknownRole              = 3
	}
	
	public struct EndRequestBody
	{
		uint _appStatus;
		byte _protocolStatus;
		
		public EndRequestBody (int appStatus, ProtocolStatus protocolStatus)
		{
			unchecked
			{
				_appStatus      = (uint) appStatus;
				_protocolStatus = (byte) protocolStatus;
			}
		}
		
		public byte [] Data
		{
			get
			{
				byte [] data = new byte [8];
				data [0] = (byte)((_appStatus >> 24) & 0xFF);
				data [1] = (byte)((_appStatus >> 16) & 0xFF);
				data [2] = (byte)((_appStatus >>  8) & 0xFF);
				data [3] = (byte)((_appStatus      ) & 0xFF);
				data [4] = _protocolStatus;
				return data;
			}
		}
	}
}
