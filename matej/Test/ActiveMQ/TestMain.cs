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
using ActiveMQ.Commands;
using NMS;
using System;

namespace ActiveMQ
{
    public class TestMain
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("About to connect to ActiveMQ");

                // START SNIPPET: demo
                IConnectionFactory factory = new ConnectionFactory(new Uri("tcp://localhost:61616"));
                using (IConnection connection = factory.CreateConnection())
                {
                    Console.WriteLine("Created a connection!");
                    
                    ISession session = connection.CreateSession();
                    
                    IDestination destination = session.GetQueue("FOO.BAR");
                    Console.WriteLine("Using destination: " + destination);
                    
                    // lets create a consumer and producer
                    IMessageConsumer consumer = session.CreateConsumer(destination);
                    
                    IMessageProducer producer = session.CreateProducer(destination);
                    producer.Persistent = true;
                    
                    // lets send a message
                    ITextMessage request = session.CreateTextMessage("Hello World!");
                    request.NMSCorrelationID = "abc";
                    request.Properties["NMSXGroupID"] = "cheese";
                    request.Properties["myHeader"] = "James";
                    
                    producer.Send(request);
                    
                    // lets consume a message
                    ActiveMQTextMessage message = (ActiveMQTextMessage) consumer.Receive();
                    if (message == null)
                    {
                        Console.WriteLine("No message received!");
                    }
                    else
                    {
                        Console.WriteLine("Received message with ID:   " + message.NMSMessageId);
                        Console.WriteLine("Received message with text: " + message.Text);
                    }
                }
                // END SNIPPET: demo
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught: " + e);
                Console.WriteLine("Stack: " + e.StackTrace);
            }
        }
    }
}
