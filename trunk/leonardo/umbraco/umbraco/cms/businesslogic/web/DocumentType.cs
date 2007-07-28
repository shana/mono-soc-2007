using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using Microsoft.ApplicationBlocks.Data;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.propertytype;

namespace umbraco.cms.businesslogic.web
{
    /// <summary>
    /// Summary description for DocumentType.
    /// </summary>
    public class DocumentType : ContentType
    {
        private static Guid _objectType = new Guid("a2cb7800-f571-4787-9638-bc48539a0efb");
        private ArrayList _templateIds = new ArrayList();
        private int _defaultTemplate;

        public DocumentType(int id) : base(id)
        {
            setupDocumentType();
        }

        public DocumentType(Guid id) : base(id)
        {
            setupDocumentType();
        }

        public DocumentType(int id, bool UseOptimizedMode) : base(id, UseOptimizedMode)
        {
            // Only called if analyze hasn't happend yet
            AnalyzeContentTypes(_objectType, false);

            // Check if this document type can run in optimized mode
            if (IsOptimized())
            {
                OptimizedMode = true;

                // Run optimized sql query here 
            }
            else
            {
                base.setupNode();
                base.setupContentType();
                OptimizedMode = false;
            }
        }

        /// <summary>
        /// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
        /// </summary>
        public override void Save()
        {
            base.Save();
        }

        public new static DocumentType GetByAlias(string Alias)
        {
            try
            {
                return
                    new DocumentType(
                        int.Parse(
                            SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
                                                    "select nodeid from cmsContentType where alias = '" +
                                                    sqlHelper.safeString(Alias) + "'").ToString()));
            }
            catch
            {
                return null;
            }
        }

