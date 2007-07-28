using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Web;
using Microsoft.ApplicationBlocks.Data;
using umbraco.cms.businesslogic.cache;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.language;
using umbraco.cms.businesslogic.propertytype;

namespace umbraco.cms.businesslogic
{
    /// <summary>
    /// ContentTypes defines the datafields of Content objects of that type, it's similar to defining columns 
    /// in a database table, where the PropertyTypes on the ContentType each responds to a Column, and the Content
    /// objects is similar to a row of data, where the Properties on the Content object corresponds to the PropertyTypes
    /// on the ContentType.
    /// 
    /// Besides data definition, the ContentType also defines the sorting and grouping (in tabs) of Properties/Datafields
    /// on the Content and which Content (by ContentType) can be created as child to the Content of the ContentType.
    /// </summary>
    public class ContentType : CMSNode
    {
        private bool _optimizedMode = false;
        private string _alias;
        private string _iconurl;
        private static Hashtable _analyzedContentTypes = new Hashtable();
        private static Hashtable _optimziedContentTypes = new Hashtable();

        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                          "update cmsContentType set description = @description where nodeId = @id",
                                          new SqlParameter("@description", value),
                                          new SqlParameter("@id", Id));

                FlushFromCache(Id);
            }
        }

        private string _thumbnail;

        public string Thumbnail
        {
            get { return _thumbnail; }
            set
            {
                _thumbnail = value;
                SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                          "update cmsContentType set thumbnail = @thumbnail where nodeId = @id",
                                          new SqlParameter("@thumbnail", value),
                                          new SqlParameter("@id", Id));

                FlushFromCache(Id);
            }
        }


        public bool OptimizedMode
        {
            get { return _optimizedMode; }
            set { _optimizedMode = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public ContentType(int id) : base(id)
        {
            setupContentType();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public ContentType(Guid id) : base(id)
        {
            setupContentType();
        }

        public ContentType(int id, bool UseOptimizedMode) : base(id, true)
        {
        }

        public string GetRawText()
        {
            return base.Text;
        }

        public override string Text
        {
            get
            {
                string tempText = base.Text;
                if (!tempText.StartsWith("#"))
                    return tempText;
                else
                {
                    Language lang =
                        Language.GetByCultureCode(Thread.CurrentThread.CurrentCulture.Name);
                    if (lang != null)
                    {
                        if (Dictionary.DictionaryItem.hasKey(tempText.Substring(1, tempText.Length - 1)))
                        {
                            Dictionary.DictionaryItem di =
                                new Dictionary.DictionaryItem(tempText.Substring(1, tempText.Length - 1));
                            return di.Value(lang.id);
                        }
                    }

                    return "[" + tempText + "]";
                }
            }
            set
            {
                base.Text = value;

                // Remove from cache
                FlushFromCache(Id);
            }
        }


        /// <summary>
        /// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
        /// </summary>
        public override void Save()
        {
            base.Save();

            // Remove from cache
            FlushFromCache(Id);
        }

        /// <summary>
        /// Initializes a ContentType object given the Alias.
        /// </summary>
        /// <param name="Alias">Alias of the contenttype</param>
        /// <returns>The ContentType with the corrosponding Alias</returns>
        public static ContentType GetByAlias(string Alias)
        {
            return new ContentType(int.Parse(SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
                                                                     "select nodeid from cmsContentType where alias = '" +
                                                                     sqlHelper.safeString(Alias) + "'").ToString()));
        }

        /// <summary>
        /// Set up the internal data of the ContentType
        /// </summary>
        protected void setupContentType()
        {
            using (SqlDataReader dr =
                SqlHelper.ExecuteReader(_ConnString, CommandType.Text,
                                        "Select Alias,icon,thumbnail,description from cmsContentType where nodeid=" + Id)
                )
            {
                if (!dr.Read())
                    throw new ArgumentException("No Contenttype with id: " + Id);
                _alias = dr["Alias"].ToString();
                _iconurl = dr["icon"].ToString();
                if (!dr.IsDBNull(dr.GetOrdinal("thumbnail")))
                    _thumbnail = dr["thumbnail"].ToString();
                if (!dr.IsDBNull(dr.GetOrdinal("description")))
                    _description = dr["description"].ToString();
            }
        }

        /// <summary>
        /// Retrieve a list of all ContentTypes
        /// </summary>
        /// <returns>The list of all ContentTypes</returns>
        public ContentType[] GetAll()
        {
            // Fetch contenttypes current objectType
            Guid[] Ids = getAllUniquesFromObjectType(base.nodeObjectType);
            SortedList initRetVal = new SortedList();
            for (int i = 0; i < Ids.Length; i++)
            {
                ContentType dt = new ContentType(Ids[i]);
                initRetVal.Add(dt.Text + dt.UniqueId, dt);
            }

            ContentType[] retVal = new ContentType[Ids.Length];

            IDictionaryEnumerator ide = initRetVal.GetEnumerator();

            int count = 0;
            while (ide.MoveNext())
            {
                retVal[count] = (ContentType) ide.Value;
                count++;
            }

            return retVal;
        }


        /// <summary>
        /// The "datafield/column" definitions, a Content object of this type will have an equivalent
        /// list of Properties.
        /// </summary>
		private static readonly object propertyTypesCacheSyncLock = new object();
        public PropertyType[] PropertyTypes
        {
            get
            {
				string cacheKey = "ContentType_PropertyTypes_Content:" + this.Id;
            	return Cache.GetCacheItem<PropertyType[]>(cacheKey, propertyTypesCacheSyncLock,
            		TimeSpan.FromMinutes(15),
            		delegate
            		{
            			List<PropertyType> result = new List<PropertyType>();
            			using(SqlDataReader dr =
            				SqlHelper.ExecuteReader(_ConnString, CommandType.Text,
            					"select id from cmsPropertyType where contentTypeId = @ctId order by sortOrder",
            					new SqlParameter("@ctId", Id)))
            			{
            				while(dr.Read())
            				{
								int id = (int)dr["id"];
								PropertyType pt = PropertyType.GetPropertyType(id);
								if(pt != null)
									result.Add(pt);
            				}
            			}
            			return result.ToArray();
            		});
            }
        }

        /// <summary>
        /// The Alias of the ContentType, is used for import/export and more human readable initialization see: GetByAlias 
        /// method.
        /// </summary>
        public string Alias
        {
            get { return _alias; }
            set
            {
                SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                          "update cmsContentType set alias = '" + value + "' where nodeId = " +
                                          Id.ToString());
                _alias = value;

                // Remove from cache
                FlushFromCache(Id);
            }
        }

        /// <summary>
        /// A Content object is often (always) represented in the treeview in the Umbraco console, the ContentType defines
        /// which Icon the Content of this type is representated with.
        /// </summary>
        public string IconUrl
        {
            get { return _iconurl; }
            set
            {
                _iconurl = value;
                SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                          "update cmsContentType set icon='" + value + "' where nodeid = " + Id);
                // Remove from cache
                FlushFromCache(Id);
            }
        }

        /// <summary>
        /// Adding a PropertyType to the ContentType, will add a new datafield/Property on all Documents of this Type.
        /// </summary>
        /// <param name="dt">The DataTypeDefinition of the PropertyType</param>
        /// <param name="Alias">The Alias of the PropertyType</param>
        /// <param name="Name">The userfriendly name</param>
        public void AddPropertyType(DataTypeDefinition dt, string Alias, string Name)
        {
            PropertyType pt = PropertyType.MakeNew(dt, this, Name, Alias);

            // Optimized call
            SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
                                      "insert into cmsPropertyData (contentNodeId, versionId, propertyTypeId) select contentId, versionId, @propertyTypeId from cmsContent inner join cmsContentVersion on cmsContent.nodeId = cmsContentVersion.contentId where contentType = @contentTypeId",
                                      new SqlParameter("@propertyTypeId", pt.Id),
                                      new SqlParameter("@contentTypeId", Id));

