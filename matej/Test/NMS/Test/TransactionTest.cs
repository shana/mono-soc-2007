/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using NUnit.Framework;
using System;
using System.Collections;


namespace NMS.Test
{
	[TestFixture]
    abstract public class TransactionTest : NMSTestSupport
    {
        private static int destinationCounter;
        
        IMessageProducer producer;
        IMessageConsumer consumer;
        
        [SetUp]
		override public void SetUp()
        {
            base.SetUp();
			acknowledgementMode = AcknowledgementMode.Transactional;
            Drain();
            consumer = Session.CreateConsumer(Destination);
            producer = Session.CreateProducer(Destination);
        }
		
        [TearDown]
        override public void TearDown()
        {
			base.TearDown();
        }
		
		
        [Test]
        public void TestSendRollback()
        {
            IMessage[] outbound = new IMessage[]{
                Session.CreateTextMessage("First Message"),
                Session.CreateTextMessage("Second Message")
            };
            
            //sends a message
            producer.Send(outbound[0]);
            Session.Commit();
            
            //sends a message that gets rollbacked
            producer.Send(Session.CreateTextMessage("I'm going to get rolled back."));
            Session.Rollback();
            
            //sends a message
            producer.Send(outbound[1]);
            Session.Commit();
            
            //receives the first message
            ArrayList messages = new ArrayList();
            Console.WriteLine("About to consume message 1");
            IMessage message = consumer.Receive(TimeSpan.FromMilliseconds(1000));
            messages.Add(message);
            Console.WriteLine("Received: " + message);
            
            //receives the second message
            Console.WriteLine("About to consume message 2");
            message = consumer.Receive(TimeSpan.FromMilliseconds(4000));
            messages.Add(message);
            Console.WriteLine("Received: " + message);
            
            //validates that the rollbacked was not consumed
            Session.Commit();
            IMessage[] inbound = new IMessage[messages.Count];
            messages.CopyTo(inbound);
            AssertTextMessagesEqual("Rollback did not work.", outbound, inbound);
        }
        
        [Test]
        public void TestSendSessionClose()
        {
            IMessage[] outbound = new IMessage[]{
                Session.CreateTextMessage("First Message"),
                Session.CreateTextMessage("Second Message")
            };
            
            //sends a message
            producer.Send(outbound[0]);
            Session.Commit();
            
            //sends a message that gets rollbacked
            producer.Send(Session.CreateTextMessage("I'm going to get rolled back."));
            consumer.Dispose();
            Session.Dispose();
            
            Reconnect();
            
            //sends a message
            producer.Send(outbound[1]);
            Session.Commit();
            
            //receives the first message
            ArrayList messages = new ArrayList();
            Console.WriteLine("About to consume message 1");
            IMessage message = consumer.Receive(TimeSpan.FromMilliseconds(1000));
            messages.Add(message);
            Console.WriteLine("Received: " + message);
            
            //receives the second message
            Console.WriteLine("About to consume message 2");
            message = consumer.Receive(TimeSpan.FromMilliseconds(4000));
            messages.Add(message);
            Console.WriteLine("Received: " + message);
            
            //validates that the rollbacked was not consumed
            Session.Commit();
            IMessage[] inbound = new IMessage[messages.Count];
            messages.CopyTo(inbound);
            AssertTextMessagesEqual("Rollback did not work.", outbound, inbound);
        }
        
        [Test]
        public void TestReceiveRollback()
        {
            IMessage[] outbound = new IMessage[]{
                Session.CreateTextMessage("First Message"),
                Session.CreateTextMessage("Second Message")
            };
            
            //sent both messages
            producer.Send(outbound[0]);
            producer.Send(outbound[1]);
            Session.Commit();
            
            Console.WriteLine("Sent 0: " + outbound[0]);
            Console.WriteLine("Sent 1: " + outbound[1]);
            
            ArrayList messages = new ArrayList();
            IMessage message = consumer.Receive(TimeSpan.FromMilliseconds(1000));
            messages.Add(message);
            Assert.AreEqual(outbound[0], message);
            Session.Commit();
            
            // rollback so we can get that last message again.
            message = consumer.Receive(TimeSpan.FromMilliseconds(1000));
            Assert.IsNotNull(message);
            Assert.AreEqual(outbound[1], message);
            Session.Rollback();
            
            // Consume again.. the previous message should
            // get redelivered.
            message = consumer.Receive(TimeSpan.FromMilliseconds(5000));
            Assert.IsNotNull(message, "Should have re-received the message again!");
            messages.Add(message);
            Session.Commit();
            
            IMessage[] inbound = new IMessage[messages.Count];
            messages.CopyTo(inbound);
            AssertTextMessagesEqual("Rollback did not work", outbound, inbound);
        }
        
        
        [Test]
        public void TestReceiveTwoThenRollback()
        {
            IMessage[] outbound = new IMessage[]{
                Session.CreateTextMessage("First Message"),
                Session.CreateTextMessage("Second Message")
            };
            
            producer.Send(outbound[0]);
            producer.Send(outbound[1]);
            Session.Commit();
            
            Console.WriteLine("Sent 0: " + outbound[0]);
            Console.WriteLine("Sent 1: " + outbound[1]);
            
            ArrayList messages = new ArrayList();
            IMessage message = consumer.Receive(TimeSpan.FromMilliseconds(1000));
            AssertTextMessageEqual("first mesage received before rollback", outbound[0], message);
            
            message = consumer.Receive(TimeSpan.FromMilliseconds(1000));
            Assert.IsNotNull(message);
            AssertTextMessageEqual("second message received before rollback", outbound[1], message);
            Session.Rollback();
            
            // Consume again.. the previous message should
            // get redelivered.
            message = consumer.Receive(TimeSpan.FromMilliseconds(5000));
            Assert.IsNotNull(message, "Should have re-received the first message again!");
            messages.Add(message);
            AssertTextMessageEqual("first message received after rollback", outbound[0], message);
            
            message = consumer.Receive(TimeSpan.FromMilliseconds(5000));
            Assert.IsNotNull(message, "Should have re-received the second message again!");
            messages.Add(message);
            AssertTextMessageEqual("second message received after rollback", outbound[1], message);
            
            Assert.IsNull(consumer.ReceiveNoWait());
            Session.Commit();
            
            IMessage[] inbound = new IMessage[messages.Count];
            messages.CopyTo(inbound);
            AssertTextMessagesEqual("Rollback did not work", outbound, inbound);
        }
        
        protected override string CreateDestinationName()
        {
            // TODO - how can we get the test name?
            return base.CreateDestinationName() + (++destinationCounter);
        }
        
        protected void AssertTextMessagesEqual(String message, IMessage[] expected, IMessage[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length, "Incorrect number of messages received");
            
            for (int i = 0; i < expected.Length; i++)
            {
                AssertTextMessageEqual(message + ". Index: " + i, expected[i], actual[i]);
            }
        }
        
        protected void AssertTextMessageEqual(String message, IMessage expected, IMessage actual)
        {
            Assert.IsTrue(expected is ITextMessage, "expected object not a text message");
            Assert.IsTrue(actual is ITextMessage, "actual object not a text message");
            
            String expectedText = ((ITextMessage) expected).Text;
            String actualText = ((ITextMessage) actual).Text;
            
            Assert.AreEqual(expectedText, actualText, message);
        }
		
		/// <summary>
		/// Method Reconnect
		/// </summary>
		protected override void Reconnect()
		{
			base.Reconnect();
            consumer = Session.CreateConsumer(Destination);
            producer = Session.CreateProducer(Destination);
		}
		
    }
}


