using System;
using System.Xml;

namespace umbraco.standardFormhandlers
{
	/// <summary>
	/// Summary description for memberLogoff.
	/// </summary>
	public class memberLogoff : interfaces.IFormhandler	
	{
		public memberLogoff()
		{
		}

		#region IFormhandler Members

		private int _redirectID = -1;

		public bool Execute(XmlNode formhandlerNode)
		{
			cms.businesslogic.member.Member m = cms.businesslogic.member.Member.GetCurrentMember();
			if (m != null)
				cms.businesslogic.member.Member.ClearMemberFromClient(m);
			return true;
		}

		public int redirectID
		{
			get
			{
				// TODO:  Add formMail.redirectID getter implementation
				return _redirectID;
			}
		}

		#endregion
	}
}
