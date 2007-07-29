using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Umbraco.BusinessLogic;

namespace Umbraco
{
	/// <summary>
	/// This class accesses the settings configuration file and reads its data
	/// </summary>
	// TODO: Consider using .NET builtin configuration classes
	public static class UmbracoSettings
	{
		// TODO: If we use .NET config files these two lines would not be needed
		private const string CONFIG_FILENAME = "umbracoSettings.config";
		private static readonly string configurationPath = GlobalSettings.FullpathToRoot + Path.DirectorySeparatorChar + "config" +
									  Path.DirectorySeparatorChar;

		/// <summary>
		/// Gets the _Umbraco settings.
		/// </summary>
		/// <value>The _Umbraco settings.</value>
		public static XmlDocument UmbracoSettingsDocument
		{
			get
			{
				if (HttpRuntime.Cache["umbracoSettingsFile"] != null)
					EnsureSettingsDocument();
				return (XmlDocument)HttpRuntime.Cache["umbracoSettingsFile"];
			}
		}

		/// <summary>
		/// Ensures the settings document.
		/// </summary>
		private static void EnsureSettingsDocument()
		{
			object settingsFile = HttpRuntime.Cache["umbracoSettingsFile"];

			// Check for language file in cache
			if (settingsFile == null)
			{
				XmlDocument temp = new XmlDocument();
				XmlTextReader settingsReader = new XmlTextReader(ConfigurationPath + CONFIG_FILENAME);
				try
				{
					temp.Load(settingsReader);
					HttpRuntime.Cache.Insert("umbracoSettingsFile", temp,
											 new CacheDependency(ConfigurationPath + CONFIG_FILENAME));
				}
				catch (Exception e)
				{
					// TODO: Localize string
					Log.Add(LogTypes.Error, new User(0), -1, "Error reading umbracoSettings file: " + e);
				}
				settingsReader.Close();
			}
		}

		/// <summary>
		/// Gets the key as node.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <returns></returns>
		private static XmlNode GetKeyAsNode(string Key)
		{
			if (Key == null)
				throw new ArgumentException("Key cannot be null");
			EnsureSettingsDocument();
			if (UmbracoSettingsDocument == null || UmbracoSettingsDocument.DocumentElement == null)
				return null;
			return UmbracoSettingsDocument.DocumentElement.SelectSingleNode(Key);
		}

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <returns></returns>
		private static string GetKey(string Key)
		{
			EnsureSettingsDocument();

			XmlNode node = UmbracoSettingsDocument.DocumentElement.SelectSingleNode(Key);
			if (node == null || node.FirstChild == null || node.FirstChild.Value == null)
				return string.Empty;
			return node.FirstChild.Value;
		}

		/// <summary>
		/// Gets a value indicating whether [upload allow directories].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [upload allow directories]; otherwise, <c>false</c>.
		/// </value>
		public static bool UploadAllowDirectories
		{
			get { return bool.Parse(GetKey("/settings/content/UploadAllowDirectories")); }
		}

		/// <summary>
		/// Gets the package server.
		/// </summary>
		/// <value>The package server.</value>
		public static string PackageServer
		{
			get { return "packages.Umbraco.org"; }
		}

