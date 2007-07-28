using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Xml;
using umbraco.BasePages;
using umbraco.BusinessLogic;

namespace umbraco
{
    /// <summary>
    /// Summary description for ui.
    /// </summary>
    public class ui
    {
        private static readonly string umbracoDefaultUILanguage = GlobalSettings.DefaultUILanguage;
        private static readonly string umbracoPath = GlobalSettings.Path;

        public static string Culture(User u)
        {
            XmlDocument langFile = getLanguageFile(u.Language);
            try
            {
                return langFile.SelectSingleNode("/language").Attributes.GetNamedItem("culture").Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string Text(string Key, User u)
        {
            return GetText(Key);
        }

        public static string Text(string Key)
        {
            return GetText(Key);
        }

        public static string Text(string Area, string Key, User u)
        {
            return GetText(Area, Key);
        }

        public static string Text(string Area, string Key)
        {
            return GetText(Area, Key, UmbracoEnsuredPage.CurrentUser.Language);
        }

        public static string Text(string Area, string Key, string[] Variables, User u)
        {
            // Check if user is null (AutoForm)
            string _culture = "";
            if (u == null)
                _culture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            else
                _culture = new System.Globalization.CultureInfo(Culture(u)).TwoLetterISOLanguageName;

            return GetText(Area, Key, Variables, _culture);
        }

        public static string Text(string Area, string Key, string Variable, User u)
        {
            return GetText(Area, Key, Variable);
        }

        public static string GetText(string key)
        {
            if (key == null)
                return string.Empty;

            string language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            if (string.IsNullOrEmpty(language))
                language = umbracoDefaultUILanguage;

            XmlDocument langFile = getLanguageFile(language);
            if (langFile != null)
            {
                XmlNode node = langFile.SelectSingleNode(string.Format("//key [@alias = '{0}']", key));
                if (node != null && node.FirstChild != null)
                    return node.FirstChild.Value;
            }
            return "[" + key + "]";
        }

        public static string GetText(string area, string key)
        {
            if (string.IsNullOrEmpty(area) || string.IsNullOrEmpty(key))
                return string.Empty;

            string language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            if (string.IsNullOrEmpty(language))
                language = umbracoDefaultUILanguage;

            XmlDocument langFile = getLanguageFile(language);
            if (langFile != null)
            {
                XmlNode node =
                    langFile.SelectSingleNode(string.Format("//area [@alias = '{0}']/key [@alias = '{1}']", area, key));
                if (node != null)
                    return xmlHelper.GetNodeValue(node);
            }
            return "[" + key + "]";
        }

        public static string GetText(string area, string key, string[] variables)
        {
            if (string.IsNullOrEmpty(area) || string.IsNullOrEmpty(key) || variables == null)
                return string.Empty;

            string language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            if (string.IsNullOrEmpty(language))
                language = umbracoDefaultUILanguage;

            XmlDocument langFile = getLanguageFile(language);
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

        public static string GetText(string area, string key, string[] variables, string language)
        {
            if (string.IsNullOrEmpty(area) || string.IsNullOrEmpty(key) || variables == null)
                return string.Empty;

            XmlDocument langFile = getLanguageFile(language);
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

        private static string GetStringWithVars(XmlNode node, string[] variables)
        {
            string stringWithVars = xmlHelper.GetNodeValue(node);
            MatchCollection vars =
                Regex.Matches(stringWithVars, @"\%(\d)\%",
                              RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            foreach (Match var in vars)
            {
                stringWithVars =
                    stringWithVars.Replace(var.Value,
                                           variables[Convert.ToInt32(var.Groups[0].Value.Replace("%", ""))]);
            }
            return stringWithVars;
        }

        public static string GetText(string area, string key, string variable)
        {
            if (string.IsNullOrEmpty(area) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(variable))
                return string.Empty;

            string language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            if (string.IsNullOrEmpty(language))
                language = umbracoDefaultUILanguage;

            XmlDocument langFile = getLanguageFile(language);
            if (langFile != null)
            {
                XmlNode node = langFile.SelectSingleNode(string.Format("//area [@alias = '{0}']/key [@alias = '{1}']",
                                                                       area, key));
                if (node != null)
                    return xmlHelper.GetNodeValue(node).Replace("%0%", variable);
            }
            return "[" + key + "]";
        }

        public static XmlDocument getLanguageFile(string language)
        {
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
                                                       "Error reading umbraco language xml source (" + language + ")", e);
                    }
                }
            }
            else
                langFile = (XmlDocument) HttpRuntime.Cache["uitext_" + language];

            return langFile;
        }
    }
}