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
using ActiveMQ.OpenWire;
using ActiveMQ.Util;
using NMS;
using System;

namespace ActiveMQ.Commands
{
	public delegate void AcknowledgeHandler(ActiveMQMessage message);
}

namespace ActiveMQ.Commands
{
	public class ActiveMQMessage : Message, IMessage, MarshallAware
    {
        public const byte ID_ActiveMQMessage = 23;
        
        protected static MessagePropertyHelper propertyHelper = new MessagePropertyHelper();
        
        private PrimitiveMap properties;
        
        public event AcknowledgeHandler Acknowledger;
        
        public static ActiveMQMessage Transform(IMessage message)
        {
            return (ActiveMQMessage) message;
        }
        
        // TODO generate Equals method
        // TODO generate GetHashCode method
        
        
        public override byte GetDataStructureType()
        {
            return ID_ActiveMQMessage;
        }
        
        public void Acknowledge()
        {
            if (Acknowledger == null)
			{
                throw new NMSException("No Acknowledger has been associated with this message: " + this);
			}
            else
			{
                Acknowledger(this);
            }
        }
        
        
        // Properties
        
        public IPrimitiveMap Properties
        {
            get {
                if (properties == null)
                {
                    properties = PrimitiveMap.Unmarshal(MarshalledProperties);
                }
                return properties;
            }
        }
        
        public IDestination FromDestination
        {
            get { return Destination; }
            set { this.Destination = ActiveMQDestination.Transform(value); }
        }
                               
        /// <summary>
        /// The correlation ID used to correlate messages with conversations or long running business processes
        /// </summary>
        public string NMSCorrelationID
        {
            get {
                return CorrelationId;
            }
            set {
                CorrelationId = value;
            }
        }
        
        /// <summary>
        /// The destination of the message
        /// </summary>
        public IDestination NMSDestination
        {
            get {
                return Destination;
            }
        }
        
        /// <summary>
        /// The time in milliseconds that this message should expire in
        /// </summary>
        public TimeSpan NMSExpiration
        {
            get {
                return TimeSpan.FromMilliseconds(Expiration);
            }
            set {
                Expiration = value.Milliseconds;
            }
        }
        
        /// <summary>
        /// The message ID which is set by the provider
        /// </summary>
        public string NMSMessageId
        {
            get {
                return BaseDataStreamMarshaller.ToString(MessageId);
            }
        }
        
        /// <summary>
        /// Whether or not this message is persistent
        /// </summary>
        public bool NMSPersistent
        {
            get {
                return Persistent;
            }
            set {
                Persistent = value;
            }
        }
        
        /// <summary>
        /// The Priority on this message
        /// </summary>
        public byte NMSPriority
        {
            get {
                return Priority;
            }
            set {
                Priority = value;
            }
        }
        
        /// <summary>
        /// Returns true if this message has been redelivered to this or another consumer before being acknowledged successfully.
        /// </summary>
        public bool NMSRedelivered
        {
            get {
                return RedeliveryCounter > 0;
            }
        }
        
        
        /// <summary>
        /// The destination that the consumer of this message should send replies to
        /// </summary>
        public IDestination NMSReplyTo
        {
            get {
                return ReplyTo;
            }
            set {
                ReplyTo = ActiveMQDestination.Transform(value);
            }
        }
        
        
        /// <summary>
        /// The timestamp the broker added to the message
        /// </summary>
        public DateTime NMSTimestamp
        {
            get {
                return DateUtils.ToDateTime(Timestamp);
            }
        }
        
        /// <summary>
        /// The type name of this message
        /// </summary>
        public string NMSType
        {
            get {
                return Type;
            }
            set {
                Type = value;
            }
        }
        
        
        // NMS Extension headers
        
        /// <summary>
        /// Returns the number of times this message has been redelivered to other consumers without being acknowledged successfully.
        /// </summary>
        public int NMSXDeliveryCount
        {
            get {
                return RedeliveryCounter + 1;
            }
        }
        
        
        /// <summary>
        /// The Message Group ID used to group messages together to the same consumer for the same group ID value
        /// </summary>
        public string NMSXGroupID
        {
            get {
                return GroupID;
            }
            set {
                GroupID = value;
            }
        }
        /// <summary>
        /// The Message Group Sequence counter to indicate the position in a group
        /// </summary>
        public int NMSXGroupSeq
        {
            get {
                return GroupSequence;
            }
            set {
                GroupSequence = value;
            }
        }
        
        /// <summary>
        /// Returns the ID of the producers transaction
        /// </summary>
        public string NMSXProducerTXID
        {
            get {
                TransactionId txnId = OriginalTransactionId;
                if (txnId == null)
                {
                    txnId = TransactionId;
                }
                if (txnId != null)
                {
                    return BaseDataStreamMarshaller.ToString(txnId);
                }
                return null;
            }
        }
        
        public object GetObjectProperty(string name)
        {
            return propertyHelper.GetObjectProperty(this, name);
        }
        
        public void SetObjectProperty(string name, object value)
        {
            propertyHelper.SetObjectProperty(this, name, value);
        }
        
        // MarshallAware interface
        public override bool IsMarshallAware()
        {
            return true;
        }
        
        public override void BeforeMarshall(OpenWireFormat wireFormat)
        {
            MarshalledProperties = null;
            if (properties != null)
            {
                MarshalledProperties = properties.Marshal();
            }
        }
        
        
    }
}

