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
	public class ResponderRequest : Request
	{
		private ArrayList inputData = new ArrayList ();
		private Responder responder;
		
		public ResponderRequest (ushort requestID, Connection connection) : base (requestID, connection)
		{
			if (!IsSupported)
				throw new Exception ();
			
			responder = (Responder) Activator.CreateInstance(responderType, new object [] {this});
			
			InputDataReceived  += OnInputDataReceived;
			InputDataCompleted += OnInputDataCompleted;
		}
		
		public byte [] InputData
		{
			get
			{
				return (byte[]) inputData.ToArray (typeof (byte));
			}
		}
		
		private void OnInputDataReceived (Request sender, byte [] data)
		{
			inputData.AddRange (data);
		}
		
		private void OnInputDataCompleted (Request sender)
		{
			DataNeeded = false;
			int appStatus = responder.Process ();
			CompleteRequest (appStatus, ProtocolStatus.RequestComplete);
		}
		
		private static System.Type responderType = null;
		public static void SetResponder (System.Type responder)
		{
			if (!responder.IsSubclassOf (typeof (Responder)))
				throw new ArgumentException ("Responder must inherit FastCgi.Responder class.", "responder");
			
			if (responder.GetConstructor (new System.Type[] {typeof (ResponderRequest)}) == null)
				throw new ArgumentException ("Responder must contain constructor 'ctor(ResponderRequest)'", "responder");
			
			responderType = responder;
		}
		
		public static bool IsSupported {get {return responderType != null;}}
	}
	
	public abstract class Responder
	{
		public abstract int Process ();
	}
}
