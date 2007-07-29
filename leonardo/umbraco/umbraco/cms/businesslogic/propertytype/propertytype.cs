using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Web.UI;

using Microsoft.ApplicationBlocks.Data;

using Umbraco.Cms.BusinessLogic.cache;
using Umbraco.Cms.BusinessLogic.datatype;
using Umbraco.Cms.BusinessLogic.language;
using Umbraco.interfaces;

namespace Umbraco.Cms.BusinessLogic.propertytype
{
	/// <summary>
	/// Summary description for propertytype.
	/// </summary>
	public class PropertyType
	{
		#region Declarations

		private string _alias;
		private string _name;
		private int _id;
		private int _DataTypeId;
		private int _contenttypeid;
		private int _sortOrder;
		private bool _mandatory = false;
		private string _validationRegExp = "";
		private string _description = "";
		private int _tabId = 0;
		private static string _connstring = GlobalSettings.DbDSN;

		private static object propertyTypeCacheSyncLock = new object();
		private static readonly string UmbracoPropertyTypeCacheKey = "UmbracoPropertyTypeCache";

		#endregion

		#region Constructors

		public PropertyType(int id)
		{
			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_connstring, CommandType.Text,
					"Select mandatory, DataTypeId, tabId, ContentTypeId, sortOrder, alias, Name, validationRegExp, description from cmsPropertyType where id=@id",
					new SqlParameter("@id", id)))
			{
				if(!dr.Read())
					throw new ArgumentException("Propertytype with id: " + id + " doesnt exist!");
				_mandatory = bool.Parse(dr["mandatory"].ToString());
				_id = id;
				if(!dr.IsDBNull(dr.GetOrdinal("tabId")))
					_tabId = int.Parse(dr["tabId"].ToString());
				_sortOrder = int.Parse(dr["sortOrder"].ToString());
				_alias = dr["alias"].ToString();
				_name = dr["Name"].ToString();
				_validationRegExp = dr["validationRegExp"].ToString();
				_DataTypeId = int.Parse(dr["DataTypeId"].ToString());
				_contenttypeid = int.Parse(dr["contentTypeId"].ToString());
				_description = dr["description"].ToString();
			}
		}

		#endregion

		#region Properties

		public DataTypeDefinition DataTypeDefinition
		{
			get { return DataTypeDefinition.GetDataTypeDefinition(_DataTypeId); }
			set
			{
				_DataTypeId = value.Id;
				this.InvalidateCache();
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring, CommandType.Text,
					"Update cmsPropertyType set DataTypeId = " + value.Id + " where id=" + this.Id);
			}
		}

		public int Id
		{
			get { return _id; }
		}

		public int TabId
		{
			get { return _tabId; }
			set
			{
				_tabId = value;
				this.InvalidateCache();
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring, CommandType.Text, "Update cmsPropertyType set tabId = @tabId where id = @id",
					new SqlParameter("@tabId", value), new SqlParameter("@id", this.Id));
			}
		}

		public bool Mandatory
		{
			get { return _mandatory; }
			set
			{
				_mandatory = value;
				this.InvalidateCache();
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring, CommandType.Text,
					"Update cmsPropertyType set mandatory = @mandatory where id = @id", new SqlParameter("@mandatory", value),
					new SqlParameter("@id", this.Id));
			}
		}

		public string ValidationRegExp
		{
			get { return _validationRegExp; }
			set
			{
				_validationRegExp = value;
				this.InvalidateCache();
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring, CommandType.Text,
					"Update cmsPropertyType set validationRegExp = @validationRegExp where id = @id",
					new SqlParameter("@validationRegExp", value), new SqlParameter("@id", this.Id));
			}
		}

		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				this.InvalidateCache();
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring, CommandType.Text,
					"Update cmsPropertyType set description = @description where id = @id", new SqlParameter("@description", value),
					new SqlParameter("@id", this.Id));
			}
		}

		public int SortOrder
		{
			get { return _sortOrder; }
			set
			{
				_sortOrder = value;
				this.InvalidateCache();
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring, CommandType.Text,
					"Update cmsPropertyType set sortOrder = @sortOrder where id = @id", new SqlParameter("@sortOrder", value),
					new SqlParameter("@id", this.Id));
			}
		}

		public string Alias
		{
			get { return _alias; }
			set
			{
				_alias = value;
				this.InvalidateCache();
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring, CommandType.Text,
					"Update cmsPropertyType set alias = '" + value + "' where id=" + this.Id);
			}
		}

		public string Name
		{
			get
			{
				if(!_name.StartsWith("#"))
					return _name;
				else
				{
					Language lang = Language.GetByCultureCode(Thread.CurrentThread.CurrentCulture.Name);
					if(lang != null)
					{
						if(Dictionary.DictionaryItem.hasKey(_name.Substring(1, _name.Length - 1)))
						{
							Dictionary.DictionaryItem di = new Dictionary.DictionaryItem(_name.Substring(1, _name.Length - 1));
							return di.Value(lang.id);
						}
					}

					return "[" + _name + "]";
				}
			}
			set
			{
				_name = value;
				this.InvalidateCache();
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring, CommandType.Text,
					"Update cmsPropertyType set Name = '" + value + "' where id=" + this.Id);
			}
		}

		#endregion

		#region Methods

		public string GetRawName()
		{
			return _name;
		}

		public static PropertyType MakeNew(DataTypeDefinition dt, ContentType ct, string Name, string Alias)
		{
		    PropertyType pt;
			try
			{
				pt = new PropertyType(int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_connstring, CommandType.Text,
					"insert into cmsPropertyType (DataTypeId, ContentTypeId, alias, Name) values (" + dt.Id + "," + ct.Id + ",'" +
					Alias + "','" + Name + "') Select @@Identity").ToString()));
			}
			finally
			{
				// Clear cached items
				System.Web.Caching.Cache c = System.Web.HttpRuntime.Cache;
				if (c != null)
				{
					System.Collections.IDictionaryEnumerator cacheEnumerator = c.GetEnumerator();
					while (cacheEnumerator.MoveNext())
					{
						if (cacheEnumerator.Key is string && ((string)cacheEnumerator.Key).StartsWith(UmbracoPropertyTypeCacheKey))
						{
							Cache.ClearCacheItem((string)cacheEnumerator.Key);
						}
					}
				}
			}

		    return pt;
		}

		public static PropertyType[] GetAll()
		{
			List<PropertyType> result = new List<PropertyType>();
			using (SqlDataReader dr =
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select id, Name from cmsPropertyType order by Name"))
			{
				while(dr.Read())
				{
					PropertyType pt = GetPropertyType((int)dr["id"]);
					if(pt != null)
						result.Add(pt);
				}
				return result.ToArray();
			}
		}

		public void delete()
		{
            // flush cache
            FlushCache();

			// Delete all properties of propertytype
			foreach(Content c in Content.getContentOfContentType(new ContentType(_contenttypeid)))
			{
				c.getProperty(this).delete();
			}
			// Delete PropertyType ..
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring, CommandType.Text, "Delete from cmsPropertyType where id = " + this.Id);
			this.InvalidateCache();
		}

		public IDataType GetEditControl(object Value, bool IsPostBack)
		{
			IDataType dt = this.DataTypeDefinition.DataType;
			dt.DataEditor.Editor.ID = this.Alias;
			IData df = this.DataTypeDefinition.DataType.Data;
			((Control)dt.DataEditor.Editor).ID = this.Alias;

			if(!IsPostBack)
			{
				if(Value != null)
					dt.Data.Value = Value;
				else
					dt.Data.Value = "";
			}

			return dt;
		}

		/// <summary>
		/// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
		/// </summary>
		public virtual void Save()
		{
            FlushCache();
		}

        protected virtual void FlushCache()
        {
            // clear local cache
            cache.Cache.ClearCacheItem(GetCacheKey(Id));

            // clear cache in contentype
            Cache.ClearCacheItem("ContentType_PropertyTypes_Content:" + this._contenttypeid.ToString());

            // clear cache in tab
            ContentType.FlushTabCache(_tabId);
        }

		public static PropertyType GetPropertyType(int id)
		{
			return Cache.GetCacheItem<PropertyType>(GetCacheKey(id), propertyTypeCacheSyncLock,
				TimeSpan.FromMinutes(30),
				delegate
				{
					try
					{
						return new PropertyType(id);
					}
					catch
					{
						return null;
					}
				});
		}

		private void InvalidateCache()
		{
			Cache.ClearCacheItem(GetCacheKey(this.Id));
		}

		private static string GetCacheKey(int id)
		{
			return UmbracoPropertyTypeCacheKey + id;
		}

		#endregion
	}
}