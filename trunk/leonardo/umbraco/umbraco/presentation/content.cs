/// <changelog>
///   <item who="Esben" when="18. november 2006">Rewrote</item>
/// </changelog>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using Microsoft.ApplicationBlocks.Data;
using umbraco.BusinessLogic;
using umbraco.BusinessLogic.Actions;
using umbraco.cms.businesslogic.cache;
using umbraco.cms.businesslogic.web;

namespace umbraco
{
    /// <summary>
    /// Handles umbraco content
    /// </summary>
    public class content
    {
        #region Declarations

        private readonly string UmbracoXmlDiskCacheFileName = HttpRuntime.AppDomainAppPath +
                                                              GlobalSettings.ContentXML.Replace('/', '\\').TrimStart(
                                                                  '\\');

        private readonly string XmlContextContentItemKey = "UmbracoXmlContextContent";

        // Current content
        private volatile XmlDocument _xmlContent = null;

        // Sync access to disk file
        private object _readerWriterSyncLock = new object();

        // Sync access to internal cache
        private object _xmlContentInternalSyncLock = new object();

        // Sync database access
        private object _dbReadSyncLock = new object();

        #endregion

        #region Constructors

        public content()
        {
            ;
        }

        static content()
        {
            Trace.Write("Initializing content");
            ThreadPool.QueueUserWorkItem(
                delegate
                    {
                        XmlDocument xmlDoc = Instance.XmlContentInternal;
                        Trace.WriteLine("Content initialized");
                    });
        }

        #endregion

        #region Singleton

