using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;

using businesslogic;
using umbraco.BusinessLogic;

namespace umbraco
{
	/// <summary>
	/// Summary description for helper.
	/// </summary>
	public class helper
	{
		public static bool IsNumeric(string Number)
		{
			int result;
			return int.TryParse(Number, out result);
		}

		public static string Request(string text)
		{
			string temp = string.Empty;
			if (HttpContext.Current.Request[text] != null)
				if (HttpContext.Current.Request[text] != string.Empty)
					temp = HttpContext.Current.Request[text];
			return temp;
		}

		public static Hashtable ReturnAttributes(String tag)
		{
			Hashtable ht = new Hashtable();
			MatchCollection m =
				Regex.Matches(tag, "(?<attributeName>\\S*)=\"(?<attributeValue>[^\"]*)\"",
				              RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
			foreach (Match attributeSet in m)
				ht.Add(attributeSet.Groups["attributeName"].Value.ToString(), attributeSet.Groups["attributeValue"].Value.ToString());

			return ht;
		}

		public static String FindAttribute(Hashtable attributes, String key)
		{
			string attributeValue = string.Empty;
			if (attributes[key] != null)
				attributeValue = attributes[key].ToString();

			attributeValue = parseAttribute(null, attributeValue);
			return attributeValue;
		}

		private static string parseAttribute(page umbPage, string attributeValue)
		{
			// Check for potential querystring/cookie variables
			if (attributeValue.Length > 3 && attributeValue.IndexOf("[") > -1)
			{
				string[] attributeValueSplit = (attributeValue).Split(',');
				foreach (string attributeValueItem in attributeValueSplit)
				{
					attributeValue = attributeValueItem;

					// Check for special variables (always in square-brackets like [name])
					if (attributeValueItem.Substring(0, 1) == "[" &&
					    attributeValueItem.Substring(attributeValueItem.Length - 1, 1) == "]")
					{
						// find key name
						string keyName = attributeValueItem.Substring(2, attributeValueItem.Length - 3);
						string keyType = attributeValueItem.Substring(1, 1);

						switch (keyType)
						{
							case "@":
								attributeValue = HttpContext.Current.Request[keyName];
								break;
							case "%":
								attributeValue = StateHelper.GetSessionValue<string>(keyName);
								break;
							case "#":
								if (umbPage.Elements[keyName] != null)
									attributeValue = umbPage.Elements[keyName].ToString();
								break;
						}

						if (attributeValue != null)
						{
							attributeValue = attributeValue.Trim();
							if (attributeValue != string.Empty)
								break;
						}
						else
							attributeValue = string.Empty;
					}
				}
			}

			return attributeValue;
		}

		public static String FindAttribute(page umbPage, Hashtable attributes, String key)
		{
			string attributeValue = string.Empty;
			if (attributes[key] != null)
				attributeValue = attributes[key].ToString();

			attributeValue = parseAttribute(umbPage, attributeValue);

			return attributeValue;
		}

		public static string SpaceCamelCasing(string text)
		{
			string _tempString = text.Substring(0, 1).ToUpper();
			for (int i = 1; i < text.Length; i++)
			{
				if (text.Substring(i, 1) == " ")
					break;
				if (text.Substring(i, 1).ToUpper() == text.Substring(i, 1))
					_tempString += " ";
				_tempString += text.Substring(i, 1);
			}
			return _tempString;
		}


		public static string GetBaseUrl(HttpContext Context)
		{
			return Context.Request.Url.GetLeftPart(UriPartial.Authority);
		}
	}
}