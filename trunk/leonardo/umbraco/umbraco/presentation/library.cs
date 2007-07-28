using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Xml.XPath;
using CPalmTidy;
using Microsoft.ApplicationBlocks.Data;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.propertytype;
using umbraco.cms.businesslogic.relation;
using umbraco.cms.businesslogic.web;
using umbraco.cms.helpers;
using umbraco.presentation.cache;
using umbraco.scripting;

namespace umbraco
{
    /// <summary>
    /// Function library for umbraco. Includes various helper-methods and methods to
    /// save and load data from umbraco. 
    /// 
    /// Especially usefull in XSLT where any of these methods can be accesed using the umbraco.library name-space. Example:
    /// &lt;xsl:value-of select="umbraco.library:NiceUrl(@id)"/&gt;
    /// </summary>
    public class library
    {
        #region Declarations

        public static bool IsPublishing = false;
        public static int NodesPublished = 0;
        public static DateTime PublishStart;
        private page _page;

        #endregion

        #region Constructors

        /// <summary>
        /// Empty constructor
        /// </summary>
        public library()
        {
        }

        public library(page Page)
        {
            _page = Page;
        }

        #endregion

        #region Python Helper functions

        /// <summary>
        /// Executes the given python script and returns the standardoutput.
        /// The Globals known from python macros are not accessible in this context.
        /// Neighter macro or page nor the globals known from python macros are 
        /// accessible in this context. Only stuff we initialized in site.py
        /// can be used.
        /// </summary>
        /// <param name="file">The filename of the python script including the extension .py</param>
        /// <returns>Returns the StandardOutput</returns>
        public static string PythonExecuteFile(string file)
        {
            try
            {
                string path = HttpContext.Current.Server.MapPath(GlobalSettings.Path + "\\..\\python\\" + file);
                object res = python.executeFile(path);
                return res.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Executes the given python expression and returns the standardoutput.
        /// The Globals known from python macros are not accessible in this context.
        /// Neighter macro or page nor the globals known from python macros are 
        /// accessible in this context. Only stuff we initialized in site.py
        /// can be used.
        /// </summary>
        /// <param name="expression">Python expression to execute</param>
        /// <returns>Returns the StandardOutput</returns>
        public static string PythonExecute(string expression)
        {
            try
            {
                object res = python.execute(expression);
                return res.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        #endregion

        #region Publish Helper Methods

        // Esben Carlsen: Commented out, not referenced anywhere
        ///// <summary>
        ///// Updates nodes and eventually subnodes, making the latest version the one to be published.
        ///// Should always be used with library.rePublishNodes(), to ensure that the xml source is
        ///// updated
        ///// </summary>
        ///// <param name="publishChildren">Publish childnodes as well</param>
        ///// <returns></returns>
        //public static void PublishDocument(Guid nodeID, bool publishChildren, User u)
        //{
        //    Document d = new Document(nodeID, true);
        //    d.Publish(u);
        //    NodesPublished++;

        //    if(publishChildren)
        //        foreach(Document dc in d.Children)
        //        {
        //            PublishDocument(dc.UniqueId, true, u);
        //        }
        //}

        /// <summary>
        /// Unpublish a node, by removing it from the runtime xml index. Note, prior to this the Document should be 
        /// marked unpublished by setting the publish propertyon the document object to false
        /// </summary>
        /// <param name="DocumentId">The Id of the Document to be unpublished</param>
        public static void UnPublishSingleNode(int DocumentId)
        {
            content.Instance.UnPublishNode(DocumentId);
        }

        /// <summary>
        /// Publishes a Document by adding it to the runtime xml index. Note, prior to this the Document should be 
        /// marked published by calling Publish(User u) on the document object.
        /// </summary>
        /// <param name="DocumentId">The Id of the Document to be unpublished</param>
        public static void PublishSingleNode(int DocumentId)
        {
            if (UmbracoSettings.UseDistributedCalls)
                dispatcher.Refresh(
                    new Guid("27ab3022-3dfa-47b6-9119-5945bc88fd66"),
                    DocumentId);
            else
                PublishSingleNodeDo(DocumentId);
        }

        /// <summary>
        /// Internal method that shouldn't be used (TODO: Check if it safely can be private)
        /// Use umbraco.library.PublishSingleNode instead of this
        /// </summary>
        /// <param name="DocumentId">The Id of the Document to be unpublished</param>
        public static void PublishSingleNodeDo(int DocumentId)
        {
            content.Instance.PublishNode(DocumentId);

        }

        /// <summary>
        /// Re-publishes all nodes under a given node
        /// </summary>
        /// <param name="nodeID">The ID of the node and childnodes that should be republished</param>
        [Obsolete("Please use: content.Instance.RefreshContentFromDatabaseAsync")]
        public static string RePublishNodes(int nodeID)
        {
            content.Instance.RefreshContentFromDatabaseAsync();
            return string.Empty;
        }

        /// <summary>
        /// Re-publishes all nodes under a given node
        /// </summary>
        /// <param name="nodeID">The ID of the node and childnodes that should be republished</param>
        [Obsolete("Please use: content.Instance.RefreshContentFromDatabaseAsync")]
        public static void RePublishNodesDotNet(int nodeID)
        {
            content.Instance.RefreshContentFromDatabaseAsync();
        }

        //private static Hashtable parents = new Hashtable();
        //private static Hashtable nodes = new Hashtable();
        //private static int nodeRepublishCounter = 0;
        //public static void RePublishNodesDotNet(int nodeID)
        //{
        //    RePublishNodesDotNet(nodeID, true);
        //}
        // Esben Carlsen: Commented out, is not referenced anywhere
        //public static void _RePublishNodesDotNet(int nodeID, bool SaveToDisk)
        //{
        //    content.isInitializing = true;
        //    content.Instance.XmlContent = null;
        //    BusinessLogic.Log.Add(BusinessLogic.LogTypes.Debug, BusinessLogic.User.GetUser(0), -1, "Republishing starting");
        //    cms.businesslogic.cache.Cache.ClearAllCache();
        //    XmlDocument xmlDoc = new XmlDocument();
        //    // Create id -1 attribute
        //    xmlDoc.LoadXml("<root id=\"-1\"/>");
        //    XmlNode n = xmlDoc.DocumentElement;
        //    buildNodes(ref xmlDoc, ref n, -1);
        //    content.Instance.XmlContent.Load(n.OuterXml);
        //    // reload xml
        //    n = null;
        //    xmlDoc = null;
        //    if (SaveToDisk)
        //        content.SaveCacheToDisk(false);
        //    // Reload content
        //    requestHandler.ClearProcessedRequests();
        //    content.clearContentCache();
        //    BusinessLogic.Log.Add(BusinessLogic.LogTypes.Debug, BusinessLogic.User.GetUser(0), -1, "Republishing done");
        //    content.isInitializing = false;
        //}
        //        private static void buildNodes(ref XmlDocument Xd, ref XmlNode CurrentElement, int ParentId)
        //        {
        //            string sql =
        //                @"select umbracoNode.id, umbracoNode.sortOrder, cmsContentXml.xml from umbracoNode 
        //            inner join cmsContentXml on cmsContentXml.nodeId = umbracoNode.id and umbracoNode.nodeObjectType = 'C66BA18E-EAF3-4CFF-8A22-41B16D66A972'
        //            and umbracoNode.parentId = @parentId 
        //            order by umbracoNode.sortOrder";
        //            SqlDataReader dr =
        //                SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, sql, new SqlParameter("@parentId", ParentId));
        //            while(dr.Read())
        //            {
        //                int currentId = int.Parse(dr["id"].ToString());
        //                XmlNode n = xmlHelper.ImportXmlNodeFromText(dr["xml"].ToString(), ref Xd);
        //                CurrentElement.AppendChild(n);
        //                buildNodes(ref Xd, ref n, currentId);
        //            }
        //            dr.Close();
        //        }
        /// <summary>
        /// Refreshes the runtime xml index. 
        /// Note: This *doesn't* mark any non-published document objects as published
        /// </summary>
        /// <param name="nodeID">Allways use -1</param>
        /// <param name="SaveToDisk">Not used</param>
        [Obsolete("Please use: content.Instance.RefreshContentFromDatabaseAsync")]
        public static void RePublishNodesDotNet(int nodeID, bool SaveToDisk)
        {
            content.Instance.RefreshContentFromDatabaseAsync();
        }

        #endregion

        /* Helper functions primarily for xslt */

        #region Xslt Helper functions

        /// <summary>
        /// Add a session variable to the current user
        /// </summary>
        /// <param name="key">The Key of the variable</param>
        /// <param name="value">The Value</param>
        public static void setSession(string key, string value)
        {
            if (HttpContext.Current.Session != null)
                HttpContext.Current.Session[key] = value;
        }

        /// <summary>
        /// Add a cookie variable to the current user
        /// </summary>
        /// <param name="key">The Key of the variable</param>
        /// <param name="value">The Value of the variable</param>
        public static void setCookie(string key, string value)
        {
            StateHelper.SetCookieValue(key, value);
        }

        /// <summary>
        /// Returns a string with a friendly url from a node.
        /// IE.: Instead of having /482 (id) as an url, you can have
        /// /screenshots/developer/macros (spoken url)
        /// </summary>
        /// <param name="nodeID">Identifier for the node that should be returned</param>
        /// <returns>String with a friendly url from a node</returns>
        public static string NiceUrl(int nodeID)
        {
            try
            {
                int startNode = 1;
                if (GlobalSettings.HideTopLevelNodeFromPath)
                    startNode = 2;

                return niceUrlDo(nodeID, startNode);
            }
            catch
            {
                return "#";
            }
        }

        /// <summary>
        /// This method will always add the root node to the path. You should always use NiceUrl, as that is the
        /// only one who checks for toplevel node settings in the web.config
        /// </summary>
        /// <param name="nodeID">Identifier for the node that should be returned</param>
        /// <returns>String with a friendly url from a node</returns>
        public static string NiceUrlFullPath(int nodeID)
        {
            return niceUrlDo(nodeID, 1);
        }

        private static string niceUrlDo(int nodeID, int startNodeDepth)
        {
            XmlDocument umbracoXML = content.Instance.XmlContent;
            bool directoryUrls = GlobalSettings.UseDirectoryUrls;
            string baseUrl = GlobalSettings.Path;
            baseUrl = baseUrl.Substring(0, baseUrl.LastIndexOf("/"));

            bool atDomain = false;
            string currentDomain = HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToLower();
            if (UmbracoSettings.UseDomainPrefixes && Domain.Exists(currentDomain))
                atDomain = true;


            // Find path from nodeID
            String tempUrl = "";
            XmlElement node = umbracoXML.GetElementById(nodeID.ToString());
            String[] splitpath = null;
            if (node != null)
            {
                try
                {
                    splitpath =
                        umbracoXML.GetElementById(nodeID.ToString()).Attributes.GetNamedItem("path").Value.ToString().
                            Split(",".ToCharArray());

                    int startNode = startNodeDepth;

                    // check root nodes for domains
                    if (UmbracoSettings.UseDomainPrefixes && startNode > 1)
                    {
                        if (node.ParentNode.Name.ToLower() == "node")
                        {
                            Domain[] domains =
                                Domain.GetDomainsById(int.Parse(node.ParentNode.Attributes.GetNamedItem("id").Value));
                            if (
                                domains.Length > 0)
                            {
                                tempUrl =
                                    getUrlByDomain(int.Parse(node.ParentNode.Attributes.GetNamedItem("id").Value), "",
                                                   atDomain, currentDomain, true);
                            }
                        // test for domains on root nodes, then make the url domain only
                        } else if (Domain.GetDomainsById(nodeID).Length > 0)
                        {
                            tempUrl = getUrlByDomain(nodeID, "",
                                                   false, currentDomain, false);
                            return tempUrl;
                        }
                    }


                    if (splitpath.Length > startNode)
                    {
                        for (int i = startNode; i < splitpath.Length; i++)
                        {
                            tempUrl = getUrlByDomain(int.Parse(splitpath[i]), tempUrl, atDomain, currentDomain, false);
                        }
                    }
                    else
                    {
                        // check the root node for language
                        tempUrl += getUrlByDomain(nodeID, "", atDomain, currentDomain, false);
                    }
                }
                catch (Exception e)
                {
                    HttpContext.Current.Trace.Warn("library.NiceUrl",
                                                   string.Format("Error generating nice url for id '{0}'", nodeID), e);
                    tempUrl = "/" + nodeID;
                }
                tempUrl = appendUrlExtension(baseUrl, directoryUrls, tempUrl);
            }
            else
                HttpContext.Current.Trace.Warn("niceurl", string.Format("No node found at '{0}'", nodeID));

            return tempUrl;
        }

        private static string appendUrlExtension(string baseUrl, bool directoryUrls, string tempUrl)
        {
            if (!directoryUrls)
                // append .aspx extension if the url includes other than just the domain name
                if (tempUrl.ToString() != "" && (!tempUrl.StartsWith("http://") || tempUrl.Replace("http://", "").IndexOf("/") > -1))
                    tempUrl = baseUrl + tempUrl + ".aspx";
            return tempUrl;
        }

        private static string getUrlByDomain(int DocumentId, string tempUrl, bool atDomain, string currentDomain,
                                             bool emptyOnSameDomain)
        {
            Domain[] domains = Domain.GetDomainsById(DocumentId);
            if (!UmbracoSettings.UseDomainPrefixes || domains.Length == 0)
                tempUrl += "/" +
                           url.FormatUrl(
                               content.Instance.XmlContent.GetElementById(DocumentId.ToString()).Attributes.GetNamedItem
                                   ("urlName").Value);
            else
            {
                // check if one of the domains are the same as the current one
                if (atDomain)
                {
                    bool inDomainRange = false;
                    foreach (Domain d in domains)
                        if (d.Name.ToLower() == currentDomain)
                        {
                            inDomainRange = true;
                            break;
                        }

                    if (inDomainRange)
                    {
                        if (emptyOnSameDomain)
                            return tempUrl;
                        else
                            tempUrl = "/" +
                                      url.FormatUrl(
                                          content.Instance.XmlContent.GetElementById(DocumentId.ToString()).Attributes.
                                              GetNamedItem("urlName").Value);
                    }
                    else
                        tempUrl = "http://" + domains[0].Name;
                }
                else
                    tempUrl = "http://" + domains[0].Name;
            }

            return tempUrl;
        }

        /// <summary>
        /// Returns a string with the data from the given element of a node. Both elements (data-fields)
        /// and properties can be used - ie:
        /// getItem(1, nodeName) will return a string with the name of the node with id=1 even though
        /// nodeName is a property and not an element (data-field).
        /// </summary>
        /// <param name="nodeID">Identifier for the node that should be returned</param>
        /// <param name="alias">The element that should be returned</param>
        /// <returns>Returns a string with the data from the given element of a node</returns>
        public static string GetItem(int nodeID, String alias)
        {
            XmlDocument umbracoXML = content.Instance.XmlContent;

            if (umbracoXML.GetElementById(nodeID.ToString()) != null)
                if (
                    ",id,version,parentID,level,writerID,dataType,template,sortOrder,createDate,updateDate,nodeName,writerName,path,"
                        .
                        IndexOf("," + alias + ",") > -1)
                    return umbracoXML.GetElementById(nodeID.ToString()).Attributes.GetNamedItem(alias).Value;
                else if (
                    umbracoXML.GetElementById(nodeID.ToString()).SelectSingleNode("./data [@alias='" + alias + "']") !=
                    null)
                    return
                        umbracoXML.GetElementById(nodeID.ToString()).SelectSingleNode("./data [@alias = '" + alias +
                                                                                      "']").ChildNodes[0].
                            Value; //.Value + "*";
                else
                    return string.Empty;
            else
                return string.Empty;
        }

        /// <summary>
        /// Checks with the Assigned domains settings and retuns an array the the Domains matching the node
        /// </summary>
        /// <param name="NodeId">Identifier for the node that should be returned</param>
        /// <returns>A Domain array with all the Domains that matches the nodeId</returns>
        public static Domain[] GetCurrentDomains(int NodeId)
        {
            string[] pathIds = GetItem(NodeId, "path").Split(',');
            for (int i = pathIds.Length - 1; i > 0; i--)
            {
                Domain[] retVal = Domain.GetDomainsById(int.Parse(pathIds[i]));
                if (retVal.Length > 0)
                {
                    return retVal;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a string with the data from the given element of the current node. Both elements (data-fields)
        /// and properties can be used - ie:
        /// getItem(nodeName) will return a string with the name of the current node/page even though
        /// nodeName is a property and not an element (data-field).
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public static string GetItem(String alias)
        {
            try
            {
                int currentID = int.Parse(HttpContext.Current.Items["pageID"].ToString());
                return GetItem(currentID, alias);
            }
            catch (Exception ItemException)
            {
                HttpContext.Current.Trace.Warn("library.GetItem", "Error reading '" + alias + "'", ItemException);
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns that name of a generic property
        /// </summary>
        /// <param name="ContentTypeAlias">The Alias of the content type (ie. Document Type, Member Type or Media Type)</param>
        /// <param name="PropertyTypeAlias">The Alias of the Generic property (ie. bodyText or umbracoNaviHide)</param>
        /// <returns>A string with the name. If nothing matches the alias, an empty string is returned</returns>
        public static string GetPropertyTypeName(string ContentTypeAlias, string PropertyTypeAlias)
        {
            try
            {
                ContentType ct = ContentType.GetByAlias(ContentTypeAlias);
                PropertyType pt = ct.getPropertyType(PropertyTypeAlias);
                return pt.Name;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the Member Name from an umbraco member object
        /// </summary>
        /// <param name="MemberId">The identifier of the Member</param>
        /// <returns>The Member name matching the MemberId, an empty string is member isn't found</returns>
        public static string GetMemberName(int MemberId)
        {
            if (MemberId != 0)
            {
                try
                {
                    Member m = new Member(MemberId);
                    return m.Text;
                }
                catch
                {
                    return string.Empty;
                }
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Get a media object as an xml object
        /// </summary>
        /// <param name="MediaId">The identifier of the media object to be returned</param>
        /// <param name="Deep">If true, children of the media object is returned</param>
        /// <returns>An umbraco xml node of the media (same format as a document node)</returns>
        public static XPathNodeIterator GetMedia(int MediaId, bool Deep)
        {
            try
            {
                Media m = new Media(MediaId);
                XmlDocument mXml = new XmlDocument();
                mXml.LoadXml(m.ToXml(mXml, Deep).OuterXml);
                XPathNavigator xp = mXml.CreateNavigator();
                return xp.Select("/node");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get a member as an xml object
        /// </summary>
        /// <param name="MemberId">The identifier of the member object to be returned</param>
        /// <returns>An umbraco xml node of the member (same format as a document node), but with two additional attributes on the "node" element:
        /// "email" and "loginName".
        /// </returns>
        public static XPathNodeIterator GetMember(int MemberId)
        {
            try
            {
                Member m = new Member(MemberId);
                XmlDocument mXml = new XmlDocument();
                mXml.LoadXml(m.ToXml(mXml, false).OuterXml);
                XPathNavigator xp = mXml.CreateNavigator();
                return xp.Select("/node");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get the current member as an xml node
        /// </summary>
        /// <returns>Look in documentation for umbraco.library.GetMember(MemberId) for more information</returns>
        public static XPathNodeIterator GetCurrentMember()
        {
            Member m = Member.GetCurrentMember();
            if (m != null)
            {
                XmlDocument mXml = new XmlDocument();
                mXml.LoadXml(m.ToXml(mXml, false).OuterXml);
                XPathNavigator xp = mXml.CreateNavigator();
                return xp.Select("/node");
            }
            else
                return null;
        }

        /// <summary>
        /// Whether or not the current user is logged in (as a member)
        /// </summary>
        /// <returns>True is the current user is logged in</returns>
        public static bool IsLoggedOn()
        {
            return Member.IsLoggedOn();
        }

        /// <summary>
        /// Check if a document object is protected by the "Protect Pages" functionality in umbraco
        /// </summary>
        /// <param name="DocumentId">The identifier of the document object to check</param>
        /// <param name="Path">The full path of the document object to check</param>
        /// <returns>True if the document object is protected</returns>
        public static bool IsProtected(int DocumentId, string Path)
        {
            return Access.IsProtected(DocumentId, Path);
        }

        /// <summary>
        /// Check if the current user has access to a document
        /// </summary>
        /// <param name="NodeId">The identifier of the document object to check</param>
        /// <param name="Path">The full path of the document object to check</param>
        /// <returns>True if the current user has access or if the current document isn't protected</returns>
        public static bool HasAccess(int NodeId, string Path)
        {
            if (Member.IsLoggedOn())
                return Access.HasAccess(NodeId, Path, Member.GetCurrentMember());
            else
                return false;
        }

        /// <summary>
        /// Compare two dates
        /// </summary>
        /// <param name="firstDate">The first date to compare</param>
        /// <param name="secondDate">The second date to compare</param>
        /// <returns>True if the first date is greater than the second date</returns>
        public static bool DateGreaterThan(string firstDate, string secondDate)
        {
            if (DateTime.Parse(firstDate) > DateTime.Parse(secondDate))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Compare two dates
        /// </summary>
        /// <param name="firstDate">The first date to compare</param>
        /// <param name="secondDate">The second date to compare</param>
        /// <returns>True if the first date is greater than or equal the second date</returns>
        public static bool DateGreaterThanOrEqual(string firstDate, string secondDate)
        {
            if (DateTime.Parse(firstDate) >= DateTime.Parse(secondDate))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Check if a date is greater than today
        /// </summary>
        /// <param name="firstDate">The date to check</param>
        /// <returns>True if the date is greater that today (ie. at least the day of tomorrow)</returns>
        public static bool DateGreaterThanToday(string firstDate)
        {
            DateTime first = DateTime.Parse(firstDate);
            first = new DateTime(first.Year, first.Month, first.Day);
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            TimeSpan TS = new TimeSpan(first.Ticks - today.Ticks);
            if (TS.Days > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Check if a date is greater than or equal today
        /// </summary>
        /// <param name="firstDate">The date to check</param>
        /// <returns>True if the date is greater that or equal today (ie. at least today or the day of tomorrow)</returns>
        public static bool DateGreaterThanOrEqualToday(string firstDate)
        {
            DateTime first = DateTime.Parse(firstDate);
            first = new DateTime(first.Year, first.Month, first.Day);
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            TimeSpan TS = new TimeSpan(first.Ticks - today.Ticks);
            if (TS.Days >= 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get the current date
        /// </summary>
        /// <returns>Current date i xml format (ToString("s"))</returns>
        public static string CurrentDate()
        {
            return DateTime.Now.ToString("s");
        }

        /// <summary>
        /// Add a value to a date
        /// </summary>
        /// <param name="Date">The Date to user</param>
        /// <param name="AddType">The type to add: "y": year, "m": month, "d": day, "h": hour, "min": minutes, "s": seconds</param>
        /// <param name="add">An integer value to add</param>
        /// <returns>A date in xml format (ToString("s"))</returns>
        public static string DateAdd(string Date, string AddType, int add)
        {
            return DateAdd(DateTime.Parse(Date), AddType, add);
        }

        /// <summary>
        /// Get the day of week from a date matching the current culture settings
        /// </summary>
        /// <param name="Date">The date to use</param>
        /// <returns>A string with the DayOfWeek matching the current contexts culture settings</returns>
        public static string GetWeekDay(string Date)
        {
            return DateTime.Parse(Date).DayOfWeek.ToString();
        }

        /// <summary>
        /// Add a value to a date. Similar to the other overload, but uses a datetime object instead of a string
        /// </summary>
        /// <param name="Date">The Date to user</param>
        /// <param name="AddType">The type to add: "y": year, "m": month, "d": day, "h": hour, "min": minutes, "s": seconds</param>
        /// <param name="add">An integer value to add</param>
        /// <returns>A date in xml format (ToString("s"))</returns>
        public static string DateAdd(DateTime Date, string AddType, int add)
        {
            switch (AddType.ToLower())
            {
                case "y":
                    Date = Date.AddYears(add);
                    break;
                case "m":
                    Date = Date.AddMonths(add);
                    break;
                case "d":
                    Date = Date.AddDays(add);
                    break;
                case "h":
                    Date = Date.AddHours(add);
                    break;
                case "min":
                    Date = Date.AddMinutes(add);
                    break;
                case "s":
                    Date = Date.AddSeconds(add);
                    break;
            }

            return Date.ToString("s");
        }

        public static int DateDiff(string firstDate, string secondDate, string diffType)
        {
            TimeSpan TS = DateTime.Parse(firstDate).Subtract(DateTime.Parse(secondDate));

            switch (diffType.ToLower())
            {
                case "m":
                    return Convert.ToInt32(TS.TotalMinutes);
                case "s":
                    return Convert.ToInt32(TS.TotalSeconds);
                case "y":
                    return Convert.ToInt32(TS.TotalDays/365);
            }
            // return default
            return 0;
        }

        public static string FormatDateTime(string Date, string Format)
        {
            DateTime result;
            if (DateTime.TryParse(Date, out result))
                return result.ToString(Format);
            return string.Empty;
        }

        public static string LongDate(string Date, bool WithTime, string TimeSplitter)
        {
            DateTime result;
            if (DateTime.TryParse(Date, out result))
            {
                if (WithTime)
                    return result.ToLongDateString() + TimeSplitter + result.ToLongTimeString();
                return result.ToLongDateString();
            }
            return string.Empty;
        }

        public static bool CultureExists(string cultureName)
        {
            CultureInfo[] ci = CultureInfo.GetCultures(CultureTypes.AllCultures);
            CultureInfo c = Array.Find(ci, delegate(CultureInfo culture) { return culture.Name == cultureName; });
            return c != null;
        }

        public static string LongDateWithDayName(string Date, string DaySplitter, bool WithTime, string TimeSplitter,
                                                 string GlobalAlias)
        {
            if (!CultureExists(GlobalAlias))
                return string.Empty;

            DateTime result;
            CultureInfo.GetCultureInfo(GlobalAlias);
            DateTimeFormatInfo dtInfo = CultureInfo.GetCultureInfo(GlobalAlias).DateTimeFormat;
            if (DateTime.TryParse(Date, dtInfo, DateTimeStyles.None, out result))
            {
                if (WithTime)
                    return
                        result.ToString(dtInfo.LongDatePattern) + TimeSplitter + result.ToString(dtInfo.LongTimePattern);
                return result.ToString(dtInfo.LongDatePattern);
            }
            return string.Empty;
        }

        public static string LongDate(string Date)
        {
            DateTime result;
            if (DateTime.TryParse(Date, out result))
                return result.ToLongDateString();
            return string.Empty;
        }

        public static string ShortDate(string Date)
        {
            DateTime result;
            if (DateTime.TryParse(Date, out result))
                return result.ToShortDateString();
            return string.Empty;
        }

        public static string ShortDateWithGlobal(string Date, string GlobalAlias)
        {
            if (!CultureExists(GlobalAlias))
                return string.Empty;

            DateTime result;
            if (DateTime.TryParse(Date, out result))
            {
                DateTimeFormatInfo dtInfo = CultureInfo.GetCultureInfo(GlobalAlias).DateTimeFormat;
                return result.ToString(dtInfo.ShortDatePattern);
            }
            return string.Empty;
        }

        public static string ShortDateWithTimeAndGlobal(string Date, string GlobalAlias)
        {
            if (!CultureExists(GlobalAlias))
                return string.Empty;

            DateTime result;
            if (DateTime.TryParse(Date, out result))
            {
                DateTimeFormatInfo dtInfo = CultureInfo.GetCultureInfo(GlobalAlias).DateTimeFormat;
                return result.ToString(dtInfo.ShortDatePattern) + " " +
                       result.ToString(dtInfo.ShortTimePattern);
            }
            return string.Empty;
        }

        public static string ShortTime(string Date)
        {
            DateTime result;
            if (DateTime.TryParse(Date, out result))
                return result.ToShortTimeString();
            return string.Empty;
        }

        public static string ShortDate(string Date, bool WithTime, string TimeSplitter)
        {
            DateTime result;
            if (DateTime.TryParse(Date, out result))
            {
                if (WithTime)
                    return result.ToShortDateString() + TimeSplitter + result.ToLongTimeString();
                return result.ToShortDateString();
            }
            return string.Empty;
        }

        public static string ReplaceLineBreaks(string text)
        {
            if (bool.Parse(GlobalSettings.EditXhtmlMode))
                return text.Replace("\n", "<br/>\n");
            else
                return text.Replace("\n", "<br />\n");
        }

        public static string RenderMacroContent(string Text, int PageId)
        {
            try
            {
                page p = new page(((IHasXmlNode) GetXmlNodeById(PageId.ToString()).Current).GetNode());
                template t = new template(p.Template);
                Control c = t.parseStringBuilder(new StringBuilder(Text), p);

                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                c.RenderControl(hw);

                return sw.ToString();
            }
            catch (Exception ee)
            {
                return string.Format("<!-- Error generating macroContent: '{0}' -->", ee);
            }
        }

        public static string RenderTemplate(int PageId, int TemplateId)
        {
            try
            {
                page p = new page(((IHasXmlNode) GetXmlNodeById(PageId.ToString()).Current).GetNode());
                p.RenderPage(TemplateId);
                Control c = p.PageContentControl;
                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                c.RenderControl(hw);

                return sw.ToString();
            }
            catch (Exception ee)
            {
                return string.Format("<!-- Error generating macroContent: '{0}' -->", ee);
            }
        }

        public static string RenderTemplate(int PageId)
        {
            return
                RenderTemplate(PageId,
                               new page(((IHasXmlNode) GetXmlNodeById(PageId.ToString()).Current).GetNode()).Template);
        }

        public static string StripHtml(string text)
        {
            string pattern = @"<(.|\n)*?>";
            return Regex.Replace(text, pattern, string.Empty);
        }

        /// <summary>
        /// Truncates a string if it's too long
        /// </summary>
        /// <param name="Text">The text to eventually truncate</param>
        /// <param name="MaxLength">The maximum number of characters (length)</param>
        /// <param name="AddString">String to append if text is truncated (ie "...")</param>
        /// <returns>A truncated string if text if longer than MaxLength appended with the addString parameters. If text is shorter
        /// then MaxLength then the full - non-truncated - string is returned</returns>
        public static string TruncateString(string Text, int MaxLength, string AddString)
        {
            if (Text.Length > MaxLength)
                return Text.Substring(0, MaxLength - AddString.Length) + AddString;
            else
                return Text;
        }

        /// <summary>
        /// Split a string into xml elements
        /// </summary>
        /// <param name="StringToSplit">The full text to spil</param>
        /// <param name="Separator">The separator</param>
        /// <returns>An XPathNodeIterator containing the substrings in the format of <values><value></value></values></returns>
        public static XPathNodeIterator Split(string StringToSplit, string Separator)
        {
            string[] values = StringToSplit.Split(Convert.ToChar(Separator));
            XmlDocument xd = new XmlDocument();
            xd.LoadXml("<values/>");
            foreach (string id in values)
            {
                XmlNode node = xmlHelper.addTextNode(xd, "value", id);
                xd.DocumentElement.AppendChild(node);
            }
            XPathNavigator xp = xd.CreateNavigator();
            return xp.Select("/values");
        }

        public static string RemoveFirstParagraphTag(string text)
        {
            text = text.Trim().Replace("\n", string.Empty).Replace("\r", string.Empty);
            if (text.Length > 5)
            {
                if (text.ToUpper().Substring(0, 3) == "<P>")
                    text = text.Substring(3, text.Length - 3);
                if (text.ToUpper().Substring(text.Length - 4, 4) == "</P>")
                    text = text.Substring(0, text.Length - 4);
            }
            return text;
        }

        public static string Replace(string text, string oldValue, string newValue)
        {
            return text.Replace(oldValue, newValue);
        }

        public static int LastIndexOf(string Text, string Value)
        {
            return Text.LastIndexOf(Value);
        }

        public static XPathNodeIterator GetPreValues(int DataTypeId)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml("<preValues/>");

            using(SqlDataReader dr = SqlHelper.ExecuteReader(
                GlobalSettings.DbDSN,
                CommandType.Text,
                "Select id, [value] from cmsDataTypeprevalues where DataTypeNodeId = @dataTypeId order by sortorder",
                new SqlParameter("@dataTypeId", DataTypeId)))
            {
            	while(dr.Read())
            	{
            		XmlNode n = xmlHelper.addTextNode(xd, "preValue", dr[1].ToString());
            		n.Attributes.Append(xmlHelper.addAttribute(xd, "id", dr[0].ToString()));
            		xd.DocumentElement.AppendChild(n);
            	}
            }
        	XPathNavigator xp = xd.CreateNavigator();
            return xp.Select("/preValues");
        }

        public static string GetPreValueAsString(int Id)
        {
            try
            {
                return
                    SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
                                            "select [value] from cmsDataTypePreValues where id = @id",
                                            new SqlParameter("@id", Id)).ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static XPathNodeIterator GetDictionaryItems(string Key)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml("<DictionaryItems/>");

            try
            {
                int languageId = 0;

                Domain[] domains = GetCurrentDomains(int.Parse(HttpContext.Current.Items["pageID"].ToString()));
                if (domains != null)
                    if (domains.Length > -1)
                        languageId = domains[0].Language.id;

                // Get requested item
                Dictionary.DictionaryItem di =
                    new Dictionary.DictionaryItem(Key);

                foreach (Dictionary.DictionaryItem item in di.Children)
                {
                    XmlNode xe;
                    try
                    {
                        if (languageId != 0)
                            xe = xmlHelper.addTextNode(xd, "DictionaryItem", item.Value(languageId));
                        else
                            xe = xmlHelper.addTextNode(xd, "DictionaryItem", item.Value());
                    }
                    catch
                    {
                        xe = xmlHelper.addTextNode(xd, "DictionaryItem", string.Empty);
                    }
                    xe.Attributes.Append(xmlHelper.addAttribute(xd, "key", item.key));
                    xd.DocumentElement.AppendChild(xe);
                }
            }
            catch (Exception ee)
            {
                xd.DocumentElement.AppendChild(
                    xmlHelper.addTextNode(xd, "Error", ee.ToString()));
            }

            XPathNavigator xp = xd.CreateNavigator();
            return xp.Select("/");
        }

        public static string GetDictionaryItem(string Key)
        {
            try
            {
                string pageId = HttpContext.Current.Items["pageID"] as string;
                Domain[] domains = GetCurrentDomains(int.Parse(pageId));
                if (domains != null)
                {
                    if (domains.Length > -1)
                    {
                        return new Dictionary.DictionaryItem(Key).Value(domains[0].Language.id);
                    }
                }

                return new Dictionary.DictionaryItem(Key).Value();
            }
            catch (Exception errDictionary)
            {
                HttpContext.Current.Trace.Warn("library", "Error outputting dictionary item '" + Key + "'",
                                               errDictionary);
                return string.Empty;
            }
        }

        public static XPathNodeIterator GetXmlNodeCurrent()
        {
            try
            {
                XPathNavigator xp = content.Instance.XmlContent.CreateNavigator();
                xp.MoveToId(HttpContext.Current.Items["pageID"].ToString());
                return xp.Select(".");
            }
            catch
            {
                return null;
            }
        }

        public static XPathNodeIterator GetXmlNodeById(string id)
        {
            XPathNavigator xp = content.Instance.XmlContent.CreateNavigator();
            xp.MoveToId(id);
            return xp.Select(".");
        }

        public static XPathNodeIterator GetXmlNodeByXPath(string xpathQuery)
        {
            XPathNavigator xp = content.Instance.XmlContent.CreateNavigator();
            return xp.Select(xpathQuery);
        }

        public static XPathNodeIterator GetXmlAll()
        {
            XPathNavigator xp = content.Instance.XmlContent.CreateNavigator();
            return xp.Select("/root");
        }

        public static XPathNodeIterator GetXmlDocument(string Path, bool Relative)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                if (Relative)
                    xmlDoc.Load(HttpContext.Current.Server.MapPath(Path));
                else
                    xmlDoc.Load(Path);
            }
            catch (Exception err)
            {
                xmlDoc.LoadXml(string.Format("<error path=\"{0}\" relative=\"{1}\">{2}</error>",
                                             HttpContext.Current.Server.HtmlEncode(Path), Relative, err));
            }
            XPathNavigator xp = xmlDoc.CreateNavigator();
            return xp.Select("/");
        }

        public static XPathNodeIterator GetXmlDocumentByUrl(string Url)
        {
            XmlDocument xmlDoc = new XmlDocument();
            WebRequest request = WebRequest.Create(Url);
            try
            {
                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                XmlTextReader reader = new XmlTextReader(responseStream);

                xmlDoc.Load(reader);

                response.Close();
                responseStream.Close();
            }
            catch (Exception err)
            {
                xmlDoc.LoadXml(string.Format("<error url=\"{0}\">{1}</error>",
                                             HttpContext.Current.Server.HtmlEncode(Url), err));
            }
            XPathNavigator xp = xmlDoc.CreateNavigator();
            return xp.Select("/");
        }

        public static string QueryForNode(string id)
        {
            string XPathQuery = string.Empty;
            if (content.Instance.XmlContent.GetElementById(id) != null)
            {
                string[] path =
                    content.Instance.XmlContent.GetElementById(id).Attributes["path"].Value.Split((",").ToCharArray());
                for (int i = 1; i < path.Length; i++)
                {
                    if (i > 1)
                        XPathQuery += "/node [@id = " + path[i] + "]";
                    else
                        XPathQuery += " [@id = " + path[i] + "]";
                }
            }

            return XPathQuery;
        }

        /// <summary>
        /// Helper function to get a value from a comma separated string. Usefull to get
        /// a node identifier from a Page's path string
        /// </summary>
        /// <param name="path">The comma separated string</param>
        /// <param name="level">The index to be returned</param>
        /// <returns>A string with the value of the index</returns>
        public static string GetNodeFromLevel(string path, int level)
        {
            try
            {
                string[] newPath = path.Split(',');
                if (newPath.Length >= level)
                    return newPath[level].ToString();
                else
                    return string.Empty;
            }
            catch
            {
                return "<!-- error in GetNodeFromLevel -->";
            }
        }

        /// <summary>
        /// Sends an e-mail using the System.Net.Mail.MailMessage object
        /// </summary>
        /// <param name="FromMail">The sender of the e-mail</param>
        /// <param name="ToMail">The recipient of the e-mail</param>
        /// <param name="Subject">E-mail subject</param>
        /// <param name="Body">The complete content of the e-mail</param>
        /// <param name="IsHtml">Set to true when using Html formatted mails</param>
        public static void SendMail(string FromMail, string ToMail, string Subject, string Body, bool IsHtml)
        {
            try
            {
                // create the mail message 
                MailMessage mail = new MailMessage(FromMail.Trim(), ToMail.Trim());

                // populate the message
                mail.Subject = Subject;
                if (IsHtml)
                    mail.IsBodyHtml = true;
                else
                    mail.IsBodyHtml = false;

                mail.Body = Body;

                // send it
                SmtpClient smtpClient = new SmtpClient(GlobalSettings.SmtpServer);
                smtpClient.Send(mail);
            }
            catch (Exception ee)
            {
                Log.Add(LogTypes.Error, -1,
                        string.Format("umbraco.library.SendMail: Error sending mail. Exception: {0}", ee));
            }
        }

        /// <summary> 
        /// These random methods are from Eli Robillards blog - kudos for the work :-)
        /// http://weblogs.asp.net/erobillard/archive/2004/05/06/127374.aspx
        /// 
        /// Get a Random object which is cached between requests. 
        /// The advantage over creating the object locally is that the .Next 
        /// call will always return a new value. If creating several times locally 
        /// with a generated seed (like millisecond ticks), the same number can be 
        /// returned. 
        /// </summary> 
        /// <returns>A Random object which is cached between calls.</returns> 
        public static Random GetRandom(int seed)
        {
            Random r = (Random) HttpContext.Current.Cache.Get("RandomNumber");
            if (r == null)
            {
                if (seed == 0)
                    r = new Random();
                else
                    r = new Random(seed);
                HttpContext.Current.Cache.Insert("RandomNumber", r);
            }
            return r;
        }

        /// <summary> 
        /// GetRandom with no parameters. 
        /// </summary> 
        /// <returns>A Random object which is cached between calls.</returns> 
        public static Random GetRandom()
        {
            return GetRandom(0);
        }

        /// <summary>
        /// Get any value from the current Request collection. Please note that there also specialized methods for
        /// Querystring, Form, Servervariables and Cookie collections
        /// </summary>
        /// <param name="key">Name of the Request element to be returned</param>
        /// <returns>A string with the value of the Requested element</returns>
        public static string Request(string key)
        {
            if (HttpContext.Current.Request[key] != null)
                return HttpContext.Current.Request[key];
            else
                return string.Empty;
        }

        /// <summary>
        /// Get any value from the current Items collection.
        /// </summary>
        /// <param name="key">Name of the Items element to be returned</param>
        /// <returns>A string with the value of the Items element</returns>
        public static string ContextKey(string key)
        {
            if (HttpContext.Current.Items[key] != null)
                return HttpContext.Current.Items[key].ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Get any value from the current Http Items collection
        /// </summary>
        /// <param name="key">Name of the Item element to be returned</param>
        /// <returns>A string with the value of the Requested element</returns>
        public static string GetHttpItem(string key)
        {
            if (HttpContext.Current.Items[key] != null)
                return HttpContext.Current.Items[key].ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Get any value from the current Form collection
        /// </summary>
        /// <param name="key">Name of the Form element to be returned</param>
        /// <returns>A string with the value of the form element</returns>
        public static string RequestForm(string key)
        {
            if (HttpContext.Current.Request.Form[key] != null)
                return HttpContext.Current.Request.Form[key];
            else
                return string.Empty;
        }

        /// <summary>
        /// Get any value from the current Querystring collection
        /// </summary>
        /// <param name="key">Name of the querystring element to be returned</param>
        /// <returns>A string with the value of the querystring element</returns>
        public static string RequestQueryString(string key)
        {
            if (HttpContext.Current.Request.QueryString[key] != null)
                return HttpContext.Current.Request.QueryString[key];
            else
                return string.Empty;
        }

        /// <summary>
        /// Get any value from the users cookie collection
        /// </summary>
        /// <param name="key">Name of the cookie to return</param>
        /// <returns>A string with the value of the cookie</returns>
        public static string RequestCookies(string key)
        {
            if (HttpContext.Current.Request.Cookies[key] != null)
                return HttpContext.Current.Request.Cookies[key].Value;
            else
                return string.Empty;
        }

        /// <summary>
        /// Get any element from the server variables collection
        /// </summary>
        /// <param name="key">The key for the element to be returned</param>
        /// <returns>A string with the value of the requested element</returns>
        public static string RequestServerVariables(string key)
        {
            if (HttpContext.Current.Request.ServerVariables[key] != null)
                return HttpContext.Current.Request.ServerVariables[key];
            else
                return string.Empty;
        }

        /// <summary>
        /// Get any element from current user session
        /// </summary>
        /// <param name="key">The key for the element to be returned</param>
        /// <returns>A string with the value of the requested element</returns>
        public static string Session(string key)
        {
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[key] != null)
                return HttpContext.Current.Session[key].ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Returns the current ASP.NET session identifier
        /// </summary>
        /// <returns>The current ASP.NET session identifier</returns>
        public static string SessionId()
        {
            if (HttpContext.Current.Session != null)
                return HttpContext.Current.Session.SessionID;
            else
                return string.Empty;
        }

        /// <summary>
        /// Urlencodes a string 
        /// </summary>
        /// <param name="Text">The string to be encoded</param>
        /// <returns>A urlencoded string</returns>
        public static string UrlEncode(string Text)
        {
            return HttpContext.Current.Server.UrlEncode(Text);
        }

        public static Relation[] GetRelatedNodes(int NodeId)
        {
            return new CMSNode(NodeId).Relations;
        }

        public static XPathNodeIterator GetRelatedNodesAsXml(int NodeId)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml("<relations/>");
            foreach (Relation r in new CMSNode(NodeId).Relations)
            {
                XmlElement n = xd.CreateElement("relation");
                n.AppendChild(xmlHelper.addCDataNode(xd, "comment", r.Comment));
                n.Attributes.Append(xmlHelper.addAttribute(xd, "typeId", r.RelType.Id.ToString()));
                n.Attributes.Append(xmlHelper.addAttribute(xd, "typeName", r.RelType.Name));
                n.Attributes.Append(xmlHelper.addAttribute(xd, "createDate", r.CreateDate.ToString()));
                n.Attributes.Append(xmlHelper.addAttribute(xd, "parentId", r.Parent.Id.ToString()));
                n.Attributes.Append(xmlHelper.addAttribute(xd, "childId", r.Child.Id.ToString()));

                // Append the node that isn't the one we're getting the related nodes from
                if (NodeId == r.Child.Id)
                    n.AppendChild(r.Parent.ToXml(xd, false));
                else
                    n.AppendChild(r.Child.ToXml(xd, false));
                xd.DocumentElement.AppendChild(n);
            }
            XPathNavigator xp = xd.CreateNavigator();
            return xp.Select(".");
        }

        /// <summary>
        /// Returns the identifier of the current page
        /// </summary>
        /// <returns>The identifier of the current page</returns>
        public int PageId()
        {
            if (_page != null)
                return _page.PageID;
            else
                return -1;
        }

        /// <summary>
        /// Returns the title of the current page
        /// </summary>
        /// <returns>The title of the current page</returns>
        public string PageName()
        {
            if (_page != null)
                return _page.PageName;
            else
                return string.Empty;
        }

        /// <summary>
        /// Returns any element from the currentpage including generic properties
        /// </summary>
        /// <param name="key">The name of the page element to return</param>
        /// <returns>A string with the element value</returns>
        public string PageElement(string key)
        {
            if (_page != null)
            {
                if (_page.Elements[key] != null)
                    return _page.Elements[key].ToString();
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }

        /*		public static string Tidy(string StringToTidy, bool LiveEditing) 
		{
			try 
			{
				string tidyConfig;
				if (LiveEditing)
					tidyConfig = System.Web.HttpContext.Current.Server.MapPath(umbraco.GlobalSettings.Path + "/../config/") + "tidy_liveediting.cfg";
				else
					tidyConfig = System.Web.HttpContext.Current.Server.MapPath(umbraco.GlobalSettings.Path + "/../config/") + "tidy.cfg";

				NTidy.TidyDocument doc = new NTidy.TidyDocument();
				doc.LoadConfig(tidyConfig);
				NTidy.TidyStatus  status = doc.LoadString(StringToTidy);
				doc.CleanAndRepair();

				return doc.ToString();
			} catch (Exception ee) {
				BusinessLogic.Log.Add(BusinessLogic.LogTypes.Error, BusinessLogic.User.GetUser(0), -1, "Error tidying: " + ee.ToString());
				return StringToTidy;
			}		
		}
*/

        public static string Tidy(string StringToTidy, bool LiveEditing)
        {
            string fileNameBefore = string.Format("{0}tidyThis{1}.txt",
                                                  HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../data/"),
                                                  Guid.NewGuid());
            string fileNameAfter = string.Format("{0}tidyAfter{1}.txt",
                                                 HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../data/"),
                                                 Guid.NewGuid());
            string tidyConfig;
            if (LiveEditing)
                tidyConfig = HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../config/") +
                             "tidy_liveediting.cfg";
            else
                tidyConfig = HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../config/") + "tidy.cfg";

            try
            {
                File.WriteAllText(fileNameBefore, StringToTidy);

                Tidy tidy =
                    new Tidy();
                tidy.InputFile = fileNameBefore;
                tidy.OutputFile = fileNameAfter;
                tidy.ConfigFile = tidyConfig;
                string result;
                try
                {
                    tidy.Run();

                    // read the result
                    result = File.ReadAllText(fileNameAfter);
                }
                finally
                {
                    // cleanup
                    if (File.Exists(fileNameAfter))
                        File.Delete(fileNameAfter);
                }

                if ((tidy.ErrorValue == "1" || tidy.ErrorValue == "0") && result != string.Empty)
                    return result;
                else
                {
                    Log.Add(LogTypes.Error, User.GetUser(0), -1, "Error tidying - resultcode: (" + tidy.ErrorValue + ")");
                    return "[error]";
                }
            }
            finally
            {
                File.Delete(fileNameBefore);
            }
        }

        #endregion
    }
}