using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Xml;
using umbraco.BusinessLogic;

namespace umbraco
{
    /// <summary>
    /// Summary description for UmbracoSettings.
    /// </summary>
    public class UmbracoSettings
    {
        public static XmlDocument _umbracoSettings
        {
            get
            {
                if (HttpRuntime.Cache["umbracoSettingsFile"] != null)
                    ensureSettingsDocument();
                return (XmlDocument) HttpRuntime.Cache["umbracoSettingsFile"];
            }
        }

        private static string _path = GlobalSettings.FullpathToRoot + Path.DirectorySeparatorChar + "config" +
                                      Path.DirectorySeparatorChar;

        private static string _filename = "umbracoSettings.config";

        private static void ensureSettingsDocument()
        {
            object settingsFile = HttpRuntime.Cache["umbracoSettingsFile"];

            // Check for language file in cache
            if (settingsFile == null)
            {
                XmlDocument temp = new XmlDocument();
                XmlTextReader settingsReader = new XmlTextReader(_path + _filename);
                try
                {
                    temp.Load(settingsReader);
                    HttpRuntime.Cache.Insert("umbracoSettingsFile", temp,
                                             new CacheDependency(_path + _filename));
                }
                catch (Exception e)
                {
                    Log.Add(LogTypes.Error, new User(0), -1, "Error reading umbracoSettings file: " + e.ToString());
                }
                settingsReader.Close();
            }
        }

        private static void save()
        {
            _umbracoSettings.Save(_path + _filename);
        }


        private static XmlNode GetKeyAsNode(string Key)
        {
            if (Key == null)
                throw new ArgumentException("Key cannot be null");
            ensureSettingsDocument();
            if (_umbracoSettings == null || _umbracoSettings.DocumentElement == null)
                return null;
            return _umbracoSettings.DocumentElement.SelectSingleNode(Key);
        }

        private static string GetKey(string Key)
        {
            ensureSettingsDocument();

            XmlNode node = _umbracoSettings.DocumentElement.SelectSingleNode(Key);
            if (node == null || node.FirstChild == null || node.FirstChild.Value == null)
                return string.Empty;
            return node.FirstChild.Value;
        }

        public static bool UploadAllowDirectories
        {
            get { return bool.Parse(GetKey("/settings/content/UploadAllowDirectories")); }
        }

        public static string PackageServer
        {
            get { return "packages.umbraco.org"; }
        }

