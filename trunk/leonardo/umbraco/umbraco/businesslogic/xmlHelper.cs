using System;
using System.Xml;

namespace Umbraco
{
	/// <summary>
	/// Summary description for xmlHelper.
	/// </summary>
	public class XmlHelper
	{
		public static XmlNode ImportXmlNodeFromText (string text, ref XmlDocument xmlDoc) 
		{
			xmlDoc.LoadXml(text);
			return xmlDoc.FirstChild;
		}

		public static XmlAttribute addAttribute(XmlDocument Xd, string Name, string Value) 
		{
			XmlAttribute temp = Xd.CreateAttribute(Name);
			temp.Value = Value;
			return temp;
		}

		public static XmlNode addTextNode(XmlDocument Xd, string Name, string Value) 
		{
			XmlNode temp = Xd.CreateNode(XmlNodeType.Element, Name, "");
			temp.AppendChild(Xd.CreateTextNode(Value));
			return temp;
		}

		public static XmlNode addCDataNode(XmlDocument Xd, string Name, string Value) 
		{
			XmlNode temp = Xd.CreateNode(XmlNodeType.Element, Name, "");
			temp.AppendChild(Xd.CreateCDataSection(Value));
			return temp;
		}

		public static string GetNodeValue(XmlNode n) 
		{
			if (n == null || n.FirstChild == null)
				return string.Empty;
			return n.FirstChild.Value;
		}
	}
}
