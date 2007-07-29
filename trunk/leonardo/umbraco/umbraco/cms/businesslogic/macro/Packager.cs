using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;

using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

using Umbraco.Cms.BusinessLogic.web;
using Umbraco.Cms.BusinessLogic.propertytype;
using Umbraco.BusinessLogic;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;
using System.Data;

namespace Umbraco.Cms.BusinessLogic.macro
{
    /// <summary>
    /// The packager is a component which enables sharing of both data and functionality components between different Umbraco installations.
    /// 
    /// The output is a .umb (a zip compressed file) which contains the exported documents/medias/macroes/documenttypes (etc.)
    /// in a Xml document, along with the physical files used (images/usercontrols/xsl documents etc.)
    /// 
    /// Partly implemented, import of packages is done, the export is *under construction*.
    /// </summary>
    public class Packager
    {
        private string _name;
        private string _version;
        private string _url;
        private string _license;
        private string _licenseUrl;
        private int _reqMajor;
        private int _reqMinor;
        private int _reqPatch;
        private string _authorName;
        private string _authorUrl;
        private string _readme;
        private string _control;
        private ArrayList _macros = new ArrayList();
        private XmlDocument _packageConfig;

        public string Name { get { return _name; } }
        public string Version { get { return _version; } }
        public string Url { get { return _url; } }
        public string License { get { return _license; } }
        public string LicenseUrl { get { return _licenseUrl; } }
        public string Author { get { return _authorName; } }
        public string AuthorUrl { get { return _authorUrl; } }
        public string ReadMe { get { return _readme; } }
        public string Control { get { return _control; } }
        public int RequirementsMajor { get { return _reqMajor; } }
        public int RequirementsMinor { get { return _reqMinor; } }
        public int RequirementsPatch { get { return _reqPatch; } }

