using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace umbraco.cms.helpers
{
	/// <summary>
	/// Summary description for xhtml.
	/// </summary>
	public class xhtml
	{
		public xhtml()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static string BootstrapTidy(string html) 
		{
			string emptyTags = ",br,hr,input,img,";	
			string regex = "(<[^\\?][^(>| )]*>)|<([^\\?][^(>| )]*)([^>]*)>";
			Hashtable replaceTag = new Hashtable();
			replaceTag.Add("strong", "b");
			replaceTag.Add("em", "i");

			System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline) 
				| System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(regex, options);

			foreach (Match m in reg.Matches(html)) 
			{
				string orgTag = "";
				string tag = "";
				string cleanTag = "";

				if (m.Groups.Count < 2 || (m.Groups[2].Value.ToLower() != "img" || (m.Groups[2].Value.ToLower() == "img" && m.Value.IndexOf ("?UMBRACO_MACRO") == -1))) 
				{

					if (m.Groups[1].Value != "") 
					{
						orgTag = m.Groups[1].Value;
						cleanTag = replaceTags(m.Groups[1].Value.ToLower().Replace("<", "").Replace("/>", "").Replace(">", "").Trim(), replaceTag);
						tag = "<" + cleanTag + ">";
					}
					else 
					{
						orgTag = "<" + m.Groups[2].Value + m.Groups[3].Value + ">";

						// loop through the attributes and make them lowercase
						cleanTag = replaceTags(m.Groups[2].Value.ToLower(), replaceTag);
						tag = "<" + cleanTag + returnLowerCaseAttributes(m.Groups[3].Value) + ">";
					}

					// Check for empty tags
					if (bool.Parse(GlobalSettings.EditXhtmlMode) && emptyTags.IndexOf(","+cleanTag+",") > -1 && tag.IndexOf("/>") == -1)
						tag = tag.Replace(">", " />");

					html = html.Replace(orgTag, tag);
				}

			}
			return html;
		}

		private static string replaceTags(string tag, Hashtable replaceTag) 
		{
			string closeBracket = "";
			if (tag.Substring(0,1) == "/") 
			{
				closeBracket = "/";
				tag = tag.Substring(1, tag.Length-1);
			}

			if (replaceTag.ContainsKey(tag))
				return closeBracket+replaceTag[tag].ToString();
			else
				return closeBracket+tag;
		}

		private static string returnLowerCaseAttributes(String tag) 
		{
			string newTag = "";
			MatchCollection m = Regex.Matches(tag, "(?<attributeName>\\S*)=\"(?<attributeValue>[^\"]*)\"|(?<attributeName>\\S*)=(?<attributeValue>[^( |>)]*)(>| )",  RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
			foreach (System.Text.RegularExpressions.Match attributeSet in m) 
				newTag += " " + attributeSet.Groups["attributeName"].Value.ToString().ToLower() + "=\"" + attributeSet.Groups["attributeValue"].Value.ToString() + "\"";

			return newTag;
		}	
	}
}
