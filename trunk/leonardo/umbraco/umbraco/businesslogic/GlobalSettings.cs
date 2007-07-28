using System;
using System.Configuration;
using System.Web;
using System.Xml;
using umbraco.BusinessLogic;

namespace umbraco
{
    /// <summary>
    /// Summary description for GlobalSettings.
    /// </summary>
    public class GlobalSettings
    {
        public GlobalSettings()
        {
        }

        public static string ReservedUrls
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoReservedUrls"];
                return "";
            }
        }

        public static string ReservedPaths
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoReservedPaths"];
                return "";
            }
        }

        public static string ContentXML
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["umbracoContentXML"];
                }
                catch
                {
                    return "";
                }
            }
        }

        public static string StorageDirectory
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["umbracoStorageDirectory"];
                }
                catch
                {
                    return "";
                }
            }
        }

        public static string Path
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["umbracoPath"];
                }
                catch
                {
                    return "";
                }
            }
        }

        public static string DbDSN
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["umbracoDbDSN"];
                }
                catch
                {
                    return "";
                }
            }
        }

        public static string ConfigurationStatus
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["umbracoConfigurationStatus"];
                }
                catch
                {
                    return "";
                }
            }

            set
            {
                ExeConfigurationFileMap webConfig = new ExeConfigurationFileMap();
                webConfig.ExeConfigFilename = FullpathToRoot + "web.config";

                Configuration config =
                    ConfigurationManager.OpenMappedExeConfiguration(webConfig, ConfigurationUserLevel.None);
                config.AppSettings.Settings["umbracoConfigurationStatus"].Value = value;
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        public static string FullpathToRoot
        {
            get { return HttpRuntime.AppDomainAppPath; }
        }

        public static string StatDbDSN
        {
            get
            {
                string statdb = ConfigurationManager.AppSettings["umbracoStatDbDSN"];
                if (string.IsNullOrEmpty(statdb))
                    return DbDSN;
                return statdb.Trim();
            }
        }

        public static bool DebugMode
        {
            get
            {
                try
                {
                    return bool.Parse(ConfigurationManager.AppSettings["umbracoDebugMode"]);
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool Configured
        {
            get
            {
                try
                {
                    string configStatus = ConfigurationStatus;
                    string currentVersion = CurrentVersion;
                    if (currentVersion != configStatus)
                        Log.Add(LogTypes.Debug, User.GetUser(0), -1,
                                "CurrentVersion different from configStatus: '" + currentVersion + "','" + configStatus +
                                "'");

                    return (configStatus == currentVersion);
                }
                catch
                {
                    return false;
                }
            }
        }

        public static int TimeOutInMinutes
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["umbracoTimeOutInMinutes"]);
                }
                catch
                {
                    return 20;
                }
            }
        }

        public static bool UseDirectoryUrls
        {
            get
            {
                try
                {
                    return bool.Parse(ConfigurationManager.AppSettings["umbracoUseDirectoryUrls"]);
                }
                catch
                {
                    return false;
                }
            }
        }

        public static string DisableVersionCheck
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoDisableVersionCheck"];
                return "";
            }
        }

        public static string UrlForbittenCharacters
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoUrlForbittenCharacters"];
                return "";
            }
        }

        public static string UrlSpaceCharacter
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoUrlSpaceCharacter"];
                return "";
            }
        }

        public static string SmtpServer
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["umbracoSmtpServer"];
                }
                catch
                {
                    return "";
                }
            }
        }

        public static string DisableXsltExtensions
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoDisableXsltExtensions"];
                return "";
            }
        }

        public static string EditXhtmlMode
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoEditXhtmlMode"];
                return "";
            }
        }

        public static string DefaultUILanguage
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoDefaultUILanguage"];
                return "";
            }
        }

        public static string ProfileUrl
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoProfileUrl"];
                return "";
            }
        }

        public static bool HideTopLevelNodeFromPath
        {
            get
            {
                if (HttpContext.Current != null)
                    return bool.Parse(ConfigurationManager.AppSettings["umbracoHideTopLevelNodeFromPath"]);
                return false;
            }
        }

        public static bool EnableStat
        {
            get
            {
                string value = ConfigurationManager.AppSettings["umbracoEnableStat"];
                bool result;
                if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out result))
                    return result;
                return false;
            }
        }

        public static bool DisableLogging
        {
            get
            {
                string value = ConfigurationManager.AppSettings["umbracoDisableLogging"];
                bool result;
                if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out result))
                    return result;
                return false;
            }
        }

        public static bool EnableAsyncLogging
        {
            get
            {
                string value = ConfigurationManager.AppSettings["umbracoAsyncLogging"];
                bool result;
                if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out result))
                    return result;
                return false;
            }
        }

        public static bool EnableAsyncStatLogging
        {
            get
            {
                string value = ConfigurationManager.AppSettings["umbracoAsyncStatLogging"];
                bool result;
                if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out result))
                    return result;
                return false;
            }
        }

        public static string CurrentVersion
        {
            get
            {
                    XmlDocument versionDoc = new XmlDocument();
                    XmlTextReader versionReader =
                        new XmlTextReader(FullpathToRoot + Path + System.IO.Path.DirectorySeparatorChar + "version.xml");
                    versionDoc.Load(versionReader);
                    versionReader.Close();

                    // Find current versions
                    int versionMajor, versionMinor, versionPatch;
                    string versionComment = "";
                    versionMajor = Convert.ToInt32(versionDoc.SelectSingleNode("/version/major").FirstChild.Value);
                    versionMinor = Convert.ToInt32(versionDoc.SelectSingleNode("/version/minor").FirstChild.Value);
                    versionPatch = Convert.ToInt32(versionDoc.SelectSingleNode("/version/patch").FirstChild.Value);

                    if (versionDoc.SelectSingleNode("/version/comment") != null)
                        if (versionDoc.SelectSingleNode("/version/comment").FirstChild != null)
                        versionComment += " " + versionDoc.SelectSingleNode("/version/comment").FirstChild.Value;

                    return
                        versionMajor.ToString() + "." + versionMinor.ToString() + "." + versionPatch.ToString() +
                        versionComment;
            }
        }

        public static string License
        {
            get
            {
                string license =
                    "<A href=\"http://umbraco.org/redir/license\" target=\"_blank\">the open source license MIT</A>. The umbraco UI is freeware licensed under the umbraco license.";
                if (HttpContext.Current != null)
                {
                    XmlDocument versionDoc = new XmlDocument();
                    XmlTextReader versionReader =
                        new XmlTextReader(HttpContext.Current.Server.MapPath(Path + "/version.xml"));
                    versionDoc.Load(versionReader);
                    versionReader.Close();

                    // check for license
                    try
                    {
                        string licenseUrl =
                            versionDoc.SelectSingleNode("/version/licensing/licenseUrl").FirstChild.Value;
                        string licenseValidation =
                            versionDoc.SelectSingleNode("/version/licensing/licenseValidation").FirstChild.Value;
                        string licensedTo =
                            versionDoc.SelectSingleNode("/version/licensing/licensedTo").FirstChild.Value;

                        if (licensedTo != "" && licenseUrl != "")
                        {
                            license = "umbraco Commercial License<br/><b>Registered to:</b><br/>" +
                                      licensedTo.Replace("\n", "<br/>") + "<br/><b>For use with domain:</b><br/>" +
                                      licenseUrl;
                        }
                    }
                    catch
                    {
                    }
                }
                return license;
            }
        }


        public static bool test
        {
            get
            {
                try
                {
                    HttpContext.Current.Response.Write("ContentXML :" + ContentXML + "\n");
                    HttpContext.Current.Response.Write("DbDSN :" + DbDSN + "\n");
                    HttpContext.Current.Response.Write("DebugMode :" + DebugMode + "\n");
                    HttpContext.Current.Response.Write("DefaultUILanguage :" + DefaultUILanguage + "\n");
                    HttpContext.Current.Response.Write("DisableVersionCheck :" + DisableVersionCheck + "\n");
                    HttpContext.Current.Response.Write("DisableXsltExtensions :" + DisableXsltExtensions + "\n");
                    HttpContext.Current.Response.Write("EditXhtmlMode :" + EditXhtmlMode + "\n");
                    HttpContext.Current.Response.Write("HideTopLevelNodeFromPath :" + HideTopLevelNodeFromPath + "\n");
                    HttpContext.Current.Response.Write("Path :" + Path + "\n");
                    HttpContext.Current.Response.Write("ProfileUrl :" + ProfileUrl + "\n");
                    HttpContext.Current.Response.Write("ReservedPaths :" + ReservedPaths + "\n");
                    HttpContext.Current.Response.Write("ReservedUrls :" + ReservedUrls + "\n");
                    HttpContext.Current.Response.Write("StorageDirectory :" + StorageDirectory + "\n");
                    HttpContext.Current.Response.Write("TimeOutInMinutes :" + TimeOutInMinutes + "\n");
                    HttpContext.Current.Response.Write("UrlForbittenCharacters :" + UrlForbittenCharacters + "\n");
                    HttpContext.Current.Response.Write("UrlSpaceCharacter :" + UrlSpaceCharacter + "\n");
                    HttpContext.Current.Response.Write("UseDirectoryUrls :" + UseDirectoryUrls + "\n");
                    return true;
                }
                catch
                {
                }
                return false;
            }
        }
    }
}