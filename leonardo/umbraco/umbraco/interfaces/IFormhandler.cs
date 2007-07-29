using System;
using System.Xml;
namespace Umbraco.interfaces
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
