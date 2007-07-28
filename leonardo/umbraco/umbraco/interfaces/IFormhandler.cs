using System;
using System.Xml;
namespace umbraco.interfaces
{
	/// <summary>
	/// Summary description for IFormhandler.
	/// </summary>
	public interface IFormhandler
	{
		bool Execute(XmlNode formHandlerNode);
		int redirectID {get;}
	}
}