        public static bool UseDomainPrefixes
        {
            get
            {
                try
                {
                    bool result;
                    if (bool.TryParse(GetKey("/settings/requestHandler/useDomainPrefixes"), out result))
                        return result;
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool TidyEditorContent
        {
            get { return bool.Parse(GetKey("/settings/content/TidyEditorContent")); }
        }

        public static string Default404Page
        {
            get { return GetKey("/settings/content/errors/error404"); }
        }

        public static string ImageFileTypes
        {
            get { return GetKey("/settings/content/imaging/imageFileTypes"); }
        }

        public static string ScriptFileTypes
        {
            get { return GetKey("/settings/content/scripteditor/scriptFileTypes"); }
        }

        public static string ScriptFolderPath
        {
            get { return GetKey("/settings/content/scripteditor/scriptFolderPath"); }
        }

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

        public static string GraphicHeadlineFormat
        {
            get { return GetKey("/settings/content/graphicHeadlineFormat"); }
        }

        public static bool EnsureUniqueNaming
        {
            get
            {
                try
                {
                    return bool.Parse(GetKey("/settings/content/ensureUniqueNaming"));
                }
                catch
                {
                    return false;
                }
            }
        }

        public static string NotificationEmailSender
        {
            get { return GetKey("/settings/content/notifications/email"); }
        }

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

        public static string ImageAllowedAttributes
        {
            get { return GetKey("/settings/content/imaging/allowedAttributes"); }
        }

        public static XmlNode ScheduledTasks
        {
            get { return GetKeyAsNode("/settings/scheduledTasks"); }
        }

        public static XmlNode UrlReplaceCharacters
        {
            get { return GetKeyAsNode("/settings/requestHandler/urlReplacing"); }
        }

        public static bool UseDistributedCalls
        {
            get
            {
                try
                {
                    return bool.Parse(GetKeyAsNode("/settings/distributedCall").Attributes.GetNamedItem("enable").Value);
                }
                catch
                {
                    return false;
                }
            }
        }


        public static int DistributedCallUser
        {
            get
            {
                try
                {
                    return int.Parse(GetKey("/settings/distributedCall/user"));
                }
                catch
                {
                    return -1;
                }
            }
        }

        public static XmlNode DistributionServers
        {
            get
            {
                try
                {
                    return GetKeyAsNode("/settings/distributedCall/servers");
                }
                catch
                {
                    return null;
                }
            }
        }

        public static bool UseViewstateMoverModule
        {
            get
            {
                try
                {
                    return
                        bool.Parse(
                            GetKeyAsNode("/settings/viewstateMoverModule").Attributes.GetNamedItem("enable").Value);
                }
                catch
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// Tells us whether the Xml Content cache is disabled or not
        /// Default is enabled
        /// </summary>
        public static bool isXmlContentCacheDisabled
        {
            get
            {
                try
                {
                    bool xmlCacheEnabled;
                    string value = GetKey("/settings/content/XmlCacheEnabled");
                    if (bool.TryParse(value, out xmlCacheEnabled))
                        return !xmlCacheEnabled;
                    // Return default
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Tells us whether the Xml to always update disk cache, when changes are made to content
        /// Default is enabled
        /// </summary>
        public static bool continouslyUpdateXmlDiskCache
        {
            get
            {
                try
                {
                    bool updateDiskCache;
                    string value = GetKey("/settings/content/ContinouslyUpdateXmlDiskCache");
                    if (bool.TryParse(value, out updateDiskCache))
                        return updateDiskCache;
                    // Return default
                    return false;
                }
                catch
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Tells us whether to use a splash page while umbraco is initializing content. 
        /// If not, requests are queued while umbraco loads content. For very large sites (+10k nodes) it might be usefull to 
        /// have a splash page
        /// Default is disabled
        /// </summary>
        public static bool EnableSplashWhileLoading
        {
            get
            {
                try
                {
                    bool updateDiskCache;
                    string value = GetKey("/settings/content/EnableSplashWhileLoading");
                    if (bool.TryParse(value, out updateDiskCache))
                        return updateDiskCache;
                    // Return default
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Configuration regarding webservices
        /// </summary>
        /// <remarks>Put in seperate class for more logik/seperation</remarks>
        public class Webservices
        {
            public static bool Enabled
            {
                get
                {
                    try
                    {
                        return
                            bool.Parse(GetKeyAsNode("/settings/webservices").Attributes.GetNamedItem("enabled").Value);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            #region "Webservice configuration"

            public static string[] documentServiceUsers
            {
                get
                {
                    try
                    {
                        return GetKey("/settings/webservices/documentServiceUsers").Split(',');
                    }
                    catch
                    {
                        return new string[0];
                    }
                }
            }

            public static string[] fileServiceUsers
            {
                get
                {
                    try
                    {
                        return GetKey("/settings/webservices/fileServiceUsers").Split(',');
                    }
                    catch
                    {
                        return new string[0];
                    }
                }
            }


            public static string[] fileServiceFolders
            {
                get
                {
                    try
                    {
                        return GetKey("/settings/webservices/fileServiceFolders").Split(',');
                    }
                    catch
                    {
                        return new string[0];
                    }
                }
            }

            public static string[] memberServiceUsers
            {
                get
                {
                    try
                    {
                        return GetKey("/settings/webservices/memberServiceUsers").Split(',');
                    }
                    catch
                    {
                        return new string[0];
                    }
                }
            }

            public static string[] stylesheetServiceUsers
            {
                get
                {
                    try
                    {
                        return GetKey("/settings/webservices/stylesheetServiceUsers").Split(',');
                    }
                    catch
                    {
                        return new string[0];
                    }
                }
            }

            public static string[] templateServiceUsers
            {
                get
                {
                    try
                    {
                        return GetKey("/settings/webservices/templateServiceUsers").Split(',');
                    }
                    catch
                    {
                        return new string[0];
                    }
                }
            }

            public static string[] mediaServiceUsers
            {
                get
                {
                    try
                    {
                        return GetKey("/settings/webservices/mediaServiceUsers").Split(',');
                    }
                    catch
                    {
                        return new string[0];
                    }
                }
            }


            public static string[] maintenanceServiceUsers
            {
                get
                {
                    try
                    {
                        return GetKey("/settings/webservices/maintenanceServiceUsers").Split(',');
                    }
                    catch
                    {
                        return new string[0];
                    }
                }
            }

            #endregion
        }
    }
}