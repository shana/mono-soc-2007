using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Umbraco;
using Umbraco.BasePages;
using Umbraco.BusinessLogic;

namespace Umbraco
{
	/// <summary>
	/// Summary description for ui.
	/// </summary>
	public static class UIHelper
	{
		private static readonly string umbracoDefaultUILanguage = GlobalSettings.DefaultUILanguage;
		private static readonly string umbracoPath = GlobalSettings.Path;

		/// <summary>
		/// Gets the user culture.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public static string GetUserCulture(User user)
		{
			XmlDocument langFile = GetLanguageFile(user.Language);
			// TODO: Consider exception handling
			// TODO: Remove hard coded string values
			return langFile.SelectSingleNode("/language").Attributes.GetNamedItem("culture").Value;
		}

		/// <summary>
		/// Texts the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		[Obsolete("Use GetText(string key) instead")]
		public static string Text(string key)
		{
			return GetText(key);
		}

		/// <summary>
		/// Texts the specified area.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="key">The key.</param>
		/// <param name="u">The u.</param>
		/// <returns></returns>
		[Obsolete("Use GetText(string area, string key) instead")]
		public static string Text(string area, string key, User u)
		{
			return GetText(area, key);
		}

		/// <summary>
		/// Texts the specified area.
		/// </summary>
		/// <param name="Area">The area.</param>
		/// <param name="Key">The key.</param>
		/// <returns></returns>
		[Obsolete("Use GetText(string area, string key) instead")]
		public static string Text(string Area, string Key)
		{
			return GetText(Area, Key, UmbracoEnsuredPage.CurrentUser.Language);
		}

		/// <summary>
		/// Texts the specified area.
		/// </summary>
		/// <param name="Area">The area.</param>
		/// <param name="Key">The key.</param>
		/// <param name="Variables">The variables.</param>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		[Obsolete]
		public static string Text(string Area, string Key, string[] Variables, User user)
		{
			// Check if user is null (AutoForm)
			string _culture = "";
			if (user == null)
			{
				_culture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
			}
			else
			{
				_culture = new CultureInfo(GetUserCulture(user)).TwoLetterISOLanguageName;
			}

			return GetText(Area, Key, Variables, _culture);
		}

		/// <summary>
		/// Texts the specified area.
		/// </summary>
		/// <param name="Area">The area.</param>
		/// <param name="Key">The key.</param>
		/// <param name="Variable">The variable.</param>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		[Obsolete("Use GetText(string area, string key, string variable) * string user is not used")]
		public static string Text(string Area, string Key, string Variable, User user)
		{
			return GetText(Area, Key, Variable);
		}

		/// <summary>
		/// Gets the text.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static string GetText(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return string.Empty;
			}
			string language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
			if (string.IsNullOrEmpty(language))
			{
				language = umbracoDefaultUILanguage;
			}

