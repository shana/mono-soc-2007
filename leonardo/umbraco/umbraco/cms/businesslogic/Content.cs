using System;
using System.Collections.Generic;

using Microsoft.ApplicationBlocks.Data;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using Umbraco.Cms.BusinessLogic.property;
using Umbraco.Cms.BusinessLogic.propertytype;

namespace Umbraco.Cms.BusinessLogic
{
	/// <summary>
	/// Content is an intermediate layer between CMSNode and class'es which will use generic data.
	/// 
	/// Content is a datastructure that holds generic data defined in its corresponding ContentType. Content can in some
	/// sence be compared to a row in a database table, it's contenttype hold a definition of the columns and the Content
	/// contains the data
	/// 
	/// Note that Content data in Umbraco is *not* tablular but in a treestructure.
	/// 
	/// </summary>
	public class Content : CMSNode
	{
		private Guid _version;
		private DateTime _versionDate;
		private XmlNode _xml;
		private bool _versionDateInitialized;
        private string _contentTypeIcon;
        private ContentType _contentType;
        private bool _propertiesInitialized = false;
        private Properties _properties = new Properties();

        /// <summary>
        /// 
        /// </summary>
        /// <param Name="id"></param>
        public Content(int id) : base(id) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param Name="id"></param>
        /// <param Name="noSetup"></param>
        protected Content(int id, bool noSetup) : base(id, noSetup) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param Name="id"></param>
        protected Content(Guid id) : base(id) { }

        protected void InitializeContent(int InitContentType, Guid InitVersion, DateTime InitVersionDate, string InitContentTypeIcon)
        {
            _contentType = ContentType.GetContentType(InitContentType);
            _version = InitVersion;
            _versionDate = InitVersionDate;
            _contentTypeIcon = InitContentTypeIcon;
        }

	    /// <summary>
        /// The current Content objects ContentType, which defines the Properties of the Content (data)
        /// </summary>
        public ContentType ContentType
        {
            get
            {
				if (_contentType == null)
				{
					object o = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
						"Select ContentType from cmsContent where nodeId=@nodeid",
						new SqlParameter("@nodeid", this.Id));
					if (o == null)
						return null;
					int contentTypeId;
					if(!int.TryParse(o.ToString(), out contentTypeId))
						return null;
					try
					{
						_contentType = new ContentType(contentTypeId);
					}
					catch
					{
						return null;
					}
				}
            	return _contentType;
            }
            set
            {
                _contentType = value;
            }
        }

	    public Properties GenericProperties
	    {
            get
            {
                if (!_propertiesInitialized)
                    InitializeProperties();
                    
                return _properties;
            }
	    }

        /// <summary>
        /// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
        /// </summary>
        public override void Save()
        {
            base.Save();
        }


		// This is for performance only (used in tree)
		/// <summary>
		/// The icon used in the tree - placed in this layer for performance reasons.
		/// </summary>
		public string ContentTypeIcon 
		{
			get 
			{
				if (_contentTypeIcon == null && this.ContentType != null)
					_contentTypeIcon = this.ContentType.IconUrl;
				return _contentTypeIcon;
			}
			set 
			{
				_contentTypeIcon = value;
			}
		}

		/// <summary>
		/// Retrieve a Property given the alias
		/// </summary>
		/// <param Name="alias">Propertyalias (defined in the documenttype)</param>
		/// <returns>The property with the given alias</returns>
		public Property getProperty(string alias) 
		{
			ContentType ct = this.ContentType;
			if(ct == null)
				return null;
			propertytype.PropertyType pt = ct.getPropertyType(alias);
			if(pt == null)
				return null;
			return getProperty(pt);
		}

