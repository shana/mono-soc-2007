/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 17.7.2007
 * Time: 20:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Messaging;

namespace Messaging.Test
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			MessageQueue myQueue = new MessageQueue("tcp://localhost:61616:testqueue");
   			Message msg = new Message("Hello this is test message" /*, new XmlMessageFormatter() */ );
   			// Send the image to the queue.
   			myQueue.Send(msg);
			Console.WriteLine("Hello World!");
		}
	}
}
