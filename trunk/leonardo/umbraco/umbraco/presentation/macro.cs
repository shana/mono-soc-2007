using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using businesslogic;
using Microsoft.ApplicationBlocks.Data;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.macro;
using umbraco.cms.businesslogic.member;
using umbraco.interfaces;
using umbraco.presentation.xslt.Exslt;
using umbraco.scripting;
using Content=umbraco.cms.businesslogic.Content;

namespace umbraco
{
    /// <summary>
    /// Summary description for macro.
    /// </summary>
    public class macro : Page
    {
        #region private properties

        private readonly StringBuilder mContent = new StringBuilder();
        private readonly Cache macroCache = HttpRuntime.Cache;

        private readonly String macroCacheIdentifier = "umbMacro";
        private readonly string macrosAddedKey = "macrosAdded";
        private readonly string loadUserControlKey = "loadUserControl";

        // Alias hashable
        private static Hashtable _macroAlias = new Hashtable();

        // Macro-elements
        private int macroID;
        private bool cacheByPersonalization;
        private bool cacheByPage;
        private int cacheRefreshRate;
        private String alias;
        private String name;
        private String xsltFile;
        private String pythonFile;
        private String scriptType;
        private String scriptAssembly;
        private Hashtable properties = new Hashtable();
        private readonly Hashtable propertyDefinitions = new Hashtable();
        private readonly int macroType;
        private bool dontRenderInEditor;

        #endregion

        #region public properties

        public int MacroID
        {
            set { macroID = value; }
            get { return macroID; }
        }

        public bool CacheByPersonalization
        {
            set { cacheByPersonalization = value; }
            get { return cacheByPersonalization; }
        }

        public bool CacheByPage
        {
            set { cacheByPage = value; }
            get { return cacheByPage; }
        }

        public bool DontRenderInEditor
        {
            get { return dontRenderInEditor; }
            set { dontRenderInEditor = value; }
        }

        public int RefreshRate
        {
            set { cacheRefreshRate = value; }
            get { return cacheRefreshRate; }
        }

        public String Alias
        {
            set { alias = value; }
            get { return alias; }
        }

        public String Name
        {
            set { name = value; }
            get { return name; }
        }

        public String XsltFile
        {
            set { xsltFile = value; }
            get { return xsltFile; }
        }

        public String PythonFile
        {
            set { pythonFile = value; }
            get { return pythonFile; }
        }

        public String ScriptType
        {
            set { scriptType = value; }
            get { return scriptType; }
        }

        public String ScriptAssembly
        {
            set { scriptAssembly = value; }
            get { return scriptAssembly; }
        }

        public Hashtable Properties
        {
            get { return properties; }
            set { properties = value; }
        }

        public int MacroType
        {
            get { return macroType; }
        }

        public String MacroContent
        {
            set { mContent.Append(value); }
            get { return mContent.ToString(); }
        }

        public enum eMacroType
        {
            XSLT = 1,
            CustomControl = 2,
            UserControl = 3,
            Unknown = 4,
            Python = 5
        }

        #endregion

        /// <summary>
        /// Creates an empty macro object.
        /// </summary>
        public macro()
        {
        }

        public override string ToString()
        {
            return Name;
        }

        public static macro ReturnFromAlias(string alias)
        {
            if (_macroAlias.ContainsKey(alias))
                return new macro((int) _macroAlias[alias]);
            else
            {
                try
                {
                    int macroID = Macro.GetByAlias(alias).Id;
                    _macroAlias.Add(alias, macroID);
                    return new macro(macroID);
                }
                catch
                {
                    HttpContext.Current.Trace.Warn("macro", "No macro with alias '" + alias + "' found");
                    return null;
                }
            }
        }