        /// <summary>
        /// The xmldocument, describing the contents of a package.
        /// </summary>
        public XmlDocument Config
        {
            get { return _packageConfig; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Packager()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param Name="Name">The Name of the package</param>
        /// <param Name="Version">The version of the package</param>
        /// <param Name="Url">The url to a descriptionpage</param>
        /// <param Name="License">The license under which the package is released (preferably GPL ;))</param>
        /// <param Name="LicenseUrl">The url to a licensedescription</param>
        /// <param Name="Author">The original author of the package</param>
        /// <param Name="AuthorUrl">The url to the Authors website</param>
        /// <param Name="RequirementsMajor">Umbraco version major</param>
        /// <param Name="RequirementsMinor">Umbraco version minor</param>
        /// <param Name="RequirementsPatch">Umbraco version patch</param>
        /// <param Name="Readme">The readme text</param>
        /// <param Name="Control">The Name of the usercontrol used to configure the package after install</param>
        public Packager(string Name, string Version, string Url, string License, string LicenseUrl, string Author, string AuthorUrl, int RequirementsMajor, int RequirementsMinor, int RequirementsPatch, string Readme, string Control)
        {
            _name = Name;
            _version = Version;
            _url = Url;
            _license = License;
            _licenseUrl = LicenseUrl;
            _reqMajor = RequirementsMajor;
            _reqMinor = RequirementsMinor;
            _reqPatch = RequirementsPatch;
            _authorName = Author;
            _authorUrl = AuthorUrl;
            _readme = Readme;
            _control = Control;
        }

        /// <summary>
        /// Adds the macro to the package
        /// </summary>
        /// <param Name="MacroToAdd">Macro to add</param>
        public void AddMacro(Macro MacroToAdd)
        {
            _macros.Add(MacroToAdd);
        }


        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param Name="OutputPath">Not implemented</param>
        public void Package(string OutputPath)
        {
        }

        /// <summary>
        /// Imports the specified package
        /// </summary>
        /// <param Name="InputFile">Filename of the umbracopackage</param>
        /// <returns></returns>
        public string Import(string InputFile)
        {
            string tempDir = "";
            if (File.Exists(HttpContext.Current.Server.MapPath(GlobalSettings.StorageDirectory + Path.DirectorySeparatorChar + InputFile)))
            {
                FileInfo fi = new FileInfo(HttpContext.Current.Server.MapPath(GlobalSettings.StorageDirectory + Path.DirectorySeparatorChar + InputFile));
                // Check if the file is a valid package
                if (fi.Extension.ToLower() == ".umb")
                {
                    try
                    {
                        tempDir = unPack(fi.FullName);
                        LoadConfig(tempDir);
                    }
                    catch (Exception unpackE)
                    {
                        throw new Exception("Error unpacking extension...", unpackE);
                    }
                }
                else
                    throw new Exception("Error - file isn't a package (doesn't have a .umb extension). Check if the file automatically got named '.zip' upon download.");
            }
            else
                throw new Exception("Error - file not found. Could find file named '" + HttpContext.Current.Server.MapPath(GlobalSettings.StorageDirectory + Path.DirectorySeparatorChar + InputFile) + "'");
            return tempDir;
        }


        /// <summary>
        /// Invoking this method installs the current package
        /// </summary>
        /// <param Name="tempDir">Temporary folder where the package's content are extracted to</param>
        public void Install(string tempDir)
        {

            callCommands("start");

            // Install macros
            foreach (XmlNode n in _packageConfig.DocumentElement.SelectNodes("//macro"))
            {
                Cms.BusinessLogic.macro.Macro m = Cms.BusinessLogic.macro.Macro.MakeNew(XmlHelper.GetNodeValue(n.SelectSingleNode("Name")));
                m.Alias = XmlHelper.GetNodeValue(n.SelectSingleNode("alias"));
                m.Assembly = XmlHelper.GetNodeValue(n.SelectSingleNode("scriptAssembly"));
                m.Type = XmlHelper.GetNodeValue(n.SelectSingleNode("scriptType"));
                m.Xslt = XmlHelper.GetNodeValue(n.SelectSingleNode("xslt"));
                m.RefreshRate = int.Parse(XmlHelper.GetNodeValue(n.SelectSingleNode("refreshRate")));
                try
                {
                    m.UseInEditor = bool.Parse(XmlHelper.GetNodeValue(n.SelectSingleNode("useInEditor")));
                }
                catch
                { }

                // macro properties
                foreach (XmlNode mp in n.SelectNodes("properties/property"))
                {
                    try
                    {
                        Cms.BusinessLogic.macro.MacroProperty.MakeNew(
                            m,
                            bool.Parse(mp.Attributes.GetNamedItem("show").Value),
                            mp.Attributes.GetNamedItem("alias").Value,
                            mp.Attributes.GetNamedItem("Name").Value,
                            new Cms.BusinessLogic.macro.MacroPropertyType(mp.Attributes.GetNamedItem("propertyType").Value)
                            );
                    }
                    catch (Exception macroPropertyExp)
                    {
                        BusinessLogic.Log.Add(BusinessLogic.LogTypes.Error, BusinessLogic.User.GetUser(0), -1, "Error creating macro property: " + macroPropertyExp.ToString());
                    }
                }
            }

            // Copy files
            string appPath = System.Web.HttpContext.Current.Request.ApplicationPath;
            if (appPath == "/")
                appPath = "";
            foreach (XmlNode n in _packageConfig.DocumentElement.SelectNodes("//file"))
            {
                if (!Directory.Exists(HttpContext.Current.Server.MapPath(appPath + XmlHelper.GetNodeValue(n.SelectSingleNode("orgPath")))))
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(appPath + XmlHelper.GetNodeValue(n.SelectSingleNode("orgPath"))));
                File.Copy(tempDir + Path.DirectorySeparatorChar + XmlHelper.GetNodeValue(n.SelectSingleNode("guid")), HttpContext.Current.Server.MapPath(appPath + XmlHelper.GetNodeValue(n.SelectSingleNode("orgPath")) + Path.DirectorySeparatorChar + XmlHelper.GetNodeValue(n.SelectSingleNode("orgName"))), true);
                File.Delete(tempDir + Path.DirectorySeparatorChar + XmlHelper.GetNodeValue(n.SelectSingleNode("guid")));
            }


            // Get current user
            BasePages.UmbracoEnsuredPage uep = new Umbraco.BasePages.UmbracoEnsuredPage();
            BusinessLogic.User u = uep.ValidatedUser;

            // Add Templates
            foreach (XmlNode n in _packageConfig.DocumentElement.SelectNodes("Templates/Template"))
            {
                template.Template t = template.Template.MakeNew(XmlHelper.GetNodeValue(n.SelectSingleNode("Name")), u);
                t.Alias = XmlHelper.GetNodeValue(n.SelectSingleNode("Alias"));
                t.Design = XmlHelper.GetNodeValue(n.SelectSingleNode("Design"));
            }

            // Add master templates
            foreach (XmlNode n in _packageConfig.DocumentElement.SelectNodes("Templates/Template"))
            {
                string master = XmlHelper.GetNodeValue(n.SelectSingleNode("Master"));
                if (master.Trim() != "")
                {
                    template.Template t = template.Template.GetByAlias(XmlHelper.GetNodeValue(n.SelectSingleNode("Alias")));
                    template.Template masterTemplate = template.Template.GetByAlias(master);
                    if (masterTemplate != null)
                        t.MasterTemplate = template.Template.GetByAlias(master).Id;
                }
            }

            // Add documenttypes

            foreach (XmlNode n in _packageConfig.DocumentElement.SelectNodes("DocumentTypes/DocumentType"))
            {
                ImportDocumentType(n, u, false);
            }

            // Add documenttype structure
            foreach (XmlNode n in _packageConfig.DocumentElement.SelectNodes("DocumentTypes/DocumentType"))
            {
                DocumentType dt = DocumentType.GetByAlias(XmlHelper.GetNodeValue(n.SelectSingleNode("Info/Alias")));
                if (dt != null)
                {
                    ArrayList allowed = new ArrayList();
                    foreach (XmlNode structure in n.SelectNodes("Structure/DocumentType"))
                    {
                        DocumentType dtt = DocumentType.GetByAlias(XmlHelper.GetNodeValue(structure));
                        allowed.Add(dtt.Id);
                    }
                    int[] adt = new int[allowed.Count];
                    for (int i = 0; i < allowed.Count; i++)
                        adt[i] = (int)allowed[i];
                    dt.AllowedChildContentTypeIDs = adt;
                }
            }

            // Stylesheets
            foreach (XmlNode n in _packageConfig.DocumentElement.SelectNodes("Stylesheets/Stylesheet"))
            {
                StyleSheet s = StyleSheet.MakeNew(
                    u,
                    XmlHelper.GetNodeValue(n.SelectSingleNode("Name")),
                    XmlHelper.GetNodeValue(n.SelectSingleNode("FileName")),
                    XmlHelper.GetNodeValue(n.SelectSingleNode("Content")));

                foreach (XmlNode prop in n.SelectNodes("Properties/Property"))
                {
                    StylesheetProperty sp = StylesheetProperty.MakeNew(
                        XmlHelper.GetNodeValue(prop.SelectSingleNode("Name")),
                        s,
                        u);
                    sp.Alias = XmlHelper.GetNodeValue(prop.SelectSingleNode("Alias"));
                    sp.value = XmlHelper.GetNodeValue(prop.SelectSingleNode("Value"));
                }
                s.saveCssToFile();



            }

            // Documents
            foreach (XmlElement n in _packageConfig.DocumentElement.SelectNodes("Documents/DocumentSet [@importMode = 'root']/node"))
                Cms.BusinessLogic.web.Document.Import(-1, u, n);

            callCommands("end");

        }

        public static void ImportDocumentType(XmlNode n, BusinessLogic.User u, bool ImportStructure)
        {
            DocumentType dt = DocumentType.GetByAlias(XmlHelper.GetNodeValue(n.SelectSingleNode("Info/Alias")));
            if (dt == null)
            {
                dt = DocumentType.MakeNew(u, XmlHelper.GetNodeValue(n.SelectSingleNode("Info/Name")));
                dt.Alias = XmlHelper.GetNodeValue(n.SelectSingleNode("Info/Alias"));
            }
            else
            {
                dt.Text = XmlHelper.GetNodeValue(n.SelectSingleNode("Info/Name"));
            }

            // Info
            dt.IconUrl = XmlHelper.GetNodeValue(n.SelectSingleNode("Info/Icon"));
            dt.Thumbnail = XmlHelper.GetNodeValue(n.SelectSingleNode("Info/Thumbnail"));
            dt.Description = XmlHelper.GetNodeValue(n.SelectSingleNode("Info/Description"));

            // Templates	
            ArrayList templates = new ArrayList();
            foreach (XmlNode tem in n.SelectNodes("Info/AllowedTemplates/Template"))
            {
                template.Template t = template.Template.GetByAlias(XmlHelper.GetNodeValue(tem));
                if (t != null)
                    templates.Add(t);
            }

            try
            {
                template.Template[] at = new template.Template[templates.Count];
                for (int i = 0; i < templates.Count; i++)
                    at[i] = (template.Template)templates[i];
                dt.allowedTemplates = at;
            }
            catch (Exception ee)
            {
                BusinessLogic.Log.Add(BusinessLogic.LogTypes.Error, u, dt.Id, "Packager: Error handling allowed templates: " + ee.ToString());
            }

            // Default template
            try
            {
                if (XmlHelper.GetNodeValue(n.SelectSingleNode("Info/DefaultTemplate")) != "")
                    dt.DefaultTemplate = template.Template.GetByAlias(XmlHelper.GetNodeValue(n.SelectSingleNode("Info/DefaultTemplate"))).Id;
            }
            catch (Exception ee)
            {
                BusinessLogic.Log.Add(BusinessLogic.LogTypes.Error, u, dt.Id, "Packager: Error assigning default template: " + ee.ToString());
            }

            // Tabs
            Cms.BusinessLogic.ContentType.TabI[] tabs = dt.getVirtualTabs;
            string tabNames = ";";
            for (int t = 0; t < tabs.Length; t++)
                tabNames += tabs[t].Caption + ";";

            Hashtable ht = new Hashtable();
            foreach (XmlNode t in n.SelectNodes("Tabs/Tab"))
            {
                if (tabNames.IndexOf(";" + XmlHelper.GetNodeValue(t.SelectSingleNode("Caption")) + ";") == -1)
                {
                    ht.Add(int.Parse(XmlHelper.GetNodeValue(t.SelectSingleNode("Id"))),
                        dt.AddVirtualTab(XmlHelper.GetNodeValue(t.SelectSingleNode("Caption"))));
                }
            }


            // Get all tabs in hashtable
            Hashtable tabList = new Hashtable();
            foreach (Cms.BusinessLogic.ContentType.TabI t in dt.getVirtualTabs)
            {
                if (!tabList.ContainsKey(t.Caption))
                    tabList.Add(t.Caption, t.Id);
            }

            // Generic Properties
            datatype.controls.Factory f = new datatype.controls.Factory();
            foreach (XmlNode gp in n.SelectNodes("GenericProperties/GenericProperty"))
            {

                Guid dtId = new Guid(XmlHelper.GetNodeValue(gp.SelectSingleNode("Type")));
                int dfId = 0;
                foreach (datatype.DataTypeDefinition df in datatype.DataTypeDefinition.GetAll())
                    if (df.DataType.Id == dtId)
                    {
                        dfId = df.Id;
                        break;
                    }
                if (dfId != 0)
                {
                    PropertyType pt = dt.getPropertyType(XmlHelper.GetNodeValue(gp.SelectSingleNode("Alias")));
                    if (pt == null)
                    {
                        dt.AddPropertyType(
                            datatype.DataTypeDefinition.GetDataTypeDefinition(dfId),
                            XmlHelper.GetNodeValue(gp.SelectSingleNode("Alias")),
                            XmlHelper.GetNodeValue(gp.SelectSingleNode("Name"))
                            );
                        pt = dt.getPropertyType(XmlHelper.GetNodeValue(gp.SelectSingleNode("Alias")));
                    }
                    else
                    {
                        pt.DataTypeDefinition = datatype.DataTypeDefinition.GetDataTypeDefinition(dfId);
                        pt.Name = XmlHelper.GetNodeValue(gp.SelectSingleNode("Name"));
                    }

                    pt.Mandatory = bool.Parse(XmlHelper.GetNodeValue(gp.SelectSingleNode("Mandatory")));
                    pt.ValidationRegExp = XmlHelper.GetNodeValue(gp.SelectSingleNode("Validation"));
                    pt.Description = XmlHelper.GetNodeValue(gp.SelectSingleNode("Description"));

                    // tab
                    try
                    {
                        if (tabList.ContainsKey(XmlHelper.GetNodeValue(gp.SelectSingleNode("Tab"))))
                            pt.TabId = (int)tabList[XmlHelper.GetNodeValue(gp.SelectSingleNode("Tab"))];
                    }
                    catch (Exception ee)
                    {
                        BusinessLogic.Log.Add(BusinessLogic.LogTypes.Error, u, dt.Id, "Packager: Error assigning property to tab: " + ee.ToString());
                    }
                }
            }

            if (ImportStructure)
            {
                if (dt != null)
                {
                    ArrayList allowed = new ArrayList();
                    foreach (XmlNode structure in n.SelectNodes("Structure/DocumentType"))
                    {
                        DocumentType dtt = DocumentType.GetByAlias(XmlHelper.GetNodeValue(structure));
                        if (dtt != null)
                            allowed.Add(dtt.Id);
                    }
                    int[] adt = new int[allowed.Count];
                    for (int i = 0; i < allowed.Count; i++)
                        adt[i] = (int)allowed[i];
                    dt.AllowedChildContentTypeIDs = adt;
                }
            }

            // clear caching
            foreach(DocumentType.TabI t in dt.getVirtualTabs)
                DocumentType.FlushTabCache(t.Id);

        }

        private void callCommands(string runAt)
        {
            // Handle execute commands
            foreach (XmlNode n in _packageConfig.DocumentElement.SelectNodes("//command [@runAt = '" + runAt + "']"))
            {
                try
                {
                    Assembly asm = System.Reflection.Assembly.LoadFrom(HttpContext.Current.Server.MapPath("/bin/" + n.Attributes.GetNamedItem("assembly").Value + ".dll"));
                    Activator.CreateInstance(asm.GetType(n.Attributes.GetNamedItem("type").Value));
                }
                catch (Exception ex) { BusinessLogic.Log.Add(BusinessLogic.LogTypes.Error, BusinessLogic.User.GetUser(0), -1, "Error executing package command: " + ex.ToString()); }
            }

        }

        /// <summary>
        /// Reads the configuration of the package from the configuration xmldocument
        /// </summary>
        /// <param Name="tempDir">The folder to which the contents of the package is extracted</param>
        public void LoadConfig(string tempDir)
        {
            _packageConfig = new XmlDocument();
            _packageConfig.Load(tempDir + Path.DirectorySeparatorChar + "package.xml");

            _name = _packageConfig.DocumentElement.SelectSingleNode("/umbPackage/info/package/Name").FirstChild.Value;
            _version = _packageConfig.DocumentElement.SelectSingleNode("/umbPackage/info/package/version").FirstChild.Value;
            _url = _packageConfig.DocumentElement.SelectSingleNode("/umbPackage/info/package/url").FirstChild.Value;
            _license = _packageConfig.DocumentElement.SelectSingleNode("/umbPackage/info/package/license").FirstChild.Value;
            _licenseUrl = _packageConfig.DocumentElement.SelectSingleNode("/umbPackage/info/package/license").Attributes.GetNamedItem("url").Value;
            _reqMajor = int.Parse(_packageConfig.DocumentElement.SelectSingleNode("/umbPackage/info/package/requirements/major").FirstChild.Value);
            _reqMinor = int.Parse(_packageConfig.DocumentElement.SelectSingleNode("/umbPackage/info/package/requirements/major").FirstChild.Value);
            _reqPatch = int.Parse(_packageConfig.DocumentElement.SelectSingleNode("/umbPackage/info/package/requirements/patch").FirstChild.Value);
            _authorName = _packageConfig.DocumentElement.SelectSingleNode("/umbPackage/info/author/Name").FirstChild.Value;
            _authorUrl = _packageConfig.DocumentElement.SelectSingleNode("/umbPackage/info/author/website").FirstChild.Value;
            try
            {
                _readme = XmlHelper.GetNodeValue(_packageConfig.DocumentElement.SelectSingleNode("/umbPackage/info/readme"));
            }
            catch { }
            try
            {
                _control = XmlHelper.GetNodeValue(_packageConfig.DocumentElement.SelectSingleNode("/umbPackage/control"));
            }
            catch { }
        }

        private string unPack(string ZipName)
        {
            // Unzip
            string tempDir = HttpContext.Current.Server.MapPath(GlobalSettings.StorageDirectory) + Path.DirectorySeparatorChar + Guid.NewGuid().ToString();
            Directory.CreateDirectory(tempDir);

            ZipInputStream s = new ZipInputStream(File.OpenRead(ZipName));

            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = Path.GetDirectoryName(theEntry.Name);
                string fileName = Path.GetFileName(theEntry.Name);

                if (fileName != String.Empty)
                {
                    FileStream streamWriter = File.Create(tempDir + Path.DirectorySeparatorChar + fileName);

                    int size = 2048;
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        size = s.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            streamWriter.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }

                    streamWriter.Close();

                }
            }

