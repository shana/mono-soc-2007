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
		private const int DEFAULT_TIMEOUT = 20;

		private const string WEB_CONFIG_FILENAME = "web.config";
		private const string APPSETTINGS_SECTION_NAME = "appSettings";
		private const string UMBRACO_URL_FORBIDDEN_CHARACTERS = "umbracoUrlForbittenCharacters";

		private const string SETTINGS_ENABLE_STAT = "umbracoEnableStat";
		private const string SETTINGS_PATH = "umbracoPath";
		private const string SETTINGS_CONFIG_STATUS = "umbracoConfigurationStatus";
		private const string SETTINGS_RESERVED_PATHS = "umbracoReservedPaths";
		private const string SETTING_RESERVED_URLS = "umbracoReservedUrls";
		private const string SETTINGS_CONTENT_XML = "umbracoContentXML";
		private const string SETTINGS_STORAGE_DIRECTORY = "umbracoStorageDirectory";
		private const string SETTINGS_DEBUG_MODE = "umbracoDebugMode";
		private const string SETTINGS_TIMEOUT = "umbracoTimeOutInMinutes";
		private const string SETTINGS_USE_DIRECTORY_URLS = "umbracoUseDirectoryUrls";
		private const string SETTINGS_DISABLE_VERSION_CHECK = "umbracoDisableVersionCheck";
		private const string SETTINGS_URL_SPACE_CHARACTER = "umbracoUrlSpaceCharacter";
		private const string SETTINGS_SMTP_SERVER = "umbracoSmtpServer";
		private const string SETTINGS_DISABLE_XSLT_EXTENSIONS = "umbracoDisableXsltExtensions";
		private const string SETTINGS_EDIT_XHTML_MODE = "umbracoEditXhtmlMode";
		private const string SETTINGS_DEFAULT_UI_LANGUAGE = "umbracoDefaultUILanguage";
		private const string SETTINGS_PROFILE_URI = "umbracoProfileUrl";
		private const string SETTINGS_HIDE_TOP_LEVEL_NODE_FROM_PATH = "umbracoHideTopLevelNodeFromPath";
		private const string SETTINGS_DISABLE_LOGGING = "umbracoDisableLogging";
		private const string SETTINGS_ASYNC_LOGGING = "umbracoAsyncLogging";
		private const string SETTINGS_ASYNC_STAT_LOGGING = "umbracoAsyncStatLogging";

		/// <summary>
		/// Gets the reserved urls.
		/// </summary>
		/// <value>The reserved urls.</value>
		public static string ReservedUrls
		{
			get
			{
				if (HttpContext.Current != null)
				{
					return ConfigurationManager.AppSettings[SETTING_RESERVED_URLS];
				}
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
					return ConfigurationManager.AppSettings[SETTINGS_RESERVED_PATHS];
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
				// TODO: Consider exception handling
				return ConfigurationManager.AppSettings[SETTINGS_CONTENT_XML];
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
				// TODO: Consider exception handling
				return ConfigurationManager.AppSettings[SETTINGS_STORAGE_DIRECTORY];
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
				// TODO: Consider exception handling
				return ConfigurationManager.AppSettings[SETTINGS_PATH];
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
				return ConfigurationManager.AppSettings["umbracoDbDSN"];
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
				// TODO: Consider exception handling
				return ConfigurationManager.AppSettings[SETTINGS_CONFIG_STATUS];
			}

			set
			{
				ExeConfigurationFileMap webConfig = new ExeConfigurationFileMap();
				webConfig.ExeConfigFilename = FullpathToRoot + WEB_CONFIG_FILENAME;

				Configuration config =
					ConfigurationManager.OpenMappedExeConfiguration(webConfig, ConfigurationUserLevel.None);
				config.AppSettings.Settings[SETTINGS_CONFIG_STATUS].Value = value;
				config.Save();
				ConfigurationManager.RefreshSection(APPSETTINGS_SECTION_NAME);
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
				{
					return DbDSN;
				}
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
				// TODO: Consider exception handling
				return bool.Parse(ConfigurationManager.AppSettings[SETTINGS_DEBUG_MODE]);
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
				// TODO: Remove hard coded strings
				string configStatus = ConfigurationStatus;
				string currentVersion = CurrentVersion;
				if (currentVersion != configStatus)
				{
					Log.Add(LogTypes.Debug, User.GetUser(0), -1,
							"CurrentVersion different from configStatus: '" + currentVersion + "','" + configStatus +
							"'");
				}

				return (configStatus == currentVersion);
			}
		}

		/// <summary>
		/// Gets the time out in minutes.
		/// </summary>
		/// <value>The time out in minutes.</value>
		public static int TimeoutInMinutes
		{
			get
			{
				int output = ParseInt32(ConfigurationManager.AppSettings[SETTINGS_TIMEOUT]);
				
				if (output == -1)
				{
					output = DEFAULT_TIMEOUT;
				}
				return output;
			}			
		}

		/// <summary>
		/// Gets a value indicating whether [use directory urls].
		/// </summary>
		/// <value><c>true</c> if [use directory urls]; otherwise, <c>false</c>.</value>
		public static bool UseDirectoryUrls
		{
			get
			{
				string value = ConfigurationManager.AppSettings[SETTINGS_USE_DIRECTORY_URLS];
				return ParseBoolean(value);
			}
		}

		/// <summary>
		/// Parses to int32.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		private static int ParseInt32(string value)
		{
			int output;

			if (!string.IsNullOrEmpty(value) && (int.TryParse(value, out output)))
			{
				return output;
			}
			else
			{
				return -1;
			}
		}

		/// <summary>
		/// Parses to boolean.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		private static bool ParseBoolean(string value)
		{
			bool output;
				
			if (!string.IsNullOrEmpty(value) && (bool.TryParse(value, out output)))
			{
				return output;
			}
			else
			{
				return false;
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
				{
					return ConfigurationManager.AppSettings[SETTINGS_DISABLE_VERSION_CHECK];
				}
				return string.Empty;
			}
		}

		/// <summary>
		/// Gets the URL forbidden characters.
		/// </summary>
		/// <value>The URL forbidden characters.</value>
		public static string UrlForbiddenCharacters
		{
			get
			{
				if (HttpContext.Current != null)
					return ConfigurationManager.AppSettings[UMBRACO_URL_FORBIDDEN_CHARACTERS];
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
					return ConfigurationManager.AppSettings[SETTINGS_URL_SPACE_CHARACTER];
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
				// TODO: Consider exception handling
				return ConfigurationManager.AppSettings[SETTINGS_SMTP_SERVER];
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
				{
					return ConfigurationManager.AppSettings[SETTINGS_DISABLE_XSLT_EXTENSIONS];
				}
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
				{
					return ConfigurationManager.AppSettings[SETTINGS_EDIT_XHTML_MODE];
				}
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
				{
					return ConfigurationManager.AppSettings[SETTINGS_DEFAULT_UI_LANGUAGE];
				}
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
				{
					return ConfigurationManager.AppSettings[SETTINGS_PROFILE_URI];
				}
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
				{
					return bool.Parse(ConfigurationManager.AppSettings[SETTINGS_HIDE_TOP_LEVEL_NODE_FROM_PATH]);
				}
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
				string value = ConfigurationManager.AppSettings[SETTINGS_ENABLE_STAT];
				bool result;
				if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out result))
				{
					return result;
				}
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
				string value = ConfigurationManager.AppSettings[SETTINGS_DISABLE_LOGGING];
				bool result;
				if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out result))
				{
					return result;
				}
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
				string value = ConfigurationManager.AppSettings[SETTINGS_ASYNC_LOGGING];
				bool result;
				if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out result))
				{
					return result;
				}
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
				string value = ConfigurationManager.AppSettings[SETTINGS_ASYNC_STAT_LOGGING];
				bool result;
				if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out result))
				{
					return result;
				}
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
					versionMajor + "." + versionMinor + "." + versionPatch + versionComment;
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
					// TODO: Consider exception handling
					string licenseUrl = versionDoc.SelectSingleNode("/version/licensing/licenseUrl").FirstChild.Value;					
					// TODO: Remove unused code
					string licenseValidation = versionDoc.SelectSingleNode("/version/licensing/licenseValidation").FirstChild.Value;
					string licensedTo = versionDoc.SelectSingleNode("/version/licensing/licensedTo").FirstChild.Value;

					if (licensedTo != string.Empty && licenseUrl != string.Empty)
					{
						license = "Umbraco Commercial License<br/><b>Registered to:</b><br/>" +
								  licensedTo.Replace("\n", "<br/>") + "<br/><b>For use with domain:</b><br/>" +
								  licenseUrl;
					}
				}
				return license;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="GlobalSettings"/> is test.
		/// </summary>
		/// <value><c>true</c> if test; otherwise, <c>false</c>.</value>
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
					HttpContext.Current.Response.Write("TimeOutInMinutes :" + TimeoutInMinutes + "\n");
					HttpContext.Current.Response.Write("UrlForbittenCharacters :" + UrlForbiddenCharacters + "\n");
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