        /// <summary>
        /// Creates a macro object
        /// </summary>
        /// <param name="id">Specify the macro-id which should be loaded (from table macro)</param>
        public macro(int id)
        {
            macroID = id;

            if (macroCache[macroCacheIdentifier + id] != null)
            {
                macro tempMacro = (macro) macroCache[macroCacheIdentifier + id];
                Name = tempMacro.Name;
                Alias = tempMacro.Alias;
                ScriptType = tempMacro.ScriptType;
                ScriptAssembly = tempMacro.ScriptAssembly;
                XsltFile = tempMacro.XsltFile;
                PythonFile = tempMacro.PythonFile;
                Properties = tempMacro.Properties;
                propertyDefinitions = tempMacro.propertyDefinitions;
                RefreshRate = tempMacro.RefreshRate;
                CacheByPage = tempMacro.CacheByPage;
                CacheByPersonalization = tempMacro.CacheByPersonalization;
                DontRenderInEditor = tempMacro.DontRenderInEditor;

                HttpContext.Current.Trace.Write("umbracoMacro",
                                                string.Format("Macro loaded from cache (ID: {0}, {1})", id, Name));
            }
            else
            {
                using (SqlDataReader macroDef = SqlHelper.ExecuteReader(GlobalSettings.DbDSN,
                                                                        CommandType.Text,
                                                                        "select * from cmsMacro left join cmsMacroProperty property on property.macro = cmsMacro.id left join cmsMacroPropertyType propertyType on propertyType.id = property.macroPropertyType where cmsMacro.id = @macroID order by property.macroPropertySortOrder",
                                                                        new SqlParameter("@macroID", id)))
                {
                    if (!macroDef.HasRows)
                        HttpContext.Current.Trace.Warn("Macro", "No definition found for id " + id);

                    while (macroDef.Read())
                    {
                        string tmpStr;
                        bool tmpBool;
                        int tmpInt;

                        if (TryGetColumnBool(macroDef, "macroCacheByPage", out tmpBool))
                            CacheByPage = tmpBool;
                        if (TryGetColumnBool(macroDef, "macroCachePersonalized", out tmpBool))
                            CacheByPersonalization = tmpBool;
                        if (TryGetColumnBool(macroDef, "macroDontRender", out tmpBool))
                            DontRenderInEditor = tmpBool;
                        if (TryGetColumnInt32(macroDef, "macroRefreshRate", out tmpInt))
                            RefreshRate = tmpInt;
                        if (TryGetColumnInt32(macroDef, "macroRefreshRate", out tmpInt))
                            RefreshRate = tmpInt;
                        if (TryGetColumnString(macroDef, "macroName", out tmpStr))
                            Name = tmpStr;
                        if (TryGetColumnString(macroDef, "macroAlias", out tmpStr))
                            Alias = tmpStr;
                        if (TryGetColumnString(macroDef, "macroScriptType", out tmpStr))
                            ScriptType = tmpStr;
                        if (TryGetColumnString(macroDef, "macroScriptAssembly", out tmpStr))
                            ScriptAssembly = tmpStr;
                        if (TryGetColumnString(macroDef, "macroXSLT", out tmpStr))
                            XsltFile = tmpStr;
                        if (TryGetColumnString(macroDef, "macroPython", out tmpStr))
                            PythonFile = tmpStr;
                        if (TryGetColumnString(macroDef, "macroPropertyAlias", out tmpStr))
                        {
                            string typeAlias;
                            if (TryGetColumnString(macroDef, "macroPropertyTypeAlias", out typeAlias))
                                properties.Add(tmpStr, typeAlias);

                            string baseType;
                            if (TryGetColumnString(macroDef, "macroPropertyTypeBaseType", out baseType))
                                propertyDefinitions.Add(tmpStr, baseType);
                        }
                    }
                }
                // add current macro-object to cache
                macroCache.Insert(macroCacheIdentifier + id, this);
            }

            if (!string.IsNullOrEmpty(XsltFile))
                macroType = (int) eMacroType.XSLT;
            else
            {
                if (!string.IsNullOrEmpty(PythonFile))
                    macroType = (int) eMacroType.Python;
                else
                {
                    if (!string.IsNullOrEmpty(ScriptType) && ScriptType.ToLower().IndexOf(".ascx") > -1)
                    {
                        macroType = (int) eMacroType.UserControl;
                    }
                    else if (!string.IsNullOrEmpty(ScriptType) && !string.IsNullOrEmpty(ScriptAssembly))
                        macroType = (int) eMacroType.CustomControl;
                }
            }
            if (macroType.ToString() == string.Empty)
                macroType = (int) eMacroType.Unknown;
        }