		/// <summary>
		/// Retrieve a property given the propertytype
		/// </summary>
		/// <param Name="pt">PropertyType</param>
		/// <returns>The property with the given propertytype</returns>
		public Property getProperty(propertytype.PropertyType pt)
		{
			object o = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
				"select id from cmsPropertyData where versionId=@version and propertyTypeId=@propertyTypeId",
				new SqlParameter("@version", this.Version),
				new SqlParameter("@propertyTypeId", pt.Id));
			if(o == null)
				return null;
			int propertyId;
			if(!int.TryParse(o.ToString(), out propertyId))
				return null;
			try
			{
				return new Property(propertyId, pt);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// The createtimestamp on this version
		/// </summary>
		public DateTime VersionDate
		{
			get
			{
				if(!_versionDateInitialized)
				{
					object o = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
						"select VersionDate from cmsContentVersion where versionId = '" + this.Version.ToString() + "'");
					if(o == null)
					{
						_versionDate = DateTime.Now;
					}
					else
					{
						_versionDateInitialized = DateTime.TryParse(o.ToString(), out _versionDate);
					}
				}
				return _versionDate;
			}
			set
			{
				_versionDate = value;
				_versionDateInitialized = true;
			}
		}


		/// <summary>
		/// Optimized method for bulk deletion of properties´on a Content object.
		/// </summary>
		private void deleteAllProperties() 
		{
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "Delete from cmsPropertyData where contentNodeId = @nodeId", new SqlParameter("@nodeId", this.Id));
		}

		/// <summary>
		/// Retrieve a list of generic properties of the content
		/// </summary>
		public Property[] getProperties 
		{
			get
			{
				if (this.ContentType == null)
					return new Property[0];
				PropertyType[] props = this.ContentType.PropertyTypes;

				List<Property> result = new List<Property>();
				foreach(PropertyType prop in props)
				{
					if (prop == null)
						continue;
					Property p = getProperty(prop);
					if(p == null)
						continue;
					result.Add(p);
				}

				return result.ToArray();
			}
		}


        private void InitializeProperties()
        {
			using(SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString, CommandType.Text,
				string.Format("select id from cmsPropertyData where versionId = '{0}'", Version)))
			{
				while(dr.Read())
					_properties.Add(new Property(int.Parse(dr["id"].ToString())));
			}
        }


		/// <summary>
		/// Retrive a list of Content sharing the ContentType
		/// </summary>
		/// <param Name="ct">The ContentType</param>
		/// <returns>A list of Content objects sharing the ContentType defined.</returns>
		public static Content[] getContentOfContentType(ContentType ct) {
			SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString, CommandType.Text, "Select nodeId from  cmsContent INNER JOIN umbracoNode ON cmsContent.nodeId = umbracoNode.id where ContentType = " + ct.Id + " ORDER BY umbracoNode.text ");
			System.Collections.ArrayList tmp = new System.Collections.ArrayList();

			while (dr.Read())  tmp.Add(dr["nodeId"]);
			dr.Close();
			
			Content[] retval = new Content[tmp.Count];
			for (int i = 0;i < tmp.Count; i++)  retval[i] = new Content((int) tmp[i]);

			return retval;
		}

		/// <summary>
		/// Add a property to the Content
		/// </summary>
		/// <param Name="pt">The PropertyType of the Property</param>
		/// <param Name="VersionId">The version of the document on which the property should be add'ed</param>
		/// <returns>The new Property</returns>
		public property.Property addProperty(propertytype.PropertyType pt, Guid VersionId) 	{
			return property.Property.MakeNew(pt, this, VersionId);
		}

		/// <summary>
		/// Content is under version control, you are able to programatically create new versions
		/// </summary>
		public Guid Version {
			get{				
				if (_version == Guid.Empty)
				{
					string sql = "Select top 1 VersionId from cmsContentVersion where contentID = " + this.Id +
					             " order by id desc ";
					
					object result = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text, sql);

					if (result == null)
						_version = Guid.Empty;
					else
						_version = new Guid(result.ToString());
				}	
				return _version;
			}
			set{_version = value;}
		}

		/// <summary>
		/// Creates a new Content object from the ContentType.
		/// </summary>
		/// <param Name="ct"></param>
		protected void CreateContent(ContentType ct) {
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,"insert into cmsContent (nodeId,ContentType) values ("+this.Id+","+ct.Id+")");
			createNewVersion();
		}

		/// <summary>
		/// Indication if the Content exists in at least one version.
		/// </summary>
		/// <returns>Returns true if the Content has a version</returns>
		private bool hasVersion() 
		{
			int versionCount = int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text, "select Count(Id) as tmp from cmsContentVersion where contentId = " + this.Id.ToString()).ToString());
			return (versionCount > 0);
		}

		/// <summary>
		/// Method for creating a new version of the data associated to the Content.
		/// 
		/// </summary>
		/// <returns>The new version Id</returns>
		protected Guid createNewVersion() {
			Guid newVersion = Guid.NewGuid();
			bool tempHasVersion = hasVersion();
			foreach (propertytype.PropertyType pt in this.ContentType.PropertyTypes) 
			{
				object oldValue = "";
				if (tempHasVersion) 
				{
					try 
					{
						oldValue = this.getProperty(pt.Alias).Value;
					} 
					catch {}
				}
				property.Property p = this.addProperty(pt, newVersion);
				if (oldValue != null && oldValue.ToString() != "") p.Value = oldValue;
			}
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"Insert into cmsContentVersion (ContentId,VersionId) values ("+ this.Id +",'"+newVersion+"')");
			this.Version = newVersion;
			return newVersion;
		}

		/// <summary>
		/// Deletes the current Content object, must be overridden in the child class.
		/// </summary>
		protected new void delete() {

			// Delete all data associated with this content
			this.deleteAllProperties();

			// Delete version history
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,"Delete from cmsContentVersion where ContentId = " + this.Id);
			
			// Delete Contentspecific data ()
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,"Delete from cmsContent where NodeId = " + this.Id);

			// Delete xml
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "delete from cmsContentXml where nodeID = @nodeId", new SqlParameter("@nodeId", this.Id));

			// Delete Nodeinformation!!
			base.delete();
		}

		/// <summary>
		/// Initialize a contentobject given a version.
		/// </summary>
		/// <param Name="version">The version identifier</param>
		/// <returns>The Content object from the given version</returns>
		public static Content GetContentFromVersion(Guid version) {
			int tmpContentId = int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString,CommandType.Text,"Select ContentId from cmsContentVersion where VersionId = '" + version.ToString() + "'").ToString());
			return new Content(tmpContentId);
		}


		private XmlNode importXml() 
		{
			XmlDocument xmlDoc = new XmlDocument();
			SqlConnection xmlConn = new SqlConnection(GlobalSettings.DbDSN);
			xmlConn.Open();
			xmlDoc.Load(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteXmlReader(xmlConn, CommandType.Text, "select xml from cmsContentXml where nodeID = " + this.Id.ToString()));
			xmlConn.Close();

			return xmlDoc.FirstChild;
		}

		/// <summary>
		/// An Xmlrepresentation of a Content object.
		/// </summary>
		/// <param Name="xd">Xmldocument context</param>
		/// <param Name="Deep">If true, the Contents children are appended to the Xmlnode recursive</param>
		/// <returns>The Xmlrepresentation of the data on the Content object</returns>
		public new virtual XmlNode ToXml(XmlDocument xd, bool Deep) 
		{
			if (_xml == null) 
			{
				XmlDocument xmlDoc = new XmlDocument();
				SqlConnection xmlConn = new SqlConnection(GlobalSettings.DbDSN);
				xmlConn.Open();
				xmlDoc.Load(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteXmlReader(xmlConn, CommandType.Text, "select xml from cmsContentXml where nodeID = " + this.Id.ToString()));
				xmlConn.Close();

				_xml = xmlDoc.FirstChild;

				// Generate xml if xml still null (then it hasn't been initialized before)
				if (_xml == null) 
				{
					this.XmlGenerate(new XmlDocument());
					_xml = importXml();
				}

			}

			XmlNode x = xd.ImportNode(_xml, true);

			if (Deep) 
			{
				foreach(BusinessLogic.console.IconI c in this.Children)
					try 
					{
						x.AppendChild(new Content(c.Id).ToXml(xd, true));
					} 
					catch (Exception mExp)
					{
						System.Web.HttpContext.Current.Trace.Warn("Content", "Error adding node to xml: " + mExp.ToString());
					}
			}

			return x;

		}

		/// <summary>
		/// Removes the Xml cached in the database - unpublish and cleaning
		/// </summary>
		public virtual void XmlRemoveFromDB() 
		{
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "delete from cmsContentXml where nodeId = @nodeId", new SqlParameter("@nodeId", this.Id));
		}

		/// <summary>
		/// Generates the Content XmlNode
		/// </summary>
		/// <param Name="xd"></param>
		public virtual void XmlGenerate(XmlDocument xd) 
		{
			XmlNode x = xd.CreateNode(XmlNodeType.Element, "node", "");
			XmlPopulate(xd, ref x, false);

			// Save to db
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, @"
if exists(select nodeId from cmsContentXml where nodeId = @nodeId)
	update cmsContentXml set xml = @xml where nodeId = @nodeId
else
	insert into cmsContentXml(nodeId, xml) values (@nodeId, @xml)
", new SqlParameter("@nodeId", this.Id), new SqlParameter("@xml", x.OuterXml));
		}

		public virtual void XmlPopulate(XmlDocument xd, ref XmlNode x, bool Deep) 
		{
			foreach (property.Property p in this.getProperties)
				if (p != null)
					x.AppendChild(p.ToXml(xd));

			// attributes
			x.Attributes.Append(XmlHelper.AddAttribute(xd, "id", this.Id.ToString()));
			x.Attributes.Append(XmlHelper.AddAttribute(xd, "version", this.Version.ToString()));
			if (this.Level > 1)
				x.Attributes.Append(XmlHelper.AddAttribute(xd, "parentID", this.Parent.Id.ToString()));
			else
				x.Attributes.Append(XmlHelper.AddAttribute(xd, "parentID", "-1"));
			x.Attributes.Append(XmlHelper.AddAttribute(xd, "level", this.Level.ToString()));
			x.Attributes.Append(XmlHelper.AddAttribute(xd, "writerID", this.User.Id.ToString()));
			if (this.ContentType != null)
				x.Attributes.Append(XmlHelper.AddAttribute(xd, "nodeType", this.ContentType.Id.ToString()));
			x.Attributes.Append(XmlHelper.AddAttribute(	xd, "template", "0"));
			x.Attributes.Append(XmlHelper.AddAttribute(xd, "sortOrder", this.sortOrder.ToString()));
			x.Attributes.Append(XmlHelper.AddAttribute(xd, "createDate", this.CreateDateTime.ToString("s")));
			x.Attributes.Append(XmlHelper.AddAttribute(xd, "updateDate", this.VersionDate.ToString("s")));
			x.Attributes.Append(XmlHelper.AddAttribute(xd, "nodeName", this.Text));
			if (this.Text != null)
				x.Attributes.Append(XmlHelper.AddAttribute(xd, "urlName", this.Text.Replace(" ", "").ToLower()));
			x.Attributes.Append(XmlHelper.AddAttribute(xd, "writerName", this.User.Name));
			if (this.ContentType != null)
				x.Attributes.Append(XmlHelper.AddAttribute(xd, "nodeTypeAlias", this.ContentType.Alias));
			x.Attributes.Append(XmlHelper.AddAttribute(xd, "path", this.Path));

			if (Deep) 
			{
				foreach(Content c in this.Children)
					x.AppendChild(c.ToXml(xd, true));
			}
		}
	}

	/// <summary>
	/// Not implemented
	/// </summary>
	public interface ISaveHandlerContents
	{
		/// <summary>
		/// Not implemented
		/// </summary>
		bool Execute(Cms.BusinessLogic.Content contentObject);
	}
}