//			foreach (Content c in Content.getContentOfContentType(this)) 
//				c.addProperty(pt,c.Version);

            // Remove from cache
            FlushFromCache(Id);
        }

        /// <summary>
        /// Adding a PropertyType to a Tab, the Tabs are primarily used for making the 
        /// editing interface more userfriendly.
        /// 
        /// </summary>
        /// <param name="pt">The PropertyType</param>
        /// <param name="TabId">The Id of the Tab</param>
        public void SetTabOnPropertyType(PropertyType pt, int TabId)
        {
            SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                      "update cmsPropertyType set tabId = " + TabId.ToString() + " where id = " +
                                      pt.Id.ToString());

            // Remove from cache
            FlushFromCache(Id);
            foreach (TabI t in getVirtualTabs)
                FlushTabCache(t.Id);
        }

        /// <summary>
        /// Removing a PropertyType from the associated Tab
        /// </summary>
        /// <param name="pt">The PropertyType which should be freed from its tab</param>
        public void removePropertyTypeFromTab(PropertyType pt)
        {
            SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                      "update cmsPropertyType set tabId = NULL where id = " + pt.Id.ToString());

            // Remove from cache
            FlushFromCache(Id);
        }


        /// <summary>
        /// Creates a new Tab on the Content
        /// </summary>
        /// <param name="Caption">Returns the Id of the new Tab</param>
        /// <returns></returns>
        public int AddVirtualTab(string Caption)
        {
            // Remove from cache
            FlushFromCache(Id);

            return
                int.Parse(
                    SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
                                            "Insert into cmsTab (contenttypeNodeId,text,sortorder) values (" + Id + ",'" +
                                            Caption + "',1) select @@IDENTITY").ToString());
        }

        /// <summary>
        /// Helper method for getting the Tab id from a given PropertyType
        /// </summary>
        /// <param name="pt">The PropertyType from which to get the Tab Id</param>
        /// <returns>The Id of the Tab on which the PropertyType is placed</returns>
        public static int getTabIdFromPropertyType(PropertyType pt)
        {
            object tmp =
                SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
                                        "Select tabId from cmsPropertyType where id = " + pt.Id.ToString());
            if (tmp == DBNull.Value)
                return 0;
            else return int.Parse(tmp.ToString());
        }

        /// <summary>
        /// Releases all PropertyTypes on tab (this does not delete the PropertyTypes) and then Deletes the Tab
        /// </summary>
        /// <param name="id">The Id of the Tab to be deleted.</param>
        public void DeleteVirtualTab(int id)
        {
            SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                      "Update cmsPropertyType set tabId = NULL where tabId =" + id);
            SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text, "delete from cmsTab where id =" + id);

            // Remove from cache
            FlushFromCache(Id);
        }

        /// <summary>
        /// Retrieve a list of all Tabs on the current ContentType
        /// </summary>
        public TabI[] getVirtualTabs
        {
            get
            {
                List<TabI> tmp = new List<TabI>();
                using (SqlDataReader dr = SqlHelper.ExecuteReader(_ConnString, CommandType.Text,
                                                                  string.Format(
                                                                      "Select Id,text from cmsTab where contenttypeNodeId = {0} order by sortOrder",
                                                                      Id)))
                {
                    while (dr.Read())
                    {
                        tmp.Add(new Tab(int.Parse(dr["Id"].ToString()), dr["text"].ToString(), this));
                    }
                }
                return tmp.ToArray();
            }
        }

        /// <summary>
        /// Updates the caption of the Tab
        /// </summary>
        /// <param name="tabId">The Id of the Tab to be updated</param>
        /// <param name="Caption">The new Caption</param>
        public void SetTabName(int tabId, string Caption)
        {
            SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                      "Update  cmsTab set text = '" + Caption + "' where id = " + tabId);

            // Remove from cache
            FlushFromCache(Id);
        }

        /// <summary>
        /// An interface for the tabs, should be refactored
        /// </summary>
        public interface TabI
        {
            /// <summary>
            /// Public identifier
            /// </summary>
            int Id { get; }

            /// <summary>
            /// The text on the tab
            /// </summary>
            string Caption { get; }

            /// <summary>
            /// A list of all PropertyTypes on the Tab
            /// </summary>
            PropertyType[] PropertyTypes { get; }

            /// <summary>
            /// Method for moving the tab up
            /// </summary>
            void MoveUp();

            /// <summary>
            /// Method for retrieving the original, non processed name from the db
            /// </summary>
            /// <returns>The original, non processed name from the db</returns>
            string GetRawCaption();

            /// <summary>
            /// Method for moving the tab down
            /// </summary>
            void MoveDown();
        }


        /// <summary>
        /// A tab is merely a way to organize data on a ContentType to make it more
        /// human friendly
        /// </summary>
        protected class Tab : TabI
        {
            private ContentType _contenttype;
            private static object propertyTypesCacheSyncLock = new object();
            public static readonly string CacheKey = "Tab_PropertyTypes_Content:";

            /// <summary>
            /// 
            /// </summary>
            /// <param name="id"></param>
            /// <param name="caption"></param>
            /// <param name="cType"></param>
            public Tab(int id, string caption, ContentType cType)
            {
                _id = id;
                _caption = caption;
                _contenttype = cType;

                string cacheKey = CacheKey + _id;
                _propertytypes =
                    Cache.GetCacheItem<PropertyType[]>(cacheKey, propertyTypesCacheSyncLock, TimeSpan.FromMinutes(10),
                                                       delegate
                                                           {
                                                               List<PropertyType> tmp = new List<PropertyType>();

                                                               using (
                                                                   SqlDataReader dr =
                                                                       SqlHelper.ExecuteReader(_ConnString,
                                                                                               CommandType.Text,
                                                                                               string.Format(
                                                                                                   "Select id from cmsPropertyType where tabid = {0} order by sortOrder",
                                                                                                   _id)))
                                                               {
                                                                   while (dr.Read())
                                                                   {
                                                                       tmp.Add(
                                                                           PropertyType.GetPropertyType(
                                                                               int.Parse(dr["id"].ToString())));
                                                                   }
                                                               }
                                                               return tmp.ToArray();
                                                           });
            }

            public static void FlushCache(int Id)
            {
                Cache.ClearCacheItem(CacheKey + Id.ToString());
            }

            public void Delete()
            {
                SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text, "delete from cmsTab where id = @id",
                                          new SqlParameter("@id", Id));
            }

            public static string GetCaptionById(int id)
            {
                try
                {
                    string tempCaption =
                        SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
                                                "Select text from cmsTab where id = " + id.ToString()).ToString();
                    if (!tempCaption.StartsWith("#"))
                        return tempCaption;
                    else
                    {
                        Language lang =
                            Language.GetByCultureCode(Thread.CurrentThread.CurrentCulture.Name);
                        if (lang != null)
                            return
                                new Dictionary.DictionaryItem(tempCaption.Substring(1, tempCaption.Length - 1)).Value(
                                    lang.id);
                        else
                            return "[" + tempCaption + "]";
                    }
                }
                catch
                {
                    return null;
                }
            }

            private int _id;

            private int SortOrder
            {
                get
                {
                    return int.Parse(SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
                                                             "select sortOrder from cmsTab where id = " + _id).ToString());
                }
                set
                {
                    SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
                                              "update cmsTab set sortOrder = " + value + " where id =" + _id);
                }
            }

            /// <summary>
            /// Moves the Tab up
            /// </summary>
            public void MoveUp()
            {
                FixTabOrder();
                // If this tab is not the first we can switch places with the previous tab 
                // hence moving it up.
                if (SortOrder > 0)
                {
                    int newsortorder = SortOrder - 1;
                    // Find the tab to switch with
                    TabI[] Tabs = _contenttype.getVirtualTabs;
                    foreach (Tab t in Tabs)
                    {
                        if (t.SortOrder == newsortorder)
                            t.SortOrder = SortOrder;
                    }
                    SortOrder = newsortorder;
                }
            }

            /// <summary>
            /// Moves the Tab down
            /// </summary>
            public void MoveDown()
            {
                FixTabOrder();
                // If this tab is not the last tab we can switch places with the next tab 
                // hence moving it down.
                TabI[] Tabs = _contenttype.getVirtualTabs;
                if (SortOrder < Tabs.Length - 1)
                {
                    int newsortorder = SortOrder + 1;
                    // Find the tab to switch with
                    foreach (Tab t in Tabs)
                    {
                        if (t.SortOrder == newsortorder)
                            t.SortOrder = SortOrder;
                    }
                    SortOrder = newsortorder;
                }
            }

            public string GetRawCaption()
            {
                return _caption;
            }


            private void FixTabOrder()
            {
                TabI[] Tabs = _contenttype.getVirtualTabs;
                for (int i = 0; i < Tabs.Length; i++)
                {
                    Tab t = (Tab) Tabs[i];
                    t.SortOrder = i;
                }
            }


            /// <summary>
            /// Public identifier
            /// </summary>
            public int Id
            {
                get { return _id; }
            }

            private PropertyType[] _propertytypes;

            /// <summary>
            /// A list of PropertyTypes on the Tab
            /// </summary>
            public PropertyType[] PropertyTypes
            {
                get { return _propertytypes; }
            }

            private string _caption;

            /// <summary>
            ///  The text/label on the Tab
            /// </summary>
            public string Caption
            {
                get
                {
                    if (!_caption.StartsWith("#"))
                        return _caption;
                    else
                    {
                        Language lang =
                            Language.GetByCultureCode(Thread.CurrentThread.CurrentCulture.Name);
                        if (lang != null)
                        {
                            if (Dictionary.DictionaryItem.hasKey(_caption.Substring(1, _caption.Length - 1)))
                            {
                                Dictionary.DictionaryItem di =
                                    new Dictionary.DictionaryItem(_caption.Substring(1, _caption.Length - 1));
                                if (di != null)
                                    return di.Value(lang.id);
                            }
                        }

                        return "[" + _caption + "]";
                    }
                }
            }
        }


        /// <summary>
        /// Retrieve a PropertyType by it's alias
        /// </summary>
        /// <param name="alias">PropertyType alias</param>
        /// <returns>The PropertyType with the given Alias</returns>
        public PropertyType getPropertyType(string alias)
        {
			object o = SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
                "Select id from cmsPropertyType where contentTypeId=@contentTypeId And Alias=@alias",
				new SqlParameter("@contentTypeId", this.Id),
				new SqlParameter("@alias", alias));

			if(o == null)
				return null;

			int propertyTypeId;
			if (!int.TryParse(o.ToString(), out propertyTypeId))
				return null;

			return PropertyType.GetPropertyType(propertyTypeId);
        }

        /// <summary>
        /// Creates a new ContentType
        /// </summary>
        /// <param name="NodeId">The CMSNode Id of the ContentType</param>
        /// <param name="Alias">The Alias of the ContentType</param>
        /// <param name="IconUrl">The Iconurl of Contents of this ContentType</param>
        protected static void Create(int NodeId, string Alias, string IconUrl)
        {
            SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                      "Insert into cmsContentType (nodeId,alias,icon) values (" + NodeId + ",'" + Alias +
                                      "','" + IconUrl + "')");
        }

        /// <summary>
        /// Deletes the current ContentType
        /// </summary>
        protected new void delete()
        {
            // Remove from cache
            FlushFromCache(Id);

            // Delete all propertyTypes
            foreach (PropertyType pt in PropertyTypes) pt.delete();

            // delete all tabs
            foreach (Tab t in getVirtualTabs)
                t.Delete();

            // delete contenttype entrance
            SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text, "Delete from cmsContentType where NodeId = " + Id);

            // delete CMSNode entrance
            base.delete();
        }

        /// <summary>
        /// The list of ContentType Id's that defines which Content (by ContentType) can be created as child 
        /// to the Content of this ContentType
        /// </summary>
        public int[] AllowedChildContentTypeIDs
        {
            get
            {
                List<int> tmp = new List<int>();
                using (SqlDataReader dr = SqlHelper.ExecuteReader(_ConnString, CommandType.Text,
                                                                  "Select AllowedId from cmsContentTypeAllowedContentType where id=" +
                                                                  Id))
                {
                    while (dr.Read())
                    {
                        tmp.Add((int) dr["AllowedId"]);
                    }
                }
                return tmp.ToArray();
            }
            set
            {
                // 
                SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                          "delete from cmsContentTypeAllowedContentType where id=" + Id);
                foreach (int i in value)
                {
                    SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
                                              "insert into cmsContentTypeAllowedContentType (id,AllowedId) values (" +
                                              Id + "," + i + ")");
                }
            }
        }

        public static ContentType GetContentType(int id)
        {
            if (HttpRuntime.Cache[string.Format("UmbracoContentType{0}", id.ToString())] == null)
            {
                ContentType ct = new ContentType(id);
                HttpRuntime.Cache.Insert(string.Format("UmbracoContentType{0}", id.ToString()), ct);
            }
            return (ContentType) HttpRuntime.Cache[string.Format("UmbracoContentType{0}", id.ToString())];
        }


        protected void FlushFromCache(int Id)
        {
            if (HttpRuntime.Cache[string.Format("UmbracoContentType{0}", Id.ToString())] != null)
                HttpRuntime.Cache.Remove(string.Format("UmbracoContentType{0}", Id.ToString()));

            if (HttpRuntime.Cache[string.Format("ContentType_PropertyTypes_Content:{0}", Id.ToString())] != null)
                HttpRuntime.Cache.Remove(string.Format("ContentType_PropertyTypes_Content:{0}", Id.ToString()));
        }

        // This is needed, because the Tab class is protected and as such it's not possible for 
        // the PropertyType class to easily access the cache flusher
        public static void FlushTabCache(int TabId)
        {
            Tab.FlushCache(TabId);
        }

        protected void AnalyzeContentTypes(Guid ObjectType, bool ForceUpdate)
        {
            if (!_analyzedContentTypes.ContainsKey(ObjectType) || ForceUpdate)
            {
                using (SqlDataReader dr = SqlHelper.ExecuteReader(_ConnString, CommandType.Text,
                                                                  "select id from umbracoNode where nodeObjectType = @objectType",
                                                                  new SqlParameter("@objectType", ObjectType)))
                {
                    while (dr.Read())
                    {
                        ContentType ct = new ContentType(dr.GetInt32(0));
                        if (!_optimziedContentTypes.ContainsKey(ct.UniqueId))
                            _optimziedContentTypes.Add(ct.UniqueId, false);

                        _optimziedContentTypes[ct.UniqueId] = usesUmbracoDataOnly(ct);
                    }
                }
            }
        }

        protected bool IsOptimized()
        {
            return (bool) _optimziedContentTypes[UniqueId];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        private static bool usesUmbracoDataOnly(ContentType ct)
        {
            bool retVal = true;
            foreach (PropertyType pt in ct.PropertyTypes)
            {
                if (!DataTypeDefinition.IsDefaultData(pt.DataTypeDefinition.DataType.Data))
                {
                    retVal = false;
                    break;
                }
            }
            return retVal;
        }
    }
}