        public static content Instance
        {
            get { return Singleton<content>.Instance; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get content. First call to this property will initialize xmldoc
        /// subsequent calls will be blocked until initialization is done
        /// Further we cache(in context) xmlContent for each request to ensure that
        /// we always have the same XmlDoc throughout the whole request.
        /// Note that context cache does not need to be locked, because all access
        /// to it is done from a web request which runs in a single thread
        /// </summary>
        public virtual XmlDocument XmlContent
        {
            get
            {
                if (HttpContext.Current == null)
                    return XmlContentInternal;
                XmlDocument content = HttpContext.Current.Items[XmlContextContentItemKey] as XmlDocument;
                if (content == null)
                {
                    content = XmlContentInternal;
                    HttpContext.Current.Items[XmlContextContentItemKey] = content;
                }
                return content;
            }
        }

        [Obsolete("Please use: content.Instance.XmlContent")]
        public static XmlDocument xmlContent
        {
            get { return Instance.XmlContent; }
        }

        public virtual bool isInitializing
        {
            get { return _xmlContent == null; }
        }

        /// <summary>
        /// Internal reference to XmlContent
        /// </summary>
        protected virtual XmlDocument XmlContentInternal
        {
            get
            {
                if (isInitializing)
                {
                    lock (_xmlContentInternalSyncLock)
                    {
                        if (isInitializing)
                        {
                            _xmlContent = LoadContent();
                            if (!UmbracoSettings.isXmlContentCacheDisabled && !IsValidDiskCachePresent())
                                SaveContentToDiskAsync(_xmlContent);
                        }
                    }
                }

                return _xmlContent;
            }
            set
            {
                lock (_xmlContentInternalSyncLock)
                {
                    // Clear macro cache
                    Cache.ClearCacheObjectTypes("umbraco.MacroCacheContent");
                    requestHandler.ClearProcessedRequests();
                    _xmlContent = value;

                    if (!UmbracoSettings.isXmlContentCacheDisabled && UmbracoSettings.continouslyUpdateXmlDiskCache)
                        SaveContentToDiskAsync(_xmlContent);
                    else
                        // Clear cache...
                        ClearDiskCacheAsync();
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load content from database in a background thread
        /// Replaces active content when done.
        /// </summary>
        public virtual void RefreshContentFromDatabaseAsync()
        {
            ThreadPool.QueueUserWorkItem(
                delegate
                    {
                        XmlDocument xmlDoc = LoadContentFromDatabase();
                        XmlContentInternal = xmlDoc;

                        if (!UmbracoSettings.isXmlContentCacheDisabled)
                            SaveContentToDisk(xmlDoc);
                    });
        }

        public virtual void PublishNodeAsync(int documentId)
        {
            ThreadPool.QueueUserWorkItem(
                delegate { PublishNode(documentId); });
        }

        private void TransferValuesFromDocumentXmlToPublishedXml(XmlNode DocumentNode, XmlNode PublishedNode)
        {
            // Remove all attributes and data nodes from the published node
            PublishedNode.Attributes.RemoveAll();
            foreach (XmlNode n in PublishedNode.SelectNodes("./data"))
                PublishedNode.RemoveChild(n);

            // Append all attributes and datanodes from the documentnode to the publishednode
            foreach (XmlAttribute att in DocumentNode.Attributes)
                ((XmlElement) PublishedNode).SetAttribute(att.Name, att.Value);

            foreach (XmlElement el in DocumentNode.SelectNodes("./data"))
            {
                XmlNode newDatael = PublishedNode.OwnerDocument.ImportNode(el, true);
                PublishedNode.AppendChild(newDatael);
            }
        }

        /// <summary>
        /// Used by all overloaded publish methods to do the actual "noderepresentation to xml"
        /// </summary>
        /// <param name="d"></param>
        /// <param name="xmlContentCopy"></param>
        private void PublishNodeDo(Document d, XmlDocument xmlContentCopy)
        {
            // Find the document in the xml cache
            XmlNode x = xmlContentCopy.GetElementById(d.Id.ToString());

            // Find the parent (used for sortering and maybe creation of new node)
            XmlNode parentNode;
            if (d.Level == 1)
                parentNode = xmlContentCopy.DocumentElement;
            else
                parentNode = xmlContentCopy.GetElementById(d.Parent.Id.ToString());

            if (parentNode != null)
            {
                if (x == null)
                {
                    x = d.ToXml(xmlContentCopy, false);
                    parentNode.AppendChild(x);
                }
                else
                    TransferValuesFromDocumentXmlToPublishedXml(d.ToXml(xmlContentCopy, false), x);

                XmlNodeList childNodes = parentNode.SelectNodes("./node");

                // Maybe sort the nodes if the added node has a lower sortorder than the last
                if (childNodes.Count > 1 &&
                    int.Parse(childNodes[childNodes.Count - 1].Attributes.GetNamedItem("sortOrder").Value) >
                    int.Parse(x.Attributes.GetNamedItem("sortOrder").Value))
                {
                    SortNodes(ref parentNode);
                }
            }
        }

        public static void SortNodes(ref XmlNode parentNode)
        {
            XmlNode n = parentNode.CloneNode(true);

            // remove all children from original node
            foreach (XmlNode child in parentNode.SelectNodes("./node"))
                parentNode.RemoveChild(child);


            XPathNavigator nav = n.CreateNavigator();
            XPathExpression expr = nav.Compile("./node");
            expr.AddSort("@sortOrder", XmlSortOrder.Ascending, XmlCaseOrder.None, "", XmlDataType.Number);
            XPathNodeIterator iterator = nav.Select(expr);
            while (iterator.MoveNext())
                parentNode.AppendChild(
                    ((IHasXmlNode) iterator.Current).GetNode());
        }


        public virtual void PublishNode(Document d)
        {
            // We need to lock content cache here, because we cannot allow other threads
            // making changes at the same time, they need to be queued
            // Adding log entry before locking the xmlfile

            lock (_xmlContentInternalSyncLock)
            {
                // Make copy of memory content, we cannot make changes to the same document
                // the is read from elsewhere
                XmlDocument xmlContentCopy = CloneXmlDoc(XmlContentInternal);

                PublishNodeDo(d, xmlContentCopy);

                XmlContentInternal = xmlContentCopy;
                ClearContextCache();
            }
            Action.RunActionHandlers(d, new ActionPublish());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Documents"></param>
        public virtual void PublishNode(List<Document> Documents)
        {
            // We need to lock content cache here, because we cannot allow other threads
            // making changes at the same time, they need to be queued
            int parentid = Documents[0].Id;

            lock (_xmlContentInternalSyncLock)
            {
                // Make copy of memory content, we cannot make changes to the same document
                // the is read from elsewhere
                XmlDocument xmlContentCopy = CloneXmlDoc(XmlContentInternal);
                foreach (Document d in Documents)
                {
                    PublishNodeDo(d, xmlContentCopy);
                }
                XmlContentInternal = xmlContentCopy;
                ClearContextCache();
            }

            foreach (Document d in Documents)
            {
                Action.RunActionHandlers(d, new ActionPublish());
            }
        }

        /// <summary>
        /// Legacy method - you should use the overloaded publishnode(document d) method whenever possible
        /// </summary>
        /// <param name="documentId"></param>
        public virtual void PublishNode(int documentId)
        {
            // Get the document
            Document d = new Document(documentId);
            PublishNode(d);
        }

        public virtual void UnPublishNodeAsync(int documentId)
        {
            ThreadPool.QueueUserWorkItem(
                delegate { UnPublishNode(documentId); });
        }

        public virtual void UnPublishNode(int documentId)
        {
            // Get the document
            Document d = new Document(documentId);
            XmlNode x;

            // remove from xml db cache 
            d.XmlRemoveFromDB();

            // Check if node present, before cloning
            x = XmlContentInternal.GetElementById(d.Id.ToString());
            if (x == null)
                return;

            // We need to lock content cache here, because we cannot allow other threads
            // making changes at the same time, they need to be queued
            lock (_xmlContentInternalSyncLock)
            {
                // Make copy of memory content, we cannot make changes to the same document
                // the is read from elsewhere
                XmlDocument xmlContentCopy = CloneXmlDoc(XmlContentInternal);

                // Find the document in the xml cache
                x = xmlContentCopy.GetElementById(d.Id.ToString());
                if (x != null)
                {
                    // The document already exists in cache, so repopulate it
                    x.ParentNode.RemoveChild(x);
                    XmlContentInternal = xmlContentCopy;
                    ClearContextCache();
                }
            }

            if (x != null)
            {
                // Run Handler				
                Action.RunActionHandlers(d, new ActionPublish());
            }
        }

        #endregion

        #region Protected & Private methods

        /// <summary>
        /// Invalidates the disk content cache file. Effectively just deletes it.
        /// </summary>
        private void ClearDiskCache()
        {
            lock (_readerWriterSyncLock)
            {
                if (File.Exists(UmbracoXmlDiskCacheFileName))
                {
                    // Reset file attributes, to make sure we can delete file
                    File.SetAttributes(UmbracoXmlDiskCacheFileName, FileAttributes.Normal);
                    File.Delete(UmbracoXmlDiskCacheFileName);
                }
            }
        }

        /// <summary>
        /// Clear HTTPContext cache if any
        /// </summary>
        private void ClearContextCache()
        {
            // If running in a context very important to reset context cache orelse new nodes are missing
            if (HttpContext.Current != null && HttpContext.Current.Items.Contains(XmlContextContentItemKey))
                HttpContext.Current.Items.Remove(XmlContextContentItemKey);
        }

        /// <summary>
        /// Invalidates the disk content cache file. Effectively just deletes it.
        /// </summary>
        private void ClearDiskCacheAsync()
        {
            // Queue file deletion
            // We queue this function, because there can be a write process running at the same time
            // and we don't want this method to block web request
            ThreadPool.QueueUserWorkItem(
                delegate { ClearDiskCache(); });
        }

        /// <summary>
        /// Load content from either disk or database
        /// </summary>
        /// <returns></returns>
        private XmlDocument LoadContent()
        {
            if (!UmbracoSettings.isXmlContentCacheDisabled && IsValidDiskCachePresent())
            {
                try
                {
                    return LoadContentFromDiskCache();
                }
                catch (Exception e)
                {
                    // This is really bad, loading from cache file failed for some reason, now fallback to loading from database
                    Debug.WriteLine("Content file cache load failed: " + e);
                    ClearDiskCache();
                }
            }
            return LoadContentFromDatabase();
        }

        private bool IsValidDiskCachePresent()
        {
            return File.Exists(UmbracoXmlDiskCacheFileName);
        }

        /// <summary>
        /// Load content from cache file
        /// </summary>
        private XmlDocument LoadContentFromDiskCache()
        {
            lock (_readerWriterSyncLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                Log.Add(LogTypes.System, User.GetUser(0), -1, "Loading content from disk cache...");
                xmlDoc.Load(UmbracoXmlDiskCacheFileName);
                return xmlDoc;
            }
        }

        private void InitContentDocumentBase(XmlDocument xmlDoc)
        {
            // Create id -1 attribute
            xmlDoc.LoadXml(@"<!DOCTYPE umbraco [ " +
                           "<!ELEMENT nodes ANY>  " +
                           "<!ELEMENT node ANY>  " +
                           "<!ATTLIST node id ID #REQUIRED> ]>" +
                           "<root id=\"-1\"/>");
        }

        /// <summary>
        /// Load content from database
        /// </summary>
        private XmlDocument LoadContentFromDatabase()
        {
            XmlDocument xmlDoc = new XmlDocument();
            InitContentDocumentBase(xmlDoc);

            Log.Add(LogTypes.System, User.GetUser(0), -1, "Loading content from database...");

            Hashtable nodes = new Hashtable();
            Hashtable parents = new Hashtable();
            try
            {
                Log.Add(LogTypes.Debug, User.GetUser(0), -1, "Republishing starting");

                // Esben Carlsen: At some point we really need to put all data access into to a tier of its own.
                string sql =
                    @"select umbracoNode.id, umbracoNode.parentId, umbracoNode.sortOrder, cmsContentXml.xml from umbracoNode 
inner join cmsContentXml on cmsContentXml.nodeId = umbracoNode.id and umbracoNode.nodeObjectType = 'C66BA18E-EAF3-4CFF-8A22-41B16D66A972'
order by umbracoNode.level, umbracoNode.sortOrder";

                lock (_dbReadSyncLock)
                {
                    using (SqlDataReader dr = SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, sql))
                    {
                        while (dr.Read())
                        {
                            int currentId = int.Parse(dr["id"].ToString());
                            int parentId = int.Parse(dr["parentId"].ToString());

                            xmlDoc.LoadXml((string) dr["xml"]);
                            nodes.Add(currentId, xmlDoc.FirstChild);

                            if (parents.ContainsKey(parentId))
                                ((ArrayList) parents[parentId]).Add(currentId);
                            else
                            {
                                ArrayList a = new ArrayList();
                                a.Add(currentId);
                                parents.Add(parentId, a);
                            }
                        }
                    }
                }

                Log.Add(LogTypes.Debug, User.GetUser(0), -1, "Xml Pages loaded");

                // Reset
                InitContentDocumentBase(xmlDoc);

                try
                {
                    GenerateXmlDocument(parents, nodes, -1, xmlDoc.DocumentElement);
                }
                catch (Exception ee)
                {
                    Log.Add(LogTypes.Error, User.GetUser(0), -1,
                            string.Format("Error while generating XmlDocument from database: {0}", ee));
                }
            }
            catch (OutOfMemoryException)
            {
                Log.Add(LogTypes.Error, User.GetUser(0), -1,
                        string.Format("Error Republishin: Out Of Memory. Parents: {0}, Nodes: {1}",
                                      parents.Count, nodes.Count));
            }
            catch (Exception ee)
            {
                Log.Add(LogTypes.Error, User.GetUser(0), -1, string.Format("Error Republishing: {0}", ee));
            }
            finally
            {
                Log.Add(LogTypes.Debug, User.GetUser(0), -1, "Done republishing Xml Index");
            }

            return xmlDoc;
        }

        private void GenerateXmlDocument(Hashtable parents, Hashtable nodes, int parentId, XmlNode parentNode)
        {
            if (parents.ContainsKey(parentId))
            {
                ArrayList children = (ArrayList) parents[parentId];
                foreach (int i in children)
                {
                    XmlNode childNode = (XmlNode) nodes[i];
                    parentNode.AppendChild(childNode);
                    GenerateXmlDocument(parents, nodes, i, childNode);
                }
            }
        }

        /// <summary>
        /// Persist a XmlDocument to the Disk Cache
        /// </summary>
        /// <param name="xmlDoc"></param>
        internal void SaveContentToDisk(XmlDocument xmlDoc)
        {
            lock (_readerWriterSyncLock)
            {
                try
                {
                    Stopwatch stopWatch = Stopwatch.StartNew();

                    ClearDiskCache();
                    xmlDoc.Save(UmbracoXmlDiskCacheFileName);

                    Log.Add(LogTypes.Debug, User.GetUser(0), -1, string.Format("Xml saved in {0}", stopWatch.Elapsed));
                }
                catch (Exception ee)
                {
                    // If for whatever reason something goes wrong here, invalidate disk cache
                    ClearDiskCache();
                    Log.Add(LogTypes.Error, User.GetUser(0), -1, string.Format("Xml wasn't saved: {0}", ee));
                }
            }
        }

        /// <summary>
        /// Persist xml document to disk cache in a background thread
        /// </summary>
        /// <param name="xmlDoc"></param>
        private void SaveContentToDiskAsync(XmlDocument xmlDoc)
        {
            // Save copy of content
            XmlDocument xmlContentCopy = CloneXmlDoc(xmlDoc);

            ThreadPool.QueueUserWorkItem(
                delegate { SaveContentToDisk(xmlContentCopy); });
        }

        /// <summary>
        /// Make a copy of a XmlDocument
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        private XmlDocument CloneXmlDoc(XmlDocument xmlDoc)
        {
            // Save copy of content
            XmlDocument xmlCopy = new XmlDocument();
            xmlCopy.LoadXml(xmlDoc.OuterXml);
            return xmlCopy;
        }

        #endregion
    }
}