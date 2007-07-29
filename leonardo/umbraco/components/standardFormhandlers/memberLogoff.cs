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
			Cms.BusinessLogic.Member.Member m = Cms.BusinessLogic.Member.Member.GetCurrentMember();
			if (m != null)
				Cms.BusinessLogic.Member.Member.ClearMemberFromClient(m);
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
