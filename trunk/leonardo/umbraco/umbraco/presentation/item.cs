using System;
using System.Collections;
using System.Web;
using System.Xml;

namespace umbraco
{
	/// <summary>
	/// 
	/// </summary>
	public class item
	{
		private String _fieldContent = "";
		private String _fieldName;

		public String FieldContent
		{
			get { return _fieldContent; }
		}

		public item(string itemValue, Hashtable attributes)
		{
			_fieldContent = itemValue;
			parseItem(attributes);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="umbPage"></param>
		/// <param name="attributes"></param>
		/// 
		public item(page umbPage, Hashtable attributes)
		{
			_fieldName = helper.FindAttribute(attributes, "field");

			if(_fieldName.StartsWith("#"))
			{
				_fieldContent = library.GetDictionaryItem(_fieldName.Substring(1, _fieldName.Length - 1));
			}
			else
			{
				// Loop through XML children we need to find the fields recursive
				if(helper.FindAttribute(attributes, "recursive") == "true")
				{
					XmlDocument umbracoXML = content.Instance.XmlContent;

					String[] splitpath = (String[]) umbPage.Elements["splitpath"];
					for(int i = 0; i < splitpath.Length - 1; i++)
					{
						XmlNode element = umbracoXML.GetElementById(splitpath[splitpath.Length - i - 1].ToString());
						if (element == null)
							continue;
						XmlNode currentNode = element.SelectSingleNode(string.Format("./data [@alias = '{0}']",
							_fieldName));
						if(currentNode != null && currentNode.FirstChild != null &&
						   !string.IsNullOrEmpty(currentNode.FirstChild.Value) &&
						   !string.IsNullOrEmpty(currentNode.FirstChild.Value.Trim()))
						{
							HttpContext.Current.Trace.Write("item.recursive", "Item loaded from " + splitpath[splitpath.Length - i - 1]);
							_fieldContent = currentNode.FirstChild.Value;
							break;
						}
					}
				}
				else
				{
					if(umbPage.Elements[_fieldName] != null && !string.IsNullOrEmpty(umbPage.Elements[_fieldName].ToString()))
						_fieldContent = umbPage.Elements[_fieldName].ToString().Trim();
					else if(!string.IsNullOrEmpty(helper.FindAttribute(attributes, "useIfEmpty")))
                        if (umbPage.Elements[helper.FindAttribute(attributes, "useIfEmpty")] != null && !string.IsNullOrEmpty(umbPage.Elements[helper.FindAttribute(attributes, "useIfEmpty")].ToString()))
							_fieldContent = umbPage.Elements[helper.FindAttribute(attributes, "useIfEmpty")].ToString().Trim();
				}
			}

			parseItem(attributes);
		}

		private void parseItem(Hashtable attributes)
		{
			HttpContext.Current.Trace.Write("item", "Start parsing '" + _fieldName + "'");
			if(helper.FindAttribute(attributes, "textIfEmpty") != "" && _fieldContent == "")
				_fieldContent = helper.FindAttribute(attributes, "textIfEmpty");

			_fieldContent = _fieldContent.Trim();

			// DATE FORMATTING FUNCTIONS
			if(helper.FindAttribute(attributes, "formatAsDateWithTime") == "true")
			{
				if(_fieldContent == "")
					_fieldContent = DateTime.Now.ToString();
				_fieldContent = Convert.ToDateTime(_fieldContent).ToLongDateString() +
				                helper.FindAttribute(attributes, "formatAsDateWithTimeSeparator") +
				                Convert.ToDateTime(_fieldContent).ToShortTimeString();
			}
			else if(helper.FindAttribute(attributes, "formatAsDate") == "true")
			{
				if(_fieldContent == "")
					_fieldContent = DateTime.Now.ToString();
				_fieldContent = Convert.ToDateTime(_fieldContent).ToLongDateString();
			}


			// TODO: Needs revision to check if parameter-tags has attributes
			if(helper.FindAttribute(attributes, "stripParagraph") == "true" && _fieldContent.Length > 5)
			{
				_fieldContent = _fieldContent.Trim();
				if(_fieldContent.ToUpper().Substring(0, 3) == "<P>")
					_fieldContent = _fieldContent.Substring(3, _fieldContent.Length - 3);
				if(_fieldContent.ToUpper().Substring(_fieldContent.Length - 4, 4) == "</P>")
					_fieldContent = _fieldContent.Substring(0, _fieldContent.Length - 4);
			}

			// OTHER FORMATTING FUNCTIONS
			if(_fieldContent != "" && helper.FindAttribute(attributes, "insertTextBefore") != "")
				_fieldContent = HttpContext.Current.Server.HtmlDecode(helper.FindAttribute(attributes, "insertTextBefore")) +
				                _fieldContent;
			if(_fieldContent != "" && helper.FindAttribute(attributes, "insertTextAfter") != "")
				_fieldContent += HttpContext.Current.Server.HtmlDecode(helper.FindAttribute(attributes, "insertTextAfter"));
			if(helper.FindAttribute(attributes, "urlEncode") == "true")
				_fieldContent = HttpContext.Current.Server.UrlEncode(_fieldContent);
			if(helper.FindAttribute(attributes, "convertLineBreaks") == "true")
				_fieldContent = _fieldContent.Replace("\n", "<br/>\n");

			// CASING
			if(helper.FindAttribute(attributes, "case") == "lower")
				_fieldContent = _fieldContent.ToLower();
			else if(helper.FindAttribute(attributes, "case") == "upper")
				_fieldContent = _fieldContent.ToUpper();

			HttpContext.Current.Trace.Write("item", "Done parsing '" + _fieldName + "'");
		}
	}
}