		/// <summary>
		/// Gets a value indicating whether [use domain prefixes].
		/// </summary>
		/// <value><c>true</c> if [use domain prefixes]; otherwise, <c>false</c>.</value>
		public static bool UseDomainPrefixes
		{
			get
			{
				// TODO: Consider Exception Handling
				bool result;
				if (bool.TryParse(GetKey("/settings/requestHandler/useDomainPrefixes"), out result))
					return result;
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether [tidy editor content].
		/// </summary>
		/// <value><c>true</c> if [tidy editor content]; otherwise, <c>false</c>.</value>
		public static bool TidyEditorContent
		{
			get { return bool.Parse(GetKey("/settings/content/TidyEditorContent")); }
		}

		/// <summary>
		/// Gets the default404 page.
		/// </summary>
		/// <value>The default404 page.</value>
		public static string Default404Page
		{
			get { return GetKey("/settings/content/errors/error404"); }
		}

		/// <summary>
		/// Gets the image file types.
		/// </summary>
		/// <value>The image file types.</value>
		public static string ImageFileTypes
		{
			get { return GetKey("/settings/content/imaging/imageFileTypes"); }
		}

		/// <summary>
		/// Gets the script file types.
		/// </summary>
		/// <value>The script file types.</value>
		public static string ScriptFileTypes
		{
			get { return GetKey("/settings/content/scripteditor/scriptFileTypes"); }
		}

		/// <summary>
		/// Gets the script folder path.
		/// </summary>
		/// <value>The script folder path.</value>
		public static string ScriptFolderPath
		{
			get { return GetKey("/settings/content/scripteditor/scriptFolderPath"); }
		}

		/// <summary>
		/// Gets a value indicating whether [script disable editor].
		/// </summary>
		/// <value><c>true</c> if [script disable editor]; otherwise, <c>false</c>.</value>
		public static bool ScriptDisableEditor
		{
			get
			{
				string _tempValue = GetKey("/settings/content/scripteditor/scriptDisableEditor");
				if (_tempValue != String.Empty)
					return bool.Parse(_tempValue);
				else
					return false;
			}
		}

		/// <summary>
		/// Gets the graphic headline format.
		/// </summary>
		/// <value>The graphic headline format.</value>
		public static string GraphicHeadlineFormat
		{
			get { return GetKey("/settings/content/graphicHeadlineFormat"); }
		}

		/// <summary>
		/// Gets a value indicating whether [ensure unique naming].
		/// </summary>
		/// <value><c>true</c> if [ensure unique naming]; otherwise, <c>false</c>.</value>
		public static bool EnsureUniqueNaming
		{
			get
			{
				// TODO: Consider exception Handling
				return bool.Parse(GetKey("/settings/content/ensureUniqueNaming"));
			}
		}

		/// <summary>
		/// Gets the notification email sender.
		/// </summary>
		/// <value>The notification email sender.</value>
		public static string NotificationEmailSender
		{
			get { return GetKey("/settings/content/notifications/email"); }
		}

		/// <summary>
		/// Gets a value indicating whether [notification disable HTML email].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [notification disable HTML email]; otherwise, <c>false</c>.
		/// </value>
		public static bool NotificationDisableHtmlEmail
		{
			get
			{
				string _tempValue = GetKey("/settings/content/notifications/disableHtmlEmail");
				if (_tempValue != String.Empty)
					return bool.Parse(_tempValue);
				else
					return false;
			}
		}

		/// <summary>
		/// Gets the image allowed attributes.
		/// </summary>
		/// <value>The image allowed attributes.</value>
		public static string ImageAllowedAttributes
		{
			get { return GetKey("/settings/content/imaging/allowedAttributes"); }
		}

		/// <summary>
		/// Gets the scheduled tasks.
		/// </summary>
		/// <value>The scheduled tasks.</value>
		public static XmlNode ScheduledTasks
		{
			get { return GetKeyAsNode("/settings/scheduledTasks"); }
		}

		/// <summary>
		/// Gets the URL replace characters.
		/// </summary>
		/// <value>The URL replace characters.</value>
		public static XmlNode UrlReplaceCharacters
		{
			get { return GetKeyAsNode("/settings/requestHandler/urlReplacing"); }
		}

		/// <summary>
		/// Gets a value indicating whether [use distributed calls].
		/// </summary>
		/// <value><c>true</c> if [use distributed calls]; otherwise, <c>false</c>.</value>
		public static bool UseDistributedCalls
		{
			get
			{
				// TODO: Consider exception Handling
				return bool.Parse(GetKeyAsNode("/settings/distributedCall").Attributes.GetNamedItem("enable").Value);
			}
		}


		/// <summary>
		/// Gets the distributed call user.
		/// </summary>
		/// <value>The distributed call user.</value>
		public static int DistributedCallUser
		{
			get
			{
				// TODO: Consider exception Handling
				return int.Parse(GetKey("/settings/distributedCall/user"));
			}
		}

		/// <summary>
		/// Gets the distribution servers.
		/// </summary>
		/// <value>The distribution servers.</value>
		public static XmlNode DistributionServers
		{
			get
			{
				// TODO: Consider exception Handling
				return GetKeyAsNode("/settings/distributedCall/servers");
			}
		}

		/// <summary>
		/// Gets a value indicating whether [use viewstate mover module].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [use viewstate mover module]; otherwise, <c>false</c>.
		/// </value>
		public static bool UseViewstateMoverModule
		{
			get
			{
				// TODO: Consider exception Handling
				return
					bool.Parse(
						GetKeyAsNode("/settings/viewstateMoverModule").Attributes.GetNamedItem("enable").Value);
			}
		}


		/// <summary>
		/// Tells us whether the Xml Content cache is disabled or not
		/// Default is enabled
		/// </summary>
		public static bool IsXmlContentCacheDisabled
		{
			get
			{
				// TODO: Consider exception Handling
				bool xmlCacheEnabled;
				string value = GetKey("/settings/content/XmlCacheEnabled");
				if (bool.TryParse(value, out xmlCacheEnabled))
					return !xmlCacheEnabled;
				// Return default
				return false;
			}
		}

		/// <summary>
		/// Tells us whether the Xml to always update disk cache, when changes are made to content
		/// Default is enabled
		/// </summary>
		public static bool ContinouslyUpdateXmlDiskCache
		{
			get
			{
				// TODO: Consider exception Handling
				bool updateDiskCache;
				string value = GetKey("/settings/content/ContinouslyUpdateXmlDiskCache");
				if (bool.TryParse(value, out updateDiskCache))
					return updateDiskCache;
				// Return default
				return false;
			}
		}

		/// <summary>
		/// Tells us whether to use a splash page while Umbraco is initializing content. 
		/// If not, requests are queued while Umbraco loads content. For very large sites (+10k nodes) it might be usefull to 
		/// have a splash page
		/// Default is disabled
		/// </summary>
		public static bool EnableSplashWhileLoading
		{
			get
			{
				// TODO: Consider exception Handling
				bool updateDiskCache;
				string value = GetKey("/settings/content/EnableSplashWhileLoading");
				if (bool.TryParse(value, out updateDiskCache))
					return updateDiskCache;
				// Return default
				return false;
			}
		}

		/// <summary>
		/// Gets the configuration path.
		/// </summary>
		/// <value>The configuration path.</value>
		public static string ConfigurationPath
		{
			get { return configurationPath; }
		}

		/// <summary>
		/// Configuration regarding webservices
		/// </summary>
		/// <remarks>Put in seperate class for more logik/seperation</remarks>
		public static class Webservices
		{
			/// <summary>
			/// Gets a value indicating whether this <see cref="Webservices"/> is enabled.
			/// </summary>
			/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
			public static bool Enabled
			{
				get
				{
					// TODO: Consider exception Handling
					return
						bool.Parse(GetKeyAsNode("/settings/webservices").Attributes.GetNamedItem("enabled").Value);
				}
			}

			#region "Webservice configuration"

			/// <summary>
			/// Gets the document service users.
			/// </summary>
			/// <value>The document service users.</value>
			public static string[] DocumentServiceUsers
			{
				get
				{
					// TODO: Consider exception Handling
					return GetKey("/settings/webservices/documentServiceUsers").Split(',');
				}
			}

			/// <summary>
			/// Gets the file service users.
			/// </summary>
			/// <value>The file service users.</value>
			public static string[] FileServiceUsers
			{
				get
				{
					// TODO: Consider exception Handling
					return GetKey("/settings/webservices/fileServiceUsers").Split(',');
				}
			}


			/// <summary>
			/// Gets the file service folders.
			/// </summary>
			/// <value>The file service folders.</value>
			public static string[] FileServiceFolders
			{
				get
				{
					// TODO: Consider exception Handling
					return GetKey("/settings/webservices/fileServiceFolders").Split(',');
				}
			}

			/// <summary>
			/// Gets the member service users.
			/// </summary>
			/// <value>The member service users.</value>
			public static string[] MemberServiceUsers
			{
				get
				{
					// TODO: Consider exception Handling
					return GetKey("/settings/webservices/memberServiceUsers").Split(',');
				}
			}

			/// <summary>
			/// Gets the stylesheet service users.
			/// </summary>
			/// <value>The stylesheet service users.</value>
			public static string[] StylesheetServiceUsers
			{
				get
				{
					// TODO: Consider exception Handling
					return GetKey("/settings/webservices/stylesheetServiceUsers").Split(',');
				}
			}

			/// <summary>
			/// Gets the template service users.
			/// </summary>
			/// <value>The template service users.</value>
			public static string[] TemplateServiceUsers
			{
				get
				{
					// TODO: Consider exception handling
					return GetKey("/settings/webservices/templateServiceUsers").Split(',');
				}
			}

			/// <summary>
			/// Gets the media service users.
			/// </summary>
			/// <value>The media service users.</value>
			public static string[] MediaServiceUsers
			{
				get
				{
					// TODO: Consider exception Handling
					return GetKey("/settings/webservices/mediaServiceUsers").Split(',');
				}
			}


			/// <summary>
			/// Gets the maintenance service users.
			/// </summary>
			/// <value>The maintenance service users.</value>
			public static string[] MaintenanceServiceUsers
			{
				get
				{
					// TODO: Consider exception Handling
					return GetKey("/settings/webservices/maintenanceServiceUsers").Split(',');
				}
			}

			#endregion
		}
	}
}