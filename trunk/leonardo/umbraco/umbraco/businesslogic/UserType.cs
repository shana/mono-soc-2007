using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Caching;

using Umbraco.Cms.BusinessLogic;

using Microsoft.ApplicationBlocks.Data;

namespace Umbraco.BusinessLogic
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

		/// <summary>
		/// Gets or sets the alias.
		/// </summary>
		/// <value>The alias.</value>
		public string Alias
		{
			get { return _alias; }
			set { _alias = value; }
		}

		/// <summary>
		/// Gets the type of the user.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public static UserType GetUserType(int id)
		{
			return CacheHelper.GetCacheItem<UserType>(string.Format("UmbracoUserType{0}", id),
				getUserTypeSyncLock, CacheItemPriority.Normal, null, null, TimeSpan.FromMinutes(30),
				delegate { return new UserType(id); });
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UserType"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		public UserType(int id)
		{
			_id = id;
			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_connstring, CommandType.Text,
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

		/// <summary>
		/// Initializes a new instance of the <see cref="UserType"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="name">The name.</param>
		public UserType(int id, string name)
		{
			_id = id;
			_name = name;
		}


		/// <summary>
		/// Gets all.
		/// </summary>
		/// <value>All.</value>
		public static UserType[] All
		{
			get
			{
				return CacheHelper.GetCacheItem<UserType[]>("UmbracoUserTypeAll", getAllUserTypeSyncLock,
					CacheItemPriority.Normal, null, null, TimeSpan.FromMinutes(30),
					delegate
					{
						List<UserType> tmp = new List<UserType>();
						using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_connstring, CommandType.Text, "select id, UserTypeName from umbracoUserType"))
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

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		public int Id
		{
			get { return _id; }
		}

		/// <summary>
		/// Gets the default permissions.
		/// </summary>
		/// <value>The default permissions.</value>
		public string DefaultPermissions
		{
			get { return _defaultPermissions; }
		}
	}
}