            // Clean up
            s.Close();
            File.Delete(ZipName);

            return tempDir;

        }

        public string Fetch(Guid Package)
        {

            // Check for package directory
            if (!System.IO.Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(Umbraco.GlobalSettings.StorageDirectory + "\\packages")))
                System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(Umbraco.GlobalSettings.StorageDirectory + "\\packages"));

            System.Net.WebClient wc = new System.Net.WebClient();

            wc.DownloadFile(
                "http://" + UmbracoSettings.PackageServer + "/fetch?package=" + Package.ToString(),
                System.Web.HttpContext.Current.Server.MapPath(Umbraco.GlobalSettings.StorageDirectory + "\\packages\\" + Package.ToString() + ".umb"));

            return "packages\\" + Package.ToString() + ".umb";
        }

        public static void updatePackageInfo(Guid Package, int VersionMajor, int VersionMinor, int VersionPatch, User User) {

        }
    }

    public class Package
    {
        public Package()
        {
        }

        /// <summary>
        /// Initialize package install status object by specifying the internal id of the installation. 
        /// The id is specific to the local Umbraco installation and cannot be used to identify the package in general. 
        /// Use the Package(Guid) constructor to check whether a package has been installed
        /// </summary>
        /// <param Name="Id">The internal id.</param>
        public Package(int Id)
        {
            initialize(Id);
        }

        public Package(Guid Id)
        {
            int installStatusId = int.Parse(
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(
                GlobalSettings.DbDSN,
                CommandType.Text,
                "select id from umbracoInstalledPackages where package = @package and upgradeId = 0",
                new SqlParameter("@package", Id)).ToString());

            if (installStatusId > 0)
                initialize(installStatusId);
            else
                throw new ArgumentException("Package with id '" + Id.ToString() + "' is not installed");
        }

        private void initialize(int id)
        {

            SqlDataReader dr =
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(
                GlobalSettings.DbDSN,
                CommandType.Text,
                "select id, uninstalled, upgradeId, installDate, userId, package, versionMajor, versionMinor, versionPatch from umbracoInstalledPackages where id = @id",
                new SqlParameter("@id", id));

            if (dr.Read())
            {
                Id = id;
                Uninstalled = dr.GetBoolean(dr.GetOrdinal("uninstalled"));
                UpgradeId = dr.GetInt32(dr.GetOrdinal("upgradeId"));
                InstallDate = dr.GetDateTime(dr.GetOrdinal("installDate"));
                User = User.GetUser(dr.GetInt32(dr.GetOrdinal("userId")));
                PackageId = dr.GetGuid(dr.GetOrdinal("package"));
                VersionMajor = dr.GetInt32(dr.GetOrdinal("versionMajor"));
                VersionMinor = dr.GetInt32(dr.GetOrdinal("versionMinor"));
                VersionPatch = dr.GetInt32(dr.GetOrdinal("versionPatch"));
            }
            dr.Close();
        }

        public void Save()
        {

            SqlParameter[] values = {
                new SqlParameter("@uninstalled", Uninstalled),
                new SqlParameter("@upgradeId", UpgradeId),
                new SqlParameter("@installDate", InstallDate),
                new SqlParameter("@userId", User.Id),
                new SqlParameter("@versionMajor", VersionMajor),
                new SqlParameter("@versionMinor", VersionMinor),
                new SqlParameter("@versionPatch", VersionPatch),
                new SqlParameter("@id", Id)
            };

            // check if package status exists
            if (Id == 0)
            {
                Id = int.Parse(
                    Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(
                    GlobalSettings.DbDSN,
                    CommandType.Text,
                    "SET NOCOUNT ON insert into umbracoInstalledPackages (uninstalled, upgradeId, installDate, userId, versionMajor, versionMinor, versionPatch) values (@uninstalled, @upgradeId, @installDate, @userId, @versionMajor, @versionMinor, @versionPatch) select @@IDENTITY SET NOCOUNT OFF",
                    values).ToString());
            }

            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(
                GlobalSettings.DbDSN,
                CommandType.Text,
                "update umbracoInstalledPackages set " +
                "uninstalled = @uninstalled, " +
                "upgradeId = @upgradeId, " +
                "installDate = @installDate, " +
                "userId = @userId, " +
                "versionMajor = @versionMajor, " +
                "versionMinor = @versionMinor, " +
                "versionPatch = @versionPatch " +
                "where id = @id",
                values);
        }

        private bool _uninstalled;

        public bool Uninstalled
        {
            get { return _uninstalled; }
            set { _uninstalled = value; }
        }
	

        private User _user;

        public User User
        {
            get { return _user; }
            set { _user = value; }
        }
	

        private DateTime _installDate;

        public DateTime InstallDate
        {
            get { return _installDate; }
            set { _installDate = value; }
        }
	

        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
	

        private int _upgradeId;

        public int UpgradeId
        {
            get { return _upgradeId; }
            set { _upgradeId = value; }
        }
	

        private Guid _packageId;

        public Guid PackageId
        {
            get { return _packageId; }
            set { _packageId = value; }
        }
	

        private int _versionPatch;

        public int VersionPatch
        {
            get { return _versionPatch; }
            set { _versionPatch = value; }
        }
	

        private int _versionMinor;

        public int VersionMinor
        {
            get { return _versionMinor; }
            set { _versionMinor = value; }
        }
	

        private int _versionMajor;

        public int VersionMajor
        {
            get { return _versionMajor; }
            set { _versionMajor = value; }
        }


	
    }
}