			XmlDocument langFile = GetLanguageFile(language);
			if (langFile != null)
			{
				// TODO: Remove hard coded string values
				XmlNode node = langFile.SelectSingleNode(string.Format("//key [@alias = '{0}']", key));
				if (node != null && node.FirstChild != null)
				{
					return node.FirstChild.Value;
				}
			}
			return "[" + key + "]";
		}

		/// <summary>
		/// Gets the text.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static string GetText(string area, string key)
		{			
			// TODO: Remove hard coded string values

			if (string.IsNullOrEmpty(area) || string.IsNullOrEmpty(key))
			{
				return string.Empty;
			}

			string language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
			if (string.IsNullOrEmpty(language))
			{
				language = umbracoDefaultUILanguage;
			}

			XmlDocument langFile = GetLanguageFile(language);
			
			if (langFile != null)
			{
				XmlNode node =
					langFile.SelectSingleNode(string.Format("//area [@alias = '{0}']/key [@alias = '{1}']", area, key));
				
				if (node != null)
				{
					return XmlHelper.GetNodeValue(node);
				}
			}
			return "[" + key + "]";
		}

		/// <summary>
		/// Gets the text.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="key">The key.</param>
		/// <param name="variables">The variables.</param>
		/// <returns></returns>
		public static string GetText(string area, string key, string[] variables)
		{
			// TODO: Remove hard coded string values
			if (string.IsNullOrEmpty(area) || string.IsNullOrEmpty(key) || variables == null)
			{
				return string.Empty;
			}

			string language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
			if (string.IsNullOrEmpty(language))
			{
				language = umbracoDefaultUILanguage;
			}

			XmlDocument langFile = GetLanguageFile(language);
			if (langFile != null)
			{
				XmlNode node = langFile.SelectSingleNode(string.Format("//area [@alias = '{0}']/key [@alias = '{1}']",
				                                                       area, key));
				if (node != null)
				{
					string stringWithVars = GetStringWithVars(node, variables);
					return stringWithVars;
				}
			}
			return "[" + key + "]";
		}

		/// <summary>
		/// Gets the text.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="key">The key.</param>
		/// <param name="variables">The variables.</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static string GetText(string area, string key, string[] variables, string language)
		{
			// TODO: Remove hard coded string values
			if (string.IsNullOrEmpty(area) || string.IsNullOrEmpty(key) || variables == null)
			{
				return string.Empty;
			}

			XmlDocument langFile = GetLanguageFile(language);
			if (langFile != null)
			{
				XmlNode node = langFile.SelectSingleNode(string.Format("//area [@alias = '{0}']/key [@alias = '{1}']",
				                                                       area, key));
				if (node != null)
				{
					string stringWithVars = GetStringWithVars(node, variables);
					return stringWithVars;
				}
			}
			return "[" + key + "]";
		}

		/// <summary>
		/// Gets the string with vars.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="variables">The variables.</param>
		/// <returns></returns>
		private static string GetStringWithVars(XmlNode node, string[] variables)
		{
			// TODO: Remove hard coded string values
			string stringWithVars = XmlHelper.GetNodeValue(node);
			MatchCollection vars =
				Regex.Matches(stringWithVars, @"\%(\d)\%",
				              RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
			foreach (Match var in vars)
			{
				stringWithVars =
					stringWithVars.Replace(var.Value,
					                       variables[Convert.ToInt32(var.Groups[0].Value.Replace("%", string.Empty))]);
			}
			return stringWithVars;
		}

		/// <summary>
		/// Gets the text.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="key">The key.</param>
		/// <param name="variable">The variable.</param>
		/// <returns></returns>
		public static string GetText(string area, string key, string variable)
		{
			// TODO: Remove hard coded string values

			if (string.IsNullOrEmpty(area) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(variable))
			{
				return string.Empty;
			}

			string language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
			if (string.IsNullOrEmpty(language))
			{
				language = umbracoDefaultUILanguage;
			}

			XmlDocument langFile = GetLanguageFile(language);
			if (langFile != null)
			{
				XmlNode node = langFile.SelectSingleNode(string.Format("//area [@alias = '{0}']/key [@alias = '{1}']",
				                                                       area, key));
				if (node != null)
					return XmlHelper.GetNodeValue(node).Replace("%0%", variable);
			}
			return "[" + key + "]";
		}

		/// <summary>
		/// Gets the language file.
		/// </summary>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public static XmlDocument GetLanguageFile(string language)
		{
			// TODO: Remove hard coded string values

			XmlDocument langFile = new XmlDocument();

			string cacheKey = "uitext_" + language;

			// Check for language file in cache
			if (HttpRuntime.Cache[cacheKey] == null)
			{
				using (XmlTextReader langReader =
					new XmlTextReader(
						HttpContext.Current.Server.MapPath(umbracoPath + "/config/lang/" + language + ".xml")))
				{
					try
					{
						langFile.Load(langReader);
						HttpRuntime.Cache.Insert(cacheKey, langFile,
						                         new CacheDependency(
						                         	HttpContext.Current.Server.MapPath(umbracoPath + "/config/lang/" +
						                         	                                   language + ".xml")));
					}
					catch (Exception e)
					{
						HttpContext.Current.Trace.Warn("ui",
						                               "Error reading Umbraco language xml source (" + language + ")", e);
					}
				}
			}
			else
			{
				langFile = (XmlDocument) HttpRuntime.Cache["uitext_" + language];
			}

			return langFile;
		}
	}
}