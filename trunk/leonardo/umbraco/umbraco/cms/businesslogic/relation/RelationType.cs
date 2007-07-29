using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

using Microsoft.ApplicationBlocks.Data;

namespace Umbraco.Cms.BusinessLogic.relation
{
	/// <summary>
	/// Summary description for RelationType.
	/// </summary>
	public class RelationType
	{
		#region Declarations

		private int _id;
		private bool _dual;
		private string _name;
		//private Guid _parentObjectType;
		//private Guid _childObjectType;
		private string _alias;

		private static object relationTypeSyncLock = new object();

		#endregion

		#region Constructors

		public RelationType(int Id)
		{
			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text,
				"select id, dual, Name, alias from umbracoRelationType where id = @id", new SqlParameter("@id", Id)))
			{
				if(dr.Read())
				{
					this._id = int.Parse(dr["id"].ToString());
					this._dual = bool.Parse(dr["dual"].ToString());
					//this._parentObjectType = new Guid(dr["parentObjectType"].ToString());
					//this._childObjectType = new Guid(dr["childObjectType"].ToString());
					this._name = dr["Name"].ToString();
					this._alias = dr["alias"].ToString();
				}
			}
		}

		#endregion

		#region Properties

		public int Id
		{
			get { return _id; }
		}

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
					"update umbracoRelationType set Name = @Name where id = " + this.Id.ToString(), new SqlParameter("@Name", value));
				this.InvalidateCache();
			}
		}

		public string Alias
		{
			get { return _alias; }
			set
			{
				_alias = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
					"update umbracoRelationType set alias = @alias where id = " + this.Id.ToString(), new SqlParameter("@alias", value));
				this.InvalidateCache();
			}
		}

		public bool Dual
		{
			get { return _dual; }
			set
			{
				_dual = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
					"update umbracoRelationType set dual = @dual where id = " + this.Id.ToString(), new SqlParameter("@dual", value));
				this.InvalidateCache();
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
		/// </summary>
		public virtual void Save()
		{
		}

		public static RelationType GetById(int id)
		{
			try
			{
				return Umbraco.Cms.BusinessLogic.cache.Cache.GetCacheItem<RelationType>(
					GetCacheKey(id), relationTypeSyncLock, TimeSpan.FromMinutes(30),
					delegate { return new RelationType(id); });
			}
			catch
			{
				return null;
			}
		}

		public static RelationType GetByAlias(string Alias)
		{
			try
			{
				return GetById(int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
					"select id from umbracoRelationType where alias = @alias",
					new SqlParameter("@alias", Alias)).ToString()));
			}
			catch
			{
				return null;
			}
		}

		protected virtual void InvalidateCache()
		{
			Umbraco.Cms.BusinessLogic.cache.Cache.ClearCacheItem(GetCacheKey(this.Id));
		}

		private static string GetCacheKey(int id)
		{
			return string.Format("RelationTypeCacheItem_{0}", id);
		}

		#endregion
	}
}