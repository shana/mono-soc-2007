using System;
using System.Xml;

namespace Umbraco
{
	/// <summary>
	/// Summary description for xmlHelper.
	/// </summary>
	public class XmlHelper
	{
		/// <summary>
		/// Imports the XML node from text.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="xmlDoc">The XML doc.</param>
		/// <returns></returns>
		public static XmlNode ImportXmlNodeFromText (string text, ref XmlDocument xmlDoc) 
		{
			xmlDoc.LoadXml(text);
			return xmlDoc.FirstChild;
		}


		/// <summary>
		/// Adds the attribute.
		/// </summary>
		/// <param name="xmlDocument">The XML document.</param>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static XmlAttribute AddAttribute(XmlDocument xmlDocument, string name, string value) 
		{
			XmlAttribute temp = xmlDocument.CreateAttribute(name);
			temp.Value = value;
			return temp;
		}
		/// <summary>
		/// Adds the text node.
		/// </summary>
		/// <param name="xmlDocument">The XML document.</param>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static XmlNode AddTextNode(XmlDocument xmlDocument, string name, string value) 
		{
			XmlNode temp = xmlDocument.CreateNode(XmlNodeType.Element, name, string.Empty);
			temp.AppendChild(xmlDocument.CreateTextNode(value));
			return temp;
		}

		/// <summary>
		/// Adds the C data node.
		/// </summary>
		/// <param name="xmlDocument">The XML document.</param>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static XmlNode AddCDataNode(XmlDocument xmlDocument, string name, string value) 
		{
			XmlNode temp = xmlDocument.CreateNode(XmlNodeType.Element, name, string.Empty);
			temp.AppendChild(xmlDocument.CreateCDataSection(value));
			return temp;
		}


		/// <summary>
		/// Gets the node value.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		public static string GetNodeValue(XmlNode node) 
		{
			if (node == null || node.FirstChild == null)
				return string.Empty;
			return node.FirstChild.Value;
		}
	}
}
