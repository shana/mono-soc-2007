using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Caching;

using businesslogic;

using Microsoft.ApplicationBlocks.Data;

namespace umbraco.BusinessLogic
{
	/// <summary>
	/// hhh
	/// </summary>
	public class UserType
	{
		private static string _connstring = GlobalSettings.DbDSN;
		private int _id;
		private string _name;
		private string _defaultPermissions;

		private string _alias;

		private static object getUserTypeSyncLock = new object();
		private static object getAllUserTypeSyncLock = new object();

		public string Alias
		{
			get { return _alias; }
			set { _alias = value; }
		}

		public static UserType GetUserType(int id)
		{
			return CacheHelper.GetCacheItem<UserType>(string.Format("UmbracoUserType{0}", id),
				getUserTypeSyncLock, CacheItemPriority.Normal, null, null, TimeSpan.FromMinutes(30),
				delegate { return new UserType(id); });
		}

		public UserType(int id)
		{
			_id = id;
			using (SqlDataReader dr = SqlHelper.ExecuteReader(_connstring, CommandType.Text,
					"select UserTypeName, UserTypeDefaultPermissions, UserTypeAlias from umbracoUserType where id = @id",
					new SqlParameter("@id", id)))
			{
				if(dr.Read())
				{
					_name = dr["UserTypeName"].ToString();
					_defaultPermissions = dr["UserTypeDefaultPermissions"].ToString();
					_alias = dr["UserTypeAlias"].ToString();
				}
			}
		}

		public UserType(int id, string name)
		{
			_id = id;
			_name = name;
		}

		public static UserType[] getAll
		{
			get
			{
				return CacheHelper.GetCacheItem<UserType[]>("UmbracoUserTypeAll", getAllUserTypeSyncLock,
					CacheItemPriority.Normal, null, null, TimeSpan.FromMinutes(30),
					delegate
					{
						List<UserType> tmp = new List<UserType>();
						using (SqlDataReader dr =
							SqlHelper.ExecuteReader(_connstring, CommandType.Text, "select id, UserTypeName from umbracoUserType"))
						{
							while (dr.Read())
							{
								tmp.Add(new UserType(int.Parse(dr["id"].ToString()), dr["UserTypeName"].ToString()));
							}
						}
						return tmp.ToArray();
					});
			}
		}

		public string Name
		{
			get { return _name; }
		}

		public int Id
		{
			get { return _id; }
		}

		public string DefaultPermissions
		{
			get { return _defaultPermissions; }
		}
	}
}