using System;
using System.Configuration;
using System.Web;
using System.Xml;
using Umbraco.BusinessLogic;

namespace Umbraco
{
    /// <summary>
    /// Summary description for GlobalSettings.
    /// </summary>
    public static class GlobalSettings
    {
		/// <summary>
		/// Gets the reserved urls.
		/// </summary>
		/// <value>The reserved urls.</value>
        public static string ReservedUrls
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoReservedUrls"];
                return string.Empty;
            }
        }

		/// <summary>
		/// Gets the reserved paths.
		/// </summary>
		/// <value>The reserved paths.</value>
        public static string ReservedPaths
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoReservedPaths"];
                return string.Empty;
            }
        }

		/// <summary>
		/// Gets the content XML.
		/// </summary>
		/// <value>The content XML.</value>
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
                    return string.Empty;
                }
            }
        }

		/// <summary>
		/// Gets the storage directory.
		/// </summary>
		/// <value>The storage directory.</value>
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
                    return string.Empty;
                }
            }
        }

		/// <summary>
		/// Gets the path.
		/// </summary>
		/// <value>The path.</value>
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
                    return string.Empty;
                }
            }
        }

		/// <summary>
		/// Gets the db DSN.
		/// </summary>
		/// <value>The db DSN.</value>
		[Obsolete("Use ConfigurationManager.ConnectionStrings")]
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
                    return string.Empty;
                }
            }
        }

		/// <summary>
		/// Gets or sets the configuration status.
		/// </summary>
		/// <value>The configuration status.</value>
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
                    return string.Empty;
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

		/// <summary>
		/// Gets the fullpath to root.
		/// </summary>
		/// <value>The fullpath to root.</value>
        public static string FullpathToRoot
        {
            get { return HttpRuntime.AppDomainAppPath; }
        }

		/// <summary>
		/// Gets the stat db DSN.
		/// </summary>
		/// <value>The stat db DSN.</value>
        [Obsolete("Use ConfigurationManager.ConnectionStrings")]
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

		/// <summary>
		/// Gets a value indicating whether the application is on debug mode
		/// </summary>
		/// <value><c>true</c> if [debug mode]; otherwise, <c>false</c>.</value>
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

		/// <summary>
		/// Gets a value indicating whether this applicaiton is configured.
		/// </summary>
		/// <value><c>true</c> if configured; otherwise, <c>false</c>.</value>
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

		/// <summary>
		/// Gets the time out in minutes.
		/// </summary>
		/// <value>The time out in minutes.</value>
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

		/// <summary>
		/// Gets the disable version check.
		/// </summary>
		/// <value>The disable version check.</value>
        public static string DisableVersionCheck
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoDisableVersionCheck"];
                return string.Empty;
            }
        }

		/// <summary>
		/// Gets the URL forbitten characters.
		/// </summary>
		/// <value>The URL forbitten characters.</value>
        public static string UrlForbittenCharacters
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoUrlForbittenCharacters"];
                return string.Empty;
            }
        }

		/// <summary>
		/// Gets the URL space character.
		/// </summary>
		/// <value>The URL space character.</value>
        public static string UrlSpaceCharacter
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoUrlSpaceCharacter"];
                return string.Empty;
            }
        }

		/// <summary>
		/// Gets the SMTP server.
		/// </summary>
		/// <value>The SMTP server.</value>
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
                    return string.Empty;
                }
            }
        }

		/// <summary>
		/// Gets the disable XSLT extensions.
		/// </summary>
		/// <value>The disable XSLT extensions.</value>
        public static string DisableXsltExtensions
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoDisableXsltExtensions"];
                return string.Empty;
            }
        }

		/// <summary>
		/// Gets the edit XHTML mode.
		/// </summary>
		/// <value>The edit XHTML mode.</value>
        public static string EditXhtmlMode
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoEditXhtmlMode"];
                return string.Empty;
            }
        }

		/// <summary>
		/// Gets the default UI language.
		/// </summary>
		/// <value>The default UI language.</value>
        public static string DefaultUILanguage
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoDefaultUILanguage"];
                return string.Empty;
            }
        }

		/// <summary>
		/// Gets the profile URL.
		/// </summary>
		/// <value>The profile URL.</value>
        public static string ProfileUrl
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConfigurationManager.AppSettings["umbracoProfileUrl"];
                return string.Empty;
            }
        }

		/// <summary>
		/// Gets a value indicating whether [hide top level node from path].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [hide top level node from path]; otherwise, <c>false</c>.
		/// </value>
        public static bool HideTopLevelNodeFromPath
        {
            get
            {
                if (HttpContext.Current != null)
                    return bool.Parse(ConfigurationManager.AppSettings["umbracoHideTopLevelNodeFromPath"]);
                return false;
            }
        }

		/// <summary>
		/// Gets a value indicating whether [enable stat].
		/// </summary>
		/// <value><c>true</c> if [enable stat]; otherwise, <c>false</c>.</value>
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

		/// <summary>
		/// Gets a value indicating whether [disable logging].
		/// </summary>
		/// <value><c>true</c> if [disable logging]; otherwise, <c>false</c>.</value>
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

		/// <summary>
		/// Gets a value indicating whether [enable async logging].
		/// </summary>
		/// <value><c>true</c> if [enable async logging]; otherwise, <c>false</c>.</value>
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

		/// <summary>
		/// Gets a value indicating whether [enable async stat logging].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable async stat logging]; otherwise, <c>false</c>.
		/// </value>
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

		/// <summary>
		/// Gets the current version.
		/// </summary>
		/// <value>The current version.</value>
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
                    string versionComment = string.Empty;
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

		/// <summary>
		/// Gets the license.
		/// </summary>
		/// <value>The license.</value>
        public static string License
        {
            get
            {
                string license =
                    "<A href=\"http://Umbraco.org/redir/license\" target=\"_blank\">the open source license MIT</A>. The Umbraco UI is freeware licensed under the Umbraco license.";
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

                        if (licensedTo != string.Empty && licenseUrl != string.Empty)
                        {
                            license = "Umbraco Commercial License<br/><b>Registered to:</b><br/>" +
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

		[Obsolete]
        public static bool Test
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
					return false;
				}
            }
        }
    }
}