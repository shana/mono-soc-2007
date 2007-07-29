using System;

namespace Umbraco.interfaces
{

	public interface IData 
	{
		int PropertyId{set;}
		System.Xml.XmlNode ToXMl(System.Xml.XmlDocument d);
		object Value {get;set;}
		void MakeNew(int PropertyId);
		void Delete();
	}
}
