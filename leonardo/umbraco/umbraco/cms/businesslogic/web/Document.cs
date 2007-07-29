using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Web;
using System.Xml;
using Microsoft.ApplicationBlocks.Data;
using Umbraco.BusinessLogic;
using Umbraco.BusinessLogic.Actions;
using Umbraco.BusinessLogic.Console;
using Umbraco.Cms.BusinessLogic.index;
using Umbraco.Cms.BusinessLogic.property;
using Umbraco.Cms.BusinessLogic.relation;
using Umbraco.Cms.helpers;
using SqlHelper=Umbraco.SqlHelper;

namespace Umbraco.Cms.BusinessLogic.web
{
    /// <summary>
    /// Document represents a webpage,
    /// type (Cms.Umbraco.Cms.BusinessLogic.web.DocumentType)
    /// 
    /// Pubished Documents are exposed to the runtime/the public website in a cached xml document.
    /// </summary>
    public class Document : Content
    {
        public static Guid _objectType = new Guid("c66ba18e-eaf3-4cff-8a22-41b16d66a972");
        private DateTime _updated;
        private DateTime _release;
        private DateTime _expire;
        private int _template;
        private string _text;
        private bool _published;
        private XmlNode _xml;
        private User _creator;
        private User _writer;

        // special for passing httpcontext object
        private HttpContext _httpContext;

        // special for tree performance
        private int _userId = -1;

        /// <summary>
        /// The id of the user whom created the document
        /// </summary>
        public int UserId
        {
            get
            {
                if (_userId == -1)
                    _userId = Umbraco.BusinessLogic.User.Id;

                return _userId;
            }
        }

        /// <summary>
        /// The current HTTPContext
        /// </summary>
        public HttpContext HttpContext
        {
            set { _httpContext = value; }
            get
            {
                if (_httpContext == null)
                    _httpContext = HttpContext.Current;
                return _httpContext;
            }
        }

        /// <summary>
        /// Publishing a document
        /// 
        /// A xmlrepresentation of the document and its data are exposed to the runtime data
        /// (an xmlrepresentation is added -or updated if the document previously are published) ,
        /// this will lead to a new version of the document being created, for continuing editing of
        /// the data.
        /// </summary>
        /// <param Name="u">The usercontext under which the action are performed</param>
        public void Publish(User u)
        {
            _published = true;
            string tempVersion = System.Version.ToString();
            Guid newVersion = createNewVersion();

            Log.Add(LogTypes.Publish, u, Id, "");

            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                      "insert into cmsDocument (newest, nodeId, published, documentUser, versionId, Text, TemplateId) values (1," +
                                      Id + ", 0, " + u.Id + ", '" + newVersion + "', N'" + Umbraco.SqlHelper.SafeString(Text) +
                                      "', " + _template + ")");
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                      "update cmsDocument set published = 0 where nodeId = " + Id +
                                      " update cmsDocument set published = 1, newest = 0 where versionId = '" +
                                      tempVersion + "'");

            // update release and expire dates
            Document newDoc = new Document(Id, newVersion);
            if (ReleaseDate != new DateTime())
                newDoc.ReleaseDate = ReleaseDate;
            if (ExpireDate != new DateTime())
                newDoc.ExpireDate = ExpireDate;