        private bool TryGetColumnString(SqlDataReader reader, string columnName, out string value)
        {
            value = string.Empty;
            // First check if column actually exists
            foreach (DataRow row in reader.GetSchemaTable().Rows)
            {
                if ((string) row["ColumnName"] == columnName)
                {
                    int colIndex = reader.GetOrdinal(columnName);
                    if (!reader.IsDBNull(colIndex))
                    {
                        value = reader.GetString(colIndex);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool TryGetColumnInt32(SqlDataReader reader, string columnName, out int value)
        {
            value = -1;
            // First check if column actually exists
            foreach (DataRow row in reader.GetSchemaTable().Rows)
            {
                if ((string) row["ColumnName"] == columnName)
                {
                    int colIndex = reader.GetOrdinal(columnName);
                    if (!reader.IsDBNull(colIndex))
                    {
                        value = reader.GetInt32(colIndex);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool TryGetColumnBool(SqlDataReader reader, string columnName, out bool value)
        {
            value = false;
            // First check if column actually exists
            foreach (DataRow row in reader.GetSchemaTable().Rows)
            {
                if ((string) row["ColumnName"] == columnName)
                {
                    int colIndex = reader.GetOrdinal(columnName);
                    if (!reader.IsDBNull(colIndex))
                    {
                        value = reader.GetBoolean(colIndex);
                        return true;
                    }
                }
            }
            return false;
        }

        public static void ClearAliasCache()
        {
            _macroAlias = new Hashtable();
        }

        /// <summary>
        /// Deletes macro definition from cache.
        /// </summary>
        /// <returns>True if succesfull, false if nothing has been removed</returns>
        public bool removeFromCache()
        {
            ClearAliasCache();
            if (macroID > 0)
            {
                if (macroCache[macroCacheIdentifier + macroID] != null)
                {
                    macroCache.Remove(macroCacheIdentifier + macroID);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        private string getCacheGuid(Hashtable attributes, page umbPage)
        {
            string tempGuid = string.Empty;
            if (CacheByPage)
                tempGuid = umbPage.PageID + "-";
            if (CacheByPersonalization)
            {
                if (Member.GetCurrentMember() != null)
                    tempGuid += "m" + Member.GetCurrentMember().Id + "-";
                else
                    tempGuid += "m";
            }

            IDictionaryEnumerator id = attributes.GetEnumerator();
            while (id.MoveNext())
            {
                string attValue = helper.FindAttribute(umbPage, attributes, id.Key.ToString());
                if (attValue.Length > 255)
                    tempGuid += attValue.Remove(255, attValue.Length - 255) + "-";
                else
                    tempGuid += attValue + "-";
            }
            return tempGuid;
        }

        public Control renderMacro(Hashtable attributes, page umbPage)
        {
            HttpContext.Current.Trace.Write("renderMacro",
                                            string.Format("Rendering started (macro: {0}, type: {1}, cacheRate: {2})",
                                                          Name, MacroType, RefreshRate));

            StateHelper.SetContextValue(macrosAddedKey, StateHelper.GetContextValue<int>(macrosAddedKey) + 1);

            Control macroHtml = null;

            string macroGuid = getCacheGuid(attributes, umbPage);

            if (RefreshRate > 0)
            {
                if (macroCache["macroHtml_" + macroGuid] != null)
                {
                    MacroCacheContent cacheContent = (MacroCacheContent) macroCache["macroHtml_" + macroGuid];
                    macroHtml = cacheContent.Content;
                    macroHtml.ID = cacheContent.ID;
                    HttpContext.Current.Trace.Write("renderMacro", "Content loaded from cache ('" + macroGuid + "')...");
                }
            }

            if (macroHtml == null)
            {
                switch (MacroType)
                {
                    case (int) eMacroType.UserControl:
                        try
                        {
                            HttpContext.Current.Trace.Write("umbracoMacro", "Usercontrol added (" + scriptType + ")");
                            macroHtml = loadUserControl(ScriptType, attributes, umbPage);
                            break;
                        }
                        catch (Exception e)
                        {
                            HttpContext.Current.Trace.Warn("umbracoMacro",
                                                           "Error loading userControl (" + scriptType + ")", e);
                            macroHtml = new LiteralControl("Error loading userControl '" + scriptType + "'");
                            break;
                        }
                    case (int) eMacroType.CustomControl:
                        try
                        {
                            HttpContext.Current.Trace.Write("umbracoMacro", "Custom control added (" + scriptType + ")");
                            HttpContext.Current.Trace.Write("umbracoMacro", "ScriptAssembly (" + scriptAssembly + ")");
                            macroHtml = loadControl(scriptAssembly, ScriptType, attributes, umbPage);
                            break;
                        }
                        catch (Exception e)
                        {
                            HttpContext.Current.Trace.Warn("umbracoMacro",
                                                           "Error loading customControl (Assembly: " + scriptAssembly +
                                                           ", Type: '" + scriptType + "'", e);
                            macroHtml =
                                new LiteralControl("Error loading customControl (Assembly: " + scriptAssembly +
                                                   ", Type: '" +
                                                   scriptType + "'");
                            break;
                        }
                    case (int) eMacroType.XSLT:
                        macroHtml = loadMacroXSLT(this, attributes, umbPage);
                        break;
                    case (int) eMacroType.Python:
                        try
                        {
                            HttpContext.Current.Trace.Write("umbracoMacro", "Python script added (" + scriptType + ")");
                            macroHtml = loadMacroPython(this, attributes, umbPage);
                            break;
                        }
                        catch (Exception e)
                        {
                            HttpContext.Current.Trace.Warn("umbracoMacro",
                                                           "Error loading python script (file: " + PythonFile +
                                                           ", Type: '" + scriptType + "'", e);
                            macroHtml =
                                new LiteralControl("Error loading python script (file: " + PythonFile + ", Type: '" +
                                                   scriptType + "'");
                            break;
                        }
                    default:
                        if (GlobalSettings.DebugMode)
                            macroHtml =
                                new LiteralControl("&lt;Macro: " + Name + " (" + ScriptAssembly + "," + ScriptType +
                                                   ")&gt;");
                        break;
                }

                // Add result to cache
                if (RefreshRate > 0)
                {
                    // do not add to cache if there's no member and it should cache by personalization
                    if (!CacheByPersonalization || (CacheByPersonalization && Member.GetCurrentMember() != null))
                        if (macroHtml != null)
                            macroCache.Insert("macroHtml_" + macroGuid, new MacroCacheContent(macroHtml, macroHtml.ID), null,
                                              DateTime.Now.AddSeconds(RefreshRate), TimeSpan.Zero, CacheItemPriority.Low,
                                              null);
                }
            }
            return macroHtml;
        }

        public static XslCompiledTransform getXslt(string XsltFile)
        {
            if (HttpRuntime.Cache["macroXslt_" + XsltFile] != null)
            {
                return (XslCompiledTransform) HttpRuntime.Cache["macroXslt_" + XsltFile];
            }
            else
            {
                XslCompiledTransform macroXSLT = new XslCompiledTransform();
                XmlTextReader xslReader =
                    new XmlTextReader(HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../xslt/" + XsltFile));
                xslReader.EntityHandling = EntityHandling.ExpandEntities;

                try
                {
                    XmlUrlResolver xslResolver = new XmlUrlResolver();
                    xslResolver.Credentials = CredentialCache.DefaultCredentials;
                    macroXSLT.Load(xslReader, XsltSettings.TrustedXslt, xslResolver);
                    HttpRuntime.Cache.Insert(
                        "macroXslt_" + XsltFile,
                        macroXSLT,
                        new CacheDependency(
                            HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../xslt/" + XsltFile)));
                }
                finally
                {
                    xslReader.Close();
                }
                return macroXSLT;
            }
        }

        public static void unloadXslt(string XsltFile)
        {
            if (HttpRuntime.Cache["macroXslt_" + XsltFile] != null)
                HttpRuntime.Cache.Remove("macroXslt_" + XsltFile);
        }

        public Control loadMacroXSLT(macro macro, Hashtable attributes, page umbPage)
        {
            if (XsltFile.Trim() != string.Empty)
            {
                // The Xml Control - in umbraco 3.0 replaced with a literalcontrol
                Literal macroControl = new Literal();

                // Get main XML
                XmlDocument umbracoXML = content.Instance.XmlContent;

                // Create XML document for Macro
                XmlDocument macroXML = new XmlDocument();
                macroXML.LoadXml("<macro/>");

                foreach (DictionaryEntry macroDef in macro.properties)
                {
                    try
                    {
                        if (helper.FindAttribute(umbPage, attributes, macroDef.Key.ToString()) != string.Empty)
                            addMacroXmlNode(umbracoXML, macroXML, macroDef.Key.ToString(), macroDef.Value.ToString(),
                                            helper.FindAttribute(umbPage, attributes, macroDef.Key.ToString()));
                        else
                            addMacroXmlNode(umbracoXML, macroXML, macroDef.Key.ToString(), macroDef.Value.ToString(),
                                            string.Empty);
                    }
                    catch (Exception e)
                    {
                        HttpContext.Current.Trace.Warn("umbracoMacro", "Could not write XML node (" + macroDef.Key + ")",
                                                       e);
                    }
                }

                if (HttpContext.Current.Request.QueryString["umbDebug"] != null)
                {
                    return
                        new LiteralControl("<div style=\"border: 2px solid green; padding: 5px;\"><b>Debug from " +
                                           macro.Name +
                                           "</b><br/><p>" + Page.Server.HtmlEncode(macroXML.OuterXml) + "</p></div>");
                }
                else
                {
                    try
                    {
                        //					macroControl.TransformSource = GlobalSettings.Path + "/../xslt/"+this.XsltFile;
                        XslCompiledTransform result = getXslt(XsltFile);

                        try
                        {
                            // Create a textwriter
                            TextWriter tw = new StringWriter();

                            HttpContext.Current.Trace.Write("umbracoMacro", "Before adding extensions");
                            XsltArgumentList xslArgs;
                            xslArgs = AddXsltExtensions();
                            library lib = new library(umbPage);
                            xslArgs.AddExtensionObject("urn:umbraco.library", lib);
                            HttpContext.Current.Trace.Write("umbracoMacro", "After adding extensions");

                            // Add the current node
                            xslArgs.AddParam("currentPage", string.Empty, library.GetXmlNodeCurrent());
                            HttpContext.Current.Trace.Write("umbracoMacro", "Before performing transformation");

                            // Do transformation
                            result.Transform(macroXML.CreateNavigator(), xslArgs, tw);

                            macroControl.Text = tw.ToString();

                            HttpContext.Current.Trace.Write("umbracoMacro", "After performing transformation");
                        }
                        catch (Exception e)
                        {
                            HttpContext.Current.Trace.Warn("umbracoMacro", "Error parsing XSLT " + xsltFile, e);
                            return new LiteralControl("Error parsing XSLT file: \\xslt\\" + XsltFile);
                        }
                    }
                    catch (Exception e)
                    {
                        HttpContext.Current.Trace.Warn("umbracoMacro", "Error loading XSLT " + xsltFile, e);
                        return new LiteralControl("Error reading XSLT file: \\xslt\\" + XsltFile);
                    }

                    //return new LiteralControl(Page.Server.HtmlEncode(macroXML.OuterXml));
                    return macroControl;
                }
            }
            else
            {
                Page.Trace.Warn("macro", "Xslt is empty");
                return new LiteralControl(string.Empty);
            }
        }

        public static XsltArgumentList AddXsltExtensions()
        {
            return AddMacroXsltExtensions();
        }

        public static XsltArgumentList AddMacroXsltExtensions()
        {
            // Init custom extensions
            XsltArgumentList _xslArgs = new XsltArgumentList();
            XmlDocument xsltExt = new XmlDocument();
            xsltExt.Load(HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../config/xsltExtensions.config"));

            // Add eXSLTs
            _xslArgs.AddExtensionObject("urn:Exslt.ExsltCommon", new ExsltCommon());
            _xslArgs.AddExtensionObject("urn:Exslt.ExsltDatesAndTimes", new ExsltDatesAndTimes());
            _xslArgs.AddExtensionObject("urn:Exslt.ExsltMath", new ExsltMath());
            _xslArgs.AddExtensionObject("urn:Exslt.ExsltRegularExpressions", new ExsltRegularExpressions());
            _xslArgs.AddExtensionObject("urn:Exslt.ExsltStrings", new ExsltStrings());
            _xslArgs.AddExtensionObject("urn:Exslt.ExsltSets", new ExsltSets());

            if (xsltExt.SelectSingleNode("/XsltExtensions").HasChildNodes)
                foreach (XmlNode xsltEx in xsltExt.SelectSingleNode("/XsltExtensions"))
                {
                    if (xsltEx.NodeType != XmlNodeType.Comment)
                    {
                        string currentAss =
                            HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/.." +
                                                               xsltEx.Attributes["assembly"].Value + ".dll");
                        Assembly asm = Assembly.LoadFrom(currentAss);
                        Type type = asm.GetType(xsltEx.Attributes["type"].Value);

                        _xslArgs.AddExtensionObject("urn:" + xsltEx.Attributes["alias"].Value,
                                                    Activator.CreateInstance(type));
                        HttpContext.Current.Trace.Write("umbracoXsltExtension",
                                                        "Extension added: " + "urn:" + xsltEx.Attributes["alias"].Value +
                                                        ", " + type.Name);
                    }
                }
            return _xslArgs;
        }

        private void addMacroXmlNode(XmlDocument umbracoXML, XmlDocument macroXML, String macroPropertyAlias,
                                     String macroPropertyType, String macroPropertyValue)
        {
            XmlNode macroXmlNode = macroXML.CreateNode(XmlNodeType.Element, macroPropertyAlias, string.Empty);
            XmlDocument x = new XmlDocument();

            int currentID = -1;
            // If no value is passed, then use the current pageID as value
            if (macroPropertyValue == string.Empty)
            {
                page umbPage = (page) HttpContext.Current.Items["umbPageObject"];
                currentID = umbPage.PageID;
            }

            HttpContext.Current.Trace.Write("umbracoMacro",
                                            "Xslt node adding search start (" + macroPropertyAlias + ",'" +
                                            macroPropertyValue + "')");
            switch (macroPropertyType)
            {
                case "contentTree":
                    XmlAttribute nodeID = macroXML.CreateAttribute("nodeID");
                    if (macroPropertyValue != string.Empty)
                        nodeID.Value = macroPropertyValue;
                    else
                        nodeID.Value = currentID.ToString();
                    macroXmlNode.Attributes.SetNamedItem(nodeID);

                    // Get subs
                    try
                    {
                        macroXmlNode.AppendChild(macroXML.ImportNode(umbracoXML.GetElementById(nodeID.Value), true));
                    }
                    catch
                    {
                        break;
                    }
                    break;
                case "contentCurrent":
                    x.LoadXml("<nodes/>");
                    XmlNode currentNode;
                    if (macroPropertyValue != string.Empty)
                        currentNode = macroXML.ImportNode(umbracoXML.GetElementById(macroPropertyValue), true);
                    else
                        currentNode = macroXML.ImportNode(umbracoXML.GetElementById(currentID.ToString()), true);

                    // remove all sub content nodes
                    foreach (XmlNode n in currentNode.SelectNodes("./node"))
                        currentNode.RemoveChild(n);

                    macroXmlNode.AppendChild(currentNode);

                    break;
                case "contentSubs":
                    x.LoadXml("<nodes/>");
                    if (macroPropertyValue != string.Empty)
                        x.FirstChild.AppendChild(x.ImportNode(umbracoXML.GetElementById(macroPropertyValue), true));
                    else
                        x.FirstChild.AppendChild(x.ImportNode(umbracoXML.GetElementById(currentID.ToString()), true));
                    macroXmlNode.InnerXml = transformMacroXML(x, "macroGetSubs.xsl");
                    break;
                case "contentAll":
                    x.ImportNode(umbracoXML.DocumentElement.LastChild, true);
                    break;
                case "contentRandom":
                    XmlNode source = umbracoXML.GetElementById(macroPropertyValue);
                    if (source != null)
                    {
                        XmlNodeList sourceList = source.SelectNodes("node");
                        if (sourceList.Count > 0)
                        {
                            int rndNumber;
                            Random r = library.GetRandom();
                            lock (r)
                            {
                                rndNumber = r.Next(sourceList.Count);
                            }
                            XmlNode node = macroXML.ImportNode(sourceList[rndNumber], true);
                            // remove all sub content nodes
                            foreach (XmlNode n in node.SelectNodes("./node"))
                                node.RemoveChild(n);

                            macroXmlNode.AppendChild(node);
                            break;
                        }
                        else
                            HttpContext.Current.Trace.Warn("umbracoMacro",
                                                           "Error adding random node - parent (" + macroPropertyValue +
                                                           ") doesn't have children!");
                    }
                    else
                        HttpContext.Current.Trace.Warn("umbracoMacro",
                                                       "Error adding random node - parent (" + macroPropertyValue +
                                                       ") doesn't exists!");
                    break;
                case "mediaCurrent":
                    Content c = new Content(int.Parse(macroPropertyValue));
                    macroXmlNode.AppendChild(macroXML.ImportNode(c.ToXml(content.Instance.XmlContent, false), true));
                    break;
                default:
                    macroXmlNode.InnerText = System.Web.HttpContext.Current.Server.HtmlDecode(macroPropertyValue);
                    break;
            }
            macroXML.FirstChild.AppendChild(macroXmlNode);
        }

        private string transformMacroXML(XmlDocument xmlSource, string xslt_File)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            XslCompiledTransform result = getXslt(xslt_File);
            //XmlDocument xslDoc = new XmlDocument();

            result.Transform(xmlSource.CreateNavigator(), null, sw);

            if (sw.ToString() != string.Empty)
                return sw.ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Executes a python script. 
        /// </summary>
        /// <param name="macro">The instance of the macro (this). No idea why passed.</param>
        /// <param name="attributes">Relayed attributes to determine the values of the passed properties.</param>
        /// <param name="umbPage">The current page.</param>
        /// <returns>Returns a LiteralControl stuffed with the StandardOutput of the script execution.</returns>
        public Control loadMacroPython(macro macro, Hashtable attributes, page umbPage)
        {
            LiteralControl ret = new LiteralControl();
            try
            {
                // Adding some global accessible variables to the enviroment.
                // Currently no cleanup after execution is done.
                Hashtable args = new Hashtable();
                HttpContext.Current.Session.Add("page", umbPage);
                HttpContext.Current.Session.Add("macro", this);
                HttpContext.Current.Session.Add("args", args);

                foreach (DictionaryEntry macroDef in macro.properties)
                {
                    try
                    {
                        args.Add(macroDef.Key.ToString(),
                                 helper.FindAttribute(umbPage, attributes, macroDef.Key.ToString()));
                    }
                    catch (Exception e)
                    {
                        HttpContext.Current.Trace.Warn("umbracoMacro",
                                                       "Could not add global variable (" + macroDef.Key +
                                                       ") to python enviroment", e);
                    }
                }

                if (string.IsNullOrEmpty(macro.PythonFile))
                {
                    ret.Text = string.Empty;
                }
                else
                {
                    // Execute the script and set the text of our LiteralControl with the returned
                    // result of our script.
                    string path = HttpContext.Current.Server.MapPath(
                        string.Format("{0}\\..\\python\\{1}",
                                      GlobalSettings.Path, macro.PythonFile));
                    object res = python.executeFile(path);
                    ret.Text = res.ToString();
                }
            }
            catch (Exception ex)
            {
                // Let's collect as much info we can get and display it glaring red
                ret.Text = "<div style=\"border: 1px solid red; padding: 5px;\">";
                Exception ie = ex;
                while (ie != null)
                {
                    ret.Text += "<br/><b>" + ie.Message + "</b><br/>";
                    ret.Text += ie.StackTrace + "<br/>";

                    ie = ie.InnerException;
                }
                ret.Text += "</div>";
            }
            return ret;
        }

        /// <summary>
        /// Loads a custom or webcontrol using reflection into the macro object
        /// </summary>
        /// <param name="fileName">The assembly to load from</param>
        /// <param name="controlName">Name of the control</param>
        /// <returns></returns>
        /// <param name="attributes"></param>
        public Control loadControl(string fileName, string controlName, Hashtable attributes)
        {
            return loadControl(fileName, controlName, attributes, null);
        }

        /// <summary>
        /// Loads a custom or webcontrol using reflection into the macro object
        /// </summary>
        /// <param name="fileName">The assembly to load from</param>
        /// <param name="controlName">Name of the control</param>
        /// <returns></returns>
        /// <param name="attributes"></param>
        /// <param name="umbPage"></param>
        public Control loadControl(string fileName, string controlName, Hashtable attributes, page umbPage)
        {
            Type type;
            Assembly asm;
            try
            {
                string currentAss = Server.MapPath(string.Format("{0}/../bin/{1}.dll",
                                                                 GlobalSettings.Path, fileName));
                if (!File.Exists(currentAss))
                    return new LiteralControl("Unable to load user control because is does not exist: " + fileName);
                asm = Assembly.LoadFrom(currentAss);
                if (HttpContext.Current != null)
                    HttpContext.Current.Trace.Write("umbracoMacro", "Assembly file " + currentAss + " LOADED!!");
            }
            catch
            {
                throw new ArgumentException(string.Format("ASSEMBLY NOT LOADED PATH: {0} NOT FOUND!!",
                                                          Server.MapPath(GlobalSettings.Path + "/../bin/" + fileName +
                                                                         ".dll")));
            }

            if (HttpContext.Current != null)
                HttpContext.Current.Trace.Write("umbracoMacro",
                                                string.Format("Assembly Loaded from ({0}.dll)", fileName));
            type = asm.GetType(controlName);
            if (type == null)
                return new LiteralControl(string.Format("Unable to get type {0} from assembly {1}",
                                                        controlName, asm.FullName));

            Control control = Activator.CreateInstance(type) as Control;
            if (control == null)
                return new LiteralControl(string.Format("Unable to create control {0} from assembly {1}",
                                                        controlName, asm.FullName));

            /// Properties
            foreach (string propertyAlias in properties.Keys)
            {
                PropertyInfo prop = type.GetProperty(propertyAlias);
                if (prop == null)
                {
                    if (HttpContext.Current != null)
                        HttpContext.Current.Trace.Warn("macro",
                                                       string.Format("control property '{0} ({1})' didn't work",
                                                                     propertyAlias,
                                                                     helper.FindAttribute(attributes, propertyAlias)));

                    continue;
                }

                object propValue = helper.FindAttribute(umbPage, attributes, propertyAlias);
                // Special case for types of webControls.unit
                if (prop.PropertyType == typeof (Unit))
                    propValue = Unit.Parse(propValue.ToString());
                else
                {
                    foreach (object s in propertyDefinitions)
                    {
                        if (s != null)
                            Trace.Write("macroProp", s.ToString());
                    }

                    Trace.Warn("macro", propertyAlias);

                    object o = propertyDefinitions[propertyAlias];
                    if (o == null)
                        continue;
                    TypeCode st = (TypeCode) Enum.Parse(typeof (TypeCode), o.ToString(), true);

                    // Special case for booleans
                    if (prop.PropertyType == typeof (bool))
                    {
                        bool parseResult;
                        if (
                            Boolean.TryParse(propValue.ToString().Replace("1", "true").Replace("0", "false"),
                                             out parseResult))
                            propValue = parseResult;
                        else
                            propValue = false;
                    }
                    else
                        propValue = Convert.ChangeType(propValue, st);
                }

                prop.SetValue(control, Convert.ChangeType(propValue, prop.PropertyType), null);

                if (HttpContext.Current != null)
                    HttpContext.Current.Trace.Write("macro",
                                                    string.Format("control property '{0} ({1})' worked",
                                                                  propertyAlias,
                                                                  helper.FindAttribute(umbPage, attributes,
                                                                                       propertyAlias)));
            }
            return control;
        }

        /// <summary>
        /// Loads an usercontrol using reflection into the macro object
        /// </summary>
        /// <param name="fileName">Filename of the usercontrol - ie. ~wulff.ascx</param>
        /// <returns></returns>
        /// <param name="attributes"></param>
        /// <param name="umbPage"></param>
        public Control loadUserControl(string fileName, Hashtable attributes, page umbPage)
        {
            Debug.Assert(!string.IsNullOrEmpty(fileName), "fileName cannot be empty");
            Debug.Assert(attributes != null, "attributes cannot be null");
            Debug.Assert(umbPage != null, "umbPage cannot be null");
            try
            {
                string userControlPath = @"~/" + fileName;

                if (!File.Exists(HttpContext.Current.Server.MapPath(userControlPath)))
                    return new LiteralControl(string.Format("UserControl {0} does not exist.", fileName));

                UserControl oControl = (UserControl) new UserControl().LoadControl(userControlPath);

                int slashIndex = fileName.LastIndexOf("/") + 1;
                if (slashIndex < 0)
                    slashIndex = 0;

                if (attributes["controlID"] != null)
                    oControl.ID = attributes["controlID"].ToString();
                else
                    oControl.ID =
                        string.Format("{0}_{1}", fileName.Substring(slashIndex, fileName.IndexOf(".ascx") - slashIndex),
                                      StateHelper.GetContextValue<int>(macrosAddedKey));

                TraceInfo(loadUserControlKey, string.Format("Usercontrol added with id '{0}'", oControl.ID));

                Type type = oControl.GetType();
                if (type == null)
                {
                    TraceWarn(loadUserControlKey, "Unable to retrieve control type: " + fileName);
                    return oControl;
                }

                foreach (string propertyAlias in properties.Keys)
                {
                    PropertyInfo prop = type.GetProperty(propertyAlias);
                    if (prop == null)
                    {
                        TraceWarn(loadUserControlKey, "Unable to retrieve type from propertyAlias: " + propertyAlias);
                        continue;
                    }

                    object propValue =
                        helper.FindAttribute(umbPage, attributes, propertyAlias).Replace("&amp;", "&").Replace(
                            "&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">");
                    if (string.IsNullOrEmpty(propValue as string))
                        continue;

                    // Special case for types of webControls.unit
                    try
                    {
                        if (prop.PropertyType == typeof (Unit))
                            propValue = Unit.Parse(propValue.ToString());
                        else
                        {
                            try
                            {
                                object o = propertyDefinitions[propertyAlias];
                                if (o == null)
                                    continue;
                                TypeCode st = (TypeCode) Enum.Parse(typeof (TypeCode), o.ToString(), true);

                                // Special case for booleans
                                if (prop.PropertyType == typeof (bool))
                                {
                                    bool parseResult;
                                    if (
                                        Boolean.TryParse(
                                            propValue.ToString().Replace("1", "true").Replace("0", "false"),
                                            out parseResult))
                                        propValue = parseResult;
                                    else
                                        propValue = false;
                                }
                                else
                                    propValue = Convert.ChangeType(propValue, st);

                                TraceWarn("macro.loadControlProperties",
                                          string.Format("Property added '{0}' with value '{1}'", propertyAlias,
                                                        propValue));
                            }
                            catch (Exception PropException)
                            {
                                HttpContext.Current.Trace.Warn("macro.loadControlProperties",
                                                               string.Format(
                                                                   "Error adding property '{0}' with value '{1}'",
                                                                   propertyAlias, propValue), PropException);
                            }
                        }

                        prop.SetValue(oControl, Convert.ChangeType(propValue, prop.PropertyType), null);
                    }
                    catch (Exception propException)
                    {
                        HttpContext.Current.Trace.Warn("macro.loadControlProperties",
                                                       string.Format(
                                                           "Error adding property '{0}' with value '{1}', maybe it doesn't exists or maybe casing is wrong!",
                                                           propertyAlias, propValue), propException);
                    }
                }
                return oControl;
            }
            catch (Exception e)
            {
                HttpContext.Current.Trace.Warn("macro", string.Format("Error creating usercontrol ({0})", fileName), e);
                return new LiteralControl(
                    string.Format(
                        "<div style=\"color: black; padding: 3px; border: 2px solid red\"><b style=\"color:red\">Error creating control ({0}).</b><br/> Maybe file doesn't exists or the usercontrol has a cache directive, which is not allowed! See the tracestack for more information!</div>",
                        fileName));
            }
        }

        private void TraceInfo(string category, string message)
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Trace.Write(category, message);
        }

        private void TraceWarn(string category, string message)
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Trace.Warn(category, message);
        }

        /// <summary>
        /// For debug purposes only - should be deleted or made private
        /// </summary>
        /// <param name="type">The type of object (control) to show properties from</param>
        public void macroProperties(Type type)
        {
            PropertyInfo[] myProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            HttpContext.Current.Response.Write("<p>" + type.Name + "<br />");
            foreach (PropertyInfo propertyItem in myProperties)
            {
                //				if (propertyItem.CanWrite) 
                HttpContext.Current.Response.Write(propertyItem.Name + " (" + propertyItem.PropertyType +
                                                   ")<br />");
            }
            HttpContext.Current.Response.Write("</p>");
        }

        public static string renderMacroStartTag(Hashtable attributes, int pageId, Guid versionId)
        {
            string div = "<div "; 

            IDictionaryEnumerator ide = attributes.GetEnumerator();
            while (ide.MoveNext())
            {
                div += string.Format("umb_{0}=\"{1}\" ", ide.Key, ide.Value.ToString().Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\"", "&quot;"));
            }

            div += "ismacro=\"true\" onresizestart=\"return false;\" umbVersionId=\"" + versionId +
                   "\" umbPageid=\"" +
                   pageId +
                   "\" title=\"This is rendered content from a macro\" contentEditable=\"false\" class=\"umbMacroHolder mceNonEditable\" ondblclick=\"umbracoEditMacro(this);\"><!-- startUmbMacro -->";

            return div;
        }

        public static string renderMacroEndTag()
        {
            return "<!-- endUmbMacro --></div>";
        }

        public static string GetRenderedMacro(int MacroId, page umbPage, Hashtable attributes)
        {
            macro m = new macro(MacroId);
            Control c = m.renderMacro(attributes, umbPage);
            TextWriter writer = new StringWriter();
            HtmlTextWriter ht = new HtmlTextWriter(writer);
            c.RenderControl(ht);
            string result = writer.ToString();

            // remove hrefs
            string pattern = "href=\"([^\"]*)\"";
            MatchCollection hrefs =
                Regex.Matches(result, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            foreach (Match href in hrefs)
                result = result.Replace(href.Value, "href=\"javascript:void(0)\"");

            return result;
        }

        public static string MacroContentByHttp(int PageID, Guid PageVersion, Hashtable attributes)
        {
            if (!ReturnFromAlias(attributes["macroAlias"].ToString()).DontRenderInEditor)
            {
                string querystring = "umbPageId=" + PageID + "&umbVersionId=" + PageVersion;
                IDictionaryEnumerator ide = attributes.GetEnumerator();
                while (ide.MoveNext())
                    querystring += "&umb_" + ide.Key + "=" + HttpContext.Current.Server.UrlEncode(ide.Value.ToString());

                // Create a new 'HttpWebRequest' Object to the mentioned URL.
                string retVal = string.Empty;
                string url = "http://" + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + ":" +
                             HttpContext.Current.Request.ServerVariables["SERVER_PORT"] + GlobalSettings.Path +
                             "/macroResultWrapper.aspx?" +
                             querystring;

                HttpWebRequest myHttpWebRequest = (HttpWebRequest) WebRequest.Create(url);
                // Assign the response object of 'HttpWebRequest' to a 'HttpWebResponse' variable.
                HttpWebResponse myHttpWebResponse = null;
                try
                {
                    myHttpWebResponse = (HttpWebResponse) myHttpWebRequest.GetResponse();
                    if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Stream streamResponse = myHttpWebResponse.GetResponseStream();
                        StreamReader streamRead = new StreamReader(streamResponse);
                        Char[] readBuff = new Char[256];
                        int count = streamRead.Read(readBuff, 0, 256);
                        while (count > 0)
                        {
                            String outputData = new String(readBuff, 0, count);
                            retVal += outputData;
                            count = streamRead.Read(readBuff, 0, 256);
                        }
                        // Close the Stream object.
                        streamResponse.Close();
                        streamRead.Close();

                        // Find the content of a form
                        string grabStart = "<!-- grab start -->";
                        string grabEnd = "<!-- grab end -->";
                        int grabStartPos = retVal.IndexOf(grabStart) + grabStart.Length;
                        int grabEndPos = retVal.IndexOf(grabEnd) - grabStartPos;
                        retVal = retVal.Substring(grabStartPos, grabEndPos);
                    }
                    else
                        retVal = "<span style=\"color: green\">No macro content available for WYSIWYG editing</span>";

                    // Release the HttpWebResponse Resource.
                    myHttpWebResponse.Close();
                }
                catch
                {
                    retVal = "<span style=\"color: green\">No macro content available for WYSIWYG editing</span>";
                }
                finally
                {
                    // Release the HttpWebResponse Resource.
                    if (myHttpWebResponse != null)
                        myHttpWebResponse.Close();
                }

                return retVal.Replace("\n", string.Empty).Replace("\r", string.Empty);
            }
            else
                return "<span style=\"color: green\">This macro does not provides rendering in WYSIWYG editor</span>";
        }
    }

    public class MacroCacheContent
    {
        private Control _control;
        private string _id;

        public string ID
        {
            get { return _id; }
        }

        public Control Content
        {
            get { return _control; }
        }

        public MacroCacheContent(Control control, string ID)
        {
            _control = control;
            _id = ID;
        }
    }

    public class macroCacheRefresh : ICacheRefresher
    {
        #region ICacheRefresher Members

        public string Name
        {
            get
            {
                // TODO:  Add templateCacheRefresh.Name getter implementation
                return "Macro cache refresher";
            }
        }

        public Guid UniqueIdentifier
        {
            get
            {
                // TODO:  Add templateCacheRefresh.UniqueIdentifier getter implementation
                return new Guid("7B1E683C-5F34-43dd-803D-9699EA1E98CA");
            }
        }

        public void RefreshAll()
        {
            macro.ClearAliasCache();
        }

        public void Refresh(Guid Id)
        {
            // Doesn't do anything
        }

        void ICacheRefresher.Refresh(int Id)
        {
            new macro(Id).removeFromCache();
        }

        #endregion
    }
}