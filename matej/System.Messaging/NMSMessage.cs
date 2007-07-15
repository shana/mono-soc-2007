/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 17.7.2007
 * Time: 19:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using ActiveMQ.Commands;
using NMS;

namespace System.Messaging
{
	/// <summary>
	/// Description of NMSMessage.
	/// </summary>
	public class NMSMessage: Message
	{
		private IMessage msg;
		
		public NMSMessage(IMessage msg): base()
		{
			this.msg = msg;
		}
		
		public override object Body {
			get {
				return (string)msg.Properties["Body"];
			}
			[MonoTODO("serialize/deserialize to string")]
			set {
				msg.Properties["Body"] = value;
			}
		}
		
		
		public override string Label {
			get {
				return (string)msg.Properties["Label"];
			}
			set {
				msg.Properties["Label"] = value;
			}
		}
		
		public override string CorrelationId {
			get {
				return msg.NMSCorrelationID;
			}
			set {
				msg.NMSCorrelationID = value;
			}
		}
				
		public override string Id {
			get {
				return msg.NMSMessageId;
			}
		}
		
		public override DateTime SentTime {
			[MonoTODO]
			get {
				return msg.NMSTimestamp;
			}
		}
		
		
		
	}
}