            // Update xml in db using the new document (has correct version date)
            newDoc.XmlGenerate(new XmlDocument());
        }

        /// <summary>
        /// Rollbacks a document to a previous version, this will create a new version of the document and copy
        /// all of the old documents data.
        /// </summary>
        /// <param Name="u">The usercontext under which the action are performed</param>
        /// <param Name="VersionId">The unique Id of the version to roll back to</param>
        public void RollBack(Guid VersionId, User u)
        {
            Guid newVersion = createNewVersion();
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                      "insert into cmsDocument (nodeId, published, documentUser, versionId, Text, TemplateId) values (" +
                                      Id +
                                      ", 0, " + u.Id + ", '" + newVersion + "', N'" + Umbraco.SqlHelper.SafeString(Text) + "', " +
                                      _template + ")");

            // Get new version
            Document dNew = new Document(Id, newVersion);

            // Old version
            Document dOld = new Document(Id, VersionId);

            // Revert title
            dNew.Text = dOld.Text;

            // Revert all properties
            foreach (Property p in dOld.getProperties)
                try
                {
                    dNew.getProperty(p.PropertyType).Value = p.Value;
                }
                catch
                {
                    // property doesn't exists
                }
        }

        /// <summary>
        /// Recursive publishing.
        /// 
        /// Envoking this method will publish the documents and all children recursive.
        /// </summary>
        /// <param Name="u">The usercontext under which the action are performed</param>
        public void PublishWithSubs(User u)
        {
            _published = true;
            string tempVersion = System.Version.ToString();
            Guid newVersion = createNewVersion();
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                      "insert into cmsDocument (nodeId, published, documentUser, versionId, Text) values (" +
                                      Id + ", 0, " + u.Id +
                                      ", '" + newVersion + "', N'" + Umbraco.SqlHelper.SafeString(Text) + "')");
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                      "update cmsDocument set published = 0 where nodeId = " + Id +
                                      " update cmsDocument set published = 1 where versionId = '" + tempVersion + "'");

            // Update xml in db
            XmlGenerate(new XmlDocument());

            foreach (Document dc in Children)
                dc.PublishWithSubs(u);
        }

        /// <summary>
        /// Published flag is on if the document are published
        /// </summary>
        public bool Published
        {
            get { return _published; }

            set
            {
                // Esben Carlsen: ????? value never used ?? --> needs update
                _published = false;
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                          string.Format("update cmsDocument set published = 0 where nodeId = {0}", Id));
            }
        }

        public void UnPublish()
        {
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                      string.Format("update cmsDocument set published = 0 where nodeId = {0}", Id));
        }

        /// <summary>
        /// Constructs a new document
        /// </summary>
        /// <param Name="id">Id of the document</param>
        /// <param Name="noSetup">N/A</param>
        public Document(Guid id, bool noSetup) : base(id)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Document class.
        /// You can set an optional flag noSetup, used for optimizing for loading nodes in the tree, 
        /// therefor only data needed by the tree is initialized.
        /// </summary>
        /// <param Name="id">Id of the document</param>
        /// <param Name="noSetup">If flag are on the </param>
        public Document(int id, bool noSetup) : base(id, noSetup)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Document class to a specific version, used for rolling back data from a previous version
        /// of the document.
        /// </summary>
        /// <param Name="id">The id of the document</param>
        /// <param Name="Version">The version of the document</param>
        public Document(int id, Guid Version) : base(id)
        {
            this.Version = Version;
            setupDocument();
        }

        /// <summary>
        /// Initializes a new instance of the Document class.
        /// </summary>
        /// <param Name="id">The id of the document</param>
        public Document(int id) : base(id)
        {
            setupDocument();
        }

        /// <summary>
        /// Initialize the document
        /// </summary>
        /// <param Name="id">The id of the document</param>
        public Document(Guid id) : base(id)
        {
            setupDocument();
        }

        public Document(bool OptimizedMode, int id) : base(id, true)
        {
            using (SqlDataReader dr =
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text,
                                        @"
Select 
	top 1 
	(select count(id) from umbracoNode where parentId = @id) as Children, 
	(select Count(published) as tmp from cmsDocument where published = 1 And nodeId = @id) as Published,
	cmsContentVersion.VersionId,
    cmsContentVersion.versionDate,
	cmsContent.ContentType, 
	contentTypeNode.uniqueId as ContentTypeGuid, 
	cmsContentType.alias,
	cmsContentType.icon,
	published, documentUser, isnull(templateId, cmsDocumentType.templateNodeId) as templateId, cmsDocument.text as DocumentText, releaseDate, expireDate, updateDate, 
	UmbracoNode.createDate, UmbracoNode.trashed, UmbracoNode.parentId, UmbracoNode.nodeObjectType, UmbracoNode.nodeUser, UmbracoNode.level, UmbracoNode.path, UmbracoNode.sortOrder, UmbracoNode.uniqueId, umbracoNode.text 
from 
	UmbracoNode 
inner join
	cmsContentVersion on cmsContentVersion.contentID = umbracoNode.id
inner join 
	cmsDocument on cmsDocument.versionId = cmsContentVersion.versionId
inner join
	cmsContent on cmsDocument.nodeId = cmsContent.NodeId
inner join
	cmsContentType on cmsContentType.nodeId = cmsContent.ContentType
inner join 
	umbracoNode contentTypeNode on contentTypeNode.id = cmsContentType.nodeId
left join cmsDocumentType on 
	cmsDocumentType.contentTypeNodeId = cmsContent.contentType and cmsDocumentType.IsDefault = 1 
where 
	umbracoNode.id = @id
order by
	cmsContentVersion.id desc
",
                                        new SqlParameter("@id", id)))
            {
                if (dr.Read())
                {
                    // Initialize node and basic document properties
                    bool _hc = false;
                    if (int.Parse(dr["children"].ToString()) > 0)
                        _hc = true;
                    SetupDocumentForTree(new Guid(dr["uniqueId"].ToString()), int.Parse((dr["level"].ToString())),
                                         int.Parse(dr["parentId"].ToString()), int.Parse(dr["documentUser"].ToString()),
                                         bool.Parse(dr["published"].ToString()),
                                         dr["path"].ToString(), dr["text"].ToString(),
                                         DateTime.Parse(dr["createDate"].ToString()),
                                         DateTime.Parse(dr["updateDate"].ToString()),
                                         DateTime.Parse(dr["versionDate"].ToString()), dr["icon"].ToString(), _hc);

                    // initialize content object
                    InitializeContent(int.Parse(dr["ContentType"].ToString()), new Guid(dr["versionId"].ToString()),
                                      DateTime.Parse(dr["versionDate"].ToString()), dr["icon"].ToString());

                    // initialize final document properties
                    DateTime tmpReleaseDate = new DateTime();
                    DateTime tmpExpireDate = new DateTime();
                    if (!dr.IsDBNull(dr.GetOrdinal("releaseDate")))
                        tmpReleaseDate = DateTime.Parse(dr["releaseDate"].ToString());
                    if (!dr.IsDBNull(dr.GetOrdinal("expireDate")))
                        tmpExpireDate = DateTime.Parse(dr["expireDate"].ToString());

                    InitializeDocument(
                        new User(int.Parse(dr["nodeUser"].ToString()), true),
                        new User(int.Parse(dr["documentUser"].ToString()), true),
                        dr["documentText"].ToString(),
                        int.Parse(dr["templateId"].ToString()),
                        tmpReleaseDate,
                        tmpExpireDate,
                        DateTime.Parse(dr["updateDate"].ToString()),
                        bool.Parse(dr["published"].ToString())
                        );
                }
            }
        }

        /// <summary>
        /// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
        /// </summary>
        public override void Save()
        {
            base.Save();
        }

        private void setupDocument()
        {
            SqlDataReader dr =
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString, CommandType.Text,
                                        "select published, documentUser, isnull(templateId, cmsDocumentType.templateNodeId) as templateId, text, releaseDate, expireDate, updateDate from cmsDocument inner join cmsContent on cmsDocument.nodeId = cmsContent.Nodeid left join cmsDocumentType on cmsDocumentType.contentTypeNodeId = cmsContent.contentType and cmsDocumentType.IsDefault = 1 where versionId = '" +
                                        System.Version + "'");
            if (dr.Read())
            {
                _creator = Umbraco.BusinessLogic.User;
                _writer = Umbraco.BusinessLogic.User.GetUser(dr.GetInt32(dr.GetOrdinal("documentUser")));

                _text = dr.GetString(dr.GetOrdinal("text"));
                if (!dr.IsDBNull(dr.GetOrdinal("templateId")))
                    _template = dr.GetInt32(dr.GetOrdinal("templateId"));
                if (!dr.IsDBNull(dr.GetOrdinal("releaseDate")))
                    _release = dr.GetDateTime(dr.GetOrdinal("releaseDate"));
                if (!dr.IsDBNull(dr.GetOrdinal("expireDate")))
                    _expire = dr.GetDateTime(dr.GetOrdinal("expireDate"));
                if (!dr.IsDBNull(dr.GetOrdinal("updateDate")))
                    _updated = dr.GetDateTime(dr.GetOrdinal("updateDate"));
            }
            dr.Close();
            _published =
                (int.Parse(
                     Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
                                             "select Count(published) as tmp from cmsDocument where published = 1 And nodeId = " +
                                             Id).ToString()) > 0);
        }

        protected void InitializeDocument(User InitUser, User InitWriter, string InitText, int InitTemplate,
                                          DateTime InitReleaseDate, DateTime InitExpireDate, DateTime InitUpdateDate,
                                          bool InitPublished)
        {
            _creator = InitUser;
            _writer = InitWriter;
            _text = InitText;
            _template = InitTemplate;
            _release = InitReleaseDate;
            _expire = InitExpireDate;
            _updated = InitUpdateDate;
            _published = InitPublished;
        }

        /// <summary>
        /// The Name of the document, amongst other used in the nice url.
        /// </summary>
        public new string Text
        {
            get
            {
                if (_text == null || _text == "")
                    _text =
                        Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
                                                string.Format("select text from umbracoNode where id = {0}", Id)).
                            ToString();
                return _text;
            }
            set
            {
                _text = value;
                base.Text = value;
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                          "update cmsDocument set text = @value where versionId = '" + System.Version + "'",
                                          new SqlParameter("@value", _text));
                CMSNode c = new CMSNode(Id);
                c.Text = _text;
            }
        }

        /// <summary>
        /// The date of the last update of the document
        /// </summary>
        public DateTime UpdateDate
        {
            get { return _updated; }
            set
            {
                _updated = value;
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                          "update cmsDocument set updateDate = @value where versionId = '" + System.Version +
                                          "'",
                                          new SqlParameter("@value", new SqlDateTime(value)));
            }
        }

        /// <summary>
        /// A datestamp which indicates when a document should be published, used in automated publish/unpublish scenarios
        /// </summary>
        public DateTime ReleaseDate
        {
            get { return _release; }
            set
            {
                _release = value;

                if (_release.Year != 1 || _release.Month != 1 || _release.Day != 1)
                    Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                              "update cmsDocument set releaseDate = @value where versionId = '" +
                                              System.Version + "'",
                                              new SqlParameter("@value", new SqlDateTime(value)));
                else
                    Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                              "update cmsDocument set releaseDate = NULL where versionId = '" + System.Version +
                                              "'");
            }
        }

        /// <summary>
        /// A datestamp which indicates when a document should be unpublished, used in automated publish/unpublish scenarios
        /// </summary>
        public DateTime ExpireDate
        {
            get { return _expire; }
            set
            {
                _expire = value;

                if (_expire.Year != 1 || _expire.Month != 1 || _expire.Day != 1)
                    Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                              "update cmsDocument set expireDate = @value where versionId = '" + System.Version +
                                              "'",
                                              new SqlParameter("@value", new SqlDateTime(value)));
                else
                    Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                              "update cmsDocument set expireDate = NULL where versionId = '" + System.Version +
                                              "'");
            }
        }

        /// <summary>
        /// The id of the template associated to the document
        /// 
        /// When a document is created, it will get have default template given by it's documenttype,
        /// an editor is able to assign alternative templates (allowed by it's the documenttype)
        /// 
        /// You are always able to override the template in the runtime by appending the following to the querystring to the Url:
        /// 
        /// ?altTemplate=[templatealias]
        /// </summary>
        public int Template
        {
            get { return _template; }
            set
            {
                _template = value;
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                          "update cmsDocument set templateId = @value where versionId = '" + System.Version +
                                          "'",
                                          new SqlParameter("@value", _template));
            }
        }

        /// <summary>
        /// Used for rolling back documents to a previous version
        /// </summary>
        /// <returns> Previous published versions of the document</returns>
        public DocumentVersionList[] GetVersions()
        {
            ArrayList versions = new ArrayList();
            using (SqlDataReader dr =
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text,
                                        "select documentUser, versionId, updateDate, text from cmsDocument where nodeId = @nodeId order by updateDate",
                                        new SqlParameter("@nodeId", Id)))
            {
                while (dr.Read())
                {
                    DocumentVersionList dv =
                        new DocumentVersionList(new Guid(dr["versionId"].ToString()),
                                                DateTime.Parse(dr["updateDate"].ToString()),
                                                dr["text"].ToString(),
                                                Umbraco.BusinessLogic.User.GetUser(int.Parse(dr["documentUser"].ToString())));
                    versions.Add(dv);
                }
            }

            DocumentVersionList[] retVal = new DocumentVersionList[versions.Count];
            int i = 0;
            foreach (DocumentVersionList dv in versions)
            {
                retVal[i] = dv;
                i++;
            }
            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a breadcrumlike path for the document like: /ancestorname/ancestorname</returns>
        public string GetTextPath()
        {
            string tempPath = "";
            string[] splitPath = Path.Split(".".ToCharArray());
            for (int i = 1; i < Level; i++)
            {
                tempPath += new Document(int.Parse(splitPath[i])).Text + "/";
            }
            if (tempPath.Length > 0)
                tempPath = tempPath.Substring(0, tempPath.Length - 1);
            return tempPath;
        }

        /// <summary>
        /// Creates a new document of the same type and copies all data from the current onto it
        /// </summary>
        /// <param Name="CopyTo">The parentid where the document should be copied to</param>
        /// <param Name="u">The usercontext under which the action are performed</param>
        public void Copy(int CopyTo, User u)
        {
            Copy(CopyTo, u, false);
        }

        public void Copy(int CopyTo, User u, bool RelateToOrignal)
        {
            // Make the new document
            Document NewDoc = MakeNew(Text, new DocumentType(ContentType.Id), u, CopyTo);

            // update template
            NewDoc.Template = Template;

            // Copy the properties of the current document
            foreach (Property p in getProperties)
                NewDoc.getProperty(p.PropertyType.Alias).Value = p.Value;

            // Relate?
            if (RelateToOrignal)
            {
                Relation.MakeNew(Id, NewDoc.Id, RelationType.GetByAlias("relateDocumentOnCopy"), "");

                // Add to audit trail
                Log.Add(LogTypes.Copy, u, NewDoc.Id, "Copied and related from " + Text + " (id: " + Id.ToString() + ")");
            }


            // Copy the children
            foreach (Document c in Children)
                c.Copy(NewDoc.Id, u, RelateToOrignal);
        }

        /// <summary>
        /// Creates a new document
        /// </summary>
        /// <param Name="Name">The Name (.Text property) of the document</param>
        /// <param Name="dct">The documenttype</param>
        /// <param Name="u">The usercontext under which the action are performed</param>
        /// <param Name="ParentId">The id of the parent to the document</param>
        /// <returns>The newly created document</returns>
        public static Document MakeNew(string Name, DocumentType dct, User u, int ParentId)
        {
            Guid newId = Guid.NewGuid();
            // Updated to match level from base node
            CMSNode n = new CMSNode(ParentId);
            int newLevel = n.Level;
            newLevel++;
            MakeNew(ParentId, _objectType, u.Id, newLevel, Name, newId);
            Document tmp = new Document(newId, true);
            tmp.CreateContent(dct);
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                      "insert into cmsDocument (newest, nodeId, published, documentUser, versionId, Text) values (1, " +
                                      tmp.Id + ", 0, " +
                                      u.Id + ", '" + tmp.Version + "', N'" + SqlHelper.SafeString(tmp.Text) + "')");

            // Update the sortOrder if the parent was the root!
            if (ParentId == -1)
            {
                CMSNode c = new CMSNode(newId);
                c.sortOrder = GetRootDocuments().Length + 1;
            }

            Document d = new Document(newId);

            // Log
            Log.Add(LogTypes.New, u, d.Id, "");

            // Run Handler				
            Action.RunActionHandlers(d, new ActionNew());

            // Index
            try
            {
                Indexer.IndexNode(_objectType, d.Id, d.Text, d.User.Name, d.CreateDateTime, null, true);
            }
            catch (Exception ee)
            {
                Log.Add(LogTypes.Error, d.User, d.Id,
                        string.Format("Error indexing document: {0}", ee));
            }

            return d;
        }

        /// <summary>
        /// Used to get the firstlevel/root documents of the hierachy
        /// </summary>
        /// <returns>Root documents</returns>
        public static Document[] GetRootDocuments()
        {
            Guid[] topNodeIds = TopMostNodeIds(_objectType);

            Document[] retval = new Document[topNodeIds.Length];
            for (int i = 0; i < topNodeIds.Length; i++)
            {
                Document d = new Document(topNodeIds[i]);
                retval[i] = d;
            }
            return retval;
        }

        /// <summary>
        /// A collection of documents imidiately underneath this document ie. the childdocuments
        /// </summary>
        public new Document[] Children
        {
            get
            {
                IIcon[] tmp = base.Children;
                Document[] retval = new Document[tmp.Length];
                for (int i = 0; i < tmp.Length; i++) retval[i] = new Document(tmp[i].Id);
                return retval;
            }
        }

        /// <summary>
        /// Deletes the current document (and all children recursive)
        /// </summary>
        public new void delete()
        {
            Log.Add(LogTypes.Debug, Umbraco.BusinessLogic.User.GetUser(0), Id,
                    string.Format("Path: {0} (contains: {1})", Path, Path.Contains(",-20,")));

            // Check for recyle bin
            if (!Path.Contains(",-20,"))
            {
                Action.RunActionHandlers(this, new ActionDelete());
                Log.Add(LogTypes.Debug, Id, "Before indexer remove");
                UnPublish();
                Move(-20);
            }
            else
            {
                Log.Add(LogTypes.Debug, Id, "Before indexer remove");
                try
                {
                    Indexer.RemoveNode(Id);
                }
                catch
                {
                }
                Log.Add(LogTypes.Debug, Id, "After indexer remove");

                foreach (Document d in Children)
                {
                    d.delete();
                }

                Action.RunActionHandlers(this, new ActionDelete());
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text, "delete from cmsDocument where NodeId = " + Id);
                HttpContext.Current.Trace.Write("documentdelete", "base delete");
                base.delete();
                HttpContext.Current.Trace.Write("documentdelete", "after base delete");
            }
        }

        /// <summary>
        /// Deletes all documents of a type, will be invoked if a documenttype is deleted.
        /// 
        /// Note: use with care: this method can result in wast amount of data being deleted.
        /// </summary>
        /// <param Name="dt">The type of which documents should be deleted</param>
        public static void DeleteFromType(DocumentType dt)
        {
            foreach (Content c in getContentOfContentType(dt))
            {
                // due to recursive structure document might already been deleted..
                if (IsNode(c.UniqueId))
                {
                    Document tmp = new Document(c.UniqueId);
                    tmp.delete();
                }
            }
        }

        /// <summary>
        /// Indexes the documents data for internal search
        /// </summary>
        /// <param Name="Optimze">If on the indexer will optimize</param>
        public void Index(bool Optimze)
        {
            try
            {
                Hashtable fields = new Hashtable();
                foreach (Property p in getProperties)
                    fields.Add(p.PropertyType.Alias, p.Value.ToString());
                Indexer.IndexDirectory = HttpContext.Server.MapPath(Indexer.RelativeIndexDir);
                Indexer.IndexNode(_objectType, Id, Text, Umbraco.BusinessLogic.User.Name, CreateDateTime, fields, Optimze);
            }
            catch (Exception ee)
            {
                Log.Add(LogTypes.Error, Umbraco.BusinessLogic.User, Id,
                        string.Format("Error indexing node: {0}", ee));
            }
        }

        /// <summary>
        /// Refreshes the xml, used when publishing data on a document which already is published
        /// </summary>
        /// <param Name="xd">The source xmldocument</param>
        /// <param Name="x">The previous xmlrepresentation of the document</param>
        public void XmlNodeRefresh(XmlDocument xd, ref XmlNode x)
        {
            x.Attributes.RemoveAll();
            foreach (XmlNode xDel in x.SelectNodes("./data"))
                x.RemoveChild(xDel);

            XmlPopulate(xd, ref x, false);
        }

        /// <summary>
        /// Creates an xmlrepresentation of the documet and saves it to the database
        /// </summary>
        /// <param Name="xd"></param>
        public new void XmlGenerate(XmlDocument xd)
        {
            XmlNode x = xd.CreateNode(XmlNodeType.Element, "node", "");
            XmlPopulate(xd, ref x, false);

            // Save to db
            saveXml(x);
        }

        private void saveXml(XmlNode x)
        {
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
                                      @"
if exists(select nodeId from cmsContentXml where nodeId = @nodeId)
	update cmsContentXml set xml = @xml where nodeId = @nodeId
else
	insert into cmsContentXml(nodeId, xml) values (@nodeId, @xml)
",
                                      new SqlParameter("@nodeId", Id), new SqlParameter("@xml", x.OuterXml));
        }

        private XmlNode importXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            using (SqlConnection xmlConn = new SqlConnection(GlobalSettings.DbDSN))
            {
                xmlConn.Open();
                xmlDoc.Load(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteXmlReader(xmlConn, CommandType.Text,
                                                       string.Format(
                                                           "select xml from cmsContentXml where nodeID = {0}", Id)));
            }

            return xmlDoc.FirstChild;
        }

        /// <summary>
        /// A xmlrepresentaion of the document, used when publishing/exporting the document, 
        /// 
        /// Optional: Recursive get childdocuments xmlrepresentation
        /// </summary>
        /// <param Name="xd">The xmldocument</param>
        /// <param Name="Deep">Recursive add of childdocuments</param>
        /// <returns></returns>
        public new virtual XmlNode ToXml(XmlDocument xd, bool Deep)
        {
            if (Published)
            {
                if (_xml == null)
                {
                    // Load xml from db if _xml hasn't been loaded yet
                    _xml = importXml();

                    // Generate xml if xml still null (then it hasn't been initialized before)
                    if (_xml == null)
                    {
                        XmlGenerate(new XmlDocument());
                        _xml = importXml();
                    }
                }

                XmlNode x = xd.ImportNode(_xml, true);

                if (Deep)
                {
                    foreach (Document d in Children)
                        if (d.Published)
                            x.AppendChild(d.ToXml(xd, true));
                }

                return x;
            }
            else
                return null;
        }

        /// <summary>
        /// Populate a documents xmlnode
        /// </summary>
        /// <param Name="xd">Xmldocument context</param>
        /// <param Name="x">The node to fill with data</param>
        /// <param Name="Deep">If true the documents childrens xmlrepresentation will be appended to the Xmlnode recursive</param>
        public override void XmlPopulate(XmlDocument xd, ref XmlNode x, bool Deep)
        {
            foreach (Property p in getProperties)
                if (p != null)
                    x.AppendChild(p.ToXml(xd));

            // attributes
            x.Attributes.Append(addAttribute(xd, "id", Id.ToString()));
            x.Attributes.Append(addAttribute(xd, "version", System.Version.ToString()));
            if (Level > 1)
                x.Attributes.Append(addAttribute(xd, "parentID", Parent.Id.ToString()));
            else
                x.Attributes.Append(addAttribute(xd, "parentID", "-1"));
            x.Attributes.Append(addAttribute(xd, "level", Level.ToString()));
            x.Attributes.Append(addAttribute(xd, "writerID", _writer.Id.ToString()));
            x.Attributes.Append(addAttribute(xd, "creatorID", _creator.Id.ToString()));
			if (ContentType != null)
				x.Attributes.Append(addAttribute(xd, "nodeType", ContentType.Id.ToString()));
            x.Attributes.Append(addAttribute(xd, "template", _template.ToString()));
            x.Attributes.Append(addAttribute(xd, "sortOrder", sortOrder.ToString()));
            x.Attributes.Append(addAttribute(xd, "createDate", CreateDateTime.ToString("s")));
            x.Attributes.Append(addAttribute(xd, "updateDate", VersionDate.ToString("s")));
            x.Attributes.Append(addAttribute(xd, "nodeName", Text));
            x.Attributes.Append(addAttribute(xd, "urlName", url.FormatUrl(Text.ToLower())));
            x.Attributes.Append(addAttribute(xd, "writerName", _writer.Name));
            x.Attributes.Append(addAttribute(xd, "creatorName", _creator.Name.ToString()));
			if (ContentType != null)
				x.Attributes.Append(addAttribute(xd, "nodeTypeAlias", ContentType.Alias));
            x.Attributes.Append(addAttribute(xd, "path", Path));

            if (Deep)
            {
                foreach (Document d in Children)
                    x.AppendChild(d.ToXml(xd, true));
            }
        }

        public void refreshXmlSortOrder()
        {
            if (Published)
            {
                if (_xml == null)
                    // Load xml from db if _xml hasn't been loaded yet
                    _xml = importXml();

                // Generate xml if xml still null (then it hasn't been initialized before)
                if (_xml == null)
                {
                    XmlGenerate(new XmlDocument());
                    _xml = importXml();
                }
                else
                {
                    // Update the sort order attr
                    _xml.Attributes.GetNamedItem("sortOrder").Value = sortOrder.ToString();
                    saveXml(_xml);
                }

            }

        }

        private XmlAttribute addAttribute(XmlDocument Xd, string Name, string Value)
        {
            XmlAttribute temp = Xd.CreateAttribute(Name);
            temp.Value = Value;
            return temp;
        }

        /// <summary>
        /// Performance tuned method for use in the tree
        /// </summary>
        /// <param Name="NodeId">The parentdocuments id</param>
        /// <returns></returns>
        public static Document[] GetChildrenForTree(int NodeId)
        {
            ArrayList tmp = new ArrayList();
            using (SqlDataReader dr =
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text,
                                        @"
create table #temp (contentId int, versionDate datetime)      
insert into #temp
select contentId, max(versionDate) 
from cmsContentVersion 
inner join umbracoNode on umbracoNode.id = cmsContentVersion.contentId and umbracoNode.parentId = @parentId
group by contentId

select 
	count(children.id) as children, umbracoNode.id, umbracoNode.uniqueId, umbracoNode.level, umbracoNode.parentId, cmsDocument.documentUser, umbracoNode.path, umbracoNode.sortOrder, isnull(publishCheck.published,0) as published, umbracoNode.createDate, cmsDocument.text, cmsDocument.updateDate, cmsContentVersion.versionDate, cmsContentType.icon
from umbracoNode 
left join umbracoNode children on children.parentId = umbracoNode.id
inner join cmsContent on cmsContent.nodeId = umbracoNode.id
inner join cmsContentType on cmsContentType.nodeId = cmsContent.contentType
inner join #temp on #temp.contentId = cmsContent.nodeId
inner join cmsContentVersion on cmsContentVersion.contentId = #temp.contentId and cmsContentVersion.versionDate = #temp.versionDate
inner join cmsDocument on cmsDocument.versionId = cmsContentversion.versionId
left join cmsDocument publishCheck on publishCheck.nodeId = cmsContent.nodeID and publishCheck.published = 1
where umbracoNode.parentID = @parentId 
group by umbracoNode.id, umbracoNode.uniqueId, umbracoNode.level, umbracoNode.parentId, cmsDocument.documentUser, umbracoNode.path, umbracoNode.sortOrder, isnull(publishCheck.published,0), umbracoNode.createDate, cmsDocument.text, cmsDocument.updateDate, cmsContentVersion.versionDate, cmsContentType.icon 
order by umbracoNode.sortOrder

drop table #temp


",
                                        new SqlParameter("@parentId", NodeId)))
            {
                while (dr.Read())
                {
                    Document d = new Document(int.Parse(dr["id"].ToString()), true);
                    bool _hc = false;
                    if (int.Parse(dr["children"].ToString()) > 0)
                        _hc = true;
                    d.SetupDocumentForTree(new Guid(dr["uniqueId"].ToString()), int.Parse((dr["level"].ToString())),
                                           int.Parse(dr["parentId"].ToString()),
                                           int.Parse(dr["documentUser"].ToString()),
                                           bool.Parse(dr["published"].ToString()),
                                           dr["path"].ToString(), dr["text"].ToString(),
                                           DateTime.Parse(dr["createDate"].ToString()),
                                           DateTime.Parse(dr["updateDate"].ToString()),
                                           DateTime.Parse(dr["versionDate"].ToString()), dr["icon"].ToString(), _hc);
                    tmp.Add(d);
                }
            }

            Document[] retval = new Document[tmp.Count];

            for (int i = 0; i < tmp.Count; i ++)
                retval[i] = (Document) tmp[i];

            return retval;
        }

        private void SetupDocumentForTree(Guid uniqueId, int level, int parentId, int user, bool publish, string path,
                                          string text, DateTime createDate, DateTime updateDate,
                                          DateTime versionDate, string icon, bool hasChildren)
        {
            SetupNodeForTree(uniqueId, _objectType, level, parentId, user, path, text, createDate, hasChildren);

            _published = publish;
            _updated = updateDate;
            ContentTypeIcon = icon;
            VersionDate = versionDate;
        }

        public static void RePublishAll()
        {
            XmlDocument xd = new XmlDocument();
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "truncate table cmsContentXml");
            SqlDataReader dr =
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text,
                                        "select nodeId from cmsDocument where published = 1");
            while (dr.Read())
            {
                try
                {
                    new Document(int.Parse(dr["nodeId"].ToString())).XmlGenerate(xd);
                }
                catch (Exception ee)
                {
                    Log.Add(LogTypes.Error, Umbraco.BusinessLogic.User.GetUser(0), int.Parse(dr["nodeId"].ToString()),
                            string.Format("Error generating xml: {0}", ee));
                }
            }
            dr.Close();
        }

        /// <summary>
        /// Retrieve a list of documents with an expirationdate greater than today
        /// </summary>
        /// <returns>A list of documents with expirationdates than today</returns>
        public static Document[] GetDocumentsForExpiration()
        {
            ArrayList docs = new ArrayList();
            SqlDataReader dr =
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text,
                                        "select distinct nodeId from cmsDocument where newest = 1 and not expireDate is null and expireDate <= getdate()");
            while (dr.Read())
                docs.Add(dr.GetInt32(dr.GetOrdinal("nodeId")));
            dr.Close();

            Document[] retval = new Document[docs.Count];
            for (int i = 0; i < docs.Count; i++) retval[i] = new Document((int) docs[i]);
            return retval;
        }

        /// <summary>
        /// Retrieve a list of documents with with releasedate greater than today
        /// </summary>
        /// <returns>Retrieve a list of documents with with releasedate greater than today</returns>
        public static Document[] GetDocumentsForRelease()
        {
            ArrayList docs = new ArrayList();
            SqlDataReader dr =
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text,
                                        "select distinct nodeId, level, sortOrder from cmsDocument inner join umbracoNode on umbracoNode.id = cmsDocument.nodeId where newest = 1 and not releaseDate is null and releaseDate <= getdate() order by [level], sortOrder");
            while (dr.Read())
                docs.Add(dr.GetInt32(dr.GetOrdinal("nodeId")));
            dr.Close();

            Document[] retval = new Document[docs.Count];
            for (int i = 0; i < docs.Count; i++) retval[i] = new Document((int) docs[i]);
            return retval;
        }

        /// <summary>
        /// Imports (create) a document from a xmlrepresentation of a document, used by the packager
        /// </summary>
        /// <param Name="ParentId">The id to import to</param>
        /// <param Name="Creator">Creator f the new document</param>
        /// <param Name="Source">Xmlsource</param>
        public static void Import(int ParentId, User Creator, XmlElement Source)
        {
            Document d = MakeNew(
                Source.GetAttribute("nodeName"),
                DocumentType.GetByAlias(Source.GetAttribute("nodeTypeAlias")),
                Creator,
                ParentId);
            d.CreateDateTime = DateTime.Parse(Source.GetAttribute("createDate"));

            // Properties
            foreach (XmlElement n in Source.SelectNodes("data"))
            {
                d.getProperty(n.GetAttribute("alias")).Value = XmlHelper.GetNodeValue(n);
            }

            // Subpages
            foreach (XmlElement n in Source.SelectNodes("node"))
                Import(d.Id, Creator, n);
        }
    }

    /// <summary>
    /// A lightweight datastructure used to represent a version of a document
    /// </summary>
    public class DocumentVersionList
    {
        private Guid _version;
        private DateTime _date;
        private string _text;
        private User _user;

        /// <summary>
        /// The unique id of the version
        /// </summary>
        public Guid Version
        {
            get { return _version; }
        }

        /// <summary>
        /// The date of the creation of the version 
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
        }

        /// <summary>
        /// The Name of the document in the version
        /// </summary>
        public string Text
        {
            get { return _text; }
        }

        /// <summary>
        /// The user which created the version
        /// </summary>
        public User User
        {
            get { return _user; }
        }

        /// <summary>
        /// Initializes a new instance of the DocumentVersionList class.
        /// </summary>
        /// <param Name="Version">Unique version id</param>
        /// <param Name="Date">Version createdate</param>
        /// <param Name="Text">Version Name</param>
        /// <param Name="User">Creator</param>
        public DocumentVersionList(Guid Version, DateTime Date, string Text, User User)
        {
            _version = Version;
            _date = Date;
            _text = Text;
            _user = User;
        }
    }
}