        private void setupDocumentType()
        {
            if (
                int.Parse(
                    SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
                                            "select count(TemplateNodeId) as tmp from cmsDocumentType where contentTypeNodeId =" +
                                            Id).ToString()) > 0)
            {
                SqlDataReader dr =
                    SqlHelper.ExecuteReader(_ConnString, CommandType.Text,
                                            "Select templateNodeId, IsDefault from cmsDocumentType where contentTypeNodeId =" +
                                            Id);
                while (dr.Read())
                {
                    if (template.Template.IsNode(int.Parse(dr["templateNodeId"].ToString())))
                    {
                        _templateIds.Add(dr["templateNodeId"]);
                        if (dr.GetBoolean(dr.GetOrdinal("IsDefault")))
                            _defaultTemplate = int.Parse(dr["templateNodeId"].ToString());
                    }
                }
                dr.Close();
            }
        }

        public static DocumentType MakeNew(User u, string Text)
        {
            int ParentId = -1;
            int level = 1;
            Guid uniqueId = Guid.NewGuid();
            CMSNode n = MakeNew(ParentId, _objectType, u.Id, level, Text, uniqueId);

            Create(n.Id, Text, "");
            return new DocumentType(n.Id);
        }

        public new static DocumentType[] GetAll
        {
            get
            {
                Guid[] Ids = getAllUniquesFromObjectType(_objectType);
                SortedList initRetVal = new SortedList();
                for (int i = 0; i < Ids.Length; i++)
                {
                    DocumentType dt = new DocumentType(Ids[i]);
                    initRetVal.Add(dt.Text + dt.UniqueId, dt);
                }

                DocumentType[] retVal = new DocumentType[Ids.Length];

                IDictionaryEnumerator ide = initRetVal.GetEnumerator();

                int count = 0;
                while (ide.MoveNext())
                {
                    retVal[count] = (DocumentType) ide.Value;
                    count++;
                }

                return retVal;
            }
        }

        public bool HasTemplate()
        {
            return (_templateIds.Count > 0);
        }

        public int DefaultTemplate
        {
            get { return _defaultTemplate; }
            set
            {
                RemoveDefaultTemplate();
                _defaultTemplate = value;
                if (_defaultTemplate != 0)
                    SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
                                              "update cmsDocumentType set IsDefault = 1 where contentTypeNodeId = " +
                                              Id.ToString() + " and TemplateNodeId = " + value.ToString());
            }
        }

        public void RemoveDefaultTemplate()
        {
            _defaultTemplate = 0;
            SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
                                      "update cmsDocumentType set IsDefault = 0 where contentTypeNodeId = " +
                                      Id.ToString());
        }

        public template.Template[] allowedTemplates
        {
            get
            {
                if (HasTemplate())
                {
                    template.Template[] retval = new template.Template[_templateIds.Count];
                    for (int i = 0; i < _templateIds.Count; i++)
                    {
                        retval[i] = new template.Template((int) _templateIds[i]);
                    }
                    return retval;
                }
                return new template.Template[0];
            }
            set
            {
                clearTemplates();
                foreach (template.Template t in value)
                {
                    SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                              "Insert into cmsDocumentType (contentTypeNodeId, templateNodeId) values (" +
                                              Id + "," + t.Id + ")");
                    _templateIds.Add(t.Id);
                }
            }
        }

        public void clearTemplates()
        {
            SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                      "Delete from cmsDocumentType where contentTypeNodeId =" + Id);
            _templateIds.Clear();
        }

        public XmlElement ToXml(XmlDocument xd)
        {
            XmlElement doc = xd.CreateElement("DocumentType");

            // info section
            XmlElement info = xd.CreateElement("Info");
            doc.AppendChild(info);
            info.AppendChild(xmlHelper.addTextNode(xd, "Name", Text));
            info.AppendChild(xmlHelper.addTextNode(xd, "Alias", Alias));
            info.AppendChild(xmlHelper.addTextNode(xd, "Icon", IconUrl));
            info.AppendChild(xmlHelper.addTextNode(xd, "Thumbnail", Thumbnail));
            info.AppendChild(xmlHelper.addTextNode(xd, "Description", Description));

            // templates
            XmlElement allowed = xd.CreateElement("AllowedTemplates");
            foreach (template.Template t in allowedTemplates)
                allowed.AppendChild(xmlHelper.addTextNode(xd, "Template", t.Alias));
            info.AppendChild(allowed);
            if (DefaultTemplate != 0)
                info.AppendChild(
                    xmlHelper.addTextNode(xd, "DefaultTemplate", new template.Template(DefaultTemplate).Alias));
            else
                info.AppendChild(xmlHelper.addTextNode(xd, "DefaultTemplate", ""));

            // structure
            XmlElement structure = xd.CreateElement("Structure");
            doc.AppendChild(structure);

            foreach (int cc in AllowedChildContentTypeIDs)
                structure.AppendChild(xmlHelper.addTextNode(xd, "DocumentType", new DocumentType(cc).Alias));

            // generic properties
            XmlElement pts = xd.CreateElement("GenericProperties");
            foreach (PropertyType pt in PropertyTypes)
            {
                XmlElement ptx = xd.CreateElement("GenericProperty");
                ptx.AppendChild(xmlHelper.addTextNode(xd, "Name", pt.Name));
                ptx.AppendChild(xmlHelper.addTextNode(xd, "Alias", pt.Alias));
                ptx.AppendChild(xmlHelper.addTextNode(xd, "Type", pt.DataTypeDefinition.DataType.Id.ToString()));
                ptx.AppendChild(xmlHelper.addTextNode(xd, "Tab", Tab.GetCaptionById(pt.TabId)));
                ptx.AppendChild(xmlHelper.addTextNode(xd, "Mandatory", pt.Mandatory.ToString()));
                ptx.AppendChild(xmlHelper.addTextNode(xd, "Validation", pt.ValidationRegExp));
                ptx.AppendChild(xmlHelper.addCDataNode(xd, "Description", pt.Description));
                pts.AppendChild(ptx);
            }
            doc.AppendChild(pts);

            // tabs
            XmlElement tabs = xd.CreateElement("Tabs");
            foreach (TabI t in getVirtualTabs)
            {
                XmlElement tabx = xd.CreateElement("Tab");
                tabx.AppendChild(xmlHelper.addTextNode(xd, "Id", t.Id.ToString()));
                tabx.AppendChild(xmlHelper.addTextNode(xd, "Caption", t.Caption));
                tabs.AppendChild(tabx);
            }
            doc.AppendChild(tabs);
            return doc;
        }

        public new void delete()
        {
            // delete all documents of this type
            Document.DeleteFromType(this);
            // Delete contentType
            base.delete();
        }
    }
}